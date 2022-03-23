using Blazored.LocalStorage;
using eCommerceBlazor_Common;
using eCommerceBlazor_Client.Service.IService;
using eCommerceBlazor_Client.ViewModels;

namespace eCommerceBlazor_Client.Service
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorageService;
        public event Action OnChange;

        public CartService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }
        
        public async Task IncrementCart(ShoppingCart cartToAdd)
        {
            var cart = await _localStorageService.GetItemAsync<List<ShoppingCart>>(SD.ShoppingCart);
            bool itemInCart = false;

            if (cart == null)
            {
                cart = new List<ShoppingCart>();
            }
            foreach(var item in cart)
            {
                if (item.ProductId == cartToAdd.ProductId && item.ProductPriceId == cartToAdd.ProductPriceId)
                {
                    itemInCart = true;
                    item.Count += cartToAdd.Count;
                }
            }
            if (!itemInCart)
            {
                cart.Add(new ShoppingCart()
                {
                    ProductId = cartToAdd.ProductId,
                    ProductPriceId = cartToAdd.ProductPriceId,
                    Count = cartToAdd.Count
                });
            }
            await _localStorageService.SetItemAsync(SD.ShoppingCart, cart);
            OnChange.Invoke();
        }

        public async Task DecrementCart(ShoppingCart cartToDecrement)
        {
            var cart = await _localStorageService.GetItemAsync<List<ShoppingCart>>(SD.ShoppingCart);

            //If cart has 0 or 1 item, remove that item
            for(int i = 0; i < cart.Count; i++)
            {
                if (cart[i].ProductId == cartToDecrement.ProductId && cart[i].ProductPriceId == cartToDecrement.ProductPriceId)
                {
                    if (cart[i].Count == 1 || cartToDecrement.Count == 0)
                    {
                        cart.Remove(cart[i]);
                    }
                    else
                    {
                        cart[i].Count -= cartToDecrement.Count;
                    }
                }
            }
            await _localStorageService.SetItemAsync(SD.ShoppingCart, cart);
            OnChange.Invoke();
        }
    }
}
