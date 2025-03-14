using System.Globalization;
using System.Reflection;
using System.Text;

namespace FlowCommandLine {

    public static class FlowPropertyMapper {

        public static PropertyInfo? GetPropertyFromParameter ( FlowCommandParameter parameter, IEnumerable<PropertyInfo> properties ) {
            if ( !string.IsNullOrEmpty ( parameter.PropertyName ) ) return properties.FirstOrDefault ( a => a.Name.ToLowerInvariant () == parameter.PropertyName.ToLowerInvariant () );
            if ( !string.IsNullOrEmpty ( parameter.FullName ) ) return properties.FirstOrDefault ( a => a.Name.ToLowerInvariant () == parameter.FullName.ToLowerInvariant () );
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
                case Type _ when type == typeof ( IEnumerable<int> ):
                    if ( values.ContainsKey ( parameterKey ) ) {
                        property.SetValue ( model, MapIntegerCollections ( values, parameterKey ) );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( List<int> ):
                    if ( values.ContainsKey ( parameterKey ) ) {
                        property.SetValue ( model, MapIntegerCollections ( values, parameterKey ) );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( long ):
                    if ( values.ContainsKey ( parameterKey ) && long.TryParse ( values[parameterKey], out var int64value ) ) {
                        property.SetValue ( model, int64value );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( IEnumerable<long> ):
                    if ( values.ContainsKey ( parameterKey ) ) {
                        property.SetValue ( model, MapLongCollections ( values, parameterKey ) );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( List<long> ):
                    if ( values.ContainsKey ( parameterKey ) ) {
                        property.SetValue ( model, MapLongCollections ( values, parameterKey ) );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( double ):
                    if ( values.ContainsKey ( parameterKey ) && double.TryParse ( values[parameterKey], CultureInfo.InvariantCulture, out var doublevalue ) ) {
                        property.SetValue ( model, doublevalue );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( IEnumerable<double> ):
                    if ( values.ContainsKey ( parameterKey ) ) {
                        property.SetValue ( model, MapDoubleCollections ( values, parameterKey ) );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( List<double> ):
                    if ( values.ContainsKey ( parameterKey ) ) {
                        property.SetValue ( model, MapDoubleCollections ( values, parameterKey ) );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( float ):
                    if ( values.ContainsKey ( parameterKey ) && float.TryParse ( values[parameterKey], CultureInfo.InvariantCulture, out var floatvalue ) ) {
                        property.SetValue ( model, floatvalue );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( IEnumerable<float> ):
                    if ( values.ContainsKey ( parameterKey ) ) {
                        property.SetValue ( model, MapFloatCollections ( values, parameterKey ) );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( List<float> ):
                    if ( values.ContainsKey ( parameterKey ) ) {
                        property.SetValue ( model, MapFloatCollections ( values, parameterKey ) );
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
                        property.SetValue ( model, MapStringValue ( values[parameterKey] ) );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( IEnumerable<string> ):
                    if ( values.ContainsKey ( parameterKey ) ) {
                        property.SetValue ( model, MapStringCollections ( values[parameterKey] ) );
                        isChanged = true;
                    }
                    break;
                case Type _ when type == typeof ( List<string> ):
                    if ( values.ContainsKey ( parameterKey ) ) {
                        property.SetValue ( model, MapStringCollections ( values[parameterKey] ) );
                        isChanged = true;
                    }
                    break;
                default:
                    Console.WriteLine ( $"Property {property.Name} with type {property.PropertyType.FullName} inside class {type.Name} not supported!" );
                    break;
            }

            return isChanged;
        }

        private static List<int> MapIntegerCollections ( Dictionary<string, string> values, string parameterKey ) {
            return values[parameterKey]
                .Split ( "," )
                .Select (
                    a => {
                        if ( int.TryParse ( a, out var int32value ) ) {
                            return (int?) int32value;
                        } else {
                            return null;
                        }
                    }
                )
                .Where ( a => a != null )
                .Select ( a => a!.Value )
                .ToList ();
        }

        private static List<long> MapLongCollections ( Dictionary<string, string> values, string parameterKey ) {
            return values[parameterKey]
                .Split ( "," )
                .Select (
                    a => {
                        if ( long.TryParse ( a, out var int32value ) ) {
                            return (long?) int32value;
                        } else {
                            return null;
                        }
                    }
                )
                .Where ( a => a != null )
                .Select ( a => a!.Value )
                .ToList ();
        }

        private static List<double> MapDoubleCollections ( Dictionary<string, string> values, string parameterKey ) {
            return values[parameterKey]
                .Split ( "," )
                .Select (
                    a => {
                        if ( double.TryParse ( a, CultureInfo.InvariantCulture, out var doublevalue ) ) {
                            return (double?) doublevalue;
                        } else {
                            return null;
                        }
                    }
                )
                .Where ( a => a != null )
                .Select ( a => a!.Value )
                .ToList ();
        }

        private static List<float> MapFloatCollections ( Dictionary<string, string> values, string parameterKey ) {
            return values[parameterKey]
                .Split ( "," )
                .Select (
                    a => {
                        if ( float.TryParse ( a, CultureInfo.InvariantCulture, out var floatValue ) ) {
                            return (float?) floatValue;
                        } else {
                            return null;
                        }
                    }
                )
                .Where ( a => a != null )
                .Select ( a => a!.Value )
                .ToList ();
        }

        private static string MapStringValue ( string value ) {
            value = value.Trim ();
            if ( value.StartsWith ( '\"' ) ) value = value[1..];
            if ( value.EndsWith ( '\"' ) ) value = value[..^1];

            return value;
        }

        private static List<string> GetStringParts ( string value ) {
            var currentValue = new StringBuilder ();
            var result = new List<string> ();
            var quoteStarted = false;
            foreach ( var character in value ) {
                if ( character == ' ' && !quoteStarted ) {
                    if ( currentValue.Length > 0 ) {
                        result.Add ( currentValue.ToString () );
                        currentValue.Clear ();
                    }
                    continue;
                }
                if ( character == '\"' && !quoteStarted ) {
                    quoteStarted = true;
                    if ( currentValue.Length > 0 ) {
                        result.Add ( currentValue.ToString () );
                        currentValue.Clear ();
                    }
                    continue;
                }
                if ( character == '\"' && quoteStarted ) {
                    quoteStarted = false;
                    if ( currentValue.Length > 0 ) {
                        result.Add ( currentValue.ToString () );
                        currentValue.Clear ();
                    }
                    continue;
                }

                currentValue.Append ( character );
            }

            if ( currentValue.Length > 0 ) result.Add ( currentValue.ToString () );

            return result;
        }

        private static List<string> MapStringCollections ( string value ) {
            if ( value.Contains ( '\"' ) ) {
                return GetStringParts ( value );
            } else {
                return value
                    .Split ( " " )
                    .Where ( a => !string.IsNullOrEmpty ( a ) )
                    .ToList ();
            }
        }

    }

}
