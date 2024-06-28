using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Net.Http;
using Newtonsoft.Json;
using AndroidX.Lifecycle;
using static System.Net.Mime.MediaTypeNames;

namespace VMMS_Client
{
    public partial class CropPage : ContentPage
    {
        private string PageName = "����ʶ��";
        private readonly byte[] _photo;
        private readonly Action<string> _onTextRecognized;

        public CropPage(byte[] photo, Action<string> onTextRecognized)
        {
            InitializeComponent();
            _photo = photo ?? throw new ArgumentNullException(nameof(photo));
            _onTextRecognized = onTextRecognized ?? throw new ArgumentNullException(nameof(onTextRecognized));
            PhotoToCrop.Source = ImageSource.FromStream(() => new MemoryStream(photo));
        }

        private void OnPhotoSizeChanged(object sender, EventArgs e)
        {
            // ͼƬ��С�仯ʱ���߼����������Ҫ
        }

        private async void OnRecognizeTextButtonClicked(object sender, EventArgs e)
        {
            IsLoading(true);
            // ��ͼ������ת��Ϊ base64 �����ʽ
            string base64Photo = Convert.ToBase64String(_photo);

            // ��������ʶ��ӿ�
            string recognizedText = await RecognizeTextAsync(base64Photo);

            // ���ʶ��ɹ�����ʾ�ı�����ύ��ť
            if (!string.IsNullOrWhiteSpace(recognizedText))
            {
                RecognizedTextEditor.Text = recognizedText;
                RecognizedTextEditor.IsVisible = true;
                SubmitButton.IsVisible = true;
                RecognizeTextButton.IsVisible = true;
            }
            IsLoading();
        }

        private void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            // �ύ���������ֲ�������һҳ��
            _onTextRecognized?.Invoke(RecognizedTextEditor.Text);
            Navigation.PopAsync();
        }

        public void IsLoading(bool IsLoading = false)
        {
            IsLoadingLoadingPopup.IsVisible = IsLoading;

        }

        private async Task<string> RecognizeTextAsync(string base64Photo)
        {
            string IdentifyingText = string.Empty;
            var timeoutTask = Task.Delay((int)DalPrompt.Delayed);
            // �������ݼ�������
            var loadUsersTask = Api.PostOcr(base64Photo);

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
                var C = await loadUsersTask;
                if (C != null)
                {
                    IdentifyingText = C.data;
                    if (C.code != 100 & C.code != 101)
                    {
                        await DisplayAlert(PageName, "ʶ��ʧ��!", DalPrompt.OK);
                    }
                }
                
            }

            // ����ʹ��ʾ������ֵ
            return IdentifyingText;
        }

        // ʾ���������ͣ�����Ҫ����ʵ�ʽӿڷ��ص� JSON ��ʽ���ж���
        private class OcrResult
        {
            [JsonProperty("text")]
            public string Text { get; set; }
        }
    }
}
