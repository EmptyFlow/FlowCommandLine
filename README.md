[![CI](https://github.com/EmptyFlow/FlowCommandLine/actions/workflows/cipackage.yml/badge.svg)](https://github.com/EmptyFlow/FlowCommandLine/actions/workflows/cipackage.yml) [![nugeticon](https://img.shields.io/badge/nuget-available-blue)](https://www.nuget.org/packages/FlowCommandLine)

# FlowCommandLine
A fast and simple command line parser that works in two modes: command-based (e.g. `git commit ...`) or parameters-only. Parsing can be happened to any model class or record with parameterless constructor.
It support modern dotnet core runtimes (net8+), compilation in NativeAot. It supported auto documentation for commands and parameters.
Lot of types of properties [is supported](https://github.com/EmptyFlow/FlowCommandLine/wiki/Supported-mappings-types).
By default, the output will be to the system console, but can be redefined to any of your case - instead `CommandLine.Console ()` you can use `new CommandLine (new MyConsoleCommandLineProvider())` where `MyConsoleCommandLineProvider` it is you class which is implement `ICommandLineProvider` interface.

## Command-based mode

Example command line:  
`myconapp runapp --param1=stringvalue --param2=120`

```csharp
public class Test {
    public string Param1 { get; set; } = "";
    public int Param2 { get; set; }
}

CommandLine.Console ()
    // setup console application description version and so on
    .Application ( "My Console App", "1.0.0", "full description.", "Copyright My Super Corporation", "myconapp" ) 
    .AddCommand ( // add console command
        "runapp", // command name
        ( Test parameters ) => { // command delegate handler for class Test
            ...
        },
        "Command description", // command description :)
        new List<FlowCommandParameter> { // adjust command parameters
            FlowCommandParameter.CreateRequired("p1", "param1", "parameter 1 description"), // use factory methods for required parameter
            FlowCommandParameter.CreateRequired("p3", "param3", "parameter 3 description"), 
            FlowCommandParameter.Create("p4", "param4", "parameter description"), // use factory method for non required parameter
            FlowCommandParameter.Create("p2", "param2", "parameter2 description")
        }
    )
    .RunCommand ();
```

## Parameters-only mode

Example command line:  
`myconapp --param1=stringvalue --param2=120`

```csharp
var options = CommandLine.Console ()
    .Application ( "My Console App", "1.0.0", "full description.", "Copyright My Super Corporation", "myconapp" )
    .AddOption ( "p1", "param1", "parameter 1 description", required: true )
    .AddOption ( "p2", "param2", "parameter 2 description", required: false )
    .RunOptions<Test> (); // run options parse and 
```

