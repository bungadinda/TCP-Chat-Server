using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        //read received data from server
        static void ReadData(object obj)
        {
            TcpClient tcpClient = (TcpClient)obj;
            //receive data from server network stream and encode it from ascii into a string
            StreamReader sReader = new StreamReader(tcpClient.GetStream());

            while (true)
            {
                try
                {
                    //read the data and print into Client console
                    string message = sReader.ReadLine();
                    Console.WriteLine(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }
        }
        //setting up TCPClient
        static TcpClient SettingUpClient()
        {
            TcpClient Client = new TcpClient("127.0.0.1", 1234);
            Console.WriteLine("Connected to server...");
            return Client;
        }
        static void Main()
        {
            try
            {
                //set Client
                TcpClient tcpClient = SettingUpClient();
                Console.WriteLine("Please enter your name>>>>");
                string name = Console.ReadLine();
                Console.WriteLine("Welcome " + name + "^_^");

                //start new thread for reading data
                Thread thread = new Thread(ReadData);
                thread.Start(tcpClient);

                //Write Char/string from Client, encode it into ASCII and send it to server network stream
                StreamWriter sWriter = new StreamWriter(tcpClient.GetStream());
                while (true)
                {
                    if (tcpClient.Connected)
                    {
                        //Client Input
                        string input = Console.ReadLine();
                        sWriter.WriteLine(name + ">>> " + input);
                        //empty the writer so the string not mixed with previous message
                        sWriter.Flush();
                    }
                }

            }
            //show error if client disconnected
            catch (Exception e)
            {

                Console.Write(e.Message);
                Environment.Exit(1);
            }

            Console.ReadKey();
        }
    }
}