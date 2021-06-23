using System;

namespace IInputValidiatorNamespace
{
    public class ValidationEvent : EventArgs
    {
        public ValidationEvent(string source)
        {
            EventSource = source;
        }

        public readonly string EventSource;
    }
}
