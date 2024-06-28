
using System;
using System.Text.RegularExpressions;
using static Android.App.ActionBar;

namespace VMMS_Client 
{

    public partial class CarBillPage : ContentPage
    {
        private string PageName = "车辆数据";
        public bool IsAdd = true;
        public string OlicensePlate  = string.Empty;

        // 定义回调
        public Action<string> OnCarInfoSaved { get; set; }

        private CarBillViewModel viewModel;
        public CarBillPage()
        {
            InitializeComponent();
            viewModel = new CarBillViewModel();
            BindingContext = viewModel;
        }
        /// <summary>
        /// 页面加载时运行的代码
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // 页面加载时运行的代码
            if (!IsAdd)
            {
                viewModel.Car.LicensePlate = OlicensePlate;
            }

            LoadDataAsync();
        }

        /// <summary>
        /// 异步加载数据的逻辑
        /// </summary>
        /// <returns></returns>
        private async Task LoadDataAsync()
        {
            // 异步加载数据的逻辑
            // 比如从服务器获取数据、初始化视图模型等
            IsLoading(true);
            var timeoutTask = Task.Delay(DalPrompt.Delayed);
            // 启动数据加载任务
            var loadUsersTask = LoadData();

            // 等待数据加载任务和超时任务中的任何一个完成
            var completedTask = await Task.WhenAny(loadUsersTask, timeoutTask);
            IsLoading();
            // 如果完成的任务是超时任务，则显示超时警告
            if (completedTask == timeoutTask)
            {
                await DisplayAlert(PageName, "加载数据超时!", DalPrompt.OK);
            }
            else
            {
                // 否则，检查数据加载任务是否成功
                if (!await loadUsersTask)
                {
                    await DisplayAlert(PageName, "加载数据异常!", DalPrompt.OK);
                }
            }


        }

        private void VINRecognitionButtonClicked(object sender, EventArgs e)
        {
            TakePhoto_VIN();
        }

        private void LicensePlateRecognitionButtonClicked(object sender, EventArgs e)
        {
            TakePhoto_LicensePlate();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            SaveData();

        }

        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            ClearData();
        }

        /// <summary>
        /// 刷新页面
        /// </summary>
        public void RefreshPage()
        {
            viewModel.OnPropertyChanged(nameof(viewModel.Car)); // 通知UI更新
            viewModel.OnPropertyChanged(nameof(viewModel.ModelNames)); // 通知UI更新
            viewModel.OnPropertyChanged(nameof(viewModel.IsLoading)); // 通知UI更新
        }

        public void IsLoading(bool IsLoading = false)
        {
            viewModel.IsLoading = IsLoading;
            RefreshPage();

        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LoadData()
        {
            var state = false;
            try
            {

                viewModel.ModelNames = await Api.GetObjModelViewList();
                if (viewModel.ModelNames != null)
                {
                    state = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadUsers异常: {ex.Message}");
            }
            return state;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveData()
        {
            IsLoading(true);
            var state = false;
            var Model = (ObjModel)ModelNamePicker.SelectedItem;
            viewModel.Car.ModelGUID = Model.ModelGUID;
            viewModel.Car.ModelName = Model.ModelName;
            var ca = viewModel.Car;

            var timeoutTask = Task.Delay(DalPrompt.Delayed);
            // 启动数据加载任务
            var loadUsersTask = Api.NewObjCarViewList(viewModel.Car);

            // 等待数据加载任务和超时任务中的任何一个完成
            var completedTask = await Task.WhenAny(loadUsersTask, timeoutTask);
            IsLoading();
            // 如果完成的任务是超时任务，则显示超时警告
            if (completedTask == timeoutTask)
            {
                await DisplayAlert(PageName, "加载数据超时!", DalPrompt.OK);
            }
            else
            {
                // 否则，检查数据加载任务是否成功
                if (await loadUsersTask)
                {
                    if (!IsAdd)
                    {
                        bool Display = await DisplayAlert(PageName, "保存成功,对否返回维修单页面？", DalPrompt.OK, DalPrompt.Cancel);
                        if (Display)
                        {
                            await Return();
                        }
                    }
                    else
                    {
                        await DisplayAlert(PageName, "保存成功！", DalPrompt.OK);
                    }
                    state = true;
                }
                else
                {
                    await DisplayAlert(PageName, "加载数据异常!", DalPrompt.OK);
                }
            }
            return state;
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void ClearData()
        {
            viewModel.Car = new ObjCar();

        }

        /// <summary>
        /// 
        /// </summary>
        public async Task Return()
        {
            string licensePlate = viewModel.Car.LicensePlate;
            // 调用回调并传递参数
            OnCarInfoSaved?.Invoke(licensePlate);
            await Navigation.PopAsync(); // 返回上一个页面

        }

        /// <summary>
        /// VIN 拍照
        /// </summary>
        /// <returns></returns>
        public async Task TakePhoto_VIN()
        {
            var photo = await TakePhotoAsync();
            if (photo != null)
            {
                var cropPage = new CropPage(photo, OnTextRecognized_VIN);
                await Navigation.PushAsync(cropPage);
            }
        }
        /// <summary>
        /// VIN 字符串处理
        /// </summary>
        /// <param name="recognizedText"></param>
        private void OnTextRecognized_VIN(string recognizedText)
        {
            // 处理识别后的文字
            // 定义要去除的字符，可以添加更多符号到中括号内
            //string pattern = @"[ ,，。！!]";
            string pattern = @"[^\w\s]";

            // 使用正则表达式替换这些字符
            string cleanedText = Regex.Replace(recognizedText, pattern, string.Empty);

            Console.WriteLine("原始字符串: " + recognizedText);
            Console.WriteLine("清理后的字符串: " + cleanedText);
            VINEntry.Text = cleanedText;
            //await FetchCarInfo(cleanedText);
        }

        /// <summary>
        /// 车牌 拍照
        /// </summary>
        /// <returns></returns>
        public async Task TakePhoto_LicensePlate()
        {
            var photo = await TakePhotoAsync();
            if (photo != null)
            {
                var cropPage = new CropPage(photo, OnTextRecognized_LicensePlate);
                await Navigation.PushAsync(cropPage);
            }
        }
        /// <summary>
        /// 车牌 字符串处理
        /// </summary>
        /// <param name="recognizedText"></param>
        private void OnTextRecognized_LicensePlate(string recognizedText)
        {
            // 处理识别后的文字
            // 定义要去除的字符，可以添加更多符号到中括号内
            //string pattern = @"[ ,，。！!]";
            string pattern = @"[^\w\s]";

            // 使用正则表达式替换这些字符
            string cleanedText = Regex.Replace(recognizedText, pattern, string.Empty);

            Console.WriteLine("原始字符串: " + recognizedText);
            Console.WriteLine("清理后的字符串: " + cleanedText);
            LicensePlateEntry.Text = cleanedText;
            //await FetchCarInfo(cleanedText);
        }

        private async Task<byte[]> TakePhotoAsync()
        {
            try
            {
                var result = await MediaPicker.Default.CapturePhotoAsync();
                if (result != null)
                {
                    var stream = await result.OpenReadAsync();
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("错误", "不支持相机功能", "确定");
            }
            catch (PermissionException)
            {
                await DisplayAlert("错误", "未授予相机权限", "确定");
            }
            catch (Exception ex)
            {
                await DisplayAlert("错误", $"发生错误: {ex.Message}", "确定");
            }
            return null;
        }

    }
}