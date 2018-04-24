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
            this.Stream = bt.Stream;
            this.Task = bt.Task;
            this.Tuple = bt.Tuple;
            this.NeedTaskIds = bt.NeedTaskIds;
            this.ComponentId = Component.Context.ComponentId;
        }
    }
}
