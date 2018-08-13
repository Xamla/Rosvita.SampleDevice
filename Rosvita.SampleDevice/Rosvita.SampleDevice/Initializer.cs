using Microsoft.Extensions.Logging;
using Rosvita.RosMonitor;
using System.Reflection;
using Xamla.Graph;
using Microsoft.Extensions.DependencyInjection;

[assembly: GraphRuntimeInitializer(typeof(Rosvita.SampleDevice.Initializer))]

namespace Rosvita.SampleDevice
{
    class Initializer
        : IGraphRuntimeInitializer
    {
        public void Initialize(IGraphRuntime runtime)
        {
            runtime.ModuleFactory.RegisterAllModules(Assembly.GetExecutingAssembly());

            StaticModules.Init(
                runtime.ServiceLocator.GetService<ILoggerFactory>(),
                runtime.ServiceLocator.GetService<IRosClientLibrary>()
            );
        }
    }

    public static partial class StaticModules
    {
        static IRosClientLibrary rosClient;

        internal static void Init(ILoggerFactory loggerFactory, IRosClientLibrary rosClient)
        {
            StaticModules.rosClient = rosClient;
        }
    }
}
