using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Projeto_TS
{
    public partial class FormChat : Form
    {
        // Porta utilizada para a conexão TCP
        private const int PORT = 10000;
        // Stream de rede para comunicação
        NetworkStream networkStream;
        // Instância do protocolo utilizado para empacotar/desempacotar mensagens
        ProtocolSI protocolSI;
        // Cliente TCP
        TcpClient client;

        // Construtor do formulário
        // Porta utilizada para conexão com o servidor
        private const int PORT = 10000;
        // Cliente TCP para comunicação com o servidor
        private TcpClient client;
        // Stream de rede para envio/recebimento de dados
        private NetworkStream networkStream;
        // Protocolo utilizado para empacotar/desempacotar mensagens
        private ProtocolSI protocolSI;
        // Thread responsável por receber mensagens do servidor
        private Thread receiveThread;

        public FormChat()
        {
            InitializeComponent();

            // Define o endpoint para localhost na porta especificada
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            client = new TcpClient();
            // Conecta ao servidor
            client.Connect(endPoint);
            // Obtém o stream de rede para comunicação
            networkStream = client.GetStream();
            // Inicializa o protocolo
            protocolSI = new ProtocolSI();

            // Inicia a escuta de mensagens do servidor em background
            Task.Run(() => ListenServer());
            ConnectToServer(); // Conecta ao servidor ao iniciar o formulário
        }

        // Método para conectar ao servidor
        private void ConnectToServer()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
                client = new TcpClient();
                client.Connect(endPoint);
                networkStream = client.GetStream();
                protocolSI = new ProtocolSI();

                // Inicia a thread para receber mensagens do servidor
                receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar ao servidor: " + ex.Message);
            }
        }

        // Método executado em thread separada para receber mensagens do servidor
        private void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    // Lê dados do servidor
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if (protocolSI.GetCmdType() == ProtocolSICmdType.DATA)
                    {
                        // Se for uma mensagem de dados, exibe na ListBox
                        string mensagem = protocolSI.GetStringFromData();
                        AddMessageToListBox("Servidor: " + mensagem);

                        // Envia ACK de confirmação ao servidor
                        byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
                        networkStream.Write(ack, 0, ack.Length);
                    }
                    else if (protocolSI.GetCmdType() == ProtocolSICmdType.EOT)
                    {
                        // Se for fim de transmissão, exibe mensagem e encerra loop
                        AddMessageToListBox("Conexão encerrada pelo servidor.");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                AddMessageToListBox("Erro na recepção: " + ex.Message);
            }
        }

        // Adiciona mensagem à ListBox de forma thread-safe
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

        // Envia mensagem para o servidor e exibe na ListBox
        private void EnviarMensagem(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return;

            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, texto);
            networkStream.Write(packet, 0, packet.Length);

            // Aguarda ACK do servidor
            while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            }

            // Exibe a mensagem enviada na ListBox
            AddMessageToListBox("Você: " + texto);
        }

        // Evento do botão "Sair" - Envia EOT e fecha a conexão
        private void buttonSair_Click(object sender, EventArgs e)
        {
            // Fecha a conexão e encerra o formulário
            CloseClient();
            this.Close();
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            // Obtém a mensagem da caixa de texto
            string mesg = textBoxMensagem.Text;
            // Limpa a caixa de texto após obter a mensagem
            textBoxMensagem.Clear();
            // Cria o pacote de dados usando o protocolo
            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, mesg);
            // Envia o pacote pelo stream de rede
            networkStream.Write(packet, 0, packet.Length);

            // Aguarda confirmação (ACK) do servidor
            while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            }
        }

        // Método para fechar a conexão com o servidor
        private void CloseClient()
        {
            // Cria e envia o pacote de fim de transmissão (EOT)
            byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
            networkStream.Write(eot, 0, eot.Length);

            // Aguarda resposta do servidor antes de fechar
            networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            // Fecha o stream de rede
            networkStream.Close();
            // Fecha o cliente TCP
            client.Close();
        }

        // Thread para escutar mensagens do servidor e atualizar a listBox1
        private void ListenServer()
        {
            try
            {
                while (client.Connected)
                {
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if (bytesRead > 0)
                    {
                        var cmd = protocolSI.GetCmdType();
                        if (cmd == ProtocolSICmdType.DATA)
                        {
                            string msg = protocolSI.GetStringFromData();
                            // Atualiza a listBox1 na thread da UI
                            listBoxMensagens.Invoke((MethodInvoker)delegate {
                                listBoxMensagens.Items.Add(msg);
                            });
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Ignora exceção ao fechar o formulário/conexão
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro na recepção de mensagens: " + ex.Message);
            }
            try
            {
                // Envia comando de fim de transmissão ao servidor
                byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
                networkStream.Write(eot, 0, eot.Length);
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                networkStream.Close();
                client.Close();
            }
            catch { }
            Application.Exit();
        }

        // Evento do botão "Enviar" - Envia a mensagem digitada
        private void buttonEnviar_Click(object sender, EventArgs e)
        {
            EnviarMensagem(textBoxMensagem.Text);
            textBoxMensagem.Clear();
        }

        // Evento de seleção da ListBox (não utilizado)
        private void listBoxMensagens_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}