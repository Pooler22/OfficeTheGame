using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{

	public class Server
	{
		int size;
		int port;
		int numberOfThread;
		int limitThread;
		TcpListener tcpListener;
		Thread listenThread;

		byte[] messageLimmit = System.Text.Encoding.ASCII.GetBytes("Limit! Disconnected Client.");

		/*
		 * Konstruktor inicjalizacja danych, przyjmuje port i liczbę limitu wątków jako argumenty
		 */
		public Server(int port, int limitThread)
		{
			this.numberOfThread = 0;
			this.size = 1024;
			this.port = port;
			this.limitThread = limitThread;
			this.tcpListener = new TcpListener(IPAddress.Any, port);
		}

		/*
		 * Funkcja ropoczynająca pracę serwera i tworząca wątki
		 */
		public void Start()
		{
			this.listenThread = new Thread(new ThreadStart(ListenForClients));
			this.listenThread.Start();
			this.listenThread.Join();
		}

		/*
		 * Funkcja zarządzająca wątkami
		 */
		private void ListenForClients()
		{
			try
			{
				this.tcpListener.Start();
			}
			catch (Exception e)
			{
				this.SetTextOnListBox("Error: " + e.GetType().Name + "/n");
				return;
			}

			this.SetTextOnListBox("Server run on port " + port + "\n");

			while (true)
			{
				try
				{
					TcpClient client = this.tcpListener.AcceptTcpClient();
					this.SetTextOnListBox("New client: " + client.Client.RemoteEndPoint + "\n");

					Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
					clientThread.Start(client);
				}
				catch (Exception e)
				{
					this.SetTextOnListBox("Error: " + e.GetType().Name + "/n");
				}

			}
		}

		/*
		 * Funkcja zarząda wątkiem, przyjmuje obiekt TcpClient klienta
		 */
		private void HandleClient(object client)
		{
			bool running = true;
			int numberOfThread = ++this.numberOfThread;
			byte[] message = new byte[size];
			int bytesRead;

			TcpClient tcpClient = (TcpClient)client;
			NetworkStream clientStream = tcpClient.GetStream();
			ASCIIEncoding encoder = new ASCIIEncoding();

			this.SetTextOnListBox("Number of clients: " + numberOfThread.ToString() + "\n");


			while (running)
			{
				bytesRead = 0;

				try
				{
					bytesRead = clientStream.Read(message, 0, size);

					if (numberOfThread > limitThread)
					{
						message = messageLimmit;
						bytesRead = messageLimmit.Length;
						running = false;
					}

					clientStream.Write(message, 0, bytesRead);

					if ((bytesRead == 0) || (encoder.GetString(message, 0, bytesRead).Equals("exit")))
					{
						throw new IOException();
					}

				}
				catch (IOException e)
				{
					this.SetTextOnListBox("Error: " + e.GetType().Name + "\n");
					this.SetTextOnListBox("Dissconnect: " + tcpClient.Client.RemoteEndPoint + " \n");
					running = false;
					tcpClient.Close();
					this.numberOfThread--;
					return;
				}

				this.SetTextOnListBox("Message from: " + tcpClient.Client.RemoteEndPoint + " : " + encoder.GetString(message, 0, bytesRead) + "\n");
			}
			this.numberOfThread--;
			tcpClient.Close();
		}

		/*
		 * Funkcja przyjmuje string i wyświetla go w konsoli
		 */
		private void SetTextOnListBox(string text)
		{
			Console.Write(text);
		}

		static void Main()
		{
			int port = 7;
			int limitThread = 3;
			Server serwer = new Server(port, limitThread);
			serwer.Start();
		}
	}
}