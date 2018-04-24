// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Dotnet.Storm.Adapter.Messaging;

namespace Dotnet.Storm.Adapter.Test
{
    public class BoltOutput : TestOutput
    {
        public List<string> Anchors { get; set; }

        internal BoltOutput(BoltTuple bt, string compId)
        {
            Anchors = bt.Anchors;
            this.ComponentId = compId;
            this.Stream = bt.Stream;
            this.Task = bt.Task;
            this.Tuple = bt.Tuple;
            this.NeedTaskIds = bt.NeedTaskIds;
        }
    }
}
