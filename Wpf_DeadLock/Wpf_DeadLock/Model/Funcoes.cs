using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using Wpf_DeadLock.Library.Entity;
using Wpf_DeadLock.Library;

namespace Wpf_DeadLock.Model
{
    public class Funcoes
    {
        //============== Funções ====================================
        public void CriarProcesso(double top, double left)
        {
            Rectangle rectangle;
            rectangle = new Rectangle();
            Canvas.SetTop(rectangle, top);
            Canvas.SetLeft(rectangle, left);
            rectangle.Fill = System.Windows.Media.Brushes.White;
            rectangle.Height = 20;
            rectangle.Width = 40;
            rectangle.StrokeThickness = 2;
            rectangle.Stroke = System.Windows.Media.Brushes.Gray;
            

            for (int i = 0; i < Data.GetInstance().Processos.Count; i++)
            {
                if (Data.GetInstance().Processos[i].Left == left && Data.GetInstance().Processos[i].Top == top)
                {
                    Label lbl = new Label();
                    lbl.Content = "P" + Data.GetInstance().Processos[i].Id;
                    Canvas.SetLeft(lbl, left + 10);
                    Canvas.SetTop(lbl, top - 2);

                    rectangle.Name = ("P" + Data.GetInstance().Processos[i].Id);
                    Data.GetInstance().Elementos.Add(rectangle);

                    Data.GetInstance().Elementos.Add(lbl);

                    Data.GetInstance().Processos[i].IdentificationText = lbl;
                    break;
                }
            }

        }
        public void CriarRecurso(double top, double left, int pontos)
        {
            Ellipse ellipse;
            ellipse = new Ellipse();
            ellipse.Fill = System.Windows.Media.Brushes.White;
            ellipse.Height = 40;
            ellipse.Width = 40;
            ellipse.Stroke = System.Windows.Media.Brushes.Gray;
            ellipse.StrokeThickness = 2;            
            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);

            for (int i = 0; i < Data.GetInstance().Recursos.Count; i++)
            {
                if (Data.GetInstance().Recursos[i].Left == left && Data.GetInstance().Recursos[i].Top == top)
                {
                    Label lbl = new Label();
                    lbl.Content = "R" + Data.GetInstance().Recursos[i].Id + "\n";
                    Canvas.SetLeft(lbl, left + 10);
                    Canvas.SetTop(lbl, top - 2);

                    for (int x = 0; x < pontos; x++)
                    {
                        lbl.Content += "•";
                    }

                    ellipse.Name = ("R" + Data.GetInstance().Recursos[i].Id);
                    Data.GetInstance().Elementos.Add(ellipse);

                    Data.GetInstance().Elementos.Add(lbl);

                    Data.GetInstance().Recursos[i].IdentificationText = lbl;
                    break;
                }
            }
        }

