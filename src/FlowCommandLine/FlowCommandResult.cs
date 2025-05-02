namespace FlowCommandLine {

    public class FlowCommandResult {

        /// <summary>
        /// It was empty input.
        /// Can be useful if you need to make default behaviour if command line was empty.
        /// </summary>
        public bool EmptyInput { get; set; }

        /// <summary>
        /// Command was found handled.
        /// </summary>
        public bool CommandHandled { get; set; }

        /// <summary>
        /// Was handed but not command (as example --version, --help etc)
        /// </summary>
        public bool Handled { get; set; }

    }

}
