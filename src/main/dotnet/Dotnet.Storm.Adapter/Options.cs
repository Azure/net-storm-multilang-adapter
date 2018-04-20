﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using CommandLine;

namespace Dotnet.Storm.Adapter
{
    public class Options
    {

        [Option('c', "class", Required = true, HelpText = "Component class name to be instantiated.")]
        public string Class { get; set; }

        [Option('a', "assembly", Required = true, HelpText = "The assembly which contains the component class.")]
        public string Assembly { get; set; }

        [Option('p', "parameters", Required = false, HelpText = "Component command line parameters.")]
        public string Arguments { get; set; }

        [Option('s', "serializer", Required = false, Default = "json", HelpText = "Message serializer. Only JSON serializer is availiable now.")]
        public string Serializer { get; set; }

        [Option('h', "channel", Required = false, Default = "std", HelpText = "Message exchange channel. Only STD channel is availiable now.")]
        public string Channel { get; set; }
    }
}
