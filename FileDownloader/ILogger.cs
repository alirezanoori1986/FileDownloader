using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloader
{
    /// <summary>
    /// Represents any logger which can report log, debug and progress
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Write a log message
        /// </summary>
        /// <param name="message">Log message</param>
        void WriteLog(string message);

        /// <summary>
        /// Write a debug message
        /// </summary>
        /// <param name="message">Debug message</param>
        void WriteDebug(string message);

        /// <summary>
        /// Report the progress percentage
        /// </summary>
        /// <param name="percent">Progress percentage</param>
        void ReportProgress(int percent);
    }
}
