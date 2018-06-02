using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MotoRapido.ViewModels
{
	public class MensagemViewModel : ViewModelBase
	{
        public MensagemViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {

        }
	}
}
