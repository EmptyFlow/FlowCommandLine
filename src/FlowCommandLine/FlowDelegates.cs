namespace FlowCommandLine {

    public delegate void FlowCommandLineCommandDelegate<T> ( T parameters );

    public delegate Task FlowCommandLineCommandAsyncDelegate<T> ( T parameters );

}
