using System.Collections.Generic;

namespace Wpf_DeadLock.Library.Entity
{
    public class Resources
    {

        public int Id { get; set; }

        public bool IsAvailable
        {
            get
            {
                if (AvailablePoints > NeccesariesProcesses.Count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int AvailablePoints { get; set; }

        public List<int> NeccesariesProcesses { get; set; }
        
        public System.Windows.Controls.Label IdentificationText { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

    }
}
