// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;

namespace Dotnet.Storm.Adapter.Messaging
{
    class FailMessage : OutMessage
    {
        [JsonProperty("command")]
        public const string Command = "fail";

        [JsonProperty("id")]
        public string Id { get; private set; }

        public FailMessage(string id)
        {
            Id = id;
        }
    }
}
