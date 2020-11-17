using System;

namespace DeadLock.Logic.Observer
{
    public class UpdateParameter
    {
        public UpdateParameter(Guid id, EAction action)
        {
            Id = id;
            Action = action;
        }

        public Guid Id { get; private set; }
        public EAction Action { get; private set; }
    }
}
