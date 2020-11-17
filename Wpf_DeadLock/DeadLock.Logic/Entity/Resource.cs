using DeadLock.Logic.Exceptions;

namespace DeadLock.Logic.Entity
{
    internal class Resource : DeadLockObject
    {
        public Resource(int quantityPoints) : base()
        {
            QuantityPoints = quantityPoints;
            AvailablePoints = quantityPoints;
        }

        private readonly int QuantityPoints;

        private int AvailablePoints { get; set; }

        internal void LockPoint()
        {
            if (AvailablePoints == 0)
            {
                throw new LogicException("No points available");
            }

            AvailablePoints--;
        }
    }
}
