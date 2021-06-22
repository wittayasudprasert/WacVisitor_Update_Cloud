using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace WacVisitor
{
    public class ClassData
    {

        public static string page = "1";
        public static string limit_per_page = "100";
        public static int pageLooping = 5;  

        static int visitorLogCount = 0;
        static int visitorInfoCount = 0;
        public static int visitorRecordCount = 0;

        #region LOGIN
        public static string POST_LOGIN(string user, string pass)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            JToken jMessage;
            string postData = "";
            try
            {
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "login");
                postData = "username=" + Uri.EscapeDataString(user);
                postData += "&password=" + Uri.EscapeDataString(pass);
                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                jMessage = json["message"];
                classGlobal.refresh_token = (string)jMessage["refresh_token"];

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(classGlobal.refresh_token);
                var tokenS = handler.ReadToken(classGlobal.refresh_token) as JwtSecurityToken;
                classGlobal.userId = tokenS.Claims.First(claim => claim.Type == "userId").Value;
                classGlobal.rule = tokenS.Claims.First(claim => claim.Type == "rule").Value;
                classGlobal.uId = tokenS.Claims.First(claim => claim.Type == "uId").Value;
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;

                if (res != null)
                {
                    string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                    JObject jsRes = JObject.Parse(resString);
                    string status = jsRes["status"].ToString();
                    string message = jsRes["message"].ToString();
                }

                classGlobal.refresh_token = "";
                classGlobal.userId = "";
                classGlobal.rule = "";
                classGlobal.uId = "";
            }

             return classGlobal.refresh_token;
        }
        public static void GET_ACCESS_TOKEN(string refresh_token)
        {
            HttpWebResponse response;
            HttpWebRequest request;
            string responseString = "";
            JObject json;
            JToken jMessage;            
            try
            {
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "token");
                byte[] data = Encoding.UTF8.GetBytes("");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers["Authorization"] = refresh_token;
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                jMessage = json["message"];
                classGlobal.access_token = (string)jMessage["access_token"];

                string activeMode = GET_ACTIVEMODE();
                if (activeMode == null)
                    activeMode = "";

                if (activeMode != "")
                {
                    ClassHelper.clsXML cc = new ClassHelper.clsXML();
                    if (activeMode.Equals("คิดเงิน"))
                    {
                        classGlobal.boolCharge = true;
                        classGlobal.FactoryVersion = false;
                        cc.ModifyElement("root", "version", "home", classGlobal.config);
                        cc.ModifyElement("root", "billing", "true", classGlobal.config);
                    }
                    else
                    {
                        classGlobal.boolCharge = false;
                        classGlobal.FactoryVersion = true;
                        cc.ModifyElement("root", "version", "factory", classGlobal.config);
                        cc.ModifyElement("root", "billing", "false", classGlobal.config);
                    }
                    cc = null;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();
                classGlobal.access_token = "";
            }
        }
        static string GET_ACTIVEMODE()
        {
            HttpWebRequest request;
            HttpWebResponse response;
            JObject json;
            string activeMode = "";
            try
            {
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "activeList");
                request.Method = "GET";
                request.Headers.Add("Authorization", classGlobal.access_token);
                request.ContentType = "application/x-www-form-urlencoded";
                response = (HttpWebResponse)request.GetResponse();
                String responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                JArray ja = (JArray)json["message"];
                foreach (JObject js in ja)
                {
                    if ((String)js["_id"] == classGlobal.userId)
                    {
                        activeMode = (String)js["activeMode"];
                        break;
                    }
                }
               
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();
                activeMode = "";
            }

            return activeMode;
        }

        public static string POST_FOR_GET_UID(string userName)
        {
            string uID = "";
            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            JToken jMessage;
            string postData = "";
            try
            {
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "user");
                postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);
                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.Headers.Add("Authorization", classGlobal.access_token);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                jMessage = json["message"];
                JArray jaData =(JArray)jMessage["data"];
                foreach (JObject jo in jaData)
                {
                    if (userName == (String)jo["username"].ToString())
                    {
                        uID = (String)jo["userId"].ToString();
                        break;
                    }
                }

            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                uID = "";
            }


            return uID;
        }
        #endregion

        #region MAIN METHODE
        public static string GET_METHODE(string api)
        {
            ClassData.GET_ACCESS_TOKEN(classGlobal.refresh_token);

            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            JToken jMessage;
            string postData = "";
            string strReturn = "";
            try
            {

                byte[] data = Encoding.UTF8.GetBytes(postData);
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + api + "/" + classGlobal.userId);
                request.Method = "GET";
                request.Headers.Add("Authorization", classGlobal.access_token);
                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                jMessage = json["message"];
                JArray jData = jMessage[api] as JArray;

                var serializedString = JsonConvert.SerializeObject(jData);
                if (serializedString.ToString().Replace("[]", "") == "")
                    serializedString = "";
                strReturn = serializedString;
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                strReturn = "";
            }

            return strReturn;
        }
        public static string POST_METHODE(string api, string value)
        {

            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            string postData = "";
            string stringsReturn = "";
            try
            {

                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + api);
                postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);
                postData += "&" + api + "=" + Uri.EscapeDataString(value);
                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.Headers.Add("Authorization", classGlobal.access_token);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                stringsReturn = json["status"].ToString();
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                stringsReturn = ex.Message.ToString();
            }

            return stringsReturn;
        }
        public static string DELETE_METHODE(string api, string find)
        {

            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            string postData = "";
            string stringsReturn = "";
            try
            {

                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + api);
                postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);
                postData += "&" + api + "=" + Uri.EscapeDataString(find);
                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.Method = "DELETE";
                request.Headers.Add("Authorization", classGlobal.access_token);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);

                stringsReturn = json["status"].ToString();
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                stringsReturn = message;
            }

            return stringsReturn;
        }
        public static string PUT_METHODE(string api, string find, string editTo)
        {

            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            string postData = "";
            string stringsReturn = "";
            try
            {

                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + api);
                postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);
                postData += "&find=" + Uri.EscapeDataString(find);
                postData += "&editTo=" + Uri.EscapeDataString(editTo);
                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.Method = "PUT";
                request.Headers.Add("Authorization", classGlobal.access_token);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);

                stringsReturn = json["status"].ToString();
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                stringsReturn = message;
            }

            return stringsReturn;
        }
        #endregion

        #region BLACKLIST - WHITELIST
        public static string GET_METHODE_BLACK_WHITE_LIST(string api)
        {

            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            JToken jMessage;
            string postData = "";
            string stringsReturn = "";
            try
            {

                byte[] data = Encoding.UTF8.GetBytes(postData);
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL +  api + "/" + classGlobal.userId);
                request.Method = "GET";
                request.Headers.Add("Authorization", classGlobal.access_token);
                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                jMessage = json["message"];
                JArray jData = jMessage[api] as JArray;

                var serializedString = JsonConvert.SerializeObject(jData);
                if (serializedString.ToString().Replace("[]", "") == "")
                    serializedString = "";

                stringsReturn = serializedString;
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                stringsReturn = message;
            }

            return stringsReturn;
        }
        public static string POST_METHODE_BLACK_WHITE_LIST(string api, string citizenId, string fullname, string timeStart, string timeStop)
        {

            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            string postData = "";
            string stringsReturn = "";
            try
            {

                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + api);
                postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);
                postData += "&citizenId=" + Uri.EscapeDataString(citizenId);
                postData += "&name=" + Uri.EscapeDataString(fullname);
                postData += "&timeStart=" + Uri.EscapeDataString(timeStart);
                postData += "&timeStop=" + Uri.EscapeDataString(timeStop);                
                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.Headers.Add("Authorization", classGlobal.access_token);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);

                stringsReturn = json["status"].ToString();
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                stringsReturn = message;
            }

            return stringsReturn;
        }
        public static string DELETE_METHODE_BLACK_WHITE_LIST(string api, string objectName, string objectTimeName, string objectId, string objectTimeId)
        {

            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            string postData = "";
            string stringsReturn = "";
            try
            {

                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + api);               
                postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);
                postData += "&" + objectName + "=" + Uri.EscapeDataString(objectId);
                postData += "&" + objectTimeName + "=" + Uri.EscapeDataString(objectTimeId);
                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.Method = "DELETE";
                request.Headers.Add("Authorization", classGlobal.access_token);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);

                stringsReturn = json["status"].ToString();
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                stringsReturn = message;
            }

            return stringsReturn;
        }
        //*** BLACK_WHITE_LIST ไม่มีการอัพเดทข้อมูล (ไม่มี PUT) ***//
        #endregion

        #region POST ข้อมูล การเข้า/ออก ไปยัง SERVER
        static string MimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
        public static string POST_VISITOR_IN(Dictionary<string, string> myDicInfo, string access_token, string userId, string[] fileFullPath)
        {
            string stringsReturn = "";
            Uri webService = new Uri(classGlobal.API_URL  + "visitorUp");
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
            requestMessage.Headers.Add("Authorization", access_token);
            requestMessage.Headers.ExpectContinue = false;
            MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
            multiPartContent.Add(new StringContent(userId), "userId");

            foreach (KeyValuePair<string, string> entry in myDicInfo)
                multiPartContent.Add(new StringContent(entry.Value.ToString()), entry.Key.ToString());

            //++ add image jpg            
            for (int i = 0; i < fileFullPath.Length; i++)
            {
                string eachFile = fileFullPath[i].ToString();
                if (eachFile != "")
                {
                    FileInfo fi = new FileInfo(eachFile);
                    string fileName = fi.Name;
                    byte[] fileContents = File.ReadAllBytes(fi.FullName);

                    ByteArrayContent byteArrayContent = new ByteArrayContent(fileContents);
                    byteArrayContent.Headers.Add("Content-Type", MimeType(fileName));
                    multiPartContent.Add(byteArrayContent, "image" + (i + 1).ToString(), fileName);
                }
                else
                {
                    multiPartContent.Add(new StringContent(""), "image" + (i + 1).ToString());
                }
            }
            //-- add image jpg

            requestMessage.Content = multiPartContent;
            HttpClient httpClient = new HttpClient();
            try
            {
                Task<HttpResponseMessage> httpRequest = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, System.Threading.CancellationToken.None);
                HttpResponseMessage httpResponse = httpRequest.Result;
                HttpStatusCode statusCode = httpResponse.StatusCode;
                HttpContent responseContent = httpResponse.Content;
                if (responseContent != null)
                {
                    Task<String> stringContentsTask = responseContent.ReadAsStringAsync();
                    String stringContents = stringContentsTask.Result;   //{"status":200,"message":"complete"}
                    JObject json = JObject.Parse(stringContents);
                    stringsReturn = json["status"].ToString();
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                stringsReturn = message;
            }

            return stringsReturn;
        }
        public static string POST_VISITOR_OUT(string access_token, string userId, string visitorId, string terminalOut)
        {

            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            string postData = "";
            string stringsReturn = "";
            try
            {

                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "visitorUp");
                postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);
                postData += "&visitorId=" + Uri.EscapeDataString(visitorId);
                postData += "&terminalOut=" + Uri.EscapeDataString(terminalOut);
                postData += "&recordStatus=" + Uri.EscapeDataString("out");
                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.Headers.Add("Authorization", classGlobal.access_token);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);

                stringsReturn = json.ToString();//json["status"].ToString();
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                stringsReturn = message;
            }

            return stringsReturn;
        }
        #endregion

        public static string POST_CHECK_EXIST_VISITOR_NUMBER(string access_token, string userId, string visitorNumber, string eventInOut, bool success)
        {

            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            string postData = "";
            string strReturn = "";
            try
            {

                byte[] data = Encoding.UTF8.GetBytes(postData);
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "checkNum" + "/" + classGlobal.userId + "/" + visitorNumber);
                request.Method = "GET";
                request.Headers.Add("Authorization", classGlobal.access_token);
                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);

                responseString = json["message"].ToString();
                if (responseString.Equals("[]"))
                    responseString = "false";
                else
                    responseString = json.ToString(); 

                strReturn = responseString;              
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                strReturn = "false";
            }

            return strReturn;
        }
        public static string DOWNLOAD_IMAGE(string webpname, int width, int height)
        {   
            if (webpname == "")
                return "";

            if (classGlobal.watermark.Replace(" ", "") == "")
                classGlobal.watermark = "แลกบัตรผู้มาเข้าพบ";

            string parameterWaterMark = "&watermark=true&watermarkText=" + classGlobal.watermark;
            if (classGlobal.DisplayHashTag == false)
                parameterWaterMark = "";

            parameterWaterMark = ""; //fix test

            string parameter = "";
            if (width == 0 && height == 0)
                parameter = "";
            else
                parameter = "?width=" + width + "&height=" + height;

            string base64 = "";
            string output = Directory.GetCurrentDirectory() + @"\" + classGlobal.storageImages + @"\" + webpname + ".webp";
            output = Path.GetTempPath() + webpname + ".webp";  // change location to path of the current user's temporary folder.
            output = output.Replace(".webp.webp", ".webp");
            try
            {
                WebClient client = new WebClient();
                client.UseDefaultCredentials = false;
                client.Headers.Add("Authorization", classGlobal.access_token);
                client.DownloadFile(new Uri(classGlobal.API_URL + "view/images/" + webpname + parameter + parameterWaterMark), output);
                base64 = clsImage.CONVERT_WEBP_TO_PNG(output, Directory.GetCurrentDirectory() + @"\" + classGlobal.storageImages + @"\" + webpname + ".png");
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                base64 = "";
            }

            return base64;
        }
        public static string GENERATE_VISITOR_NUMBER(string userId)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            JToken jMessage;
            string postData = "";
            try
            {
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "bookingVisitorNumber");   //   /numRandom
                postData = "userId=" + Uri.EscapeDataString(userId);
                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.Headers.Add("Authorization", classGlobal.access_token);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                jMessage = json["message"];
                responseString = (string)jMessage["visitorNumber"];
               
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                responseString = message;
            }

            return responseString;

        }

        #region เกี่ยวกับ logs การเข้า/ออกที่เสร็จสมบูรณ์แล้ว (GET_LOGS_COMPLETE = visitorLog) กับ logs การเข้าออกที่ยังไม่มีการเข้า/ออกที่สมบูณ์ (GET_LOGS_STILL = visitorInfo)
        public static DataTable GET_LOGS_COMPLETE(bool getBefore, string yyMMdd = "")
        {
            //page = "1";

            ClassData.GET_ACCESS_TOKEN(classGlobal.refresh_token);

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

            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            string postData = "";
            JObject json;
            JToken jMessage;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                CultureInfo MyCultureInfo = new CultureInfo("en-US");
                string filterDate = DateTime.Now.ToString("yyyyMMdd");
                if (yyMMdd != "")
                    filterDate = yyMMdd;

                for (int _page = 1; _page <= pageLooping; _page++)
                {

                    string parameter = "&_time=" + filterDate + "&_before=" + getBefore.ToString().ToLower() + "&_page=" + _page + "&_limit=" + limit_per_page + "&_sort=-1";

                    byte[] data = Encoding.UTF8.GetBytes(postData);
                    request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "visitorLog" + "?_id=" + classGlobal.userId + parameter);
                    request.Method = "GET";
                    request.Headers.Add("Authorization", classGlobal.access_token);
                    response = (HttpWebResponse)request.GetResponse();
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    json = JObject.Parse(responseString);

                    jMessage = json["message"];
                    JObject jsMessage = (JObject)jMessage;

                    visitorRecordCount = 0;
                    try { visitorLogCount = int.Parse((String)jsMessage["total"]); }
                    catch { visitorLogCount = 0; }


                    JArray jsArray = (JArray)jsMessage["result"];
                    string timeStamp = "";
                    JArray jsArrImage;
                    int recordCount = jsArray.Count;
                    int n = 0;
                    foreach (var x in jsArray)
                    {
                        Application.DoEvents();

                        timeStamp = classGlobal.CONVERT_UTC_TO_LOCAL(x["timestamp"].ToString()).Substring(0, 10);
                        string[] splt = timeStamp.Split('-');
                        if (int.Parse(splt[0]) > 2500)
                            timeStamp = (int.Parse(splt[0]) - 543).ToString() + "-" + splt[1] + "-" + splt[2];

                        string _visitorNumber = "";
                        _visitorNumber = x["visitorNumber"].ToString();

                        string _typeName = "";
                        _typeName = x["visitorType"].ToString();

                        if (_typeName.Equals(""))
                            _typeName = "ไม่ระบุ";

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

                        string _recordTimeIn = x["recordTimeIn"].ToString();
                        if (_recordTimeIn == "")
                            _recordTimeIn = x["timestamp"].ToString();

                        listLogs.Rows.Add(x["visitorId"].ToString(),
                                                                   _typeName,
                                                                       classGlobal.CONVERT_UTC_TO_LOCAL(_recordTimeIn),
                                                                           classGlobal.CONVERT_UTC_TO_LOCAL(x["recordTimeOut"].ToString()),
                                                                               timeStamp,
                                                                                    _visitorNumber,
                                                                                        strImage1, strImage2, strImage3, strImage4,
                                                                                            visitorFrom, licensePlate, name, contactPlace, citizenId, etc, vehicleType,
                                                                                                follower, department, visitPerson, contactTopic);

                        n += 1;
                    }


                }

            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();
            }

            return listLogs;
        }
        public static DataTable GET_LOGS_STILL(bool getBefore, string yyMMdd = "")
        {
            //page = "1";

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

            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            string postData = "";
            JObject json;
            JToken jMessage;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                CultureInfo MyCultureInfo = new CultureInfo("en-US");
                string filterDate = DateTime.Now.ToString("yyyyMMdd");
                if (yyMMdd != "")
                    filterDate = yyMMdd;

                for (int _page = 1; _page <= pageLooping; _page++)
                {

                    string parameter = "&_time=" + filterDate + "&_before=" + getBefore.ToString().ToLower() + "&_page=" + _page + "&_limit=" + limit_per_page + "&_sort=-1";

                    byte[] data = Encoding.UTF8.GetBytes(postData);
                    request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "visitorInfo" + "?_id=" + classGlobal.userId + parameter);
                    request.Method = "GET";
                    request.Headers.Add("Authorization", classGlobal.access_token);
                    response = (HttpWebResponse)request.GetResponse();
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    json = JObject.Parse(responseString);

                    jMessage = json["message"];
                    JObject jsMessage = (JObject)jMessage;

                    try { visitorInfoCount = int.Parse((String)jsMessage["total"]); }
                    catch { visitorInfoCount = 0; }
                    visitorRecordCount = visitorLogCount + visitorInfoCount;

                    JArray jsArray = (JArray)jsMessage["result"];
                    string timeStamp = "";
                    JArray jsArrImage;
                    int n = 0;
                    foreach (var x in jsArray)
                    {
                        timeStamp = classGlobal.CONVERT_UTC_TO_LOCAL(x["timestamp"].ToString()).Substring(0, 10);
                        string[] splt = timeStamp.Split('-');
                        if (int.Parse(splt[0]) > 2500)
                            timeStamp = (int.Parse(splt[0]) - 543).ToString() + "-" + splt[1] + "-" + splt[2];

                        string _visitorNumber = "";
                        try { _visitorNumber = x["visitorNumber"].ToString(); }
                        catch { _visitorNumber = x["visiterNumber"].ToString(); }

                        string _typeName = "";
                        _typeName = x["visitorType"].ToString();

                        if (_typeName.Equals(""))
                            _typeName = "ไม่ระบุ";

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

                        string _recordTimeIn = x["recordTimeIn"].ToString();
                        if (_recordTimeIn == "")
                            _recordTimeIn = x["timestamp"].ToString();

                        listLogs.Rows.Add(x["visitorId"].ToString(),
                                           _typeName,
                                               classGlobal.CONVERT_UTC_TO_LOCAL(_recordTimeIn),
                                                   classGlobal.CONVERT_UTC_TO_LOCAL(x["recordTimeOut"].ToString()),
                                                       timeStamp,
                                                           _visitorNumber,
                                                                strImage1, strImage2, strImage3, strImage4,
                                                                    visitorFrom, licensePlate, name, contactPlace, citizenId, etc, vehicleType,
                                                                        follower, department, visitPerson, contactTopic);

                        n = n + 1;
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();
            }

            return listLogs;
        }       
        #endregion

        #region เกี่ยวกับการ config ค่าที่จอดรถ        
        public static JObject GET_PAID_CHAGRE_CONFIG()
        {

            List<string> visitorTypeList = new List<string>();
            string jsonString = ClassData.GET_METHODE("visitorType");
            if (jsonString != "")
            {
                JArray jsArray = JArray.Parse(jsonString);
                foreach (var x in jsArray)
                {
                    visitorTypeList.Add(x.ToString());
                }
            }

            DataTable dt = new DataTable("dtPaidConfig");

            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            JToken jMessage;
            JObject jsonObject = new JObject();
            string postData = "";
            try
            {

                byte[] data = Encoding.UTF8.GetBytes(postData);
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "cost" + "?_id=" + classGlobal.userId + "&_page=" + page + "&_limit=" + limit_per_page);
                request.Method = "GET";
                request.Headers.Add("Authorization", classGlobal.access_token);
                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                jMessage = json["message"];
                JArray ja = (JArray)jMessage["result"];

                string costId = "";
                string typeId = "";
                string visitorType = "";
                string classType = "";  // A B C D E
                string minutes = "";
                string rate = "";
                string status = ""; // Y N
                JArray jaCost = new JArray();
                List<string> alCostTime = new List<string>();
                List<string> alCostRate = new List<string>();
                string[] temp = new string[0];

                dt.Columns.Add("typeId");
                dt.Columns.Add("typename");
                dt.Columns.Add("costId");
                dt.Columns.Add("class");
                dt.Columns.Add("minutes");
                dt.Columns.Add("rate");
                dt.Columns.Add("status");

                List<string> lstVisitorType = new List<string>();
                foreach (var config in ja)    // main loop "visitorType"
                {
                    typeId = classGlobal.GetRandomHexNumber(24).ToLower();

                    costId = config["costId"].ToString();
                    visitorType = config["visitorType"].ToString();  // ทั่วไป
                    classType = config["costType"].ToString();

                    jaCost = (JArray)config["costTime"];
                    for (int i = 0; i < jaCost.Count; i++)
                        alCostTime.Add((String)jaCost[i]);
                    try
                    { alCostTime.Add((int.Parse((String)jaCost[jaCost.Count - 1]) + 1).ToString()); }
                    catch
                    { alCostTime.Add("0"); }
                    

                    temp = new string[0];
                    temp = (string[])alCostTime.ToArray();
                    minutes = String.Join(";", temp);
                    alCostTime.Clear();

                    jaCost = (JArray)config["costRate"];
                    for (int i = 0; i < jaCost.Count; i++)
                        alCostRate.Add((String)jaCost[i]);
                    try
                    {  alCostRate.Add(config["fine"].ToString()); }
                    catch
                    { alCostRate.Add("0"); }
                    

                    temp = new string[0];
                    temp = (string[])alCostRate.ToArray();
                    rate = String.Join(";", temp);
                    alCostRate.Clear();

                    status = config["checkoutStatus"].ToString().Replace("true", "Y").Replace("false", "N");

                    dt.Rows.Add(typeId, visitorType, costId, classType, minutes, rate, status);

                    if (lstVisitorType.IndexOf(visitorType) == -1)
                        lstVisitorType.Add(visitorType);
                }

                try
                {
                    jsonObject = new JObject();
                    if (dt.Rows.Count > 0 )
                    {
                        DataTable newDataTable = dt.AsEnumerable().OrderBy(r => r.Field<string>("typename")).ThenBy(r => r.Field<string>("class")).CopyToDataTable();
                        dt = newDataTable.Copy();
                    }
                    
                    JArray jArrayMain = new JArray();
                    JObject jsMain = new JObject();

                    foreach (var itms in lstVisitorType)
                    {
                        DataRow[] found = dt.Select("typename='" + itms + "'");
                        JArray classnameAr = new JArray();
                        JObject classnameOj = new JObject();

                        typeId = found[0]["typeId"].ToString();

                        for (int r = 0; r < found.Length; r++)
                        {  // typeId   typename  costId  class  minutes  rate status

                            JObject jo = new JObject();
                            jo.Add("costId", found[r]["costId"].ToString());
                            jo.Add("class", found[r]["class"].ToString());
                            jo.Add("minutes", found[r]["minutes"].ToString());
                            jo.Add("rate", found[r]["rate"].ToString());
                            jo.Add("status", found[r]["status"].ToString());

                            classnameAr.Add(jo);
                        }

                        jsMain.Add("typeId", typeId);
                        jsMain.Add("typename", itms);
                        jsMain.Add("classname", classnameAr);
                        jArrayMain.Add(jsMain);
                        jsMain = new JObject();
                    }

                    jsonObject.Add("visitorType", jArrayMain);

                }
                catch (WebException ex)
                {
                    HttpWebResponse res = ex.Response as HttpWebResponse;
                    string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                    JObject jsRes = JObject.Parse(resString);
                    string status_ex = jsRes["status"].ToString();
                    string message = jsRes["message"].ToString();

                    jsonObject = new JObject();
                }


            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status_ex = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                jsonObject = new JObject();
            }

            return jsonObject;
        }
 
        public static void POST_PUT_PAID_CHARGE_CONFIG(string access_token, string userId, string classType, 
                                                            string visitorType, string minutes, string rate, 
                                                                string status, string costId, string methode, string fine)
        {

            minutes = minutes.Replace(";", ",");
            rate = rate.Replace(";", ",");

            string[] _p = minutes.Split(',');
            string[] _p1 = rate.Split(',');

            HttpWebRequest request = null;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            string postData = "";
            try
            {

                if (methode == "POST")
                {
                    request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "cost");
                    postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);
                    postData += "&visitorType=" + Uri.EscapeDataString(visitorType);
                    postData += "&costType=" + Uri.EscapeDataString(classType);
                    postData += "&costTime=" + Uri.EscapeDataString(minutes);
                    postData += "&costRate=" + Uri.EscapeDataString(rate);
                    postData += "&fine=" + Uri.EscapeDataString(fine);
                    postData += "&checkoutStatus=" + Uri.EscapeDataString(status);
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "updateCost");
                    postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);
                    postData += "&visitorType=" + Uri.EscapeDataString(visitorType);
                    postData += "&costId=" + Uri.EscapeDataString(costId);
                    postData += "&costType=" + Uri.EscapeDataString(classType);
                    postData += "&costTime=" + Uri.EscapeDataString(minutes);
                    postData += "&costRate=" + Uri.EscapeDataString(rate);
                    postData += "&fine=" + Uri.EscapeDataString(fine);
                    postData += "&checkoutStatus=" + Uri.EscapeDataString(status);
                }

                byte[] data = Encoding.UTF8.GetBytes(postData);

                request.Method = methode;
                request.Headers.Add("Authorization", classGlobal.access_token);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);

            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status_ex = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();
            }
        }
        #endregion

        #region เกี่ยวกับการ get logs การจ่ายค่าที่จอดรถ  
        public static DataTable GET_CHARGE_LOGS(bool getBefore, string yyMMdd = "")
        {
            page = "1";

            DataTable paidLogs = new DataTable("paidLogs");
            paidLogs.Columns.Add("visitorId");
            paidLogs.Columns.Add("minutes");
            paidLogs.Columns.Add("charge");
            paidLogs.Columns.Add("extra");
            paidLogs.Columns.Add("paid");
            paidLogs.Columns.Add("charge_type");
            paidLogs.Columns.Add("timestamp");

            paidLogs.Columns.Add("visitorNumber");
            paidLogs.Columns.Add("visitorType");
            paidLogs.Columns.Add("recordTimeIn");
            paidLogs.Columns.Add("recordTimeOut");
            paidLogs.Columns.Add("contactPlace");


            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            string postData = "";
            JObject json;
            JToken jMessage;
            try
            {
                System.Windows.Forms.Application.DoEvents();   

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                CultureInfo MyCultureInfo = new CultureInfo("en-US");
                string filterDate = DateTime.Now.ToString("yyyyMMdd");
                if (yyMMdd != "")
                    filterDate = yyMMdd;

                for (int _page = 1; _page <= pageLooping; _page++)
                {

                    string parameter = "&_time=" + filterDate + "&_before=" + getBefore.ToString().ToLower() + "&_page=" + _page + "&_limit=" + limit_per_page + "&_sort=-1";

                    byte[] data = Encoding.UTF8.GetBytes(postData);
                    request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "visitorLog" + "?_id=" + classGlobal.userId + parameter);
                    request.Method = "GET";
                    request.Headers.Add("Authorization", classGlobal.access_token);
                    response = (HttpWebResponse)request.GetResponse();
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    json = JObject.Parse(responseString);

                    jMessage = json["message"];
                    JObject jsMessage = (JObject)jMessage;
                    JArray jsArray = (JArray)jsMessage["result"];
                    string timeStamp = "";
                    string visitorId = "";
                    string minutes = "";
                    string charge = "";
                    string extra = "";
                    string paid = "";
                    string chargetype = "";

                    string visitorNumber = "";
                    string visitorType = "";
                    string recordTimeIn = "";
                    string recordTimeOut = "";
                    string contactPlace = "";


                    foreach (var x in jsArray)
                    {
                        visitorId = x["visitorId"].ToString();
                        timeStamp = x["cost"]["timestamp"].ToString();
                        if (timeStamp != "")
                        {
                            timeStamp = classGlobal.CONVERT_UTC_TO_LOCAL(x["cost"]["timestamp"].ToString()).Substring(0, 10);
                            minutes = x["cost"]["totalMinute"].ToString();
                            charge = x["cost"]["totalExpenses"].ToString();
                            extra = x["cost"]["extraCost"].ToString();
                            paid = x["cost"]["paymentStatus"].ToString();
                            chargetype = x["cost"]["costType"].ToString();


                            visitorNumber = x["visitorNumber"].ToString();
                            visitorType = x["visitorType"].ToString();
                            recordTimeIn = classGlobal.CONVERT_UTC_TO_LOCAL(x["recordTimeIn"].ToString());
                            recordTimeOut = classGlobal.CONVERT_UTC_TO_LOCAL(x["recordTimeOut"].ToString());
                            contactPlace = x["contactPlace"].ToString();

                            paidLogs.Rows.Add(visitorId, minutes, charge, extra, paid, chargetype, timeStamp,
                                visitorNumber, visitorType, recordTimeIn, recordTimeOut, contactPlace);
                        }

                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status_ex = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();
            }

            return paidLogs;
        }
        public static string POST_CHARGE_LOGS(string minutes, string charge, string extra, string paid, string charge_type)
        {

            HttpWebRequest request = null;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            string postData = "";
            string stringsReturn = "";


            request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL +"costUp");
            postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);
            postData += "&visitorId=" + Uri.EscapeDataString(classGlobal.visitorId_for_out_event);
            postData += "&costType=" + Uri.EscapeDataString(charge_type);
            postData += "&totalMinute=" + Uri.EscapeDataString(minutes);
            postData += "&extraCost=" + Uri.EscapeDataString(extra);
            postData += "&totalExpenses=" + Uri.EscapeDataString(charge);
            postData += "&paymentStatus=" + Uri.EscapeDataString(paid);

            byte[] data = Encoding.UTF8.GetBytes(postData);

            request.Method = "POST";
            request.Headers.Add("Authorization", classGlobal.access_token);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
                stream.Write(data, 0, data.Length);

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                stringsReturn = json["status"].ToString();

            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status_ex = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                stringsReturn = message;
            }
           
            return stringsReturn;            
        }
        #endregion

        #region ระบบ นัดหมาย ผู้มาติดต่อ  บันทึก 1.ชื่อ-นามสกุล 2.ทะเบียนรถ 3.เวลาเข้าพบ เพื่อ สะดวก ในการ ตรวจสอบความปลอดภัย และ สำรอง ที่จอดรถ         
        public static void CHECK_APPOINTMENT(string personID, string personName, string place = "", string licensePlate = "")
        {
            //return;

            if (classGlobal.userId != "")
            {
                if (personID.Contains("#") || personName.Contains("#"))
                    return;
                if ((personID + personName + licensePlate + place).Replace(" ", "") == "")
                    return;
                if ((personID != "") && (personName + licensePlate + place).Replace(" ", "") == "")
                    return;

                DataTable table = GET_APPOINTMENT();   // ข้อมมูลในดาต้าเบส ที่ user ตั้งค่า
                if (table.Rows.Count > 0)
                {
                    personName = personName.Replace("นาย", "").Replace("นางสาว", "").Replace("นาง", "");
                    string filter = "(fullName LIKE '%" + personName + "%'" + " AND " +
                                        "place LIKE '%" + place + "%'" + " AND " +
                                            "licensePlate LIKE '%" + licensePlate + "%'" + ") AND " +
                                                "(appointStatus = 'นัด')";

                    DataRow[] dr = table.Select(filter);
                    DataTable dtFound = new DataTable("dt");
                    dtFound.Columns.Add("_id");
                    dtFound.Columns.Add("fullName");
                    dtFound.Columns.Add("licensePlate");
                    dtFound.Columns.Add("appointTime");
                    dtFound.Columns.Add("appointPlace");
                    for (int i = 0; i < dr.Length; i++)
                    {
                        string localTime = classGlobal.CONVERT_UTC_TO_LOCAL(dr[i]["appointTime"].ToString());
                        string appStamp = localTime.Substring(0, 10);
                        string[] _date = appStamp.Split('-');
                        string dateSlash = _date[2] + "/" + _date[1] + "/" + (int.Parse(_date[0]) + 543).ToString();
                        string[] _time = localTime.Split(' ');
                        appStamp = dateSlash + " " + _time[1];

                        dtFound.Rows.Add(dr[i]["_id"], 
                                                dr[i]["fullName"].ToString(),
                                                    dr[i]["licensePlate"].ToString(),
                                                        appStamp,
                                                            dr[i]["place"].ToString());  

                    }
                    if ((dtFound.Rows.Count > 0) && (classGlobal.appointMentSelectedId == ""))
                    {
                        MsgLongInfo msgInfo = new MsgLongInfo(dtFound);
                        msgInfo.ShowDialog();
                    }
                    
                }

            }
        }

        public static JArray jaAppointment = new JArray();
        public static void POST_PUT_APPOINTMENT(JArray ja, string method)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            string postData = "";
            string stringsReturn = "";
            try
            {
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "meeting");
                postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);

                if (method == "PUT")
                    postData += "&meetingId=" + Uri.EscapeDataString(classGlobal.appointMentSelectedId);

                postData += "&uId=" + Uri.EscapeDataString(classGlobal.uId);
                postData += "&name=" + Uri.EscapeDataString(ja[0]["name"].ToString());
                postData += "&meetUpLocal=" + Uri.EscapeDataString(ja[0]["meetUpLocal"].ToString());
                postData += "&daysToCome=" + Uri.EscapeDataString(ja[0]["daysToCome"].ToString());
                postData += "&licensePlate=" + Uri.EscapeDataString(ja[0]["licensePlate"].ToString());
                postData += "&status=" + Uri.EscapeDataString(ja[0]["status"].ToString());

                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.Method = method; //"POST";
                request.Headers.Add("Authorization", classGlobal.access_token);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                stringsReturn = json["status"].ToString();
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                stringsReturn = ex.Message.ToString();
            }
        }        
        public static DataTable GET_APPOINTMENT()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("_id");
            dt.Columns.Add("fullName");
            dt.Columns.Add("place");
            dt.Columns.Add("licensePlate");
            dt.Columns.Add("appointTime");
            dt.Columns.Add("appointStatus");

            //*************************//
            //===> GET DATAFROM SERVER  
            HttpWebRequest request;
            HttpWebResponse response;
            JObject json;
            string postData = "";
            JToken jMessage;
            string responseString = "";
            try
            {

                byte[] data = Encoding.UTF8.GetBytes(postData);
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + 
                                                                        "meeting" + "/" + 
                                                                            classGlobal.userId + "?uId=" + classGlobal.uId + 
                                                                                "&_page=1&_limit=100&_sort=1");
                request.Method = "GET";
                request.Headers.Add("Authorization", classGlobal.access_token);
                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);

                jMessage = json["message"];
                JObject jsMessage = (JObject)jMessage;
                JArray jsArray = (JArray)jsMessage["result"];
                jaAppointment = jsArray;
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                jaAppointment = new JArray();
            }

            foreach (JObject jo in jaAppointment)
            {
                dt.Rows.Add(jo["meetingId"].ToString(),
                            jo["name"].ToString(),
                            jo["meetUpLocal"].ToString(),
                            jo["licensePlate"].ToString(),
                            jo["daysToCome"].ToString(),
                            jo["status"].ToString());
            }

            //dt.DefaultView.Sort = "daysToCome ASC";
            dt = dt.DefaultView.ToTable();

            return dt;
        }
        public static void DELETE_APPONITMENT(string meetingId)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            string postData = "";
            string stringsReturn = "";
            try
            {
                request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "meeting");
                postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);
                postData += "&meetingId=" + Uri.EscapeDataString(meetingId);

                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.Method = "DELETE";
                request.Headers.Add("Authorization", classGlobal.access_token);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                stringsReturn = json["status"].ToString();
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                stringsReturn = ex.Message.ToString();
            }
        }

        public static string generate_object_id()
        {
            System.Threading.Thread.Sleep(10); 
            Random random = new Random();
            int digits = 24;
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result.ToString().ToLower();

            string res = result + random.Next(16).ToString("X");
            return res.ToString().ToLower();
        }

        public static void UPDATE_APPOINTMENT_STATUS(string appointmentId, string status)
        {
            //อัพเดทสถานะใน visitorInfo ให้เป็น "มา" (Default จากการสร้างการนัดจะเป็นสถานะ "นัด")

            DataTable table = ClassData.GET_APPOINTMENT();

            DataRow[] foundRows = table.Select("_id='" + appointmentId + "'");
            if (foundRows.Length > 0)
            {
                String localTime = classGlobal.CONVERT_UTC_TO_LOCAL(foundRows[0]["appointTime"].ToString());
                string appStamp = classGlobal.CONVERT_LOCAL_TO_UTC(localTime);

                JObject jo = new JObject();
                jo.Add("name", foundRows[0]["fullName"].ToString());
                jo.Add("meetUpLocal", foundRows[0]["place"].ToString());
                jo.Add("licensePlate", foundRows[0]["licensePlate"].ToString());
                jo.Add("daysToCome", appStamp);
                jo.Add("status", "มา");

                JArray jaData = new JArray();
                jaData.Add(jo);
                ClassData.POST_PUT_APPOINTMENT(jaData, "PUT");  //===> POST SAVE TO SERVER 
            }

        }
        #endregion

        #region เกี่ยวกับภาพ LOGO
        public static string GET_LOGO(ref string name)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            string responseString = "";
            JObject json;
            JToken jMessage;

            request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "logo" + @"/" + classGlobal.userId);
            request.Method = "GET";
            request.Headers.Add("Authorization", classGlobal.access_token);
            response = (HttpWebResponse)request.GetResponse();
            responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            json = JObject.Parse(responseString);
            JObject jsMessage;
            try
            {
                jMessage = json["message"];
                jsMessage = (JObject)jMessage;
            }
            catch
            {
                return "";
            }

            JArray jsArray = (JArray)jsMessage["data"];
            string imageLogo = "";
            foreach (var x in jsArray)
            {
                imageLogo = x["imageLogo"][0].ToString();
                name = x["name"].ToString();
                break;
            }

            string webpname = "logo";
            string base64 = "";
            string output = Directory.GetCurrentDirectory() + @"\" + classGlobal.storageImages + @"\" + webpname + ".webp";
            output = Path.GetTempPath() + webpname + ".webp";  // change location to path of the current user's temporary folder.
            output = output.Replace(".webp.webp", ".webp");
            try
            {
                WebClient client = new WebClient();
                client.UseDefaultCredentials = false;
                client.Headers.Add("Authorization", classGlobal.access_token);
                client.DownloadFile(new Uri(classGlobal.API_URL + "view/logo/" + imageLogo), output);
                base64 = clsImage.CONVERT_WEBP_TO_PNG(output, Directory.GetCurrentDirectory() + @"\" + classGlobal.storageImages + @"\" + webpname + ".png");
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                base64 = "";
            }

            return base64;
        }

        #endregion







        #region DELIVERY PACKAGE
        public static string DELIVERY_PACKAGE(
            string parcelId,
            string parcelImage, 
            string receiverImage, 
            string room, 
            string description, 
            string status_,
            string method)
        {
            string stringsReturn = "";
            Uri webService = new Uri(classGlobal.API_URL + "deliver");
            HttpRequestMessage requestMessage;
            if (method=="POST")
                requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
            else
                requestMessage = new HttpRequestMessage(HttpMethod.Put, webService);

            requestMessage.Headers.Add("Authorization", classGlobal.access_token);
            requestMessage.Headers.ExpectContinue = false;
            MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
            multiPartContent.Add(new StringContent(classGlobal.userId), "userId");
            if (method == "PUT")
                multiPartContent.Add(new StringContent(parcelId), "parcelId");
            multiPartContent.Add(new StringContent(room), "room");
            multiPartContent.Add(new StringContent(description), "description");
            multiPartContent.Add(new StringContent(status_), "status");

            //++ add image jpg            
            if (parcelImage != "")
            {
                FileInfo fi = new FileInfo(parcelImage);
                string fileName = fi.Name;
                byte[] fileContents = File.ReadAllBytes(fi.FullName);
                ByteArrayContent byteArrayContent = new ByteArrayContent(fileContents);
                byteArrayContent.Headers.Add("Content-Type", MimeType(fileName));
                multiPartContent.Add(byteArrayContent, "parcelImage", fileName);
            }
            else
            {
                multiPartContent.Add(new StringContent(""), "parcelImage");
            }

            if (receiverImage != "")
            {
                FileInfo fi = new FileInfo(receiverImage);
                string fileName = fi.Name;
                byte[] fileContents = File.ReadAllBytes(fi.FullName);
                ByteArrayContent byteArrayContent = new ByteArrayContent(fileContents);
                byteArrayContent.Headers.Add("Content-Type", MimeType(fileName));
                multiPartContent.Add(byteArrayContent, "receiverImage", fileName);
            }
            else
            {
                multiPartContent.Add(new StringContent(""), "receiverImage");
            }
            //-------------------

            requestMessage.Content = multiPartContent;
            HttpClient httpClient = new HttpClient();
            try
            {
                Task<HttpResponseMessage> httpRequest = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, System.Threading.CancellationToken.None);
                HttpResponseMessage httpResponse = httpRequest.Result;
                HttpStatusCode statusCode = httpResponse.StatusCode;
                HttpContent responseContent = httpResponse.Content;
                if (responseContent != null)
                {
                    Task<String> stringContentsTask = responseContent.ReadAsStringAsync();
                    String stringContents = stringContentsTask.Result;   //{"status":200,"message":"complete"}
                    JObject json = JObject.Parse(stringContents);
                    stringsReturn = json["status"].ToString();
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status = jsRes["status"].ToString();
                string message = jsRes["message"].ToString();

                stringsReturn = message;
            }

            return stringsReturn;
        }
        #endregion

        #region DELETE LOGS
        public static bool DELETE_LOGS(string _time)  // _time = yyyyMMdd
        {
            bool return_flag = false;
            HttpWebRequest request = null;
            HttpWebResponse response;
            JObject json;
            string postData = "";

            request = (HttpWebRequest)WebRequest.Create(classGlobal.API_URL + "delLog");
            postData = "userId=" + Uri.EscapeDataString(classGlobal.userId);
            postData += "&_time=" + Uri.EscapeDataString(_time);

            byte[] data = Encoding.UTF8.GetBytes(postData);

            request.Method = "DELETE";
            request.Headers.Add("Authorization", classGlobal.access_token);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
                stream.Write(data, 0, data.Length);

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JObject.Parse(responseString);
                string stringsReturn = json["status"].ToString();

                return_flag = true;
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;                
                string resString = new StreamReader(res.GetResponseStream()).ReadToEnd();
                JObject jsRes = JObject.Parse(resString);
                string status_ex = jsRes["status"].ToString();
                string message = jsRes["message"].ToString() + " " + _time;

                classGlobal.deleteLogErrorDate.Add(_time); 

                return_flag = false;
            }

            return return_flag;
        }
        #endregion

    }

}

