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
                    temp = new Shop(table.GetInt32("id"), table["name"].ToString(), table["number"].ToString(), table["old_address"].ToString(), table["new_address"].ToString(), table.GetInt32("category"));
                    Program.shops.Add(temp);
                }
                table.Close();

                for (var i = 0; i < Program.shops.Count; i++)
                {
                    cmd = new MySqlCommand("SELECT * FROM `shopitems` WHERE `shopid` = "+Program.shops[i].Id, con);
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

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("데이타베이스 접근에 실패했습니다.\n{0}", e.StackTrace);
                return false;
            }
        }
    }
}
