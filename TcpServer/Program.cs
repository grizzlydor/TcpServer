using BookLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace TcpServer
{
     class Program
    {

        #region ListOfBooks

        public static List<Book> Books = new List<Book>()
        {
            new Book() {Title = "JavaScript and JQuery: Interactive Front-End Web Development",
                Author = "Jon Duckett", PageNumber = 645, ISBN13 = "978-1118531648"},
            new Book() {Title = "The Agile Samurai: How Agile Masters Deliver Great Software (Pragmatic Programmers)",
                Author = "Jonathan Rasmusson", PageNumber = 267 , ISBN13 = "978-1934356586"},
            new Book() {Title = "Extreme Programming Explained",
                    Author = "Kent Beck, Cynthia Andres", PageNumber = 224, ISBN13 = "978-0321278654"}
        };
        #endregion
        #region Main
        static void Main(string[] args)
        {
            Console.WriteLine("This is the server");

            TcpListener listener = new TcpListener(IPAddress.Loopback, 4646);
            listener.Start();
            Console.WriteLine("The server is ready.\n" +
                $"Local address: {IPAddress.Loopback}:4646" +
                              $"\nWaiting for incoming connections...");
            
            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                Task.Run(() => { HandleClient(socket); });
            }

        }
        #endregion
        #region HandleClient
        public static void HandleClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            string userInput = "";
            string item = "";
            //console app is working until the user writes "close"
            while (userInput != "close")
            {
                userInput = reader.ReadLine();
                item = reader.ReadLine();
                Console.WriteLine("Client: " + userInput + " " + item);
                writer.WriteLine(HandleRequest(userInput, item));
                writer.Flush();
            }
            socket.Close();
        }
        #endregion
        #region HandleRequest
        public static string HandleRequest(string command, string item)
        {
            if (command == "getall")
            {
                return GetAll();
            }
            else if (command == "get")
            {
                return Get(item);
            }
            else if (command == "save")
            {
                return Save(item);
            }
            else return "Invalid command. Use getall/get/save with lowercase letters";
        }
        #endregion

        #region GetAll
        private static string GetAll()
        {
            string jsonString = "";
            jsonString = JsonSerializer.Serialize(Books);
            return jsonString;
        }
        #endregion
        #region Get
        public static string Get(string item)
        {
            Book book1 = Books.Find(book => book.ISBN13 == item);
            string jsonString = JsonSerializer.Serialize(Books);
            return jsonString;
        }
        #endregion
        #region Save
        private static string Save(string jsonBook)
        {
            try
            {
                Book book = JsonSerializer.Deserialize<Book>(jsonBook);
                Books.Add(book);
                return "Book was successfully added to the list.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "A problem has occurred while saving. " + e.Message;
            }
        }
        #endregion
    }
}
