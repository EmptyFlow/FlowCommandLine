namespace FlowCommandLine {

    internal record FlowCommand {

        public string Description { get; init; } = "";

        public List<FlowCommandParameter> Parameters { get; init; } = new List<FlowCommandParameter>();

        public FlowCommandLineCommandDelegate? @Delegate { get; init; }

    }

}
