using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Dlp.Buy4.Framework.Portable.Serialization.Extension;

namespace Server_Socket
{
    public class SynchronousSocketListener
    {
        /// <summary>
        /// ACK byte indicates positive acknowledge.
        /// </summary>
        public const byte ACK_BYTE = 0x06;
        /// <summary>
        /// NAK byte indicates negative acknowledge.
        /// </summary>
        public const byte NAK_BYTE = 0x15;


        public static void StartListening()
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8483);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Socket handler = null;

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine("Waiting for a connection...");

                handler = listener.Accept();

                Console.WriteLine("Connection Accepted.");

                AbecsCommand command = new AbecsCommand();

                int nakCounter = 0;
                byte[] msg = null;

                while (true && nakCounter <= 3)
                {
                    if (nakCounter == 0)
                    {
                        Console.WriteLine("Digite o comando:");

                        msg = command.GetRequestBody(Console.ReadLine());
                    }

                    int ret = handler.Send(msg);
                    Console.WriteLine("{0} - Message Sent.", ret);

                    byte[] buffer = new byte[1];

                    handler.ReceiveTimeout = 2000;
                    ret = handler.Receive(buffer);
                    Console.WriteLine("{0} - Receive status.", ret);

                    // ACK
                    if (buffer[0] == ACK_BYTE)
                    {
                        nakCounter = 0;
                        continue;
                    }
                    // NAK
                    else if (buffer[0] == NAK_BYTE)
                    {
                        nakCounter++;
                        break;
                    }
                }

                handler.Shutdown(SocketShutdown.Both);
                Console.WriteLine("Connection Closed after 3 NAK's received.");

            }
            catch (SocketException e)
            {
                // Closes the connection if doesn't get a response
                if (e.SocketErrorCode == SocketError.TimedOut) {
                    handler.Shutdown(SocketShutdown.Both);
                    Console.WriteLine("Connection Closed.");
                }
              
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static int Main(String[] args)
        {
            StartListening();
            return 0;
        }
    }

}

