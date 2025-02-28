namespace FlowCommandLine {

    /// <summary>
    /// Parameter for command.
    /// </summary>
    public sealed record FlowCommandParameter {

        public string PropertyName { get; init; } = "";

        public string FullName { get; init; } = "";

        public string ShortName { get; init; } = "";

        public string Description { get; init; } = "";

        public bool Required { get; init; }

    }

}
