﻿using Client.Utilities;
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
using System.Windows.Forms;

namespace Client
{
    class ClientService : IClientService
    {
        private const int BUFF_SIZE = 4096;
        private const int MAX_CONNECTIONS = 1;

        private static ClientService instance;

        private event Action<Image> UpdateDisplay;
        private event Action<string> HandleError;
        private event Func<bool> HandleRequest;

        public int Port { get; set; }
        public IPAddress IP { get; private set; }
        public int FPS { get; set; }
        public MouseOperationArgs MouseParameters { get; set; }

        private ISerializer serializer;
        private Socket clientSocket;
        private Socket listenSocket;
        private Thread receiveThread;
        private Thread broadcastThread;
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
            try
            {
                clientSocket?.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }
            clientSocket?.Close();

            broadcastThread?.Join(1);
            receiveThread?.Join(1);
        }

        public void Connect()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var remotePoint = new IPEndPoint(remoteAddress, remotePort);
            clientSocket.Connect(remotePoint);

            receiveThread = new Thread(ReceivePackages);
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

        public void SetRequestHandler(Func<bool> handler)
        {
            HandleRequest += handler;
        }

        public void SetRemoteParams(string ip, int port)
        {
            remoteAddress = IPAddress.Parse(ip);
            remotePort = port;
        }

        public void SendConnectionRequest()
        {
            Connect();

            var package = new ConnectionRequestPackage(IP, Port, PackageType.ConnectionRequest);

            if (clientSocket.Connected)
            {
                clientSocket.Send(serializer.Serialize(package));
            }
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
            MouseParameters = new MouseOperationArgs();
        }

        private void Listen()
        {
            listenSocket.Listen(MAX_CONNECTIONS);

            try
            {
                while (true)
                {
                    if (clientSocket != null)
                    {
                        if (!clientSocket.Connected)
                        {
                            clientSocket = listenSocket.Accept();

                            receiveThread = new Thread(ReceivePackages);
                            receiveThread.Start();
                        }
                    }
                    else
                    {
                        clientSocket = listenSocket.Accept();

                        receiveThread = new Thread(ReceivePackages);
                        receiveThread.Start();
                    }
                }
            }
            catch
            {
            }
        }

        private void SendConnectionResponse(bool response)
        {
            var package = new ConnectionResponsePackage(response, IP, Port, PackageType.ConnectionResponse);

            if (clientSocket.Connected)
            {
                clientSocket.Send(serializer.Serialize(package));
            }
        }

        private void BroadcastScreen()
        {
            while (clientSocket.Connected)
            {
                try
                {
                    var screenshot = ScreenCaptureUtility.CaptureDesktop();
                    var package = new ImagePackage(screenshot, PackageType.ImagePackage);

                    clientSocket.Send(serializer.Serialize(package));

                    Thread.Sleep(ScreenCaptureUtility.GetDelay(FPS));
                }
                catch
                {
                    Disconnect();
                }
            }
        }

        public void SendMouseCoordinates()
        {
            if (clientSocket.Connected)
            {
                try
                {
                    var package = new MouseInfoPackage(MouseParameters, PackageType.MouseInfoPackage);

                    clientSocket.Send(serializer.Serialize(package));
                }
                catch
                {
                }
            }
        }

        private void ReceivePackages()
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

                        Package package = serializer.Deserialize(stream.ToArray()) as Package;
                        HandlePackage(package);
                    }

                    if (waitingTime >= MAX_WAITING_TIME)
                    {
                        throw new SocketException();
                    }
                }
                catch
                {
                    HandleError?.Invoke("Соединение было разорвано. " + Port.ToString());
                    Disconnect();
                }
            }
        }

        private void HandlePackage(Package package)
        {
            switch (package.PackageType)
            {
                case PackageType.ConnectionRequest:

                    var response = (bool)HandleRequest?.Invoke();
                    SendConnectionResponse(response);

                    if (response)
                    {
                        broadcastThread = new Thread(BroadcastScreen);
                        broadcastThread.Start();
                    }

                    break;

                case PackageType.ConnectionResponse:

                    var isAllowed = (package as ConnectionResponsePackage).IsAllowed;

                    if (!isAllowed)
                    {
                        HandleError?.Invoke("Пользователь отказал вам в соединении.");
                        Disconnect();
                    }

                    break;

                case PackageType.ImagePackage:

                    var image = (package as ImagePackage).Screenshot;

                    UpdateDisplay?.Invoke(image);

                    break;

                case PackageType.MouseInfoPackage:

                    var parameters = (package as MouseInfoPackage).MouseParameters;

                    MouseOperationsUtility.DoMouseEvent(parameters);

                    break;
            }
        }
    }
}
