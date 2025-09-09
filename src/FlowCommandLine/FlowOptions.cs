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

            FlowPropertyMapper.MapParametersToType ( result, properties, Parameters, defaultParameterValue, commandLineProvider, values );

            return result;
        }

    }

}
