namespace picomessenger
{
    public interface IPicoLogger
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);
    }

    public class NullPicoLogger : IPicoLogger
    {
        public static IPicoLogger Instance { get; } = new NullPicoLogger();


        private NullPicoLogger()
        { }

        public void Info(string message)
        { }

        public void Warn(string message)
        { }

        public void Error(string message)
        { }
    }
}