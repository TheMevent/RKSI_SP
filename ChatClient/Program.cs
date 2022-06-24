using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
	internal class Program
	{
		private const string host = "127.0.0.1";
		private const int port = 8888;
		private static string userName;
		private static TcpClient client;
		private static NetworkStream stream;

		private static void Main(string[] args)
		{
			Console.Write("Enter your name: ");
			userName = Console.ReadLine();
			client = new TcpClient();
			try
			{
				client.Connect(host, port);
				stream = client.GetStream();

				var message = userName;
				var data = Encoding.Unicode.GetBytes(message);
				stream.Write(data, 0, data.Length);

				var receiveThread = new Thread(ReceiveMessage);
				receiveThread.Start();
				Console.WriteLine($"Welcome, {userName}!");
				SendMessage();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				Disconnect();
			}
		}

		private static void SendMessage()
		{
			Console.WriteLine("Enter message: ");

			while (true)
			{
				var message = Console.ReadLine();
				var data = Encoding.Unicode.GetBytes(message);
				stream.Write(data, 0, data.Length);
			}
		}

		private static void ReceiveMessage()
		{
			while (true)
				try
				{
					var data = new byte[64];
					var builder = new StringBuilder();
					do
					{
						var bytes = stream.Read(data, 0, data.Length);
						builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
					} while (stream.DataAvailable);

					var message = builder.ToString();
					Console.WriteLine(message);
				}
				catch
				{
					Console.WriteLine("Connection closed!");
					Console.ReadLine();
					Disconnect();
				}
		}

		private static void Disconnect()
		{
			stream?.Close();
			client?.Close();
			Environment.Exit(0);
		}
	}
}