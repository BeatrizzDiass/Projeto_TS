using EI.SI;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Projeto_TS
{
    public partial class FormChat : Form
    {
        private const int PORT = 10000;
        private TcpClient client;
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;
        private Thread receiveThread;

        public FormChat()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
                client = new TcpClient();
                client.Connect(endPoint);
                networkStream = client.GetStream();
                protocolSI = new ProtocolSI();

                // Inicia a thread para ouvir mensagens
                receiveThread = new Thread(ListenServer);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar ao servidor: " + ex.Message);
            }
        }

        private void ListenServer()
        {
            try
            {
                while (client.Connected)
                {
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if (bytesRead > 0)
                    {
                        ProtocolSICmdType cmdType = protocolSI.GetCmdType();

                        if (cmdType == ProtocolSICmdType.DATA)
                        {
                            string msg = protocolSI.GetStringFromData();
                            AddMessageToListBox("Servidor: " + msg);
                        }
                        else if (cmdType == ProtocolSICmdType.EOT)
                        {
                            AddMessageToListBox("Conexão encerrada pelo servidor.");
                            break;
                        }
                        else if (cmdType == ProtocolSICmdType.ACK)
                        {
                            // Pode ser tratado se necessário
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Conexão fechada, ignorar
            }
            catch (Exception ex)
            {
                AddMessageToListBox("Erro na recepção: " + ex.Message);
            }
            finally
            {
                FecharConexao();
                Application.Exit();
            }
        }

        private void AddMessageToListBox(string mensagem)
        {
            if (listBoxMensagens.InvokeRequired)
            {
                listBoxMensagens.Invoke(new Action<string>(AddMessageToListBox), mensagem);
            }
            else
            {
                listBoxMensagens.Items.Add(mensagem);
            }
        }

        private void EnviarMensagem(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return;

            try
            {
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, texto);
                networkStream.Write(packet, 0, packet.Length);

                // Espera ACK
                WaitForAck();

                AddMessageToListBox("Você: " + texto);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao enviar mensagem: " + ex.Message);
            }
        }

        private void WaitForAck()
        {
            while (true)
            {
                int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                if (bytesRead > 0)
                {
                    if (protocolSI.GetCmdType() == ProtocolSICmdType.ACK)
                        break;
                }
            }
        }

        private void buttonEnviar_Click(object sender, EventArgs e)
        {
            EnviarMensagem(textBoxMensagem.Text);
            textBoxMensagem.Clear();
        }

        private void buttonSair_Click(object sender, EventArgs e)
        {
            FecharConexao();
            this.Close();
        }

        private void FecharConexao()
        {
            if (client != null && client.Connected)
            {
                try
                {
                    // Envia EOT
                    byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
                    networkStream.Write(eot, 0, eot.Length);

                    // Opcional: aguarda confirmação
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                }
                catch { }
                finally
                {
                    networkStream.Close();
                    client.Close();
                }
            }
        }
    }
}