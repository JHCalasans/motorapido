using Acr.Settings;
using MotoRapido.Customs;
using MotoRapido.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace MotoRapido.ViewModels
{
	public class MensagemViewModel : ViewModelBase
	{
        public ObservableCollection<Message> ListMessages { get; set; }
        public DelegateCommand SendCommand => new DelegateCommand(EnviarNovaMensagem);

        public string OutText
        {
            get { return _outText; }
            set { SetProperty(ref _outText, value); }
        }
        string _outText = string.Empty;

        public MensagemViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            ListMessages = new ObservableCollection<Message>(ObterMensagens());

        }

        public async void EnviarNovaMensagem()
        {
            if (!String.IsNullOrWhiteSpace(OutText))
            {
                var message = new Message
                {
                    Text = OutText,
                    IsTextIn = false,
                    MessageDateTime = DateTime.Now
                };

                GravarMensagem(message);
                ListMessages.Add(message);
              
                await ConectarSocket();
                await WebSocketClientClass.SendMessagAsync("MensagemChat=>"+ OutText);
                OutText = "";
            }
        }


        public override  void OnNavigatingTo(NavigationParameters parameters)
        {

            MessagingCenter.Unsubscribe<MensagemRespostaSocket>(this, "NovaMensagemChat");
            MessagingCenter.Subscribe<MensagemRespostaSocket>(this, "NovaMensagemChat",  (sender) =>
            {
                var resposta = sender.msg.Split(new string[] { "=>" }, StringSplitOptions.None);
                var message = new Message
                {
                    Text = resposta[0],
                    IsTextIn = true,
                    MessageDateTime = DateTime.ParseExact(resposta[1], "dd/MM/yyyy hh:mm", System.Globalization.CultureInfo.InvariantCulture)
                };
                ListMessages.Add(message);
            });
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            //  if (parameters.ContainsKey("historicoMsgs"))
            // {
               // ListMessages = new ObservableCollection<Message>((List<Message>)parameters["historicoMsgs"]);
         //   }
           
        }
    }
}
