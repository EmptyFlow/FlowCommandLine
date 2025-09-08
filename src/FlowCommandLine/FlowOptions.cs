using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FlowCommandLine {

    public class FlowOptions<[DynamicallyAccessedMembers ( DynamicallyAccessedMemberTypes.All )] T> where T : new() {

        public List<FlowCommandParameter> Parameters { get; init; } = new List<FlowCommandParameter> ();

        public T MapParametersToType ( Dictionary<string, string> values, ICommandLineProvider commandLineProvider, string defaultParameterValue ) {
            var resultType = typeof ( T );
            var result = new T ();

            var properties = resultType
                .GetProperties ( BindingFlags.Public | BindingFlags.Instance );

            var parameters = FlowPropertyMapper.ParametersToDictionary ( Parameters );

            FlowPropertyMapper.FillDefaultParameter ( Parameters, defaultParameterValue, commandLineProvider, properties, result );

            var processedParameters = new HashSet<FlowCommandParameter> ();

            foreach ( var parameter in parameters ) {
                var property = FlowPropertyMapper.GetPropertyFromParameter ( parameter.Value, properties );
                if ( property == null ) continue;
                if ( processedParameters.Contains ( parameter.Value ) ) continue;

                var isProcessed = FlowPropertyMapper.SetPropertyValue ( property.PropertyType, values, parameter.Key, result, property );
                if ( isProcessed ) processedParameters.Add ( parameter.Value );
            }

            var requiredParameters = Parameters.Where ( a => a.Required && !a.Default ).ToList ();
            if ( requiredParameters.Intersect ( processedParameters ).Count () != requiredParameters.Count ) {
                commandLineProvider.WriteLine ( "Not all required options is defined!" );
                throw new Exception ( "Not all required options is defined!" );
            }

            return result;
        }

    }

}
