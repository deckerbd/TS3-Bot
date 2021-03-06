﻿namespace DirkSarodnick.TS3_Bot.Core.Helper
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Defines the BasicHelper class.
    /// </summary>
    public class BasicHelper
    {
        private static Stream defaultConfiguration;
        private static Stream defaultSchema;

        /// <summary>
        /// Gets the default configuration stream.
        /// </summary>
        /// <value>The default configuration stream.</value>
        public static Stream DefaultConfiguration
        {
            get
            {
                if (defaultConfiguration == null)
                {
                    defaultConfiguration = Assembly.GetExecutingAssembly().GetManifestResourceStream("DirkSarodnick.TS3_Bot.Core.Configuration.DefaultSettings.xml");
                }

                if (defaultConfiguration != null)
                {
                    defaultConfiguration.Seek(0, SeekOrigin.Begin);
                }

                return defaultConfiguration;
            }
        }

        /// <summary>
        /// Gets the default schema.
        /// </summary>
        /// <value>The default schema.</value>
        public static Stream DefaultSchema
        {
            get
            {
                if (defaultSchema == null)
                {
                    defaultSchema = Assembly.GetExecutingAssembly().GetManifestResourceStream("DirkSarodnick.TS3_Bot.Core.Configuration.Settings.xsd");
                }

                if (defaultSchema != null)
                {
                    defaultSchema.Seek(0, SeekOrigin.Begin);
                }

                return defaultSchema;
            }
        }

        /// <summary>
        /// Gets the configuration directory.
        /// </summary>
        public static string ConfigurationDirectory
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration");
            }
        }

        /// <summary>
        /// Gets the data directory.
        /// </summary>
        public static string DataDirectory
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            }
        }

        /// <summary>
        /// Determines whether [is valid culture] [the specified culture name].
        /// </summary>
        /// <param name="cultureName">Name of the culture.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid culture] [the specified culture name]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidCulture(string cultureName)
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures).Any(ci => ci.Name == cultureName);
        }

        /// <summary>
        /// Gets the timespan as readable string.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>The readable timespan string.</returns>
        public static string GetTimespanString(TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0)
            {
                return string.Format("{0}d, {1}h, {2}min, {3}s", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }

            if (timeSpan.Hours > 0)
            {
                return string.Format("{0}h, {1}min, {2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }

            if (timeSpan.Minutes > 0)
            {
                return string.Format("{0}min, {1}s", timeSpan.Minutes, timeSpan.Seconds);
            }

            return string.Format("{0}s", timeSpan.Seconds);
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <param name="fileSize">Size of the file.</param>
        /// <returns>The file size</returns>
        public static string GetFileSize(ulong fileSize)
        {
            double size = fileSize;
            int sizeType = 0;
            while(size > 1024)
            {
                size = size/1024;
                sizeType++;
            }

            switch (sizeType)
            {
                case 1: return size.ToString("F2") + " KByte";
                case 2: return size.ToString("F2") + " MByte";
                case 3: return size.ToString("F2") + " GByte";
                case 4: return size.ToString("F2") + " TByte";
                default: return size.ToString("F2") + " Byte";
            }
        }
    }
}
