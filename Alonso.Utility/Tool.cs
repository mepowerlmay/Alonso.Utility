using System;
using System.Collections.Generic;

using System.Text;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Data;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using System.Linq;

namespace Alonso.Utility
{
    /// <summary>Alonso公用类库
    /// 
    /// </summary>
    public static partial class Tool
    {
	    
	/// <summary>
        /// 取所有資料夾
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="GroupFiles"></param>
        public static void GetAllDirectories(string rootPath, List<string> GroupFold)
        {
            string[] subPaths = System.IO.Directory.GetDirectories(rootPath);//得到所有子目錄
            foreach (string path in subPaths)
            {
                GroupFold.Add(path);
                GetAllDirectories(path, GroupFold);//對每一個字目錄做與根目錄相同的操作:即找到子目錄並將當前目錄的檔名存入List
            }
        }
        /// <summary>
        /// 取所有資料夾內的檔案
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="GroupFiles"></param>
      public static  void GetAllDirectFiles(string rootPath, List<string> GroupFiles)
        {
            string[] subPaths = System.IO.Directory.GetDirectories(rootPath);//得到所有子目錄
            foreach (string path in subPaths)
            {
                GetAllDirectFiles(path, GroupFiles);//對每一個字目錄做與根目錄相同的操作:即找到子目錄並將當前目錄的檔名存入List
            }
            string[] files = System.IO.Directory.GetFiles(rootPath);
            foreach (string file in files)
            {
                GroupFiles.Add(file);//將當前目錄中的所有檔案全名存入檔案List
            }
        }
	    
        /// <summary>
        /// 轉碼http 將網址做轉換此版OK絕對順暢 encode轉http的編碼 http://tw.google?A=檔案名稱做轉換
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UrlEncode(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            string stringToEscape = input;
            string input2 = string.Empty;
            int num = input.IndexOf('?');
            if (num >= 0)
            {
                input2 = input.Substring(num).TrimStart('?');
                stringToEscape = input.Remove(num);
            }
            List<KeyValuePair<string, string>> list = SplitToKeyValuePairs(input2, '&', '=');
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> item in list)
            {
                stringBuilder.AppendFormat("{0}={1}&", Uri.EscapeDataString(item.Key), Uri.EscapeDataString(item.Value));
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            string arg = Uri.EscapeUriString(stringToEscape);
            return $"{arg}?{stringBuilder.ToString()}";
        }

        /// <summary>
        /// 解碼 將網址做轉換此版OK絕對順暢 encode轉http的解碼 http://tw.google?A=檔案名稱做轉換
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UrlDecode(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            return Uri.UnescapeDataString(input.Replace('+', ' '));
        }


