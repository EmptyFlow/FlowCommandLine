using System.Globalization;
using System.Reflection;

namespace FlowCommandLine {

    public static class FlowPropertyMapper {

        public static PropertyInfo? GetPropertyFromParameter ( FlowCommandParameter parameter, IEnumerable<PropertyInfo> properties ) {
            if ( !string.IsNullOrEmpty ( parameter.PropertyName ) ) return properties.FirstOrDefault ( a => a.Name.ToLowerInvariant () == parameter.PropertyName.ToLowerInvariant () );
            if ( !string.IsNullOrEmpty ( parameter.FullName ) ) return properties.FirstOrDefault ( a => a.Name.ToLowerInvariant () == parameter.FullName.ToLowerInvariant() );
            if ( !string.IsNullOrEmpty ( parameter.ShortName ) ) return properties.FirstOrDefault ( a => a.Name.ToLowerInvariant () == parameter.ShortName.ToLowerInvariant () );

            return null;
        }

        public static Dictionary<string, FlowCommandParameter> ParametersToDictionary ( IEnumerable<FlowCommandParameter> parameters ) {
            var result = new Dictionary<string, FlowCommandParameter> ();

            foreach ( var parameter in parameters ) {
                if ( !string.IsNullOrEmpty ( parameter.PropertyName ) ) result.Add ( parameter.PropertyName.ToLowerInvariant (), parameter );
                if ( !string.IsNullOrEmpty ( parameter.FullName ) ) result.Add ( parameter.FullName.ToLowerInvariant (), parameter );
                if ( !string.IsNullOrEmpty ( parameter.ShortName ) ) result.Add ( parameter.ShortName.ToLowerInvariant (), parameter );
            }

            return result;
        }

        public static bool SetPropertyValue<T> ( Type type, Dictionary<string, string> values, string parameterKey, T model, PropertyInfo property ) {
            bool isChanged = false;
            switch ( type ) {
                case Type _ when type == typeof ( int ):
                    if ( values.ContainsKey ( parameterKey ) && int.TryParse ( values[parameterKey], out var int32value ) ) {
                        property.SetValue ( model, int32value );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( long ):
                    if ( values.ContainsKey ( parameterKey ) && long.TryParse ( values[parameterKey], out var int64value ) ) {
                        property.SetValue ( model, int64value );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( double ):
                    if ( values.ContainsKey ( parameterKey ) && double.TryParse ( values[parameterKey], CultureInfo.InvariantCulture, out var doublevalue ) ) {
                        property.SetValue ( model, doublevalue );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( float ):
                    if ( values.ContainsKey ( parameterKey ) && float.TryParse ( values[parameterKey], CultureInfo.InvariantCulture, out var floatvalue ) ) {
                        property.SetValue ( model, floatvalue );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( DateOnly ):
                    if ( values.ContainsKey ( parameterKey ) && DateOnly.TryParse ( values[parameterKey], CultureInfo.InvariantCulture, out var dateOnlyvalue ) ) {
                        property.SetValue ( model, dateOnlyvalue );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( DateTime ):
                    if ( values.ContainsKey ( parameterKey ) && DateTime.TryParse ( values[parameterKey], CultureInfo.InvariantCulture, out var dateTimevalue ) ) {
                        property.SetValue ( model, dateTimevalue );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( TimeSpan ):
                    if ( values.ContainsKey ( parameterKey ) && TimeSpan.TryParse ( values[parameterKey], CultureInfo.InvariantCulture, out var timeSpanValue ) ) {
                        property.SetValue ( model, timeSpanValue );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( string ):
                    if ( values.ContainsKey ( parameterKey ) ) {
                        property.SetValue ( model, values[parameterKey] );
                        isChanged = true;
                    }
                    break;
                default:
                    Console.WriteLine ( $"Property {property.Name} with type {property.PropertyType.FullName} inside class {type.Name} not supported!" );
                    break;
            }

            return isChanged;
        }

    }

}
