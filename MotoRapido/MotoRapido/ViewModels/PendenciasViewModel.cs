
using Prism.Navigation;
using Prism.Services;
using System.Collections.ObjectModel;
using MotoRapido.Models;
using Prism.Commands;
using Acr.UserDialogs;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Acr.Settings;

namespace MotoRapido.ViewModels
{
    public class PendenciasViewModel : ViewModelBase
    {

        public DelegateCommand<Chamada> SelecionarChamadaCommand =>
            new DelegateCommand<Chamada>(SelecionarChamada);

        private ObservableCollection<Chamada> _chamadasPendentes;
        public ObservableCollection<Chamada> ChamadasPendentes
        {
            get { return _chamadasPendentes; }
            set { SetProperty(ref _chamadasPendentes, value); }
        }

        public PendenciasViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            
        }
        
        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("chamadasPendentes"))
                ChamadasPendentes =(ObservableCollection<Chamada>) parameters["chamadasPendentes"];
        }

        public async void SelecionarChamada(Chamada chamada)
        {

            if (App.IsGPSEnable)
            {

                var resposta = await UserDialogs.Instance.ConfirmAsync("Aceitar Chamada?", "Confirmação", "Aceitar", "Não");
                if (resposta)
                {

                    try
                    {
                        UserDialogs.Instance.ShowLoading("Carregando...");

                        Plugin.Geolocator.Abstractions.Position pos = await GetCurrentPosition();

                        SelecaoChamadaParam param = new SelecaoChamadaParam();
                        param.chamada = chamada;
                        param.dataDecisao = DateTime.Now;
                        param.codVeiculo = CrossSettings.Current.Get<RetornoVeiculosMotorista>("VeiculoSelecionado").codVeiculo;
                        param.latitudeAtual = pos.Latitude.ToString();
                        param.longitudeAtual = pos.Longitude.ToString();
                        var json = JsonConvert.SerializeObject(param);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await IniciarCliente(true).PostAsync("motorista/selecionarChamada", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var respStr = await response.Content.ReadAsStringAsync();
                            chamada = JsonConvert.DeserializeObject<Chamada>(respStr);
                            CrossSettings.Current.Remove("ChamadaAceita");
                            CrossSettings.Current.Set("ChamadaAceita", chamada);
                            // var navParam = new NavigationParameters();
                            // navParam.Add("chamadaAceita", chamada);
                            await NavigationService.NavigateAsync("Chamada", null, true);
                        }
                        else
                        {
                            await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }
                    catch (Exception e)
                    {
                        await DialogService.DisplayAlertAsync("Aviso", "Falha ao selecionar chamada", "OK");
                    }
                    finally
                    {
                        UserDialogs.Instance.HideLoading();
                    }
                }
            }
            else
            {
                await DialogService.DisplayAlertAsync("Aviso", "Ative o GPS para poder selecionar uma chamada.", "OK");
            }
        }


    }
}