﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimFTP;
using SimFTP.Config;

using SimFTP.Net;
using SimFTP.Net.DataPackets;
using SimFTP.Net.MetadataPackets;

using System.Net;
using System.Threading;
using System.Net.Sockets;


namespace SimFTP.Net.Server
{
	public delegate void ServerTransferCallback (string address, string statusMessage);

	public class ServerTransfer : IDisposable
	{
		public const int ServerPort = 44335;

		#region Private Const field
		private const int MaximunBacklog = 16;
		#endregion

		#region Private field
		private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private List<Thread> threadList = new List<Thread>();
		private Thread serverThread;

		private TransferConfig config = new TransferConfig();
		private AccountConfig accountConfig = new AccountConfig();

		#endregion

		#region Public Callback field
		public event ServerTransferCallback ServerTransferCallbackEvent = delegate { };
		#endregion

		public ServerTransfer ()
		{
			serverSocket.Bind(new IPEndPoint(IPAddress.Any, ServerPort));
			serverSocket.Listen(MaximunBacklog);
			serverSocket.NoDelay = false;

			serverThread = new Thread(new ThreadStart(ConnectionHandleRoutine));
		}

		public void Start ()
		{
			serverThread.Start();
		}

		public void Dispose ()
		{
			serverSocket.Dispose();
			serverThread.Interrupt();

			foreach(var thread in threadList)
			{
				// blocking?
				thread.Interrupt();
				threadList.Remove(thread);
			}

			return;
		}

		#region Server Thread Area
		private void ConnectionHandleRoutine ()
		{
			while(true)
			{
				Socket clientSocket = serverSocket.Accept();

				Thread connectHandleRoutine = new Thread(new ParameterizedThreadStart(ConnectHandleRoutine));
				connectHandleRoutine.Start(clientSocket);
			}
		}
		#endregion

		#region Handling Thread Area
		private void ConnectHandleRoutine (object threadArgs)
		{
			Socket clientSocket = (Socket)threadArgs;
			ClientConnectHandling(clientSocket);
		}

		private void ClientConnectHandling (Socket clientSocket)
		{
			clientSocket.ReceiveTimeout = 300;
			BasicMetadataPacket packetData = ReceiveBasicMetadataPacket(clientSocket);

			switch(packetData.PacketType)
			{
				case PacketType.BasicFrame:
					BasicMetatdataPacketHandling(clientSocket, packetData);
					break;
				case PacketType.BasicSecurity:
					BasicSecurityMetadataPacketHandling(clientSocket, packetData);
					break;
				case PacketType.ExpertSecurity:
					break;
				case PacketType.Error:
					break;
			};
		}
		#endregion
	}
}
