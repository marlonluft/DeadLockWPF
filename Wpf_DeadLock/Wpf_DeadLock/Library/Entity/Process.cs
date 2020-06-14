using System.Collections.Generic;
using System.Windows.Controls;

namespace Wpf_DeadLock.Library.Entity
{
    public class Process
    {

        public int Id { get; set; }

        public bool IsAvailable
        {
            get
            {
                if (NeccesariesResources.Count == 0 || NeccesariesResources == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public List<int> NeccesariesResources { get; set; }

        public Label IdentificationText { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

    }
}
