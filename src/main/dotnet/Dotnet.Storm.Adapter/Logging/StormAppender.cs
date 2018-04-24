// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Dotnet.Storm.Adapter.Channels;
using Dotnet.Storm.Adapter.Messaging;
using log4net;
using log4net.Appender;
using log4net.Core;

namespace Dotnet.Storm.Adapter.Logging
{
    public class StormAppender : AppenderSkeleton
    {
        internal bool Enabled { get; set; }

        internal Channel Channel { get; set; }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (Enabled)
            {
                string message = RenderLoggingEvent(loggingEvent);
                LogLevel level = GetStormLevel(loggingEvent.Level);

                Channel.Send(new LogMessage(message, level));
            }
        }

        public static LogLevel GetStormLevel(Level level)
        {
            switch (level.Name)
            {
                case "FINE":
                case "TRACE":
                case "FINER":
                case "VERBOSE":
                case "FINEST":
                case "ALL":
                    return LogLevel.TRACE;
                case "log4net:DEBUG":
                case "DEBUG":
                    return LogLevel.DEBUG;
                case "OFF":
                case "INFO":
                    return LogLevel.INFO;
                case "WARN":
                case "NOTICE":
                    return LogLevel.WARN;
                case "EMERGENCY":
                case "FATAL":
                case "ALERT":
                case "CRITICAL":
                case "SEVERE":
                case "ERROR":
                    return LogLevel.ERROR;
                default:
                    return LogLevel.INFO;
            }
        }

        public static Level GetLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.TRACE:
                    return Level.Trace;
                case LogLevel.DEBUG:
                    return Level.Debug;
                case LogLevel.INFO:
                    return Level.Info;
                case LogLevel.WARN:
                    return Level.Warn;
                case LogLevel.ERROR:
                    return Level.Error;
                default:
                    return Level.Info;
            }
        }
    }
}
