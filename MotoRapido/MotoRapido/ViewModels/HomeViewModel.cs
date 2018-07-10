﻿using Prism.Commands;
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

        
        private Color _corDeFundoStatus;
        public Color CorDeFundoStatus
        {
            get => _corDeFundoStatus;
            set => SetProperty(ref _corDeFundoStatus, value);
        }
       

        private String _textoStatus;
        public string TextoStatus
        {
            get => _textoStatus;
            set => SetProperty(ref _textoStatus, value);
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
                ImgDisponibilidade = ImageSource.FromResource("MotoRapido.Imagens.btn_ficar_indisponivel.png");
                EstaLivre = true;
                CorDeFundoStatus = Color.Green;
                TextoStatus = "LIVRE";
            }
            else
            {
                ImgDisponibilidade = ImageSource.FromResource("MotoRapido.Imagens.btn_ficar_disponivel.png");
                EstaLivre = false;
                CorDeFundoStatus = Color.Red;
                TextoStatus = "OCUPADO";
            }
            
        }

        private void BuscarInformacoesBase()
        {

        }
        
        private async void IrParaMensagem()
        {

            if (!CrossSettings.Current.Contains("mensagens") || CrossSettings.Current.Get<List<MensagemMotoristaFuncionario>>("mensagens").Count < 1)
            {
                try
                {
                    UserDialogs.Instance.ShowLoading("Carregando...");

                    MensagemParam param = new MensagemParam
                    {
                        codMotorista = MotoristaLogado.codigo
                    };

                    var json = JsonConvert.SerializeObject(param);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await IniciarCliente(true).PostAsync("motorista/atualizarMensagens", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var respStr = await response.Content.ReadAsStringAsync();
                        CrossSettings.Current.Set("mensagens", JsonConvert.DeserializeObject<List<Message>>(respStr));
                       
                    }
                    else
                    {
                        await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
                    }

                }
                catch (Exception e)
                {
                    await DialogService.DisplayAlertAsync("Aviso", "Falha ao buscar mensagens", "OK");
                }
                finally
                {
                    UserDialogs.Instance.HideLoading();
                }

            }
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
                        ImgDisponibilidade = ImageSource.FromResource("MotoRapido.Imagens.btn_ficar_disponivel.png");
                        EstaLivre = false;
                        CorDeFundoStatus = Color.Red;
                        TextoStatus = "OCUPADO";

                        await StopListening();
                    }
                    else
                    {
                        motoTemp.disponivel = "S";
                        ImgDisponibilidade = ImageSource.FromResource("MotoRapido.Imagens.btn_ficar_indisponivel.png");
                        EstaLivre = true;
                        CorDeFundoStatus = Color.Green;
                        TextoStatus = "LIVRE";
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
