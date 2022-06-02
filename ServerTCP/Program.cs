using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq;

namespace Server
{
    class Program
    {
        //prepare Server Socket Listener
        private static TcpListener Listener;
        //list of clients
        private static List<TcpClient> tcpClientsList = new List<TcpClient>();
        //list of messages
        private static List<string> MessageList = new List<string>();
        //client message save path
        private static string savePath = @"C:\Users\Bunga Adinda\OneDrive - OneDrive\Documents\bung\kulon\S4\ServerC\ChatData.txt";

        //setting up server
        public static void SetupServer()
        {
            //create new listener
            Listener = new TcpListener(IPAddress.Any, 1234);
            Listener.Start();
            Console.WriteLine("Server Listening>>>>");
        }

        //Listening for Client Connection
        public static void ClientListener(object obj)
        {
            TcpClient client = (TcpClient)obj;
            //receive data from Client stream and encode it from ascii into a string
            StreamReader reader = new StreamReader(client.GetStream());

            Console.WriteLine("Client connected");
            try
            {
                while (true)
                {
                    //read data / Client message from steamreader
                    string message = reader.ReadLine();
                    //save the message
                    SaveMessage(message);
                    //Broadcast the message to other connected client.
                    BroadcastMessage(message, client);
                    //show Client data in Server console
                    Console.WriteLine(message);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Client Disconnected...");
            }
            tcpClientsList.Remove(client);
        }

        //broadcast message to other clients except itself
        public static void BroadcastMessage(string message, TcpClient excludeClient)
        {
            foreach (TcpClient client in tcpClientsList)
            {
                //share client message to other client
                if (client != excludeClient)
                {
                    /*Write message from Client, encode it into ASCII and send it to server network stream
                     * and each connected Client will receive the data*/
                    StreamWriter Writer = new StreamWriter(client.GetStream());
                    Writer.WriteLine(message);
                    //empty the writer so the string not mixed with previous message
                    Writer.Flush();
                }
            }
        }
        //save connected Clients Message into a .txt file
        public static void SaveMessage(string newMessage)
        {
            try
            {
                //read message and add to list
                MessageList = File.ReadAllLines(savePath).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Save Error : " + e);
            }

            // Adding message to list
            MessageList.Add(newMessage);

            // Save message list to .txt on save location
            File.WriteAllLines(savePath, MessageList);
        }
        static void Main()
        {
            SetupServer();
            while (true)
            {
                //waiting and accept new Client
                Console.WriteLine("Waiting for client...");
                TcpClient Client = Listener.AcceptTcpClient();
                tcpClientsList.Add(Client);

                //create new thread after accepting Client
                Thread thread = new Thread(ClientListener);
                thread.Start(Client);

            }
        }
    }

}