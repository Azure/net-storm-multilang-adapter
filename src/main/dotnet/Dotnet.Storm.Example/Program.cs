// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using log4net;
using Dotnet.Storm.Adapter;

namespace Dotnet.Storm.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Dotnet.Storm.Adapter.Storm.CreateComponent(args);
        }
    }
}
