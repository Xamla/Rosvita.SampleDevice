# Rosvita.SampleDevice

This sample shows the .NET Core parts of a fictive sensor device (a scale) for [Rosvita](http://www.rosvita.com/).
The device has a configuration which is editable from inside Rosvita and generates a launch file and system
monitoring configuration. Additionally it provides three simple modules (RequestWeight, Tare, Calibrate) for
the Rosvita Workflow editor which call custom ROS services.

All required Xamla nuget packages are part of this repository and can be found in the
[RosvitaNuGetPackages](https://github.com/Xamla/Rosvita.SampleDevice/tree/master/RosvitaNuGetPackages) folder.
The SampleDevice project can be compiled with dotnet core 2.1 SDK. For editing we recommend either Visual Studio
2017 on Windows or Visual Studio Code on Linux. For installing the dotnet core 2.1 SDK please visit the
official Microsoft website for [download instructions](https://www.microsoft.com/net/download/dotnet-core/2.1)
(e.g. for [Ubuntu](https://www.microsoft.com/net/download/linux-package-manager/ubuntu16-04/sdk-2.1.300)).

### Use with Visual Studio Code

Please install the C# extension ms-vscode.csharp. This makes sure you can use 'Go to definition' with
F12 and intelli-sense auto-completion is available. Explore the code by clicking `F12` or `CTLR + Left Click` everywhere.

### Compilation on Linux

1. Clone the repository to your local machine: `git clone https://github.com/Xamla/Rosvita.SampleDevice.git`
2. Go to the directory that contains the file `Rosvita.SampleDevice.csproj` e.g. `cd Rosvita.SampleDevice/Rosvita.SampleDevice/Rosvita.SampleDevice`
3. Run `dotnet build` (ignore warnings that stem from the `GetWeight.cs` file which was auto-generated with [ROS.net](https://github.com/xamla/ros.net))

## Elements of a Device Integration

A custom device integration consists of the following parts:

1. [Configuration.cs](https://github.com/Xamla/Rosvita.SampleDevice/blob/master/Rosvita.SampleDevice/Rosvita.SampleDevice/Configuration.cs):
   The configuration class declares the device parameters that will be editable in the Rosvita configuration UI.
   It can be a simple C# class with public default constructor. In this sample the configuration class was derived from
   `RosNodeConfigurationBase` which contains the properties `Id`, `Name`, `Respawn`, `RespawnDelay` which are common for
   components that configure a ROS node. A component attribute allows Rosvita to automatically discover the configuration
   classes in a device assembly. The code in this sample uses the `[SensorComponent]` attribute since a 'Scale' sensor
   component is created, alternatively there exist `[ActuatorComponent]`, `[MotionPlanning]` and `[RosComponent]`. The
   type of component attribute determines in which category of Rosvita's component catalog the component will be listed.
   Individual configuration options are specified as `public` properties decorated with `[ComponentProperty]` attributes.
   Default values for all properties can be specified. Currently (v.0.7.7) the default value has to be specified in two ways:
   declaratively via `[ComponentProperty(DefaultValue=xyz)]` and with help of the constructor (e.g. via auo-property
   initializer).

2. [Generator.cs](https://github.com/Xamla/Rosvita.SampleDevice/blob/master/Rosvita.SampleDevice/Rosvita.SampleDevice/Generator.cs):
   A configuration generator class that is provides the functionality to generate the required launch files, system
   monitoring configuration and other related system configuration files. A generator class has to be decorated with
   the `[ConfigurationGenerator]` attribute. While there can potentially be multiple instances of components that are
   added by the user to a Rosvita configuration there will only be one generator of a kind. This separation helps for
   example to create nodes that can handle multiple devices at once while allowing to configure settings for each
   individual device (e.g. multiple cameras could be handled by a camera driver). A generator class has to implement
   the `IConfigurationGenerator` interface. The simplest way to do this is to derive the generator form the helper class
   `ConfigurationGeneratorBase` which has the virtual methods `GenerateInternal` and `ValidateInternal` for overriding by
   the derived generator. Each generator has a priority which defines

3. [Initializer.cs](https://github.com/Xamla/Rosvita.SampleDevice/blob/master/Rosvita.SampleDevice/Rosvita.SampleDevice/Initializer.cs):
   The initializer class is used by the Xamla graph runtime to initialize modules in the graph host process. It implements
   the `IGraphRuntimeInitializer` interface and provides an `Initialize`method that has an argument of type `IGraphRuntime`
   which allows to access all relevant parts fo the graph runtime. The most important function of the initializer is to
   register all method modules at the module factory (see
   [Initializer.cs#L16.](https://github.com/Xamla/Rosvita.SampleDevice/blob/master/Rosvita.SampleDevice/Rosvita.SampleDevice/Initializer.cs#L16.))
   Beside the module generation the initializer allows to set up static variables and register custom type converters and
   custom serializers if neccessary. The initializer class is specified as a assembly level attribute via `[assembly: GraphRuntimeInitializer(typeof(Rosvita.SampleDevice.Initializer))]`.

4. [Modules.cs](https://github.com/Xamla/Rosvita.SampleDevice/blob/master/Rosvita.SampleDevice/Rosvita.SampleDevice/Modules.cs):
   The actual workflow editor modules implementation can be found in the Modules.cs file. In this sample the simplest (and
   most convenient form) of module definition is used which is is the static method module. A static method modules is simply
   defined by writing a `public static` method that is decorated with the `[StaticModule]` attribute. This attribute defines
   the module name with which it can be created in the Rosvita workflow editor.
   The arguments of the method-module method become the input pins and the return value becomes the output pin of the module.
   Pin-options (e.g. whether the pin should be shown on the module or in the property editor by default) can be specified via the `[InputPin]` attribute.

## Configuration/Workflow Discovery of Custom Assemblies

The search process for custom components and workflow modules is separate for parts of the Rosvita project configuration
system and the workflow editor.

### Auto-Discovery of Configuration Components

Custom configuration components can be dynamically added to a project by creating a `configurationElements` sub-folder in a
project folder and copying the assemblies containing the configuration component into that folder. When the project is
loaded the configuration elements are automatically loaded. The project has to be unloaded and re-opened before a newly
copied assembly is found.

If a single assembly contains configuration and the graph modules it can be placed in the project's
`configurationElements` directory. There it will be found by the configuration system and the if a matching `.xmodule`
file is present also by the workflow editor.

Copy all the files that are generated in the bin folder of a custom device into the `configurationElements` folder.
E.g. for the SampleDevice the folder listing of the `configurationElements` folder should look similar to:

```
-rw-r--r-- 1 xamla xamla 47119 Aug 15 09:11 Rosvita.SampleDevice.deps.json
-rw-r--r-- 1 xamla xamla 19968 Aug 15 09:11 Rosvita.SampleDevice.dll
-rw-r--r-- 1 xamla xamla  5768 Aug 15 09:11 Rosvita.SampleDevice.pdb
-rw-r--r-- 1 xamla xamla   145 Aug 15 09:11 Rosvita.SampleDevice.xml
-rw-r--r-- 1 xamla xamla     3 Aug 15 09:11 Rosvita.SampleDevice.xmodule
```

### Auto-Discovery of Workflow Editor Modules

A Rosvita graph runtime process searches recursively during start-up for files with the ending `.xmodule` in the current working directory, the graph runtime host binary directory. When it finds empty `.xmodule` files it looks for an assembly with the
same same by `.module` replaced by `.dll` und tries to load it. If the assembly could be loaded it looks for the `GraphRuntimeInitializer` attribute to intantiate the specified initializer class and trigger the initialization.

## Quick References

### ConfigurationGenerator

```csharp
namespace Rosvita.Project
{
    public interface IConfigurationGenerator
    {
        string Name { get; }
        int Priority { get; }

        void Generate(IRosvitaRuntime runtime, IConfiguration configuration, IGeneratorContext context, IGeneratorMessageCollection output);
        bool Validate(IRosvitaRuntime runtime, IConfiguration configuration, IGeneratorContext context, IGeneratorMessageCollection output);
    }
}
```

```csharp
namespace Rosvita.Project.Generators
{
    public abstract class ConfigurationGeneratorBase : IConfigurationGenerator
    {
        public ConfigurationGeneratorBase(string name, int priority);

        public string Name { get; }
        public int Priority { get; }

        public void Generate(IRosvitaRuntime runtime, IConfiguration configuration, IGeneratorContext context, IGeneratorMessageCollection output);
        public bool Validate(IRosvitaRuntime runtime, IConfiguration configuration, IGeneratorContext context, IGeneratorMessageCollection output);
        protected abstract void GenerateInternal(IRosvitaRuntime runtime, IConfiguration configuration, IGeneratorContext context, IGeneratorMessageCollection messages);
        protected virtual bool ValidateInternal(IRosvitaRuntime runtime, IConfiguration configuration, IGeneratorContext context, IGeneratorMessageCollection messages);
    }
}
```

### Generator Priorities

```csharp
    public static class GeneratorPriority
    {
        public const int OutputDirecotry = 20000;
        public const int XacroProcessor = 10000;
        public const int MoveIt = 5000;
        public const int Srdf = 4500;
        public const int LaunchFileGenerator = 4000;
        public const int PostLaunchFileGeneration = 3500;
        public const int JoggingLaunchFileGenerator = 3000;
        public const int XamlaMoveItLaunchFile = 2500;
        public const int SysmonLaunchFile = 2000;
        public const int MasterLaunchFileGenerator = 1000;
        public const int PackageXml = 500;
        public const int GenerationFinalizer = 10;
    }
```
