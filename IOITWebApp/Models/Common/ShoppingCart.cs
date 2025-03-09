using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOITWebApp.Models.Common
{
    public class ShoppingCartItem
    {
        public int ProductId { get; set; }
        public string ProductUrl { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductNote { get; set; }
        public double? PointStar { get; set; }
        public decimal? PriceSpecial { get; set; }
        public int? Discount { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Total { get; set; }
        public byte? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DateStart { get; set; }
    }

    public class ShoppingCart
    {
        public ShoppingCart()
        {
            ListItem = new List<ShoppingCartItem>();
        }

        public List<ShoppingCartItem> ListItem { get; set; }

        public bool AddToCart(ShoppingCartItem item)
        {
            bool alreadyExists = ListItem.Any(x => x.ProductId == item.ProductId);
            if (alreadyExists)
            {
                ShoppingCartItem existsItem = ListItem.Where(x => x.ProductId == item.ProductId).SingleOrDefault();
                if (existsItem != null)
                {
                    existsItem.Quantity = existsItem.Quantity + item.Quantity;
                    if (existsItem.PriceSpecial != null)
                    {
                        existsItem.Total = existsItem.Quantity * existsItem.PriceSpecial;
                    }
                    else if (existsItem.Price != null)
                    {
                        existsItem.Total = existsItem.Quantity * existsItem.Price;
                    }
                    else
                    {
                        existsItem.Total = 0;
                    }
                }
            }
            else
            {
                if (item.PriceSpecial != null)
                {
                    item.Total = item.Quantity * item.PriceSpecial;
                }
                else if (item.Price != null)
                {
                    item.Total = item.Quantity * item.Price;
                }
                else
                {
                    item.Total = 0;
                }
                ListItem.Add(item);
            }
            return true;
        }

        public bool RemoveFromCart(int lngProductSellID)
        {
            ShoppingCartItem existsItem = ListItem.Where(x => x.ProductId == lngProductSellID).SingleOrDefault();
            if (existsItem != null)
            {
                ListItem.Remove(existsItem);
            }
            return true;
        }

        public bool UpdateQuantity(int lngProductSellID, int intQuantity)
        {
            ShoppingCartItem existsItem = ListItem.Where(x => x.ProductId == lngProductSellID).SingleOrDefault();
            if (existsItem != null)
            {
                existsItem.Quantity = intQuantity;
                if (existsItem.PriceSpecial != null)
                {
                    existsItem.Total = existsItem.Quantity * existsItem.PriceSpecial;
                }
                else if (existsItem.Price != null)
                {
                    existsItem.Total = existsItem.Quantity * existsItem.Price;
                }
                else
                {
                    existsItem.Total = 0;
                }
            }
            return true;
        }

        //public decimal GetTotal()
        //{
        //    return ListItem.Sum(x => x.Total);
        //}

        public bool EmptyCart()
        {
            ListItem.Clear();
            return true;
        }
    }

    public class SeenProduct
    {
        public SeenProduct()
        {
            ListItem = new List<ShoppingCartItem>();
        }
        public List<ShoppingCartItem> ListItem { get; set; }

        public bool AddToBlock(ShoppingCartItem item)
        {
            ListItem.Add(item);
            return true;
        }

    }
}