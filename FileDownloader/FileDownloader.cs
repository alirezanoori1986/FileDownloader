using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloader
{
    /// <summary>
    /// Downloads a list of DownloadItems
    /// </summary>
    public class FileDownloader
    {
        /// <summary>
        /// Number of processed items
        /// </summary>
        private int processed = 0;

        /// <summary>
        /// The total number of items in the download queue
        /// </summary>
        private int totalCount = 0;

        /// <summary>
        /// ILogger item which will handle logging operations
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Report progress
        /// </summary>
        /// <param name="percent">Progress percentage</param>
        private void OnProgress(int percent)
        {
            if (Logger != null)
            {
                Logger.ReportProgress(percent);
            }
        }

        /// <summary>
        /// Write a log message
        /// </summary>
        /// <param name="message">Log message</param>
        private void OnLog(string message)
        {
            if (Logger != null)
            {
                Logger.WriteLog(message);
            }
        }

        /// <summary>
        /// Write a debug message
        /// </summary>
        /// <param name="message">Debug message</param>
        private void OnDebug(string message)
        {
            if (Logger != null)
            {
                Logger.WriteDebug(message);
            }
        }

        /// <summary>
        /// Start download operation
        /// </summary>
        /// <param name="items">The DownloadItem queue. Could be any sort of IEnumerable</param>
        /// <param name="ct">CancellationToken to support cancellation</param>
        public void StartDownload(IEnumerable<DownloadItem> items, CancellationToken ct)
        {
            //first count the items and save it for calculating progress percentage
            totalCount = items.Count();
            OnLog(string.Format("Downloading {0} items", totalCount));

            //Use .Net's parallel library
            Parallel.ForEach
                (
                items,
                new ParallelOptions { MaxDegreeOfParallelism = 8 }, //Maximum concurrent downloads
                file =>
                {
                    if (ct.IsCancellationRequested)
                        ct.ThrowIfCancellationRequested();

                    try
                    {
                        //skip existing files. Remove this to overwrite
                        if (!File.Exists(file.Path))
                        {
                            try
                            {
                                //Simply download the file
                                using (var cli = new WebClient())
                                {
                                    OnDebug(string.Format("Downloading {0} to {1}", file.Address, file.Path));
                                    cli.DownloadFile(file.Address, file.Path);
                                }
                            }
                            catch (Exception)
                            {
                                OnLog("Error downloading file from " + file.Address);
                                //Do whatever you want. Maybe write a debug message, stop download, etc.
                            }
                        }
                        else
                            OnDebug("Skipping file " + file.Path);

                        //calculate progress and report it through logger
                        int pro = Interlocked.Increment(ref processed);
                        double percent = 100.0 * pro / totalCount;
                        OnProgress((int)percent);
                    }
                    catch (Exception)
                    {
                        //Something extraordinary happened.
                        //Do something. For example, delete the file.
                        if (File.Exists(file.Path))
                            File.Delete(file.Path);
                    }
                }
                );
        }
    }
}
