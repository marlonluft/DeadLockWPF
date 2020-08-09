using System.Collections.Generic;

namespace DeadLock.Logic.Entity
{
    public class Resource : DeadLockObject
    {
        public List<ResourcePoint> AvailablePoints { get; set; }
    }
}
