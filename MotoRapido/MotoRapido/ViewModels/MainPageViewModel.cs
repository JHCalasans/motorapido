using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Diagnostics;

namespace MotoRapido.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {

        public DelegateCommand LocalidadeCommand => new DelegateCommand(GetCurrentLocation);


        public MainPageViewModel(INavigationService navigationService,IPageDialogService dialogService) 
            : base (navigationService, dialogService)
        {
            //Title = "Main Page";
        }

        public async void GetCurrentLocation()
        {
            try
            {

                var hasPermission = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (hasPermission != PermissionStatus.Granted)
                    return;

                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;


                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));


                if (position == null)
                {

                    return;
                }


                Debug.WriteLine(string.Format("Time: {0} \nLat: {1} \nLong: {2} \nAltitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \nHeading: {6} \nSpeed: {7}",
                    position.Timestamp, position.Latitude, position.Longitude,
                    position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed));
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
            


    }
}
