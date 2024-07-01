using PHS.Networking.Enums;
using System;
using System.Threading.Tasks;
using WebsocketsSimple.Server;
using Xamarin.Forms;

namespace App7
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        WebsocketServer _server;

        protected override async void OnStart()
        {
            _server = new WebsocketServer(new(65215, "Connected Successfully"));
            _server.MessageEvent += Server_MessageEvent;
            _server.ServerEvent += Server_ServerEvent;
            _server.ConnectionEvent += Server_ConnectionEvent;
            _server.ErrorEvent += Server_ErrorEvent;

            await _server.StartAsync();
        }

        private void Server_ErrorEvent(object sender, WebsocketsSimple.Server.Events.Args.WSErrorServerEventArgs args)
        {
            Console.WriteLine(args.Message);
        }

        private void Server_ConnectionEvent(object sender, WebsocketsSimple.Server.Events.Args.WSConnectionServerEventArgs args)
        {
            Console.WriteLine(args.ConnectionEventType + " " + _server.ConnectionCount);
        }

        private void Server_ServerEvent(object sender, PHS.Networking.Server.Events.Args.ServerEventArgs args)
        {
            Console.WriteLine(args.ServerEventType);
        }

        private void Server_MessageEvent(object sender, WebsocketsSimple.Server.Events.Args.WSMessageServerEventArgs args)
        {
            switch (args.MessageEventType)
            {
                case MessageEventType.Sent:
                    break;
                case MessageEventType.Receive:
                    Console.WriteLine(args.MessageEventType + ": " + args.Message);

                    Task.Run(async () =>
                    {
                        Console.WriteLine("Connections: " + _server.ConnectionCount);
                        await _server.BroadcastToAllConnectionsAsync(args.Message);
                    });
                    break;
                default:
                    break;
            }
        }
    }
}
