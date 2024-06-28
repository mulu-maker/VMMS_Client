using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;


namespace VMMS_Client
{

    public class Api
    {

        public static string UrlPublic(string s = null)
        {
            var Url = Preferences.Get("Url", string.Empty);
            var LoadDataGrid = Preferences.Get("LoadDataGrid", string.Empty);

            return string.Format("{0}/api/{1}/{2}", Url, s, LoadDataGrid);
        }

        /// <summary>
        /// 接口测试
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> GetTestViewList()
        {
            var Test = false;
            var _Url = string.Format("{0}{1}", UrlPublic("test"), "?type=1");
            Console.WriteLine(_Url);

            try
            {

                WebRequest request = WebRequest.Create(_Url);
                request.Method = "GET";

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = await reader.ReadToEndAsync();
                        TestData result = JsonConvert.DeserializeObject<TestData>(responseFromServer);
                        if (result.type == 1)
                        {
                            Test = true;
                            Console.WriteLine($"GET 请求正常: {responseFromServer}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET 请求发生异常: {ex.Message}");
            }
            return Test;
        }

        /// <summary>
        /// 返回车型列表
        /// </summary>
        /// <returns></returns>
        public static async Task<ObservableCollection<ObjModel>> GetObjModelViewList()
        {
            var GetObjModel = new ObservableCollection<ObjModel>();
            var _Url = string.Format("{0}?type={1}", UrlPublic("GetViewList"), "1");
            Console.WriteLine(_Url);

            try
            {
                GetObjModel = null;
                WebRequest request = WebRequest.Create(_Url);
                request.Method = "GET";

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = await reader.ReadToEndAsync();
                        ObjModelData result = JsonConvert.DeserializeObject<ObjModelData>(responseFromServer);
                        if (result.Message != null)
                        {
                            GetObjModel = result.Message;
                            Console.WriteLine($"GET 请求正常: {responseFromServer}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET 请求发生异常: {ex.Message}");
            }

            return GetObjModel;
        }

        /// <summary>
        /// 返回员工列表
        /// </summary>
        /// <returns></returns>
        public static async Task<ObservableCollection<ObjUser>> GetObjUserViewList()
        {
            var GetObjUser = new ObservableCollection<ObjUser>();
            var _Url = string.Format("{0}?type={1}", UrlPublic("GetViewList"), "2");
            Console.WriteLine(_Url);

            try
            {
                GetObjUser = null;
                WebRequest request = WebRequest.Create(_Url);
                request.Method = "GET";

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = await reader.ReadToEndAsync();
                        ObjUserData result = JsonConvert.DeserializeObject<ObjUserData>(responseFromServer);
                        if (result.Message != null)
                        {
                            GetObjUser = result.Message;
                            Console.WriteLine($"GET 请求正常: {responseFromServer}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET 请求发生异常: {ex.Message}");
            }

            return GetObjUser;
        }

        /// <summary>
        /// 返回维修项目列表
        /// </summary>
        /// <returns></returns>
        public static async Task<ObservableCollection<ObjItem>> GetObjItemViewList()
        {
            var GetObjItem = new ObservableCollection<ObjItem>();
            var _Url = string.Format("{0}?type={1}", UrlPublic("GetViewList"), "3");
            Console.WriteLine(_Url);

            try
            {
                GetObjItem = null;
                WebRequest request = WebRequest.Create(_Url);
                request.Method = "GET";

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = await reader.ReadToEndAsync();
                        ObjItemData result = JsonConvert.DeserializeObject<ObjItemData>(responseFromServer);
                        if (result.Message != null)
                        {
                            GetObjItem = result.Message;
                            Console.WriteLine($"GET 请求正常: {responseFromServer}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET 请求发生异常: {ex.Message}");
            }

            return GetObjItem;
        }
        /// <summary>
        /// 使用车牌查询车辆对象
        /// </summary>
        /// <returns></returns>
        public static async Task<ObjCarData> GetObjCarViewList(string LicensePlate)
        {
            ObjCarData GetObjCar = new ObjCarData();
            var _Url = string.Format("{0}?LicensePlate={1}", UrlPublic("licensePlate"), LicensePlate);
            Console.WriteLine(_Url);

            try
            {
                GetObjCar = null;
                WebRequest request = WebRequest.Create(_Url);
                request.Method = "GET";

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = await reader.ReadToEndAsync();
                        ObjCarData result = JsonConvert.DeserializeObject<ObjCarData>(responseFromServer);
                        if (result != null && result.Message != null)
                        {
                            GetObjCar = result;
                            Console.WriteLine($"GET 请求正常: {responseFromServer}");
                        }
                        else
                        {
                            Console.WriteLine("未找到相关车辆信息");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET 请求发生异常: {ex.Message}");
            }

            return GetObjCar;
        }

        /// <summary>
        /// 使用车牌查询车辆对象 精确
        /// </summary>
        /// <returns></returns>
        public static async Task<ObjCarData> GetObjCarCountViewList(string LicensePlate)
        {
            ObjCarData GetObjCar = new ObjCarData();
            var _Url = string.Format("{0}?LicensePlate={1}", UrlPublic("licensePlateCount"), LicensePlate);
            Console.WriteLine(_Url);

            try
            {
                GetObjCar = null;
                WebRequest request = WebRequest.Create(_Url);
                request.Method = "GET";

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = await reader.ReadToEndAsync();
                        ObjCarData result = JsonConvert.DeserializeObject<ObjCarData>(responseFromServer);
                        if (result.Message != null)
                        {
                            GetObjCar = result;
                            Console.WriteLine($"GET 请求正常: {responseFromServer}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET 请求发生异常: {ex.Message}");
            }

            return GetObjCar;
        }

        /// <summary>
        /// 新建车辆对象
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> NewObjCarViewList(ObjCar Car)
        {
            bool state = false;
            var _Url = string.Format("{0}", UrlPublic("InsertCar"));
            Console.WriteLine(_Url);
            try
            {
                WebRequest request = WebRequest.Create(_Url);
                request.Method = "POST";
                request.ContentType = "application/json";

                // 将请求数据写入请求流
                string jsonPayload = JsonConvert.SerializeObject(Car);
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonPayload);
                request.ContentLength = byteArray.Length;

                using (Stream dataStream = await request.GetRequestStreamAsync())
                {
                    await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
                }

                // 获取响应
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (StreamReader reader = new StreamReader(responseStream))
                            {
                                string responseText = await reader.ReadToEndAsync();
                                NewObjData result = JsonConvert.DeserializeObject<NewObjData>(responseText);
                                Console.WriteLine(responseText);
                                if (result != null & result.LicensePlate == Car.LicensePlate)
                                {
                                    state = true;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("POST 响应流为空");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST 请求发生异常: {ex.Message}");
            }
            return state;
        }
        /// <summary>
        /// 新建维修单对象
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> NewObjBillViewList(ObjBill Bill)
        {
            bool state = false;
            var _Url = string.Format("{0}", UrlPublic("Repair"));
            Console.WriteLine(_Url);
            try
            {
                WebRequest request = WebRequest.Create(_Url);
                request.Method = "POST";
                request.ContentType = "application/json";

                // 将请求数据写入请求流
                string jsonPayload = JsonConvert.SerializeObject(Bill);
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonPayload);
                request.ContentLength = byteArray.Length;

                using (Stream dataStream = await request.GetRequestStreamAsync())
                {
                    await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
                }

                // 获取响应
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (StreamReader reader = new StreamReader(responseStream))
                            {
                                string responseText = await reader.ReadToEndAsync();
                                NewObjData result = JsonConvert.DeserializeObject<NewObjData>(responseText);
                                Console.WriteLine(responseText);
                                if (result != null & result.LicensePlate == Bill.LicensePlate)
                                {
                                    state = true;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("POST 响应流为空");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST 请求发生异常: {ex.Message}");
            }
            return state;
        }

        /// <summary>
        /// 文字识别
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static async Task<OcrData> PostOcr(string base64)
        {
            OcrData state = null;
            var ocrReqData = new OcrReqData();
            ocrReqData.base64 = base64;

            var Url = Preferences.Get("OCR_Url", string.Empty);
            var _Url = string.Format("{0}/api/ocr", Url);
            Console.WriteLine(_Url);
            try
            {
                WebRequest request = WebRequest.Create(_Url);
                request.Method = "POST";
                request.ContentType = "application/json";

                // 将请求数据写入请求流
                string jsonPayload = JsonConvert.SerializeObject(ocrReqData);
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonPayload);
                request.ContentLength = byteArray.Length;

                using (Stream dataStream = await request.GetRequestStreamAsync())
                {
                    await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
                }

                // 获取响应
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (StreamReader reader = new StreamReader(responseStream))
                            {
                                string responseText = await reader.ReadToEndAsync();
                                OcrData result = JsonConvert.DeserializeObject<OcrData>(responseText);
                                Console.WriteLine(responseText);
                                if (result != null)
                                {
                                    state = result;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("POST 响应流为空");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST 请求发生异常: {ex.Message}");
                if (Url == string.Empty)
                {
                    state = new OcrData();
                    state.code = 0;
                    state.data = "未设置请求地址";
                    return state;
                }
                else
                {
                    state = new OcrData();
                    state.code = 0;
                    state.data = $"POST 请求发生异常: {ex.Message}";
                    return state;
                }
            }
            return state;
        }

    }

}
