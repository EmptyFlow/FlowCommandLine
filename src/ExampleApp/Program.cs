using FlowCommandLine;

await CommandLine.Console ()
    .Application ( "Console Application", "1.0.0", "Application full description.", "Copyright (c) Macroloft All right Reserved", "conapp" )
    .AddCommand ( "testcommand", ( parameters ) => { Console.WriteLine ( "Lalalla" ); }, "Command description", new List<FlowCommandParameter> () )
    .RunCommandAsync ();
