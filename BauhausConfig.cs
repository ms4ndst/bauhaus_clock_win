using System;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;

namespace BauhausScreensaver
{
    /// <summary>
    /// Configuration settings for the Bauhaus Clock screensaver.
    /// Handles loading, saving, and managing user preferences.
    /// </summary>
    [Serializable]
    public class BauhausConfig
    {
        #region Enums

        public enum AppearanceMode
        {
            Day,
            Night,
            System
        }

        public enum ClockDialTheme
        {
            White,
            Turquoise,
            Glacier,
            Ocean,
            Tennis,
            SignalBlue,
            SkyBlue,
            Beige,
            Cream,
            Lavender,
            Rose,
            Salmon,
            Yellow,
            Pistachio,
            Slate,
            Noir
        }

        public enum ClockSize
        {
            Classic,
            Compact
        }

        public enum MovementType
        {
            Quartz,
            Mechanical,
            Digital
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the appearance mode (Day, Night, or System).
        /// </summary>
        public AppearanceMode Appearance { get; set; }

        /// <summary>
        /// Gets or sets the clock dial theme.
        /// </summary>
        public ClockDialTheme DialTheme { get; set; }

        /// <summary>
        /// Gets or sets the clock size.
        /// </summary>
        public ClockSize Size { get; set; }

        /// <summary>
        /// Gets or sets the movement type.
        /// </summary>
        public MovementType Movement { get; set; }

        /// <summary>
        /// Gets or sets the lume color (stored as ARGB value).
        /// </summary>
        public int LumeColorArgb { get; set; }

        #endregion

        #region Non-Serialized Properties

        /// <summary>
        /// Gets or sets the lume color.
        /// </summary>
        [XmlIgnore]
        public Color LumeColor
        {
            get { return Color.FromArgb(LumeColorArgb); }
            set { LumeColorArgb = value.ToArgb(); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the BauhausConfig class with default values.
        /// </summary>
        public BauhausConfig()
        {
            // Set default values
            Appearance = AppearanceMode.Day;
            DialTheme = ClockDialTheme.White;
            Size = ClockSize.Classic;
            Movement = MovementType.Mechanical;
            LumeColor = Color.FromArgb(255, 255, 255); // White lume
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Gets the configuration file path in the user's application data folder.
        /// </summary>
        private static string ConfigFilePath
        {
            get
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "BauhausScreensaver");
                
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                return Path.Combine(folder, "config.xml");
            }
        }

        /// <summary>
        /// Loads the configuration from disk, or returns default if not found.
        /// </summary>
        /// <returns>The loaded or default configuration.</returns>
        public static BauhausConfig Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(BauhausConfig));
                    using (FileStream fs = new FileStream(ConfigFilePath, FileMode.Open))
                    {
                        return (BauhausConfig)serializer.Deserialize(fs);
                    }
                }
            }
            catch
            {
                // If loading fails, return default config
            }

            return new BauhausConfig();
        }

        /// <summary>
        /// Saves the current configuration to disk.
        /// </summary>
        public void Save()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(BauhausConfig));
                using (FileStream fs = new FileStream(ConfigFilePath, FileMode.Create))
                {
                    serializer.Serialize(fs, this);
                }
            }
            catch
            {
                // Silently fail if unable to save
            }
        }

        #endregion
    }
}
