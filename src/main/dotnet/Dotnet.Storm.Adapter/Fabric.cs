// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using CommandLine;
using Dotnet.Storm.Adapter.Channels;
using Dotnet.Storm.Adapter.Components;
using Dotnet.Storm.Adapter.Logging;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using System.Reflection;

namespace Dotnet.Storm.Adapter
{
    public static class Fabric
    {
        private readonly static ILog Logger = LogManager.GetLogger(typeof(Fabric));

        public static void RunComponent(string[] args)
        {
            ILoggerRepository repository = ConfigureLogging();

            Options options = ParseArgumants(args);

            DisableStormLogger(repository);

            Component component = CreateComponent(options.Class, options.Arguments);

            EnableStormLogger(repository);

            component.Start();
        }

        private static void EnableStormLogger(ILoggerRepository repository)
        {
            Logger.Debug($"Enabling storm logging mechanism.");
            foreach (IAppender appender in ((Hierarchy)repository).GetAppenders())
            {
                if (appender.GetType().IsAssignableFrom(typeof(StormAppender)))
                {
                    ((StormAppender)appender).Enabled = true;
                }
            }
        }

        private static Component CreateComponent(string className, string arguments)
        {
            Type type = Type.GetType(className, true);
            Component component = (Component)Activator.CreateInstance(type);
            component.Connect(arguments, new StandardChannel());
            return component;
        }

        private static void DisableStormLogger(ILoggerRepository repository)
        {
            foreach (IAppender appender in repository.GetAppenders())
            {
                if (appender.GetType().IsAssignableFrom(typeof(StormAppender)))
                {
                    Logger.Debug($"Disabling storm logging mechanism.");
                    ((StormAppender)appender).Enabled = false;
                }
            }
        }

        private static ILoggerRepository ConfigureLogging()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            string config = Path.Combine(Path.GetDirectoryName(entryAssembly.Location), "log4net.config");

            var repository = LogManager.GetRepository(entryAssembly);
            if (File.Exists(config))
            {
                XmlConfigurator.Configure(repository, new FileInfo(config));
            }
            else
            {
                XmlConfigurator.Configure(repository);
            }

            Logger.Debug($"Current working dir: {Environment.CurrentDirectory}.");

            return repository;
        }

        private static Options ParseArgumants(string[] args)
        {
            Options result = null;

            var parser = new Parser(with => with.EnableDashDash = true).ParseArguments<Options>(args);
                parser.WithParsed(options => { result = options; });

            Logger.Info($"Current parameters: className={result.Class}, arguments={result.Arguments}.");

            return result;
        }
    }
}
