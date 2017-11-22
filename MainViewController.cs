using AppKit;
using coinTicker.Models;
using coinTicker.Services;

namespace coinTicker
{
    public class MainViewController : NSViewController
    {
        private readonly IDataService _dataService = new DataService();
        private readonly IFileService _fileService = new FileService();
        private readonly IProductTickerFeed _productTickerFeed = new ProductTickerFeed();
        private readonly NSMenu _mainMenu = new NSMenu();

        private NSStatusItem _statusItem;
        private Product _defaultProduct;

        public MainViewController(NSStatusItem statusItem)
        {
            _statusItem = statusItem;


            CreateMenu(statusItem);
            CreateTickerMenu();

            SetDefaultProduct(_fileService.GetDefaultProduct());
        }

        private void CreateMenu(NSStatusItem statusItem)
        {
            _statusItem = statusItem;
            var quitItem = new NSMenuItem {Title = "Quit"};
            quitItem.Activated += (sender, e) => QuitApplication();
            _mainMenu.AddItem(quitItem);
            _statusItem.Menu = _mainMenu;
        }

        private void CreateTickerMenu()
        {
           InvokeOnMainThread(async () =>
            {
                var productsItem = new NSMenuItem {Title = "Ticker"};
                _mainMenu.AddItem(productsItem);

                var allProducts = await _dataService.GetAllProducts();
                var tickerSubmenu = new NSMenu();
                _statusItem.Menu.SetSubmenu(tickerSubmenu, productsItem);

                foreach (var product in allProducts)
                {
                    var item = new NSMenuItem
                    {
                        Title = product.Id
                    };
                    item.Activated += (sender, e) =>
                    {
                        SetDefaultProduct(product);
                        _fileService.SaveDefaultProduct(product);
                    };

                    tickerSubmenu.AddItem(item);
                }
            });
        }

        private void QuitApplication()
        {
            _productTickerFeed.Disconnect();
            NSApplication.SharedApplication.Terminate(null);
        }

        private void SetCurrentPrice(double price)
        {
            InvokeOnMainThread(() =>
            {
                _statusItem.Button.Title = price.ToCurrency(_defaultProduct.TargetCurrency);
            });
        }

        private async void SetDefaultProduct(Product product)
        {
            _defaultProduct = product;

            SetCurrentPrice((await _dataService.GetTickerByProduct(_defaultProduct)).Price);
            _productTickerFeed.OnTickerReceived = (ticker) => SetCurrentPrice(ticker.Price);
            _productTickerFeed.Connect();
            _productTickerFeed.Unsubscribe();
            _productTickerFeed.SubscribeTicker(_defaultProduct.Id);
        }
    }
}
