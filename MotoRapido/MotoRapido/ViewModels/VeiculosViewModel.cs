using Acr.Settings;
using Acr.UserDialogs;
using Microsoft.AppCenter.Crashes;
using MotoRapido.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace MotoRapido.ViewModels
{
	public class VeiculosViewModel : ViewModelBase
	{

        public DelegateCommand<RetornoVeiculosMotorista> SelecionarVeiculoCommand =>
         new DelegateCommand<RetornoVeiculosMotorista>(SelecionarVeiculo);

        private ObservableCollection<RetornoVeiculosMotorista> _veiculos;
        public ObservableCollection<RetornoVeiculosMotorista> Veiculos
        {
            get { return _veiculos; }
            set { SetProperty(ref _veiculos, value); }
        }

        private Boolean _mostrarLista;
        public Boolean MostrarLista
        {
            get { return _mostrarLista; }
            set { SetProperty(ref _mostrarLista, value); }
        }


        public VeiculosViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("pesquisar"))
            {
               PesquisarVeiculos();
            }else if (MotoristaLogado.veiculos.Count > 0)
            {
                MostrarLista = true;
                Veiculos = new ObservableCollection<RetornoVeiculosMotorista>(MotoristaLogado.veiculos);
                
            }
            else
                MostrarLista = false;
        }

        private async void PesquisarVeiculos()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Carregando...");
                var json = JsonConvert.SerializeObject(MotoristaLogado.codigo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                using (var response = await IniciarCliente(true).PostAsync("atualizarVeiculos", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var respStr = await response.Content.ReadAsStringAsync();
                        Motorista motoTemp = new Motorista();
                        motoTemp = MotoristaLogado;
                        List<RetornoVeiculosMotorista> lista = JsonConvert.DeserializeObject<List<RetornoVeiculosMotorista>>(respStr);
                        motoTemp.veiculos = lista;
                        CrossSettings.Current.Set("MotoristaLogado", motoTemp);
                        if(motoTemp.veiculos.Count > 0 )
                            MostrarLista = true;
                        else
                            MostrarLista = false;
                        Veiculos = new ObservableCollection<RetornoVeiculosMotorista>(motoTemp.veiculos);
                    }
                    else
                    {
                        await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
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
                await DialogService.DisplayAlertAsync("Aviso", "Falha ao buscar veículos", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void SelecionarVeiculo(RetornoVeiculosMotorista veiculo)
        {
            CrossSettings.Current.Set("VeiculoSelecionado", veiculo);
            try
            {
                UserDialogs.Instance.ShowLoading("Carregando...");
                using (var response = await IniciarCliente(true).PostAsync("selecionarVeiculo/"+ MotoristaLogado.codigo+"/"+veiculo.codVeiculo,null))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        await NavigationService.NavigateAsync("//NavigationPage/Home");
                    }
                    else
                    {
                        await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
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
                await DialogService.DisplayAlertAsync("Aviso", "Falha ao selecionar veículo", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
           
          
        }

        
    }
}
