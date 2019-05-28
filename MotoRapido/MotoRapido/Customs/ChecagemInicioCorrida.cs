using Acr.Settings;
using Matcha.BackgroundService;
using MotoRapido.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MotoRapido.Customs
{
    public class ChecagemInicioCorrida : IPeriodicTask
    {
        public ChecagemInicioCorrida() { }

        public TimeSpan Interval => TimeSpan.FromSeconds(30);

        public Task<bool> StartJob()
        {
            ChecarInicioCorridaAsync();
            return null;
        }

        private async void ChecarInicioCorridaAsync()
        {
            var json = JsonConvert.SerializeObject(CrossSettings.Current.Get<SelecaoChamadaParam>("ChecagemChamada"));
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await IniciarCliente(true).PostAsync("motorista/iniciarCorrida", content);

            if (response.IsSuccessStatusCode)
            {
                BackgroundAggregatorService.StopBackgroundService();

            }
        }

        private HttpClient IniciarCliente(bool comChave)
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(35000),
                BaseAddress = new Uri("http://192.168.0.4:8080/motorapido/ws/")
            };
            if (comChave) client.DefaultRequestHeaders.Add("Authentication", CrossSettings.Current.Get<Motorista>("MotoristaLogado").chaveServicos);
            return client;
        }

    }
}
