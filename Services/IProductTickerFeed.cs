using coinTicker.Models;

namespace coinTicker.Services
{
    public delegate void OnTickerReceivedDelegate(Ticker ticker);

    public interface IProductTickerFeed
    {
        OnTickerReceivedDelegate OnTickerReceived { get; set; }
        void Connect();
        void Disconnect();
        void SubscribeTicker(string productId);
        void Unsubscribe();
    }
}
