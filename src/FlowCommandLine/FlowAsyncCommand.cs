using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FlowCommandLine {

    internal record FlowCommandAsyncDelegate<[DynamicallyAccessedMembers ( DynamicallyAccessedMemberTypes.All )] T> : FlowAsyncCommand where T : new() {

        public FlowCommandLineCommandAsyncDelegate<T>? @Delegate { get; init; }

        public T MapParametersToType ( Dictionary<string, string> values, ICommandLineProvider commandLineProvider, string defaultParameterValue ) {
            var resultType = typeof ( T );
            var result = new T ();

            var properties = resultType
                .GetProperties ( BindingFlags.Public | BindingFlags.Instance );

            FlowPropertyMapper.MapParametersToType ( result, properties, Parameters, defaultParameterValue, commandLineProvider, values );

            return result;
        }

        public override Task Execute ( Dictionary<string, string> values, ICommandLineProvider commandLineProvider, string defaultParameterValue ) {
            var task = Delegate?.Invoke ( MapParametersToType ( values, commandLineProvider, defaultParameterValue ) );
            if ( task != null ) return task;

            return Task.CompletedTask;
        }

    }

    internal record FlowAsyncCommand {

        public string Description { get; init; } = "";

        public List<FlowCommandParameter> Parameters { get; init; } = new List<FlowCommandParameter> ();

        public virtual Task Execute ( Dictionary<string, string> values, ICommandLineProvider commandLineProvider, string defaultParameterValue ) => Task.CompletedTask;

    }

}
