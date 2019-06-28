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

        public ResponderChamadaViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            string teste = null;
            //if (parameters.ContainsKey("codChamada"))
            //     teste = parameters["codChamada"].ToString();
            if (CrossSettings.Current.Contains("ChamadaParaResposta"))
            {
                teste = CrossSettings.Current.Get<string>("ChamadaParaResposta");
              //  CrossSettings.Current.Remove("ChamadaParaResposta");
            }
        }


        private async void RecusarChamada()
        {
            CrossSettings.Current.Remove("ChamadaParaResposta");
            await NavigationService.NavigateAsync("//NavigationPage/Home");
        }

        private async void IrParaChamada()
        {
            CrossSettings.Current.Remove("ChamadaParaResposta");
            await NavigationService.NavigateAsync("//NavigationPage/Home");
            //try
            //{
            //    UserDialogs.Instance.ShowLoading("Carregando...");

            //    Plugin.Geolocator.Abstractions.Position pos = await GetCurrentPosition();

            //    SelecaoChamadaParam param = new SelecaoChamadaParam();
            //    param.chamada = chamada;
            //    param.dataDecisao = DateTime.Now;
            //    param.codVeiculo = CrossSettings.Current.Get<RetornoVeiculosMotorista>("VeiculoSelecionado").codVeiculo;
            //    param.latitudeAtual = pos.Latitude.ToString();
            //    param.longitudeAtual = pos.Longitude.ToString();
            //    var json = JsonConvert.SerializeObject(param);
            //    var content = new StringContent(json, Encoding.UTF8, "application/json");

            //    var response = await IniciarCliente(true).PostAsync("motorista/selecionarChamada", content);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var respStr = await response.Content.ReadAsStringAsync();
            //        chamada = JsonConvert.DeserializeObject<Chamada>(respStr);
            //        CrossSettings.Current.Remove("ChamadaAceita");
            //        CrossSettings.Current.Set("ChamadaAceita", chamada);
            //        // var navParam = new NavigationParameters();
            //        // navParam.Add("chamadaAceita", chamada);
            //        await NavigationService.NavigateAsync("Chamada", null, true);
            //    }
            //    else
            //    {
            //        await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
            //    }
            //}
            //catch (Exception e)
            //{
            //    await DialogService.DisplayAlertAsync("Aviso", "Falha ao aceitar chamada", "OK");
            //}
            //finally
            //{
            //    UserDialogs.Instance.HideLoading();
            //}
        }
    
    }
}
