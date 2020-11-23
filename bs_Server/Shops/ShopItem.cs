using System;
using System.Collections.Generic;
using System.Text;

namespace bs_Server.Shops
{
    class ShopItem
    {
        public int Shop;
        public String Name;
        public String Image;
        public int Price;
        public int Category;

        public ShopItem(int shop_, String name_, String image_, int price_, int category_)
        {
            Shop = shop_;
            Name = name_;
            Image = image_;
            Price = price_;
            Category = category_;

        }
    }
}
