using System;
using System.Collections.Generic;

namespace DeadLock.Logic.Entity
{
    internal abstract class DeadLockObject
    {
        public DeadLockObject()
        {
            Id = Guid.NewGuid();
            Dependecies = new List<Guid>();
        }

        public Guid Id { get; set; }
        public List<Guid> Dependecies { get; set; }
    }
}