        public static List<KeyValuePair<string, string>> SplitToKeyValuePairs(string input, char itemSeparator, char keyValueSeparator)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            string[] array = input.Split(new char[1]
            {
            itemSeparator
            }, StringSplitOptions.RemoveEmptyEntries);
            string[] array2 = array;
            foreach (string text in array2)
            {
                string[] array3 = text.Split(keyValueSeparator);
                if (array3.Length >= 2)
                {
                    KeyValuePair<string, string> item = new KeyValuePair<string, string>(array3[0], array3[1]);
                    list.Add(item);
                }
                else if (array3.Length == 1)
                {
                    KeyValuePair<string, string> item = new KeyValuePair<string, string>(array3[0], string.Empty);
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// Copy文件夹
        /// </summary>
        /// <param name="sPath">源文件夹路径</param>
        /// <param name="dPath">目的文件夹路径</param>
        /// <returns>完成状态：success-完成；其他-报错</returns>
        public static string CopyFolder(string sPath, string dPath)
        {
            string flag = "success";
            try
            {
                // 创建目的文件夹
                if (!Directory.Exists(dPath))
                {
                    Directory.CreateDirectory(dPath);
                }

                // 拷贝文件
                DirectoryInfo sDir = new DirectoryInfo(sPath);
                FileInfo[] fileArray = sDir.GetFiles();
                foreach (FileInfo file in fileArray)
                {
                    file.CopyTo(dPath + "\\" + file.Name, true);
                }

                // 循环子文件夹
                DirectoryInfo dDir = new DirectoryInfo(dPath);
                DirectoryInfo[] subDirArray = sDir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirArray)
                {
                    CopyFolder(subDir.FullName, dPath + "//" + subDir.Name);
                }
            }
            catch (Exception ex)
            {
                flag = ex.ToString();
            }
            return flag;
        }


        /// <summary>
        /// 多个字节数组合并
        /// </summary>
        /// <param name="arrays"></param>
        /// <returns></returns>
        public static byte[] CombineByte(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        /// <summary>
        /// 绑定ddl控件，数据源是一个IEnumerator，
        /// </summary>
        /// <param name="ddlxz">下拉框控件</param>
        /// <param name="list">数据源</param>
        /// <param name="textfield">text字段</param>
        /// <param name="valuefield">value字段</param>
        public static void BindDDL<T>(DropDownList ddl, IEnumerable<T> list, string textfield, string valuefield)
        { 
            ddl.DataTextField = textfield;
            ddl.DataValueField = valuefield;
            ddl.DataSource = list;
            ddl.DataBind();
        }

        /// <summary>  
        /// 截取字节数组  
        /// </summary>  
        /// <param name="srcBytes">要截取的字节数组</param>  
        /// <param name="startIndex">开始截取位置的索引</param>  
        /// <param name="length">要截取的字节长度</param>  
        /// <returns>截取后的字节数组</returns>  
        public static byte[] SubByte(byte[] srcBytes, int startIndex, int length)
        {
            System.IO.MemoryStream bufferStream = new System.IO.MemoryStream();
            byte[] returnByte = new byte[] { };
            if (srcBytes == null) { return returnByte; }
            if (startIndex < 0) { startIndex = 0; }
            if (startIndex < srcBytes.Length)
            {
                if (length < 1 || length > srcBytes.Length - startIndex) { length = srcBytes.Length - startIndex; }
                bufferStream.Write(srcBytes, startIndex, length);
                returnByte = bufferStream.ToArray();
                bufferStream.SetLength(0);
                bufferStream.Position = 0;
            }
            bufferStream.Close();
            bufferStream.Dispose();
            return returnByte;
        }


        /// <summary>
        /// 从类似于URL参数的字符串中取值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="param">URL参数，类似于：username=niunan&password=123456(其中&符号有可能被\u0026代替)</param>
        /// <returns></returns>
        public static string GetValueByKeyFromUrlParam(string key, string param)
        {
            param = param.Replace("\u0026", "&");
            string[] ss = param.Split('&');
            string val = "";
            foreach (var item in ss)
            {
                if (item.Contains(key + "="))
                {
                    val = item.Replace(key + "=", "");
                }
            }
            return val;
        }


        /// <summary>把一串字符弄成byte[]字节数组返回
        /// 传入的示例：68 31 00 31 00 68 c9 76 45 74 bf 0c 02 70 00 00 01 00 36 16
        /// 必须是十六进制，中间用一个空格隔开
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ToBytesByString(string input)
        {
            string[] ss = input.Split(' ');
            byte[] bytes = new byte[ss.Length];
            for (int i = 0; i < ss.Length; i++)
            {
                bytes[i] = byte.Parse(Convert.ToInt64("0x" + ss[i], 16).ToString());
            }
            return bytes;
        }

        /// <summary>把字节数组转为字符串
        /// 输出如：68 31 00 31 00 68 c9 76 45 74 bf 0c 02 70 00 00 01 00 36 16
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToStringByBytes(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", " ");
        }

        /// <summary>根据传入的星期几的整型取最近的日期
        /// 如传入0 ，返回最近星期日的日期
        /// </summary>
        /// <param name="xingqi">1，2，3，4，5，6，0</param>
        /// <returns></returns>
        public static DateTime GetNearDateByXingQiInt(int xingqi)
        {
            int now = (int)DateTime.Now.DayOfWeek;
            int tmp = Math.Abs(now - (xingqi + 7));
            return DateTime.Now.AddDays(tmp);
        }

        /// <summary>根据时间显示几分钟前
        ///  if(今年){
        // if(今天){
        //   if(大于等于1小时){
        //       显示：‘几’小时前
        //    }else if(大于等于1分钟){
        //       显示： ‘几’分钟前
        //    }else{
        //       //小于1分钟
        //       显示： 刚刚
        //    }
        //  }else if(昨天){
        //   显示：昨天 HH:mm
        //  }else{
        //  //昨天之前
        //  显示: MM-dd HH:mm
        // }
        //}else{
        // 显示：yyyy-MM-dd HH:mm
        //}
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DateFormatToString(DateTime dt)
        {
            DateTime now = DateTime.Now;
            if (now.Year == dt.Year)
            {
                if (now.ToString("MM-dd") == dt.ToString("MM-dd"))
                {
                    //今天
                    TimeSpan span = (now - dt).Duration();
                    if (span.TotalHours >= 1)
                    {
                        return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                    }
                    else if (span.TotalMinutes > 1)
                    {
                        return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                    }
                    else
                    {
                        return "刚刚";
                    }
                }
                else if (now.AddDays(-1).ToString("MM-dd") == dt.ToString("MM-dd")) {
                    //昨天
                    return "昨天 " + dt.ToString("HH:mm");
                }
                else
                {
                    //昨天之前
                    return dt.ToString("MM-dd HH:mm");
                }
            }
            else
            {
                return dt.ToString("yyyy-MM-dd HH:mm");
            }
            //TimeSpan span = (DateTime.Now - dt).Duration();
            //if (span.TotalDays > 60)
            //{
            //    return dt.ToString("yyyy-MM-dd");
            //}
            //else if (span.TotalDays > 30)
            //{
            //    return "1个月前";
            //}
            //else if (span.TotalDays > 14)
            //{
            //    return "2周前";
            //}
            //else if (span.TotalDays > 7)
            //{
            //    return "1周前";
            //}
            //else if (span.TotalDays > 1)
            //{
            //    return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
            //}
            //else if (span.TotalHours > 1)
            //{
            //    return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
            //}
            //else if (span.TotalMinutes > 1)
            //{
            //    return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
            //}
            //else if (span.TotalSeconds >= 1)
            //{
            //    return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
            //}
            //else
            //{
            //    return "1秒前";
            //}
        }

        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式（10位）</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }


        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  （10位）
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>  
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  （13位）
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>  
        public static long ConvertDateTimeInt_13(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }

        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式（10位）</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime GetTime_13(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }



        /// <summary>
        /// 是否是移动浏览器
        /// daniel.xiaofei@gmail.com
        /// 2013-05-13
        /// </summary>
        /// <returns></returns>
        public static bool IsMobileBrowser()
        {

            //GETS THE CURRENT USER CONTEXT
            HttpContext context = HttpContext.Current;

            //FIRST TRY BUILT IN ASP.NT CHECK
            if (context.Request.Browser.IsMobileDevice)
            { 
                return true;
            }
            //THEN TRY CHECKING FOR THE HTTP_X_WAP_PROFILE HEADER
            if (context.Request.ServerVariables["HTTP_X_WAP_PROFILE"] != null)
            { 
                return true;
            }
            //THEN TRY CHECKING THAT HTTP_ACCEPT EXISTS AND CONTAINS WAP
            if (context.Request.ServerVariables["HTTP_ACCEPT"] != null &&
                context.Request.ServerVariables["HTTP_ACCEPT"].ToLower().Contains("wap"))
            { 
                return true;
            }
            //AND FINALLY CHECK THE HTTP_USER_AGENT 
            //HEADER VARIABLE FOR ANY ONE OF THE FOLLOWING
            if (context.Request.ServerVariables["HTTP_USER_AGENT"] != null)
            {

                //根据HTTP_USER_AGENT，用正则表达式来判断是否是手机在访问
                string u = context.Request.ServerVariables["HTTP_USER_AGENT"];
                System.Text.RegularExpressions.Regex b = new System.Text.RegularExpressions.Regex(@"android|avantgo|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\\/|plucker|pocket|psp|symbian|treo|up\\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
                System.Text.RegularExpressions.Regex v = new System.Text.RegularExpressions.Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\\-(n|u)|c55\\/|capi|ccwa|cdm\\-|cell|chtm|cldc|cmd\\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\\-s|devi|dica|dmob|do(c|p)o|ds(12|\\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\\-|_)|g1 u|g560|gene|gf\\-5|g\\-mo|go(\\.w|od)|gr(ad|un)|haie|hcit|hd\\-(m|p|t)|hei\\-|hi(pt|ta)|hp( i|ip)|hs\\-c|ht(c(\\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\\-(20|go|ma)|i230|iac( |\\-|\\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\\/)|klon|kpt |kwc\\-|kyo(c|k)|le(no|xi)|lg( g|\\/(k|l|u)|50|54|e\\-|e\\/|\\-[a-w])|libw|lynx|m1\\-w|m3ga|m50\\/|ma(te|ui|xo)|mc(01|21|ca)|m\\-cr|me(di|rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\\-2|po(ck|rt|se)|prox|psio|pt\\-g|qa\\-a|qc(07|12|21|32|60|\\-[2-7]|i\\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\\-|oo|p\\-)|sdk\\/|se(c(\\-|0|1)|47|mc|nd|ri)|sgh\\-|shar|sie(\\-|m)|sk\\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\\-|v\\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\\-|tdg\\-|tel(i|m)|tim\\-|t\\-mo|to(pl|sh)|ts(70|m\\-|m3|m5)|tx\\-9|up(\\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|xda(\\-|2|g)|yas\\-|your|zeto|zte\\-", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
                if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
                { 
                    return true;
                }

                //Create a list of all mobile types
                string[] mobiles =
                    new[]
        {
            "midp", "j2me", "avant", "docomo",
            "novarra", "palmos", "palmsource",
            "240x320", "opwv", "chtml",
            "pda", "windows ce", "mmp/",
            "blackberry", "mib/", "symbian",
            "wireless", "nokia", "hand", "mobi",
            "phone", "cdm", "up.b", "audio",
            "SIE-", "SEC-", "samsung", "HTC",
            "mot-", "mitsu", "sagem", "sony"
            , "alcatel", "lg", "eric", "vx",
            "NEC", "philips", "mmm", "xx",
            "panasonic", "sharp", "wap", "sch",
            "rover", "pocket", "benq", "java",
            "pt", "pg", "vox", "amoi",
            "bird", "compal", "kg", "voda",
            "sany", "kdd", "dbt", "sendo",
            "sgh", "gradi", "jb", "dddi",
            "moto", "iphone"
        };

                //Loop through each item in the list created above 
                //and check if the header contains that text
                foreach (string s in mobiles)
                {
                    if (context.Request.ServerVariables["HTTP_USER_AGENT"].
                                                        ToLower().Contains(s.ToLower()))
                    { 
                        return true;
                    }
                }
            }

       
            return false;
        }




        #region 根据地址取百度地图里的经度和纬度
        /// <summary>根据地址获取百度地图的经纬度
        /// http://developer.baidu.com/map/index.php?title=webapi/guide/webservice-geocoding
        /// </summary>
        /// <param name="baidu_ak">百度AK</param>
        /// <param name="address">地址，如：南宁淡村市场</param>
        /// <param name="lng">取到的经度，取不到则是空字符串</param>
        /// <param name="lat">取到的纬度，取不到则是空字符串</param>
        public static void GetBaiduMapJWDByAddress(string baidu_ak, string address, out string lng, out string lat)
        {
            string url = "http://api.map.baidu.com/geocoder/v2/?address=" + address + "&output=json&ak=" + baidu_ak;
            HttpHelper hh = new HttpHelper();
            HttpResult hr = hh.GetHtml(new HttpItem()
            {
                URL = url,
                Method = "GET",
            });

            Baidu_Geocoder geo = Newtonsoft.Json.JsonConvert.DeserializeObject<Baidu_Geocoder>(hr.Html);
            if (geo.status == 0)
            {
                lng = geo.result.location.lng;
                lat = geo.result.location.lat;
            }
            else
            {
                lng = "";
                lat = "";
            }
        }
        public class Baidu_Location
        {
            /// <summary>
            /// 经度
            /// </summary>
            public string lng { set; get; }
            /// <summary>
            /// 纬度
            /// </summary>

            public string lat { set; get; }
        }

        public class Baidu_Result
        {
            public Baidu_Location location { set; get; }
            public int precise { set; get; }
            public int confidence { set; get; }
            public string level { set; get; }
        }

        public class Baidu_Geocoder
        {
            public int status { set; get; }
            public Baidu_Result result { set; get; }
        }
        #endregion


        /// <summary>导出word, 只用于导出网页内容为word
        /// 
        /// </summary>
        /// <param name="bodyhtml">html代码,如果有样式也直接写在里面</param>
        public static void ExportWord(string bodyhtml)
        {

            HttpContext.Current.Response.ContentType = "application/msword";

            HttpContext.Current.Response.ContentEncoding = System.Text.UnicodeEncoding.UTF8;

            HttpContext.Current.Response.Charset = "UTF-8";

            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=我的创业计划书.doc");



            HttpContext.Current.Response.Write("<html>");

            HttpContext.Current.Response.Write("<head>");

            HttpContext.Current.Response.Write("<META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=UTF-8\">");

            HttpContext.Current.Response.Write("<meta name=ProgId content=Word.Document>");

            HttpContext.Current.Response.Write("<meta name=Generator content=\"Microsoft Word 9\">");

            HttpContext.Current.Response.Write("<meta name=Originator content=\"Microsoft Word 9\">");



            HttpContext.Current.Response.Write("</head>");

            HttpContext.Current.Response.Write("<body>");

            HttpContext.Current.Response.Write(bodyhtml);

            HttpContext.Current.Response.Write("</body>");

            HttpContext.Current.Response.Write("</html>");


            HttpContext.Current.Response.Flush();

        }


        /// <summary>取IP地址所对应的地方
        /// 查询IP纯真库
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="ipfilePath">qqwry.dat的绝对路径</param>        
        /// <returns></returns>
        public static string GetIPAddress(string ip, string ipfilePath)
        {
            try
            {
                IPSearch ipSearch = new IPSearch(ipfilePath);
                IPSearch.IPLocation loc = ipSearch.GetIPLocation(ip);


                return loc.country + " " + loc.area;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        /// <summary>保存网络上的地址到本地
        /// 以二进制的方式从网上下载下来
        /// </summary>
        /// <param name="FileName">本地全路径 ，如：d:/aaa.jpg</param>
        /// <param name="Url">网络地址，如：http://aaa.com/aaa.jpg </param>
        /// <returns></returns>
        public static bool SavePhotoFromUrl(string FileName, string Url)
        {
            bool Value = false;
            WebResponse response = null;
            Stream stream = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

                response = request.GetResponse();
                stream = response.GetResponseStream();

                if (!response.ContentType.ToLower().StartsWith("text/"))
                {
                    Value = true;
                    byte[] buffer = new byte[1024];

                    try
                    {
                        if (System.IO.File.Exists(FileName))
                        {
                            System.IO.File.Delete(FileName);
                        }

                        Stream outStream = System.IO.File.Create(FileName);
                        Stream inStream = response.GetResponseStream();

                        int l;
                        do
                        {
                            l = inStream.Read(buffer, 0, buffer.Length);
                            if (l > 0)
                                outStream.Write(buffer, 0, l);
                        }
                        while (l > 0);

                        outStream.Close();
                        inStream.Close();
                    }
                    catch
                    {
                        Value = false;
                    }
                    return Value;
                }

            }
            catch (Exception err)
            {
                throw err;
            }
            return Value;
        }

        /// <summary>
        /// CS客户端获取本地IP地址信息
        /// </summary>
        public static string GetAddressIP_CS()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }


        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);
        /// <summary>取用户的网卡MAC地址
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetMAC()
        {
            string strClientIP = "";
            try
            {
                strClientIP = GetRealIP();
            }
            catch (Exception)
            {

                strClientIP = GetAddressIP_CS();
            }
            Int32 ldest = inet_addr(strClientIP); //目的地的ip 
            Int64 macinfo = new Int64();
            Int32 len = 6;
            int res = SendARP(ldest, 0, ref macinfo, ref len);
            string mac_src = macinfo.ToString("X");
            while (mac_src.Length < 12)
            {
                mac_src = mac_src.Insert(0, "0");
            }
            string mac_dest = "";

            for (int i = 0; i < 11; i++)
            {
                if (0 == (i % 2))
                {
                    if (i == 10)
                    {
                        mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
                    }
                    else
                    {
                        mac_dest = "-" + mac_dest.Insert(0, mac_src.Substring(i, 2));
                    }
                }
            }

            return mac_dest;
        }



        /// <summary>取属性或方法的注释说明
        /// 必须是[Description("注释说明")]这样的才能取到
        /// </summary>
        /// <param name="classname">类名,包含命名空间,如:JiuFen.Today.Web.niunan_aaa</param>
        /// <param name="pmname">属性或方法名</param>
        /// <param name="op">属性还是方法, p属性m方法,默认m</param>
        public static string GetDescription(string classname, string pmname, string op = "m")
        {
            try
            {
                Type t = Type.GetType(classname);
                if (op == "m")
                {
                    System.Reflection.MethodInfo method = t.GetMethod(pmname);
                    return ((DescriptionAttribute)method.GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description;
                }
                else
                {
                    System.Reflection.PropertyInfo prop = t.GetProperty(pmname);
                    return ((DescriptionAttribute)prop.GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description;
                }
            }
            catch (Exception ex)
            {
                return "出错: " + ex.Message;
            }
        }


        /// <summary>取真实IP地址
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetRealIP()
        {
            string ip = "";
            try
            { 
                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
 
                if ((string.IsNullOrEmpty(ip) || ip == "127.0.0.1" || ip == "::1") && HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    ip = HttpContext.Current.Request.ServerVariables["HTTP_VIA"].ToString();

                } 
                if ((string.IsNullOrEmpty(ip) || ip == "127.0.0.1" || ip == "::1") && HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
                {
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.IsNullOrEmpty(ip))
                {
                    ip = HttpContext.Current.Request.UserHostAddress;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return ip;
        }

        /// <summary>把Unicode解码为普通文字
        /// 
        /// </summary>
        /// <param name="unicodeString">要解码的Unicode字符集</param>
        /// <returns>解码后的字符串</returns>
        public static string ConvertToGB(string unicodeString)
        {
            string pat = @"(\\u[0-9A-Fa-f]{2,})";
            string[] arr = Regex.Split(unicodeString, pat);
            string result = string.Empty;
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);
            Match m = r.Match(unicodeString);
            for (int i = 0; i < arr.Length; i++)
            {
                //奇数则是\u****
                if (i % 2 == 0)
                {
                    result += arr[i];
                }
                else
                {
                    result += char.ConvertFromUtf32(Convert.ToInt32(arr[i].Replace("\\u", ""), 16));  //char.ConvertFromUtf32(Convert.ToInt32("7F8E", 16))="美"
                }

                m = m.NextMatch();
            }
            return result;
        }

        /// <summary>把汉字字符转码为Unicode字符集
        /// 
        /// </summary>
        /// <param name="strGB">要转码的字符</param>
        /// <returns>转码后的字符</returns>
        public static string ConvertToUnicode(string strGB)
        {
            char[] chs = strGB.ToCharArray();
            string result = string.Empty;
            foreach (char c in chs)
            {
                result += @"\u" + char.ConvertToUtf32(c.ToString(), 0).ToString("x");
            }
            return result;
        }

        /// <summary>取当前主机地址
        ///  如：http://niunan.net:80/   
        ///  注意最后是没有/
        /// </summary>
        /// <returns></returns>
        public static string GetHostUrl()
        {
            string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port;

            return url;
        }

        /// <summary>根据日期取是星期几
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string GetXingQi(DateTime d)
        {
            string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = Day[Convert.ToInt32(d.DayOfWeek.ToString("d"))].ToString();
            return week;
        }

        /// <summary>取下一个星期几
        /// 例：今天星期三，传入4，则会取明天星期四的日期
        /// 传入3，则会返回下星期的星期三的日期
        /// 传入2，则会返回下星期二的日期
        /// </summary>
        /// <param name="xingqi">0表示星期天</param>
        /// <returns></returns>
        public static DateTime GetNextXingQi(int xingqi)
        {
            DateTime outdate;

            DateTime now = DateTime.Now;

            if (xingqi == 0)
            {
                xingqi = 7;
            }
            if ((int)now.DayOfWeek == xingqi)
            {
                //今天正好是星期二
                outdate = now.AddDays(7);
            }
            else if ((int)now.DayOfWeek < xingqi)
            {
                //今天星期二，我要找下个星期六
                outdate = now.AddDays(xingqi - (int)now.DayOfWeek);
            }
            else
            {
                //今天星期二，我要找下个星期一 
                outdate = now.AddDays((7 - (int)now.DayOfWeek) + xingqi);
            }
            return outdate;
        }


        /// <summary>后台执行alert弹出对话框
        /// 
        /// </summary>
        /// <param name="_Msg">警告字串</param>
        /// <param name="_Page">this.Page</param>
        /// <returns>警告框JS</returns>
        public static void Alert(string _Msg, Page _Page)
        {
            string StrScript;
            StrScript = ("<script language=javascript>");
            StrScript += ("alert('" + _Msg.Replace("'", " ") + "');");
            StrScript += ("</script>");
            _Page.ClientScript.RegisterStartupScript(_Page.GetType(), "MsgBox", StrScript);
        }

        /// <summary>后台执行alert后跳转到新页面
        /// 
        /// </summary>
        /// <param name="_msg">提示框中的字符串</param>
        /// <param name="_href">跳转的页面</param>
        /// <param name="_page">this.Page</param>
        public static void AlertAndGo(string _msg, string _href, Page _page)
        {
            string StrScript;
            StrScript = ("<script language=javascript>");
            StrScript += ("alert('" + _msg.Replace("'", " ") + "');location.href='" + _href + "'");
            StrScript += ("</script>");
            _page.ClientScript.RegisterStartupScript(_page.GetType(), "MsgBox", StrScript);
        }

        /// <summary>后台执行alert后跳转到新页面,用于MVC
        /// 
        /// </summary>
        /// <param name="_msg">提示框中的字符串</param>
        /// <param name="_href">跳转的页面</param>
        /// <param name="_page">this.Page</param>
        public static void AlertAndGo_MVC(string _msg, string _href)
        {
            string StrScript;
            StrScript = ("<script language=javascript>");
            StrScript += ("alert('" + _msg.Replace("'", " ") + "');location.href='" + _href + "'");
            StrScript += ("</script>");
            HttpContext.Current.Response.Write(StrScript);
        }

        /// <summary>后台执行alert后关闭页面,用于MVC
        /// 
        /// </summary>
        /// <param name="_msg">提示框中的字符串</param> 
        public static void AlertAndCloseWin_MVC(string _msg)
        {
            string StrScript;
            StrScript = ("<script language=javascript>");
            StrScript += ("alert('" + _msg.Replace("'", " ") + "');window.close();");
            StrScript += ("</script>");
            HttpContext.Current.Response.Write(StrScript);
        }


        /// <summary>根据传入的图片路径获取宽度和高度
        /// 
        /// </summary>
        /// <param name="img">图片绝对路径，传入如：d:/aweb/upload/20140303/fdas.jpg</param>
        /// <param name="w">返回的宽度宽度</param>
        /// <param name="h">返回的高度</param>
        public static void GetImgWH(string img, out int w, out int h)
        {
            Bitmap bit = new Bitmap(img);
            if (bit != null)
            {
                w = bit.Width;
                h = bit.Height;
            }
            else
            {
                w = 0;
                h = 0;
            }
        }

        /// <summary>创建规定大小的图像   源图像只能是JPG格式和PNG格式
        /// 生成的图片默认背景是白色
        /// </summary>
        /// <param name="oPath">源图像绝对路径</param>
        /// <param name="tPath">生成图像绝对路径</param>
        /// <param name="width">生成图像的宽度</param>
        /// <param name="height">生成图像的高度</param>
        /// <param name="color">颜色，默认是白色，如：white,black</param>
        /// <param name="scale">是否拉伸，yes|no，默认no,如果是yes则会把整个画布填充满，此时color属性无效,生成出来的图像会变形</param>
        public static void CreateImage(string oPath, string tPath, int width, int height, string color = "white", string scale = "no")
        {
            FileInfo fi = new FileInfo(tPath);
            if (fi != null)
            {
                if (!Directory.Exists(fi.DirectoryName))
                {
                    Directory.CreateDirectory(fi.DirectoryName);
                }
            }

            if (string.IsNullOrEmpty(color))
            {
                color = "white";
            }
            Bitmap originalBmp = new Bitmap(oPath);
            // 源图像在新图像中的位置
            int left, top;

            #region 拉伸图片，不按等比例缩放
            if (scale == "yes")
            {
                Bitmap bmpOut = new Bitmap(width, height);
                using (Graphics graphics = Graphics.FromImage(bmpOut))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.FillRectangle(Brushes.White, 0, 0, width, height);
                    graphics.DrawImage(originalBmp, 0, 0, width, width);
                }
                bmpOut.Save(tPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmpOut.Dispose();
                originalBmp.Dispose();
                return;
            }
            #endregion


            if (originalBmp.Width <= width && originalBmp.Height <= height)
            {
                // 原图像的宽度和高度都小于生成的图片大小
                left = (int)Math.Round((decimal)(width - originalBmp.Width) / 2);
                top = (int)Math.Round((decimal)(height - originalBmp.Height) / 2);


                // 最终生成的图像
                Bitmap bmpOut = new Bitmap(width, height);
                using (Graphics graphics = Graphics.FromImage(bmpOut))
                {
                    // 设置高质量插值法
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    // 清空画布并以白色背景色填充
                    graphics.Clear(Color.FromName(color));
                    // 把源图画到新的画布上
                    graphics.DrawImage(originalBmp, left, top, originalBmp.Width, originalBmp.Height);
                }
                bmpOut.Save(tPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmpOut.Dispose();
                originalBmp.Dispose();
                return;
            }


            // 新图片的宽度和高度，如400*200的图像，想要生成160*120的图且不变形，
            // 那么生成的图像应该是160*80，然后再把160*80的图像画到160*120的画布上
            int newWidth, newHeight;
            if (width * originalBmp.Height < height * originalBmp.Width)
            {
                newWidth = width;
                newHeight = (int)Math.Round((decimal)originalBmp.Height * width / originalBmp.Width);
                // 缩放成宽度跟预定义的宽度相同的，即left=0，计算top
                left = 0;
                top = (int)Math.Round((decimal)(height - newHeight) / 2);
            }
            else
            {
                newWidth = (int)Math.Round((decimal)originalBmp.Width * height / originalBmp.Height);
                newHeight = height;
                // 缩放成高度跟预定义的高度相同的，即top=0，计算left
                left = (int)Math.Round((decimal)(width - newWidth) / 2);
                top = 0;
            }


            // 生成按比例缩放的图，如：160*80的图
            Bitmap bmpOut2 = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(bmpOut2))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
                graphics.DrawImage(originalBmp, 0, 0, newWidth, newHeight);
            }
            // 再把该图画到预先定义的宽高的画布上，如160*120
            Bitmap lastbmp = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(lastbmp))
            {
                // 设置高质量插值法
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // 清空画布并以白色背景色填充
                graphics.Clear(Color.FromName(color));
                // 把源图画到新的画布上
                graphics.DrawImage(bmpOut2, left, top);
            }
            lastbmp.Save(tPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            lastbmp.Dispose();
            originalBmp.Dispose();
        }

        /// <summary>按比例缩放图片
        /// 
        /// </summary>
        /// <param name="oPath">源图像绝对路径</param>
        /// <param name="tPath">生成图像绝对路径</param>
        /// <param name="wh">数值</param>
        /// <param name="op">按宽度还是高度进行等比缩放，width or height</param>
        /// <param name="other_wh">另一个宽度或者高度值的输出</param>
        public static void ZoomImage(string oPath, string tPath, int wh, string op, out int other_wh)
        {
            Bitmap originalBmp = new Bitmap(oPath);
            int newWidth, newHeight;
            if (op == "width")
            {
                //按宽度等比例缩放
                newWidth = wh;
                newHeight = (int)Math.Round((decimal)originalBmp.Height * wh / originalBmp.Width);
                other_wh = newHeight;
            }
            else
            {
                //按高度等比例缩放
                newWidth = (int)Math.Round((decimal)originalBmp.Width * wh / originalBmp.Height);
                newHeight = wh;
                other_wh = newWidth;
            }

            // 生成按比例缩放的图
            Bitmap bmpOut2 = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(bmpOut2))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
                graphics.DrawImage(originalBmp, 0, 0, newWidth, newHeight);
            }
            bmpOut2.Save(tPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            bmpOut2.Dispose();
            originalBmp.Dispose();
        }

        /// <summary>按比例缩放图片
        /// 
        /// </summary>
        /// <param name="oPath">源图像绝对路径</param>
        /// <param name="tPath">生成图像绝对路径</param>
        /// <param name="wh">数值</param>
        /// <param name="op">按宽度还是高度进行等比缩放,width or height</param>
        public static void ZoomImage(string oPath, string tPath, int wh, string op)
        {
            int x;
            ZoomImage(oPath, tPath, wh, op, out x);
        }

        /// <summary>后台执行JS
        /// 
        /// </summary>
        /// <param name="js">JS代码</param>
        /// <param name="_Page">this.Page</param>
        public static void ExecJS(string js, Page _Page)
        {
            string StrScript;
            StrScript = ("<script language='javascript' type='text/javascript'>");
            StrScript += js;
            StrScript += ("</script>");
            _Page.ClientScript.RegisterStartupScript(_Page.GetType(), "ExecJS", StrScript);
        }



        /// <summary>返回当前应用程序虚拟路径的根路径
        /// 在网上则返回/，本地则返回/5mdn，本地返回的要在最后加个/，即返回/5mdn/
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationPath()
        {
            string path = HttpContext.Current.Request.ApplicationPath;
            return path == "/" ? path : path + "/";
        }

        /// <summary>根据视频地址生成相应的emabed标签
        /// 
        /// </summary>
        /// <param name="url">允许：.swf .flv .mp3 .wav .wma .wmv .mid .avi .mpg .asf .rm .rmvb</param>
        /// <returns></returns>
        public static string GenEmabed(string url)
        {
            string tmp = url.Substring(url.LastIndexOf(".") + 1);
            if (tmp == "swf" || tmp == "swf" || tmp == "flv" || tmp == "mp3" || tmp == "wav" || tmp == "wma" || tmp == "wmv" || tmp == "mid" || tmp == "avi" || tmp == "mpg" || tmp == "asf" || tmp == "rm" || tmp == "rmvb")
            {
                string str = "";
                switch (tmp)
                {
                    case "flv":
                    case "mp3":
                    case "swf":
                        str = "<embed src='" + url + "' type='application/x-shockwave-flash' width='550' height='400' autostart='false' loop='true' />";
                        break;
                    case "rm":
                    case "rmvb":
                        str = "<embed src='" + url + "' type='audio/x-pn-realaudio-plugin' width='550' height='400' autostart='false' loop='true' />";
                        break;
                    default:
                        str = "<embed src='" + url + "' type='video/x-ms-asf-plugin' width='550' height='400' autostart='false' loop='true' />";
                        break;
                }
                return str;
            }
            else
            {
                return "视频格式只能是swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb";
            }
        }

        ///   <summary>去除HTML标记    
        ///      
        ///   </summary>   
        ///   <param    name="NoHTML">包括HTML的源码</param>   
        ///   <returns>已经去除后的文字</returns>   
        public static string GetNoHTMLString(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

            return Htmlstring;
        }

        /// <summary>获取显示的字符串，可显示HTML标签，但把危险的HTML标签过滤，如iframe,script等。  
        ///   
        /// </summary>  
        /// <param name="str">未处理的字符串</param>  
        /// <returns></returns>  
        public static string GetSafeHTMLString(string str)
        {
            str = Regex.Replace(str, @"<applet[^>]*?>.*?</applet>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<body[^>]*?>.*?</body>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<embed[^>]*?>.*?</embed>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<frame[^>]*?>.*?</frame>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<frameset[^>]*?>.*?</frameset>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<html[^>]*?>.*?</html>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<iframe[^>]*?>.*?</iframe>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<style[^>]*?>.*?</style>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<layer[^>]*?>.*?</layer>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<link[^>]*?>.*?</link>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<ilayer[^>]*?>.*?</ilayer>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<meta[^>]*?>.*?</meta>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"<object[^>]*?>.*?</object>", "", RegexOptions.IgnoreCase);
            return str;
        }

        /// <summary>过滤SQL非法字符串
        /// 字符串长度不能超过20个,把前后空格都去除
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetSafeSQL(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            value = value.Trim();
            value = Tool.StringTruncat(value, 40, "");
            value = Regex.Replace(value, @";", string.Empty);
            value = Regex.Replace(value, @"'", string.Empty);
            value = Regex.Replace(value, @"&", string.Empty);
            value = Regex.Replace(value, @"%20", string.Empty);
            value = Regex.Replace(value, @"--", string.Empty);
            value = Regex.Replace(value, @"==", string.Empty);
            value = Regex.Replace(value, @"<", string.Empty);
            value = Regex.Replace(value, @">", string.Empty);
            value = Regex.Replace(value, @"%", string.Empty);
            return value;
        }

        /// <summary>隐藏IP最后一位
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string HiddenIP(string ip)
        {
            int x = ip.LastIndexOf('.');
            string tmp = ip.Substring(0, x + 1);
            return tmp + "*";
        }

        /// <summary>MD5加密字符串
        /// 
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <returns></returns>
        public static string MD5(string str)
        {
            string Password = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            return Password;
        }

        /// <summary>SHA1加密字符串
        /// 
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <returns></returns>
        public static string SHA1(string str)
        {
            string Password = FormsAuthentication.HashPasswordForStoringInConfigFile(str, System.Web.Configuration.FormsAuthPasswordFormat.SHA1.ToString());
            return Password;
        }

        /// <summary>发送email,默认是25端口
        /// 
        /// </summary>
        /// <param name="title">邮件标题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="toAdress">收件人</param>
        /// <param name="fromAdress">发件人</param>
        /// <param name="userName">发件用户名</param>
        /// <param name="userPwd">发件密码</param>
        /// <param name="smtpHost">smtp地址</param>
        public static void SendMail(string title, string body, string toAdress, string fromAdress,
                              string userName, string userPwd, string smtpHost)
        {
            try
            {
                MailAddress to = new MailAddress(toAdress);
                MailAddress from = new MailAddress(fromAdress);
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(from, to);
                message.IsBodyHtml = true; // 如果不加上这句那发送的邮件内容中有HTML会原样输出 
                message.Subject = title; message.Body = body;
                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = true;
                smtp.Port = 25;
                smtp.Credentials = new NetworkCredential(userName, userPwd);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = smtpHost;
                message.To.Add(toAdress);
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>上传文件方法，默认生成日期文件夹
        /// 返回文件名
        /// </summary>
        /// <param name="myFileUpload">上传控件ID</param>
        /// <param name="allowExtensions">允许上传的扩展文件名类型,如：string[] allowExtensions = { ".doc", ".xls", ".ppt", ".jpg", ".gif" };</param>
        /// <param name="maxLength">允许上传的最大大小，以M为单位</param>
        /// <param name="savePath">保存文件的目录，注意是绝对路径,如：Server.MapPath("~/upload/");</param>
        /// <param name="gendatedir">是否生成日期文件夹,默认true</param>
		/// <param name="guidname">是否是GUID名字，默认true</param>
        /// <param name="savename">保存文件名（不带后缀），如果非空则以此文件名保存</param>
        public static string Upload(FileUpload myFileUpload, string[] allowExtensions, int maxLength, string savePath, bool gendatedir = true, bool guidname = true, string savename = "")
        {
            // 文件格式是否允许上传
            bool fileAllow = false;

            //检查是否有文件案
            if (myFileUpload.HasFile)
            {
                // 检查文件大小, ContentLength获取的是字节，转成M的时候要除以2次1024
                if (myFileUpload.PostedFile.ContentLength / 1024 / 1024 >= maxLength)
                {
                    throw new Exception("只能上传小于" + maxLength + "M的文件！");
                }

                //取得上传文件之扩展文件名，并转换成小写字母
                string fileExtension = System.IO.Path.GetExtension(myFileUpload.FileName).ToLower();
                string tmp = "";   // 存储允许上传的文件后缀名
                //检查扩展文件名是否符合限定类型
                for (int i = 0; i < allowExtensions.Length; i++)
                {
                    tmp += i == allowExtensions.Length - 1 ? allowExtensions[i] : allowExtensions[i] + ",";
                    if (fileExtension == allowExtensions[i])
                    {
                        fileAllow = true;
                    }
                }

                if (fileAllow)
                {
                    try
                    {
                        string datedir = DateTime.Now.ToString("yyyyMMdd");
                        if (!Directory.Exists(savePath + datedir) && gendatedir)
                        {
                            Directory.CreateDirectory(savePath + datedir);
                        }
                        string saveName = guidname ? Guid.NewGuid() + fileExtension : myFileUpload.FileName;
                        if (!string.IsNullOrEmpty(savename))
                        {
                            saveName = savename + fileExtension;
                        }
                        string path = gendatedir ? savePath + datedir + "/" + saveName : savePath + saveName;
                        //存储文件到文件夹
                        myFileUpload.SaveAs(path);
                        return gendatedir ? datedir + "/" + saveName : saveName;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                else
                {
                    throw new Exception("文件格式不符，可以上传的文件格式为：" + tmp);
                }
            }
            else
            {
                throw new Exception("请选择要上传的文件！");
            }
        }

        /// <summary>上传文件方法，默认生成日期文件夹,MVC使用
        /// 返回文件名
        /// </summary>
        /// <param name="PostedFile">mvc后台获取的</param>
        /// <param name="allowExtensions">允许上传的扩展文件名类型,如：string[] allowExtensions = { ".doc", ".xls", ".ppt", ".jpg", ".gif" };</param>
        /// <param name="maxLength">允许上传的最大大小，以M为单位</param>
        /// <param name="savePath">保存文件的目录，注意是绝对路径,如：Server.MapPath("~/upload/");</param>
        /// <param name="gendatedir">是否生成日期文件夹</param>
        /// <param name="guidname">是否是GUID名字，默认true</param>
        /// <param name="savename">保存文件名（不带后缀），如果非空则以此文件名保存</param>
        public static string Upload_MVC(HttpPostedFileBase PostedFile, string[] allowExtensions, int maxLength, string savePath, bool gendatedir = true, bool guidname = true, string savename = "")
        {


            // 文件格式是否允许上传
            bool fileAllow = false;


            // 检查文件大小, ContentLength获取的是字节，转成M的时候要除以2次1024
            if (PostedFile.ContentLength / 1024 / 1024 >= maxLength)
            {
                throw new Exception("只能上传小于" + maxLength + "M的文件！");
            }

            //取得上传文件之扩展文件名，并转换成小写字母
            string fileExtension = System.IO.Path.GetExtension(PostedFile.FileName).ToLower();
            string tmp = "";   // 存储允许上传的文件后缀名
                               //检查扩展文件名是否符合限定类型
            for (int i = 0; i < allowExtensions.Length; i++)
            {
                tmp += i == allowExtensions.Length - 1 ? allowExtensions[i] : allowExtensions[i] + ",";
                if (fileExtension == allowExtensions[i])
                {
                    fileAllow = true;
                }
            }

            if (fileAllow)
            {
                try
                {
                    string datedir = DateTime.Now.ToString("yyyyMMdd");
                    if (!Directory.Exists(savePath + datedir) && gendatedir)
                    {
                        Directory.CreateDirectory(savePath + datedir);
                    }
                    string saveName = guidname ? Guid.NewGuid() + fileExtension : PostedFile.FileName;
                    if (!string.IsNullOrEmpty(savename))
                    {
                        saveName = savename + fileExtension;
                    }
                    string path = gendatedir ? savePath + datedir + "/" + saveName : savePath + saveName;
                    //存储文件到文件夹
                    PostedFile.SaveAs(path);
                    return gendatedir ? datedir + "/" + saveName : saveName;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                throw new Exception("文件格式（" + fileExtension + "）不符，可以上传的文件格式为：" + tmp);
            }
        }

        /// <summary>写日志文件
        /// 
        /// </summary>
        /// <param name="logmsg">日记内容</param>
        /// <param name="absolutefile">绝对路径文件，如：d:/aaa/bbb/ccc/log.txt</param>
        public static void TxtLog(string logmsg, string absolutefile)
        {
            try
            {
                StreamWriter sw = new StreamWriter(absolutefile, true, Encoding.Default);
                sw.WriteLine("时间：" + DateTime.Now);
                sw.WriteLine(logmsg);
                sw.WriteLine("");
                sw.WriteLine("");
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {

            }
        }


        ///   <summary>将指定字符串按指定长度进行剪切  
        ///   
        ///   </summary> 
        ///   <param   name= "oldStr "> 需要截断的字符串 </param> 
        ///   <param   name= "maxLength "> 字符串的最大长度 </param> 
        ///   <param   name= "endWith "> 超过长度的后缀 </param> 
        ///   <returns> 如果超过长度，返回截断后的新字符串加上后缀，否则，返回原字符串 </returns> 
        public static string StringTruncat(string oldStr, int maxLength, string endWith)
        {
            if (string.IsNullOrEmpty(oldStr))
                //   throw   new   NullReferenceException( "原字符串不能为空 "); 
                return oldStr + endWith;
            if (maxLength < 1)
                throw new Exception("返回的字符串长度必须大于[0] ");
            if (oldStr.Length > maxLength)
            {
                string strTmp = oldStr.Substring(0, maxLength);
                if (string.IsNullOrEmpty(endWith))
                    return strTmp;
                else
                    return strTmp + endWith;
            }
            return oldStr;
        }

        /// <summary>金额转大写
        /// 
        /// </summary>
        /// <param name="LowerMoney"></param>
        /// <returns></returns>
        public static string MoneyToChinese(string LowerMoney)
        {
            string functionReturnValue = null;
            bool IsNegative = false; // 是否是负数
            if (LowerMoney.Trim().Substring(0, 1) == "-")
            {
                // 是负数则先转为正数
                LowerMoney = LowerMoney.Trim().Remove(0, 1);
                IsNegative = true;
            }
            string strLower = null;
            string strUpart = null;
            string strUpper = null;
            int iTemp = 0;
            // 保留两位小数 123.489→123.49　　123.4→123.4
            LowerMoney = Math.Round(double.Parse(LowerMoney), 2).ToString();
            if (LowerMoney.IndexOf(".") > 0)
            {
                if (LowerMoney.IndexOf(".") == LowerMoney.Length - 2)
                {
                    LowerMoney = LowerMoney + "0";
                }
            }
            else
            {
                LowerMoney = LowerMoney + ".00";
            }
            strLower = LowerMoney;
            iTemp = 1;
            strUpper = "";
            while (iTemp <= strLower.Length)
            {
                switch (strLower.Substring(strLower.Length - iTemp, 1))
                {
                    case ".":
                        strUpart = "圆";
                        break;
                    case "0":
                        strUpart = "零";
                        break;
                    case "1":
                        strUpart = "壹";
                        break;
                    case "2":
                        strUpart = "贰";
                        break;
                    case "3":
                        strUpart = "叁";
                        break;
                    case "4":
                        strUpart = "肆";
                        break;
                    case "5":
                        strUpart = "伍";
                        break;
                    case "6":
                        strUpart = "陆";
                        break;
                    case "7":
                        strUpart = "柒";
                        break;
                    case "8":
                        strUpart = "捌";
                        break;
                    case "9":
                        strUpart = "玖";
                        break;
                }

                switch (iTemp)
                {
                    case 1:
                        strUpart = strUpart + "分";
                        break;
                    case 2:
                        strUpart = strUpart + "角";
                        break;
                    case 3:
                        strUpart = strUpart + "";
                        break;
                    case 4:
                        strUpart = strUpart + "";
                        break;
                    case 5:
                        strUpart = strUpart + "拾";
                        break;
                    case 6:
                        strUpart = strUpart + "佰";
                        break;
                    case 7:
                        strUpart = strUpart + "仟";
                        break;
                    case 8:
                        strUpart = strUpart + "万";
                        break;
                    case 9:
                        strUpart = strUpart + "拾";
                        break;
                    case 10:
                        strUpart = strUpart + "佰";
                        break;
                    case 11:
                        strUpart = strUpart + "仟";
                        break;
                    case 12:
                        strUpart = strUpart + "亿";
                        break;
                    case 13:
                        strUpart = strUpart + "拾";
                        break;
                    case 14:
                        strUpart = strUpart + "佰";
                        break;
                    case 15:
                        strUpart = strUpart + "仟";
                        break;
                    case 16:
                        strUpart = strUpart + "万";
                        break;
                    default:
                        strUpart = strUpart + "";
                        break;
                }

                strUpper = strUpart + strUpper;
                iTemp = iTemp + 1;
            }

            strUpper = strUpper.Replace("零拾", "零");
            strUpper = strUpper.Replace("零佰", "零");
            strUpper = strUpper.Replace("零仟", "零");
            strUpper = strUpper.Replace("零零零", "零");
            strUpper = strUpper.Replace("零零", "零");
            strUpper = strUpper.Replace("零角零分", "整");
            strUpper = strUpper.Replace("零分", "整");
            strUpper = strUpper.Replace("零角", "零");
            strUpper = strUpper.Replace("零亿零万零圆", "亿圆");
            strUpper = strUpper.Replace("亿零万零圆", "亿圆");
            strUpper = strUpper.Replace("零亿零万", "亿");
            strUpper = strUpper.Replace("零万零圆", "万圆");
            strUpper = strUpper.Replace("零亿", "亿");
            strUpper = strUpper.Replace("零万", "万");
            strUpper = strUpper.Replace("零圆", "圆");
            strUpper = strUpper.Replace("零零", "零");

            // 对壹圆以下的金额的处理
            if (strUpper.Substring(0, 1) == "圆")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "零")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "角")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "分")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "整")
            {
                strUpper = "零圆整";
            }
            functionReturnValue = strUpper;

            if (IsNegative == true)
            {
                return "负" + functionReturnValue;
            }
            else
            {
                return functionReturnValue;
            }
        }

        /// <summary>转全角的函数
        /// 
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>全角字符串</returns>
        public static string DBCToSBC(string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);

        }


        /// <summary>转半角的函数
        /// 
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        public static string SBCToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        #region 道池做的加解密，可以与他用C++写的DES加解密对应起来
        /*  下面是调用示例
             Console.WriteLine("DES加密");
            string src = "abcdefghijklmn1234567890";
            string key = "abcd1234";
            string enc = EncryptString(src, key);
            Console.WriteLine("Enc: " + enc);
            string dec = DecryptString(enc, key);
            Console.WriteLine("Dec: " + dec);

            Console.WriteLine("AES_CBC_256加密");
            src = "abcdefgh12345678abcdefgh123456";
            key = "wxyz20121221!@#$wxyz20121221!@#$";
            enc = Encrypt(src, key);
            Console.WriteLine("Enc: " + enc);
            dec = Decrypt(enc, key);
            Console.WriteLine("Dec: " + dec);
             */
        static byte[] StringArgToByteArray(string arg)
        {
            byte[] arr = new byte[arg.Length / 2];

            for (int i = 0, j = 0; i < arg.Length; i += 2, ++j)
            {
                string sub = arg.Substring(i, 2);
                arr[j] = Convert.ToByte(sub, 16);
            }

            return arr;
        }

        /// <summary>
        /// DES加密
        /// <param name="txt">要加密的字符串</param>
        /// <param name="key">密钥，得是8位</param>
        /// </summary>
        public static string EncryptString(string txt, string key)
        {
            byte[] data = Encoding.UTF8.GetBytes(txt);
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.UTF8.GetBytes(key);
            DES.IV = ASCIIEncoding.UTF8.GetBytes(key);
            DES.Mode = CipherMode.ECB;
            ICryptoTransform desEncrypt = DES.CreateEncryptor();
            byte[] result = desEncrypt.TransformFinalBlock(data, 0, data.Length);
            StringBuilder sb = new StringBuilder();
            foreach (byte item in result)
            {
                sb.AppendFormat("{0:X2}", item);
            }
            return sb.ToString();
        }
        /// <summary>
        /// DES解密
        /// <param name="txt">要解密的字符串</param>
        /// <param name="key">密钥，得是8位</param>
        /// </summary>
        public static string DecryptString(string txt, string key)
        {
            byte[] data = StringArgToByteArray(txt);
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.UTF8.GetBytes(key);
            DES.IV = ASCIIEncoding.UTF8.GetBytes(key);
            DES.Mode = CipherMode.ECB;
            ICryptoTransform desencrypt = DES.CreateDecryptor();
            byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            return Encoding.UTF8.GetString(result);
        }

        /// <summary>  
        /// 256位AES加密  
        /// C++采用OpenSSL加密时，不管明文长度是否正好为16字节的整数倍，都会在末尾填充16字节的0x10（即使十进制的16）
        /// 而明文长度不是16字节的整数倍时，先填充零，再填充16字节的0x10
        /// <param name="toEncrypt">要加密的字符串</param>
        /// <param name="key">加密密钥，得是32位</param>
        /// </summary>   
        public static string Encrypt(string toEncrypt, string key)
        {
            byte[] toEncryptArray0 = Encoding.UTF8.GetBytes(toEncrypt);
            int pad = (16 - toEncrypt.Length % 16) % 16;
            int arrSize = toEncryptArray0.Length + pad;
            Console.WriteLine("size = " + arrSize + ", pad = " + pad);
            if (pad == 0)            //原文长度正好是16字节的整数倍，需要在末尾填充16字节
                arrSize += 16;
            byte[] toEncryptArray = new byte[arrSize];
            toEncryptArray0.CopyTo(toEncryptArray, 0);
            if (pad > 0)
            {
                byte[] pb = new byte[pad];
                for (int i = 0; i < pad; ++i)
                    pb[i] = (byte)pad;
                pb.CopyTo(toEncryptArray, toEncryptArray0.Length);
            }
            else
            {
                for (int i = arrSize - 16; i < arrSize; ++i)
                    toEncryptArray[i] = 0x10;
            }
            //byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = Encoding.UTF8.GetBytes(key);
            rDel.IV = Encoding.UTF8.GetBytes(key.Substring(0, 16));
            //rDel.BlockSize = 128;
            //rDel.KeySize = 256;
            //rDel.FeedbackSize = 128;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            StringBuilder sb = new StringBuilder();
            //foreach (byte item in resultArray)
            //{
            //    sb.AppendFormat("{0:X2}", item);
            //    //sb.Append(" ");
            //}
            for (int i = 0; i < resultArray.Length; ++i)
            {
                sb.AppendFormat("{0:X2}", resultArray[i]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 256位AES加密，返回字节数组
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Encrypt_byte(string toEncrypt, string key)
        {
            byte[] toEncryptArray0 = Encoding.UTF8.GetBytes(toEncrypt);
            int pad = (16 - toEncrypt.Length % 16) % 16;
            int arrSize = toEncryptArray0.Length + pad;
            Console.WriteLine("size = " + arrSize + ", pad = " + pad);
            if (pad == 0)            //原文长度正好是16字节的整数倍，需要在末尾填充16字节
                arrSize += 16;
            byte[] toEncryptArray = new byte[arrSize];
            toEncryptArray0.CopyTo(toEncryptArray, 0);
            if (pad > 0)
            {
                byte[] pb = new byte[pad];
                for (int i = 0; i < pad; ++i)
                    pb[i] = (byte)pad;
                pb.CopyTo(toEncryptArray, toEncryptArray0.Length);
            }
            else
            {
                for (int i = arrSize - 16; i < arrSize; ++i)
                    toEncryptArray[i] = 0x10;
            }
            //byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = Encoding.UTF8.GetBytes(key);
            rDel.IV = Encoding.UTF8.GetBytes(key.Substring(0, 16));
            //rDel.BlockSize = 128;
            //rDel.KeySize = 256;
            //rDel.FeedbackSize = 128;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return resultArray;
        }

        /// <summary>  
        /// 256位AES解密  
        /// <param name="toDecrypt">要解密的字符串</param>
        /// <param name="key">密钥，得是32位</param>
        /// </summary>
        public static string Decrypt(string toDecrypt, string key)
        {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = Encoding.UTF8.GetBytes(key);
            rDel.IV = Encoding.UTF8.GetBytes(key.Substring(0, 16)); ;
            //rDel.BlockSize = 128;
            //rDel.KeySize = 256;
            //rDel.FeedbackSize = 128;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            //byte[] bdec = UTF8Encoding.UTF8.GetBytes(toDecrypt);
            byte[] bdec = StringArgToByteArray(toDecrypt);
            byte[] resultArray = cTransform.TransformFinalBlock(bdec, 0, bdec.Length);
            StringBuilder sb = new StringBuilder();
            // for (int i = 0; i < resultArray.Length && resultArray[i] != 0x00; ++i)
            for (int i = 0; i < resultArray.Length && resultArray[i] != 0x00 && resultArray[i] != 0x10; ++i)
            {
                //sb.AppendFormat("{0:X2}", resultArray[i]);
                sb.Append(Encoding.UTF8.GetString(resultArray, i, 1));
            }

            //return UTF8Encoding.UTF8.GetString(resultArray);
            return sb.ToString();
        }
        /// <summary>
        /// 256位AES解密，传入字节数组
        /// </summary>
        /// <param name="toDecrypt">字节数组</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt_byte(byte[] toDecrypt, string key)
        {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = Encoding.UTF8.GetBytes(key);
            rDel.IV = Encoding.UTF8.GetBytes(key.Substring(0, 16)); ;
            //rDel.BlockSize = 128;
            //rDel.KeySize = 256;
            //rDel.FeedbackSize = 128;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            //byte[] bdec = UTF8Encoding.UTF8.GetBytes(toDecrypt);
            byte[] bdec = toDecrypt;
            byte[] resultArray = cTransform.TransformFinalBlock(bdec, 0, bdec.Length);
            StringBuilder sb = new StringBuilder();
            // for (int i = 0; i < resultArray.Length && resultArray[i] != 0x00; ++i)
            for (int i = 0; i < resultArray.Length && resultArray[i] != 0x00 && resultArray[i] != 0x10; ++i)
            {
                //sb.AppendFormat("{0:X2}", resultArray[i]);
                sb.Append(Encoding.UTF8.GetString(resultArray, i, 1));
            }

            //return UTF8Encoding.UTF8.GetString(resultArray);
            return sb.ToString();
        }
        #endregion

        #region 加解密
        private static string CIV = "aXwL7X2+fgM=";//密钥
        private static string CKEY = "FwGQWRRgKCI=";//初始化向量
        /// <summary>
        ///加密字符串
        /// </summary>
        /// <param name="Value">要加密的字符</param>
        /// <returns>加密了的字符串</returns>
        public static string EncryptString(string Value)
        {
            ICryptoTransform ct;//定义基本的加密运算符
            MemoryStream ms;
            CryptoStream cs;//定义将数据流转链接到加密转换的流
            byte[] byt;
            //CreateEncryptor创建加密对象
            SymmetricAlgorithm mCSP = new DESCryptoServiceProvider();
            ct = mCSP.CreateEncryptor(Convert.FromBase64String(CKEY), Convert.FromBase64String(CIV));

            byt = Encoding.UTF8.GetBytes(Value);

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();

            cs.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="Value">要解密字符串</param>
        /// <returns>解密了的字符串</returns>
        public static string DecryptString(string Value)
        {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;

            SymmetricAlgorithm mCSP = new DESCryptoServiceProvider();
            ct = mCSP.CreateDecryptor(Convert.FromBase64String(CKEY), Convert.FromBase64String(CIV));

            byt = Convert.FromBase64String(Value);

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();

            cs.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
        #endregion

        /// <summary>获取汉字拼音首字母
        /// 
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static string GetChineseSpell(string strText)
        {
            return StrToPinyin.GetChineseSpell(strText);
        }

        /// <summary>获取网页源代码
        /// 
        /// </summary>
        /// <param name="url">目标网页地址</param>
        /// <param name="code">如果目标网页是utf-8编码的在此输入utf8，若是gb2312编码的则不用输入</param>
        /// <returns></returns>
        public static string GetWebresourceFile(string url, string code)
        {

            WebClient myWebClient = new WebClient();
            myWebClient.Headers.Add(HttpRequestHeader.UserAgent, "anything");
            byte[] myDataBuffer;
            try
            {
                myDataBuffer = myWebClient.DownloadData(url);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string SourceCode = "";
            if ("utf8" == code)
            {
                SourceCode = Encoding.UTF8.GetString(myDataBuffer);  //url对应的网页是 utf-8 的编码则是用这个 
            }
            else
            {
                SourceCode = Encoding.Default.GetString(myDataBuffer);  //url对应的网页是 gb2312 的编码则是用这个
            }

            return SourceCode;
        }

        /// <summary>获取CheckBoxList控件选中的值
        ///  默认连接Text
        /// </summary>
        /// <param name="chkl">CheckBoxList对象</param>
        /// <param name="joinstr">连接字符，如,</param>
        /// <param name="mode">text或者value</param>
        /// <returns></returns>
        public static string GetCheckBoxListValue(CheckBoxList chkl, char joinstr, string mode = "text")
        {
            string str = "";
            foreach (ListItem item in chkl.Items)
            {
                if (item.Selected)
                {
                    if (mode == "text")
                    {
                        str += item.Text + joinstr;
                    }
                    else
                    {
                        str += item.Value + joinstr;
                    }
                }
            }
            if (str.Length != 0)
            {
                str = str.Substring(0, str.Length - 1);
            }
            return str;
        }

        /// <summary>设置CheckBoxList的值
        /// 
        /// </summary>
        /// <param name="chkl">CheckBoxList对象</param>
        /// <param name="values">字符串</param>
        /// <param name="mode">text或者value</param>
        public static void SetCheckBoxListValue(CheckBoxList chkl, string values, string mode = "text")
        {
            foreach (ListItem item in chkl.Items)
            {
                if (mode == "text")
                {
                    if (values.Contains(item.Text))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                }
                else
                {
                    if (values.Contains(item.Value))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                }
            }
        }

        /// <summary>设置DropDownList的值
        /// 
        /// </summary>
        /// <param name="ddl">DDL控件</param>
        /// <param name="values">要设置的值</param>
        /// <param name="findtype">查找类型：value,text</param>
        public static void SetDropDownListValue(DropDownList ddl, string values, string findtype)
        {
            ListItem li = null;
            if (findtype.ToLower() == "value")
            {
                li = ddl.Items.FindByValue(values);
            }
            else
            {
                li = ddl.Items.FindByText(values);
            }
            if (li != null)
            {
                ddl.ClearSelection();
                li.Selected = true;
            }
        }

        /// <summary>通过URL生成静态页
        /// 
        /// </summary>
        /// <param name="strURL">URL，如：http://localhost:123/aaa.aspx</param>
        /// <param name="strRealPath">保存的静态页的绝对路径 ，如：Server.MapPath("~/static/aaa.html")</param>
        /// <param name="strCode">页面编码，空则默认为utf-8，如：gb2312,utf-8</param>
        /// <param name="isYaSuo">是否压缩HTML，默认否</param>
        /// <returns></returns>
        public static bool GenHTMLFromURL(string strURL, string strRealPath, string strCode, bool isYaSuo = false)
        {

            if (string.IsNullOrEmpty(strCode))
            {
                strCode = "utf-8";
            }
            string strFilePage;
            strFilePage = strRealPath;
            StreamWriter sw = null;
            //获得aspx的静态html  
            try
            {

                if (File.Exists(strFilePage))
                {
                    File.Delete(strFilePage);
                }
                sw = new StreamWriter(strFilePage, false, System.Text.Encoding.GetEncoding(strCode));
                System.Net.WebRequest wReq = System.Net.WebRequest.Create(strURL);
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                System.IO.StreamReader reader = new System.IO.StreamReader(respStream, System.Text.Encoding.GetEncoding(strCode));
                string strTemp = reader.ReadToEnd();

                Regex r1 = new Regex("<input type=\"hidden\" name=\"__EVENTTARGET\".*/>", RegexOptions.IgnoreCase);
                Regex r2 = new Regex("<input type=\"hidden\" name=\"__EVENTARGUMENT\".*/>", RegexOptions.IgnoreCase);
                Regex r3 = new Regex("<input type=\"hidden\" name=\"__VIEWSTATE\".*/>", RegexOptions.IgnoreCase);

                Regex r4 = new Regex("<form .*id=\"form1\">", RegexOptions.IgnoreCase);
                Regex r5 = new Regex("</form>");

                Regex r6 = new Regex("<input type=\"hidden\" name=\"__EVENTVALIDATION\".*/>", RegexOptions.IgnoreCase);
                strTemp = r1.Replace(strTemp, "");
                strTemp = r2.Replace(strTemp, "");
                strTemp = r3.Replace(strTemp, "");
                strTemp = r4.Replace(strTemp, "");
                strTemp = r5.Replace(strTemp, "");
                strTemp = r6.Replace(strTemp, "");

                if (isYaSuo)
                {
                    strTemp = Regex.Replace(strTemp, "\\n+\\s+", string.Empty);
                }

                sw.Write(strTemp);
            }
            catch (Exception ex)
            {
                throw ex;
                HttpContext.Current.Response.Write(ex.Message);
                HttpContext.Current.Response.End();
                return false;//生成到出错  
            }
            finally
            {
                sw.Flush();
                sw.Close();
                sw = null;
            }

            return true;
        }

        /// <summary>生成静态HTML页面
        /// 
        /// </summary>
        /// <param name="temppath">模板页面路径，如： Server.MapPath("static/about_temp.html")</param>
        /// <param name="genfilepath">生成的静态页路径，如：Server.MapPath("static/about.html")</param>
        /// <param name="sourcestr">模板页中需要替换的字符集合</param>
        /// <param name="replacestr">替换后的字符集合</param>
        /// <param name="strcode">页面编码，默认utf-8，如：gb2312,utf-8</param>
        public static void GenHTMLFromTemp(string temppath, string genfilepath, List<string> sourcestr, List<string> replacestr, string strcode)
        {
            //思路是替换掉模板中的特征字符  
            if (string.IsNullOrEmpty(strcode))
            {
                strcode = "utf-8";
            }
            string mbPath = temppath;
            Encoding code = Encoding.GetEncoding(strcode);
            StreamReader sr = null;
            StreamWriter sw = null;
            string str = null;

            //读取  
            try
            {
                sr = new StreamReader(mbPath, code);
                str = sr.ReadToEnd();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sr.Close();
            }

            //替换内容
            for (int i = 0; i < sourcestr.Count; i++)
            {
                if (i <= replacestr.Count)
                {
                    str = str.Replace(sourcestr[i], replacestr[i]);
                }
            }


            //生成静态文件  
            try
            {
                sw = new StreamWriter(genfilepath, false, code);
                sw.Write(str);
                sw.Flush();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sw.Close();
                //  Response.Write("恭喜<a href=static/" + fileName + " target=_blank>" + fileName + "</a>已经生成，保存在static文件夹下！");
            }
        }

        /// <summary>产生随机字符串
        /// 
        /// </summary>
        /// <param name="str">从该字符串中随机选择,如：0123456789</param>
        /// <param name="num">随机出几个字符</param>
        /// <returns>随机出的字符串</returns>
        public static string GenRandomCode(string str, int num)
        {
            // string str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";//"的一是在不了有和人这中大为上个国我以要他时来用们生到作地于出就分对成会可主发年动同工也能下过子说产种面而方后多定行学法所民得经十三之进着等部度家电力里如水化高自二理起小物现实加量都两体制机当使点从业本去把性好应开它合还因由其些然前外天政四日那社义事平形相全表间样与关各重新线内数正心反你明看原又么利比或但质气第向道命此变条只没结解问意建月公无系军很情者最立代想已通并提直题党程展五果料象员革位入常文总次品式活设及管特件长求老头基资边流路级少图山统接知较将组见计别她手角期根论运农指几九区强放决西被干做必战先回则任取据处队南给色光门即保治北造百规热领七海口东导器压志世金增争济阶油思术极交受联什认六共权收证改清己美再采转更单风切打白教速花带安场身车例真务具万每目至达走积示议声报斗完类八离华名确才科张信马节话米整空元况今集温传土许步群广石记需段研界拉林律叫且究观越织装影算低持音众书布复容儿须际商非验连断深难近矿千周委素技备半办青省列习响约支般史感劳便团往酸历市克何除消构府称太准精值号率族维划选标写存候毛亲快效斯院查江型眼王按格养易置派层片始却专状育厂京识适属圆包火住调满县局照参红细引听该铁价严";
            char[] chastr = str.ToCharArray();
            // string[] source ={ "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "#", "$", "%", "&", "@" };
            string code = "";
            Random rd = new Random();
            int i;
            for (i = 0; i < num; i++)
            {
                //code += source[rd.Next(0, source.Length)];
                code += str.Substring(rd.Next(0, str.Length), 1);
            }
            return code;

        }
        /// <summary>输出硬盘文件，提供下载
        /// 
        /// </summary>  
        /// <param name="_fileName">下载文件名</param>
        /// <param name="_fullPath">带文件名下载路径</param>
        /// <param name="_speed">每秒允许下载的字节数</param>
        /// <returns>返回是否成功</returns>
        public static bool DownloadFile(string _fileName, string _fullPath, long _speed)
        {
            HttpRequest _Request = System.Web.HttpContext.Current.Request;
            HttpResponse _Response = System.Web.HttpContext.Current.Response;
            try
            {
                FileStream myFile = new FileStream(_fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(myFile);
                try
                {
                    _Response.AddHeader("Accept-Ranges", "bytes");
                    _Response.Buffer = false;
                    long fileLength = myFile.Length;
                    long startBytes = 0;

                    int pack = 10240; //10K bytes
                                      //int sleep = 200;   //每秒5次   即5*10K bytes每秒
                    int sleep = (int)Math.Floor((decimal)1000 * pack / _speed) + 1;
                    if (_Request.Headers["Range"] != null)
                    {
                        _Response.StatusCode = 206;
                        string[] range = _Request.Headers["Range"].Split(new char[] { '=', '-' });
                        startBytes = Convert.ToInt64(range[1]);
                    }
                    _Response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                    if (startBytes != 0)
                    {
                        _Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }
                    _Response.AddHeader("Connection", "Keep-Alive");
                    _Response.ContentType = "application/octet-stream";
                    _Response.Charset = "UTF-8";
                    _Response.ContentEncoding = Encoding.UTF8;

                    HttpBrowserCapabilities bc = _Request.Browser;
                    string browser = bc.Browser.ToString();
                    bool IsIE = browser.ToLower().Contains("ie") || browser.ToLower().Contains("internetexplorer") || browser.ToLower().Contains("chrome");
                    string filename = IsIE ? HttpUtility.UrlEncode(System.Text.UTF8Encoding.UTF8.GetBytes(_fileName)) : _fileName;

                    _Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);

                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    int maxCount = (int)Math.Floor((decimal)(fileLength - startBytes) / pack) + 1;

                    for (int i = 0; i < maxCount; i++)
                    {
                        if (_Response.IsClientConnected)
                        {
                            _Response.BinaryWrite(br.ReadBytes(pack));
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            i = maxCount;
                        }
                    }
                    _Response.End();
                }
                catch
                {
                    return false;
                }
                finally
                {
                    br.Close();
                    myFile.Close();
                }

            }
            catch (Exception err)
            {
                throw err;
            }
            return true;
        }

        /// <summary>输出硬盘文件，提供下载
        /// 
        /// </summary>  
        /// <param name="_fileName">下载文件名</param>
        /// <param name="_URL">带文件名下载路径</param>
        /// <param name="_speed">每秒允许下载的字节数</param>
        /// <returns>返回是否成功</returns>
        public static bool DownloadURLFile(string _fileName, string _URL, long _speed)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(_URL);
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            HttpRequest _Request = System.Web.HttpContext.Current.Request;
            HttpResponse _Response = System.Web.HttpContext.Current.Response;

            System.IO.Stream dataStream = httpResponse.GetResponseStream();  // 取資料流
            byte[] buffer = new byte[10240 * 10];

            try
            {


                try
                {
                    _Response.AddHeader("Accept-Ranges", "bytes");
                    _Response.Buffer = false;
                    long fileLength = httpResponse.ContentLength;
                    long startBytes = 0;

                    int pack = 10240; //10K bytes
                                      //int sleep = 200;   //每秒5次   即5*10K bytes每秒
                    int sleep = (int)Math.Floor((decimal)1000 * pack / _speed) + 1;
                    if (_Request.Headers["Range"] != null)
                    {
                        _Response.StatusCode = 206;
                        string[] range = _Request.Headers["Range"].Split(new char[] { '=', '-' });
                        startBytes = Convert.ToInt64(range[1]);
                    }
                    _Response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                    if (startBytes != 0)
                    {
                        _Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }
                    _Response.AddHeader("Connection", "Keep-Alive");
                    _Response.ContentType = "application/octet-stream";
                    _Response.Charset = "UTF-8";
                    _Response.ContentEncoding = Encoding.UTF8;

                    HttpBrowserCapabilities bc = _Request.Browser;
                    string browser = bc.Browser.ToString();
                    string filename = browser.ToLower().Contains("ie") ? HttpUtility.UrlEncode(System.Text.UTF8Encoding.UTF8.GetBytes(_fileName)) : _fileName;

                    _Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);


                    int maxCount = (int)Math.Floor((decimal)(fileLength - startBytes) / pack) + 1;
                    int size = 0;
                    for (int i = 0; i < maxCount; i++)
                    {
                        if (_Response.IsClientConnected)
                        {
                            size = dataStream.Read(buffer, 0, buffer.Length);
                            if (size > 0)
                            {
                                _Response.OutputStream.Write(buffer, 0, size);
                                Thread.Sleep(sleep);
                            }
                        }
                        else
                        {
                            i = maxCount;
                        }
                    }
                    _Response.End();
                }
                catch
                {
                    return false;
                }
                finally
                {

                }

            }
            catch (Exception err)
            {
                throw err;
            }
            return true;
        }

        /// <summary>从XLS文件读取到DataTable中
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(string path)
        {
            return DataTableRenderToExcel.RenderDataTableFromExcel(path);
        }

        /// <summary>把excel导入到datatable中，根据sheet的索引
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="sheetnum"></param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(string filepath, int sheetnum)
        {
            return DataTableRenderToExcel.RenderDataTableFromExcel(filepath, sheetnum);
        }

        /// <summary>把excel导入到datatable中，根据sheet的名称
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="sheetnum"></param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(string filepath, string sheetname)
        {
            return DataTableRenderToExcel.RenderDataTableFromExcel(filepath, sheetname);
        }

        /// <summary>从DataTable中输出XLS数据流，前台下载代码如下：
        /// DataTable dt = DataTableRenderToExcel.RenderDataTableFromExcel(MapPath("~/bbb.xls"));
        ///    MemoryStream ms = DataTableRenderToExcel.RenderDataTableToExcel(dt) as MemoryStream;
        ///    Response.AddHeader("Content-Disposition", string.Format("attachment; filename=Download.xls"));
        ///    Response.BinaryWrite(ms.ToArray());
        ///    ms.Close();
        ///     ms.Dispose();
        /// </summary>
        /// <param name="SourceTable"></param>
        /// <returns></returns>
        public static Stream RenderDataTableToExcel(DataTable SourceTable)
        {
            return DataTableRenderToExcel.RenderDataTableToExcel(SourceTable);
        }

        /// <summary>获取Excel文件中sheet的集合
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static List<string> GetExcelSheet(string filepath)
        {
            return DataTableRenderToExcel.GetExcelSheet(filepath);
        }




        /// <summary> 给图片上水印   
        ///  原图片大小小于水印图片的也直接把水印画上去，水印不会自动缩放
        /// </summary>   
        /// <param name="filePath">原图片地址(物理路径)</param>   
        /// <param name="waterFile">水印图片地址（物理路径）</param>   
        /// <param name="savepath">保存的图片路径（物理路径）</param>
        public static void MarkWater(string filePath, string waterFile, string savepath)
        {
            //GIF不水印   
            int i = filePath.LastIndexOf(".");
            string ex = filePath.Substring(i, filePath.Length - i);
            if (string.Compare(ex, ".gif", true) == 0)
            {
                return;
            }

            string ModifyImagePath = filePath;//修改的图像路径   
            int lucencyPercent = 45;
            System.Drawing.Image modifyImage = null;
            System.Drawing.Image drawedImage = null;
            Graphics g = null;
            try
            {
                //建立图形对象   
                modifyImage = System.Drawing.Image.FromFile(ModifyImagePath, true);
                drawedImage = System.Drawing.Image.FromFile(waterFile, true);
                g = Graphics.FromImage(modifyImage);
                //获取要绘制图形坐标   
                int x = modifyImage.Width - drawedImage.Width;
                int y = modifyImage.Height - drawedImage.Height;
                //设置颜色矩阵   
                float[][] matrixItems ={
            new float[] {1, 0, 0, 0, 0},
            new float[] {0, 1, 0, 0, 0},
            new float[] {0, 0, 1, 0, 0},
            new float[] {0, 0, 0, (float)lucencyPercent/100f, 0},
            new float[] {0, 0, 0, 0, 1}};

                ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
                ImageAttributes imgAttr = new ImageAttributes();
                imgAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                //绘制阴影图像   
                g.DrawImage(drawedImage, new Rectangle(x, y, drawedImage.Width, drawedImage.Height), 0, 0, drawedImage.Width, drawedImage.Height, GraphicsUnit.Pixel, imgAttr);
                //保存文件   
                string[] allowImageType = { ".jpg", ".gif", ".png", ".bmp", ".tiff", ".wmf", ".ico" };
                FileInfo fi = new FileInfo(ModifyImagePath);
                ImageFormat imageType = ImageFormat.Gif;
                switch (fi.Extension.ToLower())
                {
                    case ".jpg": imageType = ImageFormat.Jpeg; break;
                    case ".gif": imageType = ImageFormat.Gif; break;
                    case ".png": imageType = ImageFormat.Png; break;
                    case ".bmp": imageType = ImageFormat.Bmp; break;
                    case ".tif": imageType = ImageFormat.Tiff; break;
                    case ".wmf": imageType = ImageFormat.Wmf; break;
                    case ".ico": imageType = ImageFormat.Icon; break;
                    default: break;
                }
                MemoryStream ms = new MemoryStream();
                modifyImage.Save(ms, imageType);
                byte[] imgData = ms.ToArray();
                modifyImage.Dispose();
                drawedImage.Dispose();
                g.Dispose();
                ms.Dispose();
                FileStream fs = null;
                // File.Delete(ModifyImagePath);
                fs = new FileStream(savepath, FileMode.Create, FileAccess.Write);
                if (fs != null)
                {
                    fs.Write(imgData, 0, imgData.Length);
                    fs.Close();
                }
            }
            finally
            {
                try
                {
                    drawedImage.Dispose();
                    modifyImage.Dispose();
                    g.Dispose();
                }
                catch
                {
                }
            }
        }


   
    }
}



