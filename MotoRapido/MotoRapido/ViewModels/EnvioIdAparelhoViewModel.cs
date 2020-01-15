using Acr.Settings;
using Acr.UserDialogs;
using Com.OneSignal;
using Microsoft.AppCenter.Crashes;
using MotoRapido.Interfaces;
using MotoRapido.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace MotoRapido.ViewModels
{
    public class EnvioIdAparelhoViewModel : ViewModelBase
    {

        public DelegateCommand EnviarCommand => new DelegateCommand(Enviar);

        public EnvioIdAparelhoViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            :base(navigationService, pageDialogService)
        {

        }

        private static void NovaThread()
        {
            int c = 0;
            while (true)
            {
                IAudio audio = Xamarin.Forms.DependencyService.Get<IAudio>();
                audio.PlayAudio();
                Thread.Sleep(TimeSpan.FromSeconds(1));
                if (c == 4)
                    break;

                c++;
            }
            
        }

        private async void Enviar()
        {
          
            Thread t = new Thread(NovaThread);
            t.Start();
            
           
            //try
            //{
            //    UserDialogs.Instance.ShowLoading("Carregando...");
            //    Motorista motorista = new Motorista();
            //    motorista.idAparelho = App.DeviceID;
            //    motorista.idPush = App.OneSignalID;

            //    var json = JsonConvert.SerializeObject(motorista);
            //    var content = new StringContent(json, Encoding.UTF8, "application/json");

            //    using (var response = await IniciarCliente(false, 20000).PostAsync("motorista/enviarID", content))
            //    {
            //        //var response = await ChamarServicoPost(true, "login", content);
            //        if (response != null)
            //        {
            //            if (response.IsSuccessStatusCode)
            //            {

            //                await DialogService.DisplayAlertAsync("Aviso", "ID do aparelho enviado, entre em contato com a central para ativar o aparelho.", "OK");
            //                CrossSettings.Current.Set("IdAparelhoVinculado", true);
            //                await NavigationService.NavigateAsync("//NavigationPage/Logar");
                            
            //            }
            //            else
            //            {
            //                await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
            //            }
            //        }
            //    }

            //}
            //catch (AccessViolationException e)
            //{
            //    await DialogService.DisplayAlertAsync("Aviso", e.Message, "OK");
            //}
            //catch (Exception e)
            //{
            //    Crashes.TrackError(e);
            //    await DialogService.DisplayAlertAsync("Aviso", "Falha ao enviar ID", "OK");
            //}
            //finally
            //{
            //    UserDialogs.Instance.HideLoading();
            //}


        }
    }
}
