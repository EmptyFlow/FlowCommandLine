namespace FlowCommandLine {

    /// <summary>
    /// Parameter for command.
    /// </summary>
    public sealed record FlowCommandParameter {

        public string Name { get; init; } = "";

        public string Description { get; init; } = "";

        public bool Required { get; init; }

    }

}
