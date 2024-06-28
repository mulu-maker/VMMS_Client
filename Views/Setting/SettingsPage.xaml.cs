using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Diagnostics;

namespace VMMS_Client
{
    public partial class SettingsPage : ContentPage
    {
        private string PageName = "设置";
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = new SettingsViewModel();
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var viewModel = (SettingsViewModel)BindingContext;
            Preferences.Set("Url", viewModel.Url);
            Preferences.Set("LoadDataGrid", viewModel.LoadDataGrid);
            Preferences.Set("OCR_Url", viewModel.OCR_Url);
            DisplayAlert(PageName, "设置保存成功!", DalPrompt.OK);
        }
        
        /// <summary>
        /// 测试服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnTestButtonClicked(object sender, EventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var timeoutTask = Task.Delay(DalPrompt.Delayed);
            // 启动数据加载任务
            var loadUsersTask = Api.GetTestViewList();

            // 等待数据加载任务和超时任务中的任何一个完成
            var completedTask = await Task.WhenAny(loadUsersTask, timeoutTask);
            stopwatch.Stop();
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            // 如果完成的任务是超时任务，则显示超时警告
            if (completedTask == timeoutTask)
            {
                await DisplayAlert(PageName, "测试超时!", DalPrompt.OK);
            }
            else
            {
                // 否则，检查数据加载任务是否成功
                if (await loadUsersTask)
                {
                    await DisplayAlert(PageName, $"通讯时间：{elapsedMilliseconds}MS,测试成功!", DalPrompt.OK);
                }
                else
                {
                    await DisplayAlert(PageName, "测试失败!",  DalPrompt.OK);
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
