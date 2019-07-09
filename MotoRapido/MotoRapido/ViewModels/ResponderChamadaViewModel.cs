using Acr.Settings;
using Acr.UserDialogs;
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
    public class ResponderChamadaViewModel : ViewModelBase
    {

        public DelegateCommand IrParaChamadaCommand =>
           new DelegateCommand(IrParaChamada);


        public DelegateCommand RecusarChamadaCommand =>
           new DelegateCommand(RecusarChamada);

        private Chamada _chamada;

        public Chamada chamada
        {
            get { return _chamada; }
            set { SetProperty(ref _chamada, value); }
        }

        private Boolean _expirouTempo { get; set; }

        public ResponderChamadaViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {

        }

        public override  void OnNavigatingTo(NavigationParameters parameters)
        {
            string codChamadaVeiculo = null;
            //if (parameters.ContainsKey("codChamada"))
            //     teste = parameters["codChamada"].ToString();
            ValidaTempoEspera();

            if (_expirouTempo)
                NavegarHome();
            else
            {

                if (CrossSettings.Current.Contains("ChamadaParaResposta"))
                {
                    codChamadaVeiculo = CrossSettings.Current.Get<string>("ChamadaParaResposta");
                    BuscarChamada(codChamadaVeiculo);
                    //  CrossSettings.Current.Remove("ChamadaParaResposta");
                }
            }
        }

        private async void NavegarHome()
        {
            await NavigationService.NavigateAsync("/NavigationPage/Home", useModalNavigation: true);
        }

        private async void BuscarChamada(String codChamadaVeiculo)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Carregando...");

                Plugin.Geolocator.Abstractions.Position pos = await GetCurrentPosition();

                //var json = JsonConvert.SerializeObject(Int64.Parse(codChamada));
                // var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await IniciarCliente(true).GetAsync("motorista/buscarDetalhesChamada/" + Int64.Parse(codChamadaVeiculo));

                if (response.IsSuccessStatusCode)
                {
                    var respStr = await response.Content.ReadAsStringAsync();
                    chamada = JsonConvert.DeserializeObject<Chamada>(respStr);
                    // CrossSettings.Current.Remove("ChamadaAceita");
                    // CrossSettings.Current.Set("ChamadaAceita", chamada);
                    // var navParam = new NavigationParameters();
                    // navParam.Add("chamadaAceita", chamada);
                    // await NavigationService.NavigateAsync("Chamada", null, true);
                }
                else
                {
                    await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
                }
            }
            catch (Exception e)
            {
                await DialogService.DisplayAlertAsync("Aviso", "Falha ao buscar informações da chamada", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void  ValidaTempoEspera()
        {

            if (CrossSettings.Current.Contains("dataRecebimentoChamada"))
            {
                string tempoEspera = CrossSettings.Current.Get<string>("tempoEsperaAceitacao");
                DateTime dtRecebimento = CrossSettings.Current.Get<DateTime>("dataRecebimentoChamada");
                if (DateTime.Now.Subtract(TimeSpan.FromSeconds(Double.Parse(tempoEspera))) > dtRecebimento)
                {
                    CrossSettings.Current.Remove("tempoEsperaAceitacao");
                    CrossSettings.Current.Remove("dataRecebimentoChamada");
                    CrossSettings.Current.Remove("ChamadaParaResposta");
                    await DialogService.DisplayAlertAsync("Aviso", "Tempo limite para resposta expirado", "OK");
                    _expirouTempo = true;

                }
                else
                    _expirouTempo = false;
            }
        }


        private async void RecusarChamada()
        {

            ValidaTempoEspera();
            if (_expirouTempo)
                await NavigationService.NavigateAsync("/NavigationPage/Home", useModalNavigation: true);
            else
            {
                try
                {
                    UserDialogs.Instance.ShowLoading("Processando...");

                    CancelarChamadaParam param = new CancelarChamadaParam();
                    param.chamada = chamada;
                    param.codChamadaVeiculo = chamada.codChamadaVeiculo;
                    param.dataCancelamento = DateTime.Now;
                    param.codVeiculo = CrossSettings.Current.Get<RetornoVeiculosMotorista>("VeiculoSelecionado").codVeiculo;

                    var json = JsonConvert.SerializeObject(param);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    CrossSettings.Current.Remove("ChamadaParaResposta");
                    var response = await IniciarCliente(true).PostAsync("motorista/cancelarChamada", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // CrossSettings.Current.Remove("ChamadaParaResposta");
                        await DialogService.DisplayAlertAsync("Aviso", "Corrida recusada.", "OK");
                        await NavigationService.NavigateAsync("/NavigationPage/Home", useModalNavigation: true);
                    }
                    else
                    {
                        await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
                        await NavigationService.NavigateAsync("/NavigationPage/Home", useModalNavigation: true);
                    }
                }
                catch (Exception e)
                {
                    await DialogService.DisplayAlertAsync("Aviso", "Falha ao recusar corrida", "OK");
                }
                finally
                {
                    UserDialogs.Instance.HideLoading();
                }
            }

            // await NavigationService.NavigateAsync("//NavigationPage/Home");
        }

        private async void IrParaChamada()
        {
            ValidaTempoEspera();
            if (_expirouTempo)
                await NavigationService.NavigateAsync("/NavigationPage/Home", useModalNavigation: true);
            else
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
                    await DialogService.DisplayAlertAsync("Aviso", "Falha ao aceitar chamada", "OK");
                }
                finally
                {
                    UserDialogs.Instance.HideLoading();
                }
            }
        }

    }
}
