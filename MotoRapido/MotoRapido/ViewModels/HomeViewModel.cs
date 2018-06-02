using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using Acr.Settings;
using Acr.UserDialogs;
using MotoRapido.Models;
using MotoRapido.Views;
using Newtonsoft.Json;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace MotoRapido.ViewModels
{
	public class HomeViewModel : ViewModelBase
    {
        public DelegateCommand DisponibilidadeCommand => new DelegateCommand(AlterarDisponibilidade);

        public DelegateCommand MensagemCommand => new DelegateCommand(IrParaMensagem);

        private ImageSource _imgDisponibilidade;

        public ImageSource ImgDisponibilidade
        {
            get { return _imgDisponibilidade; }
            set { SetProperty(ref _imgDisponibilidade, value); }
        }

        private Boolean _estaLivre;

        public Boolean EstaLivre
        {
            get { return _estaLivre; }
            set { SetProperty(ref _estaLivre, value); }
        }

        PermissionStatus status;

        public HomeViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {

             VerificaPermissaoLocalizacao();

           
        }

        private async void VerificaPermissaoLocalizacao()
        {
             status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await DialogService.DisplayAlertAsync("Aviso", "Preciso acessar sua localização", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

                if (results.ContainsKey(Permission.Location))
                    status = results[Permission.Location];
            }
            if (status == PermissionStatus.Granted)
            {
                if (MotoristaLogado.disponivel.Equals("S"))
                {
                    iniciarTimerPosicao();
                }
            }else if (status == PermissionStatus.Unknown || status == PermissionStatus.Denied)
             {
                await DialogService.DisplayAlertAsync("Aviso", "Permissão apra acessar localização negada.", "OK");
             }

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            if (MotoristaLogado.disponivel.Equals("S"))
            {
                ImgDisponibilidade = ImageSource.FromResource("MotoRapido.Imagens.btn_ficar_indisponive.png");
                EstaLivre = true;
            }
            else
            {
                ImgDisponibilidade = ImageSource.FromResource("MotoRapido.Imagens.btn_ficar_disponive.png");
                EstaLivre = false;
            }
            
        }

        private void BuscarInformacoesBase()
        {

        }
        
        private async void IrParaMensagem()
        {
            await NavigationService.NavigateAsync("Mensagem");
        }

        private async void AlterarDisponibilidade()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Carregando...");

                var response = await IniciarCliente(true).GetAsync("motorista/alterarDisponivel?codMotorista="+ MotoristaLogado.codigo);

                Motorista motoTemp = new Motorista();
                motoTemp = MotoristaLogado;
                if (response.IsSuccessStatusCode)
                {
                    if (motoTemp.disponivel.Equals("S"))
                    {
                        motoTemp.disponivel = "N";
                        ImgDisponibilidade = ImageSource.FromResource("MotoRapido.Imagens.btn_disponivel.png");
                        EstaLivre = false;
                        pararTimerPosicao();
                    }
                    else
                    {
                        motoTemp.disponivel = "S";
                        ImgDisponibilidade = ImageSource.FromResource("MotoRapido.Imagens.btn_indisponive.png");
                        EstaLivre = true;
                        CrossSettings.Current.Set("isTimerOn", true);
                        iniciarTimerPosicao();
                    }

                    CrossSettings.Current.Set("MotoristaLogado", motoTemp);
                    // await DialogService.DisplayAlertAsync("Aviso", "Mudou", "OK");
                }
                else
                {
                    await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
                }

            }
            catch (Exception e)
            {
                await DialogService.DisplayAlertAsync("Aviso", "Falha ao alterar disponibilidade", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }
    }
}
