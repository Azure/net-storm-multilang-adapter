// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Dotnet.Storm.Adapter.Test
{
    public class TestOutput
    {
        public string ComponentId { get; set; }

        public long Task { get; set; }

        public string Stream { get; set; }

        public List<object> Tuple { get; set; }

        public bool NeedTaskIds { get; set; }
    }
}
