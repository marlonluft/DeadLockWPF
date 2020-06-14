using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf_DeadLock.Library.Entity;
using Wpf_DeadLock.Model;

namespace Wpf_DeadLock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public Funcoes funcoes;

        //============== Botões =====================================

        /// <summary>
        /// Faz a lógica do programa ao ser clicado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            btnEnviar.Focusable = false;//retirar o foco do botão

            //Criar linhas
            for (int i = 0; i < funcoes.Recursos.Count; i++)
            {
                for (int x = 0; x < funcoes.Recursos[i].NeccesariesProcesses.Count; x++)
                {
                    funcoes.CriarLinha(funcoes.Recursos[i].NeccesariesProcesses[x], funcoes.Recursos[i].Id, null, false);
                }
            }

            for (int i = 0; i < funcoes.Processos.Count; i++)
            {
                for (int x = 0; x < funcoes.Processos[i].Recursos_Necessarios.Count; x++)
                {
                    funcoes.CriarLinha(funcoes.Processos[i].ID, funcoes.Processos[i].Recursos_Necessarios[x], null, true);
                }
            }
            AtualizarCanvas();            
            MessageBox.Show("Linhas Adicionadas");

            //Logica do programa
            int vezes = funcoes.Quantidade_Processos + funcoes.Quantidade_Recursos;

            bool segundaChance=false; //impede que o programa identifique como deadlock falso
            for (int i = 0; i < vezes; i++)
            {
                Processo();
                Recurso();

                AtualizarCanvas();

                int qtdNecessaria = 0;
                for (int x = 0; x < funcoes.Processos.Count; x++)
                {
                    if (funcoes.Processos[x].Recursos_Necessarios.Count > 0)
                    {
                        qtdNecessaria++;
                    }
                }
                if (qtdNecessaria == funcoes.Processos_Necessitam_Recursos && 
                    funcoes.Processos_Necessitam_Recursos>0)
                {
                    qtdNecessaria = 0;
                    for (int x = 0; x < funcoes.Recursos.Count; x++)
                    {
                        if (funcoes.Recursos[x].NeccesariesProcesses.Count > 0)
                        {
                            qtdNecessaria++;
                        }
                    }
                    if (qtdNecessaria == funcoes.Recursos_Necessitam_Processos)
                    {
                        if (segundaChance)
                        {
                            break;
                        }
                        else
                        {
                            segundaChance = true;
                        }
                    }
                }

            }

            //Mostrar resultado
            StringBuilder builder = new StringBuilder();
            builder.Append("============= O sistema está em DeadLock! =============\n");

            bool deadLock = false;
            for (int i = 0; i < funcoes.Processos.Count; i++)
            {
                if (funcoes.Processos[i].Recursos_Necessarios.Count > 0)
                {
                    for (int x = 0; x < funcoes.Processos[i].Recursos_Necessarios.Count; x++)
                    {
                        builder.Append("- O Processo " + funcoes.Processos[i].ID + " necessita do Recurso " + funcoes.Processos[i].Recursos_Necessarios[x] + " \n");
                    }

                    deadLock = true;
                }
            }

            for (int i = 0; i < funcoes.Recursos.Count; i++)
            {
                if (funcoes.Recursos[i].NeccesariesProcesses.Count > 0)
                {
                    for (int x = 0; x < funcoes.Recursos[i].NeccesariesProcesses.Count; x++)
                    {
                        builder.Append("- O Recurso " + funcoes.Recursos[i].Id + " necessita do Processo " + funcoes.Recursos[i].NeccesariesProcesses[x] + " \n");
                    }

                    deadLock = true;
                }
            }

            if (deadLock)
            {
                lblStatus.Content += "DeadLock";
                MessageBox.Show(builder.ToString());
            }
            else
            {
                lblStatus.Content += "Safe";
                MessageBox.Show("O sistema NÃO está em DeadLock!");
            }
        }

        /// <summary>
        /// Limpar os campos da tela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            funcoes.Recursos_Necessitam_Processos = 0;
            funcoes.Processos_Necessitam_Recursos = 0;
            funcoes.Processos.Clear();
            funcoes.Recursos.Clear();
            funcoes.Elementos.Clear();
            funcoes.Linhas.Clear();

            CanvasMaroto.Children.Clear();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CanvasMaroto.Children.Clear();

            foreach (var item in funcoes.Elementos)
            {
                item.MouseLeftButtonDown += shape_MouseLeftButtonDown;
                item.MouseMove += shape_MouseMove;
                item.MouseLeftButtonUp += shape_MouseLeftButtonUp;
                CanvasMaroto.Children.Add(item);
            }
        }

        //============== Funções =====================================
        private void AtualizarCanvas()
        {            
            CanvasMaroto.Children.Clear();

            foreach (var item in funcoes.Elementos)
            {
                item.MouseLeftButtonDown += shape_MouseLeftButtonDown;
                item.MouseMove += shape_MouseMove;
                item.MouseLeftButtonUp += shape_MouseLeftButtonUp;
                CanvasMaroto.Children.Add(item);
            }

            foreach (var item in funcoes.Linhas)
            {
                CanvasMaroto.Children.Add(item.LineDraw);
            }            
        }

        //============== Lógica ======================================
        /// <summary>
        /// Faz a verificação de todos os processos
        /// </summary>
        public void Processo()
        {
            for (int i = 0; i < funcoes.Processos.Count; i++)
            {
                if (funcoes.Processos[i].Recursos_Necessarios.Count > 0)
                {
                    for (int q = 0; q < funcoes.Processos[i].Recursos_Necessarios.Count; q++)
                    {
                        for (int x = 0; x < funcoes.Recursos.Count; x++)
                        {
                            if (funcoes.Processos[i].Recursos_Necessarios[q] == funcoes.Recursos[x].Id)
                            {
                                if (funcoes.Recursos[x].IsAvailable)
                                {
                                    funcoes.CriarLinha(funcoes.Processos[i].ID, funcoes.Recursos[x].Id, false, true);
                                    funcoes.Processos[i].Recursos_Necessarios.RemoveAt(q);
                                    AtualizarCanvas();
                                    //MessageBox.Show("Linha removida");
                                    break;
                                }
                                else
                                {
                                    for (int m = 0; m < funcoes.Recursos[x].NeccesariesProcesses.Count; m++)
                                    {
                                        funcoes.Recursos = Recurso_Unico(funcoes.Recursos[x].NeccesariesProcesses[m]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Faz a verificação de todos os recursos
        /// </summary>
        public void Recurso()
        {
            for (int i = 0; i < funcoes.Recursos.Count; i++)
            {
                if (funcoes.Recursos[i].NeccesariesProcesses.Count > 0)
                {
                    for (int q = 0; q < funcoes.Recursos[i].NeccesariesProcesses.Count; q++)
                    {
                        for (int x = 0; x < funcoes.Processos.Count; x++)
                        {
                            if (funcoes.Recursos[i].NeccesariesProcesses[q] == funcoes.Processos[x].ID)
                            {
                                //Verificado se o processo necessário está diponivel, se não tentará resolver o processo
                                if (funcoes.Processos[x].Disponivel)
                                {
                                    funcoes.CriarLinha(funcoes.Processos[x].ID, funcoes.Recursos[i].Id, false, false);
                                    funcoes.Recursos[i].NeccesariesProcesses.RemoveAt(q);
                                    AtualizarCanvas();
                                    //MessageBox.Show("Linha removida");
                                    break;
                                }
                                else
                                {
                                    for (int m = 0; m < funcoes.Processos[x].Recursos_Necessarios.Count; m++)
                                    {
                                        funcoes.Processos = Processo_Unico(funcoes.Processos[x].Recursos_Necessarios[m]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Faz a verificação de um único Processo
        /// </summary>
        /// <param name="ID_Processo">Id do processo a ser verificado</param>
        /// <returns></returns>
        private List<Processos> Processo_Unico(int ID_Processo)
        {
            for (int t = 0; t < funcoes.Processos.Count; t++)
            {
                if (funcoes.Processos[t].ID == ID_Processo)
                {
                    if (funcoes.Processos[t].Recursos_Necessarios.Count > 0)
                    {
                        for (int q = 0; q < funcoes.Processos[t].Recursos_Necessarios.Count; q++)
                        {
                            for (int x = 0; x < funcoes.Recursos.Count; x++)
                            {
                                if (funcoes.Processos[t].Recursos_Necessarios[q] == funcoes.Recursos[x].Id)
                                {
                                    if (funcoes.Recursos[x].IsAvailable)
                                    {
                                        funcoes.CriarLinha(funcoes.Processos[t].ID, funcoes.Recursos[x].Id, false, true);
                                        funcoes.Processos[t].Recursos_Necessarios.RemoveAt(q);
                                        AtualizarCanvas();
                                        //MessageBox.Show("Linha removida");
                                        break;
                                    }
                                    else
                                    {
                                        funcoes.CriarLinha(funcoes.Processos[t].ID, funcoes.Recursos[x].Id, true, true);
                                        AtualizarCanvas();
                                        //MessageBox.Show("Possivel DeadLock");
                                    }                                    
                                }
                            }
                        }
                    }
                    break;
                }
            }
            return funcoes.Processos;
        }

        /// <summary>
        /// Faz a verificação de um único recurso
        /// </summary>
        /// <param name="ID_Recurso">Id do recurso a ser verificado</param>
        /// <returns></returns>
        private List<Resources> Recurso_Unico(int ID_Recurso)
        {
            for (int t = 0; t < funcoes.Recursos.Count; t++)
            {
                if (funcoes.Recursos[t].Id == ID_Recurso)
                {
                    if (funcoes.Recursos[t].NeccesariesProcesses.Count > 0)
                    {
                        for (int q = 0; q < funcoes.Recursos[t].NeccesariesProcesses.Count; q++)
                        {
                            for (int x = 0; x < funcoes.Processos.Count; x++)
                            {
                                if (funcoes.Recursos[t].NeccesariesProcesses[q] == funcoes.Processos[x].ID)
                                {
                                    if (funcoes.Processos[x].Disponivel)
                                    {
                                        funcoes.CriarLinha(funcoes.Processos[x].ID, funcoes.Recursos[t].Id, false, false);
                                        funcoes.Recursos[t].NeccesariesProcesses.RemoveAt(q);
                                        AtualizarCanvas();
                                        //MessageBox.Show("Linha removida");
                                        break;
                                    }
                                    else
                                    {
                                        funcoes.CriarLinha(funcoes.Processos[x].ID, funcoes.Recursos[t].Id, true, false);
                                        AtualizarCanvas();
                                        //MessageBox.Show("Possivel DeadLock");
                                    }                                                                        
                                }
                            }
                        }
                    }
                    break;
                }
            }
            return funcoes.Recursos;
        }

        /*============ Mover objetos =================================*/
        private bool captured = false;
        private double x_shape, x_canvas, y_shape, y_canvas;
        private UIElement source = null;
        private int IDElement=-1;
        private bool ProcessoBool;

        private void shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            source = (UIElement)sender;
            Mouse.Capture(source);
            captured = true;
            x_shape = Canvas.GetLeft(source);
            x_canvas = e.GetPosition(CanvasMaroto).X;
            y_shape = Canvas.GetTop(source);
            y_canvas = e.GetPosition(CanvasMaroto).Y;
        }

        private void shape_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                //Processo
                if (source is System.Windows.Shapes.Rectangle)
                {
                    for (int i = 0; i < funcoes.Processos.Count; i++)
                    {
                        if (funcoes.Processos[i].Left == x_shape && funcoes.Processos[i].Top == y_shape)
                        {
                            IDElement = funcoes.Processos[i].ID;
                            break;
                        }
                    }
                }

                //Recurso
                if (source is System.Windows.Shapes.Ellipse)
                {
                    for (int i = 0; i < funcoes.Recursos.Count; i++)
                    {
                        if (funcoes.Recursos[i].Left == x_shape && funcoes.Recursos[i].Top == y_shape)
                        {
                            IDElement = funcoes.Recursos[i].Id;
                            break;
                        }
                    }
                }

                //Label
                if (source is Label)
                {
                    for (int i = 0; i < funcoes.Processos.Count; i++)
                    {
                        if (Canvas.GetLeft(funcoes.Processos[i].Texto) == x_shape && Canvas.GetTop(funcoes.Processos[i].Texto) == y_shape)
                        {
                            IDElement = funcoes.Processos[i].ID;
                            ProcessoBool = true;
                            break;
                        }
                    }
                    if (IDElement == -1)
                    {
                        for (int i = 0; i < funcoes.Recursos.Count; i++)
                        {
                            if (Canvas.GetLeft(funcoes.Recursos[i].IdentificationText) == x_shape && Canvas.GetTop(funcoes.Recursos[i].IdentificationText) == y_shape)
                            {
                                IDElement = funcoes.Recursos[i].Id;
                                ProcessoBool = false;
                                break;
                            }
                        }
                    }
                }

                double x = e.GetPosition(CanvasMaroto).X;
                double y = e.GetPosition(CanvasMaroto).Y;
                x_shape += x - x_canvas;
                Canvas.SetLeft(source, x_shape);
                x_canvas = x;
                y_shape += y - y_canvas;
                Canvas.SetTop(source, y_shape);
                y_canvas = y;

                //Processo
                if (source is System.Windows.Shapes.Rectangle)
                {
                    for (int i = 0; i < funcoes.Processos.Count; i++)
                    {
                        if (funcoes.Processos[i].ID == IDElement)
                        {
                            funcoes.Processos[i].Left = x_shape;
                            funcoes.Processos[i].Top = y_shape;

                            for (int q = 0; q < CanvasMaroto.Children.Count; q++)
                            {
                                if (CanvasMaroto.Children[q] is Label)
                                {
                                    if (((Label)CanvasMaroto.Children[q]).Content.ToString() == "P" + funcoes.Processos[i].ID)
                                    {
                                        Canvas.SetLeft(CanvasMaroto.Children[q], funcoes.Processos[i].Left + 10);
                                        Canvas.SetTop(CanvasMaroto.Children[q], funcoes.Processos[i].Top - 2);
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }

                //Recurso
                if (source is System.Windows.Shapes.Ellipse)
                {
                    for (int i = 0; i < funcoes.Recursos.Count; i++)
                    {
                        if (funcoes.Recursos[i].Id == IDElement)
                        {
                            funcoes.Recursos[i].Left = x_shape;
                            funcoes.Recursos[i].Top = y_shape;

                            for (int q = 0; q < CanvasMaroto.Children.Count; q++)
                            {
                                if (CanvasMaroto.Children[q] is Label)
                                {
                                    if (((Label)CanvasMaroto.Children[q]).Content.ToString().Contains("R" + funcoes.Recursos[i].Id))
                                    {
                                        Canvas.SetLeft(CanvasMaroto.Children[q], funcoes.Recursos[i].Left + 10);
                                        Canvas.SetTop(CanvasMaroto.Children[q], funcoes.Recursos[i].Top - 2);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }


                //Label
                if(source is Label)
                {
                    if (ProcessoBool)
                    {
                        for (int i = 0; i < funcoes.Processos.Count; i++)
                        {
                            if (funcoes.Processos[i].ID == IDElement)
                            {
                                for (int q = 0; q < CanvasMaroto.Children.Count; q++)
                                {
                                    if (CanvasMaroto.Children[q] is System.Windows.Shapes.Rectangle)
                                    {
                                        if (Canvas.GetLeft(CanvasMaroto.Children[q]) == funcoes.Processos[i].Left &&
                                            Canvas.GetTop(CanvasMaroto.Children[q]) == funcoes.Processos[i].Top &&
                                            ((System.Windows.Shapes.Rectangle)CanvasMaroto.Children[q]).Name == ("P"+funcoes.Processos[i].ID))
                                        {
                                            Canvas.SetLeft(CanvasMaroto.Children[q], (x_shape - 10));
                                            Canvas.SetTop(CanvasMaroto.Children[q], (y_shape + 2));
                                            
                                            funcoes.Processos[i].Left = x_shape - 10;
                                            funcoes.Processos[i].Top = y_shape + 2;

                                            Canvas.SetLeft(funcoes.Processos[i].Texto, x_shape);
                                            Canvas.SetTop(funcoes.Processos[i].Texto, y_shape);   

                                            break;
                                        }
                                    }
                                }                                                         
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < funcoes.Recursos.Count; i++)
                        {
                            if (funcoes.Recursos[i].Id == IDElement)
                            {
                                for (int q = 0; q < CanvasMaroto.Children.Count; q++)
                                {
                                    if (CanvasMaroto.Children[q] is System.Windows.Shapes.Ellipse)
                                    {
                                        if (Canvas.GetLeft(CanvasMaroto.Children[q]) == funcoes.Recursos[i].Left &&
                                            Canvas.GetTop(CanvasMaroto.Children[q]) == funcoes.Recursos[i].Top &&
                                            ((System.Windows.Shapes.Ellipse)CanvasMaroto.Children[q]).Name == ("R" + funcoes.Recursos[i].Id))
                                        {
                                            Canvas.SetLeft(CanvasMaroto.Children[q], (x_shape - 10));
                                            Canvas.SetTop(CanvasMaroto.Children[q], (y_shape + 2));


                                            funcoes.Recursos[i].Left = x_shape - 10;
                                            funcoes.Recursos[i].Top = y_shape + 2;

                                            Canvas.SetLeft(funcoes.Recursos[i].IdentificationText, x_shape);
                                            Canvas.SetTop(funcoes.Recursos[i].IdentificationText, y_shape);

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        private void shape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            captured = false;
            IDElement = -1;
        }

    }
}