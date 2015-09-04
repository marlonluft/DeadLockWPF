using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wpf_DeadLock.Model
{
    public class Linha
    {

        public int ID { get; set; }

        public int ProcessoID { get; set; }
        public int RecursoID { get; set; }

        public bool Processo { get; set; }

        public System.Windows.Shapes.Path Path { get; set; }

    }
}
