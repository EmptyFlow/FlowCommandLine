using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FlowCommandLine {

    internal record FlowCommandDelegate<[DynamicallyAccessedMembers ( DynamicallyAccessedMemberTypes.All )] T> : FlowCommand where T : new() {

        public FlowCommandLineCommandDelegate<T>? @Delegate { get; init; }

        public T MapParametersToType ( Dictionary<string, string> values, ICommandLineProvider commandLineProvider, string defaultParameterValue ) {
            var resultType = typeof ( T );
            var result = new T ();

            var properties = resultType
                .GetProperties ( BindingFlags.Public | BindingFlags.Instance );

            FlowPropertyMapper.MapParametersToType ( result, properties, Parameters, defaultParameterValue, commandLineProvider, values );

            return result;
        }

        public override void Execute ( Dictionary<string, string> values, ICommandLineProvider commandLineProvider, string defaultParameter ) {
            if ( Delegate == null ) return;

            @Delegate.Invoke ( MapParametersToType ( values, commandLineProvider, defaultParameter ) );
        }

    }

    internal record FlowCommand {

        public string Description { get; init; } = "";

        public List<FlowCommandParameter> Parameters { get; init; } = new List<FlowCommandParameter> ();

        public virtual void Execute ( Dictionary<string, string> values, ICommandLineProvider commandLineProvider, string defaultParameter ) {
        }

    }

}
