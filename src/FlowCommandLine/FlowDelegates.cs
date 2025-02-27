namespace FlowCommandLine {

    public delegate void FlowCommandLineCommandDelegate ( Dictionary<string, string> parameters );

    public delegate Task FlowCommandLineCommandAsyncDelegate ( Dictionary<string, string> parameters );

}
