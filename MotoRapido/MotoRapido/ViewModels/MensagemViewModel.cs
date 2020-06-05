using MotoRapido.Customs;
using MotoRapido.Interfaces;
using MotoRapido.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
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
            var tempList = ObterMensagens();
            var nvList = tempList.OrderBy(msg => msg.codMessage).ToList();
            ListMessages = new ObservableCollection<Message>(nvList);
           
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
                GravarMensagem(message, ListMessages);
               
            });
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            //Lógica para automaticamente colocar a tela de chat na ultima msg recebida
            if (ListMessages.Count > 0)
            {
                var msg = ListMessages.Last<Message>();
                ListMessages.Remove(msg);
                ListMessages.Add(msg);
            }

        }
    }
}
