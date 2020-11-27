using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace bs_Server.Shops
{
    class Shop
    {
        private int Id;
        private String Name;
        private String Number;
        private String old_Address;
        private String new_Address;
        private int Category;
        private string Image;
        private int star;

        private string taxnumber;
        private string company;
        private string owner;

        private List<ShopItem> Items = new List<ShopItem>();
        private List<Review> Reviews = new List<Review>();

        public Shop(int id_, String name_, String number_, String old_a, String new_a, int category_, string image_, int star_, string tax, string company_, string owner_)
        {
            Id = id_;
            Name = name_;
            Number = number_;
            old_Address = old_a;
            new_Address = new_a;
            Category = category_;
            Image = image_;
            star = star_;
            taxnumber = tax;
            company = company_;
            owner = owner_;
        }

        public int getId() => Id;
        public string getName() => Name;
        public string getOld() => old_Address;
        public string getNew() => new_Address;
        public string getNumber() => Number;
        public string getTax() => taxnumber;
        public string getCompany() => company;
        public string getOwner() => owner;

        public int getCategory() => Category;
        public int getStar() => star;
        public string getImage() => Image;
        public List<ShopItem> getItems() => Items;
        public List<Review> getReviews() => Reviews;

        public bool addReview(Review a)
        {
            try
            {
                Reviews.Add(a);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("오류가 발생했습니다.\n{0}", e.StackTrace);
                return false;
            }
        }


        public bool addItem(ShopItem item)
        {
            try
            {
                Items.Add(item);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("오류가 발생했습니다.\n{0}", e.StackTrace);
                return false;
            }
        }
    }
}
