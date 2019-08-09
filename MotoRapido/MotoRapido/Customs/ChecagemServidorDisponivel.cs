using Acr.Settings;
using Matcha.BackgroundService;
using MotoRapido.Models;
using MotoRapido.ViewModels;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MotoRapido.Customs
{
    public class ChecagemServidorDisponivel : ViewModelBase,IPeriodicTask
    {
        public ChecagemServidorDisponivel() { }

        public TimeSpan Interval => TimeSpan.FromSeconds(15);

        public async Task<bool> StartJob()
        {
            ChecagemServidorDisponivelAsync();
            
            return true;
        }

        private async void ChecagemServidorDisponivelAsync()
        {
            Plugin.Geolocator.Abstractions.Position pos = await GetCurrentPosition();
            CrossSettings.Current.Set("UltimaLocalizacaoValida", pos);
            BuscaPosicao(pos);
        }

     
      
    }
}
