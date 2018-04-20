// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dotnet.Storm.Adapter.Messaging
{
    class ExecuteTuple : InMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("comp")]
        public string Component { get; set; }

        [JsonProperty("task")]
        public string TaskId { get; set; }

        [JsonProperty("stream")]
        public string Stream { get; set; }

        [JsonProperty("tuple")]
        public List<object> Tuple { get; set; }

    }
}
