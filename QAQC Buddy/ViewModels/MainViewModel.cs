using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using QAQC_Buddy.Models;
using QAQC_Buddy.Misc;

namespace QAQC_Buddy.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private PacketViewModel myPacketViewModel;
        public PacketViewModel MyPacketViewModel
        {
            get { return myPacketViewModel; }
            set { myPacketViewModel = value; RaisePropertyChanged(nameof(MyPacketViewModel)); }
        }

        private LockboxViewModel myLockboxViewModel;
        public LockboxViewModel MyLockboxViewModel
        {
            get { return myLockboxViewModel; }
            set { myLockboxViewModel = value; RaisePropertyChanged(nameof(MyLockboxViewModel)); }
        }

        public MainViewModel()
        {
            MyPacketViewModel = new PacketViewModel();
            MyLockboxViewModel = new LockboxViewModel();
        }

        #region PropertyChanged Management
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }
}
