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
		IPEndPoint mcEndPoint = null;
		private bool arretServeur = false;
		public bool ArretServeur {
			set {
				arretServeur = value;
			}
		}

		public ServeurMcEchoUdp (IPEndPoint mcEndPoint,int servPort, FileInfo fichierLog)
		{
			byte prefix = mcEndPoint.Address.GetAddressBytes ()[0];
			prefix &= 240;
			if (prefix != 224)//if (addressBytes [0] < 224 | addressBytes [0] > 239)
				throw new Exception ("Adresse multicast doit avoir le préfixe 224.0.0.0/4 !");
			serveurUdp = new UdpClient(servPort);
			serveurUdp.Client.SetSocketOption (SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);

			this.mcEndPoint = mcEndPoint;

			this.fichierLog = fichierLog;
			StreamWriter streamLog = fichierLog.AppendText();
			streamLog.WriteLine (DateTime.Now.ToString ("dd/MM/yy HH:mm:ss") + " - Création du serveur");
			streamLog.Close ();
		}

		public void RenvoyerUnEcho()
		{
			StreamWriter streamLog = null;
			try{
				streamLog = fichierLog.AppendText();
				streamLog.WriteLine(DateTime.Now.ToString()+" - En attente de datagrammes");
				streamLog.Flush();

				IPEndPoint epDistant = new IPEndPoint(IPAddress.Any,0);
				byte[] byteBuffer = serveurUdp.Receive(ref epDistant);
				streamLog.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm:ss")+" - Traitement du client " + epDistant);
				streamLog.WriteLine("\t\"" + System.Text.UTF8Encoding.UTF8.GetString(byteBuffer) + "\"");

				byteBuffer = System.Text.UTF8Encoding.UTF8.GetBytes("Ceci n'est pas un echo !");
				serveurUdp.Send(byteBuffer, byteBuffer.Length, mcEndPoint);
				streamLog.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm:ss")+" - {0} octets renvoyés vers {1}", byteBuffer.Length,mcEndPoint);
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
			streamLog.WriteLine (DateTime.Now.ToString ("dd/MM/yy HH:mm:ss") + " - Démarrage du serveur "+serveurUdp.Client.LocalEndPoint.ToString());
			streamLog.Close ();
			while (!arretServeur) {

				RenvoyerUnEcho ();
			}
		}
	}
}

