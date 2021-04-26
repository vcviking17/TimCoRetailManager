﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMWPFDesktopUI.Library.Api;
using TRMWPFDesktopUI.Library.Helpers;
using TRMWPFDesktopUI.Library.Models;

namespace TRMWPFDesktopUI.ViewModels
{
    public  class SalesViewModel : Screen
    {
        IProductEndpoint _productEndpoint;
        IConfigHelper _configHelper;

        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper)
        {
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;
        }

        //event for when the view is loaded
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }
        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }
        //listbox of products from SalesView
        private BindingList<ProductModel> _products;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set 
            { 
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductModel _selectedProduct;

        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            { 
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);  //When the item quantity changes, check the value
            }
        }


        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set 
            { 
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }


        private int _itemQuantity = 1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set 
            { 
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);  //When the item quantity changes, check the value
            }
        }

        public string SubTotal
        {
            get 
            {
                decimal subTotal = CalculateSubTotal();
                return subTotal.ToString("C");
            }
        }

        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;
            foreach (var item in Cart)
            {
                subTotal += (item.Product.RetailPrice * item.QuantityInCart);
            }
            return subTotal;
        }

        public string Tax        {
            get
            {
                decimal taxAmount = CalculateTax();
                return taxAmount.ToString("C");  //there could be issues with rounding.  Add all subtotals then calculate tax or tax on each item?
            }
        }

        private decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate() / 100;
            //foreach (var item in Cart)
            //{
            //    if (item.Product.IsTaxable)
            //    {
            //        taxAmount += (item.Product.RetailPrice * item.QuantityInCart) *  taxRate;
            //        //can't multiple decimal and double since there are different precisions.
            //    }
            //}
             taxAmount = Cart
                .Where(x => x.Product.IsTaxable)
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);
            return taxAmount;
        }

        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTax();
                return total.ToString("C");
            }
        }


        //names taken from LoginView as a template
        public void AddToCart()
        {
            //is there an item in the cart already for the selected one?
            CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct); //if item not found, it's null

            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity; //add item quantitiy to existing quantity
                //this is a memory location, so the change updates SelectedProduct as well
                ////HACK to trick the system into updating the quantity on the screen - Will come back to this. 
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }
            else
            {
                CartItemModel item = new CartItemModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);  //add to listbox on UI
            }
                        
            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;  //default item quantity in text box
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
        }

        public bool CanAddToCart
        {
            get 
            {
                bool output = false;

                //make sure something is selected
                //make sure there is an item quantity
                if (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)  //if null, skip the if
                {
                    output = true;
                }

                return output; 
            }
        }

        public void RemoveFromCart()
        {

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                //make sure something is selected

                return output;
            }
        }

        public void CheckOut()
        {

        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //make sure something is in the cart

                return output;
            }
        }

    }
}
