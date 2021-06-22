using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WacVisitor
{
    public class ClassTest
    {
        public static int visitorLogCount = 0;
        public static int visitorInfoCount = 0;
        public static JArray array;

        public static DataTable dataLogTodayComplete = new DataTable("dataLogTodayComplete");
        public static DataTable dataLogTodayNotComplete = new DataTable("dataLogTodayNotComplete");
        public static DataTable dataLogPassComplete = new DataTable("dataLogPasstComplete");
        public static DataTable dataLogPassNotComplete = new DataTable("dataLogPasstNotComplete");

        public static DataTable dataLogPassAllNotComplete = new DataTable("dataLogPassAllNotComplete");

        public static DataTable TEST_METHODE(bool getBackward, string yyyyMMdd = "", int page = 1, int limit_per_page = 100, 
                                                string visitorType = "", string another = "")
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            if (yyyyMMdd == "")
                yyyyMMdd = DateTime.Now.ToString("yyyyMMdd");

            classGlobal.userId = "5fe2c8a089d27d3818e4bcba";            
            array = GET_ALL_TRANSACTION(yyyyMMdd, getBackward, page, limit_per_page);
            Console.Write(ClassData.visitorRecordCount);

            #region get count each visitor type
            //string[] stringsVisitorType = new string[4] { "ไม่ระบุ", "พนักงานส่งของ", "เจ้าหน้าที่", "Normal" };
            ////++ Group distinct visitorType 
            //foreach (var itms in stringsVisitorType)
            //{
            //    List<JToken> lstVisitorTypeDistinct = new List<JToken>();

            //    lstVisitorTypeDistinct = array
            //         .Where(x => x.Value<String>("visitorType").Equals(itms, StringComparison.InvariantCultureIgnoreCase) &&
            //                     x.Value<String>("timestamp").Contains(DateTime.Now.ToString("MM/dd/yyyy")))
            //         .Select(x => x)
            //         .ToList();
            //    int countIn = lstVisitorTypeDistinct.Count;  //=> วันนี้ เข้า

            //    lstVisitorTypeDistinct = array
            //         .Where(x => x.Value<String>("visitorType").Equals(itms, StringComparison.InvariantCultureIgnoreCase) &&
            //                     x.Value<String>("timestamp").Contains(DateTime.Now.ToString("MM/dd/yyyy")) &&
            //                     x.Value<String>("recordTimeOut").Contains(":"))
            //         .Select(x => x)
            //         .ToList();
            //    int countOut = lstVisitorTypeDistinct.Count; //=> วันนี้ ออก 

            //    lstVisitorTypeDistinct = array
            //         .Where(x => x.Value<String>("visitorType").Equals(itms, StringComparison.InvariantCultureIgnoreCase) &&
            //                     x.Value<String>("recordTimeOut").Equals("", StringComparison.InvariantCultureIgnoreCase))
            //         .Select(x => x)
            //         .ToList();
            //    int countStill = lstVisitorTypeDistinct.Count; //=> ยังไม่ออก
            //}
            ////-- Group distinct visitorType => วันนี้ เข้า
            #endregion

            #region search in json array
            //JArray arrayFilter;
            //List<JToken> lstFilter = new List<JToken>();

            //if (visitorType != "") // all visitor type
            //{
            //    lstFilter = array
            //     .Where(x => x.Value<String>("visitorType").Equals(visitorType, StringComparison.InvariantCultureIgnoreCase))
            //     .Select(x => x)
            //     .ToList();
            //    arrayFilter = new JArray(lstFilter);
            //}
            //else
            //{
            //    arrayFilter = array;
            //}

            //if (another != "")
            //{                
            //    lstFilter = arrayFilter
            //      .Where(x => x.Value<String>("visitorNumber").Contains(another) ||
            //                x.Value<String>("licensePlate").Contains(another) ||
            //                x.Value<String>("contactPlace").Contains(another))
            //      .Select(x => x)
            //      .ToList();
            //    arrayFilter = new JArray(lstFilter);
            //}
            //else
            //{
            //    //Console.Write(arrayFilter);
            //}
            //array = arrayFilter;
            #endregion

            float pageAll;
            int pageSize = 100;
            int cntRow = ClassData.visitorRecordCount;
            if (cntRow % pageSize == 0)
                pageAll = cntRow / pageSize;
            else
                pageAll = (int)Math.Floor((float)(cntRow / pageSize)) + 1;

            //++ loop
            array.Clear();
            for (int p = 1; p <= pageAll; p++)
            {
                JArray arrayTemp = GET_ALL_TRANSACTION(yyyyMMdd, getBackward, p, pageSize);
                array.Merge(arrayTemp); 
            }
            //-- loop

            DataTable listLogs = new DataTable("listLogs");
            listLogs.Columns.Add("visitorId");
            listLogs.Columns.Add("visitorType");
            listLogs.Columns.Add("recordTimeIn");
            listLogs.Columns.Add("recordTimeOut");
            listLogs.Columns.Add("timestamp");

            listLogs.Columns.Add("visitorNumber");
            listLogs.Columns.Add("image1");
            listLogs.Columns.Add("image2");
            listLogs.Columns.Add("image3");
            listLogs.Columns.Add("image4");

            listLogs.Columns.Add("visitorFrom");
            listLogs.Columns.Add("licensePlate");
            listLogs.Columns.Add("name");
            listLogs.Columns.Add("contactPlace");
            listLogs.Columns.Add("citizenId");
            listLogs.Columns.Add("etc");
            listLogs.Columns.Add("vehicleType");

            listLogs.Columns.Add("follower");
            listLogs.Columns.Add("department");
            listLogs.Columns.Add("visitPerson");
            listLogs.Columns.Add("contactTopic");

            int n = 0;
            JArray jsArrImage;
            foreach (var x in array)  // foreach (var x in array.Skip((page - 1) * pageSize).Take(pageSize))  //foreach (var x in jsArray)
            {
                string timeStamp = classGlobal.CONVERT_UTC_TO_LOCAL(x["timestamp"].ToString()).Substring(0, 10);
                string[] splt = timeStamp.Split('-');
                if (int.Parse(splt[0]) > 2500)
                    timeStamp = (int.Parse(splt[0]) - 543).ToString() + "-" + splt[1] + "-" + splt[2];

                string _visitorNumber = x["visitorNumber"].ToString();
                string _typeName = x["visitorType"].ToString();
                if (_typeName.Equals("")) _typeName = "ไม่ระบุ";

                string strImage1 = "";
                string strImage2 = "";
                string strImage3 = "";
                string strImage4 = "";
                jsArrImage = (JArray)x["image1"];
                foreach (var y in jsArrImage)
                    strImage1 = y.ToString();

                jsArrImage = (JArray)x["image2"];
                foreach (var y in jsArrImage)
                    strImage2 = y.ToString();

                jsArrImage = (JArray)x["image3"];
                foreach (var y in jsArrImage)
                    strImage3 = y.ToString();

                jsArrImage = (JArray)x["image4"];
                foreach (var y in jsArrImage)
                    strImage4 = y.ToString();

                string visitorFrom = x["visitorFrom"].ToString();
                string licensePlate = x["licensePlate"].ToString();
                string name = x["name"].ToString();
                string contactPlace = x["contactPlace"].ToString();
                string citizenId = x["citizenId"].ToString();
                string etc = x["etc"].ToString();
                string vehicleType = x["vehicleType"].ToString();

                string follower = x["follower"].ToString();
                string department = x["department"].ToString();
                string visitPerson = x["visitPerson"].ToString();
                string contactTopic = x["contactTopic"].ToString();

                listLogs.Rows.Add(x["visitorId"].ToString(),
                                   _typeName,
                                       classGlobal.CONVERT_UTC_TO_LOCAL(x["recordTimeIn"].ToString()),
                                           classGlobal.CONVERT_UTC_TO_LOCAL(x["recordTimeOut"].ToString()),
                                               timeStamp,
                                                   _visitorNumber,
                                                        strImage1, strImage2, strImage3, strImage4,
                                                            visitorFrom, licensePlate, name, contactPlace, citizenId, etc, vehicleType,
                                                                follower, department, visitPerson, contactTopic);

                n += 1;
            }

            dataLogTodayComplete = listLogs.Select("timestamp ='" + DateTime.Now.ToString("yyyy-MM-dd") + "'" + " AND recordTimeOut <> ''").CopyToDataTable();
            dataLogTodayNotComplete = listLogs.Select("timestamp ='" + DateTime.Now.ToString("yyyy-MM-dd") + "'" + " AND recordTimeOut = ''").CopyToDataTable();

            dataLogPassComplete = listLogs.Select("timestamp <> '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" + " AND recordTimeOut <> ''").CopyToDataTable();
            dataLogPassNotComplete = listLogs.Select("timestamp <>'" + DateTime.Now.ToString("yyyy-MM-dd") + "'" + " AND recordTimeOut = ''").CopyToDataTable();

            dataLogPassAllNotComplete = listLogs.Select("recordTimeOut <> ''").CopyToDataTable();

            return listLogs;
        }

        static JArray GET_ALL_TRANSACTION(string yyMMdd = "", bool getBefore = false, int page = 1, int limit_per_page = 12)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            JToken jMessage;
            JArray jsArray = null;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                CultureInfo MyCultureInfo = new CultureInfo("en-US");
                string filterDate = DateTime.Now.ToString("yyyyMMdd");
                if (yyMMdd != "")
                    filterDate = yyMMdd;

                string parameter = "";

                //++ visitorLog
                parameter = "&_time=" + filterDate + "&_before=" + getBefore.ToString().ToLower() + "&_page=" + page + "&_limit=" + limit_per_page + "&_sort=-1";
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "visitorLog" + "?_id=" + classGlobal.userId + parameter);
                request.Method = "GET";
                request.Headers.Add("Authorization", classGlobal.access_token);
                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                jMessage = json["message"];
                try { visitorLogCount = int.Parse((String)jMessage["total"]); }
                catch { visitorLogCount = 0; }
                JObject jsMessageVisitorLog = (JObject)jMessage;
                //-- visitorLog

                //++ visitorInfo
                parameter = "&_time=" + filterDate + "&_before=" + getBefore.ToString().ToLower() + "&_page=" + page + "&_limit=" + limit_per_page + "&_sort=-1";
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "visitorInfo" + "?_id=" + classGlobal.userId + parameter);
                request.Method = "GET";
                request.Headers.Add("Authorization", classGlobal.access_token);
                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                jMessage = json["message"];
                try { visitorInfoCount = int.Parse((String)jMessage["total"]); }
                catch { visitorInfoCount = 0; }
                JObject jsMessageVisitorInfo = (JObject)jMessage;
                //-- visitorInfo

                ClassData.visitorRecordCount = visitorLogCount + visitorInfoCount;

                jsMessageVisitorLog.Merge(jsMessageVisitorInfo);
                JArray array = (JArray)jsMessageVisitorLog["result"];
                jsArray = new JArray(array.OrderByDescending(obj => (DateTime)obj["timestamp"]));  // sort timestamp desc


            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();
            }

            return jsArray;
        }
    }
}
