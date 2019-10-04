using Acr.Settings;
using Acr.UserDialogs;
using Microsoft.AppCenter.Crashes;
using MotoRapido.Models;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
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

        private Boolean _isPendencia { get; set; }

        public ResponderChamadaViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            //string codChamadaVeiculo = null;
            _isPendencia = parameters.ContainsKey("IsPendencia");
            if (parameters.ContainsKey("ChamadaSelecionada"))
            {
                chamada = (Chamada)parameters["ChamadaSelecionada"];
            }          
            else
            {


                if (CrossSettings.Current.Contains("ChamadaParaResposta"))
                {
                    chamada = CrossSettings.Current.Get<Chamada>("ChamadaParaResposta");
                    ValidaTempoEspera();
                    if (_expirouTempo)
                        NavegarHome();
                    else
                    {
                        
                        // BuscarChamada(codChamadaVeiculo);
                        //  CrossSettings.Current.Remove("ChamadaParaResposta");
                    }
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

                // Plugin.Geolocator.Abstractions.Position pos = await GetCurrentPosition();

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
            catch (AccessViolationException e)
            {
                await DialogService.DisplayAlertAsync("Aviso", e.Message, "OK");
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                await DialogService.DisplayAlertAsync("Aviso", "Falha ao buscar informações da chamada", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void ValidaTempoEspera()
        {

            if (CrossSettings.Current.Contains("dataRecebimentoChamada"))
            {
               // int tempoEspera = chamada.tempoParaResposta;//CrossSettings.Current.Get<string>("tempoEsperaAceitacao");
                //DateTime dtRecebimento = CrossSettings.Current.Get<DateTime>("dataRecebimentoChamada");
                if (DateTime.Now.Subtract(TimeSpan.FromSeconds(chamada.tempoParaResposta)) > chamada.dataRecebimento)
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

                    var navParam = new NavigationParameters();
                    if(!_isPendencia)
                        CrossSettings.Current.Set("RecusouChamada", true);
                    else
                        CrossSettings.Current.Set("RecusouChamadaPendente", true);

                    if (response.IsSuccessStatusCode)
                    {
                        // CrossSettings.Current.Remove("ChamadaParaResposta");
                        await DialogService.DisplayAlertAsync("Aviso", "Corrida recusada.", "OK");                       
                        await NavigationService.NavigateAsync("//NavigationPage/Home");
                    }
                    else
                    {
                        await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
                        await NavigationService.NavigateAsync("//NavigationPage/Home");
                    }
                }
                catch (AccessViolationException e)
                {
                    await DialogService.DisplayAlertAsync("Aviso", e.Message, "OK");
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
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
                if (CrossConnectivity.Current.IsConnected)
                {
                    try
                    {
                        UserDialogs.Instance.ShowLoading("Carregando...");

                        Plugin.Geolocator.Abstractions.Position pos = await GetCurrentPosition();

                        SelecaoChamadaParam param = new SelecaoChamadaParam();
                        param.chamada = chamada;
                        param.dataDecisao = DateTime.Now;
                        param.codVeiculo = CrossSettings.Current.Get<RetornoVeiculosMotorista>("VeiculoSelecionado").codVeiculo;
                        param.latitudeAtual = pos.Latitude.ToString().Replace(",", ".");
                        param.longitudeAtual = pos.Longitude.ToString().Replace(",", ".");
                        var json = JsonConvert.SerializeObject(param);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await IniciarCliente(true).PostAsync("motorista/aceitarChamada", content);


                        if (response.IsSuccessStatusCode)
                        {
                            var respStr = await response.Content.ReadAsStringAsync();
                            chamada = JsonConvert.DeserializeObject<Chamada>(respStr);
                            CrossSettings.Current.Remove("ChamadaAceita");
                            CrossSettings.Current.Set("ChamadaAceita", chamada);
                            CrossSettings.Current.Remove("tempoEsperaAceitacao");
                            CrossSettings.Current.Remove("dataRecebimentoChamada");
                            CrossSettings.Current.Remove("ChamadaParaResposta");
                            // var navParam = new NavigationParameters();
                            // navParam.Add("chamadaAceita", chamada);
                            await NavigationService.NavigateAsync("Chamada", null, true);
                        }
                        else
                        {
                            await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
                        }
                    }
                    catch (AccessViolationException e)
                    {
                        await DialogService.DisplayAlertAsync("Aviso", e.Message, "OK");
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                        await DialogService.DisplayAlertAsync("Aviso", "Falha ao aceitar chamada", "OK");
                    }
                    finally
                    {
                        UserDialogs.Instance.HideLoading();
                    }
                }
                else
                {
                    await DialogService.DisplayAlertAsync("Aviso", " Sem conexão com internet!", "OK");
                }
            }
        }

    }
}
