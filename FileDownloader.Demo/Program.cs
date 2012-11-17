using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloader.Demo
{
    class Program
    {
        /// <summary>
        /// Test program for FileDownloader class.
        /// </summary>
        static void Main(string[] args)
        {
            //Test .Net 4.5 version
            Test();

            //Test .Net 3.5 version
            Test35();
        }

        private static void Test()
        {
            //Initialize the logger and its events
            var logger = new EventBasedLogger();
            logger.Log += WriteLog;
            logger.Debug += WriteDebug;
            logger.Progress += WriteProgress;

            //create the downloader object and set its logger
            var downloader = new FileDownloader { Logger = logger };

            //enqueu two images
            var items = new Queue<DownloadItem>();
            items.Enqueue(new DownloadItem { Address = new Uri("https://www.google.com/images/srpr/logo3w.png"), Path = "google.png" });
            items.Enqueue(new DownloadItem
            {
                Address = new Uri("http://www.bing.com/community/cfs-filesystemfile.ashx/__key/CommunityServer-Blogs-Components-WeblogFiles/00-00-00-41-77-metablogapi/6644.b_2D00_fund_2D00_logo_5F00_3669B89F.png"),
                Path = "bing.png"
            });

            //create cancellation token source
            using (var source = new CancellationTokenSource())
            {
                //Start download...
                // Note: You can use async, instead of the synced version
                downloader.StartDownload(items, source.Token);
            }
        }
                
        /// <summary>
        /// Test program for FileDownloader class.
        /// </summary>
        private static void Test35()
        {
            //Initialize the logger and its events
            var logger = new FileDownloader35.EventBasedLogger();
            logger.Log += WriteLog;
            logger.Debug += WriteDebug;
            logger.Progress += WriteProgress;

            //create the downloader object and set its logger
            var downloader = new FileDownloader35.FileDownloader { Logger = logger };

            //enqueu two images
            var items = new Queue<FileDownloader35.DownloadItem>();
            items.Enqueue(new FileDownloader35.DownloadItem { Address = new Uri("https://www.google.com/images/srpr/logo3w.png"), Path = "google.png" });
            items.Enqueue(new FileDownloader35.DownloadItem
            {
                Address = new Uri("http://www.bing.com/community/cfs-filesystemfile.ashx/__key/CommunityServer-Blogs-Components-WeblogFiles/00-00-00-41-77-metablogapi/6644.b_2D00_fund_2D00_logo_5F00_3669B89F.png"),
                Path = "bing.png"
            });

            downloader.StartDownload(items);

            //wait for download to finish.
            // Usually, because the operation is async, we won't need something like this
            while (!finished)
            {
                Thread.Sleep(100);
            }
        }

        //Write progress to console
        static bool finished = false;
        static void WriteProgress(int percent)
        {
            if (percent == 100)
                finished = true;
            Console.WriteLine("Progress: {0}%", percent);
        }

        //Write debug message to console
        static void WriteDebug(string message)
        {
            Console.WriteLine("Debug: {0}", message);
        }

        //Write log message to console
        static void WriteLog(string message)
        {
            Console.WriteLine("Log: {0}", message);
        }
    }
}
