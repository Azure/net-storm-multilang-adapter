// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Dotnet.Storm.Adapter.Messaging;
using Dotnet.Storm.Adapter.Components;

namespace Dotnet.Storm.Adapter.Test
{
    public class BoltOutput : TestOutput
    {
        public List<string> Anchors { get; set; }

        internal BoltOutput(BoltTuple bt)
        {
            Anchors = bt.Anchors;
            Stream = bt.Stream;
            Task = bt.Task;
            Tuple = bt.Tuple;
            NeedTaskIds = bt.NeedTaskIds;
        }
    }
}
