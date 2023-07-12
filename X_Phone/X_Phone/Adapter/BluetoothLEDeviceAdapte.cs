﻿using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamarinBleWifiPhone.Adapter
{
    public class BluetoothDeviceLEAdapter : BaseAdapter<IDevice>
    {

        Context context;
        public List<IDevice> devicesList = new List<IDevice>();
        int resId;

        public BluetoothDeviceLEAdapter(Context context) //: base(context)
        {
            this.context = context;
            //this.resId = resId;
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override IDevice this[int position]
        {
            get { return devicesList[position]; }
        }
        public override int Count
        {
            get { return devicesList.Count; }
        }
        public void removeItemAt(int index)
        {
            devicesList.RemoveAt(index);
            this.NotifyDataSetChanged();
        }
        public void Clear()
        {
            devicesList.Clear();
            this.NotifyDataSetChanged();
        }
        public void Add(IDevice dev)
        {
            devicesList.Add(dev);
            this.NotifyDataSetChanged();
        }

        //public override BluetoothDevice this[int position] => throw new NotImplementedException();

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View currentItemView = convertView;
            if (currentItemView == null)
            {
                currentItemView = LayoutInflater.From(this.context).Inflate(Resource.Layout.device_list_item, parent, false);
            }

            // get the position of the view from the ArrayAdapter
            IDevice currentDevice = devicesList[position];

            TextView textView1 = currentItemView.FindViewById<TextView>(Resource.Id.textView1);
            textView1.Text = currentDevice.Name;

            TextView textView2 = currentItemView.FindViewById<TextView>(Resource.Id.textView2);
            textView2.Text = (currentDevice.NativeDevice as BluetoothDevice).Address;
            //var view = convertView;
            //BluetoothDeviceAdapterViewHolder holder = null;

            //if (view != null)
            //    holder = view.Tag as BluetoothDeviceAdapterViewHolder;

            //if (holder == null)
            //{
            //    holder = new BluetoothDeviceAdapterViewHolder();
            //    var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
            //    //replace with your item and your holder items
            //    //comment back in
            //    //view = inflater.Inflate(Resource.Layout.item, parent, false);
            //    //holder.Title = view.FindViewById<TextView>(Resource.Id.text);
            //    view.Tag = holder;
            //}


            ////fill in your items
            ////holder.Title.Text = "new text here";

            return currentItemView;
        }
    }
}