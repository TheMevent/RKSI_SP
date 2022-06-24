using System;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
	public class ClientObject
	{
		private readonly TcpClient client;
		private readonly ServerObject server;
		private string userName;

		public ClientObject(TcpClient tcpClient, ServerObject serverObject)
		{
			Id = Guid.NewGuid().ToString();
			client = tcpClient;
			server = serverObject;
			serverObject.AddConnection(this);
		}

		protected internal string Id { get; }
		protected internal NetworkStream Stream { get; private set; }

		public void Process()
		{
			try
			{
				Stream = client.GetStream();
				
				var message = GetMessage();
				userName = message;

				message = userName + " entered the chat room";
				
				server.BroadcastMessage(message, Id);
				Console.WriteLine(message);
				
				while (true)
					try
					{
						message = GetMessage();
						message = $"{userName}: {message}";
						Console.WriteLine(message);
						server.BroadcastMessage(message, Id);
					}
					catch
					{
						message = $"{userName}: left the chat room";
						Console.WriteLine(message);
						server.BroadcastMessage(message, Id);
						break;
					}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				server.RemoveConnection(Id);
				Close();
			}
		}

		private string GetMessage()
		{
			var data = new byte[64];
			var builder = new StringBuilder();
			do
			{
				var bytes = Stream.Read(data, 0, data.Length);
				builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
			} while (Stream.DataAvailable);

			return builder.ToString();
		}

		protected internal void Close()
		{
			Stream?.Close();
			client?.Close();
		}
	}
}