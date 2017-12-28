using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using MotoRapido.Models;
using Prism.Services;

namespace MotoRapido.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }

        protected IPageDialogService DialogService { get; private set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService, IPageDialogService dialogService)
        {
            NavigationService = navigationService;
            DialogService = dialogService;
        }

        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        public virtual void Destroy()
        {
            
        }


        protected Motorista MotoristaLogado { get; set; }

        protected HttpClient IniciarCliente()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(35000);
            client.BaseAddress = new Uri("http://192.168.0.15:8080/motorapido/ws/");
            return client;
            // client.DefaultRequestHeaders.Add("Authentication",);
        }
    }
}
