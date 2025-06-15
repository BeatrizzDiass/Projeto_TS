using EI.SI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;



namespace Servidor
{
    class Program
    {
        // Porta utilizada pelo servidor para escutar conexões
        private const int PORT = 10000;

        // Lista thread-safe de todos os clientes conectados
        private static List<ClientHandler> clientes = new List<ClientHandler>();
        private static object clientesLock = new object();

        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);
            TcpListener listener = new TcpListener(endPoint);

            listener.Start();
            Console.WriteLine("SERVIDOR PRONTO");

            int clientCounter = 0;

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clientCounter++;
                Console.WriteLine($"Cliente {clientCounter} conectado.");

                ClientHandler handler = new ClientHandler(client, clientCounter);

                lock (clientesLock)
                {
                    clientes.Add(handler);
                }
                handler.Start();

            }
        }

        // Envia mensagem para todos os clientes conectados
        public static void Broadcast(string mensagem, int remetenteId)
        {
            lock (clientesLock)
            {
                foreach (var cliente in clientes.ToArray())
                {
                    try
                    {
                        cliente.SendMessage(mensagem);
                    }
                    catch
                    {
                        // Ignora falhas de envio (cliente pode ter desconectado)
                    }
                }
            }
        }

        // Remove cliente da lista
        public static void RemoveClient(ClientHandler handler)
        {
            lock (clientesLock)
            {
                clientes.Remove(handler);
            }
        }
    }

    class ClientHandler
    {
        private TcpClient client;
        private int clientId;
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;
        private Thread thread;

        public ClientHandler(TcpClient client, int clientId)
        {
            this.client = client;
            this.clientId = clientId;
            this.networkStream = client.GetStream();
            this.protocolSI = new ProtocolSI();
        }

        public void Start()
        {
            thread = new Thread(HandleClient);
            thread.IsBackground = true;
            thread.Start();
        }

        private void HandleClient()
        {
            try
            {
                while (true)
                {
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if (bytesRead == 0) break;

                    var cmd = protocolSI.GetCmdType();
                    if (cmd == ProtocolSICmdType.DATA)
                    {
                        string mensagem = protocolSI.GetStringFromData();
                        string mensagemFormatada = $"Cliente {clientId}: {mensagem}";
                        Console.WriteLine(mensagemFormatada);

                        // Broadcast para todos os clientes
                        Program.Broadcast(mensagemFormatada, clientId);

                        // Envia ACK para o remetente
                        byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
                        networkStream.Write(ack, 0, ack.Length);
                    }
                    else if (cmd == ProtocolSICmdType.EOT)
                    {
                        string saida = $"Cliente {clientId} saiu do chat.";
                        Console.WriteLine(saida);
                        Program.Broadcast(saida, clientId);

                        // Envia ACK para o remetente
                        byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
                        networkStream.Write(ack, 0, ack.Length);
                        break;
                    }
                }
            }
            catch
            {
                // Cliente desconectou abruptamente
            }
            finally
            {
                networkStream.Close();
                client.Close();
                Program.RemoveClient(this);
            }
        }

        // Envia mensagem para este cliente
        public void SendMessage(string mensagem)
        {
            byte[] data = protocolSI.Make(ProtocolSICmdType.DATA, mensagem);
            networkStream.Write(data, 0, data.Length);
        }
    }
}
