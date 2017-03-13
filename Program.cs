using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Dlp.Buy4.Framework.Portable.Serialization.Extension;

namespace Server_Socket
{
    public class SynchronousSocketListener
    {
        public static void StartListening()
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8583);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine("Waiting for a connection...");

                Socket handler = listener.Accept();

                Console.WriteLine("Connection Accepted.");

                //byte[] msg = Encoding.ASCII.GetBytes("<PinpadCommand xmlns =\"http://schemas.datacontract.org/2004/07/WcfServiceLibrary12\"><Command>GIN00200</Command><IsCustomized>false</IsCustomized></PinpadCommand>");
                AbecsCommand command = new AbecsCommand();
                PinpadCommand rawCommand = new PinpadCommand();

                while (true)
                {
                    Console.WriteLine("Digite o comando:");

                    rawCommand.Command = Console.ReadLine();
                    rawCommand.IsCustomized = false;

                    string serializedCommand = rawCommand.SerializeToXml();

                    int ret = handler.Send(command.GetRequestBody(serializedCommand));
                    Console.WriteLine("{0} - Message Sent.", ret);
                }
            
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
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

