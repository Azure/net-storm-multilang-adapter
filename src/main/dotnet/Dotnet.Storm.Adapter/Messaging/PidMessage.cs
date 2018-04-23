// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;

namespace Dotnet.Storm.Adapter.Messaging
{
    class PidMessage : OutMessage
    {
        public PidMessage(int pid)
        {
            Pid = pid;
        }

        [JsonProperty("pid")]
        public int Pid { get; set; }
    }
}
