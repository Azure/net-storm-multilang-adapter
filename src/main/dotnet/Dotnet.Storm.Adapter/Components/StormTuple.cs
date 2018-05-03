// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Dotnet.Storm.Adapter.Messaging;
using System.Collections.Generic;

namespace Dotnet.Storm.Adapter.Components
{
    public class StormTuple
    {
        internal StormTuple(ExecuteTuple tuple)
        {
            Id = tuple.Id;
            Component = tuple.Component;
            TaskId = tuple.TaskId;
            Stream = tuple.Stream;
            Tuple = tuple.Tuple;
        }

        public StormTuple(string id, string component, string taskId, string stream, List<object> tuple)
        {
            Id = id;
            Component = component;
            TaskId = taskId;
            Stream = stream;
            Tuple = tuple;
        }

        public string Id { get; internal set; }

        public string Component { get; internal set; }

        public string TaskId { get; internal set; }

        public string Stream { get; internal set; }

        public List<object> Tuple { get; internal set; }
    }
}
