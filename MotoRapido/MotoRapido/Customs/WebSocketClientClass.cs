using MotoRapido.Models;
using MotoRapido.ViewModels;
using PureWebSockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MotoRapido.Customs
{
    public  class WebSocketClientClass : ViewModelBase
    {
        private  static  PureWebSocket _ws;
        private static int _sendCount;
        private static Timer _timer;

        public static void CloseWs()
        {
            _ws.Dispose(true);
        }

        public static PureWebSocket GetWs()
        {
            return _ws;
        }

        public static async Task Connect(String chaveServicos, String codMotorista)
        {
            // _timer = new Timer(OnTickAsync, null, 2000, 1);
           
            if (_ws != null && _ws.State == WebSocketState.Open) return;

            Tuple<String, String> tupla = new Tuple<string, string>("Authentication", chaveServicos);
            List<Tuple<String, String>> lista = new List<Tuple<String, String>>();
            lista.Add(tupla);
            tupla = new Tuple<string, string>("CodMotorista", codMotorista);
            lista.Add(tupla);
            IEnumerable<Tuple<String, String>> en = lista;

            var socketOptions = new PureWebSocketOptions()
            {
                DebugMode = true,
                SendDelay = 100,
                IgnoreCertErrors = true,
                MyReconnectStrategy = new ReconnectStrategy(2000, 4000, 20),
                Headers = en,
                
                
            };

            _ws = new PureWebSocket("ws://192.168.0.4:8080/motorapido/socket", socketOptions);

            _ws.OnStateChanged += Ws_OnStateChanged;
            _ws.OnMessage += Ws_OnMessage;
            _ws.OnClosed += Ws_OnClosed;
            _ws.OnSendFailed += Ws_OnSendFailed;
            await _ws.ConnectAsync();

        }

        private static void Ws_OnSendFailed(string data, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now} Send Failed: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("");
        }

        private static async void OnTickAsync(object state)
        {
            if (_ws.State != WebSocketState.Open) return;

            if (_sendCount == 1000)
            {
                _timer = new Timer(OnTickAsync, null, 30000, 1);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now} Max Send Count Reached: {_sendCount}");
                Console.ResetColor();
                Console.WriteLine("");
                _sendCount = 0;
            }
            if (await _ws.SendAsync(_sendCount + " | " + DateTime.Now.Ticks.ToString()))
            {
                _sendCount++;
            }
            else
            {
                Ws_OnSendFailed("", new Exception("Send Returned False"));
            }
        }

        public static async Task<bool> SenMessagAsync(string data)
        {
            if (_ws.State != WebSocketState.Open) return false;

            return await _ws.SendAsync(data);

        }

        private static void Ws_OnClosed(WebSocketCloseStatus reason)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now} Connection Closed: {reason}");
            Console.ResetColor();
            Console.WriteLine("");
            Console.ReadLine();
        }

        private static  void Ws_OnMessage(string message)
        {
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine($"{DateTime.Now} New message: {message}");
            //Console.ResetColor();
            //Console.WriteLine("");
            // MessagingCenter.Send(WebSocketClientClass._ws, "MSGSocket", message);
            var resp = message.Split(new string[] { "=>" }, StringSplitOptions.None);
            switch (resp[0])
            {
                case "ErroResp": Device.BeginInvokeOnMainThread(async () => await App.Current.MainPage.DisplayAlert("Aviso", resp[1], "OK")); break;
                case "ErroMotoPosResp": MessagingCenter.Send(new MensagemErroArea() { msg = resp[1]}, "ErroPosicaoArea");  break;
                case "LocalizacaoResp":   Debug.WriteLine(resp[1]); break;
                case "InformacaoPendenteResp": RemoverInfoPendente(Convert.ToInt64(resp[1])); break;
            }
           // WebSocketClientClass._ws, "ErroPosicaoArea", resp[0] + DateTime.No



        }

        private static void Ws_OnStateChanged(WebSocketState newState, WebSocketState prevState)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{DateTime.Now} Status changed from {prevState} to {newState}");
            Console.ResetColor();
            Console.WriteLine("");
        }
    }
}
