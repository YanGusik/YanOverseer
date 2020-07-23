using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using YanOverseer.Services;

namespace YanOverseer
{
    public class ProgramModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigLoader>().As<IConfigLoader>();

            // TODO: Add some useful code here
            // NOTE: You need to create a singletone class Config and fill it with the service: ConfigLoader
            var config = (new ConfigLoader()).Load();
            builder.RegisterInstance(config);
            builder.Register(c => config).AsSelf().SingleInstance();

            builder.RegisterType<Bot>().AsSelf().SingleInstance();
            builder.RegisterType<LoggingMessage>().As<ILoggingMessage>();
        }
    }
}