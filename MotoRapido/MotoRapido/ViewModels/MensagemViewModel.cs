using Acr.Settings;
using MotoRapido.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MotoRapido.ViewModels
{
	public class MensagemViewModel : ViewModelBase
	{
        public ObservableCollection<Message> ListMessages { get; set; }
        public DelegateCommand SendCommand { get; set; }

        public string OutText
        {
            get { return _outText; }
            set { SetProperty(ref _outText, value); }
        }
        string _outText = string.Empty;

        public MensagemViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            ListMessages = new ObservableCollection<Message>(CrossSettings.Current.Get<List<Message>>("mensagens")); 

            SendCommand = new DelegateCommand(() =>
            {
                if (!String.IsNullOrWhiteSpace(OutText))
                {
                    var message = new Message
                    {
                        Text = OutText,
                        IsTextIn = false,
                        MessageDateTime = DateTime.Now
                    };


                    ListMessages.Add(message);
                    OutText = "";
                }

            });
        }


        public override void OnNavigatedTo(NavigationParameters parameters)
        {
           

        }
    }
}
