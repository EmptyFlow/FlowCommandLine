using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FlowCommandLine {

    public sealed class CommandLine {

        private ICommandLineProvider m_commandLineProvider;

        private string m_commandLine = "";

        private string m_applicationName = "";

        private string m_applicationExecutable = "";

        private string m_applicationVersion = "";

        private string m_applicationDescription = "";

        private string m_applicationCopyright = "";

        private Dictionary<string, FlowCommand> m_commands = new ();

        private Dictionary<string, FlowAsyncCommand> m_asyncCommands = new ();

        private List<FlowCommandParameter> m_options = new ();

        /// <summary>
        /// Create instance of command line compiler and adjust string with command line.
        /// </summary>
        /// <param name="commandLine">String containing command line.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CommandLine ( ICommandLineProvider provider ) {
            if ( provider == null ) throw new ArgumentNullException ( nameof ( provider ) );
            m_commandLineProvider = provider;

            m_commandLine = m_commandLineProvider.GetCommandLine () ?? throw new ArgumentNullException ( "GetCommandLine" );
            m_commandLine = m_commandLine.Trim ();
        }

        /// <summary>
        /// Setup application information.
        /// </summary>
        /// <param name="applicationName">Application name.</param>
        /// <param name="version">Version.</param>
        /// <param name="description">Description.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CommandLine Application ( string applicationName, string version, string description = "", string copyright = "", string applicationExecutable = "" ) {
            m_applicationName = applicationName ?? throw new ArgumentNullException ( nameof ( applicationName ) );
            m_applicationVersion = version ?? throw new ArgumentNullException ( nameof ( version ) );
            m_applicationDescription = description ?? "";
            m_applicationCopyright = copyright ?? "";
            m_applicationExecutable = applicationExecutable ?? "";

            return this;
        }

        /// <summary>
        /// Add command with synchronious handler.
        /// </summary>
        /// <param name="name">The name of the command to be processed from the command line.</param>
        /// <param name="delegate">Delegate that will be called in case if user select this command in console.</param>
        /// <param name="description">A human-readable description of the command.</param>
        /// <param name="parameters">Command parameters.</param>
        public CommandLine AddCommand<[DynamicallyAccessedMembers ( DynamicallyAccessedMemberTypes.All )] T> ( string name, FlowCommandLineCommandDelegate<T> @delegate, string description, IEnumerable<FlowCommandParameter> parameters ) where T : new() {
            var lowerName = name.ToLowerInvariant ();
            m_commands[lowerName] = new FlowCommandDelegate<T> {
                Delegate = @delegate,
                Description = description,
                Parameters = parameters
                    .Where (
                        a => a != null && ( !string.IsNullOrEmpty ( a.FullName ) || !string.IsNullOrEmpty ( a.ShortName ) )
                    )
                    .ToList ()
            };

            return this;
        }

        public CommandLine AddAsyncCommand<[DynamicallyAccessedMembers ( DynamicallyAccessedMemberTypes.All )] T> ( string name, FlowCommandLineCommandAsyncDelegate<T> @delegate, string description, IEnumerable<FlowCommandParameter> parameters ) where T : new() {
            var lowerName = name.ToLowerInvariant ();
            m_asyncCommands[lowerName] = new FlowCommandAsyncDelegate<T> {
                Delegate = @delegate,
                Description = description,
                Parameters = parameters
                    .Where ( a => a != null && !string.IsNullOrEmpty ( a.FullName ) )
                    .ToList ()
            };

            return this;
        }

        /// <summary>
        /// Run the command from the command line.
        /// </summary>
        public Task RunCommandAsync () {
            if ( IsVersion () ) {
                ShowVersion ();
                return Task.CompletedTask;
            }

            var parts = GetParts ();
            if ( string.IsNullOrEmpty ( m_commandLine ) || !parts.Any () || IsHelpParameter ( m_commandLine ) || parts.Any ( IsHelpParameter ) ) {
                ShowHelp ( parts, true );
                return Task.CompletedTask;
            }

            ParseParameters ( parts, out var command, out var parameters );

            try {
                if ( m_commands.TryGetValue ( command, out var flowCommand ) ) {
                    flowCommand.Execute ( parameters, m_commandLineProvider );
                    return Task.CompletedTask;
                }
                if ( m_asyncCommands.TryGetValue ( command, out var flowAsyncCommand ) ) return flowAsyncCommand.Execute ( parameters, m_commandLineProvider );
            } catch {
            }

            ShowHelp ( parts );
            return Task.CompletedTask;
        }

        /// <summary>
        /// Run the command from the command line.
        /// </summary>
        public void RunCommand () {
            if ( m_asyncCommands.Any () ) throw new ArgumentException ( "You have asynchronized commands, you need to use RunCommandAsync method inside!" );

            if ( IsVersion () ) {
                ShowVersion ();
                return;
            }

            var parts = GetParts ();
            if ( string.IsNullOrEmpty ( m_commandLine ) || !parts.Any () || IsHelpParameter ( m_commandLine ) || parts.Any ( IsHelpParameter ) ) {
                ShowHelp ( parts, true );
                return;
            }

            ParseParameters ( parts, out var command, out var parameters );

            try {
                if ( m_commands.TryGetValue ( command, out var flowCommand ) ) {
                    flowCommand.Execute ( parameters, m_commandLineProvider );
                    return;
                }
            } catch {
            }

            ShowHelp ( parts );
        }

        public CommandLine AddOption ( string shortName = "", string fullName = "", string description = "", string propertyName = "", bool required = false ) {
            m_options.Add (
                new FlowCommandParameter {
                    Description = description,
                    FullName = fullName,
                    ShortName = shortName,
                    Required = required,
                    PropertyName = propertyName
                }
            );

            return this;
        }

        /// <summary>
        /// Run the command from the command line.
        /// </summary>
        public T? RunOptions<[DynamicallyAccessedMembers ( DynamicallyAccessedMemberTypes.All )] T> () where T : new() {
            if ( IsVersion () ) {
                ShowVersion ();
                return default;
            }

            var parts = GetParts ();
            if ( string.IsNullOrEmpty ( m_commandLine ) || !parts.Any () || IsHelpParameter ( m_commandLine ) || parts.Any ( IsHelpParameter ) ) {
                ShowOptionsHelp ();
                return default;
            }

            ParseOptionParameters ( parts, out var parameters );

            var flowOptions = new FlowOptions<T> {
                Parameters = m_options
            };
            return flowOptions.MapParametersToType ( parameters, m_commandLineProvider );
        }

        private void ShowCommandHelp ( string description, List<FlowCommandParameter> parameters ) {
            if ( !string.IsNullOrEmpty ( description ) ) {
                m_commandLineProvider.WriteLine ( description );
                m_commandLineProvider.WriteLine ( " " );
            }
            ShowParameters ( parameters );
        }

        private void ShowParameters ( List<FlowCommandParameter> commandParameters ) {
            m_commandLineProvider.WriteLine ( "The following parameters are available:" );
            foreach ( var parameter in commandParameters ) {
                var parameters = new List<string> ();
                if ( !string.IsNullOrEmpty ( parameter.FullName ) ) parameters.Add ( $"--{parameter.FullName}" );
                if ( !string.IsNullOrEmpty ( parameter.ShortName ) ) parameters.Add ( $"-{parameter.ShortName}" );

                m_commandLineProvider.WriteLine ( $"  {string.Join ( ' ', parameters )} {parameter.Description}" );
            }
        }

        /// <summary>
        /// Show application help and a list of available commands.
        /// </summary>
        private void ShowHelp ( List<string> parts, bool fullInfo = false ) {
            if ( fullInfo ) {
                m_commandLineProvider.WriteLine ( $"{m_applicationName} version {m_applicationVersion}" );
                if ( !string.IsNullOrEmpty ( m_applicationCopyright ) ) m_commandLineProvider.WriteLine ( m_applicationCopyright );
                m_commandLineProvider.WriteLine ( " " );
                if ( !string.IsNullOrEmpty ( m_applicationDescription ) ) {
                    m_commandLineProvider.WriteLine ( m_applicationDescription );
                    m_commandLineProvider.WriteLine ( " " );
                }
                if ( !string.IsNullOrEmpty ( m_applicationExecutable ) ) {
                    m_commandLineProvider.WriteLine ( $"usage: {m_applicationExecutable} [<command>] [<parameters>]" );
                    m_commandLineProvider.WriteLine ( " " );
                }

                if ( m_commands.Any () || m_asyncCommands.Any () ) {
                    if ( parts.Any ( IsHelpParameter ) && ShowHelpBySingleCommand ( parts ) ) return;

                    var maximumLength = m_commands
                        .Select ( a => a.Key )
                        .Concat ( m_asyncCommands.Select ( b => b.Key ) )
                        .Select ( a => a.Length )
                        .Max () + 1;
                    m_commandLineProvider.WriteLine ( "The following commands are available:" );

                    foreach ( var command in m_commands ) {
                        var name = command.Key;
                        var value = command.Value;
                        if ( name.Length < maximumLength ) name += string.Join ( "", Enumerable.Repeat ( ' ', maximumLength - name.Length ) );

                        m_commandLineProvider.WriteLine ( $"  {name}{value.Description}" );
                    }
                    foreach ( var asyncCommand in m_asyncCommands ) {
                        var name = asyncCommand.Key;
                        var value = asyncCommand.Value;
                        if ( name.Length < maximumLength ) name += string.Join ( "", Enumerable.Repeat ( ' ', maximumLength - name.Length ) );

                        m_commandLineProvider.WriteLine ( $"  {name}{value.Description}" );
                    }
                }
            } else {
                if ( !parts.Any () ) return;

                var command = parts.First ();
                if ( m_commands.ContainsKey ( command ) || m_asyncCommands.ContainsKey ( command ) ) {
                    m_commandLineProvider.WriteLine ( $" " );
                    ShowHelpBySingleCommand ( parts );
                } else {
                    m_commandLineProvider.WriteLine ( $"{m_applicationName} version {m_applicationVersion}" );
                    m_commandLineProvider.WriteLine ( "Command is incorrect!" );
                }
            }
        }

        private void ShowOptionsHelp () {
            m_commandLineProvider.WriteLine ( $"{m_applicationName} version {m_applicationVersion}" );
            if ( !string.IsNullOrEmpty ( m_applicationCopyright ) ) m_commandLineProvider.WriteLine ( m_applicationCopyright );
            m_commandLineProvider.WriteLine ( " " );
            if ( !string.IsNullOrEmpty ( m_applicationDescription ) ) {
                m_commandLineProvider.WriteLine ( m_applicationDescription );
                m_commandLineProvider.WriteLine ( " " );
            }
            if ( !string.IsNullOrEmpty ( m_applicationExecutable ) ) {
                m_commandLineProvider.WriteLine ( $"usage: {m_applicationExecutable} [<options>]" );
                m_commandLineProvider.WriteLine ( " " );
            }

            if ( m_options.Any () ) ShowParameters ( m_options );
        }

        private bool ShowHelpBySingleCommand ( List<string> parts ) {
            var command = parts.First ();

            if ( m_commands.ContainsKey ( command ) ) {
                var selectedCommand = m_commands[command];
                ShowCommandHelp ( selectedCommand.Description, selectedCommand.Parameters );
                return true;
            }
            if ( m_asyncCommands.ContainsKey ( command ) ) {
                var selectedCommand = m_asyncCommands[command];
                ShowCommandHelp ( selectedCommand.Description, selectedCommand.Parameters );
                return true;
            }

            return false;
        }

        private List<string> GetParts () {
            var result = new List<string> ();
            var currentPart = new StringBuilder ();
            var parameterStarted = false;
            char? previousCharacter = null;
            int currentIndex = -1;
            foreach ( var character in m_commandLine ) {
                if ( currentIndex > -1 ) previousCharacter = m_commandLine[currentIndex];
                currentIndex++;

                if ( character == ' ' && !parameterStarted ) {
                    result.Add ( currentPart.ToString () );
                    currentPart.Clear ();
                    continue;
                }
                if ( character == '-' && (previousCharacter == ' ' || currentIndex == 0) && !parameterStarted ) {
                    parameterStarted = true;
                    result.Add ( currentPart.ToString () );
                    currentPart.Clear ();
                }
                if ( character == '-' && previousCharacter == ' ' && parameterStarted && currentPart.Length > 2 ) {
                    parameterStarted = true;
                    result.Add ( currentPart.ToString () );
                    currentPart.Clear ();
                }

                currentPart.Append ( character );
            }

            if ( currentPart.Length > 0 ) result.Add ( currentPart.ToString () );

            return result
                .Where ( a => !string.IsNullOrEmpty ( a ) )
                .Select ( a => a.Trim () )
                .ToList ();
        }

        private void ShowVersion () => m_commandLineProvider.WriteLine ( $"{m_applicationVersion}" );

        private bool IsVersion () => m_commandLine is "-v" or "--version";

        private bool IsHelpParameter ( string part ) => part is "-h" or "--help";

        private static void ParseParameters ( List<string> parts, out string command, out Dictionary<string, string> parameters ) {
            command = parts.First ().ToLowerInvariant ();
            parameters = parts
                .Skip ( 1 )
                .Where ( a => ( a.StartsWith ( "-" ) && a.Length > 1 ) || ( a.StartsWith ( "--" ) && a.Length > 2 ) )
                .Select (
                    a => {
                        var pair = a.StartsWith ( "--" ) ? a.Substring ( 2 ).Split ( "=" ) : a.Substring ( 1 ).Split ( "=" );
                        return new { Name = pair[0], Value = pair.Length > 1 ? pair[1] : "" };
                    }
                )
                .ToDictionary ( a => a.Name, a => a.Value );
        }

        private static void ParseOptionParameters ( List<string> parts, out Dictionary<string, string> parameters ) {
            parameters = parts
                .Where ( a => ( a.StartsWith ( "-" ) && a.Length > 1 ) || ( a.StartsWith ( "--" ) && a.Length > 2 ) )
                .Select (
                    a => {
                        var pair = a.StartsWith ( "--" ) ? a.Substring ( 2 ).Split ( "=" ) : a.Substring ( 1 ).Split ( "=" );
                        return new { Name = pair[0], Value = pair.Length > 1 ? pair[1] : "" };
                    }
                )
                .ToDictionary ( a => a.Name, a => a.Value );
        }

        public static CommandLine Console () => new CommandLine ( new ConsoleCommandLineProvider () );

    }

}
