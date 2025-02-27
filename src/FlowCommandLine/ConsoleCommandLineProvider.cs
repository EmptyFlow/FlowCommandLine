namespace FlowCommandLine {

    /// <summary>
    /// Command line provider working with system <see cref="Console"/> class.
    /// </summary>
    public sealed class ConsoleCommandLineProvider : ICommandLineProvider {

        public string GetCommandLine () {
            var commandLine = Environment.CommandLine.AsSpan ();
            var firstIndexOfSpace = commandLine[0] == '"' ? commandLine[1..].IndexOf ( '"' ) + 2 : commandLine.IndexOf ( ' ' );
            if ( firstIndexOfSpace == -1 ) return "";

            return commandLine[firstIndexOfSpace..].ToString ();
        }

        public void WriteLine ( string line ) => Console.WriteLine ( line );

    }

}
