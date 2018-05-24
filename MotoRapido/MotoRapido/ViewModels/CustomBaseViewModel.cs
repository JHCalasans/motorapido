using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using MotoRapido.Models;
using Prism.Navigation;
using Prism.Services;

namespace MotoRapido.ViewModels
{
    public class CustomBaseViewModel : ViewModelBase
    {
        public CustomBaseViewModel(INavigationService navigationService, IPageDialogService dialogService)
        :base(navigationService,dialogService)
        {
            
        }



        public Motorista MotoristaLogado { get; set; }


        public static bool IsInPolygon(Point[] poly, Point point)
        {
            var coef = poly.Skip(1).Select((p, i) =>
                    (point.Y - poly[i].Y) * (p.X - poly[i].X)
                    - (point.X - poly[i].X) * (p.Y - poly[i].Y))
                .ToList();

            if (coef.Any(p => p == 0))
                return true;

            for (int i = 1; i < coef.Count(); i++)
            {
                if (coef[i] * coef[i - 1] < 0)
                    return false;
            }
            return true;
        }

        protected void MontarPoligonos(List<CoordenadasArea> lista)
        {
            var resultado = lista.GroupBy(coord => coord.area.codigo).Select(group => group.First());
            foreach (var res in resultado)
            {
                var list = lista.Where(area => area.area == res.area);
                
            }
        }

        protected HttpClient IniciarCliente(bool comChave)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(35000);
            client.BaseAddress = new Uri("http://192.168.0.15:8080/motorapido/ws/");
            if (comChave)
                client.DefaultRequestHeaders.Add("Authentication", MotoristaLogado.chaveServicos);
            return client;
        }
    }
}
