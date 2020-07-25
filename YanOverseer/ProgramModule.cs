using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Microsoft.EntityFrameworkCore;
using YanOverseer.BLL.Interfaces;
using YanOverseer.BLL.Services;
using YanOverseer.DAL;
using YanOverseer.Services;
using YanOverseer.Services.Interfaces;

namespace YanOverseer
{
    public class ProgramModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // DB
            builder.RegisterType<MainContext>().InstancePerLifetimeScope();

            builder.RegisterType<ProfileService>().As<IProfileService>();
            builder.RegisterType<MessageService>().As<IMessageService>();
            builder.RegisterType<StatisticsService>().As<IStatisticsService>();

            builder.RegisterType<ConfigLoader>().As<IConfigLoader>();

            // TODO: Add some useful code here
            // NOTE: You need to create a singletone class Config and fill it with the service: ConfigLoader
            var config = (new ConfigLoader()).Load();
            builder.RegisterInstance(config);
            builder.Register(c => config).AsSelf().SingleInstance();

            builder.RegisterType<Bot>().AsSelf().SingleInstance();
            builder.RegisterType<LoggingMessage>().As<ILoggingMessage>();
            builder.RegisterType<StatisticsProfile>().As<IStatisticsProfile>();
            builder.RegisterType<ServerSettingsService>().As<IServerSettingsService>();
        }
    }
}