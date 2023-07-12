using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using Java.Interop;
using System;

namespace XamarinBleWifiPhone
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class HomeActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            
            SetContentView(Resource.Layout.activity_home);
        }

        [Export("btnBluetooth_click")]
        public void btnBluetooth_click(View v)
        {
            StartActivity(new Intent(Application.Context, typeof(BluetoothActivity)));
        }

        [Export("btnControl_click")]
        public void btnControl_click(View v)
        {
            StartActivity(new Intent(Application.Context, typeof(SplashActivity)));
        }
    }
}