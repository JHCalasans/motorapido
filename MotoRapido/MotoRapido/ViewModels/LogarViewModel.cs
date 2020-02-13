using Acr.Settings;
using Acr.UserDialogs;
using Microsoft.AppCenter.Crashes;
using MotoRapido.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;

namespace MotoRapido.ViewModels
{
    public class LogarViewModel : ViewModelBase
    {

        public DelegateCommand LoginCommand => new DelegateCommand(Logar);

        private String _idMoto;

        public String IdMoto
        {
            get { return _idMoto; }
            set { SetProperty(ref _idMoto, value); }

        }

        private String _senha;

        public String Senha
        {
            get { return _senha; }
            set { SetProperty(ref _senha, value); }

        }

        public LogarViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {

        }

        private async void Logar()
        {

            if (String.IsNullOrEmpty(IdMoto))
                await DialogService.DisplayAlertAsync("Aviso", "Informe o Login", "OK");
            else if (String.IsNullOrEmpty(Senha))
                await DialogService.DisplayAlertAsync("Aviso", "Informe a Senha", "OK");
            else
            {
                try
                {
                    UserDialogs.Instance.ShowLoading("Carregando...");
                    Motorista motorista = new Motorista();
                    motorista.senha = HashPassword(Senha);
                    motorista.iDMotorista = Int32.Parse(IdMoto);
                    motorista.idAparelho = App.DeviceID;
                    motorista.idPush = App.OneSignalID;

                    // OneSignal.Current.IdsAvailable((id, token) => motorista.idPush = id);

                    var json = JsonConvert.SerializeObject(motorista);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    using (var response = await IniciarCliente(false).PostAsync("motorista/login", content))
                    {
                        //var response = await ChamarServicoPost(true, "login", content);
                        if (response != null)
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var respStr = await response.Content.ReadAsStringAsync();
                                CrossSettings.Current.Set("MotoristaLogado", JsonConvert.DeserializeObject<Motorista>(respStr));
                                CrossSettings.Current.Set("IsTimerOn", MotoristaLogado.disponivel.Equals("S"));
                                // CrossSettings.Current.Set("UltimaLocalizacaoValida", );
                                // ConectarSocket();
                                MessagingCenter.Subscribe<MensagemRespostaSocket>(this, "ErroPosicaoArea", (sender) =>
                                {

                                    AreaPosicao = new RetornoVerificaPosicao() { msgErro = sender.msg };

                                });



                                await NavigationService.NavigateAsync("//NavigationPage/Veiculos", null, true);

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
                    await DialogService.DisplayAlertAsync("Aviso", "Falha ao efetuar login", "OK");
                }
                finally
                {
                    UserDialogs.Instance.HideLoading();
                }
            }

        }

        private static string HashPassword(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }
        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }






}
