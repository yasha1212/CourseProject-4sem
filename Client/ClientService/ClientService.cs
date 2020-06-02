using Client.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class ClientService : IClientService
    {
        private const int BUFF_SIZE = 4096;
        private const int MAX_CONNECTIONS = 1;

        private static ClientService instance;

        private event Action<Image> UpdateDisplay;
        private event Action<string> HandleError;

        public int Port { get; set; }
        public IPAddress IP { get; private set; }
        public int FPS { get; set; }

        private ISerializer serializer;
        private Socket clientSocket;
        private Socket listenSocket;
        private Thread receiveThread;
        private Thread listenThread;
        private IPAddress remoteAddress;
        private int remotePort;

        public static ClientService GetInstance()
        {
            if (null == instance)
            {
                instance = new ClientService();
            }
            return instance;
        }

        public void Start()
        {
            IP = NodeInfoUtility.GetCurrentIP();

            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localPoint = new IPEndPoint(IP, Port);
            listenSocket.Bind(localPoint);

            listenThread = new Thread(Listen);
            listenThread.Start();
        }

        public void Disconnect()
        {
            receiveThread?.Join(1);
            clientSocket?.Close();
        }

        public void Connect()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var remotePoint = new IPEndPoint(remoteAddress, remotePort);
            clientSocket.Connect(remotePoint);

            receiveThread = new Thread(ReceiveImage);
            receiveThread.Start();
        }

        public void SetUpdateHandler(Action<Image> handler)
        {
            UpdateDisplay += handler;
        }

        public void SetErrorHandler(Action<string> handler)
        {
            HandleError += handler;
        }

        public void SetRemoteParams(string ip, int port)
        {
            remoteAddress = IPAddress.Parse(ip);
            remotePort = port;
        }

        public void Close()
        {
            Disconnect();

            listenSocket?.Close();
            listenThread?.Abort();
        }

        private ClientService()
        {
            serializer = new BinarySerializeService();
        }

        private void Listen()
        {
            listenSocket.Listen(MAX_CONNECTIONS);

            try
            {
                while (true) // сделать код данного цикла более элегантным
                {
                    if (clientSocket != null)
                    {
                        if (!clientSocket.Connected)
                        {
                            clientSocket = listenSocket.Accept();

                            receiveThread = new Thread(BroadcastScreen);
                            receiveThread.Start();
                        }
                    }
                    else
                    {
                        clientSocket = listenSocket.Accept();

                        receiveThread = new Thread(BroadcastScreen);
                        receiveThread.Start();
                    }
                }
            }
            catch
            {
            }
        }

        private void BroadcastScreen()
        {
            while (clientSocket.Connected)
            {
                try
                {
                    var screenshot = ScreenCaptureUtility.CaptureDesktop();

                    clientSocket.Send(serializer.Serialize(screenshot));

                    Thread.Sleep(ScreenCaptureUtility.GetDelay(FPS));
                }
                catch
                {
                    HandleError?.Invoke("Соединение было разорвано.");
                    Disconnect();
                }
            }
            HandleError?.Invoke("Трансляция экрана завершена.");
        }

        private void ReceiveImage()
        {
            const int MAX_WAITING_TIME = 100;
            int waitingTime = 0;

            while (clientSocket.Connected)
            {
                var stream = new MemoryStream();

                try
                {
                    waitingTime++;

                    do
                    {
                        var data = new byte[BUFF_SIZE];
                        int receivedBytes = clientSocket.Receive(data);
                        stream.Write(data, 0, receivedBytes);
                    }
                    while (clientSocket.Available > 0);

                    if (stream.Length > 0)
                    {
                        waitingTime = 0;

                        Image image = serializer.Deserialize(stream.ToArray()) as Image;
                        HandleImage(image);
                    }

                    if (waitingTime >= MAX_WAITING_TIME)
                    {
                        throw new SocketException();
                    }
                }
                catch
                {
                    HandleError?.Invoke("Соединение было разорвано.");
                    Thread.CurrentThread.Join(1);
                    Disconnect();
                }
            }
            HandleError?.Invoke("Источник больше не доступен.");
        }

        private void HandleImage(Image receivedImage)
        {
            UpdateDisplay?.Invoke(receivedImage);
        }
    }
}
