using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VMMS_Client
{
    class CarBillViewModel : INotifyPropertyChanged
    {
        private ObjCar _Car;
        private ObservableCollection<ObjModel> _ModelNames;
        private bool _isLoading;

        public event PropertyChangedEventHandler PropertyChanged;

        public CarBillViewModel()
        {
            Car = new ObjCar();
            ModelNames = new ObservableCollection<ObjModel>();
            IsLoading = false;


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
        public ObservableCollection<ObjModel> ModelNames
        {
            get => _ModelNames;
            set
            {
                if (_ModelNames != value)
                {
                    _ModelNames = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsLoading
        {
            get => _isLoading;
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
