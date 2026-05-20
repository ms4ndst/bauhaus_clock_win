using System.Collections.Generic;
using System.Drawing;

namespace BauhausScreensaver
{
    /// <summary>
    /// Hand silhouette options. Extension point for future variants that need
    /// a non-baton hand (sword, dauphine, etc.). Today every variant is Baton.
    /// </summary>
    public enum HandShape
    {
        Baton
    }

    /// <summary>
    /// 5-minute marker silhouette options. Extension point for future variants
    /// that need a non-pill marker (rectangle, dot, numeral, etc.).
    /// </summary>
    public enum PillStyle
    {
        Rounded
    }

    /// <summary>
    /// Pure-data description of a single clock-face variant. The renderer reads
    /// every visual property from here; there are no per-variant branches in the
    /// drawing code. Adding a new variant means appending one entry to
    /// <see cref="ClockFaceCatalog.All"/>, not editing the renderer.
    /// </summary>
    public sealed class ClockFaceTheme
    {
        /// <summary>Stable internal id used for persistence (matches the enum slug).</summary>
        public string Id { get; }

        /// <summary>Display label shown in the settings UI, verbatim from the screenshots.</summary>
        public string DisplayName { get; }

        public Color Background { get; }
        public Color PillColor { get; }
        public Color TickColor { get; }
        public Color HourNumeralColor { get; }
        public Color MinuteNumeralColor { get; }
        public Color HandColor { get; }
        public Color CentreHubColor { get; }

        public HandShape Hand { get; }
        public PillStyle Pill { get; }

        /// <summary>Hints the renderer to skip the subtle drop-shadow that doesn't read on dark dials.</summary>
        public bool IsDark { get; }

        public ClockFaceTheme(
            string id, string displayName,
            Color background, Color pillColor, Color tickColor,
            Color hourNumeralColor, Color minuteNumeralColor,
            Color handColor, Color centreHubColor,
            bool isDark,
            HandShape hand = HandShape.Baton, PillStyle pill = PillStyle.Rounded)
        {
            Id = id;
            DisplayName = displayName;
            Background = background;
            PillColor = pillColor;
            TickColor = tickColor;
            HourNumeralColor = hourNumeralColor;
            MinuteNumeralColor = minuteNumeralColor;
            HandColor = handColor;
            CentreHubColor = centreHubColor;
            Hand = hand;
            Pill = pill;
            IsDark = isDark;
        }

        /// <summary>
        /// Produces a runtime variant with every visible element swapped for a single
        /// lume colour over a near-black background. Used when night appearance kicks in.
        /// </summary>
        public ClockFaceTheme AsNight(Color lume)
        {
            return new ClockFaceTheme(
                Id, DisplayName,
                background: Color.FromArgb(10, 10, 10),
                pillColor: lume,
                tickColor: lume,
                hourNumeralColor: lume,
                minuteNumeralColor: lume,
                handColor: lume,
                centreHubColor: lume,
                isDark: true,
                hand: Hand, pill: Pill);
        }
    }

