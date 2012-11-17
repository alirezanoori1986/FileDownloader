using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloader35
{
    /// <summary>
    /// Downloads a list of DownloadItems
    /// </summary>
    public class FileDownloader
    {
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
        /// Download queue
        /// </summary>
        private Queue<DownloadItem> queue = new Queue<DownloadItem>();

        /// <summary>
        /// Maximum number of threads. Could be an argument to StartDownload method
        /// </summary>
        private const int maxThreads = 8;

        /// <summary>
        /// Processed items.
        /// </summary>
        private int processed = 0;

        /// <summary>
        /// Number of currently running downloads
        /// </summary>
        private int processesing = 0;

        /// <summary>
        /// Total number of download items
        /// </summary>
        private int totalCount = 0;

        /// <summary>
        /// Locking object
        /// </summary>
        private string lockObj = "";

        //this is NOT the best way to cancel async operations
        // this is just for simplicity
        public bool CANCEL = false;


        /// <summary>
        /// Start download operation
        /// </summary>
        /// <param name="queue">The download queue</param>
        public void StartDownload(Queue<DownloadItem> queue)
        {
            OnLog("Starting download... File count: " + queue.Count);
            this.queue = queue;
            this.totalCount = queue.Count;
            StartDownload();
        }

        private void StartDownload()
        {
            //check if the downloads has been canceled, items have been finished or 
            if (CANCEL || queue.Count == 0 || Interlocked.Equals(processed, totalCount))
            {
                if (CANCEL)
                    OnLog("Download canceled.");
                else
                    OnLog("Finished download.");
                return;
            }

            lock (lockObj)
            {
                //check if any file remains
                for (int i = 0; !Interlocked.Equals(processesing, maxThreads) && (queue.Count > 0) && !CANCEL; i++)
                {
                    //get next item
                    var file = queue.Dequeue();

                    try
                    {
                        //skip existing files. Remove this to overwrite
                        if (!File.Exists(file.Path))
                        {
                            //start downloading the file
                            var cli = new WebClient();
                            OnDebug(string.Format("Downloading {0} to {1}", file.Address, file.Path));
                            cli.DownloadFileCompleted += cli_DownloadFileCompleted;
                            Interlocked.Increment(ref processesing);
                            cli.DownloadFileAsync(file.Address, file.Path);
                        }
                        else
                            OnDebug("Skipping file " + file.Path);
                    }
                    catch (Exception)
                    {
                        OnLog("Error downloading file from " + file.Address);
                        //Do whatever you want. Maybe write a debug message, stop download, etc.
                    }
                }
            }
        }

        void cli_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //update processed and remaining numbers
            Interlocked.Decrement(ref processesing);
            Interlocked.Increment(ref processed);

            //report the progress
            if (!CANCEL)
            {                
                double percent = 100.0 * processed / totalCount;
                OnProgress((int)percent);
                OnLog(string.Format("Downloading: {0:00}%, remaining: {1}", percent, processesing));
            }
            else
                OnLog(String.Format("Canceled... wait for running downloads to finish. {0} item(s) remain", processesing));

            //download more files
            StartDownload();
        }
    }
}
