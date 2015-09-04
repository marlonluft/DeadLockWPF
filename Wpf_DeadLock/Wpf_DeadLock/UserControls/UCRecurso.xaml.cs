using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Wpf_DeadLock
{
    /// <summary>
    /// Interaction logic for UCRescurso1.xaml
    /// </summary>
    public partial class UCRescurso : UserControl
    {
        public UCRescurso()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Valida se é numéro o conteudo das textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidacaoNumeroTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtPontos.Text = "1";
        }
    }
}
