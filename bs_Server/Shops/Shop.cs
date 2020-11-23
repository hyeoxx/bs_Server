using System;
using System.Collections.Generic;
using System.Text;

namespace bs_Server.Shops
{
    class Shop
    {
        public int Id;
        public String Name;
        public String Number;
        public String old_Address;
        public String new_Address;
        public int Category;

        public List<ShopItem> Items = new List<ShopItem>();

        public Shop(int id_, String name_, String number_, String old_a, String new_a, int category_)
        {
            Id = id_;
            Name = name_;
            Number = number_;
            old_Address = old_a;
            new_Address = new_a;
            Category = category_;
        }


        public bool addItem(ShopItem item)
        {
            try
            {
                Items.Add(item);
                return true;
            } catch (Exception e)
            {
                Console.WriteLine("오류가 발생했습니다.\n{0}", e.StackTrace);
                return false;
            }
        }
    }
}
