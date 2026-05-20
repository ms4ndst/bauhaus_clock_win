using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BauhausScreensaver
{
    /// <summary>
    /// Main screensaver form that displays multiple Bauhaus clock faces.
    /// Supports both full-screen and preview modes with optimized rendering.
    /// </summary>
    public partial class ScreensaverForm : Form
    {
        #region Fields

        private BauhausClock clock;
        private BauhausConfig config;
        private Timer renderTimer;
        private Point mouseLocation;
        private bool isPreviewMode;
        private BufferedGraphicsContext context;
        private BufferedGraphics buffer;

        #endregion

        #region Win32 API Imports

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        private const int GWL_STYLE = -16;
        private const int WS_CHILD = 0x40000000;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance for full-screen mode.
        /// </summary>
        /// <param name="bounds">Screen bounds.</param>
        public ScreensaverForm(Rectangle bounds)
        {
            InitializeComponent();
            InitializeScreensaver(bounds, false);
        }

        /// <summary>
        /// Initializes a new instance for preview mode.
        /// </summary>
        /// <param name="previewHandle">Handle to the preview window.</param>
        public ScreensaverForm(IntPtr previewHandle)
        {
            InitializeComponent();
            
            // Set the preview window as the parent of this window
            SetParent(this.Handle, previewHandle);

            // Make this a child window so it will close when the parent closes
            SetWindowLong(this.Handle, GWL_STYLE, new IntPtr(GetWindowLong(this.Handle, GWL_STYLE) | WS_CHILD));

            // Get the bounds of the preview window
            Rectangle parentRect;
            GetClientRect(previewHandle, out parentRect);

            InitializeScreensaver(parentRect, true);
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the screensaver with the specified bounds and mode.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form settings
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "ScreensaverForm";
            this.StartPosition = FormStartPosition.Manual;
            this.Text = "Bauhaus Clock Screensaver";
            this.DoubleBuffered = true;
            
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Initializes the screensaver components.
        /// </summary>
        private void InitializeScreensaver(Rectangle bounds, bool preview)
        {
            isPreviewMode = preview;
            this.Bounds = bounds;
            config = BauhausConfig.Load();

            // Set up form properties
            if (!isPreviewMode)
            {
                this.TopMost = true;
                Cursor.Hide();
            }

            this.BackColor = Color.Black;

            // Initialize single clock
            clock = new BauhausClock(config);

            // Set up double buffering
            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            buffer = context.Allocate(this.CreateGraphics(), this.ClientRectangle);

            // Set up timer for rendering
            renderTimer = new Timer();
            renderTimer.Interval = 1000 / 30; // 30 FPS
            renderTimer.Tick += RenderTimer_Tick;
            renderTimer.Start();

            // Event handlers for exit conditions
            if (!isPreviewMode)
            {
                this.MouseMove += ScreensaverForm_MouseMove;
                this.MouseClick += ScreensaverForm_MouseClick;
                this.KeyPress += ScreensaverForm_KeyPress;
                mouseLocation = Cursor.Position;
            }
        }

        #endregion

        #region Rendering

        /// <summary>
        /// Handles the timer tick event for rendering.
        /// </summary>
        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            // Render to buffer
            Graphics g = buffer.Graphics;
            
            // Render the clock (it draws its own background)
            clock.Render(g, this.ClientRectangle);

            // Draw buffer to screen
            buffer.Render(Graphics.FromHwnd(this.Handle));
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles mouse movement to detect exit condition.
        /// </summary>
        private void ScreensaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseLocation.IsEmpty)
            {
                // Check if mouse has moved more than a small threshold
                if (Math.Abs(mouseLocation.X - e.X) > 5 || Math.Abs(mouseLocation.Y - e.Y) > 5)
                {
                    ExitScreensaver();
                }
            }
            mouseLocation = e.Location;
        }

        /// <summary>
        /// Handles mouse click to exit screensaver.
        /// </summary>
        private void ScreensaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            ExitScreensaver();
        }

        /// <summary>
        /// Handles key press to exit screensaver.
        /// </summary>
        private void ScreensaverForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            ExitScreensaver();
        }

        /// <summary>
        /// Exits the screensaver gracefully.
        /// </summary>
        private void ExitScreensaver()
        {
            if (!isPreviewMode)
            {
                Cursor.Show();
                Application.Exit();
            }
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Clean up resources when form is closing.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!isPreviewMode)
                {
                    Cursor.Show();
                }

                if (renderTimer != null)
                {
                    renderTimer.Stop();
                    renderTimer.Dispose();
                }

                if (buffer != null)
                {
                    buffer.Dispose();
                }

                if (context != null)
                {
                    context.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
