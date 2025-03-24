using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FlowCommandLine {

    internal record FlowCommandAsyncDelegate<[DynamicallyAccessedMembers ( DynamicallyAccessedMemberTypes.All )] T> : FlowAsyncCommand where T : new() {

        public FlowCommandLineCommandAsyncDelegate<T>? @Delegate { get; init; }

        public T MapParametersToType ( Dictionary<string, string> values, ICommandLineProvider commandLineProvider ) {
            var resultType = typeof ( T );

            var properties = resultType
                .GetProperties ( BindingFlags.Public | BindingFlags.Instance );
            var parameters = FlowPropertyMapper.ParametersToDictionary ( Parameters );

            var processedParameters = new HashSet<FlowCommandParameter> ();
            var result = new T ();

            foreach ( var parameter in parameters ) {
                var property = FlowPropertyMapper.GetPropertyFromParameter ( parameter.Value, properties );
                if ( property == null ) continue;
                if ( processedParameters.Contains ( parameter.Value ) ) continue;

                var isProcessed = FlowPropertyMapper.SetPropertyValue ( property.PropertyType, values, parameter.Key, result, property );
                if ( isProcessed ) processedParameters.Add ( parameter.Value );
            }

            var requiredParameters = Parameters.Where ( a => a.Required ).ToList ();
            if ( requiredParameters.Intersect ( processedParameters ).Count () != requiredParameters.Count ) {
                commandLineProvider.WriteLine ( "Not all required parameters is defined!" );
                throw new Exception ( "Not all required parameters is defined!" );
            }

            return result;
        }

        public override Task Execute ( Dictionary<string, string> values, ICommandLineProvider commandLineProvider ) {
            var task = Delegate?.Invoke ( MapParametersToType ( values, commandLineProvider ) );
            if ( task != null ) return task;

            return Task.CompletedTask;
        }

    }

    internal record FlowAsyncCommand {

        public string Description { get; init; } = "";

        public List<FlowCommandParameter> Parameters { get; init; } = new List<FlowCommandParameter> ();

        public virtual Task Execute ( Dictionary<string, string> values, ICommandLineProvider commandLineProvider ) => Task.CompletedTask;

    }

}
