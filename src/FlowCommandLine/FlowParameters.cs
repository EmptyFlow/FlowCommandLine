using System.Dynamic;

namespace FlowCommandLine {

    public class FlowParameters : DynamicObject {

        private readonly Dictionary<string, string> m_parameters;

        /// <summary>
        /// Create class that is wrapper on Dictionary.
        /// </summary>
        /// <param name="parameters">Dictionary with parameters.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FlowParameters ( Dictionary<string, string> parameters ) {
            m_parameters = parameters ?? throw new ArgumentNullException ( nameof ( parameters ) );
        }

        public override IEnumerable<string> GetDynamicMemberNames () => m_parameters.Keys;

        public override bool TrySetMember ( SetMemberBinder binder, object? value ) {
            return false;
        }

        public override bool TryGetMember ( GetMemberBinder binder, out object? result ) {
            result = null;
            if ( m_parameters.ContainsKey ( binder.Name ) ) {
                result = m_parameters[binder.Name];
            }
            return true;
        }

    }
}
