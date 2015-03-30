using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace UdpEchoServer
{
	public class ServeurEchoUdp
	{
		private UdpClient serveurUdp = null;
		private FileInfo fichierLog = null;
		IPEndPoint remoteIPEndPoint = new IPEndPoint (IPAddress.Any, 0);
		private bool arretServeur = false;
		public bool ArretServeur {
			set {
				arretServeur = value;
			}
		}

		public ServeurEchoUdp (int numPort,FileInfo fichierLog)
		{
			serveurUdp = new UdpClient(numPort);
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
				byte[] byteBuffer = serveurUdp.Receive(ref remoteIPEndPoint);
				streamLog.WriteLine(DateTime.Now.ToString()+" - Traitement du client " + remoteIPEndPoint);

				streamLog.Write("\"" + System.Text.UTF8Encoding.UTF8.GetString(byteBuffer) + "\"");
				serveurUdp.Send(byteBuffer, byteBuffer.Length, remoteIPEndPoint);
				streamLog.WriteLine(" - echoed {0} bytes.", byteBuffer.Length);
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

