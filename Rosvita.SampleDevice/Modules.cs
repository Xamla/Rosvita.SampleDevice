using System.Threading.Tasks;
using Uml.Robotics.Ros;
using Xamla.Graph;
using Xamla.Graph.MethodModule;

namespace Rosvita.SampleDevice
{
    using sample_device = Messages.sample_device;

    public static partial class StaticModules
    {
        const string DEFAULT_REQUEST_WEIGHT_NAME = "/sample_device/request_weight";
        const string DEFAULT_TARE_NAME = "/sample_device/tare";
        const string DEFAULT_CALIBRATE_NAME = "/sample_device/calibrate";

        [StaticModule(ModuleType = "SampleDevice.RequestWeight", Flow = true)]
        public static async Task<double> RequestWeight(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_REQUEST_WEIGHT_NAME)] string serviceName)
        {
            using (var client = rosClient.GlobalNodeHandle.ServiceClient<sample_device.GetWeight>(serviceName))
            {
                var srv = new sample_device.GetWeight();
                if (!await client.CallAsync(srv))
                    throw new ServiceCallFailedException(serviceName);

                return srv.resp.weight;
            }
        }

        [StaticModule(ModuleType = "SampleDevice.Tare", Flow = true)]
        public static async Task Tare(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_TARE_NAME)] string serviceName)
        {
            using (var client = rosClient.GlobalNodeHandle.ServiceClient<Messages.std_srvs.Empty>(serviceName))
            {
                var srv = new Messages.std_srvs.Empty();
                if (!await client.CallAsync(srv))
                    throw new ServiceCallFailedException(serviceName);
            }
        }

        [StaticModule(ModuleType = "SampleDevice.Calibrate", Flow = true)]
        public static async Task Calibrate(
           [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_CALIBRATE_NAME)] string serviceName)
        {
            using (var client = rosClient.GlobalNodeHandle.ServiceClient<Messages.std_srvs.Empty>(serviceName))
            {
                var srv = new Messages.std_srvs.Empty();
                if (!await client.CallAsync(srv))
                    throw new ServiceCallFailedException(serviceName);
            }
        }
    }
}
