// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Reflection;
using CommandLine;
using Dotnet.Storm.Adapter.Channels;
using Dotnet.Storm.Adapter.Components;
using Dotnet.Storm.Adapter.Logging;
using Dotnet.Storm.Adapter.Serializers;
using log4net;
using log4net.Config;
using log4net.Core;

namespace Dotnet.Storm.Adapter
{
    class Program
    {
        private readonly static ILog Logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            Storm.CreateComponent(args);
        }
    }
}
