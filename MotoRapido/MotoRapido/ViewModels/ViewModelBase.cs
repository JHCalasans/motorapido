using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Acr.Settings;
using MotoRapido.Interfaces;
using MotoRapido.Models;
using Prism.Services;
using MotoRapido.Customs;

namespace MotoRapido.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }

        protected IPageDialogService DialogService { get; private set; }

        private StoppableTimer _stoppableTimer;

        public StoppableTimer StoppableTimer
        {
            get => _stoppableTimer;
            set { SetProperty(ref _stoppableTimer, value); }
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

        public void iniciarTimer()
        {
            if (StoppableTimer == null)
                StoppableTimer = new StoppableTimer(TimeSpan.FromSeconds(2), null);

            StoppableTimer.Start();
            //int count = 0;
            //Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            //{

            //    Debug.WriteLine(count + " - " + TimerOn);
            //    count++;
            //    return TimerOn; // True = Repeat again, False = Stop the timer
            //});
        }

        public void pararTimer()
        {
            //StoppableTimer = new StoppableTimer(TimeSpan.FromSeconds(2), teste);
            StoppableTimer.Stop();
            //int count = 0;
            //Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            //{

            //    Debug.WriteLine(count + " - " + TimerOn);
            //    count++;
            //    return TimerOn; // True = Repeat again, False = Stop the timer
            //});
        }


        public Motorista MotoristaLogado
        {
            get { return CrossSettings.Current.Get<Motorista>("MotoristaLogado"); }
        }

       


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
