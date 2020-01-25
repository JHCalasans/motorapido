using Acr.Settings;
using Matcha.BackgroundService;
using MotoRapido.ViewModels;
using System;
using System.Threading.Tasks;

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
