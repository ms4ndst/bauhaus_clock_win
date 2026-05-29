using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BauhausScreensaver
{
    /// <summary>
    /// Renders a single Bauhaus-style clock face. All visual properties come from
    /// the <see cref="ClockFaceTheme"/> resolved at draw time — there are no
    /// per-variant branches in this file. Adding a variant means adding a catalog
    /// entry, not editing the renderer.
    /// </summary>
    public class BauhausClock
    {
        #region Fields

        private readonly BauhausConfig config;
        private float currentSecondAngle;

        #endregion

        #region Constructor

        public BauhausClock(BauhausConfig configuration)
        {
            config = configuration;
            currentSecondAngle = DateTime.Now.Second * 6f;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Renders the clock to the provided graphics context.
        /// </summary>
        public void Render(Graphics g, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            DateTime now = DateTime.Now;
            ClockFaceTheme theme = ResolveTheme(now);

            int size = Math.Min(bounds.Width, bounds.Height);
            if (config.Size == BauhausConfig.ClockSize.Compact)
            {
                size = (int)(size * 0.7f);
            }

            float centerX = bounds.Width / 2f;
            float centerY = bounds.Height / 2f;
            float radius = size / 2f * 0.8f;

            using (Brush bgBrush = new SolidBrush(theme.Background))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            DrawTickMarks(g, centerX, centerY, radius, theme);
            DrawNumbers(g, centerX, centerY, radius, theme);

            UpdateSecondHand(now);
            DrawHands(g, centerX, centerY, radius, now, theme);
            DrawCenterHub(g, centerX, centerY, radius, theme);
        }

        #endregion

        #region Theme Resolution

        /// <summary>
        /// Picks the catalog theme for the current dial, then applies the night-mode
        /// lume override if the appearance setting calls for it.
        /// </summary>
        private ClockFaceTheme ResolveTheme(DateTime now)
        {
            ClockFaceTheme baseTheme = ClockFaceCatalog.For(config.DialTheme);
            return IsNightMode(now) ? baseTheme.AsNight(config.LumeColor) : baseTheme;
        }

        private bool IsNightMode(DateTime now)
        {
            switch (config.Appearance)
            {
                case BauhausConfig.AppearanceMode.Night:
                    return true;
                case BauhausConfig.AppearanceMode.System:
                    return now.Hour >= 20 || now.Hour < 6;
                default:
                    return false;
            }
        }

        #endregion

        #region Drawing

        private void DrawTickMarks(Graphics g, float centerX, float centerY, float radius, ClockFaceTheme theme)
        {
            for (int i = 0; i < 60; i++)
            {
                double angle = (i * 6 - 90) * Math.PI / 180.0;
                bool isHourMark = (i % 5) == 0;

                if (isHourMark)
                {
                    DrawFiveMinuteMarker(g, centerX, centerY, radius, angle, theme);
                }
                else
                {
                    float tickLength = radius * 0.04f;
                    float tickWidth = Math.Max(2f, radius * 0.005f);
                    float innerRadius = radius;
                    float outerRadius = innerRadius - tickLength;

                    float x1 = centerX + (float)(Math.Cos(angle) * innerRadius);
                    float y1 = centerY + (float)(Math.Sin(angle) * innerRadius);
                    float x2 = centerX + (float)(Math.Cos(angle) * outerRadius);
                    float y2 = centerY + (float)(Math.Sin(angle) * outerRadius);

                    using (Pen tickPen = new Pen(theme.TickColor, tickWidth))
                    {
                        tickPen.StartCap = LineCap.Round;
                        tickPen.EndCap = LineCap.Round;
                        g.DrawLine(tickPen, x1, y1, x2, y2);
                    }
                }
            }
        }

        private void DrawFiveMinuteMarker(Graphics g, float centerX, float centerY, float radius, double angle, ClockFaceTheme theme)
        {
            // Chunkier markers — references show pills as the dominant outer-ring element.
            float markerLength = radius * 0.13f;
            float markerWidth = radius * 0.045f;
            
            // Prevent crash when clock is rendered too small for valid arc geometry
            if (markerWidth < 1f || markerLength < 1f)
                return;
            
            // Pills sit just inside the tick ring (~0.88 of radius), between hour numerals
            // (~0.65) and minute numerals (~1.05) — matches the reference dials.
            float markerRadius = radius * 0.88f;

            float markerCenterX = centerX + (float)(Math.Cos(angle) * markerRadius);
            float markerCenterY = centerY + (float)(Math.Sin(angle) * markerRadius);

            Matrix oldTransform = g.Transform.Clone();
            g.TranslateTransform(markerCenterX, markerCenterY);
            g.RotateTransform((float)(angle * 180.0 / Math.PI));

            using (GraphicsPath path = new GraphicsPath())
            {
                RectangleF rect = new RectangleF(-markerLength / 2, -markerWidth / 2, markerLength, markerWidth);
                path.AddArc(rect.X, rect.Y, markerWidth, markerWidth, 90, 180);
                path.AddArc(rect.Right - markerWidth, rect.Y, markerWidth, markerWidth, 270, 180);
                path.CloseFigure();

                if (!theme.IsDark)
                {
                    using (Brush shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                    {
                        g.TranslateTransform(0.5f, 1f);
                        g.FillPath(shadowBrush, path);
                        g.TranslateTransform(-0.5f, -1f);
                    }
                }

                using (Brush markerBrush = new SolidBrush(theme.PillColor))
                {
                    g.FillPath(markerBrush, path);
                }
            }

            g.Transform = oldTransform;
        }

        private void DrawNumbers(Graphics g, float centerX, float centerY, float radius, ClockFaceTheme theme)
        {
            // StringFormat-centered draw avoids the MeasureString padding quirk
            // that nudges multi-digit numerals like "12" off-axis.
            using (Font hourFont = new Font("Arial", radius * 0.20f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Font minuteFont = new Font("Arial", radius * 0.085f, FontStyle.Regular, GraphicsUnit.Pixel))
            using (Brush hourBrush = new SolidBrush(theme.HourNumeralColor))
            using (Brush minuteBrush = new SolidBrush(theme.MinuteNumeralColor))
            using (StringFormat centered = new StringFormat(StringFormat.GenericTypographic)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                for (int i = 1; i <= 12; i++)
                {
                    double angle = (i * 30 - 90) * Math.PI / 180.0;

                    float hourRadius = radius * 0.65f;
                    float hx = centerX + (float)(Math.Cos(angle) * hourRadius);
                    float hy = centerY + (float)(Math.Sin(angle) * hourRadius);
                    g.DrawString(i.ToString(), hourFont, hourBrush, hx, hy, centered);

                    float minuteRadius = radius * 1.04f;
                    float mx = centerX + (float)(Math.Cos(angle) * minuteRadius);
                    float my = centerY + (float)(Math.Sin(angle) * minuteRadius);
                    g.DrawString((i * 5).ToString("00"), minuteFont, minuteBrush, mx, my, centered);
                }
            }
        }

        private void UpdateSecondHand(DateTime now)
        {
            switch (config.Movement)
            {
                case BauhausConfig.MovementType.Quartz:
                    currentSecondAngle = now.Second * 6f;
                    break;

                case BauhausConfig.MovementType.Mechanical:
                    float beatsPerSecond = 6f;
                    float totalBeats = now.Second * beatsPerSecond + (now.Millisecond / 1000f) * beatsPerSecond;
                    float beatPosition = (float)Math.Floor(totalBeats) / beatsPerSecond;
                    currentSecondAngle = beatPosition * 6f;
                    break;

                case BauhausConfig.MovementType.Digital:
                    currentSecondAngle = (now.Second + now.Millisecond / 1000f) * 6f;
                    break;
            }
        }

        private void DrawHands(Graphics g, float centerX, float centerY, float radius, DateTime now, ClockFaceTheme theme)
        {
            float hourAngle = ((now.Hour % 12) + now.Minute / 60f) * 30f - 90f;
            float minuteAngle = (now.Minute + now.Second / 60f) * 6f - 90f;
            float secondAngle = currentSecondAngle - 90f;

            // Hour hand reaches just inside the hour-numeral ring (~0.65),
            // minute hand reaches just inside the pill-marker ring (~0.88) — close in length,
            // with the minute hand only ~25% longer than the hour hand, matching the references.
            DrawBatonHand(g, centerX, centerY, hourAngle, radius * 0.62f, radius * 0.040f, theme);
            DrawBatonHand(g, centerX, centerY, minuteAngle, radius * 0.98f, radius * 0.032f, theme);

            if (config.Movement != BauhausConfig.MovementType.Digital)
            {
                DrawSecondHand(g, centerX, centerY, secondAngle, radius * 0.75f, theme);
            }
        }

        private void DrawBatonHand(Graphics g, float centerX, float centerY, float angleDegrees,
            float length, float width, ClockFaceTheme theme)
        {
            // Capsule-shaped hand: uniform width body with rounded caps at both ends.
            // The base extends slightly behind the centre so the hand visually emerges
            // from under the hub instead of dangling off it.
            float angleRad = angleDegrees * (float)Math.PI / 180f;
            float baseOverhang = width * 0.4f;

            float baseX = centerX - (float)(Math.Cos(angleRad) * baseOverhang);
            float baseY = centerY - (float)(Math.Sin(angleRad) * baseOverhang);
            float tipX = centerX + (float)(Math.Cos(angleRad) * length);
            float tipY = centerY + (float)(Math.Sin(angleRad) * length);

            DrawDropShadowLine(g, baseX, baseY, tipX, tipY, width, theme);

            using (Pen handPen = new Pen(theme.HandColor, width))
            {
                handPen.StartCap = LineCap.Round;
                handPen.EndCap = LineCap.Round;
                g.DrawLine(handPen, baseX, baseY, tipX, tipY);
            }
        }

        // Drop-shadow falloff profile, ordered outer-to-inner. Each pair = (widthMultiplier, alpha).
        // Four passes of progressively-smaller-and-darker capsules under the hand simulate a
        // Gaussian blur — the eye reads it as a soft elevated shadow.
        private static readonly float[] ShadowWidthMultipliers = { 2.20f, 1.75f, 1.40f, 1.10f };
        private static readonly int[]   ShadowAlphasLight      = { 12,    22,    38,    65 };
        private static readonly int[]   ShadowAlphasDark       = { 7,     13,    22,    40 };

        /// <summary>
        /// Draws a soft, elevated drop shadow under a hand. Four blur passes (wide+faint outer
        /// halo through narrow+darker core) give the illusion of distance between the hand and
        /// the dial. The directional offset is small — the shadow sits mostly underneath the
        /// hand, slightly nudged toward the lower-right to suggest a light source above.
        /// </summary>
        private void DrawDropShadowLine(Graphics g, float x1, float y1, float x2, float y2,
            float width, ClockFaceTheme theme)
        {
            // Small directional offset — keeps shadow under the hand rather than next to it.
            float dx = Math.Max(1.0f, width * 0.12f);
            float dy = Math.Max(2.0f, width * 0.30f);

            int[] alphas = theme.IsDark ? ShadowAlphasDark : ShadowAlphasLight;

            for (int i = 0; i < ShadowWidthMultipliers.Length; i++)
            {
                float w = width * ShadowWidthMultipliers[i];
                using (Pen pen = new Pen(Color.FromArgb(alphas[i], 0, 0, 0), w))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, x1 + dx, y1 + dy, x2 + dx, y2 + dy);
                }
            }
        }

        private void DrawSecondHand(Graphics g, float centerX, float centerY, float angleDegrees, float length, ClockFaceTheme theme)
        {
            float angleRad = angleDegrees * (float)Math.PI / 180f;
            float x2 = centerX + (float)(Math.Cos(angleRad) * length);
            float y2 = centerY + (float)(Math.Sin(angleRad) * length);

            // Second hand is much thinner than the batons, so its shadow is correspondingly subtler.
            float secondWidth = Math.Max(1.2f, length * 0.012f);
            DrawDropShadowLine(g, centerX, centerY, x2, y2, secondWidth, theme);

            using (Pen pen = new Pen(theme.HandColor, secondWidth))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, centerX, centerY, x2, y2);
            }
        }

        private void DrawCenterHub(Graphics g, float centerX, float centerY, float radius, ClockFaceTheme theme)
        {
            float hubSize = radius * 0.065f;

            if (!theme.IsDark)
            {
                using (Brush shadowBrush = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
                {
                    g.FillEllipse(shadowBrush, centerX - hubSize / 2 + 0.5f, centerY - hubSize / 2 + 0.5f, hubSize, hubSize);
                }
            }

            using (Brush hubBrush = new SolidBrush(theme.CentreHubColor))
            {
                g.FillEllipse(hubBrush, centerX - hubSize / 2, centerY - hubSize / 2, hubSize, hubSize);
            }

            float highlightSize = hubSize * 0.4f;
            using (Brush highlightBrush = new SolidBrush(Color.FromArgb(80, 255, 255, 255)))
            {
                g.FillEllipse(highlightBrush,
                    centerX - highlightSize / 2 - hubSize * 0.12f,
                    centerY - highlightSize / 2 - hubSize * 0.12f,
                    highlightSize, highlightSize);
            }
        }

        #endregion
    }
}
