using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FlowCommandLine {

    internal record FlowCommandDelegate<[DynamicallyAccessedMembers ( DynamicallyAccessedMemberTypes.PublicProperties )] T> : FlowCommand where T : new() {

        public FlowCommandLineCommandDelegate<T>? @Delegate { get; init; }

        public T MapParametersToType ( Dictionary<string, string> values ) {
            var result = new T ();

            var properties = result
                .GetType ()
                .GetProperties ( BindingFlags.Public | BindingFlags.Instance );
            var parameters = ParametersToDictionary ();

            var processedParameters = new HashSet<FlowCommandParameter> ();

            foreach ( var parameter in parameters ) {
                var property = properties.FirstOrDefault ( a => a.Name.ToLowerInvariant () == parameter.Key );
                if ( property == null ) continue;
                if ( processedParameters.Contains ( parameter.Value ) ) continue;

                var isProcessed = FlowPropertyMapper.SetPropertyValue ( property.PropertyType, values, parameter.Key, result, property );
                if ( isProcessed ) processedParameters.Add ( parameter.Value );
            }

            var requiredParameters = Parameters.Where ( a => a.Required ).ToList ();
            if ( requiredParameters.Intersect ( processedParameters ).Count () == requiredParameters.Count ) {
                Console.WriteLine ( "Not all required parameters is defined!" );
            }

            return result;
        }

        private Dictionary<string, FlowCommandParameter> ParametersToDictionary () {
            var result = new Dictionary<string, FlowCommandParameter> ();

            foreach ( var parameter in Parameters ) {
                if ( !string.IsNullOrEmpty ( parameter.PropertyName ) ) result.Add ( parameter.PropertyName.ToLowerInvariant (), parameter );
                if ( !string.IsNullOrEmpty ( parameter.FullName ) ) result.Add ( parameter.FullName.ToLowerInvariant (), parameter );
                if ( !string.IsNullOrEmpty ( parameter.ShortName ) ) result.Add ( parameter.ShortName.ToLowerInvariant (), parameter );
            }

            return result;
        }

        public override void Execute ( Dictionary<string, string> values ) {
            if ( Delegate == null ) return;

            @Delegate.Invoke ( MapParametersToType ( values ) );
        }

    }

    internal record FlowCommand {

        public string Description { get; init; } = "";

        public List<FlowCommandParameter> Parameters { get; init; } = new List<FlowCommandParameter> ();

        public virtual void Execute ( Dictionary<string, string> values ) {
        }

    }

}
