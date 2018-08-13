using Rosvita.Project;
using System.Collections.Generic;

namespace Rosvita.SampleDevice
{
    [SensorComponent(
        Name = ConfigurationType,
        DisplayName = "Sample Device",
        Manufacturer = "Sample Comany",
        Version = "1.0.0.0",
        Description = "This is a .net sample device configuration Configuration for the ",
        Hidden = false,
        DefaultNodeName = "sample_device",
        DefaultRespawn = true,
        DefaultRespawnDelay = 1
    )]
    public class Configuration
        : RosNodeConfigurationBase
    {
        public const string ConfigurationType = "SampleDevice";

        const string DEFAULT_TELNET_HOST = "192.168.1.116";
        const int DEFAULT_PORT = 2001;
        const double DEFAULT_RATE = 10;
        const int DEFAULT_READ_TIMEOUT = 4;
        const int DEFAULT_ACTION_TIMEOUT = 90;

        public static List<IconImage> Icons => new List<IconImage>
        {
            new IconImage { Type = "thumb", Path = "/images/part-thumbs/sensor.png" }
        };

        public static ComponentMaintainer Maintainer => new ComponentMaintainer
        {
            Organization = "Xamla",
            Email = "support@xamla.com",
            Url = "http://www.xamla.com/"
        };

        public Configuration()
        {
        }

        [ComponentProperty(DisplayName = "Telnet Host", DefaultValue = DEFAULT_TELNET_HOST)]
        public string Host { get; set; } = DEFAULT_TELNET_HOST;

        [ComponentProperty(DisplayName = "Port", DefaultValue = DEFAULT_PORT)]
        public int Port { get; set; } = DEFAULT_PORT;

        [ComponentProperty(DisplayName = "Rate (Hz)", DefaultValue = DEFAULT_RATE)]
        public double Rate { get; set; } = DEFAULT_RATE;

        [ComponentProperty(DisplayName = "Read Timeout (s)", DefaultValue = DEFAULT_READ_TIMEOUT)]
        public int ReadTimeout { get; set; } = DEFAULT_READ_TIMEOUT;

        [ComponentProperty(DisplayName = "System Monitoring", DefaultValue = true)]
        public bool EnableSystemMonitoring { get; set; } = true;
    }
}
