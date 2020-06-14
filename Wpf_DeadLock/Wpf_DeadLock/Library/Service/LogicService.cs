using System;
using System.Collections.Generic;
using Wpf_DeadLock.Library.Entity;
using Wpf_DeadLock.Model;

namespace Wpf_DeadLock.Library.Service
{
    public static class LogicService
    {
        public static void Process(Action refreshCanvas)
        {
            //Logica do programa
            int vezes = Data.GetInstance().Quantidade_Processos + Data.GetInstance().Quantidade_Recursos;

            bool segundaChance = false; //impede que o programa identifique como deadlock falso
            for (int i = 0; i < vezes; i++)
            {
                CheckEveryProcess(refreshCanvas);
                CheckEveryResource(refreshCanvas);

                refreshCanvas();

                int qtdNecessaria = 0;
                for (int x = 0; x < Data.GetInstance().Processos.Count; x++)
                {
                    if (Data.GetInstance().Processos[x].NeccesariesResources.Count > 0)
                    {
                        qtdNecessaria++;
                    }
                }
                if (qtdNecessaria == Data.GetInstance().Processos_Necessitam_Recursos &&
                    Data.GetInstance().Processos_Necessitam_Recursos > 0)
                {
                    qtdNecessaria = 0;
                    for (int x = 0; x < Data.GetInstance().Recursos.Count; x++)
                    {
                        if (Data.GetInstance().Recursos[x].NeccesariesProcesses.Count > 0)
                        {
                            qtdNecessaria++;
                        }
                    }
                    if (qtdNecessaria == Data.GetInstance().Recursos_Necessitam_Processos)
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
        }

        /// <summary>
        /// Faz a verificação de todos os processos
        /// </summary>
        public static void CheckEveryProcess(Action refreshCanvas)
        {
            for (int i = 0; i < Data.GetInstance().Processos.Count; i++)
            {
                if (Data.GetInstance().Processos[i].NeccesariesResources.Count > 0)
                {
                    for (int q = 0; q < Data.GetInstance().Processos[i].NeccesariesResources.Count; q++)
                    {
                        for (int x = 0; x < Data.GetInstance().Recursos.Count; x++)
                        {
                            if (Data.GetInstance().Processos[i].NeccesariesResources[q] == Data.GetInstance().Recursos[x].Id)
                            {
                                if (Data.GetInstance().Recursos[x].IsAvailable)
                                {
                                    Funcoes.UpdateLine(Data.GetInstance().Processos[i].Id, Data.GetInstance().Recursos[x].Id, false, true);
                                    Data.GetInstance().Processos[i].NeccesariesResources.RemoveAt(q);
                                    refreshCanvas();
                                    break;
                                }
                                else
                                {
                                    for (int m = 0; m < Data.GetInstance().Recursos[x].NeccesariesProcesses.Count; m++)
                                    {
                                        Data.GetInstance().Recursos = Recurso_Unico(Data.GetInstance().Recursos[x].NeccesariesProcesses[m], refreshCanvas);
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
        public static void CheckEveryResource(Action refreshCanvas)
        {
            for (int i = 0; i < Data.GetInstance().Recursos.Count; i++)
            {
                if (Data.GetInstance().Recursos[i].NeccesariesProcesses.Count > 0)
                {
                    for (int q = 0; q < Data.GetInstance().Recursos[i].NeccesariesProcesses.Count; q++)
                    {
                        for (int x = 0; x < Data.GetInstance().Processos.Count; x++)
                        {
                            if (Data.GetInstance().Recursos[i].NeccesariesProcesses[q] == Data.GetInstance().Processos[x].Id)
                            {
                                //Verificado se o processo necessário está diponivel, se não tentará resolver o processo
                                if (Data.GetInstance().Processos[x].IsAvailable)
                                {
                                    Funcoes.UpdateLine(Data.GetInstance().Processos[x].Id, Data.GetInstance().Recursos[i].Id, false, false);
                                    Data.GetInstance().Recursos[i].NeccesariesProcesses.RemoveAt(q);
                                    refreshCanvas();
                                    break;
                                }
                                else
                                {
                                    for (int m = 0; m < Data.GetInstance().Processos[x].NeccesariesResources.Count; m++)
                                    {
                                        Data.GetInstance().Processos = Processo_Unico(Data.GetInstance().Processos[x].NeccesariesResources[m], refreshCanvas);
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
        private static List<Process> Processo_Unico(int ID_Processo, Action refreshCanvas)
        {
            for (int t = 0; t < Data.GetInstance().Processos.Count; t++)
            {
                if (Data.GetInstance().Processos[t].Id == ID_Processo)
                {
                    if (Data.GetInstance().Processos[t].NeccesariesResources.Count > 0)
                    {
                        for (int q = 0; q < Data.GetInstance().Processos[t].NeccesariesResources.Count; q++)
                        {
                            for (int x = 0; x < Data.GetInstance().Recursos.Count; x++)
                            {
                                if (Data.GetInstance().Processos[t].NeccesariesResources[q] == Data.GetInstance().Recursos[x].Id)
                                {
                                    if (Data.GetInstance().Recursos[x].IsAvailable)
                                    {
                                        Funcoes.UpdateLine(Data.GetInstance().Processos[t].Id, Data.GetInstance().Recursos[x].Id, false, true);
                                        Data.GetInstance().Processos[t].NeccesariesResources.RemoveAt(q);
                                        refreshCanvas();
                                        break;
                                    }
                                    else
                                    {
                                        Funcoes.UpdateLine(Data.GetInstance().Processos[t].Id, Data.GetInstance().Recursos[x].Id, true, true);
                                        refreshCanvas();
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
            }
            return Data.GetInstance().Processos;
        }

        /// <summary>
        /// Faz a verificação de um único recurso
        /// </summary>
        /// <param name="ID_Recurso">Id do recurso a ser verificado</param>
        /// <returns></returns>
        private static List<Resources> Recurso_Unico(int ID_Recurso, Action refreshCanvas)
        {
            for (int t = 0; t < Data.GetInstance().Recursos.Count; t++)
            {
                if (Data.GetInstance().Recursos[t].Id == ID_Recurso)
                {
                    if (Data.GetInstance().Recursos[t].NeccesariesProcesses.Count > 0)
                    {
                        for (int q = 0; q < Data.GetInstance().Recursos[t].NeccesariesProcesses.Count; q++)
                        {
                            for (int x = 0; x < Data.GetInstance().Processos.Count; x++)
                            {
                                if (Data.GetInstance().Recursos[t].NeccesariesProcesses[q] == Data.GetInstance().Processos[x].Id)
                                {
                                    if (Data.GetInstance().Processos[x].IsAvailable)
                                    {
                                        Funcoes.UpdateLine(Data.GetInstance().Processos[x].Id, Data.GetInstance().Recursos[t].Id, false, false);
                                        Data.GetInstance().Recursos[t].NeccesariesProcesses.RemoveAt(q);
                                        refreshCanvas();
                                        break;
                                    }
                                    else
                                    {
                                        Funcoes.UpdateLine(Data.GetInstance().Processos[x].Id, Data.GetInstance().Recursos[t].Id, true, false);
                                        refreshCanvas();
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
            }
            return Data.GetInstance().Recursos;
        }
    }
}
