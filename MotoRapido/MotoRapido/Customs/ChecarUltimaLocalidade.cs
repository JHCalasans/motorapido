using Acr.Settings;
using Matcha.BackgroundService;
using MotoRapido.ViewModels;
using System;
using System.Threading.Tasks;

namespace MotoRapido.Customs
{
    public class ChecarUltimaLocalidade : ViewModelBase, IPeriodicTask
    {

        public ChecarUltimaLocalidade() { }

        public TimeSpan Interval => TimeSpan.FromMinutes(1);

        public async Task<bool> StartJob()
        {
            if (new DateTime().Subtract(CrossSettings.Current.Get<DateTime>("UltimaAtualizacaoLocalidade")).TotalMinutes > 4)
            {
                ChecarLocalizaao();
            }
            return true;
        }

        private void ChecarLocalizaao()
        {
            Localizar(GetCurrentPosition().Result);
        }
    }
}
