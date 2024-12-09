using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ControleDeCaixa
{
   
    public partial class WindowEdit : Window
    {
        public int RegistroId { get; set; }
        public string TipoAnterior { get; set; }
        public decimal ValorAnterior { get; set; }
        public string DescricaoAnterior { get; set; }

        public WindowEdit(int id, string tipo, decimal valor, string descricao)
        {
            InitializeComponent();
            RegistroId = id;
            TipoAnterior = tipo;
            ValorAnterior = valor;
            DescricaoAnterior = descricao;

            cbTipo.SelectedItem = cbTipo.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == tipo);
            txtValor.Text = valor.ToString();
            txtDescricao.Text = descricao;
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
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

            
            string descricao = string.IsNullOrWhiteSpace(txtDescricao.Text) ? null : txtDescricao.Text;

            DateTime? dataAlteracao = null;

            
            if (tipo != TipoAnterior || valor != ValorAnterior || descricao != DescricaoAnterior)
            {
                dataAlteracao = DateTime.Now;
            }

            try
            {
                using (var connection = new Database().GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE Registros SET Tipo = @Tipo, Valor = @Valor, Descricao = @Descricao, DataAlteracao = @DataAlteracao WHERE Id = @Id";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", RegistroId);
                        command.Parameters.AddWithValue("@Tipo", tipo);
                        command.Parameters.AddWithValue("@Valor", valor);
                        command.Parameters.AddWithValue("@Descricao", (object)descricao ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DataAlteracao", (object)dataAlteracao ?? DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Registro alterado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao salvar as alterações: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Cancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
