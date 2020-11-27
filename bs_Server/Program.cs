using bs_Server.Shops;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace bs_Server
{
    class Program
    {
        static Socket sock;
        public static StringBuilder sb = new StringBuilder();

        static List<Socket> Clients = new List<Socket>();
        static byte[] szData;

        static MySqlConnection con;

        public static List<Shop> shops = new List<Shop>();
        public static bool isDebug = true;


        static void Main(string[] args)
        {
            /*
             * Database init
             * 가게 파싱 시작
             */
            con = new MySqlConnection("Server=localhost;Port=3306;Database=beakchelin;Uid=root;Pwd=tkfkd784;CharSet=utf8;");
            if (ShopFactory.LoadShop(con))
            {
                Console.WriteLine("모든 로딩이 끝났습니다.");
            }


            /*
             * 서버 오픈
             */
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
                    case "테스트":
                        Send(CommandArgs[2], Int32.Parse(CommandArgs[1]));
                        break;
                    case "디버그":
                        
                        isDebug = !(isDebug);
                        break;
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

                // 클라이언트 소켓에 번호부여
                Write packet = new Write(0);
                packet.writeLine(Clients.Count.ToString());
                Send("0<>"+packet.getPacket(true) + "<EOF>", (Clients.Count - 1));


                Console.WriteLine("{0}이 접속하였습니다.", Client.RemoteEndPoint.ToString());
            }

            e.AcceptSocket = null;
            sock.AcceptAsync(e);
        }

        static void Send(string msg, int idx)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(msg);
                if (idx == -1)
                {
                    for (int i = 0; i < Clients.Count; i++)
                    {
                        Clients[i].Send(data, data.Length, SocketFlags.None);
                    }
                } else
                {

                    Clients[idx].Send(data, data.Length, SocketFlags.None);
                }
                if (isDebug)
                {
                    Console.WriteLine("[S]{0}", msg);
                }
            } catch (SocketException e)
            {
                Console.WriteLine("전송 실패\n{0}", e.StackTrace);
            }
        }
        static void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            var Client = (Socket)sender;
            if (Client.Connected && e.BytesTransferred > 0)
            {
                byte[] szData = e.Buffer;
                string sData = Encoding.Unicode.GetString(szData);

                string Temp = sData.Replace("\0", "").Trim();
                if (isDebug)
                {
                    Console.WriteLine("[R]{0}", Temp);
                }

                string[] recv = Temp.Split('|');
                ReceiveHander(recv);



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
                Console.WriteLine("{0}의 연결이 끊어졌습니다.", Client.RemoteEndPoint.ToString());
            }
        }

        static void ReceiveHander(string[] recv)
        {
            int opcode = -1;
            int idx = -1;
            try
            {
                opcode = Int32.Parse(recv[0]);
                idx = Int32.Parse(recv[1]);
            } catch (Exception e)
            {

            }

            if (opcode == -1 || idx == -1)
            {
                return;
            }


            switch (opcode)
            {
                case 0: //getHello
                    break;
                case 1: //requestShopInfo
                    {
                        sb.Append("1<>");
                        for (int i = 0; i < shops.Count; i++)
                        {
                            Write packet = new Write(1);
                            packet.writeLine(shops[i].getId().ToString());
                            packet.writeLine(shops[i].getName().ToString());
                            packet.writeLine(shops[i].getNumber().ToString());
                            packet.writeLine(shops[i].getNew().ToString());
                            packet.writeLine(shops[i].getOld().ToString());
                            packet.writeLine(shops[i].getCategory().ToString());
                            packet.writeLine(shops[i].getImage().ToString());
                            packet.writeLine(shops[i].getStar().ToString());
                            packet.writeLine(shops[i].getTax().ToString());
                            packet.writeLine(shops[i].getCompany().ToString());
                            packet.writeLine(shops[i].getOwner().ToString());

                            sb.Append(packet.getPacket(true));
                            packet = null;
                        }
                        sb.Append("<EOF>");
                        Send(sb.ToString(), idx);
                        sb.Clear();
                        System.Threading.Thread.Sleep(250);


                        sb.Append("2<>");
                        for (int i = 0; i < shops.Count; i++)
                        {
                            for (int j = 0; j < shops[i].getItems().Count; j++)
                            {
                                Write packet = new Write(2);
                                ShopItem temp = shops[i].getItems()[j];
                                packet.writeLine(i.ToString());
                                packet.writeLine(temp.getShopId().ToString());
                                packet.writeLine(temp.getName().ToString());
                                packet.writeLine(temp.getImage().ToString());
                                packet.writeLine(temp.getPrice().ToString());
                                packet.writeLine(temp.getCategory().ToString());

                                sb.Append(packet.getPacket(true));
                                packet = null;
                            }
                        }
                        sb.Append("<EOF>");
                        Send(sb.ToString(), idx);
                        sb.Clear();
                        System.Threading.Thread.Sleep(250);

                        sb.Append("3<>");
                        for (int i = 0; i < shops.Count; i++)
                        {
                            for (int j = 0; j < shops[i].getReviews().Count; j++)
                            {
                                Write packet = new Write(3);
                                Review temp = shops[i].getReviews()[j];
                                packet.writeLine(i.ToString());
                                packet.writeLine(temp.getShopId().ToString());
                                packet.writeLine(temp.getNickName().ToString());
                                packet.writeLine(temp.getComment().ToString());
                                packet.writeLine(temp.getScore().ToString());

                                sb.Append(packet.getPacket(true));
                                packet = null;
                            }
                        }
                        sb.Append("<EOF>");
                        Send(sb.ToString(), idx);
                        sb.Clear();
                        System.Threading.Thread.Sleep(250);
                    }
                    break;
                case 3:
                    {
                        int id = Int32.Parse(recv[2]);
                        string nickname = recv[3];
                        int score = Int32.Parse(recv[4]);
                        string comment = recv[5];

                        foreach (var shop in shops)
                        {
                            if (shop.getId() == id)
                            {
                                shop.addReview(new Review(nickname, comment, score, id));
                            }
                        }
                        ShopFactory.saveReview(con, nickname, comment, id, score);
                    }
                    break;
            }
            opcode = -1;
            idx = -1;
        }


        static string ReadableByteArray(byte[] bytes) => String.Join(" ", bytes);
    }
}
