namespace FlowCommandLine {

    /// <summary>
    /// Abstraction for access to command line.
    /// </summary>
    public interface ICommandLineProvider {

        /// <summary>
        /// Get command line string (must be not include path to executable).
        /// </summary>
        string GetCommandLine ();

        /// <summary>
        /// Write line to console.
        /// </summary>
        void WriteLine ( string line );

    }

}
