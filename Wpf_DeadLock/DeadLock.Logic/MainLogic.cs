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
        private Subject _subjectObserver;

        public MainLogic()
        {
            _processes = new List<Process>();
            _resources = new List<Resource>();
            _subjectObserver = new Subject();
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

        public void SetResourceDependency(Guid resourceId, Guid processId)
        {
            var resource = _resources.FirstOrDefault(x => x.Id == resourceId);
            var process = _processes.FirstOrDefault(x => x.Id == processId);

            if (resource == null)
            {
                throw new LogicException("Resource not found");
            }
            else if (process == null)
            {
                throw new LogicException("Process not found");
            }

            resource.Dependecies.Add(process.Id);
        }

        public void SetProcessDependency(Guid processId, Guid resourceId)
        {
            var resource = _resources.FirstOrDefault(x => x.Id == resourceId);
            var process = _processes.FirstOrDefault(x => x.Id == processId);

            if (resource == null)
            {
                throw new LogicException("Resource not found");
            }
            else if (process == null)
            {
                throw new LogicException("Process not found");
            }

            resource.LockPoint();

            process.Dependecies.Add(resource.Id);
        }

        public void ExecuteStep()
        {
            // Start by executing resource that's a dependency but depend no one.
            var freeResource =
                _resources.FirstOrDefault(resource =>
                    !resource.Dependecies.Any() &&
                    _processes.Any(process => process.Dependecies.Any(dependency => dependency.Equals(resource.Id)))
                );

            if (freeResource != null)
            {
                // Remove those dependencies on free resource
                _processes.ForEach((process) =>
                {
                    process.Dependecies.Remove(freeResource.Id);
                });

                var updateParameter = new UpdateParameter(freeResource.Id, EAction.UNLOCK);
                _subjectObserver.Notify(updateParameter);
            }
            else
            {
                // Then execute a process that's a dependency but depend no one.
                var freeProcess =
                    _processes.FirstOrDefault(process =>
                        !process.Dependecies.Any() &&
                        _resources.Any(resource => resource.Dependecies.Any(dependency => dependency.Equals(process.Id)))
                    );

                if (freeProcess != null)
                {
                    // Remove those dependencies on free process
                    _resources.ForEach((resource) =>
                    {
                        resource.Dependecies.Remove(freeProcess.Id);
                    });

                    var updateParameter = new UpdateParameter(freeProcess.Id, EAction.UNLOCK);
                    _subjectObserver.Notify(updateParameter);
                }
                else
                {
                    // TODO: execute who's a dependency and have dependencies
                }
            }
        }

        public void AttachObserver(IObserver observer) => _subjectObserver.Attach(observer);
        public void DetachObserver(IObserver observer) => _subjectObserver.Detach(observer);
    }
}
