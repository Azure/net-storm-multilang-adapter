// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Dotnet.Storm.Adapter.Logging;
using Newtonsoft.Json;

namespace Dotnet.Storm.Adapter.Messaging
{
    class LogMessage : OutMessage
    {
        public LogMessage(string message, LogLevel level)
        {
            Message = message;
            Level = level;
        }

        [JsonProperty("command")]
        public const string Command = "log";

        [JsonProperty("msg")]
        public string Message { get; private set; }

        [JsonProperty("level")]
        public LogLevel Level { get; private set; }
    }
}
