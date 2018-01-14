using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using MotoRapido.Models;
using Prism.Navigation;
using Prism.Services;

namespace MotoRapido.ViewModels
{
    public class CustomBaseViewModel : ViewModelBase
    {
        public CustomBaseViewModel(INavigationService navigationService, IPageDialogService dialogService)
        :base(navigationService,dialogService)
        {
            
        }



        public Motorista MotoristaLogado { get; set; }


        protected HttpClient IniciarCliente(bool comChave)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(35000);
            client.BaseAddress = new Uri("http://192.168.0.15:8080/motorapido/ws/");
            if (comChave)
                client.DefaultRequestHeaders.Add("Authentication", MotoristaLogado.chaveServicos);
            return client;
        }
    }
}
