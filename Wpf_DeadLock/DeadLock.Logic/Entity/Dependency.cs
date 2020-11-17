using System;

namespace DeadLock.Logic.Entity
{
    public class Dependency
    {
        public Dependency(Guid recipientId, Guid dependentId)
        {
            Id = Guid.NewGuid();
            RecipientId = recipientId;
            DependentId = dependentId;
        }

        public Guid Id { get; set; }
        public Guid RecipientId { get; set; }
        public Guid DependentId { get; set; }
    }
}
