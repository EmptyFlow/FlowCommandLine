using System.Globalization;
using System.Reflection;

namespace FlowCommandLine {

    public static class FlowPropertyMapper {

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
