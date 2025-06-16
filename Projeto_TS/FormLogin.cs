using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Projeto_TS
{
    public partial class FormLogin : Form
    {

        private const int SALTSIZE = 8;
        private const int NUMBER_OF_ITERATIONS = 1000;
       // private RSACryptoServiceProvider rsa;

        public FormLogin()
        {
            InitializeComponent();
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private bool VerifyLogin(string username, string password)
        {
            SqlConnection conn = null;
            try
            {
                // Configurar ligação à Base de Dados
                conn = new SqlConnection();
                conn.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\beatr\Documents\1_ano\TS\Projeto_TS\Projeto_TS\BDProjetoTS.mdf';Integrated Security=True");

                // Abrir ligação à Base de Dados
                conn.Open();

                // Declaração do comando SQL
                String sql = "SELECT * FROM Users WHERE Username = @username";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;

                // Declaração dos parâmetros do comando SQL
                SqlParameter param = new SqlParameter("@username", username);

                // Introduzir valor ao parâmentro registado no comando SQL
                cmd.Parameters.Add(param);

                // Associar ligação à Base de Dados ao comando a ser executado
                cmd.Connection = conn;

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

                conn.Close();

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


        private void buttonRegistar_Click(object sender, EventArgs e)
        {
            FormRegistar formRegistrar = new FormRegistar();
            formRegistrar.Show();
            this.Hide(); // Oculta o form atual depois de abrir o outro
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string username = textBoxUser.Text;
            if (username == "")
            {
                MessageBox.Show("Por favor, introduza um nome de utilizador.");
                return;
            }

            string pass = textBoxPass.Text;
            if (pass == "")
            {
                MessageBox.Show("Por favor, introduza uma password.");
                return;
            }

            bool success = VerifyLogin(username, pass);
            if (!success)
            {
                MessageBox.Show("Credenciais inválidas.");
                return;
            }
            else
            {
                string nomeUsuario = textBoxUser.Text;
                FormChat formChat = new FormChat(nomeUsuario);
                formChat.Show();
                this.Hide();
            }



                /*
                try
                {
                    // Conectar ao servidor
                    TcpClient client = new TcpClient("127.0.0.1", 10000); // ajusta se necessário
                    NetworkStream networkStream = client.GetStream();
                    ProtocolSI protocolSI = new ProtocolSI();

                    // Ler a chave pública do ficheiro
                    string publicKey = File.ReadAllText("publicKey.txt");
                    MessageBox.Show(publicKey, "Chave Pública (cliente)");

                    byte[] publicKeyMsg = protocolSI.Make(ProtocolSICmdType.USER_OPTION_1, publicKey);
                    networkStream.Write(publicKeyMsg, 0, publicKeyMsg.Length);

                    // Esperar pela resposta do servidor (chave AES cifrada)
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                    if (protocolSI.GetCmdType() == ProtocolSICmdType.USER_OPTION_2)
                    {
                        byte[] encryptedAesKey = protocolSI.GetData();

                        // Ler chave privada
                        string privateKeyXml = File.ReadAllText("privateKey.txt");
                        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                        rsa.FromXmlString(privateKeyXml);

                        // Descifrar a chave AES
                        byte[] aesKeyAndIV = rsa.Decrypt(encryptedAesKey, false);
                        byte[] aesKey = aesKeyAndIV.Take(32).ToArray();  // AES-256 key
                        byte[] aesIV = aesKeyAndIV.Skip(32).ToArray();   // IV

                        MessageBox.Show("Chave simétrica recebida e descifrada com sucesso!");

                        // Aqui podes guardar aesKey e aesIV para uso futuro
                    }
                    else
                    {
                        MessageBox.Show("Erro: tipo de mensagem inesperado do servidor.");
                    }

                    // Se tudo correr bem, abrir o FormChat
                    FormChat formChat = new FormChat();
                    formChat.Show();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao comunicar com o servidor: " + ex.Message);
                }*/
            }

      

     
       
    }
}
