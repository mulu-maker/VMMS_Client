using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace VMMS_Client
{
    public class RepairBillViewModel : INotifyPropertyChanged
    {
        private ObjBill _objBill;
        private ObjCar _Car;
        private ObservableCollection<ObjUser> _users;
        private ObservableCollection<ObjItem> _carItems;
        private bool _isLoading;

        public event PropertyChangedEventHandler PropertyChanged;

        public RepairBillViewModel()
        {
            Obj = new ObjBill();
            Users = new ObservableCollection<ObjUser>();
            CarItems = new ObservableCollection<ObjItem>();
            IsLoading = false;



        }


        public ObjBill Obj
        {
            get => _objBill;
            set
            {
                if (_objBill != value)
                {
                    _objBill = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObjCar Car
        {
            get => _Car;
            set
            {
                if (_Car != value)
                {
                    _Car = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ObjUser> Users
        {
            get => _users;
            set
            {
                if (_users != value)
                {
                    _users = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ObjItem> CarItems
        {
            get => _carItems;
            set
            {
                if (_carItems != value)
                {
                    _carItems = value;
                    OnPropertyChanged();
                }
            }
        }


        public bool IsLoading
        {
            get  => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(); // 触发属性更改通知
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
