using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControleDeCaixa
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

        private void Botao1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Botão 1 clicado!");
        }

        private void Botao2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Botão 2 clicado!");
        }
    }
}
