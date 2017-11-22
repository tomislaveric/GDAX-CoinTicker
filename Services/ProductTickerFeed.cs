using System;
using System.Collections.Generic;
using System.Linq;
using coinTicker.Models;
using coinTicker.Models.Feed;
using Newtonsoft.Json;
using WebSocketSharp;

namespace coinTicker.Services
{
    public class ProductTickerFeed : IProductTickerFeed
    {
        private const string _feedUrl = "wss://ws-feed.gdax.com";

        private readonly List<Message> _pendingRequestMessages = new List<Message>();

        private WebSocket _ws;
        private Message _activeSubscriptions;

        public OnTickerReceivedDelegate OnTickerReceived { get; set; }

        public void Connect()
        {
            if (_ws == null || _ws.ReadyState == WebSocketState.Closed)
            {
                if (_ws == null)
                {
                    _ws = new WebSocket(_feedUrl);
                    _ws.OnMessage += (sender, args) => OnWebSocketMessage(args);
                    _ws.OnClose += (sender, args) => OnWebSocketClose();
                    _ws.OnOpen += (sender, args) => OnWebSocketOpen();
                    _ws.OnError += (sender, args) => OnWebSocketError(args);
                }

                _ws.Connect();
            }
        }

        public void Disconnect()
        {
            if (_ws != null && _ws.ReadyState == WebSocketState.Open)
            {
                _pendingRequestMessages.Clear();
                _ws = null;
            }
        }

        public void SubscribeTicker(string productId)
        {
            var message = new Message
            {
                Type = MessageType.Subscribe,
                ProductIds = new List<string> { productId },
                Channels = new List<object> { ChannelType.Ticker }
            };

            Send(message);
        }

        public void Unsubscribe()
        {
            if (_activeSubscriptions != null && _activeSubscriptions.Channels != null)
            {
                var channels = _activeSubscriptions.Channels.Select(item => {
                    if (item is Channel channel)
                    {
                        return channel.Name;
                    }

                    return item.ToString();
                }).ToList();

                var message = new Message
                {
                    Type = MessageType.Unsubscribe,
                    ProductIds = _activeSubscriptions.ProductIds,
                    Channels = new List<object>(channels),
                };

                Send(message);
            }
        }

        private void OnWebSocketError(ErrorEventArgs args)
        {
            Console.WriteLine($"Realtime feed error: {args.Message}");
        }

        private void OnWebSocketMessage(MessageEventArgs args)
        {
            Console.WriteLine($"Received realtime feed message: {args.Data}");

            try
            {
                var message = JsonConvert.DeserializeObject<Message>(args.Data);

                if (message.Channels != null && message.Channels.Count > 0)
                {
                    List<object> newChannels = new List<object>();

                    message.Channels.ForEach(obj => {
                        Channel channel = null;

                        try
                        {
                            channel = JsonConvert.DeserializeObject<Channel>(obj.ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error while deserializing realtime feed message channel: {ex.Message}");
                        }

                        newChannels.Add(channel ?? obj);
                    });

                    message.Channels = newChannels;
                }

                switch (message.Type)
                {
                    case MessageType.Subscriptions:
                        _activeSubscriptions = message;
                        break;
                    case MessageType.Ticker:
                        HandleTickerMessage(args.Data);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deserializing realtime feed message: {ex.Message}");
            }
        }

        private void OnWebSocketClose()
        {
            Console.WriteLine("Realtime feed disconnected");
        }

        private void OnWebSocketOpen()
        {
            Console.WriteLine("Realtime feed connected");
            _pendingRequestMessages.ForEach(message => Send(message, true));
        }

        private void Send(Message message, bool isAlreadyPending = false)
        {
            if (_ws.ReadyState == WebSocketState.Open)
            {
                if (isAlreadyPending)
                {
                    _pendingRequestMessages.Remove(message);
                }

                _ws.Send(JsonConvert.SerializeObject(message));
            }
            else if (isAlreadyPending)
            {
                _pendingRequestMessages.Add(message);
            }
        }

        private void HandleTickerMessage(string data)
        {
            try
            {
                OnTickerReceived?.Invoke(JsonConvert.DeserializeObject<Ticker>(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deserializing Ticker data: {ex.Message}");
            }
        }
    }
}
