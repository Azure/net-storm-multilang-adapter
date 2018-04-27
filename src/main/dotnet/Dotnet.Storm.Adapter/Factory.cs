// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using CommandLine;
using Dotnet.Storm.Adapter.Channels;
using Dotnet.Storm.Adapter.Components;
using Dotnet.Storm.Adapter.Logging;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using System.Reflection;

namespace Dotnet.Storm.Adapter
{
    public static class Factory
    {
        private readonly static ILog Logger = LogManager.GetLogger(typeof(Factory));

        public static void RunComponent(string[] args)
        {
            Hierarchy repository = (Hierarchy)ConfigureLogging();

            foreach (IAppender appender in repository.GetAppenders())
            {

                if (appender.GetType().IsAssignableFrom(typeof(StormAppender)) || appender.GetType().IsAssignableFrom(typeof(ConsoleAppender)))
                {
                    Logger.Debug($"Removing {appender.GetType().Name} appender.");
                    repository.Root.RemoveAppender(appender);
                }
            }

            Options options = ParseArgumants(args);

            Channel channel = new StandardChannel();

            Component component = CreateComponent(options.Class, options.Arguments, channel);

            Logger.Debug($"Enabling storm logging mechanism.");
            repository.Root.AddAppender(new StormAppender(channel) { Layout = new PatternLayout("%m") });

            component.Start();
        }

        private static Component CreateComponent(string className, string arguments, Channel channel)
        {
            Type type = Assembly.GetEntryAssembly().GetType(className, true);
            Component component = (Component)Activator.CreateInstance(type);
            component.Connect(arguments, channel);
            return component;
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
