using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMWPFDesktopUI.Library.Api;
using TRMWPFDesktopUI.Library.Helpers;
using TRMWPFDesktopUI.Library.Models;
using TRMWPFDesktopUI.Models;

namespace TRMWPFDesktopUI.ViewModels
{
    public  class SalesViewModel : Screen
    {
        IProductEndpoint _productEndpoint;
        ISaleEndpoint _saleEndpoint;
        IConfigHelper _configHelper;
        IMapper _mapper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;

        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, ISaleEndpoint saleEndpoint,
            IMapper mapper, StatusInfoViewModel status ,IWindowManager window)
        {
            //on the contructor, load the items from dependency injection system
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
            _status = status;
            _window = window;
        }

        //event for when the view is loaded
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadProducts(); //load products when the view is loaded
            }
            catch (Exception ex)
            {
                //throw;
                //We want the Sales page to close down and alert the user (MessageBox)

                //create a dynamic list of settings
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form");
                    await _window.ShowDialogAsync(_status, null, settings);  //show the _status window as popup (window dislag)
                }
                else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    await _window.ShowDialogAsync(_status, null, settings);  //show the _status window as popup (window dislag)
                }

                //we can show a second dialog by repeating the calls
                //_status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form");
                //_window.ShowDialog(_status, null, settings);  //show the _status window as popup (window dislag)

                TryCloseAsync(); //after they close the dialog box, close the sales form:
            }
        }
        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            //Products = new BindingList<ProductModel>(productList);
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            //this will map the productlist to a productDisplayModel.
            Products = new BindingList<ProductDisplayModel>(products);

        }
        //listbox of products from SalesView
        private BindingList<ProductDisplayModel> _products;

        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set 
            { 
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            { 
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);  //When the item quantity changes, check the value
            }
        }

        private CartItemDisplayModel _selectedCartItem;

        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>(); //use the property not internal variable so notify of propertychanges work

            //TODO: add clearing selectedCartItem if it doesn't do it itself. 
            await LoadProducts();

            //needed to reset the numbers after clearing the cart out
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
        }

        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);  //When the item quantity changes, check the value
            }
        }

        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
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
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct); //if item not found, it's null

            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity; //add item quantitiy to existing quantity
                //this is a memory location, so the change updates SelectedProduct as well
                ////HACK to trick the system into updating the quantity on the screen - Will come back to this. 
                //Cart.Remove(existingItem);
                //Cart.Add(existingItem);

            }
            else
            {
                CartItemDisplayModel item = new CartItemDisplayModel
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
            NotifyOfPropertyChange(() => CanCheckOut);
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
            SelectedCartItem.Product.QuantityInStock += 1;
            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.QuantityInCart -= 1;
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                //make sure something is selected
                
                if (SelectedCartItem != null  && SelectedCartItem.QuantityInCart > 0)  
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task CheckOut()
        {
            //Create a SaleModel and POST to the API
            SaleModel sale = new SaleModel();
            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });               
            }
            //now the cart is converted from a cartitem list to a SaleModel
            //POST to the API.
            await _saleEndpoint.PostSale(sale);

            await ResetSalesViewModel();
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                if (Cart.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }

    }
}
