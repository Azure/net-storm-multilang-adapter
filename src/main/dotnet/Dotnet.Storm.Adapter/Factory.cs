// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using System;
using System.IO;
using System.Reflection;

using Dotnet.Storm.Adapter.Channels;
using Dotnet.Storm.Adapter.Components;
using Dotnet.Storm.Adapter.Logging;

using CommandLine;

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Dotnet.Storm.Adapter
{
    public static class Factory
    {
        private readonly static ILog Logger = LogManager.GetLogger(typeof(Factory));

        public static void RunComponent(string[] args)
        {
            Hierarchy repository = ConfigureLogging();

            repository.CleanAppenders();

            Options options = args.ReadOptions();

            Channel channel = new StandardChannel();

            Component component = CreateComponent(options, channel);

            repository.AppendStorm(channel);

            component.Start();
        }

        private static Component CreateComponent(Options options, Channel channel)
        {
            Type type = Assembly.GetEntryAssembly().GetType(options.Class, true);
            Component component = (Component)Activator.CreateInstance(type);
            component.Connect(options.Arguments, channel);
            return component;
        }

        private static Hierarchy ConfigureLogging()
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

            return (Hierarchy)repository;
        }

        private static Options ParseOptions(string[] args)
        {
            Options result = null;

            var parser = new Parser(with => with.EnableDashDash = true).ParseArguments<Options>(args);
                parser.WithParsed(options => { result = options; });

            Logger.Info($"Current parameters: className={result.Class}, arguments={result.Arguments}.");

            return result;
        }

        private static void CleanAppenders(this Hierarchy hierarchy)
        {
            foreach (IAppender appender in hierarchy.GetAppenders())
            {

                if (appender.GetType().IsAssignableFrom(typeof(StormAppender)) || appender.GetType().IsAssignableFrom(typeof(ConsoleAppender)))
                {
                    Logger.Debug($"Removing {appender.GetType().Name} appender.");
                    hierarchy.Root.RemoveAppender(appender);
                }
            }
        }

        private static void AppendStorm(this Hierarchy hierarchy, Channel channel)
        {
            Logger.Debug($"Enabling storm logging mechanism.");
            hierarchy.Root.AddAppender(new StormAppender(channel) { Layout = new PatternLayout("%m") });
            hierarchy.Configured = true;
        }

        private static Options ReadOptions(this string[] options)
        {
            Options result = null;

            var parser = new Parser(with => with.EnableDashDash = true).ParseArguments<Options>(options);
            parser.WithParsed(opts => { result = opts; });

            Logger.Info($"Current parameters: className={result.Class}, arguments={result.Arguments}.");

            return result;
        }
    }
}
