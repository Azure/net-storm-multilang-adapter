using System;
using System.Collections.Generic;
using System.Text;
using Dotnet.Storm.Adapter.Messaging;

namespace Dotnet.Storm.Adapter.Test
{
    public class SpoutOutput : TestOutput
    {
        public string Id { get; set; }
   
        internal SpoutOutput(SpoutTuple bt)
        {
            Id = bt.Id;
            this.Stream = bt.Stream;
            this.Task = bt.Task;
            this.Tuple = bt.Tuple;
            this.NeedTaskIds = bt.NeedTaskIds;
        }
    }
}
