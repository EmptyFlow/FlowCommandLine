namespace FlowCommandLine.SpecialTypes {

    /// <summary>
    /// Range
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommandLineRange<T> where T: struct {

        public T Start { get; set; }

        public T End { get; set; }

    }

}
