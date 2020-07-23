using System;
using Autofac;

namespace YanOverseer
{
    class Program
    {
        public static IContainer Container;

        static void Main(string[] args)
        {
            // DI
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ProgramModule>();
            Container = containerBuilder.Build();
            
            var bot = Container.Resolve<Bot>();
            bot.RunBotAsync().GetAwaiter().GetResult();
        }
    }
}
