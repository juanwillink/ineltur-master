using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

namespace Ineltur.WebService.Clases
{
    public static class Tracker
    {
        /*
            TABLA LOG
	            IDLOG
	            TYPE
	            APPLICATION
	            PROCESS
	            THREAD
	            METHOD
	            LINE	(TYPE=ERROR/EXCEPTION)
	            DETAIL
         */
        public enum TraceType { Info, Error, Fatal }

        private static string TAB = new string(' ', 50);

        public static string SEPARATOR { get { return TAB + new string('-', 75); } }
        public static string DOUBLE_SEPARATOR { get { return new string('=', 90); } }

        //public static void WriteTrace()
        //{
        //    WriteTrace(string.Empty);
        //}

        //public static void WriteTrace(string trace)
        //{
        //    WriteTrace(TraceType.Info, trace, false);
        //}

        //public static void WriteTrace(TraceType traceType, string trace)
        //{
        //    WriteTrace(traceType, trace, false);
        //}

        //public static void WriteTrace(string trace, bool additionalNewLine)
        //{
        //    WriteTrace(TraceType.Info, trace, additionalNewLine);
        //}

        public static void WriteTrace(string identifier, string trace = "", bool additionalNewLine = true, TraceType traceType = TraceType.Info)
        {
            var logger = LogManager.GetCurrentClassLogger();

            if (traceType != TraceType.Fatal)
                trace = trace.Replace("\n", "\n" + TAB);

            if (additionalNewLine)
                trace += "\n";

            LogEventInfo loggerEvent;

            switch (traceType)
            {
                case TraceType.Info:
                    loggerEvent = new LogEventInfo(LogLevel.Info, "", trace);
                    loggerEvent.Properties["LogTypeId"] = 1;
                    loggerEvent.Properties["SessionIdentifier"] = identifier;
                    logger.Log(loggerEvent);
                    //logger.Info(trace);
                    break;
            
                case TraceType.Error:
                    loggerEvent = new LogEventInfo(LogLevel.Error, "", trace);
                    loggerEvent.Properties["LogTypeId"] = 2;
                    loggerEvent.Properties["SessionIdentifier"] = identifier;
                    logger.Log(loggerEvent);
                    //logger.Error(trace); 
                    break;
            
                case TraceType.Fatal:
                    loggerEvent = new LogEventInfo(LogLevel.Fatal, "", trace);
                    loggerEvent.Properties["LogTypeId"] = 2;
                    loggerEvent.Properties["SessionIdentifier"] = identifier;
                    logger.Log(loggerEvent);
                    //logger.Fatal(trace); 
                    break;
            }
        }
    }
}