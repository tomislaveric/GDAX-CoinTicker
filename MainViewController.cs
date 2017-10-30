using System;
using System.Timers;
using AppKit;
using coinTicker.Models;
using coinTicker.Services;

namespace coinTicker
{
    public class MainViewController : NSViewController
    {
        private readonly IDataService _dataService = new DataService();
        private readonly IFileService _fileService = new FileService();
        private readonly NSMenu mainMenu = new NSMenu();

        private NSStatusItem statusItem { get; set; }
        private Product defaultProduct { get; set; }

        public MainViewController(NSStatusItem statusItem)
        {
            this.statusItem = statusItem;
            defaultProduct = _fileService.GetDefaultProduct();

            CreateMenu(statusItem);
            CreateTickerMenu();

            StartTickerInterval(10000);
        }

        private void CreateMenu(NSStatusItem _statusItem)
        {
            statusItem = _statusItem;
            var quitItem = new NSMenuItem {Title = "Quit"};
            quitItem.Activated += (sender, e) => QuitApplication();
            mainMenu.AddItem(quitItem);
            statusItem.Menu = mainMenu;
        }

        private void CreateTickerMenu()
        {
           InvokeOnMainThread(async () =>
            {
                var productsItem = new NSMenuItem {Title = "Ticker"};
                mainMenu.AddItem(productsItem);

                var allProducts = await _dataService.GetAllProducts();
                var tickerSubmenu = new NSMenu();
                statusItem.Menu.SetSubmenu(tickerSubmenu, productsItem);

                foreach (var product in allProducts)
                {
                    var item = new NSMenuItem
                    {
                        Title = product.Id
                    };
                    item.Activated += (sender, e) =>
                    {
                        defaultProduct = product;
                        _fileService.SaveDefaultProduct(product);
                        SetCurrentPrice(product);
                    };

                    tickerSubmenu.AddItem(item);
                }
            });
        }

        private void StartTickerInterval(int milliSeconds)
        {
            SetCurrentPrice(defaultProduct);
            var timer = new Timer(milliSeconds);
            timer.Elapsed += (obj, e) => { SetCurrentPrice(defaultProduct); };
            timer.Enabled = true;
            GC.KeepAlive(timer);
        }

        private void QuitApplication()
        {
            NSApplication.SharedApplication.Terminate(null);
        }

        private void SetCurrentPrice(Product product)
        {
            InvokeOnMainThread(async () =>
            {
                var fetchedProduct = await _dataService.GetTickerByProduct(defaultProduct);
                statusItem.Button.Title = fetchedProduct.Price.ToCurrency(defaultProduct.TargetCurrency);
            });
        }
    }
}
