using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Wpf_DeadLock.Model
{
    public class Recursos
    {

        public int ID { get; set; }

        public bool Disponivel
        {
            get
            {
                if (Pontos_Disponivel > Processos_Necessarios.Count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int Pontos_Disponivel { get; set; }

        public List<int> Processos_Necessarios { get; set; }
        
        public System.Windows.Controls.Label Texto { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

    }
}
