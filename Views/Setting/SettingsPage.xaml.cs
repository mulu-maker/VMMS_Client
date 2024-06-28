using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Diagnostics;

namespace VMMS_Client
{
    public partial class SettingsPage : ContentPage
    {
        private string PageName = "����";
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = new SettingsViewModel();
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var viewModel = (SettingsViewModel)BindingContext;
            Preferences.Set("Url", viewModel.Url);
            Preferences.Set("LoadDataGrid", viewModel.LoadDataGrid);
            Preferences.Set("OCR_Url", viewModel.OCR_Url);
            DisplayAlert(PageName, "���ñ���ɹ�!", DalPrompt.OK);
        }
        
        /// <summary>
        /// ���Է�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnTestButtonClicked(object sender, EventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var timeoutTask = Task.Delay(DalPrompt.Delayed);
            // �������ݼ�������
            var loadUsersTask = Api.GetTestViewList();

            // �ȴ����ݼ�������ͳ�ʱ�����е��κ�һ�����
            var completedTask = await Task.WhenAny(loadUsersTask, timeoutTask);
            stopwatch.Stop();
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            // �����ɵ������ǳ�ʱ��������ʾ��ʱ����
            if (completedTask == timeoutTask)
            {
                await DisplayAlert(PageName, "���Գ�ʱ!", DalPrompt.OK);
            }
            else
            {
                // ���򣬼�����ݼ��������Ƿ�ɹ�
                if (await loadUsersTask)
                {
                    await DisplayAlert(PageName, $"ͨѶʱ�䣺{elapsedMilliseconds}MS,���Գɹ�!", DalPrompt.OK);
                }
                else
                {
                    await DisplayAlert(PageName, "����ʧ��!",  DalPrompt.OK);
                }
            }


        }
    }

    public class SettingsViewModel : BindableObject
    {
        private string _Url;
        private string _LoadDataGrid;
        private string _OCR_Url;

        public SettingsViewModel()
        {
            Url = Preferences.Get("Url", string.Empty);
            LoadDataGrid = Preferences.Get("LoadDataGrid", string.Empty);
            OCR_Url = Preferences.Get("OCR_Url", string.Empty);
        }

        public string Url
        {
            get => _Url;
            set
            {
                _Url = value;
                OnPropertyChanged();
            }
        }

        public string LoadDataGrid
        {
            get => _LoadDataGrid;
            set
            {
                _LoadDataGrid = value;
                OnPropertyChanged();
            }
        }

        public string OCR_Url
        {
            get => _OCR_Url;
            set
            {
                _OCR_Url = value;
                OnPropertyChanged();
            }
        }
    }
}
