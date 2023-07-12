using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Java.Interop;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XamarinBleWifiPhone.Adapter;
using IAdapter = Plugin.BLE.Abstractions.Contracts.IAdapter;

namespace XamarinBleWifiPhone
{
    [Activity(Label = "BluetoothActivity")]
    public class BluetoothActivity : Activity
    {
        private IAdapter _adapter;
        BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
        private List<IDevice> _gattDevices = new List<IDevice>();
        SwitchCompat swtBluetoothOnOff;
        ProgressBar progressBar;
        Button search_bt;

        //private BluetoothDeviceAdapter pairedDevicesArrayAdapter;
        private BluetoothDeviceLEAdapter bleDevicesArrayAdapter;

        //ListView paired_devices_listview;
        ListView ble_devices_listview;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_bluetooth);
            swtBluetoothOnOff = FindViewById<SwitchCompat>(Resource.Id.swt_bluetoothOnOff);
            swtBluetoothOnOff.Checked = bluetoothAdapter.IsEnabled;
            FindViewById<TextView>(Resource.Id.lbl_status).Text = swtBluetoothOnOff.Checked ? "Turn on bluetooth" : "Turn off bluetooth";
            search_bt = FindViewById<Button>(Resource.Id.search_bt);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            //paired_devices_listview = FindViewById<ListView>(Resource.Id.paired_devices_listview);
            ble_devices_listview = FindViewById<ListView>(Resource.Id.new_devices_listview);

            //pairedDevicesArrayAdapter = new BluetoothDeviceAdapter(this);
            bleDevicesArrayAdapter = new BluetoothDeviceLEAdapter(this);

            //paired_devices_listview.Adapter = pairedDevicesArrayAdapter;
            ble_devices_listview.Adapter = bleDevicesArrayAdapter;

            _adapter = CrossBluetoothLE.Current.Adapter;

            _adapter.DeviceDiscovered += (s, a) =>
            {
                _gattDevices.Add(a.Device);
            };

            BluetoothReceiver receiver = new BluetoothReceiver(this);
            IntentFilter filter = new IntentFilter(BluetoothDevice.ActionFound);
            RegisterReceiver(receiver, filter);
            filter = new IntentFilter(BluetoothAdapter.ActionStateChanged);
            RegisterReceiver(receiver, filter);
            // Register for broadcasts when discovery has finished
            filter = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);
            RegisterReceiver(receiver, filter);
            //paired_devices_listview.Visibility = ViewStates.Invisible;
            progressBar.Visibility = ViewStates.Invisible;
            //LoadPairedDevices();

        }

        [Export("swt_BluetoothOnOff_click")]
        public void swt_BluetoothOnOff_click(View v)
        {
            if(swtBluetoothOnOff.Checked)
            {
                BluetoothAdapter.DefaultAdapter.Enable();
            }else
            {
                BluetoothAdapter.DefaultAdapter.Disable();
            }
            FindViewById<TextView>(Resource.Id.lbl_status).Text = swtBluetoothOnOff.Checked ? "Turn on bluetooth" : "Turn off bluetooth";
        }
        [Export("btnSearch_click")]
        public void btnSearch_click(View v)
        {
            
            ScanBLEDevicesAsync();
            //if (bluetoothAdapter.IsDiscovering)
            //{
            //    StopScanDevices();
            //}else
            //{
            //    StartScanDevices();
            //}
        }

        public async void ScanBLEDevicesAsync()
        {
            bleDevicesArrayAdapter.Clear();
            search_bt.Text = "Stop";
            progressBar.Visibility = ViewStates.Visible;
            _gattDevices.Clear();
            await _adapter.StartScanningForDevicesAsync();
            foreach(var dev in _gattDevices)
            {
                if(dev.Name !=null && dev.Name.Length > 0 ) bleDevicesArrayAdapter.Add(dev);
            }
            search_bt.Text = "Search";
            progressBar.Visibility = ViewStates.Invisible;

        }

        public void StartScanDevices()
        {
            if(bluetoothAdapter.IsDiscovering)
            {
                bluetoothAdapter.CancelDiscovery();
            }
            bleDevicesArrayAdapter.Clear();
            //pairedDevicesArrayAdapter.Clear();

            bluetoothAdapter.StartDiscovery();
            progressBar.Visibility = ViewStates.Visible;
            search_bt.Text = "Stop";
        }

        public void StopScanDevices()
        {
            if (bluetoothAdapter.IsDiscovering)
            {
                bluetoothAdapter.CancelDiscovery();
            }
            progressBar.Visibility = ViewStates.Invisible;
            search_bt.Text = "Search";
        }

        private void LoadPairedDevices()
        {
            //var pairedDevices = bluetoothAdapter.BondedDevices;
            //pairedDevicesArrayAdapter.devicesList.Clear();
            //if (pairedDevices.Count > 0)
            //{
            //    List<BluetoothDevice> arrayDevices = new List<BluetoothDevice>();
            //    foreach (var device in pairedDevices)
            //    {
            //        pairedDevicesArrayAdapter.devicesList.Add(device);
            //    }
            //}
            //pairedDevicesArrayAdapter.NotifyDataSetChanged();
        }


        [BroadcastReceiver(Enabled = true)]
        public class BluetoothReceiver : BroadcastReceiver
        {
            Context context;
            public BluetoothReceiver():base()
            {
                
            }
            public BluetoothReceiver(Context context)
            {
                this.context = context;
            }
            public override void OnReceive(Context context, Intent intent)
            {
                var action = intent.Action;

                //if (action != BluetoothDevice.ActionFound)
                //{
                //    return;
                //}

                if (BluetoothDevice.ActionFound.Equals(action))
                {
                    var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    var ConnectState = device.BondState;
                    switch (ConnectState)
                    {
                        case Bond.None:
                            //((BluetoothActivity)this.context).newDevicesArrayAdapter.Add(device);                            
                            break;
                        case Bond.Bonded:
                            break;
                        case Bond.Bonding:
                            break;
                    }
                    
                    if (device.Name != null)
                    {
                        //Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Found device", device.Name, "ok");
                    }
                }else if(BluetoothAdapter.ActionDiscoveryFinished.Equals(action))
                {
                    ((BluetoothActivity)this.context).search_bt.Text = "Search";
                    ((BluetoothActivity)this.context).progressBar.Visibility = ViewStates.Invisible;
                    ((BluetoothActivity)this.context).LoadPairedDevices();
                }
                else if(BluetoothAdapter.ActionStateChanged.Equals(action))
                {
                    if (((BluetoothActivity)this.context).bluetoothAdapter.IsEnabled)
                    {
                        ((BluetoothActivity)this.context).LoadPairedDevices();
                    }
                    else
                    {
                        //((BluetoothActivity)this.context).newDevicesArrayAdapter.Clear();
 //                       ((BluetoothActivity)this.context).pairedDevicesArrayAdapter.Clear();
                    }
                }
            }
        }


    }
}