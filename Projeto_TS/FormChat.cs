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
        }

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
        }
    }
}
