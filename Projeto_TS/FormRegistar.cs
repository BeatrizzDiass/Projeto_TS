using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Projeto_TS
{
    public partial class FormRegistar : Form
    {

        private const int SALTSIZE = 8;
        private const int NUMBER_OF_ITERATIONS = 1000;

        //private RSACryptoServiceProvider rsa;

        public FormRegistar()
        {
            InitializeComponent();
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private bool VerifyLogin(string username, string password)
        {
            SqlConnection con = null;
            try
            {
                //configurar a ligacao à BD
                con = new SqlConnection();
                con.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\beatr\Documents\1_ano\TS\Projeto_TS\Projeto_TS\BDProjetoTS.mdf';Integrated Security=True");

                //abri ligação à BD
                con.Open();

                //declaracao do comando SQL
                // Declaração do comando SQL
                String sql = "SELECT * FROM Users WHERE Username = @username";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;

                //declaração dos parametros do comando sql
                SqlParameter param = new SqlParameter("@username", username);

                //introduzir valor ao parametro registado no comando sql
                cmd.Parameters.Add(param);

                //associar ligacao à BD ao comando a ser executado
                cmd.Connection = con;

                // Executar comando SQL
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    throw new Exception("Error while trying to access an user");
                }

                // Ler resultado da pesquisa
                reader.Read();

                // Obter Hash (password + salt)
                byte[] saltedPasswordHashStored = (byte[])reader["SaltedPasswordHash"];

                // Obter salt
                byte[] saltStored = (byte[])reader["Salt"];

                con.Close();

                //TODO: verificar se a password na base de dados 
                byte[] hash = GenerateSaltedHash(password, saltStored);

                return saltedPasswordHashStored.SequenceEqual(hash);

                throw new NotImplementedException();

            }
            catch (Exception e)
            {
                MessageBox.Show("Ocorreu um erro: " + e.Message);
                return false;
            }
        }

        private static byte[] GenerateSaltedHash(string plainText, byte[] salt)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(plainText, salt, NUMBER_OF_ITERATIONS);
            return rfc2898.GetBytes(32);
        }



        private bool Register(string username, byte[] saltedPasswordHash, byte[] salt)
        {
            SqlConnection conn = null;
            try
            {
                // Configurar ligação à Base de Dados
                conn = new SqlConnection();
                conn.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\beatr\Documents\1_ano\TS\Projeto_TS\Projeto_TS\BDProjetoTS.mdf';Integrated Security=True");

                // Abrir ligação à Base de Dados
                conn.Open();

                // Declaração dos parâmetros do comando SQL
                SqlParameter paramUsername = new SqlParameter("@username", username);
                SqlParameter paramPassHash = new SqlParameter("@saltedPasswordHash", saltedPasswordHash);
                SqlParameter paramSalt = new SqlParameter("@salt", salt);


                // Declaração do comando SQL
                String sql = "INSERT INTO Users (Username, SaltedPasswordHash, Salt) VALUES (@username,@saltedPasswordHash,@salt)";

                // Prepara comando SQL para ser executado na Base de Dados
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Introduzir valores aos parâmetros registados no comando SQL
                cmd.Parameters.Add(paramUsername);
                cmd.Parameters.Add(paramPassHash);
                cmd.Parameters.Add(paramSalt);

                // Executar comando SQL
                int lines = cmd.ExecuteNonQuery();

                // Fechar ligação
                conn.Close();
                if (lines == 0)
                {
                    // Se forem devolvidas 0 linhas alteradas então o não foi executado com sucesso
                    throw new Exception("Erro ao inserir um utilizador!");
                }
                MessageBox.Show("Utilizador registado com sucesso!");
                return true; // Adicionado retorno de sucesso
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao inserir um utilizador!" + e.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private static byte[] GenerateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return buff;
        }


        private void buttonRegistar_Click(object sender, EventArgs e)
        {

            string username = textBoxUser.Text;
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Por favor, introduza um nome de utilizador.");
                return;
            }

            string pass = textBoxPass.Text;
            if (string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Por favor, introduza uma password.");
                return;
            }

            byte[] salt = GenerateSalt(SALTSIZE);
            byte[] hash = GenerateSaltedHash(pass, salt);

            bool success = Register(username, hash, salt);

            if (success)
            {
                // Inicializar o RSA e gerar chaves
              /*  rsa = new RSACryptoServiceProvider(2048);

                string publicKey = rsa.ToXmlString(false); // chave pública
                string privateKey = rsa.ToXmlString(true); // chave privada

                string folderPath = AppDomain.CurrentDomain.BaseDirectory;

                // Guardar chaves no diretório da aplicação
                File.WriteAllText(Path.Combine(folderPath, "publicKey.txt"), publicKey);
                File.WriteAllText(Path.Combine(folderPath, "privateKey.txt"), privateKey);*/

                // Abrir formulário de login
                FormLogin formLogin = new FormLogin();
                formLogin.Show();
                this.Hide();
            }


        }

        private void FormRegistar_Load(object sender, EventArgs e)
        {

        }
    }
}
