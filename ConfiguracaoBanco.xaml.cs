using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace ControleDeCaixa
{
    public  partial class ConfiguracaoBanco : Window
    {
        private static string caminhoConfig = "configBanco.json";
        private MainWindow mainWindow;

        public ConfiguracaoBanco(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;


            var config = Carregar();
            if (config != null)
            {
                txtServidor.Text = config.Servidor;
                txtNomeBanco.Text = config.NomeBanco;
                txtUsuario.Text = config.Usuario;
                txtSenha.Password = config.Senha;
            }
        }



        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            var config = new ConfiguracaoModel
            {
                Servidor = txtServidor.Text,
                NomeBanco = txtNomeBanco.Text,
                Usuario = txtUsuario.Text,
                Senha = txtSenha.Password
            };

            try
            {
                Salvar(config);
                mainWindow.RecarregarRegistros(); // Chama o método da MainWindow para recarregar o DataGrid
                MessageBox.Show("Configuração salva com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // Fecha a janela de configuração
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configuração: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            
            Close();
        }

        private static void Salvar(ConfiguracaoModel config)
        {
           
            string json = JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(caminhoConfig, json);
        }

        public static ConfiguracaoModel Carregar()
        {
          
            if (File.Exists(caminhoConfig))
            {
                string json = File.ReadAllText(caminhoConfig);
                return JsonConvert.DeserializeObject<ConfiguracaoModel>(json);
            }
            return null;
        }

    }

    public class ConfiguracaoModel
    {
        public string Servidor { get; set; }
        public string NomeBanco { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }
}
