using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloader
{
    /// <summary>
    /// Fire events whenever a log message is happening
    /// </summary>
    public class EventBasedLogger : ILogger
    {
        /// <summary>
        /// Delegate for writing log or debug messages
        /// </summary>
        /// <param name="message">The log or debug message</param>
        public delegate void LogHandler(string message);

        /// <summary>
        /// Log event. A new log is pushed
        /// </summary>
        public event LogHandler Log;

        /// <summary>
        /// Debug event. A new debug message is pushed
        /// </summary>
        public event LogHandler Debug;

        /// <summary>
        /// Delegate for reporting download progress
        /// </summary>
        /// <param name="percent">Download progress percentage</param>
        public delegate void ProgressHandler(int percent);

        /// <summary>
        /// Progress event. The progress percentage has been changed
        /// </summary>
        public event ProgressHandler Progress;

        /// <summary>
        /// Write a log message
        /// </summary>
        /// <param name="message">Log message</param>
        public void WriteLog(string message)
        {
            if (Log != null)
            {
                Log(message);
            }
        }

        /// <summary>
        /// Write a debug message
        /// </summary>
        /// <param name="message">Debug message</param>
        public void WriteDebug(string message)
        {
            if (Debug != null)
            {
                Debug(message);
            }
        }

        /// <summary>
        /// Report the progress percentage
        /// </summary>
        /// <param name="percent">Progress percentage</param>
        public void ReportProgress(int percent)
        {
            if (Progress != null)
            {
                Progress(percent);
            }
        }
    }
}
