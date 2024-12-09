using System;
using System.Data.SqlClient;

namespace ControleDeCaixa
{
    public class Database
    {
        private ConfiguracaoModel configuracao;

        public Database()
        {
            configuracao = ConfiguracaoBanco.Carregar();
            if (configuracao == null)
            {
                throw new Exception("As configura��es do banco de dados n�o foram definidas. Configure clicando na engrenagem ao lado.");
            }
        }

        public SqlConnection GetConnection()
        {
            string connectionString = $"Server={configuracao.Servidor};Database={configuracao.NomeBanco};User Id={configuracao.Usuario};Password={configuracao.Senha};";
            return new SqlConnection(connectionString);
        }
    }
}
