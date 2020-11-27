using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace bs_Server.Shops
{
    class ShopFactory
    {
        public static bool LoadShop(MySqlConnection con)
        {
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `shops`", con);
                MySqlDataReader table = cmd.ExecuteReader();

                while (table.Read())
                {
                    Shop temp;
                    temp = new Shop(table.GetInt32("id"), table["name"].ToString().Replace("\n", ""), table["number"].ToString().Replace("\n", ""), table["old_address"].ToString().Replace("\n", ""), table["new_address"].ToString().Replace("\n", ""), table.GetInt32("category"), table["image"].ToString(), table.GetInt32("star"), table["tax"].ToString(), table["company"].ToString(), table["owner"].ToString());
                    Program.shops.Add(temp);
                }
                table.Close();

                for (var i = 0; i < Program.shops.Count; i++)
                {
                    cmd = new MySqlCommand("SELECT * FROM `shopitems` WHERE `shopid` = "+Program.shops[i].getId(), con);
                    table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        var item = new ShopItem(table.GetInt32("shopid"), table["name"].ToString(), table["image"].ToString(), table.GetInt32("price"), table.GetInt32("category"));
                        if (!Program.shops[i].addItem(item))
                        {
                            return false;
                        }
                    }
                    table.Close();
                }

                cmd = new MySqlCommand("SELECT * FROM `reviews`", con);
                table = cmd.ExecuteReader();

                while (table.Read())
                {
                    Review temp;
                    foreach (var shop in Program.shops)
                    {
                        if (shop.getId() == table.GetInt32("shopid"))
                        {
                            temp = new Review(table["nickname"].ToString(), table["comment"].ToString(), table.GetInt32("score"), table.GetInt32("shopid"));
                            shop.addReview(temp);
                        }
                    }
                }
                table.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("데이타베이스 접근에 실패했습니다.\n{0}", e.StackTrace);
                return false;
            }
        }

        public static void saveReview(MySqlConnection con, string v1, string v2, int v3, int v4)
        {
            string sql = "INSERT INTO `reviews`(`shopid`, `nickname`, `comment`, `score`) VALUES (" + v3 + ", '" + v1 + "', '" + v2 + "', " + v4 + ");";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            
        }
    }
}
