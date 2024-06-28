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
        private string PageName = "文字识别";
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
            // 图片大小变化时的逻辑处理，如果需要
        }

        private async void OnRecognizeTextButtonClicked(object sender, EventArgs e)
        {
            IsLoading(true);
            // 将图像数据转换为 base64 编码格式
            string base64Photo = Convert.ToBase64String(_photo);

            // 调用文字识别接口
            string recognizedText = await RecognizeTextAsync(base64Photo);

            // 如果识别成功，显示文本框和提交按钮
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
            // 提交处理后的文字并返回上一页面
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
            // 启动数据加载任务
            var loadUsersTask = Api.PostOcr(base64Photo);

            // 等待数据加载任务和超时任务中的任何一个完成
            var completedTask = await Task.WhenAny(loadUsersTask, timeoutTask);
            IsLoading();
            // 如果完成的任务是超时任务，则显示超时警告
            if (completedTask == timeoutTask)
            {
                await DisplayAlert(PageName, "搜索数据超时!", DalPrompt.OK);
            }
            else
            {
                // 否则，检查数据加载任务是否成功
                var C = await loadUsersTask;
                if (C != null)
                {
                    IdentifyingText = C.data;
                    if (C.code != 100 & C.code != 101)
                    {
                        await DisplayAlert(PageName, "识别失败!", DalPrompt.OK);
                    }
                }
                
            }

            // 这里使用示例返回值
            return IdentifyingText;
        }

        // 示例返回类型，你需要根据实际接口返回的 JSON 格式进行定义
        private class OcrResult
        {
            [JsonProperty("text")]
            public string Text { get; set; }
        }
    }
}
