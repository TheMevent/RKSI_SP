using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
	public class ServerObject
	{
		private static TcpListener tcpListener;
		private readonly List<ClientObject> clients = new();

		protected internal void AddConnection(ClientObject clientObject)
		{
			clients.Add(clientObject);
		}

		protected internal void RemoveConnection(string id)
		{
			var client = clients.FirstOrDefault(c => c.Id == id);
			if (client != null)
				clients.Remove(client);
		}

		protected internal void Listen()
		{
			try
			{
				tcpListener = new TcpListener(IPAddress.Any, 8888);
				tcpListener.Start();
				Console.WriteLine("The server is running. Waiting for connections...");

				while (true)
				{
					var tcpClient = tcpListener.AcceptTcpClient();

					var clientObject = new ClientObject(tcpClient, this);
					var clientThread = new Thread(clientObject.Process);
					clientThread.Start();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Disconnect();
			}
		}

		protected internal void BroadcastMessage(string message, string id)
		{
			var data = Encoding.Unicode.GetBytes(message);
			for (var i = 0; i < clients.Count; i++)
				if (clients[i].Id != id)
					clients[i].Stream.Write(data, 0, data.Length);
		}

		protected internal void Disconnect()
		{
			tcpListener.Stop();

			for (var i = 0; i < clients.Count; i++) clients[i].Close(); 
			
			Environment.Exit(0);
		}
	}
}