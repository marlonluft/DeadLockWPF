using System.Collections.Generic;
using System.Windows;
using Wpf_DeadLock.Library.Entity;

namespace Wpf_DeadLock.Library
{
    /// <summary>
    /// Singleton class for access general data used to perform deadlock test.
    /// </summary>
    internal class Data
    {
        private static Data _instance;

        private Data()
        {
            Quantidade_Processos = 0;
            Quantidade_Recursos = 0;
            Processos_Necessitam_Recursos = 0;
            Recursos_Necessitam_Processos = 0;

            Processos = new List<Process>();
            Recursos = new List<Resources>();
            Linhas = new List<LineConnection>();
            Elementos = new List<UIElement>();
        }

        public static Data GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Data();
            }

            return _instance;
        }

        public int Quantidade_Processos { get; set; }
        public int Quantidade_Recursos { get; set; }

        public int Processos_Necessitam_Recursos { get; set; }
        public int Recursos_Necessitam_Processos { get; set; }

        public List<Process> Processos { get; set; }
        public List<Resources> Recursos { get; set; }

        public List<LineConnection> Linhas { get; set; }
        public List<UIElement> Elementos { get; set; }
    }
}
