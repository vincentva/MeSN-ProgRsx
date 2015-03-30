using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace UdpEchoServer
{
	public class ServeurMcEchoUdp
	{
		private UdpClient serveurUdp = null;
		private FileInfo fichierLog = null;
		IPEndPoint mcIPEndPoint = null;
		private bool arretServeur = false;
		public bool ArretServeur {
			set {
				arretServeur = value;
			}
		}

		public ServeurMcEchoUdp (IPAddress mcIp, int mcPort,int servPort, FileInfo fichierLog)
		{
			byte prefix = mcIp.GetAddressBytes ()[0];
			prefix &= 240;
			if (prefix != 224)//if (addressBytes [0] < 224 | addressBytes [0] > 239)
				throw new Exception ("Adresse multicast doit avoir le préfixe 224.0.0.0/4 !");
			serveurUdp = new UdpClient(servPort);
			serveurUdp.Client.SetSocketOption (SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);

			mcIPEndPoint = new IPEndPoint (mcIp, mcPort);

			this.fichierLog = fichierLog;
			StreamWriter streamLog = fichierLog.AppendText();
			streamLog.WriteLine (DateTime.Now.ToString () + " - Création du serveur");
			streamLog.Close ();
		}

		public void RenvoyerUnEcho()
		{
			StreamWriter streamLog = null;
			try{
				streamLog = fichierLog.AppendText();
				streamLog.WriteLine(DateTime.Now.ToString()+" - En attente de datagrammes");
				IPEndPoint epDistant = new IPEndPoint(IPAddress.Any,0);
				byte[] byteBuffer = serveurUdp.Receive(ref epDistant);
				streamLog.WriteLine(DateTime.Now.ToString()+" - Traitement du client " + epDistant);
				streamLog.WriteLine("\t\"" + System.Text.UTF8Encoding.UTF8.GetString(byteBuffer) + "\"");

				serveurUdp.Send(byteBuffer, byteBuffer.Length, mcIPEndPoint);
				streamLog.WriteLine(DateTime.Now.ToString()+" - {0} octets renvoyés vers {1}", byteBuffer.Length,mcIPEndPoint);
			}
			catch (SocketException se) {
				streamLog.WriteLine (se.ErrorCode + ": " + se.Message);
			}
			finally {
				if (streamLog != null) {
					streamLog.Close ();
				}
			}
		}

		public void RenvoyerEchoEnBoucle()
		{
			StreamWriter streamLog = fichierLog.AppendText();
			streamLog.WriteLine (DateTime.Now.ToString () + " - Démarrage du serveur");
			streamLog.Close ();
			while (!arretServeur) {
				RenvoyerUnEcho ();
			}
		}
	}
}

