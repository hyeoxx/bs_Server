using bs_Server.Shops;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace bs_Server
{
    class Program
    {
        static Socket sock;
        static List<Socket> Clients = new List<Socket>();
        static byte[] szData;

        static MySqlConnection con;

        public static List<Shop> shops = new List<Shop>();

        static void Main(string[] args)
        {
            /*
             * 로딩, 가게 파싱 시작
             */
            con = new MySqlConnection("Server=localhost;Port=3306;Database=beakchelin;Uid=root;Pwd=tkfkd784");
            if (ShopFactory.LoadShop(con))
            {
                Console.WriteLine("모든 로딩이 끝났습니다.");
            }

            /*
             * 서버 오픈
             */
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            sock.Bind(new IPEndPoint(IPAddress.Any, 10100));
            sock.Listen(100);
            Console.WriteLine("서버가 시작되었습니다.");


            var S_args = new SocketAsyncEventArgs();
            S_args.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
            sock.AcceptAsync(S_args);

            /*
             * 명령어
             */
            while (true)
            {
                var Command = Console.ReadLine();
                var CommandArgs = Command.Split(" ");
                var Op = CommandArgs[0];
                
                switch(Op)
                {
                    case "종료":
                        return;
                }
            }
        }

        static void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            var Client = e.AcceptSocket;
            if (Client != null)
            {
                Clients.Add(Client);
                var args = new SocketAsyncEventArgs();
                szData = new byte[1024];
                args.SetBuffer(szData, 0, 1024);
                args.UserToken = Client;
                args.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
                Client.ReceiveAsync(args);
            }

            e.AcceptSocket = null;
            sock.AcceptAsync(e);
        }
        static void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            var Client = (Socket)sender;
            if (Client.Connected && e.BytesTransferred > 0)
            {
                byte[] szData = e.Buffer;
                string sData = Encoding.Unicode.GetString(szData);

                string Temp = sData.Replace("\0", "").Trim();
                Console.WriteLine(Temp);
                
                for (var i = 0; i < szData.Length; i++ )
                {
                    szData[i] = 0;
                }

                e.SetBuffer(szData, 0, 1024);
                Client.ReceiveAsync(e);
            } else
            {
                Client.Disconnect(false);
                Clients.Remove(Client);
                Console.WriteLine(Client.RemoteEndPoint.ToString() + "의 연결이 끊어졌습니다.");
            }
        }
    }
}
