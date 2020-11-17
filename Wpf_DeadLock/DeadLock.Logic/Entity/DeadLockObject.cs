using System;

namespace DeadLock.Logic.Entity
{
    internal abstract class DeadLockObject
    {
        public DeadLockObject()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}
