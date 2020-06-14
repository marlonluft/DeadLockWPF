using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf_DeadLock.Library;
using Wpf_DeadLock.Library.Entity;
using Wpf_DeadLock.Library.Service;
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
            for (int i = 0; i < Data.GetInstance().Recursos.Count; i++)
            {
                for (int x = 0; x < Data.GetInstance().Recursos[i].NeccesariesProcesses.Count; x++)
                {
                    Funcoes.CriarLinha(Data.GetInstance().Recursos[i].NeccesariesProcesses[x], Data.GetInstance().Recursos[i].Id, null, false);
                }
            }

            for (int i = 0; i < Data.GetInstance().Processos.Count; i++)
            {
                for (int x = 0; x < Data.GetInstance().Processos[i].NeccesariesResources.Count; x++)
                {
                    Funcoes.CriarLinha(Data.GetInstance().Processos[i].Id, Data.GetInstance().Processos[i].NeccesariesResources[x], null, true);
                }
            }
            AtualizarCanvas();            
            MessageBox.Show("Linhas Adicionadas");

            LogicService.Process(AtualizarCanvas);
            
            //Mostrar resultado
            StringBuilder builder = new StringBuilder();
            builder.Append("============= O sistema está em DeadLock! =============\n");

            bool deadLock = false;
            for (int i = 0; i < Data.GetInstance().Processos.Count; i++)
            {
                if (Data.GetInstance().Processos[i].NeccesariesResources.Count > 0)
                {
                    for (int x = 0; x < Data.GetInstance().Processos[i].NeccesariesResources.Count; x++)
                    {
                        builder.Append("- O Processo " + Data.GetInstance().Processos[i].Id + " necessita do Recurso " + Data.GetInstance().Processos[i].NeccesariesResources[x] + " \n");
                    }

                    deadLock = true;
                }
            }

            for (int i = 0; i < Data.GetInstance().Recursos.Count; i++)
            {
                if (Data.GetInstance().Recursos[i].NeccesariesProcesses.Count > 0)
                {
                    for (int x = 0; x < Data.GetInstance().Recursos[i].NeccesariesProcesses.Count; x++)
                    {
                        builder.Append("- O Recurso " + Data.GetInstance().Recursos[i].Id + " necessita do Processo " + Data.GetInstance().Recursos[i].NeccesariesProcesses[x] + " \n");
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
            Data.GetInstance().Recursos_Necessitam_Processos = 0;
            Data.GetInstance().Processos_Necessitam_Recursos = 0;
            Data.GetInstance().Processos.Clear();
            Data.GetInstance().Recursos.Clear();
            Data.GetInstance().Elementos.Clear();
            Data.GetInstance().Linhas.Clear();

            CanvasMaroto.Children.Clear();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CanvasMaroto.Children.Clear();

            foreach (var item in Data.GetInstance().Elementos)
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

            foreach (var item in Data.GetInstance().Elementos)
            {
                item.MouseLeftButtonDown += shape_MouseLeftButtonDown;
                item.MouseMove += shape_MouseMove;
                item.MouseLeftButtonUp += shape_MouseLeftButtonUp;
                CanvasMaroto.Children.Add(item);
            }

            foreach (var item in Data.GetInstance().Linhas)
            {
                CanvasMaroto.Children.Add(item.LineDraw);
            }            
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
                    for (int i = 0; i < Data.GetInstance().Processos.Count; i++)
                    {
                        if (Data.GetInstance().Processos[i].Left == x_shape && Data.GetInstance().Processos[i].Top == y_shape)
                        {
                            IDElement = Data.GetInstance().Processos[i].Id;
                            break;
                        }
                    }
                }

                //Recurso
                if (source is System.Windows.Shapes.Ellipse)
                {
                    for (int i = 0; i < Data.GetInstance().Recursos.Count; i++)
                    {
                        if (Data.GetInstance().Recursos[i].Left == x_shape && Data.GetInstance().Recursos[i].Top == y_shape)
                        {
                            IDElement = Data.GetInstance().Recursos[i].Id;
                            break;
                        }
                    }
                }

                //Label
                if (source is Label)
                {
                    for (int i = 0; i < Data.GetInstance().Processos.Count; i++)
                    {
                        if (Canvas.GetLeft(Data.GetInstance().Processos[i].IdentificationText) == x_shape && Canvas.GetTop(Data.GetInstance().Processos[i].IdentificationText) == y_shape)
                        {
                            IDElement = Data.GetInstance().Processos[i].Id;
                            ProcessoBool = true;
                            break;
                        }
                    }
                    if (IDElement == -1)
                    {
                        for (int i = 0; i < Data.GetInstance().Recursos.Count; i++)
                        {
                            if (Canvas.GetLeft(Data.GetInstance().Recursos[i].IdentificationText) == x_shape && Canvas.GetTop(Data.GetInstance().Recursos[i].IdentificationText) == y_shape)
                            {
                                IDElement = Data.GetInstance().Recursos[i].Id;
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
                    for (int i = 0; i < Data.GetInstance().Processos.Count; i++)
                    {
                        if (Data.GetInstance().Processos[i].Id == IDElement)
                        {
                            Data.GetInstance().Processos[i].Left = x_shape;
                            Data.GetInstance().Processos[i].Top = y_shape;

                            for (int q = 0; q < CanvasMaroto.Children.Count; q++)
                            {
                                if (CanvasMaroto.Children[q] is Label)
                                {
                                    if (((Label)CanvasMaroto.Children[q]).Content.ToString() == "P" + Data.GetInstance().Processos[i].Id)
                                    {
                                        Canvas.SetLeft(CanvasMaroto.Children[q], Data.GetInstance().Processos[i].Left + 10);
                                        Canvas.SetTop(CanvasMaroto.Children[q], Data.GetInstance().Processos[i].Top - 2);
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
                    for (int i = 0; i < Data.GetInstance().Recursos.Count; i++)
                    {
                        if (Data.GetInstance().Recursos[i].Id == IDElement)
                        {
                            Data.GetInstance().Recursos[i].Left = x_shape;
                            Data.GetInstance().Recursos[i].Top = y_shape;

                            for (int q = 0; q < CanvasMaroto.Children.Count; q++)
                            {
                                if (CanvasMaroto.Children[q] is Label)
                                {
                                    if (((Label)CanvasMaroto.Children[q]).Content.ToString().Contains("R" + Data.GetInstance().Recursos[i].Id))
                                    {
                                        Canvas.SetLeft(CanvasMaroto.Children[q], Data.GetInstance().Recursos[i].Left + 10);
                                        Canvas.SetTop(CanvasMaroto.Children[q], Data.GetInstance().Recursos[i].Top - 2);
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
                        for (int i = 0; i < Data.GetInstance().Processos.Count; i++)
                        {
                            if (Data.GetInstance().Processos[i].Id == IDElement)
                            {
                                for (int q = 0; q < CanvasMaroto.Children.Count; q++)
                                {
                                    if (CanvasMaroto.Children[q] is System.Windows.Shapes.Rectangle)
                                    {
                                        if (Canvas.GetLeft(CanvasMaroto.Children[q]) == Data.GetInstance().Processos[i].Left &&
                                            Canvas.GetTop(CanvasMaroto.Children[q]) == Data.GetInstance().Processos[i].Top &&
                                            ((System.Windows.Shapes.Rectangle)CanvasMaroto.Children[q]).Name == ("P"+Data.GetInstance().Processos[i].Id))
                                        {
                                            Canvas.SetLeft(CanvasMaroto.Children[q], (x_shape - 10));
                                            Canvas.SetTop(CanvasMaroto.Children[q], (y_shape + 2));
                                            
                                            Data.GetInstance().Processos[i].Left = x_shape - 10;
                                            Data.GetInstance().Processos[i].Top = y_shape + 2;

                                            Canvas.SetLeft(Data.GetInstance().Processos[i].IdentificationText, x_shape);
                                            Canvas.SetTop(Data.GetInstance().Processos[i].IdentificationText, y_shape);   

                                            break;
                                        }
                                    }
                                }                                                         
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Data.GetInstance().Recursos.Count; i++)
                        {
                            if (Data.GetInstance().Recursos[i].Id == IDElement)
                            {
                                for (int q = 0; q < CanvasMaroto.Children.Count; q++)
                                {
                                    if (CanvasMaroto.Children[q] is System.Windows.Shapes.Ellipse)
                                    {
                                        if (Canvas.GetLeft(CanvasMaroto.Children[q]) == Data.GetInstance().Recursos[i].Left &&
                                            Canvas.GetTop(CanvasMaroto.Children[q]) == Data.GetInstance().Recursos[i].Top &&
                                            ((System.Windows.Shapes.Ellipse)CanvasMaroto.Children[q]).Name == ("R" + Data.GetInstance().Recursos[i].Id))
                                        {
                                            Canvas.SetLeft(CanvasMaroto.Children[q], (x_shape - 10));
                                            Canvas.SetTop(CanvasMaroto.Children[q], (y_shape + 2));


                                            Data.GetInstance().Recursos[i].Left = x_shape - 10;
                                            Data.GetInstance().Recursos[i].Top = y_shape + 2;

                                            Canvas.SetLeft(Data.GetInstance().Recursos[i].IdentificationText, x_shape);
                                            Canvas.SetTop(Data.GetInstance().Recursos[i].IdentificationText, y_shape);

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