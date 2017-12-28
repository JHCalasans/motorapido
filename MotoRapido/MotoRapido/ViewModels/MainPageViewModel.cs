using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Services;

namespace MotoRapido.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService,IPageDialogService dialogService) 
            : base (navigationService, dialogService)
        {
            Title = "Main Page";
        }
         
       
        
         
    }
}
