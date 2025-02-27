namespace FlowCommandLine {

    internal record FlowAsyncCommand {

        public string Description { get; init; } = "";

        public List<FlowCommandParameter> Parameters { get; init; } = new List<FlowCommandParameter> ();

        public FlowCommandLineCommandAsyncDelegate? @Delegate { get; init; }

    }

}
