using DeadLock.Logic.Entity;
using DeadLock.Logic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeadLock.Logic
{
    public class MainLogic
    {
        private List<Process> _processes;
        private List<Resource> _resources;

        public MainLogic()
        {
            _processes = new List<Process>();
            _resources = new List<Resource>();
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
    }
}