    /// <summary>
    /// Built-in clock-face variants. Order matches <see cref="BauhausConfig.ClockDialTheme"/>
    /// exactly so a theme can be looked up by enum index. Display names are reproduced
    /// verbatim from the settings-UI screenshots.
    /// </summary>
    public static class ClockFaceCatalog
    {
        public static readonly IReadOnlyList<ClockFaceTheme> All = new[]
        {
            new ClockFaceTheme("White", "White",
                background: Color.FromArgb(245, 245, 245),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(120, 120, 120),
                hourNumeralColor: Color.FromArgb(44, 44, 44),
                minuteNumeralColor: Color.FromArgb(44, 44, 44),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(200, 200, 200),
                isDark: false),

            new ClockFaceTheme("Turquoise", "Turquoise",
                background: Color.FromArgb(168, 230, 230),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(60, 120, 120),
                hourNumeralColor: Color.FromArgb(14, 70, 84),
                minuteNumeralColor: Color.FromArgb(14, 70, 84),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: false),

            new ClockFaceTheme("Glacier", "Glacier",
                background: Color.FromArgb(200, 210, 230),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(90, 100, 140),
                hourNumeralColor: Color.FromArgb(30, 40, 80),
                minuteNumeralColor: Color.FromArgb(30, 40, 80),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: false),

            new ClockFaceTheme("Ocean", "Ocean",
                background: Color.FromArgb(30, 60, 100),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(180, 200, 230),
                hourNumeralColor: Color.FromArgb(255, 255, 255),
                minuteNumeralColor: Color.FromArgb(180, 200, 230),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: true),

            new ClockFaceTheme("Tennis", "Tennis",
                background: Color.FromArgb(50, 150, 80),
                pillColor: Color.FromArgb(240, 230, 200),
                tickColor: Color.FromArgb(165, 200, 150),
                hourNumeralColor: Color.FromArgb(230, 220, 0),
                minuteNumeralColor: Color.FromArgb(100, 200, 180),
                handColor: Color.FromArgb(240, 230, 200),
                centreHubColor: Color.FromArgb(240, 230, 200),
                isDark: true),

            new ClockFaceTheme("SignalBlue", "Signal Blue",
                background: Color.FromArgb(42, 161, 225),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(225, 240, 248),
                hourNumeralColor: Color.FromArgb(255, 255, 255),
                minuteNumeralColor: Color.FromArgb(255, 255, 255),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: true),

            new ClockFaceTheme("SkyBlue", "Sky Blue",
                background: Color.FromArgb(196, 220, 232),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(80, 100, 120),
                hourNumeralColor: Color.FromArgb(30, 40, 64),
                minuteNumeralColor: Color.FromArgb(30, 40, 64),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: false),

            new ClockFaceTheme("Beige", "Beige",
                background: Color.FromArgb(212, 196, 168),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(122, 106, 80),
                hourNumeralColor: Color.FromArgb(58, 44, 20),
                minuteNumeralColor: Color.FromArgb(58, 44, 20),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: false),

            new ClockFaceTheme("Cream", "Cream",
                background: Color.FromArgb(240, 232, 216),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(154, 138, 106),
                hourNumeralColor: Color.FromArgb(60, 52, 36),
                minuteNumeralColor: Color.FromArgb(60, 52, 36),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(216, 200, 168),
                isDark: false),

            new ClockFaceTheme("Lavender", "Lavender",
                background: Color.FromArgb(212, 200, 224),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(112, 96, 160),
                hourNumeralColor: Color.FromArgb(42, 30, 80),
                minuteNumeralColor: Color.FromArgb(42, 30, 80),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: false),

            new ClockFaceTheme("Rose", "Rose",
                background: Color.FromArgb(232, 180, 200),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(160, 96, 120),
                hourNumeralColor: Color.FromArgb(90, 30, 60),
                minuteNumeralColor: Color.FromArgb(90, 30, 60),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: false),

            new ClockFaceTheme("Salmon", "Salmon",
                background: Color.FromArgb(232, 165, 132),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(160, 100, 80),
                hourNumeralColor: Color.FromArgb(90, 40, 20),
                minuteNumeralColor: Color.FromArgb(90, 40, 20),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: false),

            new ClockFaceTheme("Yellow", "Yellow",
                background: Color.FromArgb(245, 184, 0),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(138, 104, 0),
                hourNumeralColor: Color.FromArgb(30, 20, 8),
                minuteNumeralColor: Color.FromArgb(30, 20, 8),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: false),

            new ClockFaceTheme("Pistachio", "Pistachio",
                background: Color.FromArgb(196, 212, 166),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(122, 140, 90),
                hourNumeralColor: Color.FromArgb(40, 52, 24),
                minuteNumeralColor: Color.FromArgb(40, 52, 24),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: false),

            new ClockFaceTheme("Slate", "Slate",
                background: Color.FromArgb(84, 84, 84),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(160, 160, 160),
                hourNumeralColor: Color.FromArgb(255, 255, 255),
                minuteNumeralColor: Color.FromArgb(208, 208, 208),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: true),

            new ClockFaceTheme("Noir", "Noir",
                background: Color.FromArgb(43, 43, 43),
                pillColor: Color.FromArgb(255, 255, 255),
                tickColor: Color.FromArgb(144, 144, 144),
                hourNumeralColor: Color.FromArgb(255, 255, 255),
                minuteNumeralColor: Color.FromArgb(160, 160, 160),
                handColor: Color.FromArgb(255, 255, 255),
                centreHubColor: Color.FromArgb(255, 255, 255),
                isDark: true)
        };

        /// <summary>
        /// Look up the theme for a given dial-theme enum value.
        /// </summary>
        public static ClockFaceTheme For(BauhausConfig.ClockDialTheme dial)
        {
            int index = (int)dial;
            if (index < 0 || index >= All.Count)
            {
                return All[0];
            }
            return All[index];
        }
    }
}
