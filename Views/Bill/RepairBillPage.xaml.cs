using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using System.Reflection.Metadata;
using static Android.App.ActionBar;
using System;
using System.Text.RegularExpressions;

namespace VMMS_Client
{
    public partial class RepairBillPage : ContentPage
    {
        private string PageName = "ά�޵�";

        private RepairBillViewModel viewModel;

        public Image CapturedImage { get; set; }

        public RepairBillPage()
        {
            InitializeComponent();
            viewModel = new RepairBillViewModel();
            BindingContext = viewModel;
        }

        /// <summary>
        /// ҳ�����ʱ���еĴ���
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // ҳ�����ʱ���еĴ���

            LoadDataAsync();
        }

        /// <summary>
        /// �첽�������ݵ��߼�
        /// </summary>
        /// <returns></returns>
        private async Task LoadDataAsync()
        {
            // �첽�������ݵ��߼�
            // ����ӷ�������ȡ���ݡ���ʼ����ͼģ�͵�
            // ����һ��3��ĳ�ʱ����
            IsLoading(true);
            var timeoutTask = Task.Delay(DalPrompt.Delayed);
            // �������ݼ�������
            var loadUsersTask = LoadUsers();

            // �ȴ����ݼ�������ͳ�ʱ�����е��κ�һ�����
            var completedTask = await Task.WhenAny(loadUsersTask, timeoutTask);

            IsLoading(false);
            // �����ɵ������ǳ�ʱ��������ʾ��ʱ����
            if (completedTask == timeoutTask)
            {
                await DisplayAlert(PageName, "�������ݳ�ʱ!", DalPrompt.OK);
            }
            else
            {
                // ���򣬼�����ݼ��������Ƿ�ɹ�
                if (!await loadUsersTask)
                {
                    await DisplayAlert(PageName, "���������쳣!", DalPrompt.OK);
                }
            }

        }

        /// <summary>
        /// ���հ�ť
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LicensePlateRecognitionButtonClicked(object sender, EventArgs e)
        {
            //

        }

