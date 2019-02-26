using Rosvita.Parsers.Launch;
using Rosvita.Project;
using Rosvita.Project.Generators;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using Xamla.Types;

namespace Rosvita.SampleDevice
{
    [ConfigurationGenerator]
    public class Generator
        : ConfigurationGeneratorBase
    {
        public Generator()
            : base(nameof(Generator), GeneratorPriority.LaunchFileGenerator)
        {
        }

        protected override void GenerateInternal(IRosvitaRuntime runtime, IConfiguration configuration, IGeneratorContext context, IGeneratorMessageCollection messages)
        {
            var project = runtime.Project.Current;
            if (project == null)
                throw new XamlaException("Project must not be null.", XamlaError.ArgumentNull);

            // find matching components for this generator
            var deviceConfigurations = configuration.Where(x => x.ConfigurationType == Configuration.ConfigurationType)
                .OfType<IStandardComponent>()
                .Select(x => (Configuration)x.GetConfiguration())
                .Where(x => x.Enabled)
                .ToList();

            foreach (var device in deviceConfigurations)
            {
                LaunchFileDocument launchFile = GenerateLaunchFile(device);

                string launchFilename = $"sample_device_{device.Name}.launch";
                string launchTempFilePath = Path.Combine(context.TempOutputDirecotry, launchFilename);
                string launchFilePath = Path.Combine(context.FinalOutputDirecotry, launchFilename);
                launchFile.Save(launchTempFilePath);
                context.LaunchFilesToInclude.Add(launchFilePath);

                if (device.EnableSystemMonitoring)
                {
                    context.SysMonElements.Add(new SystemMonitorNode { Name = device.Name, Timeout = Math.Max(2.0, 2.0 / device.Rate), Type = SystemMonitorNodeType.Heartbeat });
                }

                messages.AddInfo(this, $"Launch file'{project.MakeRelative(launchFilePath)}' written.");
            }
        }

        /*
<launch>
  <node name="sample_device" pkg="sample_device" type="sample_device.py" respawn="true" respawn_delay="4" >
    <param name="telnet_host" value="192.168.1.116"/>
    <param name="telnet_port" value="2001"/>

    <param name="rate" value="10"/>
    <param name="read_timeout" value="4"/>
  </node>
</launch>
         */
        private LaunchFileDocument GenerateLaunchFile(Configuration device)
        {
            var launchFile = new LaunchFileDocument();

            var node = new NodeElement
            {
                Name = device.Name,
                Pkg = "sample_device",
                Type = "sample_device.py",
                Output = "screen"
            };

            node.SetRespawnSettings(device.Respawn, device.RespawnDelay);

            node.AddParam("telnet_host", value: device.Host);
            node.AddParam("telnet_port", value: XmlConvert.ToString(device.Port));
            node.AddParam("rate", value: XmlConvert.ToString(device.Rate));
            node.AddParam("read_timeout", value: XmlConvert.ToString(device.ReadTimeout));

            launchFile.AddNode(node);

            return launchFile;
        }
    }
}
