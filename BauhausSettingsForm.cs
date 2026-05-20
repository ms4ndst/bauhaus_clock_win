using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BauhausScreensaver
{
    /// <summary>
    /// Settings dialog for configuring the Bauhaus Clock screensaver.
    /// Modern design matching bauhausclock.com with appearance, dial theme, size, movement, and lume color options.
    /// </summary>
    public partial class BauhausSettingsForm : Form
    {
        #region Fields

        private BauhausConfig config;
        
        // Radio button groups
        private RadioButton[] appearanceButtons;
        private RadioButton[] dialThemeButtons;
        private RadioButton[] sizeButtons;
        private RadioButton[] movementButtons;

        // Dynamic descriptor next to the Movement section title (e.g. "· Mechanical sweep").
        private Label lblMovementSubtitle;

        // Per-movement descriptors shown after the section title — order matches MovementType enum.
        private static readonly string[] MovementSubtitles = { "· Quartz tick", "· Mechanical sweep", "· Smooth sweep" };

        // Watch Dial pagination — 5 swatches per page with < > navigation in the section header.
        private int currentDialPage;
        private Button btnDialPrev, btnDialNext;
        private const int DialsPerPage = 5;
        private const int DialButtonWidth = 100;
        private const int DialButtonHeight = 115;
        private const int DialButtonGap = 20;

        // Form layout constants (single source of truth for the portrait layout).
        private const int FormWidth = 610;
        private const int FormHeight = 700;
        private const int LeftPad = 10;
        private const int RightPad = 10;
        private const int ContentWidth = FormWidth - LeftPad - RightPad; // 590
        
        // Lume color buttons
        private Button[] lumeColorButtons;
        private Button selectedLumeButton;
        
        // Action buttons
        private Button btnOK;
        private Button btnCancel;
        
        // Lume color palette (14 colors matching bauhausclock.com)
        private readonly Color[] lumeColors = new Color[]
        {
            Color.FromArgb(255, 59, 48),    // Red
            Color.FromArgb(255, 149, 0),    // Orange
            Color.FromArgb(191, 90, 242),   // Purple
            Color.FromArgb(255, 175, 204),  // Pink
            Color.FromArgb(90, 200, 250),   // Light Cyan
            Color.FromArgb(100, 210, 255),  // Cyan
            Color.FromArgb(52, 199, 89),    // Green (Swiss BGW9)
            Color.FromArgb(255, 214, 179),  // Beige
            Color.FromArgb(48, 209, 88),    // Bright Green
            Color.FromArgb(64, 200, 224),   // Teal
            Color.FromArgb(255, 214, 10),   // Yellow
            Color.FromArgb(50, 215, 75),    // Light Green
            Color.FromArgb(90, 200, 245),   // Sky Blue
            Color.FromArgb(255, 255, 255)   // White
        };

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the BauhausSettingsForm class.
        /// </summary>
        public BauhausSettingsForm()
        {
            InitializeComponent();
            config = BauhausConfig.Load();
            LoadSettings();
            UpdateMovementSubtitle();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the form components with modern bauhausclock.com design.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.ClientSize = new Size(FormWidth, FormHeight);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Bauhaus Clock Settings";
            this.BackColor = Color.FromArgb(235, 235, 235);
            this.Padding = new Padding(30, 30, 30, 30);

            int yPos = 20;

            // Appearance Section
            yPos = CreateAppearanceSection(yPos);
            
            // Clock Dial Section
            yPos = CreateClockDialSection(yPos);
            
            // Size and Movement Section (side by side)
            yPos = CreateSizeAndMovementSection(yPos);
            
            // Lume Color Section
            yPos = CreateLumeColorSection(yPos);
            
            // Action Buttons
            CreateActionButtons(yPos);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Creates the Appearance section with Day/Night/System radio buttons.
        /// </summary>
        private int CreateAppearanceSection(int yPos)
        {
            Label lblAppearance = new Label();
            lblAppearance.Text = "Appearance";
            lblAppearance.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAppearance.ForeColor = Color.FromArgb(60, 60, 60);
            lblAppearance.Location = new Point(LeftPad, yPos);
            lblAppearance.AutoSize = true;
            this.Controls.Add(lblAppearance);
            yPos += 30;

            Panel appearancePanel = new Panel();
            appearancePanel.Location = new Point(LeftPad, yPos);
            appearancePanel.Size = new Size(ContentWidth, 115);
            appearancePanel.BackColor = Color.Transparent;
            this.Controls.Add(appearancePanel);

            appearanceButtons = new RadioButton[3];
            string[] appearanceLabels = { "Day", "Night", "System" };

            // 3 buttons of 180 wide with 25 px gaps fill the 590-wide content area.
            const int btnW = 180, btnGap = 25;
            for (int i = 0; i < 3; i++)
            {
                appearanceButtons[i] = CreateAppearanceButton(appearanceLabels[i], i);
                appearanceButtons[i].Size = new Size(btnW, 115);
                appearanceButtons[i].Location = new Point(i * (btnW + btnGap), 0);
                appearancePanel.Controls.Add(appearanceButtons[i]);
            }

            return yPos + 115 + 20;
        }

        /// <summary>
        /// Creates the Clock Dial section with theme radio buttons.
        /// </summary>
        private int CreateClockDialSection(int yPos)
        {
            Label lblWatchDial = new Label();
            lblWatchDial.Text = "Watch Dial";
            lblWatchDial.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblWatchDial.ForeColor = Color.FromArgb(60, 60, 60);
            lblWatchDial.Location = new Point(LeftPad, yPos);
            lblWatchDial.AutoSize = true;
            this.Controls.Add(lblWatchDial);

            // Pagination arrows aligned to the right edge of the content area.
            // FormWidth=650, RightPad=30, so right edge at x=620. Pagers are 30 wide.
            btnDialNext = CreatePagerButton("›");
            btnDialNext.Location = new Point(FormWidth - RightPad - 30, yPos - 5);
            btnDialNext.Click += (s, e) => ShowDialPage(currentDialPage + 1);
            this.Controls.Add(btnDialNext);

            btnDialPrev = CreatePagerButton("‹");
            btnDialPrev.Location = new Point(FormWidth - RightPad - 65, yPos - 5);
            btnDialPrev.Click += (s, e) => ShowDialPage(currentDialPage - 1);
            this.Controls.Add(btnDialPrev);

            yPos += 30;

            Panel dialPanel = new Panel();
            dialPanel.Location = new Point(LeftPad, yPos);
            dialPanel.Size = new Size(ContentWidth, DialButtonHeight);
            dialPanel.BackColor = Color.Transparent;
            this.Controls.Add(dialPanel);

            int themeCount = ClockFaceCatalog.All.Count;
            dialThemeButtons = new RadioButton[themeCount];

            for (int i = 0; i < themeCount; i++)
            {
                ClockFaceTheme theme = ClockFaceCatalog.All[i];
                dialThemeButtons[i] = CreateDialThemeButton(theme.DisplayName, theme.Background);
                dialThemeButtons[i].Size = new Size(DialButtonWidth, DialButtonHeight);
                dialPanel.Controls.Add(dialThemeButtons[i]);
            }

            return yPos + DialButtonHeight + 20;
        }

        /// <summary>
        /// Creates a small unobtrusive pager button (used for the &lt; and &gt; arrows next to "Watch Dial").
        /// </summary>
        private Button CreatePagerButton(string glyph)
        {
            Button btn = new Button();
            btn.Text = glyph;
            btn.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
            btn.ForeColor = Color.FromArgb(80, 80, 80);
            btn.Size = new Size(30, 30);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.Transparent;
            btn.Cursor = Cursors.Hand;
            btn.TabStop = false;
            return btn;
        }

        /// <summary>
        /// Shows the requested page of dial swatches and hides the others. Updates the
        /// enabled state of the pager arrows to reflect first/last page boundaries.
        /// </summary>
        private void ShowDialPage(int page)
        {
            int totalPages = (dialThemeButtons.Length + DialsPerPage - 1) / DialsPerPage;
            currentDialPage = Math.Max(0, Math.Min(page, totalPages - 1));

            int rowWidth = DialsPerPage * DialButtonWidth + (DialsPerPage - 1) * DialButtonGap;
            int xStart = (ContentWidth - rowWidth) / 2;

            for (int i = 0; i < dialThemeButtons.Length; i++)
            {
                int btnPage = i / DialsPerPage;
                bool onCurrentPage = (btnPage == currentDialPage);
                dialThemeButtons[i].Visible = onCurrentPage;
                if (onCurrentPage)
                {
                    int posInPage = i % DialsPerPage;
                    dialThemeButtons[i].Location = new Point(xStart + posInPage * (DialButtonWidth + DialButtonGap), 0);
                }
            }

            btnDialPrev.Enabled = currentDialPage > 0;
            btnDialNext.Enabled = currentDialPage < totalPages - 1;
            btnDialPrev.ForeColor = btnDialPrev.Enabled ? Color.FromArgb(80, 80, 80) : Color.FromArgb(200, 200, 200);
            btnDialNext.ForeColor = btnDialNext.Enabled ? Color.FromArgb(80, 80, 80) : Color.FromArgb(200, 200, 200);
        }

        /// <summary>
        /// Creates the Size and Movement sections side by side.
        /// </summary>
        private int CreateSizeAndMovementSection(int yPos)
        {
            // Layout: Size on the left (2 buttons), Movement on the right (3 buttons).
            // Both rows use 95×95 square icon cards.
            const int btnSize = 95, btnGap = 15;
            int sizeRowW = 2 * btnSize + 1 * btnGap;       // 205
            int movementRowW = 3 * btnSize + 2 * btnGap;   // 315
            int sizeX = LeftPad;                            // 30
            int movementX = FormWidth - RightPad - movementRowW; // 305

            Label lblSize = new Label();
            lblSize.Text = "Size";
            lblSize.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSize.ForeColor = Color.FromArgb(60, 60, 60);
            lblSize.Location = new Point(sizeX, yPos);
            lblSize.AutoSize = true;
            this.Controls.Add(lblSize);

            Label lblMovement = new Label();
            lblMovement.Text = "Movement";
            lblMovement.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblMovement.ForeColor = Color.FromArgb(60, 60, 60);
            lblMovement.Location = new Point(movementX, yPos);
            lblMovement.AutoSize = true;
            this.Controls.Add(lblMovement);

            // Subtitle e.g. "· Quartz tick". Sits to the right of the Movement label.
            lblMovementSubtitle = new Label();
            lblMovementSubtitle.Text = "";
            lblMovementSubtitle.Font = new Font("Segoe UI", 9F);
            lblMovementSubtitle.ForeColor = Color.FromArgb(140, 140, 140);
            lblMovementSubtitle.Location = new Point(movementX + 80, yPos + 2);
            lblMovementSubtitle.AutoSize = true;
            this.Controls.Add(lblMovementSubtitle);

            yPos += 30;

            Panel sizePanel = new Panel();
            sizePanel.Location = new Point(sizeX, yPos);
            sizePanel.Size = new Size(sizeRowW, btnSize);
            sizePanel.BackColor = Color.Transparent;
            this.Controls.Add(sizePanel);

            sizeButtons = new RadioButton[2];
            string[] sizeLabels = { "Classic", "Compact" };
            for (int i = 0; i < 2; i++)
            {
                sizeButtons[i] = CreateSizeButton(sizeLabels[i]);
                sizeButtons[i].Size = new Size(btnSize, btnSize);
                sizeButtons[i].Location = new Point(i * (btnSize + btnGap), 0);
                sizePanel.Controls.Add(sizeButtons[i]);
            }

            Panel movementPanel = new Panel();
            movementPanel.Location = new Point(movementX, yPos);
            movementPanel.Size = new Size(movementRowW, btnSize);
            movementPanel.BackColor = Color.Transparent;
            this.Controls.Add(movementPanel);

            movementButtons = new RadioButton[3];
            string[] movementLabels = { "Quartz", "Mechanical", "Digital" };
            for (int i = 0; i < 3; i++)
            {
                movementButtons[i] = CreateMovementButton(movementLabels[i]);
                movementButtons[i].Size = new Size(btnSize, btnSize);
                movementButtons[i].Location = new Point(i * (btnSize + btnGap), 0);
                movementPanel.Controls.Add(movementButtons[i]);
            }

            return yPos + btnSize + 20;
        }

        /// <summary>
        /// Creates the Lume Color section with color palette buttons.
        /// </summary>
        private int CreateLumeColorSection(int yPos)
        {
            const int lumeHeight = 130;
            Panel lumePanel = new Panel();
            lumePanel.Location = new Point(0, yPos);
            lumePanel.Size = new Size(FormWidth, lumeHeight);
            lumePanel.BackColor = Color.FromArgb(30, 30, 30);
            this.Controls.Add(lumePanel);

            Label lblLumeColor = new Label();
            lblLumeColor.Text = "Lume Color";
            lblLumeColor.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblLumeColor.ForeColor = Color.FromArgb(220, 220, 220);
            lblLumeColor.Location = new Point(LeftPad, 15);
            lblLumeColor.AutoSize = true;
            lumePanel.Controls.Add(lblLumeColor);

            // Center the colour palette. 14 dots × 30 px + 13 gaps × 5 px = 485 px.
            const int dotSize = 30, dotGap = 5;
            int rowWidth = lumeColors.Length * dotSize + (lumeColors.Length - 1) * dotGap;
            int rowX = (FormWidth - rowWidth) / 2;

            Panel colorsContainer = new Panel();
            colorsContainer.Location = new Point(rowX, 50);
            colorsContainer.Size = new Size(rowWidth, dotSize);
            colorsContainer.BackColor = Color.Transparent;
            lumePanel.Controls.Add(colorsContainer);

            lumeColorButtons = new Button[lumeColors.Length];
            for (int i = 0; i < lumeColors.Length; i++)
            {
                lumeColorButtons[i] = CreateLumeColorButton(lumeColors[i], i);
                lumeColorButtons[i].Location = new Point(i * (dotSize + dotGap), 0);
                colorsContainer.Controls.Add(lumeColorButtons[i]);
            }

            // "Swiss BGW9" centered under the palette.
            Label lblSwissBGW9 = new Label();
            lblSwissBGW9.Text = "Swiss BGW9";
            lblSwissBGW9.Font = new Font("Segoe UI", 8F);
            lblSwissBGW9.ForeColor = Color.FromArgb(150, 150, 150);
            lblSwissBGW9.AutoSize = true;
            lumePanel.Controls.Add(lblSwissBGW9);
            // Position after AutoSize takes effect.
            lblSwissBGW9.Location = new Point((FormWidth - lblSwissBGW9.PreferredWidth) / 2, 95);

            return yPos + lumeHeight + 10;
        }

        /// <summary>
        /// Creates the Cancel and OK action buttons.
        /// </summary>
        private void CreateActionButtons(int yPos)
        {
            const int rightEdge = FormWidth - RightPad;  // 620
            const int buttonWidth = 90;
            const int buttonHeight = 36;
            const int buttonGap = 10;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Font = new Font("Segoe UI", 9F);
            btnCancel.Location = new Point(rightEdge - buttonWidth * 2 - buttonGap, yPos);
            btnCancel.Size = new Size(buttonWidth, buttonHeight);
            btnCancel.BackColor = Color.FromArgb(100, 100, 100);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.DialogResult = DialogResult.Cancel;
            ApplyPillShape(btnCancel);
            this.Controls.Add(btnCancel);

            btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Font = new Font("Segoe UI", 9F);
            btnOK.Location = new Point(rightEdge - buttonWidth, yPos);
            btnOK.Size = new Size(buttonWidth, buttonHeight);
            btnOK.BackColor = Color.FromArgb(240, 240, 240);
            btnOK.ForeColor = Color.FromArgb(40, 40, 40);
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Cursor = Cursors.Hand;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Click += BtnOK_Click;
            ApplyPillShape(btnOK);
            this.Controls.Add(btnOK);
        }

        /// <summary>
        /// Applies a fully-rounded "pill" region to the given button so it renders with
        /// semicircular ends instead of a rectangle.
        /// </summary>
        private static void ApplyPillShape(Control c)
        {
            int radius = c.Height / 2;
            int d = radius * 2;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(0, 0, d, d, 90, 180);
                path.AddArc(c.Width - d, 0, d, d, 270, 180);
                path.CloseFigure();
                c.Region = new Region(path);
            }
        }

        #endregion

        #region Control Creation Helpers

        /// <summary>
        /// Creates an appearance radio button with custom appearance.
        /// </summary>
        private RadioButton CreateAppearanceButton(string text, int index)
        {
            RadioButton btn = new RadioButton();
            btn.Text = "";
            btn.Size = new Size(150, 100);
            btn.Appearance = Appearance.Button;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btn.FlatAppearance.CheckedBackColor = Color.FromArgb(220, 240, 255);
            btn.BackColor = Color.White;
            btn.Cursor = Cursors.Hand;
            btn.Tag = text;
            btn.Paint += AppearanceButton_Paint;
            btn.CheckedChanged += (s, e) => { if (btn.Checked) btn.Invalidate(); };
            
            return btn;
        }

        /// <summary>
        /// Creates a dial theme radio button with custom appearance.
        /// </summary>
        private RadioButton CreateDialThemeButton(string text, Color themeColor)
        {
            RadioButton btn = new RadioButton();
            btn.Text = "";
            btn.Size = new Size(85, 100);
            btn.Appearance = Appearance.Button;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.Transparent;
            btn.Cursor = Cursors.Hand;
            btn.Tag = new object[] { text, themeColor };
            btn.Paint += DialThemeButton_Paint;
            btn.CheckedChanged += (s, e) => btn.Invalidate();
            
            return btn;
        }

        /// <summary>
        /// Creates a size radio button with custom appearance.
        /// </summary>
        private RadioButton CreateSizeButton(string text)
        {
            RadioButton btn = new RadioButton();
            btn.Text = "";
            btn.Size = new Size(105, 100);
            btn.Appearance = Appearance.Button;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btn.FlatAppearance.CheckedBackColor = Color.FromArgb(220, 240, 255);
            btn.BackColor = Color.White;
            btn.Cursor = Cursors.Hand;
            btn.Tag = text;
            btn.Paint += SizeButton_Paint;
            btn.CheckedChanged += (s, e) => btn.Invalidate();
            
            return btn;
        }

        /// <summary>
        /// Creates a movement radio button with custom appearance.
        /// </summary>
        private RadioButton CreateMovementButton(string text)
        {
            RadioButton btn = new RadioButton();
            btn.Text = "";
            btn.Size = new Size(70, 100);
            btn.Appearance = Appearance.Button;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btn.FlatAppearance.CheckedBackColor = Color.FromArgb(220, 240, 255);
            btn.BackColor = Color.White;
            btn.Cursor = Cursors.Hand;
            btn.Tag = text;
            btn.Paint += MovementButton_Paint;
            btn.CheckedChanged += (s, e) =>
            {
                btn.Invalidate();
                if (btn.Checked) UpdateMovementSubtitle();
            };

            return btn;
        }

        /// <summary>
        /// Updates the small descriptor next to the Movement section title to match the
        /// currently-selected movement button.
        /// </summary>
        private void UpdateMovementSubtitle()
        {
            if (lblMovementSubtitle == null || movementButtons == null) return;
            for (int i = 0; i < movementButtons.Length && i < MovementSubtitles.Length; i++)
            {
                if (movementButtons[i].Checked)
                {
                    lblMovementSubtitle.Text = MovementSubtitles[i];
                    return;
                }
            }
            lblMovementSubtitle.Text = "";
        }

        /// <summary>
        /// Creates a lume color button.
        /// </summary>
        private Button CreateLumeColorButton(Color color, int index)
        {
            Button btn = new Button();
            btn.Size = new Size(30, 30);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.Cursor = Cursors.Hand;
            btn.Tag = color;
            btn.Click += LumeColorButton_Click;
            btn.Paint += LumeColorButton_Paint;
            
            return btn;
        }

        #endregion

        #region Paint Handlers

        /// <summary>
        /// Custom paint for appearance buttons.
        /// </summary>
        private void AppearanceButton_Paint(object sender, PaintEventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            string text = btn.Tag as string;
            
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw preview based on appearance mode
            Rectangle previewRect = new Rectangle(15, 10, btn.Width - 30, btn.Height - 40);
            
            if (text == "Day")
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, 230, 240)), previewRect);
                DrawMiniClock(e.Graphics, previewRect, Color.FromArgb(60, 60, 60), Color.FromArgb(175, 238, 238));
            }
            else if (text == "Night")
            {
                e.Graphics.FillRectangle(Brushes.Black, previewRect);
                DrawMiniClock(e.Graphics, previewRect, Color.FromArgb(0, 255, 255), Color.Black);
            }
            else // System
            {
                Rectangle leftRect = new Rectangle(previewRect.X, previewRect.Y, previewRect.Width / 2, previewRect.Height);
                Rectangle rightRect = new Rectangle(previewRect.X + previewRect.Width / 2, previewRect.Y, previewRect.Width / 2, previewRect.Height);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, 230, 240)), leftRect);
                e.Graphics.FillRectangle(Brushes.Black, rightRect);
                DrawMiniClock(e.Graphics, previewRect, Color.FromArgb(0, 255, 255), Color.Transparent);
            }
            
            // Draw label
            using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far })
            {
                e.Graphics.DrawString(text, new Font("Segoe UI", 9F), new SolidBrush(Color.FromArgb(60, 60, 60)), 
                    new RectangleF(0, btn.Height - 25, btn.Width, 20), sf);
            }
            
            // Draw selection border
            if (btn.Checked)
            {
                using (Pen pen = new Pen(Color.FromArgb(40, 40, 40), 3))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, btn.Width - 3, btn.Height - 3);
                }
            }
        }

        /// <summary>
        /// Custom paint for dial theme buttons.
        /// </summary>
        private void DialThemeButton_Paint(object sender, PaintEventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            object[] data = btn.Tag as object[];
            string text = data[0] as string;
            Color themeColor = (Color)data[1];

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Scale dial preview to fit the (now larger) button — leave room for the label.
            int centerX = btn.Width / 2;
            int centerY = 45;
            int radius = 38;

            using (SolidBrush brush = new SolidBrush(themeColor))
            {
                e.Graphics.FillEllipse(brush, centerX - radius, centerY - radius, radius * 2, radius * 2);
            }

            // Selected swatch: thick dark ring on the circle. Unselected: thin neutral ring.
            int borderWidth = btn.Checked ? 3 : 1;
            Color borderColor = btn.Checked ? Color.FromArgb(40, 40, 40) : Color.FromArgb(180, 180, 180);
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
            }

            DrawDialMarkings(e.Graphics, centerX, centerY, radius - 6, themeColor);

            // Selected label: bold + dark. Unselected: regular + mid-grey.
            FontStyle labelStyle = btn.Checked ? FontStyle.Bold : FontStyle.Regular;
            Color labelColor = btn.Checked ? Color.FromArgb(40, 40, 40) : Color.FromArgb(110, 110, 110);
            using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center })
            using (Font labelFont = new Font("Segoe UI", 9F, labelStyle))
            using (SolidBrush labelBrush = new SolidBrush(labelColor))
            {
                e.Graphics.DrawString(text, labelFont, labelBrush, centerX, btn.Height - 22, sf);
            }
        }

        /// <summary>
        /// Custom paint for size buttons.
        /// </summary>
        private void SizeButton_Paint(object sender, PaintEventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            string text = btn.Tag as string;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle iconRect = new Rectangle(15, 10, btn.Width - 30, btn.Height - 35);
            int tickCount = (text == "Classic") ? 12 : 8;
            DrawTickClockIcon(e.Graphics, iconRect, tickCount, drawHands: false);

            DrawCardLabel(e.Graphics, btn, text);
            DrawCardSelectionBorder(e.Graphics, btn);
        }

        /// <summary>
        /// Custom paint for movement buttons.
        /// </summary>
        private void MovementButton_Paint(object sender, PaintEventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            string text = btn.Tag as string;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle iconRect = new Rectangle(15, 10, btn.Width - 30, btn.Height - 35);
            DrawMovementIcon(e.Graphics, iconRect, text);

            DrawCardLabel(e.Graphics, btn, text);
            DrawCardSelectionBorder(e.Graphics, btn);
        }

        /// <summary>
        /// Draws the bottom-centred label shared by Size and Movement card buttons.
        /// Selected card → bold + dark; unselected → regular + mid-grey.
        /// </summary>
        private void DrawCardLabel(Graphics g, RadioButton btn, string text)
        {
            FontStyle style = btn.Checked ? FontStyle.Bold : FontStyle.Regular;
            Color colour = btn.Checked ? Color.FromArgb(40, 40, 40) : Color.FromArgb(110, 110, 110);
            using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center })
            using (Font f = new Font("Segoe UI", 9F, style))
            using (Brush b = new SolidBrush(colour))
            {
                g.DrawString(text, f, b, btn.Width / 2, btn.Height - 22, sf);
            }
        }

        /// <summary>
        /// Draws the selection border around a card (Size or Movement) when checked.
        /// </summary>
        private static void DrawCardSelectionBorder(Graphics g, RadioButton btn)
        {
            if (!btn.Checked) return;
            using (Pen pen = new Pen(Color.FromArgb(40, 40, 40), 2))
            {
                g.DrawRectangle(pen, 1, 1, btn.Width - 3, btn.Height - 3);
            }
        }

        /// <summary>
        /// Draws a tick-mark-only clock face (no outer ring) — used by Size cards.
        /// </summary>
        private static void DrawTickClockIcon(Graphics g, Rectangle bounds, int tickCount, bool drawHands)
        {
            int centerX = bounds.X + bounds.Width / 2;
            int centerY = bounds.Y + bounds.Height / 2;
            int radius = Math.Min(bounds.Width, bounds.Height) / 2 - 4;
            using (Pen pen = new Pen(Color.FromArgb(70, 70, 70), 1.8f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                for (int i = 0; i < tickCount; i++)
                {
                    double angle = i * (2 * Math.PI / tickCount) - Math.PI / 2;
                    int x1 = centerX + (int)((radius - 5) * Math.Cos(angle));
                    int y1 = centerY + (int)((radius - 5) * Math.Sin(angle));
                    int x2 = centerX + (int)(radius * Math.Cos(angle));
                    int y2 = centerY + (int)(radius * Math.Sin(angle));
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
                if (drawHands)
                {
                    g.DrawLine(pen, centerX, centerY, centerX, centerY - radius / 2);
                    g.DrawLine(pen, centerX, centerY, centerX + radius / 3, centerY);
                }
            }
        }

        /// <summary>
        /// Draws the per-movement icon shown inside Movement card buttons.
        ///   Quartz:    12 ticks + single hand pointing to the 1 o'clock position.
        ///   Mechanical: 12 ticks + two hands + small sweep arrow near the centre.
        ///   Digital:   12 ticks + two hands at 10:10.
        /// </summary>
        private static void DrawMovementIcon(Graphics g, Rectangle bounds, string movement)
        {
            // First draw the tick face that all three share.
            DrawTickClockIcon(g, bounds, 12, drawHands: false);

            int centerX = bounds.X + bounds.Width / 2;
            int centerY = bounds.Y + bounds.Height / 2;
            int radius = Math.Min(bounds.Width, bounds.Height) / 2 - 4;

            using (Pen pen = new Pen(Color.FromArgb(50, 50, 50), 1.8f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                if (movement == "Quartz")
                {
                    // Single hand at the 1 o'clock direction (-60° from up).
                    double a = -Math.PI / 2 + Math.PI / 6;
                    g.DrawLine(pen,
                        centerX, centerY,
                        centerX + (float)(radius * 0.75 * Math.Cos(a)),
                        centerY + (float)(radius * 0.75 * Math.Sin(a)));
                }
                else if (movement == "Mechanical")
                {
                    // Two hands at 10:10 plus a small upward sweep arrow near the hub.
                    double mAngle = -Math.PI / 2 + (10 * Math.PI / 30);    // minute → 2 o'clock
                    double hAngle = -Math.PI / 2 - (Math.PI / 3);           // hour → 10 o'clock
                    g.DrawLine(pen, centerX, centerY,
                        centerX + (float)(radius * 0.7 * Math.Cos(mAngle)),
                        centerY + (float)(radius * 0.7 * Math.Sin(mAngle)));
                    g.DrawLine(pen, centerX, centerY,
                        centerX + (float)(radius * 0.5 * Math.Cos(hAngle)),
                        centerY + (float)(radius * 0.5 * Math.Sin(hAngle)));
                    // Tiny up-arrow indicating sweep, sitting just above centre.
                    int ax = centerX, ay = centerY - 2;
                    g.DrawLine(pen, ax, ay, ax - 3, ay + 3);
                    g.DrawLine(pen, ax, ay, ax + 3, ay + 3);
                    g.DrawLine(pen, ax, ay, ax, ay + 6);
                }
                else // Digital
                {
                    double mAngle = -Math.PI / 2 + (10 * Math.PI / 30);
                    double hAngle = -Math.PI / 2 - (Math.PI / 3);
                    g.DrawLine(pen, centerX, centerY,
                        centerX + (float)(radius * 0.7 * Math.Cos(mAngle)),
                        centerY + (float)(radius * 0.7 * Math.Sin(mAngle)));
                    g.DrawLine(pen, centerX, centerY,
                        centerX + (float)(radius * 0.5 * Math.Cos(hAngle)),
                        centerY + (float)(radius * 0.5 * Math.Sin(hAngle)));
                }
            }
        }

        /// <summary>
        /// Custom paint for lume color buttons.
        /// </summary>
        private void LumeColorButton_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw circular color button
            using (SolidBrush brush = new SolidBrush(btn.BackColor))
            {
                e.Graphics.FillEllipse(brush, 3, 3, btn.Width - 6, btn.Height - 6);
            }
            
            // Draw selection ring
            if (btn == selectedLumeButton)
            {
                using (Pen pen = new Pen(Color.White, 2))
                {
                    e.Graphics.DrawEllipse(pen, 1, 1, btn.Width - 2, btn.Height - 2);
                }
            }
        }

        /// <summary>
        /// Draws a mini clock face.
        /// </summary>
        private void DrawMiniClock(Graphics g, Rectangle bounds, Color handColor, Color bgColor)
        {
            int centerX = bounds.X + bounds.Width / 2;
            int centerY = bounds.Y + bounds.Height / 2;
            int radius = Math.Min(bounds.Width, bounds.Height) / 3;
            
            // Draw clock markings
            using (Pen pen = new Pen(handColor, 1.5f))
            {
                for (int i = 0; i < 12; i++)
                {
                    double angle = i * 30 * Math.PI / 180 - Math.PI / 2;
                    int x1 = centerX + (int)((radius - 3) * Math.Cos(angle));
                    int y1 = centerY + (int)((radius - 3) * Math.Sin(angle));
                    int x2 = centerX + (int)(radius * Math.Cos(angle));
                    int y2 = centerY + (int)(radius * Math.Sin(angle));
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
            
            // Draw hands
            using (Pen pen = new Pen(handColor, 2))
            {
                // Hour hand (10:10)
                double hourAngle = (10 * 30 + 10 * 0.5) * Math.PI / 180 - Math.PI / 2;
                g.DrawLine(pen, centerX, centerY, 
                    centerX + (int)(radius * 0.5 * Math.Cos(hourAngle)), 
                    centerY + (int)(radius * 0.5 * Math.Sin(hourAngle)));
                
                // Minute hand (10:10)
                double minAngle = 10 * 6 * Math.PI / 180 - Math.PI / 2;
                g.DrawLine(pen, centerX, centerY, 
                    centerX + (int)(radius * 0.75 * Math.Cos(minAngle)), 
                    centerY + (int)(radius * 0.75 * Math.Sin(minAngle)));
            }
        }

        /// <summary>
        /// Draws dial markings for theme preview.
        /// </summary>
        private void DrawDialMarkings(Graphics g, int centerX, int centerY, int radius, Color bgColor)
        {
            Color markColor = GetContrastColor(bgColor);
            
            using (Pen pen = new Pen(markColor, 1))
            {
                // Draw hour markers
                for (int i = 0; i < 12; i++)
                {
                    double angle = i * 30 * Math.PI / 180 - Math.PI / 2;
                    int x1 = centerX + (int)((radius - 2) * Math.Cos(angle));
                    int y1 = centerY + (int)((radius - 2) * Math.Sin(angle));
                    int x2 = centerX + (int)(radius * Math.Cos(angle));
                    int y2 = centerY + (int)(radius * Math.Sin(angle));
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
            
            // Draw "IQ" text
            using (Font font = new Font("Segoe UI", 7F, FontStyle.Bold))
            using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString("IQ", font, new SolidBrush(markColor), centerX, centerY, sf);
            }
        }

        /// <summary>
        /// Draws a clock icon for size buttons.
        /// </summary>
        private void DrawClockIcon(Graphics g, Rectangle bounds, int tickCount)
        {
            int centerX = bounds.X + bounds.Width / 2;
            int centerY = bounds.Y + bounds.Height / 2;
            int radius = Math.Min(bounds.Width, bounds.Height) / 2 - 5;
            
            using (Pen pen = new Pen(Color.FromArgb(60, 60, 60), 2))
            {
                // Draw circle
                g.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
                
                // Draw tick marks
                for (int i = 0; i < tickCount; i++)
                {
                    double angle = i * (360.0 / tickCount) * Math.PI / 180 - Math.PI / 2;
                    int x1 = centerX + (int)((radius - 4) * Math.Cos(angle));
                    int y1 = centerY + (int)((radius - 4) * Math.Sin(angle));
                    int x2 = centerX + (int)(radius * Math.Cos(angle));
                    int y2 = centerY + (int)(radius * Math.Sin(angle));
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
                
                // Draw hands
                g.DrawLine(pen, centerX, centerY, centerX + radius / 2, centerY - radius / 3);
                g.DrawLine(pen, centerX, centerY, centerX + radius / 3, centerY + radius / 2);
            }
        }

        /// <summary>
        /// Gets a contrasting color for text/markings.
        /// </summary>
        private Color GetContrastColor(Color bgColor)
        {
            int brightness = (int)(bgColor.R * 0.299 + bgColor.G * 0.587 + bgColor.B * 0.114);
            return brightness > 128 ? Color.FromArgb(60, 60, 60) : Color.FromArgb(220, 220, 220);
        }

        #endregion

        #region Settings Management

        /// <summary>
        /// Loads current settings into the form controls.
        /// </summary>
        private void LoadSettings()
        {
            // Load Appearance
            appearanceButtons[(int)config.Appearance].Checked = true;
            
            // Load Dial Theme — also flip to the page that contains the selected swatch.
            dialThemeButtons[(int)config.DialTheme].Checked = true;
            ShowDialPage((int)config.DialTheme / DialsPerPage);
            
            // Load Size
            sizeButtons[(int)config.Size].Checked = true;
            
            // Load Movement
            movementButtons[(int)config.Movement].Checked = true;
            
            // Load Lume Color
            int closestColorIndex = FindClosestColorIndex(config.LumeColor);
            selectedLumeButton = lumeColorButtons[closestColorIndex];
            selectedLumeButton.Invalidate();
        }

        /// <summary>
        /// Saves settings from form controls to configuration.
        /// </summary>
        private void SaveSettings()
        {
            // Save Appearance
            for (int i = 0; i < appearanceButtons.Length; i++)
            {
                if (appearanceButtons[i].Checked)
                {
                    config.Appearance = (BauhausConfig.AppearanceMode)i;
                    break;
                }
            }
            
            // Save Dial Theme
            for (int i = 0; i < dialThemeButtons.Length; i++)
            {
                if (dialThemeButtons[i].Checked)
                {
                    config.DialTheme = (BauhausConfig.ClockDialTheme)i;
                    break;
                }
            }
            
            // Save Size
            for (int i = 0; i < sizeButtons.Length; i++)
            {
                if (sizeButtons[i].Checked)
                {
                    config.Size = (BauhausConfig.ClockSize)i;
                    break;
                }
            }
            
            // Save Movement
            for (int i = 0; i < movementButtons.Length; i++)
            {
                if (movementButtons[i].Checked)
                {
                    config.Movement = (BauhausConfig.MovementType)i;
                    break;
                }
            }
            
            // Save Lume Color
            if (selectedLumeButton != null)
            {
                config.LumeColor = (Color)selectedLumeButton.Tag;
            }
            
            config.Save();
        }

        /// <summary>
        /// Finds the closest color index in the palette.
        /// </summary>
        private int FindClosestColorIndex(Color targetColor)
        {
            int closestIndex = 0;
            int minDistance = int.MaxValue;
            
            for (int i = 0; i < lumeColors.Length; i++)
            {
                int distance = Math.Abs(lumeColors[i].R - targetColor.R) +
                              Math.Abs(lumeColors[i].G - targetColor.G) +
                              Math.Abs(lumeColors[i].B - targetColor.B);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }
            
            return closestIndex;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles lume color button clicks.
        /// </summary>
        private void LumeColorButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            
            if (selectedLumeButton != null)
            {
                selectedLumeButton.Invalidate();
            }
            
            selectedLumeButton = btn;
            btn.Invalidate();
        }

        /// <summary>
        /// Handles the OK button click.
        /// </summary>
        private void BtnOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        #endregion
    }
}
