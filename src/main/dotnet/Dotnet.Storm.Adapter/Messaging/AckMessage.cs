// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;

namespace Dotnet.Storm.Adapter.Messaging
{
    class AckMessage : OutMessage
    {
        [JsonProperty("command")]
        public const string Command = "ack";

        [JsonProperty("id")]
        public string Id { get; private set; }

        public AckMessage(string id)
        {
            Id = id;
        }
    }
}
