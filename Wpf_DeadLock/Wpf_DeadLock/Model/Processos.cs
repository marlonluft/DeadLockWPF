using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Wpf_DeadLock.Model
{
    public class Processos
    {

        public int ID { get; set; }

        public bool Disponivel
        {
            get
            {
                if (Recursos_Necessarios.Count == 0 || Recursos_Necessarios == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public List<int> Recursos_Necessarios { get; set; }

        public System.Windows.Controls.Label Texto { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

    }
}
