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

        public static FlowCommandParameter Create ( string name = "", string alias = "", string help = "", string property = "" ) {
            return new FlowCommandParameter {
                FullName = alias,
                ShortName = name,
                Description = help,
                PropertyName = property,
                Required = false
            };
        }

        public static FlowCommandParameter CreateRequired ( string name = "", string alias = "", string help = "", string property = "" ) {
            return new FlowCommandParameter {
                FullName = alias,
                ShortName = name,
                Description = help,
                PropertyName = property,
                Required = true
            };
        }

    }

}
