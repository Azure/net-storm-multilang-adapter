using CommandLine;
using Dotnet.Storm.Adapter.Channels;
using Dotnet.Storm.Adapter.Components;
using Dotnet.Storm.Adapter.Logging;
using Dotnet.Storm.Adapter.Serializers;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using System.Reflection;

namespace Dotnet.Storm.Adapter
{
    public static class Storm
    {
        private readonly static ILog Logger = LogManager.GetLogger(typeof(Program));

        public static void CreateComponent(string[] args)
        {
            var parser = new Parser(with => with.EnableDashDash = true).ParseArguments<Options>(args);

            string className = null;
            string assemblyName = null;
            string arguments = null;
            string serializer = null;
            string channel = null;

            parser.WithParsed(options =>
            {
                className = options.Class;
                assemblyName = options.Assembly;
                arguments = options.Arguments;
                serializer = options.Serializer;
                channel = options.Channel;
            });

            ConfigureLogging();

            Logger.Debug($"Current working dir: {Environment.CurrentDirectory}.");

            Logger.Debug($"Initialize channel.");
            Channel.Instance = new StandardChannel { Serializer = new JsonSerializer() };

            Logger.Debug($"Loading assembly: {assemblyName}.");
            Assembly assembly = GetAssembly(assemblyName);
            Type type = assembly.GetType(className, true);

            Logger.Debug($"Trying to create instance of {type.Name}.");
            Component component = (Component)Activator.CreateInstance(type);

            Logger.Debug($"Setting up the arguments {arguments}.");
            component.SetArguments(arguments);

            Logger.Debug($"Executing handshake protocol.");
            component.Connect();

            EnableLogging();

            Logger.Debug($"Starting component.");
            component.Start();
        }

        private static void EnableLogging()
        {
            Logger.Debug($"Enabling storm logging mechanism.");
            Hierarchy repository = (Hierarchy)LogManager.GetRepository(Assembly.GetEntryAssembly());

            foreach (IAppender appender in repository.GetAppenders())
            {
                if (appender.GetType().IsAssignableFrom(typeof(StormAppender)))
                {
                    ((StormAppender)appender).Enabled = true;
                }
            }
        }

        private static void ConfigureLogging()
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

            foreach (IAppender appender in repository.GetAppenders())
            {
                if (appender.GetType().IsAssignableFrom(typeof(StormAppender)))
                {
                    ((StormAppender)appender).Enabled = false;
                }
            }
        }

        private static Assembly GetAssembly(string assemblyName)
        {
            string directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string fileName = assemblyName.EndsWith(".dll") ? assemblyName : assemblyName + ".dll";

            return Assembly.Load(File.ReadAllBytes(Path.Combine(directory, fileName)));
        }
    }
}
