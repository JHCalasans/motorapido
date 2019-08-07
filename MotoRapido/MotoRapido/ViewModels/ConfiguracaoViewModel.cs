using Acr.Settings;
using Acr.UserDialogs;
using Microsoft.AppCenter.Crashes;
using MotoRapido.Customs;
using MotoRapido.Models;
using Newtonsoft.Json;
using Plugin.Geolocator;
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
	public class ConfiguracaoViewModel : ViewModelBase
    {
        public DelegateCommand AlterarSenhaCommand => new DelegateCommand(AlterarSenha);

        public DelegateCommand AlterarVeiculoCommand => new DelegateCommand(AlterarVeiculo);

        public DelegateCommand LogOffCommand => new DelegateCommand(LogOff);

        public ConfiguracaoViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
        }

        public async void LogOff()
        {
            try
            {
                var resposta = await DialogService.DisplayAlertAsync("AVISO", "Deseja realmente sair?", "Sim", "Não");
                if (resposta)
                {
                    UserDialogs.Instance.ShowLoading("Processando...", MaskType.Gradient);

                    var json = JsonConvert.SerializeObject(MotoristaLogado);
                      var content = new StringContent(json, Encoding.UTF8, "application/json");
                    //ConectarSocket();
                    //await WebSocketClientClass.SenMessagAsync("LogOut=>"+json);
                    //CrossSettings.Current.Clear();

                    var client = new HttpClient();
                    client.Timeout = TimeSpan.FromMilliseconds(25000);


                    using (var response = await IniciarCliente(true).PostAsync("motorista/logoff",  content))
                    {
                        UserDialogs.Instance.HideLoading();

                        CrossSettings.Current.Clear();

                       await CrossGeolocator.Current.StopListeningAsync();

                        DesconectarSocket();
                        await NavigationService.NavigateAsync("/NavigationPage/Login", useModalNavigation: true);
                    }
                }
            }
            catch (AccessViolationException e)
            {
                await DialogService.DisplayAlertAsync("Aviso", e.Message, "OK");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await DialogService.DisplayAlertAsync("Aviso", "Falha ao tentar realizar logoff", "Ok");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }

        }

        public async void AlterarSenha()
        {
            try
            {
                var resposta = await UserDialogs.Instance.PromptAsync(new PromptConfig()
                       .SetTitle("Nova Senha")
                       .SetOkText("Ok")
                       .SetCancelText("Cancelar")
                       .SetInputMode(InputType.Password));
                if (resposta.Ok)
                {
                    UserDialogs.Instance.ShowLoading("Processando...", MaskType.Gradient);
                    Motorista motorista = new Motorista();
                    motorista.codigo = MotoristaLogado.codigo;
                    motorista.senha = resposta.Value;

                    var json = JsonConvert.SerializeObject(motorista);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var client = new HttpClient();
                    client.Timeout = TimeSpan.FromMilliseconds(25000);


                    using (var response = await IniciarCliente(true).PostAsync("motorista/alterarSenha",
                        content))
                    {
                        UserDialogs.Instance.HideLoading();

                        await DialogService.DisplayAlertAsync("AVISO",
                            "Senha Alterada Com Sucesso", "Ok");


                    }
                }
            }
            catch (AccessViolationException e)
            {
                await DialogService.DisplayAlertAsync("Aviso", e.Message, "OK");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                UserDialogs.Instance.HideLoading();
                await DialogService.DisplayAlertAsync("Aviso", "Falha ao tentar alterar senha", "Ok");
            }

        }

        public async void AlterarVeiculo()
        {
            NavigationParameters param = new NavigationParameters();
            param.Add("pesquisar", true);
            await NavigationService.NavigateAsync("Veiculos", param);

        }

    }
}
