using System;
using System.Net;	// For IPEndPoint
using System.Net.Sockets;	// For UdpClient, SocketException
using System.IO; // Pour les fichiers

namespace UdpEchoServer
{
	class UdPEchoServer
	{
		public static void Main (string[] args)
		{
			if (args.Length > 1) {// Test for correct # of args
				throw new ArgumentException ("Parameters: <Port>");
			}

			int servPort = (args.Length == 1) ? Int32.Parse (args [0]) : 7;

			FileInfo fichierLog = new FileInfo ("LogEchoUdp_" + DateTime.Now.Date.ToString ("dd-MM-yy") + ".txt");
			ServeurEchoUdp serveur = new ServeurEchoUdp (servPort, fichierLog);

			serveur.RenvoyerEchoEnBoucle ();



		}
	}
}
