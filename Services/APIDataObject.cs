using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMMS_Client
{

    /// <summary>
    /// 网络连接测试对象
    /// </summary>
    public class TestData
    {
        public int type { get; set; }
        public string Message { get; set; } = string.Empty;

    }
    /// <summary>
    /// 网络连接车型列表
    /// </summary>
    public class ObjModelData
    {
        public int type { get; set; }
        public ObservableCollection<ObjModel> Message { get; set; }

    }
    /// <summary>
    /// 网络连接员工列表
    /// </summary>
    public class ObjUserData
    {
        public int type { get; set; }
        public ObservableCollection<ObjUser> Message { get; set; }

    }
    /// <summary>
    /// 网络连接维修项目列表
    /// </summary>
    public class ObjItemData
    {
        public int type { get; set; }
        public ObservableCollection<ObjItem> Message { get; set; }

    }
    /// <summary>
    /// 网络连接车牌查询车辆对象
    /// </summary>
    public class ObjCarData
    {
        public string LicensePlate { get; set; } = string.Empty;
        public ObservableCollection<ObjCar> Message { get; set; } = new ObservableCollection<ObjCar>();
        public int Count { get; set; }

    }
    /// <summary>
    /// 网络连接车牌查询车辆对象
    /// </summary>
    public class ObjCarCountData
    {
        public string LicensePlate { get; set; } = string.Empty;
        public ObjCar Message { get; set; }
        public int Count { get; set; }

    }

    /// <summary>
    /// 网络连接新建对象返回状态
    /// </summary>
    public class  NewObjData
    { 
        public string LicensePlate { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

    }

    /// <summary>
    /// 网络请求OCR对象
    /// </summary>
    public class OcrReqData
    {
        public string base64 { get; set; } = string.Empty;
        public OcrOptionsData options { get; set; } = new OcrOptionsData();

    }
    public class OcrOptionsData
    {
        // 通用参数
        [JsonProperty("tbpu.parser")]
        public string TbpuParser { get; set; } = "multi_para";

        [JsonProperty("data.format")]
        public string DataFormat { get; set; } = "text";

        //// 引擎参数
        //[JsonProperty("ocr.angle")]
        //public bool OcrAngle { get; set; } = false;

        //[JsonProperty("ocr.language")]
        //public string OcrLanguage { get; set; } = "简体中文";

        //[JsonProperty("ocr.maxSideLen")]
        //public int OcrMaxSideLen { get; set; } = 1024;
    }



    /// <summary>
    /// 网络连接OCR对象
    /// </summary>
    public class OcrData
    {
        public int code { get; set; } //任务状态。100为成功，101为无文本，其余为失败
        public string data { get; set; } = string.Empty; //识别结果

    }

}
