﻿namespace DirkSarodnick.TS3_Bot.Core.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Settings;

    /// <summary>
    /// Defines the SettingsDirectoryService class.
    /// </summary>
    public class SettingsDirectoryService : IDisposable
    {
        private bool disposed;
        private readonly List<InstanceSettings> settingsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDirectoryService"/> class.
        /// </summary>
        public SettingsDirectoryService(string path)
        {
            this.settingsList = Directory.GetFiles(path, "*.xml").Select(SettingsManager.LoadSettings).ToList();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SettingsDirectoryService"/> is reclaimed by garbage collection.
        /// </summary>
        ~SettingsDirectoryService()
        {
            Dispose();
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public SettingsService GetService(int index)
        {
            return new SettingsService(this.settingsList.ElementAt(index).FilePath);
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public SettingsService GetService(string fileName)
        {
            var settings = this.settingsList.SingleOrDefault(file => Path.GetFileName(file.FilePath) == fileName);
            if (settings != null)
            {
                return new SettingsService(settings.FilePath);
            }

            return null;
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <returns></returns>
        public List<InstanceSettings> GetFiles()
        {
            return this.settingsList;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}