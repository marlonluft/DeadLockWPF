using System.Collections.Generic;

namespace DeadLock.Logic.Observer
{
    internal class Subject : ISubject
    {
        private List<IObserver> _observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(UpdateParameter parameter)
        {
            _observers.ForEach((observer) => observer.Update(parameter));
        }
    }
}
