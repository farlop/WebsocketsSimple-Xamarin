using PHS.Networking.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsocketsSimple.Client;
using WebsocketsSimple.Client.Events.Args;
using WebsocketsSimple.Client.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App7
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            using var client = new WebsocketClient(new ParamsWSClient("localhost", 65215, false));
            client.ConnectionEvent += OnConnectionEvent;
            client.MessageEvent += OnMessageEvent;
            client.ErrorEvent += OnErrorEvent;

            if (!await client.ConnectAsync())
            {
                TextLabel.Text += $"{Environment.NewLine}Not Connected: ";
            }

            await client.SendAsync($"[{DateTime.Now}] Hello WebSockets!!");

            await Task.Delay(1000);
            await client.DisconnectAsync();
        }

        private async void OnErrorEvent(object sender, WSErrorClientEventArgs args)
        {
            await MainThread.InvokeOnMainThreadAsync(() => TextLabel.Text += $"{Environment.NewLine}Error: " + args.Message);
        }

        private async void OnMessageEvent(object sender, WSMessageClientEventArgs args)
        {
            switch (args.MessageEventType)
            {
                case MessageEventType.Sent:
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        TextLabel.Text += $"{Environment.NewLine}Message sent: " + args.Message;
                    });
                    break;
                case MessageEventType.Receive:
                    await MainThread.InvokeOnMainThreadAsync(() => TextLabel.Text += $"{Environment.NewLine}Message received: " + args.Message);
                    break;
                default:
                    break;
            }
        }

        private async void OnConnectionEvent(object sender, WSConnectionClientEventArgs args)
        {
            Console.WriteLine(args.ConnectionEventType);

            switch (args.ConnectionEventType)
            {
                case ConnectionEventType.Connected:
                    await MainThread.InvokeOnMainThreadAsync(() => TextLabel.Text = $"{Environment.NewLine}Connected");

                    break;
                case ConnectionEventType.Disconnect:
                    await MainThread.InvokeOnMainThreadAsync(() => TextLabel.Text += $"{Environment.NewLine}Disconnected");

                    var client = (IWebsocketClient)sender;

                    client.ConnectionEvent -= OnConnectionEvent;
                    client.MessageEvent -= OnMessageEvent;
                    client.ErrorEvent -= OnErrorEvent;

                    client.Dispose();
                    break;
                default:
                    break;
            }
        }
    }
}
