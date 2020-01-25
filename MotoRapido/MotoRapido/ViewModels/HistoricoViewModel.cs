using MotoRapido.Models;
using Prism.Navigation;
using Prism.Services;
using System.Collections.ObjectModel;

namespace MotoRapido.ViewModels
{
    public class HistoricoViewModel : ViewModelBase
    {


        private ObservableCollection<RetornoHistoricoMotorista> _historico;
        public ObservableCollection<RetornoHistoricoMotorista> Historico
        {
            get { return _historico; }
            set { SetProperty(ref _historico, value); }
        }

        public HistoricoViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("historico"))
                Historico = (ObservableCollection<RetornoHistoricoMotorista>)parameters["historico"];
        }



    }
}
