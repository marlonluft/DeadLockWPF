using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using Wpf_DeadLock.Model;
using Wpf_DeadLock.UserControls;

namespace Wpf_DeadLock
{
    /// <summary>
    /// Interaction logic for Informacao.xaml
    /// </summary>
    public partial class Informacao : Window
    {
        public Informacao()
        {
            InitializeComponent();
        }

        private Funcoes funcoes = new Funcoes();

        private void btnRecursosMais_Click(object sender, RoutedEventArgs e)
        {
            UCRescurso recurso = new UCRescurso();
            recurso.txtRecurso.Content = "Recurso " + WPRecursos.Items.Count;            
            WPRecursos.Items.Add(recurso);
            Limpar();            
        }

        private void btnRecursosMenos_Click(object sender, RoutedEventArgs e)
        {
            if (WPRecursos.Items.Count > 0)
            {
                WPRecursos.Items.RemoveAt(WPRecursos.Items.Count - 1);
                Limpar();
            }
        }

        private void btnProcessoMais_Click(object sender, RoutedEventArgs e)
        {
            UCProcesso processo = new UCProcesso();
            processo.lblProcesso.Content = "Processo " + WPProcessos.Items.Count;
            WPProcessos.Items.Add(processo);
            Limpar();
        }

        private void btnProcessoMenos_Click(object sender, RoutedEventArgs e)
        {            
            if (WPProcessos.Items.Count > 0)
            {
                WPProcessos.Items.RemoveAt(WPProcessos.Items.Count - 1);
                Limpar();
            }
        }

        private void btnRecursosMais2_Click(object sender, RoutedEventArgs e)
        {
            //bloquear modificação de processos e recursos
            for (int i = 0; i < WPRecursos2.Items.Count; i++)
            {
                ((UCRequer)WPRecursos2.Items[i]).cmb1.IsEnabled = false;
            }            

            if (WPProcessos.Items.Count > 0 && WPRecursos.Items.Count > 0)
            {
                Bloquear(true);

                UCRequer recurso = new UCRequer();
                for (int i = 0; i < WPRecursos.Items.Count; i++)
                {
                    //Bloquear por questões de pontos
                    int cont = 0;
                    for (int x = 0; x < WPRecursos2.Items.Count; x++)
                    {
                        if (((UCRequer)WPRecursos2.Items[x]).cmb1.SelectedValue.Equals("Recurso " + i))
                        {
                            cont++;
                        }
                    }
                    if (cont < int.Parse(((UCRescurso)WPRecursos.Items[i]).txtPontos.Text))
                    {
                        recurso.cmb1.Items.Add("Recurso " + i);
                    }
                }
                for (int i = 0; i < WPProcessos.Items.Count; i++)
                {
                    recurso.cmb2.Items.Add("Processo " + i);
                }

                recurso.cmb1.SelectedIndex = 0;
                recurso.cmb2.SelectedIndex = 0;

                if (recurso.cmb1.Items.Count > 0 && recurso.cmb2.Items.Count > 0)
                    WPRecursos2.Items.Add(recurso);

                if (WPRecursos2.Items.Count > 0)
                {
                    ((UCRequer)WPRecursos2.Items[(WPRecursos2.Items.Count - 1)]).cmb1.IsEnabled = true;
                }
            }
            else
            {
                MessageBox.Show("Tenha certeza que esteja adicionado ao menos 1 Recurso e 1 Processo para prosseguir.");
            }
        }

        private void btnRecursosMenos2_Click(object sender, RoutedEventArgs e)
        {
            if (WPRecursos2.SelectedIndex >= 0)
            {
                WPRecursos2.Items.RemoveAt(WPRecursos2.SelectedIndex);
            }
            else
            {
              if (WPRecursos2.Items.Count > 0)
              {
                  WPRecursos2.Items.RemoveAt(WPRecursos2.Items.Count - 1);
              }
            }

            if (WPRecursos2.Items.Count > 0)
            {
                ((UCRequer)WPRecursos2.Items[(WPRecursos2.Items.Count-1)]).cmb1.IsEnabled = true;
            }
        }

        private void btnProcessoMais2_Click(object sender, RoutedEventArgs e)
        {
            if (WPProcessos.Items.Count > 0 && WPRecursos.Items.Count > 0)
            {
                //bloquear modificação de processos e recursos
                Bloquear(true);

                UCRequer recurso = new UCRequer();
                for (int i = 0; i < WPProcessos.Items.Count; i++)
                {
                    recurso.cmb1.Items.Add("Processo " + i);
                }
                for (int i = 0; i < WPRecursos.Items.Count; i++)
                {
                    recurso.cmb2.Items.Add("Recurso " + i);
                }

                if (WPProcessos2.Items.Count > 0)
                {
                    recurso.cmb1.SelectedIndex = ((UCRequer)WPProcessos2.Items[WPProcessos2.Items.Count - 1]).cmb1.SelectedIndex;
                }
                else
                {
                    recurso.cmb1.SelectedIndex = 0;
                }
                recurso.cmb2.SelectedIndex = 0;

                WPProcessos2.Items.Add(recurso);
            }
            else
            {
                MessageBox.Show("Tenha certeza que esteja adicionado ao menos 1 Recurso e 1 Processo para prosseguir.");                
            }
        }

        private void btnProcessoMenos2_Click(object sender, RoutedEventArgs e)
        {
            if (WPProcessos2.SelectedIndex >= 0)
            {
                WPProcessos2.Items.RemoveAt(WPProcessos2.SelectedIndex);
            }
            else
            {
                if (WPProcessos2.Items.Count > 0)
                {
                    WPProcessos2.Items.RemoveAt(WPProcessos2.Items.Count - 1);
                }
            }
        }

