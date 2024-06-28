
using System;
using System.Text.RegularExpressions;
using static Android.App.ActionBar;

namespace VMMS_Client 
{

    public partial class CarBillPage : ContentPage
    {
        private string PageName = "��������";
        public bool IsAdd = true;
        public string OlicensePlate  = string.Empty;

        // ����ص�
        public Action<string> OnCarInfoSaved { get; set; }

        private CarBillViewModel viewModel;
        public CarBillPage()
        {
            InitializeComponent();
            viewModel = new CarBillViewModel();
            BindingContext = viewModel;
        }
        /// <summary>
        /// ҳ�����ʱ���еĴ���
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // ҳ�����ʱ���еĴ���
            if (!IsAdd)
            {
                viewModel.Car.LicensePlate = OlicensePlate;
            }

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
            IsLoading(true);
            var timeoutTask = Task.Delay(DalPrompt.Delayed);
            // �������ݼ�������
            var loadUsersTask = LoadData();

            // �ȴ����ݼ�������ͳ�ʱ�����е��κ�һ�����
            var completedTask = await Task.WhenAny(loadUsersTask, timeoutTask);
            IsLoading();
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

        private void VINRecognitionButtonClicked(object sender, EventArgs e)
        {
            TakePhoto_VIN();
        }

        private void LicensePlateRecognitionButtonClicked(object sender, EventArgs e)
        {
            TakePhoto_LicensePlate();
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            SaveData();

        }

        /// <summary>
        /// ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            ClearData();
        }

        /// <summary>
        /// ˢ��ҳ��
        /// </summary>
        public void RefreshPage()
        {
            viewModel.OnPropertyChanged(nameof(viewModel.Car)); // ֪ͨUI����
            viewModel.OnPropertyChanged(nameof(viewModel.ModelNames)); // ֪ͨUI����
            viewModel.OnPropertyChanged(nameof(viewModel.IsLoading)); // ֪ͨUI����
        }

        public void IsLoading(bool IsLoading = false)
        {
            viewModel.IsLoading = IsLoading;
            RefreshPage();

        }

        /// <summary>
        /// ��������
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
                Console.WriteLine($"LoadUsers�쳣: {ex.Message}");
            }
            return state;
        }

        /// <summary>
        /// ��������
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
            // �������ݼ�������
            var loadUsersTask = Api.NewObjCarViewList(viewModel.Car);

            // �ȴ����ݼ�������ͳ�ʱ�����е��κ�һ�����
            var completedTask = await Task.WhenAny(loadUsersTask, timeoutTask);
            IsLoading();
            // �����ɵ������ǳ�ʱ��������ʾ��ʱ����
            if (completedTask == timeoutTask)
            {
                await DisplayAlert(PageName, "�������ݳ�ʱ!", DalPrompt.OK);
            }
            else
            {
                // ���򣬼�����ݼ��������Ƿ�ɹ�
                if (await loadUsersTask)
                {
                    if (!IsAdd)
                    {
                        bool Display = await DisplayAlert(PageName, "����ɹ�,�Է񷵻�ά�޵�ҳ�棿", DalPrompt.OK, DalPrompt.Cancel);
                        if (Display)
                        {
                            await Return();
                        }
                    }
                    else
                    {
                        await DisplayAlert(PageName, "����ɹ���", DalPrompt.OK);
                    }
                    state = true;
                }
                else
                {
                    await DisplayAlert(PageName, "���������쳣!", DalPrompt.OK);
                }
            }
            return state;
        }

        /// <summary>
        /// �������
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
            // ���ûص������ݲ���
            OnCarInfoSaved?.Invoke(licensePlate);
            await Navigation.PopAsync(); // ������һ��ҳ��

        }

        /// <summary>
        /// VIN ����
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
        /// VIN �ַ�������
        /// </summary>
        /// <param name="recognizedText"></param>
        private void OnTextRecognized_VIN(string recognizedText)
        {
            // ����ʶ��������
            // ����Ҫȥ�����ַ���������Ӹ�����ŵ���������
            //string pattern = @"[ ,������!]";
            string pattern = @"[^\w\s]";

            // ʹ��������ʽ�滻��Щ�ַ�
            string cleanedText = Regex.Replace(recognizedText, pattern, string.Empty);

            Console.WriteLine("ԭʼ�ַ���: " + recognizedText);
            Console.WriteLine("�������ַ���: " + cleanedText);
            VINEntry.Text = cleanedText;
            //await FetchCarInfo(cleanedText);
        }

        /// <summary>
        /// ���� ����
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
        /// ���� �ַ�������
        /// </summary>
        /// <param name="recognizedText"></param>
        private void OnTextRecognized_LicensePlate(string recognizedText)
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