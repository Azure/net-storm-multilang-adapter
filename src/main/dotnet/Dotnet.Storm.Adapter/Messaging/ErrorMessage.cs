// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;

namespace Dotnet.Storm.Adapter.Messaging
{
    class ErrorMessage : OutMessage
    {
        [JsonProperty("command")]
        public const string Command = "error";

        [JsonProperty("msg")]
        public string Message { get; }

        public ErrorMessage(string message)
        {
            Message = message;
        }
    }
}
