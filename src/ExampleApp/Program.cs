using FlowCommandLine;
using static FlowCommandLine.FlowCommandParameter;

await CommandLine.Console ()
    .Application ( "Console Application", "1.0.0", "Application full description.", "Copyright (c) Macroloft All right Reserved", "conapp" )
    .AddCommand (
        "testcommand",
        ( Test parameters ) => {
            Console.WriteLine ( "Param1: " + parameters.Param1 );
            Console.WriteLine ( "Param2: " + parameters.Param2 );
        },
        "Command description",
        [
            CreateDefault(alias: "param1", help: "parameter description"),
            Create(alias: "param2", help: "parameter2 description"),
        ]
    )
    .RunCommandAsync ();

public class Test {
    public string Param1 { get; set; } = "";
    public int Param2 { get; set; }
}