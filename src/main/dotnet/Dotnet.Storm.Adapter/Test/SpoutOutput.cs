using Dotnet.Storm.Adapter.Messaging;
using Dotnet.Storm.Adapter.Components;

namespace Dotnet.Storm.Adapter.Test
{
    public class SpoutOutput : TestOutput
    {
        public string Id { get; set; }
   
        internal SpoutOutput(SpoutTuple st)
        {
            Id = st.Id;
            this.Stream = st.Stream;
            this.Task = st.Task;
            this.Tuple = st.Tuple;
            this.NeedTaskIds = st.NeedTaskIds;
            this.ComponentId = Component.Context.ComponentId;
        }
    }
}
