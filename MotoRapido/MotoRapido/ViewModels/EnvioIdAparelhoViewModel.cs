using Acr.UserDialogs;
using Com.OneSignal;
using Microsoft.AppCenter.Crashes;
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

namespace MotoRapido.ViewModels
{
    public class EnvioIdAparelhoViewModel : ViewModelBase
    {

        public DelegateCommand EnviarCommand => new DelegateCommand(Enviar);

        public EnvioIdAparelhoViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            :base(navigationService, pageDialogService)
        {

        }


        private async void Enviar()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Carregando...");
                Motorista motorista = new Motorista();

                OneSignal.Current.IdsAvailable((id, token) => motorista.idPush = id);

                var json = JsonConvert.SerializeObject(motorista);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                using (var response = await IniciarCliente(false).PostAsync("motorista/login", content))
                {
                    //var response = await ChamarServicoPost(true, "login", content);
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                           

                            //await NavigationService.NavigateAsync("//NavigationPage/Home");
                        }
                        else
                        {
                            await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }
                }

            }
            catch (AccessViolationException e)
            {
                await DialogService.DisplayAlertAsync("Aviso", e.Message, "OK");
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                await DialogService.DisplayAlertAsync("Aviso", "Falha ao enviar ID", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }


        }
    }
}
