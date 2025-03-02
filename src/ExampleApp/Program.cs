using FlowCommandLine;

await CommandLine.Console ()
    .Application ( "Console Application", "1.0.0", "Application full description.", "Copyright (c) Macroloft All right Reserved", "conapp" )
    .AddCommand (
        "testcommand",
        ( Test parameters ) => {
            Console.WriteLine ( "Param1: " + parameters.Param1 );
            Console.WriteLine ( "Param2: " + parameters.Param2 );
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

public class Test {
    public string Param1 { get; set; } = "";
    public int Param2 { get; set; }
}