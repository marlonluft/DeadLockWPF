using System;
using System.Collections.Generic;

namespace DeadLock.Logic.Entity
{
    public abstract class DeadLockObject
    {
        public DeadLockObject()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public List<Guid> Dependecies { get; set; }
    }
}
