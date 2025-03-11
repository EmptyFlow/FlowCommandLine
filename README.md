[![CI](https://github.com/EmptyFlow/FlowCommandLine/actions/workflows/cipackage.yml/badge.svg)](https://github.com/EmptyFlow/FlowCommandLine/actions/workflows/cipackage.yml) [![nugeticon](https://img.shields.io/badge/nuget-available-blue)](https://www.nuget.org/packages/FlowCommandLine)

# FlowCommandLine
A fast and simple command line parser that works in two modes: command-based (e.g. `git commit ...`) or parameters-only. Parsing can be happened to any model class or record with parameterless constructor.
It support modern dotnet core runtimes (net8+), compilation in NativeAot. It supported auto documentation for commands and parameters.
Model parameters mapped types is supported: int, long, double, float, string, DateOnly, DateTime, TimeSpan.
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
            new FlowCommandParameter {
                FullName = "param1", // for parameter in format --param1
                ShortName = "p1, // for parameter in format -p1
                PropertyName = "Param1" // it 
                Description = "parameter description",
                Required = true, // parameter is required
            },
            new FlowCommandParameter {
                FullName = "param2", // full name is required property, other properties ShortName or PropertyName can be inferred from FullName
                Description = "parameter2 description",
                Required = false,
            }
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

