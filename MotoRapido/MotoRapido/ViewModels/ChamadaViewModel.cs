using Acr.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MotoRapido.ViewModels
{
	public class ChamadaViewModel : ViewModelBase
	{



        private String _urlMapa;
        public String UrlMapa
        {
            get { return _urlMapa; }
            set { SetProperty(ref _urlMapa, value); }
        }


        public ChamadaViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            if (CrossSettings.Current.Contains("ExisteChamada"))            
                CrossSettings.Current.Remove("ExisteChamada");

            UrlMapa = "https://www.google.com/maps/search/?api=1&query=-10.950752,-37.069523";
            //"http://maps.google.com/maps?&daddr=-10.950752,-37.069523";


        }

    }
	
}
