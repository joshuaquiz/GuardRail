using System;

namespace GuardRail.Core
{
    /// <summary>
    /// Represents a file to be downloaded for the install.
    /// </summary>
    public sealed class InstallFile
    {
        /// <summary>
        /// The Location of the file on the web.
        /// </summary>
        public Uri DownloadUri { get; set; }

        /// <summary>
        /// Where the file goes locally.
        /// </summary>
        public string LocalPath { get; set; }
    }
}