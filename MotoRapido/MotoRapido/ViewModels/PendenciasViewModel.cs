using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Prism.Services;
using System.Collections.ObjectModel;
using MotoRapido.Models;

namespace MotoRapido.ViewModels
{
    public class PendenciasViewModel : ViewModelBase
    {

        private ObservableCollection<Chamada> _chamadasPendentes;
        public ObservableCollection<Chamada> ChamadasPendentes
        {
            get { return _chamadasPendentes; }
            set { SetProperty(ref _chamadasPendentes, value); }
        }

        public PendenciasViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            
        }
        
        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("chamadasPendentes"))
                ChamadasPendentes =(ObservableCollection<Chamada>) parameters["chamadasPendentes"];
        }


    }
}