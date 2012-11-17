using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloader
{
    /// <summary>
    /// Represents a download item.
    /// </summary>
    public class DownloadItem
    {
        /// <summary>
        /// Web address of the file. Usually a URL
        /// </summary>
        public Uri Address { get; set; }

        /// <summary>
        /// File path in which the file will be downloaded
        /// </summary>
        public string Path { get; set; }
    }
}
