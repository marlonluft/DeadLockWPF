using DeadLock.Logic.Entity;
using DeadLock.Logic.Exceptions;
using DeadLock.Logic.Observer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeadLock.Logic
{
    public class MainLogic
    {
        private List<Process> _processes;
        private List<Resource> _resources;
        private List<Dependency> _dependency;
        private bool _firstStep;

        private Subject _subjectObserver;

        public MainLogic()
        {
            _processes = new List<Process>();
            _resources = new List<Resource>();
            _dependency = new List<Dependency>();

            _subjectObserver = new Subject();
            _firstStep = true;
        }

        public Guid CreateProcess()
        {
            var newProcess = new Process();

            _processes.Add(newProcess);

            return newProcess.Id;
        }

        public Guid CreateResource(int availablePoints)
        {
            var newResource = new Resource(availablePoints);

            _resources.Add(newResource);

            return newResource.Id;
        }

        public Guid SetResourceDependency(Guid resourceId, Guid processId)
        {
            ValidateDependencyEntities(processId, resourceId);

            var dependency = new Dependency(processId, resourceId);
            _dependency.Add(dependency);

            return dependency.Id;
        }

        public Guid SetProcessDependency(Guid processId, Guid resourceId)
        {
            ValidateDependencyEntities(processId, resourceId);

            var dependency = new Dependency(processId, resourceId);
            _dependency.Add(dependency);

            return dependency.Id;
        }

        private void ValidateDependencyEntities(Guid processId, Guid resourceId)
        {
            if (!_resources.Any(x => x.Id == resourceId))
            {
                throw new LogicException("Resource not found");
            }
            else if (!_processes.Any(x => x.Id == processId))
            {
                throw new LogicException("Process not found");
            }
        }

        public void ExecuteStep()
        {
            if (_firstStep)
            {
                foreach (var dependecy in _dependency)
                {
                    Notify(dependecy.Id, EAction.LOCK);
                    Notify(dependecy.RecipientId, EAction.LOCK);
                }

                _firstStep = false;
            }
            else
            {
                // Start by executing resource that's a dependency but depend no one.
                var freeResource =
                    _resources.FirstOrDefault(resource =>
                        _dependency.Any(d => d.DependentId == resource.Id) &&
                        !_dependency.Any(r => r.RecipientId == resource.Id)
                    );

                if (freeResource != null)
                {
                    // Remove those dependencies on free resource
                    var dependecies = _dependency.Where(d => d.DependentId == freeResource.Id);

                    for (int i = dependecies.Count() - 1; i >= 0; i--)
                    {
                        Notify(dependecies.ElementAt(i).Id, EAction.REMOVE);
                        _dependency.RemoveAt(i);
                    }

                    Notify(freeResource.Id, EAction.UNLOCK);
                }
                else
                {
                    // Then execute a process that's a dependency but depend no one.
                    var freeProcess =
                        _processes.FirstOrDefault(process =>
                            _dependency.Any(d => d.DependentId == process.Id) &&
                        !_dependency.Any(r => r.RecipientId == process.Id)
                        );

                    if (freeProcess != null)
                    {
                        // Remove those dependencies on free process
                        var dependecies = _dependency.Where(d => d.DependentId == freeProcess.Id);

                        for (int i = dependecies.Count() - 1; i >= 0; i--)
                        {
                            Notify(dependecies.ElementAt(i).Id, EAction.REMOVE);
                            _dependency.RemoveAt(i);
                        }

                        Notify(freeProcess.Id, EAction.UNLOCK);
                    }
                    else
                    {
                        // Check DEADLOCK or ended
                    }
                }
            }
        }

        public void AttachObserver(IObserver observer) => _subjectObserver.Attach(observer);
        public void DetachObserver(IObserver observer) => _subjectObserver.Detach(observer);

        private void Notify(Guid id, EAction action)
        {
            _subjectObserver.Notify(new UpdateParameter(id, action));
        }
    }
}
