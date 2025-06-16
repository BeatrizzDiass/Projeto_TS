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
        // Porta utilizada para a conexão TCP
        private const int PORT = 10000;
        // Stream de rede para comunicação
        NetworkStream networkStream;
        // Instância do protocolo utilizado para empacotar/desempacotar mensagens
        ProtocolSI protocolSI;
        // Cliente TCP
        TcpClient client;

        private string nomeUsuario;

        public FormChat(string nomeUsuario)
        {
            InitializeComponent();
            this.nomeUsuario = nomeUsuario;

            // Define o endpoint para localhost na porta especificada
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 10000);
            client = new TcpClient();
            client.Connect(endPoint);
            networkStream = client.GetStream();
            protocolSI = new ProtocolSI();

            // 🔁 Envia o nome do usuário ao servidor após conectar
            byte[] nomePacket = protocolSI.Make(ProtocolSICmdType.USER_OPTION_1, nomeUsuario);
            networkStream.Write(nomePacket, 0, nomePacket.Length);

            // Inicia a escuta em background
            Task.Run(() => ListenServer());
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            // ✅ CORREÇÃO: usar textBoxMensagem em vez de listBoxMensagens
            string mesg = textBoxMensagem.Text;

            // Verificar se a mensagem não está vazia
            if (string.IsNullOrWhiteSpace(mesg))
            {
                return; // Não envia mensagens vazias
            }

            textBoxMensagem.Clear();
            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, mesg);
            networkStream.Write(packet, 0, packet.Length);

            // ⚠️ REMOVER O BLOCO ABAIXO:
            /*
            while (true)
            {
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                if (protocolSI.GetCmdType() == ProtocolSICmdType.ACK)
                    break;
            }
            */
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

        private void buttonSair_Click(object sender, EventArgs e)
        {
            // Fecha a conexão e encerra o formulário
            CloseClient();
            this.Close();
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

                            // Atualiza a interface de forma thread-safe
                            if (listBoxMensagens.InvokeRequired)
                            {
                                listBoxMensagens.Invoke((MethodInvoker)delegate {
                                    // Adiciona timestamp às mensagens
                                    string mensagemComHora = $"[{DateTime.Now:HH:mm:ss}] {msg}";
                                    listBoxMensagens.Items.Add(mensagemComHora);

                                    // Auto-scroll para a mensagem mais recente
                                    listBoxMensagens.TopIndex = listBoxMensagens.Items.Count - 1;
                                });
                            }
                            else
                            {
                                string mensagemComHora = $"[{DateTime.Now:HH:mm:ss}] {msg}";
                                listBoxMensagens.Items.Add(mensagemComHora);
                                listBoxMensagens.TopIndex = listBoxMensagens.Items.Count - 1;
                            }
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
                // Só mostra erro se ainda estiver conectado
                if (client.Connected)
                {
                    this.Invoke((MethodInvoker)delegate {
                        MessageBox.Show("Erro na recepção de mensagens: " + ex.Message, "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    });
                }
            }
        }
    }
}