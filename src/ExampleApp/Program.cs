using FlowCommandLine;

await CommandLine.Console ()
    .Application ( "Console Application", "1.0.0", "Application full description.", "Copyright (c) Macroloft All right Reserved", "conapp" )
    .AddCommand (
        "testcommand",
        ( parameters ) => {
            
        },
        "Command description",
        new List<FlowCommandParameter> {
            new FlowCommandParameter {
                FullName = "param1",
                Description = "parameter description",
                Required = true,
            },
            new FlowCommandParameter {
                FullName = "param2",
                Description = "parameter2 description",
                Required = false,
            }
        }
    )
    .RunCommandAsync ();