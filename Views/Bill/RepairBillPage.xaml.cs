using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using System.Reflection.Metadata;
using static Android.App.ActionBar;
using System;
using System.Text.RegularExpressions;

namespace VMMS_Client
{
    public partial class RepairBillPage : ContentPage
    {
        private string PageName = "维修单";

        private RepairBillViewModel viewModel;

        public Image CapturedImage { get; set; }

        public RepairBillPage()
        {
            InitializeComponent();
            viewModel = new RepairBillViewModel();
            BindingContext = viewModel;
        }

        /// <summary>
        /// 页面加载时运行的代码
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // 页面加载时运行的代码

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
            // 创建一个3秒的超时任务
            IsLoading(true);
            var timeoutTask = Task.Delay(DalPrompt.Delayed);
            // 启动数据加载任务
            var loadUsersTask = LoadUsers();

            // 等待数据加载任务和超时任务中的任何一个完成
            var completedTask = await Task.WhenAny(loadUsersTask, timeoutTask);

            IsLoading(false);
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

        /// <summary>
        /// 拍照按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LicensePlateRecognitionButtonClicked(object sender, EventArgs e)
        {
            //

        }

        /// <summary>
        /// 输入框回车后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnLicensePlateCompleted(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                string licensePlate = entry.Text;
                await FetchCarInfo(licensePlate);
            }
        }

        /// <summary>
        /// 按确认按钮以后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            string licensePlate = LicensePlateEntry.Text;
            await FetchCarInfo(licensePlate);

        }

        /// <summary>
        /// 按取消按钮以后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClearButtonClicked(object sender, EventArgs e)
        {
            ClearData();
        }

        /// <summary>
        /// 按保存按钮以后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var User = (ObjUser)UserPicker.SelectedItem;
            var Item = (ObjItem)CarItemPicker.SelectedItem;

            SaveBillInfo(User, Item);
            DisplayAlert(PageName, "维修单保存成功!", DalPrompt.OK);
        }

        private async Task TakeAndCropPhotoAsync()
        {

        }



        /// <summary>
        /// 刷新页面
        /// </summary>
        public void RefreshPage()
        {
            viewModel.OnPropertyChanged(nameof(viewModel.Obj)); // 通知UI更新
            viewModel.OnPropertyChanged(nameof(viewModel.Users)); // 通知UI更新
            viewModel.OnPropertyChanged(nameof(viewModel.CarItems)); // 通知UI更新
            viewModel.OnPropertyChanged(nameof(viewModel.IsLoading)); // 通知UI更新

        }

        /// <summary>
        /// 初始加载数据
        /// </summary>
        public async Task<bool> LoadUsers()
        {
            var state = false;
            try
            {

                viewModel.Users = await Api.GetObjUserViewList();
                viewModel.CarItems = await Api.GetObjItemViewList();
                if (viewModel.Users != null & viewModel.CarItems != null)
                {
                    state = true;
                }

                RefreshPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadUsers异常: {ex.Message}");
            }
            return state;
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public void ClearData()
        {
            viewModel.Obj = new ObjBill();
            RefreshPage();

        }

        public void IsLoading(bool IsLoading = false)
        {
            viewModel.IsLoading = IsLoading;
            RefreshPage();

        }

        /// <summary>
        /// 获取汽车信息
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <returns></returns>
        public async Task<bool> FetchCarInfo(string licensePlate)
        {
            IsLoading(true);
            var state = false;
            try
            {
                var Car = await GetCarInfo(licensePlate);
                IsLoading(false);
                if (Car != null)
                {
                    await SyncCarInfo(Car);
                }
                else
                {
                    bool Display = await DisplayAlert(PageName, "未找到相关车辆信息,对否创建车辆信息？", DalPrompt.OK, DalPrompt.Cancel);
                    if (Display)
                    {
                        // 用户选择了 "是"
                        // 在这里执行相应的操作
                        var tcs = new TaskCompletionSource<string>();
                        CarBillPage child = new CarBillPage();
                        child.IsAdd = false;
                        child.OlicensePlate = licensePlate;
                        // 设置回调
                        child.OnCarInfoSaved = (licensePlate) =>
                        {
                            // 处理子页面传递回来的参数
                            Console.WriteLine($"Received licensePlate: {licensePlate}");
                            tcs.SetResult(licensePlate);
                            // 这里可以更新主页面的UI或执行其他逻辑
                        };
                        await Navigation.PushAsync(child);
                        string returnedlicensePlate = await tcs.Task;
                        if (returnedlicensePlate != null) 
                        {
                            await SyncCarInfo( await GetCarInfo(returnedlicensePlate));
                        }
                    }
                }
            } 
            catch (Exception ex)
            {
                IsLoading(false);
                Console.WriteLine($"FetchCarInfo异常: {ex.Message}");
            }
            return state;
        }

        /// <summary>
        /// 获取车辆信息
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <returns></returns>
        public async Task<ObjCarData> GetCarInfo(string licensePlate)
        {
            ObjCarData Car = null;
            var timeoutTask = Task.Delay((int)DalPrompt.Delayed);
            // 启动数据加载任务
            var loadUsersTask = Api.GetObjCarViewList(licensePlate);

            // 等待数据加载任务和超时任务中的任何一个完成
            var completedTask = await Task.WhenAny(loadUsersTask, timeoutTask);
            
            // 如果完成的任务是超时任务，则显示超时警告
            if (completedTask == timeoutTask)
            {
                await DisplayAlert(PageName, "搜索数据超时!", DalPrompt.OK);
            }
            else
            {
                // 否则，检查数据加载任务是否成功
                var C  = await loadUsersTask;
                if (C != null) 
                { 
                    Car = C;
                }
            }

            return Car;
        }

        /// <summary>
        /// 页面刷新同步
        /// </summary>
        public async Task SyncCarInfo(ObjCarData Car)
        {
            if (Car.Message.Count > 1)
            {
                var options = Car.Message.Select(car => car.LicensePlate).ToList();
                var selectedPlate = await DisplayActionSheet("选择车辆", DalPrompt.Cancel, null, options.ToArray());

                if (!string.IsNullOrEmpty(selectedPlate))
                {

                    var O_Car = Car.Message.FirstOrDefault(car => car.LicensePlate == selectedPlate);
                    if (await CheckCarBillDate(O_Car.LicensePlate) == false)
                    {
                        viewModel.Car = O_Car;
                    }
                }
            }
            else if (Car.Message.Count == 1)
            {
                var O_Car = Car.Message[0];
                if (await CheckCarBillDate(O_Car.LicensePlate) == false)
                {
                    viewModel.Car = O_Car;
                }
            }
            viewModel.Obj.LicensePlate = viewModel.Car.LicensePlate;
            viewModel.Obj.VIN = viewModel.Car.VIN;
            viewModel.Obj.CustomerName = viewModel.Car.CustomerName;
            viewModel.Obj.SendName = viewModel.Car.CustomerName;
            viewModel.Obj.MobilePhone = viewModel.Car.MobilePhone;
            RefreshPage();
        }
        /// <summary>
        /// 搜索最近30日维修单
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <returns></returns>
        public async Task<bool> CheckCarBillDate(string licensePlate)
        {
            bool result = false;
            IsLoading(true);
            var timeoutTask2 = Task.Delay(DalPrompt.Delayed);
            var CountTask = Api.GetObjCarCountViewList(licensePlate);

            // 等待数据加载任务和超时任务中的任何一个完成
            var completedTask2 = await Task.WhenAny(CountTask, timeoutTask2);
            IsLoading(false);
            // 如果完成的任务是超时任务，则显示超时警告
            if (completedTask2 == timeoutTask2)
            {
                await DisplayAlert(PageName, "搜索数据超时!", DalPrompt.OK);
            }
            else
            {
                var Car = await CountTask;
                if (Car.Count > 0)
                {
                    result = true;
                    bool Display = await DisplayAlert(PageName, "30日内此车辆已开具过维修单，是否继续？", DalPrompt.OK, DalPrompt.Cancel);

                    if (Display)
                    {
                        // 用户选择了 "是"
                        // 在这里执行相应的操作
                        result = false;
                    }

                }

            }
            return result;
        }

        public void SaveBillInfo(ObjUser User, ObjItem Item)
        {
            try
            {

                viewModel.Obj.UserGUID = User.UserGUID;
                viewModel.Obj.ItemGUID = Item.ItemGUID;
                RefreshPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FetchCarInfo异常: {ex.Message}");
            }
        }

        private async void OnTakePhotoButtonClicked(object sender, EventArgs e)
        {
            var photo = await TakePhotoAsync();
            if (photo != null)
            {
                var cropPage = new CropPage(photo, OnTextRecognized);
                await Navigation.PushAsync(cropPage);
            }
        }

        private async void OnTextRecognized(string recognizedText)
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
            await FetchCarInfo(cleanedText);
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