        private void btnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (WPRecursos2.Items.Count > 0 || WPProcessos2.Items.Count > 0)
            {

                //Inicializa as listas
                funcoes.Processos = new List<Processos>();
                funcoes.Recursos = new List<Recursos>();
                funcoes.Linhas = new List<Linha>();
                funcoes.Elementos = new List<UIElement>();

                //Criar e adiciona a lista os recursos
                int Top = 110;
                int Left = 10;
                for (int i = 0; i < WPRecursos.Items.Count; i++)
                {
                    Recursos recurso = new Recursos();
                    recurso.ID = i;
                    if (string.IsNullOrWhiteSpace(((UCRescurso)WPRecursos.Items[i]).txtPontos.Text))
                    {
                        recurso.Pontos_Disponivel = 1;
                    }
                    else
                    {
                        recurso.Pontos_Disponivel = int.Parse(((UCRescurso)WPRecursos.Items[i]).txtPontos.Text);
                    }
                    recurso.Top = Top;
                    recurso.Left = Left;
                    recurso.Processos_Necessarios = new List<int>();
                    funcoes.Recursos.Add(recurso);
                    Left += 60;
                }

                //Criar e adiciona a lista os processos
                Top = 10;
                Left = 10;
                for (int i = 0; i < WPProcessos.Items.Count; i++)
                {
                    Processos processo = new Processos();
                    processo.ID = i;
                    processo.Top = Top;
                    processo.Left = Left;
                    processo.Recursos_Necessarios = new List<int>();
                    funcoes.Processos.Add(processo);
                    Left += 60;
                }

                /*=================================================================================*/

                //Adiciona os processos necessários pelo recurso
                for (int i = 0; i < WPRecursos2.Items.Count; i++)
                {
                    for (int x = 0; x < funcoes.Recursos.Count; x++)
                    {
                        if (FiltrarNumero(((UCRequer)WPRecursos2.Items[i]).cmb1.SelectedValue.ToString()) == funcoes.Recursos[x].ID)
                        {
                            funcoes.Recursos[x].Processos_Necessarios.Add(FiltrarNumero(((UCRequer)WPRecursos2.Items[i]).cmb2.SelectedValue.ToString()));
                            break;
                        }
                    }
                }

                //Adiciona os recursos necessários pelo processo
                for (int i = 0; i < WPProcessos2.Items.Count; i++)
                {
                    for (int x = 0; x < funcoes.Processos.Count; x++)
                    {
                        if (FiltrarNumero(((UCRequer)WPProcessos2.Items[i]).cmb1.SelectedValue.ToString()) == funcoes.Processos[x].ID)
                        {
                            funcoes.Processos[x].Recursos_Necessarios.Add(FiltrarNumero(((UCRequer)WPProcessos2.Items[i]).cmb2.SelectedValue.ToString()));
                            break;
                        }
                    }
                }

                //Guargar quantidade de processos que necessitam de recursos e vice versa
                for (int i = 0; i < funcoes.Processos.Count; i++)
                {
                    if (funcoes.Processos[i].Recursos_Necessarios.Count > 0)
                    {
                        funcoes.Processos_Necessitam_Recursos++;
                    }
                }
                for (int i = 0; i < funcoes.Recursos.Count; i++)
                {
                    if (funcoes.Recursos[i].Processos_Necessarios.Count > 0)
                    {
                        funcoes.Recursos_Necessitam_Processos++;
                    }
                }

                funcoes.Ilustrar();
                funcoes.Quantidade_Processos = WPProcessos2.Items.Count;
                funcoes.Quantidade_Recursos = WPRecursos2.Items.Count;

                MainWindow main = new MainWindow();
                main.funcoes = funcoes;
                main.Closing += Window_Closing;
                this.Hide();
                main.ShowDialog();
            }
            else
            {
                MessageBox.Show("Adicione alguma condição antes de continuar!");
            }
        }

        private void btnResetar_Click(object sender, RoutedEventArgs e)
        {
            Bloquear(false);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Show();
        }

        /// <summary>
        /// Irá ser chamado caso seja adicionado um novo recurso ou processo
        /// </summary>
        private void Limpar()
        {
            WPRecursos2.Items.Clear();
            WPProcessos2.Items.Clear();
        }

        private void Bloquear(bool bloquear)
        {
            if (bloquear)
            {
                WPRecursos.IsEnabled = false;
                WPProcessos.IsEnabled = false;
                btnRecursosMais.IsEnabled = false;
                btnRecursosMenos.IsEnabled = false;
                btnProcessoMais.IsEnabled = false;
                btnProcessoMenos.IsEnabled = false;
            }
            else
            {
                WPRecursos.IsEnabled = true;
                WPProcessos.IsEnabled = true;
                btnRecursosMais.IsEnabled = true;
                btnRecursosMenos.IsEnabled = true;
                btnProcessoMais.IsEnabled = true;
                btnProcessoMenos.IsEnabled = true;

                WPProcessos2.Items.Clear();
                WPRecursos2.Items.Clear();
            }
        }

        private int FiltrarNumero(string valor)
        {
            if (valor.Contains("Processo"))
            {
                valor = valor.Replace("Processo ", string.Empty);
            }
            else
            {
                valor = valor.Replace("Recurso ", string.Empty);
            }
            return int.Parse(valor);
        }

        private void WPRecursos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            WPRecursos.SelectedIndex = -1;
        }

        private void WPProcessos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            WPProcessos.SelectedIndex = -1;
        }

    }
}