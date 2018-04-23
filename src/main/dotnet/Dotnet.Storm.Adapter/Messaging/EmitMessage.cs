// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;

namespace Dotnet.Storm.Adapter.Messaging
{
    abstract class EmitMessage : OutMessage
    {
        [JsonProperty("command")]
        public readonly string Command = "emit";
    }
}
