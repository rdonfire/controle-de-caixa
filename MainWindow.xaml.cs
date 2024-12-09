using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControleDeCaixa
{
    public partial class MainWindow : Window
    {
        private MainWindow mainWindow;
        public MainWindow()
        {
            InitializeComponent();

          
        }

        private void Atualiza_Click(object sender, RoutedEventArgs e)
        {
            RecarregarRegistros();
        } 

        public class RegistroModel
        {
            public int Id { get; set; }
            public string Tipo { get; set; }
            public decimal Valor { get; set; }
            public string Descricao { get; set; }
            public DateTime DataCadastro { get; set; }

            public DateTime? DataAlteracao { get; set; }

        }



        public void RecarregarRegistros()
        {
            var registros = new List<RegistroModel>();
            decimal totalEntradas = 0;
            decimal totalSaidas = 0;

            using (var connection = new Database().GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Registros ORDER BY DataCadastro desc";
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tipo = reader["Tipo"].ToString();
                        var valor = (decimal)reader["Valor"];

                        
                        if (tipo == "Entrada")
                        {
                            totalEntradas += valor;
                        }
                        else if (tipo == "Retirada")
                        {
                            totalSaidas += valor;
                        }

                       
                        registros.Add(new RegistroModel
                        {
                            Id = (int)reader["Id"],
                            Tipo = tipo,
                            Valor = valor,
                            Descricao = reader["Descricao"].ToString(),
                            DataCadastro = (DateTime)reader["DataCadastro"],
                            DataAlteracao = reader["DataAlteracao"] as DateTime?,
                        });
                    }
                }
            }

           
            lblTotalEntradas.Text = $"R$ {totalEntradas:F2}";
            lblTotalSaidas.Text = $"R$ {totalSaidas:F2}";
            lblSaldoTotal.Text = $"R$ {totalEntradas - totalSaidas:F2}";

            
            dgRegistros.ItemsSource = null;  
            dgRegistros.ItemsSource = registros;
            dgRegistros.Items.Refresh();
        }

        private void Adicionar_Click(object sender, RoutedEventArgs e)
        {
            
            if (cbTipo.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione um tipo válido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            string tipo = ((ComboBoxItem)cbTipo.SelectedItem).Content.ToString();

            if (!decimal.TryParse(txtValor.Text, out decimal valor))
            {
                MessageBox.Show("Por favor, insira um valor numérico válido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string descricao = txtDescricao.Text;

            
            try
            {
                using (var connection = new Database().GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO Registros (Tipo, Valor, Descricao, DataCadastro) VALUES (@Tipo, @Valor, @Descricao, @DataCadastro)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Tipo", tipo);
                        command.Parameters.AddWithValue("@Valor", valor);
                        command.Parameters.AddWithValue("@Descricao", descricao);
                        command.Parameters.AddWithValue("@DataCadastro", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Lançamento adicionado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                
                txtValor.Clear(); 
                txtDescricao.Clear(); 
                cbTipo.SelectedItem = null; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao adicionar o lançamento: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            RecarregarRegistros();
        }




        private void Alterar_Click(object sender, RoutedEventArgs e)
        {
            var registroSelecionado = dgRegistros.SelectedItem;
            if (registroSelecionado == null)
            {
                MessageBox.Show("Selecione um registro para alterar", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int id = ((dynamic)registroSelecionado).Id;
            string tipo = ((dynamic)registroSelecionado).Tipo;
            decimal valor = ((dynamic)registroSelecionado).Valor;
            string descricao = ((dynamic)registroSelecionado).Descricao;

            
            WindowEdit editWindow = new WindowEdit(id, tipo, valor, descricao);
            editWindow.ShowDialog(); 
            
            RecarregarRegistros();
        }


        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            if (dgRegistros.SelectedItems.Count > 0)
            {
                var result = MessageBox.Show("Tem certeza que deseja excluir os registros selecionados?", "Confirmar Exclusão", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var connection = new Database().GetConnection())
                        {
                            connection.Open();

                            foreach (var item in dgRegistros.SelectedItems)
                            {
                                if (item is RegistroModel registro)
                                {
                                    string query = "DELETE FROM Registros WHERE Id = @Id";
                                    using (var command = new SqlCommand(query, connection))
                                    {
                                        command.Parameters.AddWithValue("@Id", registro.Id);
                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        MessageBox.Show("Registros excluídos com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Recarregar os registros após a exclusão
                        RecarregarRegistros();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao excluir os registros: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione ao menos um registro para excluir.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        

        

        private void ConfigurarBanco_Click(object sender, RoutedEventArgs e)
        {
            var configuracaoBanco = new ConfiguracaoBanco(this);  
            configuracaoBanco.ShowDialog(); 
        }





        private void Sair_Click(object sender, EventArgs e)
        {
            var confirmacao = MessageBox.Show("Deseja sair? ", "Sair", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmacao == MessageBoxResult.Yes)
            {
                Close();
            }

            return;
        }


    }


}
