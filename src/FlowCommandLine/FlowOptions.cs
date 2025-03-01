using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FlowCommandLine {

    public class FlowOptions<[DynamicallyAccessedMembers ( DynamicallyAccessedMemberTypes.PublicProperties )] T> where T : new() {

        public List<FlowCommandParameter> Parameters { get; init; } = new List<FlowCommandParameter> ();

        public T MapParametersToType ( Dictionary<string, string> values, ICommandLineProvider commandLineProvider ) {
            var result = new T ();

            var properties = result
                .GetType ()
                .GetProperties ( BindingFlags.Public | BindingFlags.Instance );
            var parameters = FlowPropertyMapper.ParametersToDictionary ( Parameters );

            var processedParameters = new HashSet<FlowCommandParameter> ();

            foreach ( var parameter in parameters ) {
                var property = FlowPropertyMapper.GetPropertyFromParameter ( parameter.Value, properties );
                if ( property == null ) continue;
                if ( processedParameters.Contains ( parameter.Value ) ) continue;

                var isProcessed = FlowPropertyMapper.SetPropertyValue ( property.PropertyType, values, parameter.Key, result, property );
                if ( isProcessed ) processedParameters.Add ( parameter.Value );
            }

            var requiredParameters = Parameters.Where ( a => a.Required ).ToList ();
            if ( requiredParameters.Intersect ( processedParameters ).Count () != requiredParameters.Count ) {
                commandLineProvider.WriteLine ( "Not all required options is defined!" );
                throw new Exception ( "Not all required options is defined!" );
            }

            return result;
        }

    }

}