        private int cont = 0;
        public void CriarLinha(int ProcessoID, int RecursoID, bool? Deadlock, bool Processo)
        {
            SolidColorBrush[] Strokes = { Brushes.Black, Brushes.Blue, Brushes.Red };
            int x = 0;
            int repetido = 0;

            /*=================================*/
            var processoM = new Process();
            var recursoM = new Resources();

            /*=================================*/
            switch (Processo)
            {
                case true:
                    x = 0;
                    break;
                case false:
                    x = 1;
                    break;
            }

            switch (Deadlock)
            {
                case true:

                    foreach (var item in Data.GetInstance().Linhas)
                    {
                        if (item.ProcessId == ProcessoID &&
                            item.ResourceId == RecursoID &&
                            item.Process == Processo)
                        {
                            if(item.LineDraw.Stroke != Strokes[2])
                            {
                                MessageBox.Show("Possivel DeadLock");
                            }

                            item.LineDraw.Stroke = Strokes[2];                            
                            break;
                        }
                    }
                    break;

                case false:

                    foreach (var item in Data.GetInstance().Linhas)
                    {
                        if (item.ProcessId == ProcessoID &&
                            item.ResourceId == RecursoID &&
                            item.Process == Processo)
                        {
                            Data.GetInstance().Linhas.Remove(item);
                            MessageBox.Show("Linha irá ser Removida");
                            break;
                        }
                    }
                    break;
            }

            if (Deadlock == null)
            {
                foreach (var item in Data.GetInstance().Processos)
                {
                    if (item.Id == ProcessoID)
                    {
                        processoM = item;
                        break;
                    }
                }
                foreach (var item in Data.GetInstance().Recursos)
                {
                    if (item.Id == RecursoID)
                    {
                        recursoM = item;
                        break;
                    }
                }

                System.Windows.Shapes.Path p;
                p = new System.Windows.Shapes.Path();
                p.Stroke = Strokes[x];
                p.StrokeThickness = 2;

                for (int i = 0; i < Data.GetInstance().Linhas.Count; i++)
                {
                    if (Data.GetInstance().Linhas[i].ProcessId == ProcessoID &&
                    Data.GetInstance().Linhas[i].ResourceId == RecursoID)
                    {
                        repetido++;//Espaçamento entre linhas repetidas...
                    }
                }

                if (Processo)
                {
                    //Linhas com pontos de partida de processos
                    if (recursoM.Top < (processoM.Top + 50) && recursoM.Top > (processoM.Top - 50))
                    {
                        if (recursoM.Left > processoM.Left)
                        {
                            //linha direita
                            p.Data = Geometry.Parse(
                                "M " + (processoM.Left + 40) + " " + (processoM.Top + 10) +
                                " L " + ((int)((processoM.Left + recursoM.Left) / 2)) + " " + ((int)(((processoM.Top + 10) + (recursoM.Top + 20)) / 2) + (repetido * 5)) +
                                " L " + recursoM.Left + " " + (recursoM.Top + 20) +
                                " L " + recursoM.Left + " " + ((recursoM.Top + 20) + 5) +
                                " L " + (recursoM.Left + 5) + " " + (recursoM.Top + 20) +
                                " L " + recursoM.Left + " " + ((recursoM.Top + 20) - 5) +
                                " L " + recursoM.Left + " " + (recursoM.Top + 20));
                        }
                        else
                        {
                            //linha esquerda
                            p.Data = Geometry.Parse(
                                "M " + (processoM.Left) + " " + (processoM.Top + 10) +
                                " L " + ((int)((processoM.Left + (recursoM.Left + 40)) / 2)) + " " + ((int)(((processoM.Top + 10) + (recursoM.Top + 20)) / 2) + (repetido * 5)) +
                                " L " + (recursoM.Left + 40) + " " + (recursoM.Top + 20) +
                                " L " + (recursoM.Left + 40) + " " + ((recursoM.Top + 20) + 5) +
                                " L " + ((recursoM.Left + 40) - 5) + " " + (recursoM.Top + 20) +
                                " L " + (recursoM.Left + 40) + " " + ((recursoM.Top + 20) - 5) +
                                " L " + (recursoM.Left + 40) + " " + (recursoM.Top + 20));
                        }
                    }
                    else
                    {
                        if (recursoM.Left > (processoM.Left + 50))
                        {
                            repetido = repetido * 15;
                        }
                        else
                        {
                            repetido = repetido * 5;
                        }

                        if (recursoM.Top > (processoM.Top + 50))
                        {
                            //linha baixo
                            p.Data = Geometry.Parse(
                                "M " + (processoM.Left + 20) + " " + (processoM.Top + 20) +
                                " L " + ((int)(((processoM.Left + 20) + (recursoM.Left)) / 2) + repetido) + " " + ((int)((recursoM.Top + (processoM.Top + 20)) / 2)) +
                                " L " + (recursoM.Left + 20) + " " + recursoM.Top +
                                " L " + ((recursoM.Left + 20) + 5) + " " + (recursoM.Top - 10) +
                                " L " + ((recursoM.Left + 20) - 5) + " " + (recursoM.Top - 10) +
                                " L " + (recursoM.Left + 20) + " " + recursoM.Top);
                        }
                        else
                        {
                            //linha topo
                            p.Data = Geometry.Parse(
                                "M " + (processoM.Left + 20) + " " + processoM.Top +
                                " L " + (((int)((processoM.Left + 20) + (recursoM.Left + 20)) / 2) + repetido) + " " + ((int)(((recursoM.Top + 40) + processoM.Top) / 2)) +
                                " L " + (recursoM.Left + 20) + " " + (recursoM.Top + 40) +
                                " L " + ((recursoM.Left + 20) + 5) + " " + ((recursoM.Top + 40) + 10) +
                                " L " + ((recursoM.Left + 20) - 5) + " " + ((recursoM.Top + 40) + 10) +
                                " L " + (recursoM.Left + 20) + " " + (recursoM.Top + 40));
                        }
                    }

                }
                else
                {
                    //Linhas com pontos de partida de de recursos
                    if (processoM.Top < (recursoM.Top + 50) && processoM.Top > (recursoM.Top - 50))
                    {
                        if (processoM.Left > recursoM.Left)
                        {
                            //linha direita
                            p.Data = Geometry.Parse(
                                "M " + (recursoM.Left + 40) + " " + (recursoM.Top + 20) +
                                " L " + ((int)((recursoM.Left + processoM.Left) / 2)) + " " + ((int)(((recursoM.Top + 20) + (processoM.Top + 10)) / 2) + (repetido * 5)) +
                                " L " + processoM.Left + " " + (processoM.Top + 10) +
                                " L " + processoM.Left + " " + ((processoM.Top + 10) + 5) +
                                " L " + (processoM.Left + 5) + " " + (processoM.Top + 10) +
                                " L " + processoM.Left + " " + ((processoM.Top + 10) - 5) +
                                " L " + processoM.Left + " " + (processoM.Top + 10));
                        }
                        else
                        {
                            //linha esquerda
                            p.Data = Geometry.Parse(
                                "M " + (recursoM.Left) + " " + (recursoM.Top + 20) +
                                " L " + ((int)((recursoM.Left + (processoM.Left + 40)) / 2)) + " " + ((int)(((recursoM.Top + 20) + (processoM.Top + 10)) / 2) + (repetido * 5)) +
                                " L " + (processoM.Left + 40) + " " + (processoM.Top + 10) +
                                " L " + (processoM.Left + 40) + " " + ((processoM.Top + 10) + 5) +
                                " L " + ((processoM.Left + 40) - 5) + " " + (processoM.Top + 10) +
                                " L " + (processoM.Left + 40) + " " + ((processoM.Top + 10) - 5) +
                                " L " + (processoM.Left + 40) + " " + (processoM.Top + 10));
                        }
                    }
                    else
                    {
                        if (recursoM.Left > (processoM.Left + 50))
                        {
                            repetido = repetido * 15;
                        }
                        else
                        {
                            repetido = repetido * 5;
                        }

                        if (processoM.Top > (recursoM.Top + 50))
                        {
                            //linha baixo
                            p.Data = Geometry.Parse(
                                "M " + (recursoM.Left + 20) + " " + (recursoM.Top + 40) +
                                " L " + ((int)(((recursoM.Left + 20) + (processoM.Left)) / 2) + repetido) + " " + ((int)((processoM.Top + (recursoM.Top + 40)) / 2)) +
                                " L " + (processoM.Left + 20) + " " + processoM.Top +
                                " L " + ((processoM.Left + 20) + 5) + " " + (processoM.Top - 10) +
                                " L " + ((processoM.Left + 20) - 5) + " " + (processoM.Top - 10) +
                                " L " + (processoM.Left + 20) + " " + processoM.Top);
                        }
                        else
                        {
                            //linha topo
                            p.Data = Geometry.Parse(
                                "M " + (recursoM.Left + 20) + " " + recursoM.Top +
                                " L " + (((int)((recursoM.Left + 20) + (processoM.Left + 20)) / 2) + repetido) + " " + ((int)(((processoM.Top + 40) + recursoM.Top) / 2)) +
                                " L " + (processoM.Left + 20) + " " + (processoM.Top + 20) +
                                " L " + ((processoM.Left + 20) + 5) + " " + ((processoM.Top + 20) + 10) +
                                " L " + ((processoM.Left + 20) - 5) + " " + ((processoM.Top + 20) + 10) +
                                " L " + (processoM.Left + 20) + " " + (processoM.Top + 20));
                        }
                    }
                }

                cont++;
                LineConnection linha = new LineConnection { Id = cont, ProcessId = ProcessoID, ResourceId = RecursoID, LineDraw = p, Process = Processo };
                Data.GetInstance().Linhas.Add(linha);
            }
        }

        /// <summary>
        /// Criar todos os processos e recursos
        /// </summary>
        public void Ilustrar()
        {
            //Acrescenta os processos e recursos ao desenho
            for (int i = 0; i < Data.GetInstance().Processos.Count; i++)
            {
                CriarProcesso(Data.GetInstance().Processos[i].Top, Data.GetInstance().Processos[i].Left);
            }
            for (int i = 0; i < Data.GetInstance().Recursos.Count; i++)
            {
                CriarRecurso(Data.GetInstance().Recursos[i].Top, Data.GetInstance().Recursos[i].Left, Data.GetInstance().Recursos[i].AvailablePoints);
            }
        }
    }
}