        /// <summary>
        /// �����س���
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
        /// ��ȷ�ϰ�ť�Ժ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            string licensePlate = LicensePlateEntry.Text;
            await FetchCarInfo(licensePlate);

        }

        /// <summary>
        /// ��ȡ����ť�Ժ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClearButtonClicked(object sender, EventArgs e)
        {
            ClearData();
        }

        /// <summary>
        /// �����水ť�Ժ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var User = (ObjUser)UserPicker.SelectedItem;
            var Item = (ObjItem)CarItemPicker.SelectedItem;

            SaveBillInfo(User, Item);
            DisplayAlert(PageName, "ά�޵�����ɹ�!", DalPrompt.OK);
        }

        private async Task TakeAndCropPhotoAsync()
        {

        }



        /// <summary>
        /// ˢ��ҳ��
        /// </summary>
        public void RefreshPage()
        {
            viewModel.OnPropertyChanged(nameof(viewModel.Obj)); // ֪ͨUI����
            viewModel.OnPropertyChanged(nameof(viewModel.Users)); // ֪ͨUI����
            viewModel.OnPropertyChanged(nameof(viewModel.CarItems)); // ֪ͨUI����
            viewModel.OnPropertyChanged(nameof(viewModel.IsLoading)); // ֪ͨUI����

        }

        /// <summary>
        /// ��ʼ��������
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
                Console.WriteLine($"LoadUsers�쳣: {ex.Message}");
            }
            return state;
        }

        /// <summary>
        /// �������
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
        /// ��ȡ������Ϣ
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
                    bool Display = await DisplayAlert(PageName, "δ�ҵ���س�����Ϣ,�Է񴴽�������Ϣ��", DalPrompt.OK, DalPrompt.Cancel);
                    if (Display)
                    {
                        // �û�ѡ���� "��"
                        // ������ִ����Ӧ�Ĳ���
                        var tcs = new TaskCompletionSource<string>();
                        CarBillPage child = new CarBillPage();
                        child.IsAdd = false;
                        child.OlicensePlate = licensePlate;
                        // ���ûص�
                        child.OnCarInfoSaved = (licensePlate) =>
                        {
                            // ������ҳ�洫�ݻ����Ĳ���
                            Console.WriteLine($"Received licensePlate: {licensePlate}");
                            tcs.SetResult(licensePlate);
                            // ������Ը�����ҳ���UI��ִ�������߼�
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
                Console.WriteLine($"FetchCarInfo�쳣: {ex.Message}");
            }
            return state;
        }

        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <returns></returns>
        public async Task<ObjCarData> GetCarInfo(string licensePlate)
        {
            ObjCarData Car = null;
            var timeoutTask = Task.Delay((int)DalPrompt.Delayed);
            // �������ݼ�������
            var loadUsersTask = Api.GetObjCarViewList(licensePlate);

            // �ȴ����ݼ�������ͳ�ʱ�����е��κ�һ�����
            var completedTask = await Task.WhenAny(loadUsersTask, timeoutTask);
            
            // �����ɵ������ǳ�ʱ��������ʾ��ʱ����
            if (completedTask == timeoutTask)
            {
                await DisplayAlert(PageName, "�������ݳ�ʱ!", DalPrompt.OK);
            }
            else
            {
                // ���򣬼�����ݼ��������Ƿ�ɹ�
                var C  = await loadUsersTask;
                if (C != null) 
                { 
                    Car = C;
                }
            }

            return Car;
        }

        /// <summary>
        /// ҳ��ˢ��ͬ��
        /// </summary>
        public async Task SyncCarInfo(ObjCarData Car)
        {
            if (Car.Message.Count > 1)
            {
                var options = Car.Message.Select(car => car.LicensePlate).ToList();
                var selectedPlate = await DisplayActionSheet("ѡ����", DalPrompt.Cancel, null, options.ToArray());

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
        /// �������30��ά�޵�
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <returns></returns>
        public async Task<bool> CheckCarBillDate(string licensePlate)
        {
            bool result = false;
            IsLoading(true);
            var timeoutTask2 = Task.Delay(DalPrompt.Delayed);
            var CountTask = Api.GetObjCarCountViewList(licensePlate);

            // �ȴ����ݼ�������ͳ�ʱ�����е��κ�һ�����
            var completedTask2 = await Task.WhenAny(CountTask, timeoutTask2);
            IsLoading(false);
            // �����ɵ������ǳ�ʱ��������ʾ��ʱ����
            if (completedTask2 == timeoutTask2)
            {
                await DisplayAlert(PageName, "�������ݳ�ʱ!", DalPrompt.OK);
            }
            else
            {
                var Car = await CountTask;
                if (Car.Count > 0)
                {
                    result = true;
                    bool Display = await DisplayAlert(PageName, "30���ڴ˳����ѿ��߹�ά�޵����Ƿ������", DalPrompt.OK, DalPrompt.Cancel);

                    if (Display)
                    {
                        // �û�ѡ���� "��"
                        // ������ִ����Ӧ�Ĳ���
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
                Console.WriteLine($"FetchCarInfo�쳣: {ex.Message}");
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
            // ����ʶ��������
            // ����Ҫȥ�����ַ���������Ӹ�����ŵ���������
            //string pattern = @"[ ,������!]";
            string pattern = @"[^\w\s]";

            // ʹ��������ʽ�滻��Щ�ַ�
            string cleanedText = Regex.Replace(recognizedText, pattern, string.Empty);

            Console.WriteLine("ԭʼ�ַ���: " + recognizedText);
            Console.WriteLine("�������ַ���: " + cleanedText);
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
                await DisplayAlert("����", "��֧���������", "ȷ��");
            }
            catch (PermissionException)
            {
                await DisplayAlert("����", "δ�������Ȩ��", "ȷ��");
            }
            catch (Exception ex)
            {
                await DisplayAlert("����", $"��������: {ex.Message}", "ȷ��");
            }
            return null;
        }


    }
}
