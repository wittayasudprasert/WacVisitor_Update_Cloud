using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web.Services.Description;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Web.Services.Protocols;
using System.Reflection;
using System.Data;
using iFinTechIDCard;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Data.OleDb;
using PCSC;
using PCSC.Iso7816;
using System.IO;
using ClassHelper;
using System.Collections;
using System.Net.Http;
using System.Drawing;

using Npgsql;
using System.Security.Cryptography;
using System.IO.Ports;
using System.Management;
using System.Net;
using System.Dynamic;
using System.Drawing.Printing;
using System.Diagnostics;

namespace WacVisitor
{
    public class classGlobal
    {
        public static bool EnableAppointmentMenu = false;
        public static string strBirthDateInLicensePlate = "";
        public static int CheckAge = 0;
        public static bool textBox_CardId_Name_Readonly = true;  // textbox เลขบัตรประชาชน กับ ชื่อ-สกุล อ่านได้อย่างเดียว (read only)

        public static string appointMentSelectedId = "";
        public static bool statusCheckIN = false;

        public static string settingFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "VISITORPASS");
        public static string config = AppDomain.CurrentDomain.BaseDirectory + @"config.xml"; //Path.Combine(settingFile, "config.xml");

        public static bool fastCheckOut = true;
        public static string logInAlive = "";
        public static string sortOrderBy = "asc";  // asc / desc

        public static List<string> deleteLogErrorDate = new List<string>(0) { };

        public static bool enableAuthenticationConfirm = true;   // ต้องมีการยืนยัน user / password เพื่อเข้าใช้งานหรือไม่ true = ต้อง authen ; false = ไม่ต้อง

        //public static bool boolPGUI = true;  // requirement ของพี่กุ่ย ชื่อ-สกุล/เลขบัตรปชช ในสลิปให้เป็น ##### กับ มี dialog ยืนยันรหัสก่อนแสดงรายงาน
        public static bool bAuthenReportPass = false;
        public static string usrAuthenReport = "wacinfotech";

        public static string visitorId_for_out_event = "";
        public static JObject public_JsonChargePark = new JObject();
        
        public static string minutesDafault = "0;60;120;180;240;300;360;420;480;540;600;660;720;780;840;900;960;1020;1080;1140;1200;1260;1320;1380;1440;1500;1560;1620;1680;1740;1800;1860;1920;1980;2040;2100;2160;2220;2280;2340;2400;2460;2520;2580;2640;2700;2760;2820;2880;2940;3000;3060;3120;3180;3240;3300;3360;3420;3480;3540;3600;3660;3720;3780;3840;3900;3960;4020;4080;4140;4200;4260;4320";
        public static string rateDefault = "0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0";
        public static List<string> lstClassType = new List<string>(5) { "A", "B", "C", "D", "E" }; 

        public static ArrayList arrayListVisitorId = new ArrayList(); 
        public static string public_visitorId = "";
        public static DataTable dtLogsPaging;

        public static DataTable dtAllLogs;
        public static DataTable dtAllLogsView;

        public static string DATABASENANE = "wac_visitor";
        public static string DATABASELOCATION = AppDomain.CurrentDomain.BaseDirectory + @"database\";  // DATABASELOCATION = @"\\10.0.0.46\database\"; 

        #region declare variable

        public static int timerClearIn = 3;
        public static int timerClearOut = 5; //20

        public static bool enableTimeAttendance = false;  // ถ้าเป็น cloud version ไม่ใช้ TimeAttendance เซตให้เป็น false 

        #region สำหรับ version TimeAttendance ลูกค้าของพี่กุ่ย (Sritrang ที่เดียว)
        public static string firstFixDigit = "31"; // Fix ='31' ทุก Company
        public static string sIN = "0001";
        public static string sOUT = "0002";
        public static string terminalId = "0000";
        public static string firstTwoYearDigit = "20"; // 2 หลักแรกของปี ค.ศ.
        public static string STA_Mode = "";  // S01 : STA FAMILY   S02 : STA FRIEND                                                
        #endregion

        public static string public_strIN = "";
        public static string public_GuestName = "";

        public static string text = "";

        public static string class_charge = "A";

        public static int pub_MinuteCharge = 0;

        //++
        public static bool FactoryVersion = true;    // version โรงงานของหนุ่ม
        public static bool boolCharge = false;    // เปิดใช้ฟังก์ชั่นคิดเงินหรือไม่
        //--

        public static string strRandomNumber = "";
        public static string strNumpad = "";

        public static string databaseType = "acc";  //acc   psql   cloud
        public static string strBarcodeQrCordText { get; set; }

        public static string accountUsr = "";
        public static string accountPwd = "";
        public static string MachineAddress { get; set; }

        public static string printThermalName = @"XP";   //@"POS-80C" OR @"POS-80"  OR @"POS"
        public static string smartCardReaderName = @"ACS@ACR1281@1S@Dual@Reader@ICC@0";   //ACS@ACR1281@1S@Dual@Reader@ICC@0  // Generic@EMV@Smartcard@Reader@0

        public static string commportName = @"COM8";
        
        public static IDCard mIdCard;

        public static bool addEventSmartcard = false;

        public static bool PlustekDeviceConnect = false;
        public static bool SmartcardDeviceConnect = false;
        public static bool WebCamConnect = false;

        public static bool filterDateNow = false;
        public static int pub_id { get; set; }
        public static IDCardProfile pub_personal { get; set; }
        public static JToken pub_passport { get; set; }
                
        public static string pub_query { get; set; }
        public static string view_status { get; set; } 

        public static string click4scan = AppDomain.CurrentDomain.BaseDirectory + @"icon\click4scan.png";
        public static string click4capture = AppDomain.CurrentDomain.BaseDirectory + @"icon\click4capture.png";
        public static string notfoundscanner = AppDomain.CurrentDomain.BaseDirectory + @"icon\notfoundscanner.png";
        public static string notfoundwebcam = AppDomain.CurrentDomain.BaseDirectory + @"icon\notfoundwebcam.png";
        public static string unknown = AppDomain.CurrentDomain.BaseDirectory + @"icon\unknown.png";

        public static string strPlace = "";
        public static string strTitle = "";
        public static string strBlackOrWhiteList = "";

        public static string Reprint_text = "";
        public static string Reprint_card_number = "";
        public static int Reprint_bc_qr_location = 0;

        public static string Reprint_text1 = "";
        public static string Reprint_text2 = "";
        public static string Reprint_text3 = "";
        public static Bitmap Reprint_Bitmap = null;

        public static float pubWidth = 200.0F;  // 100.0F - 300.0F
        public static float pubHeight = 200.0F; // 100.0F - 300.0F

        public static string visitor_type_name { get; set; } 
        public static DataTable dtVisitor { get; set; }
        public static string WebcamBase64String { get; set; }
        public static string WebcamBase64String1 { get; set; }
        public static string WebcamBase64String2 { get; set; }

        public static string cam { get; set; }
        public static string PlustekBase64String { get; set; }
        public static string statusIn_OUT { get; set; }
        public static bool bool_Exit { get; set; }
        public static bool status_online { get; set; }
        public static string strCompanyID { get; set; }

        public static int SelectedWebcamDevice = -1;

        public static ArrayList arrVehicleInfo = new ArrayList();

        public static string MsgText = "";
        public static string MsgConfirm = "";

        public static string destinationNotification = "";

        public static string PreviewPrintIN = "";

        public static string visitorStatus = "normal";  //normal  whitelist  blacklist 

        public static bool boolStatuteSave = false;
        public static bool DisplayHashTag = false;   //boolStatuteDisplay
        public static string watermark = "แลกบัตรผู้มาเข้าพบ";

        public static string personID = "";
        public static string personName = "";

        public static bool boolX50 = false;
        public static bool boolWatcherActive = false;

        public static string API_URL = "https://api.visitors.wacappcloud.com/";
        //public static string API_URL = "http://10.0.0.205:4004/";
        public static string userName = "";
        public static string passWord = "";


        public static string uId = "";
        public static string userId = "";
        public static string refresh_token = "";
        public static string access_token = "";
        public static string rule = "";
 
        public static string ipcamstoragePath = "";

        public static string storageImages = "storageImages";
        public static string webcam = "webcam";

        public static string[] monthTextShort = new string[13] { "-", "ม.ค.", "ก.พ.", "มี.ค.", "เม.ย.", "พ.ค.", "มิ.ย.", "ก.ค.", "ส.ค.", "ก.ย.", "ต.ค.", "พ.ย.", "ธ.ค." };
        public static string[] monthTextLong = new string[13] { "-", "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน",
                                                                        "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม" };

        #endregion


        public static Bitmap ResizeImageRatio(Bitmap image, float width, float height)
        {
            try
            {
                //float width = 100;
                //float height = 100;
                var brush = new SolidBrush(Color.Transparent);

                float scale = Math.Min(width / image.Width, height / image.Height);


                var bmp = new Bitmap((int)width, (int)height);
                var graph = Graphics.FromImage(bmp);



                var scaleWidth = (int)(image.Width * scale);
                var scaleHeight = (int)(image.Height * scale);

                graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
                graph.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);

                return bmp;
            }
            catch
            {
                return image;
            }
        }      

        public static OleDbConnection conn = new OleDbConnection();
        public static NpgsqlConnection connP = new NpgsqlConnection();

       
        public static void ConnectToDatabase()
        {
            if (conn.State != ConnectionState.Closed)
                conn.Close();

            string strConnectionString = "";

            ClassHelper.clsXML cls = new clsXML();
            if (cls.CheckExistElement("root", "db", classGlobal.config) == false)
            {
                cls.AppendElement("root", "db", "acc", classGlobal.config);
            }
            databaseType = cls.GetReadXML("root", "db", classGlobal.config).ToLower();
            cls = null;

            if (databaseType == "acc") //ACC
            {
                strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                                            "Data source=" + DATABASELOCATION + DATABASENANE + ".mdb;" +
                                                "Jet OLEDB:Database Password=wacinfotech;";

                //strConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                //                            "Data source=" + DATABASELOCATION + DATABASENANE + ".mdb;" +
                //                                "Jet OLEDB:Database Password=wacinfotech;";

                 conn.ConnectionString = strConnectionString;
                try
                {
                    conn.Open();                    
                }
                catch(Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Failed to connect to data source", ex.Message.ToString());
                    Environment.Exit(0);  
                }
            }
            else if (databaseType == "psql") //SQL
            {
                cls = new clsXML();
                string serverIP = cls.GetReadXML("root", "ServerIP", AppDomain.CurrentDomain.BaseDirectory + @"db.xml");
                string port = cls.GetReadXML("root", "Port", AppDomain.CurrentDomain.BaseDirectory + @"db.xml");
                string db = cls.GetReadXML("root", "DB", AppDomain.CurrentDomain.BaseDirectory + @"db.xml");
                string usr = cls.GetReadXML("root", "User", AppDomain.CurrentDomain.BaseDirectory + @"db.xml");
                string pwd = cls.GetReadXML("root", "Password", AppDomain.CurrentDomain.BaseDirectory + @"db.xml");
                pwd = classGlobal.Decrypt(pwd);  
                //NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;User Id=postgres;" +
                //               "Password=password@1;Database=local;");
                strConnectionString = String.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};", serverIP, port, db, usr, pwd);
                cls = null;

                connP.ConnectionString = strConnectionString;
                try
                {
                    connP.Open();
                }
                catch (Exception ex)
                {
                    connP.Close();
                    System.Windows.Forms.MessageBox.Show("Failed to connect to data source", ex.Message.ToString());
                    Environment.Exit(0);  
                }
            }
            else  //CLOUD
            {
                ClassHelper.clsXML clsxml = new ClassHelper.clsXML();
                string enableTimeAtt = "false";  
                clsxml.ModifyElement("root", "timeattendance", "false", classGlobal.config); 
                enableTimeAtt = clsxml.GetReadXML("root", "timeattendance", classGlobal.config).ToString();
                classGlobal.enableTimeAttendance = bool.Parse(enableTimeAtt);
                clsxml = null;
            }
            
        }
        public static string YearTwoToFour(string tmpYear)
        {
            if (tmpYear.Length == 1)
            {
                tmpYear = "0" + tmpYear;
            }
            if (Convert.ToInt16(tmpYear) > 50)
            {
                tmpYear = String.Format("19{0}", tmpYear);
            }
            else
            {
                tmpYear = String.Format("20{0}", tmpYear);
            }
            return tmpYear;
        }
        public static string CheckYear(string strInput)
        {
            strInput = strInput.Replace(".", "/").Replace("-", "/").Replace(",", "/").Replace("//", "/");
            int minus = 0;
            string[] strCheckYear = new string[] { };
            int years = 0;
            try
            {
                strCheckYear = strInput.Split('/');
                if (strCheckYear.Length == 1)
                {
                    strInput = strInput.Substring(0, 2) + "/" + strInput.Substring(2, 2) + "/" + strInput.Substring(4, 4);
                    strCheckYear = strInput.Split('/');
                }

                years = int.Parse(strCheckYear[2]);

                if (years > 3000)
                { minus = -543; }

                string last4 = Regex.Match(strInput, @"(.{4})\s*$").ToString();
                int x = Int32.Parse(last4) + minus;
                return x.ToString();
            }
            catch
            {
                if (years == 0)
                {
                    return "";
                }
                else
                {
                    return years.ToString();
                }
            }

         }
        public static string YearEngToTha(string strInput)
        {
            strInput = strInput.Replace(".", "/").Replace("-", "/").Replace(",", "/").Replace("//", "/");
            int minus = 0;
            string[] strCheckYear = new string[] { };
            int years = 0;
            try
            {
                strCheckYear = strInput.Split('/');
                if (strCheckYear.Length == 1)
                {
                    strInput = strInput.Substring(0, 2) + "/" + strInput.Substring(2, 2) + "/" + strInput.Substring(4, 4);
                    strCheckYear = strInput.Split('/');
                }

                years = int.Parse(strCheckYear[2]);

                if (years < 2500)
                { minus = 543; }

                string last4 = Regex.Match(strInput, @"(.{4})\s*$").ToString();
                int x = Int32.Parse(last4) + minus;
                return strCheckYear[0] + "/" + strCheckYear[1] + "/" + x.ToString();
            }
            catch
            {
                return strInput;
            }

        }
        public static string ReadCardNfc(string readerName, bool uidOnly)
        {
            SCardContext context = new SCardContext();
            context.Establish(SCardScope.System);
            SCardReader reader = new SCardReader(context);

            SCardError result = reader.Connect(readerName, SCardShareMode.Shared , SCardProtocol.Any);  

            if (result != SCardError.Success)
            {
                context.Dispose();
                reader.Dispose();
                return string.Format("No card is detected (or reader reserved by another application){0}{1}",
                    Environment.NewLine, SCardHelper.StringifyError(result));
            }

            string[] readerNames; SCardProtocol protocol; SCardState state; byte[] atr;
            result = reader.Status(out readerNames, out state, out protocol, out atr);

            if (result != SCardError.Success)
            {
                context.Dispose();
                reader.Dispose();
                return string.Format("Unable to read from card.{0}{1}", Environment.NewLine, SCardHelper.StringifyError(result));
            }

            string message = string.Format("Card detected:{0}Protocol: {1}{0}State: {2}{0}ATR: {3}{0}",
                Environment.NewLine, protocol, state, BitConverter.ToString(atr ?? new byte[0]));

            CommandApdu apdu = new CommandApdu(IsoCase.Case2Short, reader.ActiveProtocol)
            {
                CLA = 0xFF,
                Instruction = InstructionCode.GetData,
                P1 = 0x00,
                P2 = 0x00,
                Le = 0
            };

            result = reader.BeginTransaction();

            if (result != SCardError.Success)
            {
                context.Dispose();
                reader.Dispose();
                return string.Format("Cannot start transaction.{0} {1}", Environment.NewLine, SCardHelper.StringifyError(result));
            }

            SCardPCI recievePci = new SCardPCI();
            IntPtr sendPci = SCardPCI.GetPci(reader.ActiveProtocol);

            byte[] recieveBuffer = new byte[256];

            result = reader.Transmit(sendPci, apdu.ToArray(), recievePci, ref recieveBuffer);

            if (result != SCardError.Success)
            {
                context.Dispose();
                reader.Dispose();
                return string.Format("Cannot transmit data.{0} {1}", Environment.NewLine, SCardHelper.StringifyError(result));
            }

            var responseApdu = new ResponseApdu(recieveBuffer, IsoCase.Case2Short, reader.ActiveProtocol);

            message += string.Format("SW1: {1}{0}SW2: {2}{0}", Environment.NewLine, responseApdu.SW1, responseApdu.SW2);

            string data = responseApdu.HasData ? BitConverter.ToString(responseApdu.GetData()) : "--";

            if (uidOnly)
            {
                context.Dispose();
                reader.Dispose();
                return string.Format("{0}", data);
            }

            message += string.Format("UID: {0}", data);

            reader.EndTransaction(SCardReaderDisposition.Leave);
            reader.Disconnect(SCardReaderDisposition.Reset);

            context.Dispose();
            reader.Dispose();
            return message;
        }

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        //***************************************************************************//
        //***************************************************************************//
        //***************************************************************************//
  
        public static Int32 COMPARE_BETWEEN_TIME_TO_MINUTES(string time_start, string time_end)
        {
            Int32 nMinutes = 0;
            try
            {
                //time_start = "2019-11-22 09:36:05";
                //time_end = "2019-11-22 09:36:54";
                DateTime startTime = DateTime.Parse(time_start);  
                DateTime endTime = DateTime.Parse(time_end);      

                TimeSpan span = endTime.Subtract(startTime);
                nMinutes = Convert.ToInt32(span.TotalMinutes);
            }
            catch
            {
                nMinutes = 0;
            }

            return nMinutes;            
        }
        public static Int32 VISITOR_TYPE_CHARGE_STATUS(string typename)
        {
            Int32 ret = 0;
            try
            {
                string query = "";
                //query = "SELECT ID FROM tbl_charge_car_park WHERE typename='" + typename + "'";
                query = "SELECT ID FROM tbl_charge_car_park WHERE typename='" + typename + "'" + " AND " + "status = 'Y'";
                DataTable dtID = new DataTable("typename");

                if (classGlobal.databaseType == "acc")
                {
                    OleDbDataAdapter ad = new OleDbDataAdapter(query, classGlobal.conn);
                    ad.Fill(dtID);
                    ad.Dispose();
                    ad = null;
                    ret = dtID.Rows.Count;
                    dtID.Dispose();
                    dtID = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlDataAdapter ad = new NpgsqlDataAdapter(query, classGlobal.connP);
                    ad.Fill(dtID);
                    ad.Dispose();
                    ad = null;
                    ret = dtID.Rows.Count;
                    dtID.Dispose();
                    dtID = null;
                }
                else
                {
                    dtID.Columns.Add("ID");
                    int _foundId = 1;
                    JToken jMessage = classGlobal.public_JsonChargePark["visitorType"];
                    foreach (var node in jMessage)
                    {
                        JArray ja = (JArray)node["classname"];
                        foreach (var nodeSub in ja)
                        {
                            if (node["typename"].ToString() == typename)
                            {
                                if (nodeSub["status"].ToString() == "Y")
                                {
                                    dtID.Rows.Add(_foundId);
                                    _foundId += 1;
                                }
                                else
                                {
                                    //dt.Clear();
                                    //break;
                                }
                            }
                        }
                    }

                    ret = dtID.Rows.Count;
                }
   
            }
            catch
            {
                ret = 0;
            }

            return ret;
        }
        public static int findClosest(int[] arr, int target)
        {
            int n = arr.Length;

            // Corner cases 
            if (target <= arr[0])
                return arr[0];
            if (target >= arr[n - 1])
                return arr[n - 1];

            // Doing binary search  
            int i = 0, j = n, mid = 0;
            while (i < j)
            {
                mid = (i + j) / 2;

                if (arr[mid] == target)
                    return arr[mid];

                /* If target is less  
                than array element, 
                then search in left */
                if (target < arr[mid])
                {

                    // If target is greater  
                    // than previous to mid,  
                    // return closest of two 
                    if (mid > 0 && target > arr[mid - 1])
                        return getClosest(arr[mid - 1],
                                     arr[mid], target);

                    /* Repeat for left half */
                    j = mid;
                }

                // If target is  
                // greater than mid 
                else
                {
                    if (mid < n - 1 && target < arr[mid + 1])
                        return getClosest(arr[mid],
                             arr[mid + 1], target);
                    i = mid + 1; // update i 
                }
            }

            // Only single element 
            // left after search 
            return arr[mid];
        }
        public static int getClosest(int val1, int val2, int target)
        {
            if (target - val1 >= val2 - target)
                return val2;
            else
                return val1;
        }

        public static string NUMBER_TO_MONTH(string input)
        {
            int i = Int32.Parse(input) -1 ;  
            string[] month = new string[12] { "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน", 
                                                "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม"};

            return month[i];
        }
        public static string NUMBER_TO_MONTH_SHORT(string input)
        {
            int i = Int32.Parse(input) - 1;
            string[] month = new string[12] { "ม.ค.", "ก.พ.", "มี.ค.", "เม.ย.", "พ.ค.", "มิ.ย.", 
                                                "ก.ค.", "ส.ค.", "ก.ย.", "ต.ค.", "พ.ย.", "ธ.ค."};

            return month[i];
        }
        public static Int32 MINUTES_BETWEEN_RANGE(int[] arr, int target)
        {
            int iResult = -1;

            if (target <= arr[0])  //ถ้าเวลาที่ใช้ไปมีค่าน้อยกว่าหรือเท่ากับ range แรก
            {
                return 0;
            }

            for (int i = 0; i < arr.Length; i++)
            {
                if ((i + 1) == arr.Length)  
                {
                    iResult = arr.Length - 1;  //ถ้าเวลาที่ใช้ไปมีค่ามากกว่าเวลาของค่าปรับ
                    break;
                }
                else
                {
                    if (target >= arr[i] && target <= arr[i + 1])  
                    {
                        iResult = i + 1;  // เอาค่า range เวลาปัดขึ้น ถ้าปัดลง ใช้ค่า i
                        break;
                    }
                }
            }
            return iResult;
        }

        #region MS ACCESS
        public static bool INSERT_MOREIN_FACTORY(int vid)
        {
            bool bResult = false;
            try
            {

                string query = String.Format("INSERT INTO tbl_moreinfo_factory (vid, follower, company, license_plate, vehicle_type, contact_to, department, topic, place, etc) VALUES ({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}')",
                                            classGlobal.pub_id,
                                            clsInfo.info_follower,
                                            clsInfo.info_visitor_company,
                                            clsInfo.info_license_plate,
                                            clsInfo.info_vehicle_type,
                                            clsInfo.info_visit_to,
                                            clsInfo.info_department,
                                            clsInfo.info_business_topic,
                                            clsInfo.info_place,
                                            clsInfo.info_etc);

                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else
                {
                    // do nothing
                }

               
                bResult = true;
            }
            catch
            {
                bResult = false;
            }

            return bResult;
        }
        public static void ALTER_TABLE_MAC_LOCATION()
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd = new OleDbCommand("SELECT TOP 1 mac_checkin FROM tbl_visitor", conn);
            bool exists = true;
            try
            {
                var x = cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                exists = false;
                Console.Write(e.Message.ToString()); 
            }
            cmd.Dispose();
            cmd = null;

            string query = "";
            if (exists == false)
            {
                query = "ALTER TABLE tbl_visitor ADD COLUMN [mac_checkin] TEXT(50)";
                if (classGlobal.databaseType == "psql")
                { query = "ALTER TABLE tbl_visitor ADD mac_checkin NVARCHAR(50)"; }
                cmd = new OleDbCommand(query, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                query = "ALTER TABLE tbl_visitor ADD COLUMN [mac_checkout] TEXT(50)";
                if (classGlobal.databaseType == "psql")
                { query = "ALTER TABLE tbl_visitor ADD mac_checkout NVARCHAR(50)"; }
                cmd = new OleDbCommand(query, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
        }
        public static void ALTER_TABLE_UPLOAD_SERVER()
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd = new OleDbCommand("SELECT TOP 1 upload FROM tbl_visitor", conn);
            bool exists = true;
            try
            {
                var x = cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                exists = false;
                Console.Write(e.Message.ToString()); 
            }
            cmd.Dispose();
            cmd = null;

            string query = "";
            if (exists == false)
            {
                query = "ALTER TABLE tbl_visitor ADD COLUMN [upload] TEXT(1)";
                if (classGlobal.databaseType == "psql")
                { query = "ALTER TABLE tbl_visitor ADD upload NVARCHAR(1)"; }
                cmd = new OleDbCommand(query, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
        }
        public static void ALTER_TABLE_VISITOR()
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd = new OleDbCommand("SELECT TOP 1 location FROM tbl_visitor", conn);
            bool exists = true;
            try
            {
                var x = cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                exists = false;
                Console.Write(e.Message.ToString()); 
            }
            cmd.Dispose();
            cmd = null;

            string query = "";
            if (exists == false)
            {
                query = "ALTER TABLE tbl_visitor ADD COLUMN [location] TEXT(50)";
                if (classGlobal.databaseType == "psql")
                { query = "ALTER TABLE tbl_visitor ADD location NVARCHAR(50)"; }
                cmd = new OleDbCommand(query, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }

            //+++ webcam 2 - 3
            cmd = new OleDbCommand("SELECT TOP 1 str_imagewebcamera1 FROM tbl_visitor", conn);
            exists = true;
            try
            {
                var x = cmd.ExecuteScalar();
            }
            catch (Exception e1)
            {
                exists = false;
                Console.Write(e1.Message.ToString()); 
            }
            cmd.Dispose();
            cmd = null;

            query = "";
            if (exists == false)
            {
                query = "ALTER TABLE tbl_visitor ADD COLUMN [str_imagewebcamera1] Memo ";
                cmd = new OleDbCommand(query, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }

            //---------------

            cmd = new OleDbCommand("SELECT TOP 1 str_imagewebcamera2 FROM tbl_visitor", conn);
            exists = true;
            try
            {
                var x = cmd.ExecuteScalar();
            }
            catch (Exception e1)
            {
                exists = false;
                Console.Write(e1.Message.ToString()); 
            }
            cmd.Dispose();
            cmd = null;

            query = "";
            if (exists == false)
            {
                query = "ALTER TABLE tbl_visitor ADD COLUMN [str_imagewebcamera2] Memo ";
                cmd = new OleDbCommand(query, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
            //--- webcam 2 - 3



        }
        public static void ALTER_TABLE_CHARGE_CAR_PARK()
        {
            OleDbCommand cmd;
            string query = "";

            try
            {
                query = "ALTER TABLE tbl_charge_car_park ALTER COLUMN [minutes] LONGTEXT";
                cmd = new OleDbCommand(query, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
            catch (Exception ex1)
            {
                Console.Write(ex1.Message.ToString());  
            }

            try
            {
                query = "ALTER TABLE tbl_charge_car_park ALTER COLUMN [rate] LONGTEXT";
                cmd = new OleDbCommand(query, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
            catch (Exception ex2)
            {
                Console.Write(ex2.Message.ToString()); 
            }

        }


        public static void CREATE_TABLE_IDCARD()
        {
            string query = "";
            query = "CREATE TABLE tbl_idcard " +
                    "(" +
                        "[id] NUMBER ," +
                        "[id_number] TEXT(50), " +
                        "[birthdate] TEXT(50), " +
                        "[gendar] TEXT(10), " +
                        "[th_title] TEXT(50), " +
                        "[th_firstname] TEXT(255), " +
                        "[th_lastname] TEXT(255), " +
                        "[en_title] TEXT(50), " +
                        "[en_firstname] TEXT(255), " +
                        "[en_lastname] TEXT(255), " +
                        "[issuedate] TEXT(50), " +
                        "[expiredate] TEXT(50), " +
                        "[house_no] TEXT(255), " +
                        "[village_no] TEXT(255), " +
                        "[lane] TEXT(255), " +
                        "[road] TEXT(255), " +
                        "[subdistrict] TEXT(255), " +
                        "[district] TEXT(255), " +
                        "[province] TEXT(255), " +
                        "[photo] Memo " +
                    ");";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }           
            cmd.Dispose();
            cmd = null;
        }
        public static void CREATE_TABLE_PASSPORT()
        {
            string query = "";
            query = "CREATE TABLE tbl_passport " +
                    "(" +
                        "[id] NUMBER ," +
                        "[DocumentNo] TEXT(50), " +
                        "[Familyname] TEXT(255), " +
                        "[Givenname] TEXT(255), " +
                        "[Birthday] TEXT(50), " +
                        "[PersonalNo] TEXT(50), " +
                        "[Nationality] TEXT(50), " +
                        "[Sex] TEXT(10), " +
                        "[Dateofexpiry] TEXT(50), " +
                        "[IssueState] TEXT(50), " +
                        "[NativeName] TEXT(50), " +
                        "[MRTDs] TEXT(255), " +
                        "[photo] Memo " +
                    ");";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            cmd.Dispose();
            cmd = null;
        }
        public static void CREATE_TABLE_TBL_USER()
        {
            string query = "";
            query = "CREATE TABLE tbl_user " +
                    "(" +
                        "[ID] AUTOINCREMENT PRIMARY KEY," +
                        "[USR] TEXT(50), " +
                        "[PWD] TEXT(50), " +
                        "[TYPE] TEXT(50) " +
                    ");";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                cmd = new OleDbCommand("INSERT INTO tbl_user (USR, PWD, TYPE) VALUES ('wacadmin','wacpassword','admin')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
            catch
            {

            }
        }
        public static void CREATE_TABLE_TBL_PERSONAL()
        {
            string query = "";
            query = "CREATE TABLE tbl_personal " +
                    "(" +
                        "[id] NUMBER ," +
                        "[id_number] TEXT(50), " +
                        "[fullname] TEXT(255) " +
                    ");";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }            
            cmd.Dispose();
            cmd = null;
        }
        public static void CREATE_TABLE_TBL_TYPE()
        {
            string query = "";
            query = "CREATE TABLE tbl_type " +
                    "(" +
                        "[typeid] NUMBER ," +
                        "[typename] TEXT(50) " +
                    ");";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                //++
                cmd = new OleDbCommand("INSERT INTO tbl_type (typeid, typename) VALUES (0,'ไม่ระบุ')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
                cmd = new OleDbCommand("INSERT INTO tbl_type (typeid, typename) VALUES (1,'ผู้รับเหมา')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
                cmd = new OleDbCommand("INSERT INTO tbl_type (typeid, typename) VALUES (2,'VIP')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
                cmd = new OleDbCommand("INSERT INTO tbl_type (typeid, typename) VALUES (3,'พนักงาน')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
                cmd = new OleDbCommand("INSERT INTO tbl_type (typeid, typename) VALUES (4,'ผู้ขาย')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
                cmd = new OleDbCommand("INSERT INTO tbl_type (typeid, typename) VALUES (5,'ทั่วไป')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
                //--
            }
            catch
            {

            }
        
        }
        public static void CREATE_TABLE_TBL_VISITOR()
        {
            string query = "";
            query = "CREATE TABLE tbl_visitor " +
                    "(" +
                        "[id] NUMBER ," +
                        "[object_idcompany] TEXT(50) ," +
                        "[card_number] TEXT(50) ," +
                        "[str_imagedocument] Memo ," +
                        "[str_imagewebcamera] Memo ," +
                        "[str_template] Memo ," +
                        "[status_in] TEXT(50) ," +
                        "[status_out] TEXT(50) ," +
                        "[typeid] NUMBER ," +
                        "[location] TEXT(50) " +
                    ");";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            cmd.Dispose();
            cmd = null;
        }
        public static void CREATE_TABLE_TBL_BLACKLIST()
        {
            string query = "";
            query = "CREATE TABLE tbl_blacklist " +
                    "(" +
                        "[ID] AUTOINCREMENT PRIMARY KEY," +
                        "[personal_number] TEXT(50), " +
                        "[fullname] TEXT(255), " +
                        "[start] TEXT(50), " +
                        "[stop] TEXT(50) " +
                    ");";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            cmd.Dispose();
            cmd = null;
        }
        public static void CREATE_TABLE_TBL_WHITELIST()
        {
            string query = "";
            query = "CREATE TABLE tbl_whitelist " +
                    "(" +
                        "[ID] AUTOINCREMENT PRIMARY KEY," +
                        "[personal_number] TEXT(50), " +
                        "[fullname] TEXT(255), " +
                        "[start] TEXT(50), " +
                        "[stop] TEXT(50) " +
                    ");";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            cmd.Dispose();
            cmd = null;
        }
        public static void CREATE_TABLE_TBL_MOREINFO()
        {
            string query = "";
            query = "CREATE TABLE tbl_moreinfo " +
                    "(" +
                       "[id] NUMBER ," +
                       "[place] TEXT(200), " +
                       "[register] TEXT(50), " +
                       "[v_nameType] TEXT(100), " +
                       "[etc] TEXT(255) " +
                    ");";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            cmd.Dispose();
            cmd = null;
        }
        public static void CREATE_TABLE_TBL_VEHICLE()
        {
            string query = "";
            query = "CREATE TABLE tbl_vehicle " +
                    "(" +
                        "[ID] AUTOINCREMENT PRIMARY KEY," +
                        "[v_id] TEXT(50), " +
                        "[v_nametype] TEXT(100) " +
                    ");";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                cmd = new OleDbCommand("INSERT INTO tbl_vehicle (v_id, v_nametype) VALUES ('1','รถยนต์')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                cmd = new OleDbCommand("INSERT INTO tbl_vehicle (v_id, v_nametype) VALUES ('2','รถจักรยานยนต์')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                cmd = new OleDbCommand("INSERT INTO tbl_vehicle (v_id, v_nametype) VALUES ('3','รถกระบะ')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                cmd = new OleDbCommand("INSERT INTO tbl_vehicle (v_id, v_nametype) VALUES ('4','รถTaxi')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                cmd = new OleDbCommand("INSERT INTO tbl_vehicle (v_id, v_nametype) VALUES ('5','อื่นๆ')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

            }
            catch
            {

            }
      
        }
        public static void CREATE_MOREINFO_FACTORY()
        {
            string tableName = "";
            bool tblExist = false;
            var schema = classGlobal.conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (var row in schema.Rows.OfType<DataRow>())
            {
                tableName = row.ItemArray[2].ToString();
                if (tableName == "tbl_moreinfo_factory")
                {
                    tblExist = true;
                    break;
                }
            }
            if (tblExist == false)
            {
                string query = "";
                query = "CREATE TABLE tbl_moreinfo_factory" +
                        "(" +
                            "[ID] AUTOINCREMENT PRIMARY KEY," +
                            "[vid] NUMBER ," +
                            "[follower] TEXT(20), " +
                            "[company] TEXT(100), " +
                            "[license_plate] TEXT(10), " +
                            "[vehicle_type] TEXT(100), " +
                            "[contact_to] TEXT(100), " +
                            "[department] TEXT(100), " +
                            "[topic] TEXT(200), " +
                            "[place] TEXT(100), " +
                            "[etc] TEXT(200) " +
                        ");";

                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
        }
        public static void CREATE_TABLE_VISITOR_COMPANY()
        {
            string tableName = "";
            bool tblExist = false;
            var schema = classGlobal.conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (var row in schema.Rows.OfType<DataRow>())
            {
                tableName = row.ItemArray[2].ToString();
                if (tableName == "tbl_visitor_company")
                {
                    tblExist = true;
                    break;
                }
            }
            if (tblExist == false)
            {
                string query = "";
                query = "CREATE TABLE tbl_visitor_company" +
                        "(" +
                            "[ID] AUTOINCREMENT PRIMARY KEY," +
                            "[visitor_company] TEXT(255) " +
                        ");";

                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
        }
        public static void CREATE_TABLE_LICENSE_PLATE()
        {
            string tableName = "";
            bool tblExist = false;
            var schema = classGlobal.conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (var row in schema.Rows.OfType<DataRow>())
            {
                tableName = row.ItemArray[2].ToString();
                if (tableName == "tbl_license_plate")
                {
                    tblExist = true;
                    break;
                }
            }
            if (tblExist == false)
            {
                string query = "";
                query = "CREATE TABLE tbl_license_plate" +
                        "(" +
                            "[ID] AUTOINCREMENT PRIMARY KEY," +
                            "[license_plate] TEXT(255) " +
                        ");";

                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
        }
        public static void CREATE_TABLE_VEHICLE_TYPE()
        {
            string tableName = "";
            bool tblExist = false;
            var schema = classGlobal.conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (var row in schema.Rows.OfType<DataRow>())
            {
                tableName = row.ItemArray[2].ToString();
                if (tableName == "tbl_vehicle_type")
                {
                    tblExist = true;
                    break;
                }
            }
            if (tblExist == false)
            {
                string query = "";
                query = "CREATE TABLE tbl_vehicle_type" +
                        "(" +
                            "[ID] AUTOINCREMENT PRIMARY KEY," +
                            "[vehicle_type] TEXT(255) " +
                        ");";
                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                cmd = new OleDbCommand("INSERT INTO tbl_vehicle_type (vehicle_type) VALUES ('รถยนต์')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                cmd = new OleDbCommand("INSERT INTO tbl_vehicle_type (vehicle_type) VALUES ('รถจักรยานยนต์')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                cmd = new OleDbCommand("INSERT INTO tbl_vehicle_type (vehicle_type) VALUES ('รถกระบะ')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                cmd = new OleDbCommand("INSERT INTO tbl_vehicle_type (vehicle_type) VALUES ('รถTaxi')", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

            }
        }
        public static void CREATE_TABLE_VISIT_TO()
        {
            string tableName = "";
            bool tblExist = false;
            var schema = classGlobal.conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (var row in schema.Rows.OfType<DataRow>())
            {
                tableName = row.ItemArray[2].ToString();
                if (tableName == "tbl_visit_to")
                {
                    tblExist = true;
                    break;
                }
            }
            if (tblExist == false)
            {
                string query = "";
                query = "CREATE TABLE tbl_visit_to" +
                        "(" +
                            "[ID] AUTOINCREMENT PRIMARY KEY," +
                            "[visit_to] TEXT(255) " +
                        ");";

                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
        }
        public static void CREATE_TABLE_DEPARTMENT()
        {
            string tableName = "";
            bool tblExist = false;
            var schema = classGlobal.conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (var row in schema.Rows.OfType<DataRow>())
            {
                tableName = row.ItemArray[2].ToString();
                if (tableName == "tbl_department")
                {
                    tblExist = true;
                    break;
                }
            }
            if (tblExist == false)
            {
                string query = "";
                query = "CREATE TABLE tbl_department" +
                        "(" +
                            "[ID] AUTOINCREMENT PRIMARY KEY," +
                            "[department] TEXT(255) " +
                        ");";

                 OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
        }
        public static void CREATE_TABLE_BUSINESS_TOPIC()
        {
            string tableName = "";
            bool tblExist = false;
            var schema = classGlobal.conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (var row in schema.Rows.OfType<DataRow>())
            {
                tableName = row.ItemArray[2].ToString();
                if (tableName == "tbl_business_topic")
                {
                    tblExist = true;
                    break;
                }
            }
            if (tblExist == false)
            {
                string query = "";
                query = "CREATE TABLE tbl_business_topic" +
                        "(" +
                            "[ID] AUTOINCREMENT PRIMARY KEY," +
                            "[business_topic] TEXT(255) " +
                        ");";

                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
        }
        public static void CREATE_TABLE_PLACE()
        {
            string tableName = "";
            bool tblExist = false;
            var schema = classGlobal.conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (var row in schema.Rows.OfType<DataRow>())
            {
                tableName = row.ItemArray[2].ToString();
                if (tableName == "tbl_place")
                {
                    tblExist = true;
                    break;
                }
            }
            if (tblExist == false)
            {
                string query = "";
                query = "CREATE TABLE tbl_place" +
                        "(" +
                            "[ID] AUTOINCREMENT PRIMARY KEY," +
                            "[place] TEXT(255) " +
                        ");";

                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
        }
        public static void CREATE_CHARGE_CAR_PARK()
        {
            string tableName = "";
            bool tblExist = false;
            var schema = classGlobal.conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (var row in schema.Rows.OfType<DataRow>())
            {
                tableName = row.ItemArray[2].ToString();
                if (tableName == "tbl_charge_car_park")
                {
                    tblExist = true;
                    break;
                }
            }
            if (tblExist == false)
            {
                string query = "";
                query = "CREATE TABLE tbl_charge_car_park" +
                        "(" +
                            "[ID] AUTOINCREMENT PRIMARY KEY, " +
                            "[typeid] NUMBER ," +
                            "[class] TEXT(5), " +
                            "[typename] TEXT(100), " +
                            "[minutes] TEXT(250), " +
                            "[rate] TEXT(250), " +
                            "[status] TEXT(5) " +
                        ");";

                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
        }
        public static void CREATE_CHARGE_LOGS()
        {
            string tableName = "";
            bool tblExist = false;
            var schema = classGlobal.conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (var row in schema.Rows.OfType<DataRow>())
            {
                tableName = row.ItemArray[2].ToString();
                if (tableName == "tbl_charge_logs")
                {
                    tblExist = true;
                    break;
                }
            }
            if (tblExist == false)
            {
                string query = "";
                query = "CREATE TABLE tbl_charge_logs" +
                        "(" +
                            "[AID] AUTOINCREMENT PRIMARY KEY, " +
                            "[id] NUMBER ," +
                            "[minutes] TEXT(50), " +
                            "[charge] TEXT(50), " +
                            "[extra] TEXT(50), " +
                            "[paid] TEXT(5), " +
                            "[charge_type] TEXT(5) " +
                        ");";


                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }

            //+++ ALTER +++//
            OleDbCommand cmd1 = new OleDbCommand("SELECT TOP 1 charge_type FROM tbl_charge_logs", classGlobal.conn);
            bool exists = true;
            try
            {
                var x = cmd1.ExecuteScalar();
            }
            catch (Exception e)
            {
                exists = false;
                Console.Write(e.Message.ToString()); 
            }
            cmd1.Dispose();
            cmd1 = null;

            if (exists == false)
            {
                string query = "ALTER TABLE tbl_charge_logs ADD COLUMN [charge_type] TEXT(5)";
                if (classGlobal.databaseType == "psql")
                { query = "ALTER TABLE tbl_charge_logs ADD charge_type NVARCHAR(5)"; }
                cmd1 = new OleDbCommand(query, conn);
                cmd1.ExecuteNonQuery();
                cmd1.Dispose();
                cmd1 = null;
            }
            //--- ALTER ---//
        }
        #endregion


        static byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        static byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        public static string Crypt(string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }
        public static string Decrypt(string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }

        public static string PLACEWATERMARK_FROM_BASE64(string basestringImage)
        {
            if (basestringImage == null)
                basestringImage = "";

            if (basestringImage == "")
                return basestringImage;

            if (classGlobal.watermark.Replace(" ", "") == "")
                classGlobal.watermark = "แลกบัตรผู้มาเข้าพบ";

            byte[] bytes = Convert.FromBase64String(basestringImage);
            byte[] byteImage = PLACEWATERMARK(bytes);
            basestringImage = Convert.ToBase64String(byteImage); // Get Base64            
            return basestringImage;
        }
        public static byte[] PLACEWATERMARK_FROM_BYTE(byte[] bytes)
        {
            byte[] byteImage = new byte[0];

            if (bytes == null || bytes.Length == 0)
                return bytes;

            if (classGlobal.watermark.Replace(" ", "") == "")
                classGlobal.watermark = "แลกบัตรผู้มาเข้าพบ";

            byteImage = PLACEWATERMARK(bytes);
            return byteImage;
        }
        public static Bitmap PLACEWATERMARK_FROM_BITMAP(Bitmap bmp)
        {
            Bitmap bm = null;

            if (bmp == null)
                return bm;

            if (classGlobal.watermark.Replace(" ", "") == "")
                classGlobal.watermark = "แลกบัตรผู้มาเข้าพบ";

            ImageConverter converter = new ImageConverter();
            byte[] bytes = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
            bytes = PLACEWATERMARK(bytes);
            var ms = new MemoryStream(bytes);
            bm = (Bitmap)Image.FromStream(ms);
            return bm;
        }
        public static Image PLACEWATERMARK_FROM_IMAGE(Image img)
        {
            Image bm = null;

            if (img == null)
                return bm;

            if (classGlobal.watermark.Replace(" ", "") == "")
                classGlobal.watermark = "แลกบัตรผู้มาเข้าพบ";

            ImageConverter converter = new ImageConverter();
            byte[] bytes = (byte[])converter.ConvertTo(img, typeof(byte[]));
            bytes = PLACEWATERMARK(bytes);
            var ms = new MemoryStream(bytes);
            bm = Image.FromStream(ms);
            return bm;
        }

        static byte[] PLACEWATERMARK(byte[] bytes)
        {
            byte[] byteImage = new byte[0];
            try
            {
                Bitmap bg = (Bitmap)((new ImageConverter()).ConvertFrom(bytes));
                float angle = -30.0F;
                Font fnt = new Font("Tahoma", 14, FontStyle.Bold);    
                int opacity = 128; // 50% opaque (0 = invisible, 255 = fully opaque)  128
                using (Graphics g = Graphics.FromImage(bg))
                {
                    g.TranslateTransform(bg.Width / 2, bg.Height / 2);
                    g.RotateTransform(angle);
                    SizeF textSize = g.MeasureString(classGlobal.watermark, fnt);
                    g.DrawString(classGlobal.watermark, fnt,
                        new SolidBrush(Color.FromArgb(opacity, Color.Green)),       
                        -(textSize.Width / 2),
                        -(textSize.Height / 2));
                }

                Bitmap bImage = bg;  // Your Bitmap Image
                System.IO.MemoryStream ms = new MemoryStream();
                bImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byteImage = ms.ToArray();
                ms.Dispose();
            }
            catch
            {
                //
            }

            return byteImage;
        }
        public static string REPLACE_IDCARD(string str)
        {
            try
            {
                //str = str.Remove(str.Length - 3, 3) + "***";
                string tmp = str.Remove(0, 5);
                str = tmp.Remove(tmp.Length - 1, 1) + "#";

                if (classGlobal.DisplayHashTag == true)    
                    str = "#############";

            }
            catch
            {
                //
            }

            return str;
        }
        public static string REPLACE_NAME(string str)
        {
            if (str == "")
                return "";

            string[] name = str.Split(' ');
            try
            {
                //string tmp = "";
                //for (int i = 0; i < name.Length; i++)
                //{
                //    tmp = name[i];
                //    try
                //    {
                //        if (tmp.Length <= 3)
                //            tmp = tmp.Remove(tmp.Length - 1, 1) + "#";
                //        else
                //            tmp = tmp.Remove(tmp.Length - 3, 3) + "###";
                //    }
                //    catch
                //    {
                //        tmp = tmp.Remove(tmp.Length - 1, 1) + "#";
                //    }

                //    name[i] = tmp;
                //}
                //str = String.Join(" ", name);

                if (classGlobal.DisplayHashTag == true)
                    str = "###### ######";

            }
            catch
            {
                //
            }
            return str;
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        public static void CHECK_EXIST_TABLE_PSQL()
        {
            string[] table = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"Scripts","*.psql");
            string query = "";
            NpgsqlCommand cmd = null;
            NpgsqlDataReader reader = null;

            for(int i=0;i< table.Length;i++ )
            {
                string[] txtScript = new string[0];
                string[] InsertScript = new string[0];

                String[] spearator = { "GO;" }; 
                string[] spearatorNewline = new string[] { "\r\n" };

                string tbl = Path.GetFileNameWithoutExtension(table[i] ); 
                try
                {
                    query = "SELECT * FROM " + tbl + " LIMIT 1";
                    cmd = new NpgsqlCommand(query, connP);
                    reader =  cmd.ExecuteReader();
                    reader.Dispose();
                    reader = null;
                }
                catch
                {
                    txtScript = System.IO.File.ReadAllText(table[i]).Split(spearator, StringSplitOptions.RemoveEmptyEntries);

                    cmd = new NpgsqlCommand(txtScript[0], connP);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    InsertScript = txtScript[1].Split(spearatorNewline, StringSplitOptions.RemoveEmptyEntries);

                    for (int j = 0; j < InsertScript.Length; j++)
                    {
                        cmd = new NpgsqlCommand(InsertScript[j], connP);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    txtScript = new string[0];
                    InsertScript = new string[0];
                }                
                cmd.Dispose();
                cmd = null;
            }
 
        }

        public static void OPEN_CASH_DRAWER()  //COM8
        {
            ClassHelper.clsXML clsxml = new ClassHelper.clsXML();
            classGlobal.commportName = clsxml.GetReadXML("root", "commport", classGlobal.config).ToString();
            clsxml = null;

            try
            {
                Encoding enc = Encoding.Unicode;
                SerialPort sp = new SerialPort();
                sp.PortName = classGlobal.commportName.ToUpper();

                sp.Encoding = enc;
                sp.BaudRate = 38400;                         // อัตราการรับส่งข้อมูล (Baud Rate)
                sp.Parity = System.IO.Ports.Parity.None;     // None Parity คือ ไม่มีการตรวจสอบความถูกต้องของข้อมูล
                sp.DataBits = 8;                             // ขนาดข้อมูลจำนวน 8 บิต
                sp.StopBits = System.IO.Ports.StopBits.One;  // มี Stop Bit ปิดท้ายอีก 1 บิต
                sp.DtrEnable = true;
                sp.Open();
                sp.Write(char.ConvertFromUtf32(28699) + char.ConvertFromUtf32(9472) + char.ConvertFromUtf32(3365));
                sp.Close();
            }
            catch
            {
                //--
            }

        }
        public static string GetPortInformation()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
                {
                    var portnames = SerialPort.GetPortNames();
                    var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());
                    var portList = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList();
                    foreach (string s in portList)
                    {
                        //Prolific USB-to-Serial Comm Port
                        if (s.ToString().Contains("Prolific USB-to-Serial Comm Port"))
                        {
                            string[] splt = s.Split('-');
                            return splt[0];
                        }
                    }
                }
                return "-";
            }
            catch
            {
                return "-";
            }
          
        }

        public static string CONVERT_UTC_TO_LOCAL(string utcTime)
        {
            string time = "";
            if (utcTime == "")
                return utcTime;

            try
            {
                DateTime dt1 = DateTime.Parse(utcTime);
                utcTime = dt1.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                if (!utcTime.Equals(""))
                {
                    //utcTime = "2020-12-01T07:44:25.929Z";
                    DateTime localDateTime = DateTime.Parse(utcTime);
                    time = localDateTime.Year + "-" +
                                    localDateTime.Month.ToString().PadLeft(2, '0') + "-" +
                                        localDateTime.Day.ToString().PadLeft(2, '0') + " " +
                                            localDateTime.Hour.ToString().PadLeft(2, '0') + ":" +
                                                localDateTime.Minute.ToString().PadLeft(2, '0') + ":" +
                                                    localDateTime.Second.ToString().PadLeft(2, '0');

                }
                else
                {
                    time = "";
                }
            }
            catch
            {
                time = "";
            }
            
            return time;
        }

        public static string CONVERT_LOCAL_TO_UTC(string localTime)
        {   //string.Format("{0:yyyy-MM-ddTHH:mm:ss.FFFZ}", DateTime.UtcNow)
            DateTime utcTime = DateTime.Parse(localTime).ToUniversalTime();
            string utc = utcTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            return utc;
        }        
        public static string GetRandomHexNumber(int digits)
        {
            Random random = new Random();
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;

            return result + random.Next(16).ToString("X");
        }

        static string checkOfflineExistNumber()
        {

        BEGIN:
            Console.Write("BEGIN GEN");

            Random random = new Random();
            string vRandom = random.Next(0, int.Parse(classGlobal.strRandomNumber)).ToString().PadLeft(classGlobal.strRandomNumber.Length, '0');
            random = null;

            ////++ test random plus 1 step number
            //if (classGlobal.databaseType == "acc")
            //{                
            //    OleDbDataAdapter ad = new OleDbDataAdapter("SELECT TOP 1 card_number FROM tbl_visitor WHERE status_in <> '' AND (status_out = '' OR status_out IS NULL) ORDER BY id DESC", classGlobal.conn);
            //    DataTable card_number = new DataTable("card_number");
            //    ad.Fill(card_number);
            //    ad.Dispose();
            //    if (card_number.Rows.Count > 0)
            //        vRandom = (Int32.Parse(card_number.Rows[0][0].ToString()) + 1).ToString();

            //}
            //else if (classGlobal.databaseType == "psql")
            //{
            //    NpgsqlDataAdapter ad = new NpgsqlDataAdapter("SELECT card_number FROM tbl_visitor WHERE status_in <> '' AND (status_out = '' OR status_out IS NULL) ORDER BY id DESC LIMIT 1", classGlobal.connP);
            //    DataTable card_number = new DataTable("card_number");
            //    ad.Fill(card_number);
            //    ad.Dispose();
            //    if (card_number.Rows.Count > 0)
            //        vRandom = (Int32.Parse(card_number.Rows[0][0].ToString()) + 1).ToString();
            //}
            //else
            //{
            //    //-- do nothing
            //}
            ////-- test random plus 1 step number

            string query = String.Format("SELECT id FROM tbl_visitor WHERE object_idcompany ='{0}' AND " +
                                                "card_number ='{1}' AND status_in <> '' AND (status_out ='' OR status_out IS NULL)",
                                                                "", vRandom);
            int id = 0;
            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    id = Int32.Parse("0" + reader.GetValue(0).ToString());
                }

                reader.Close();
                command.Dispose();
                command = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlCommand commandP = new NpgsqlCommand(query, classGlobal.connP);
                NpgsqlDataReader readerP = commandP.ExecuteReader();
                while (readerP.Read())
                {
                    id = Int32.Parse("0" + readerP.GetValue(0).ToString());
                }

                readerP.Close();
                commandP.Dispose();
                commandP = null;
            }
            else
            {
                //-- cloud function not use in this case --//
            }

            if (id > 0)
                goto BEGIN;

            return vRandom;
        }
        public static string RandomVisitorNumber()
        {
            string visitorNumber = "";

            if (classGlobal.userId != "")
                visitorNumber = ClassData.GENERATE_VISITOR_NUMBER(classGlobal.userId);  // ได้ค่า random จาก server ที่คำนวนให้แล้วไม่มีทางซ้ำแน่นอน      
            else
                visitorNumber = checkOfflineExistNumber();

            return visitorNumber;

            #region โค้ดเดิม ซึ่งก็ยังใช้ได้ปกติ
            //Random random = new Random();
            //string visitorNumber = random.Next(0, int.Parse(classGlobal.strRandomNumber)).ToString().PadLeft(classGlobal.strRandomNumber.Length, '0');            

            //if (classGlobal.userId != "")
            //    visitorNumber = ClassData.GENERATE_VISITOR_NUMBER(classGlobal.userId);

            //if (visitorNumber == "")
            //{
            //    visitorNumber = random.Next(0, int.Parse(classGlobal.strRandomNumber)).ToString().PadLeft(classGlobal.strRandomNumber.Length, '0');
            //}
            //else
            //{
            //    if (classGlobal.userId != "")
            //    {
            //        string sExist = ClassData.POST_CHECK_EXIST_VISITOR_NUMBER(classGlobal.access_token, classGlobal.userId, visitorNumber, "in", true);
            //        if (sExist != "false")
            //            visitorNumber = ClassData.GENERATE_VISITOR_NUMBER(classGlobal.userId);
            //    }                
            //}

            //return visitorNumber;
            #endregion
        }

        public static List<String> GET_LIST_OF_DATE_BETWEEN(DateTime startDateTime, DateTime stopDateTime)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.CultureInfo MyCultureInfo = new System.Globalization.CultureInfo("en-US");

            List<String> lstDate = new List<string>(0);
            TimeSpan diffResult = stopDateTime - startDateTime;
            int nDateDiff = diffResult.Days + 1; 
            string yyyyMMdd = "";
            for (int i = 0; i < nDateDiff; i++)
            {
                yyyyMMdd = startDateTime.ToString("yyyyMMdd");
                lstDate.Add(yyyyMMdd);
                startDateTime = startDateTime.AddDays(1);
            }

            return lstDate;
        }

        public static List<String> GET_LIST_OF_LAST_SEVEN_DAY(DateTime toDate, int backLastDay)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.CultureInfo MyCultureInfo = new System.Globalization.CultureInfo("en-US");

            List<String> lstDate = new List<string>(0);
            string yyyyMMdd = "";
            for (int i = 1; i <= backLastDay; i++)
            {
                yyyyMMdd = toDate.ToString("yyyyMMdd");
                lstDate.Add(yyyyMMdd);
                toDate = toDate.AddDays(-1);
            }
            return lstDate;
        }

        public static void isStillRunning()
        {
            string processName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
            ManagementObjectSearcher mos = new ManagementObjectSearcher();
            mos.Query.QueryString = @"SELECT * FROM Win32_Process WHERE Name = '" + processName + @"'";
            if (mos.Get().Count > 1)
            {
                frmMessageBox f_exist = new frmMessageBox();
                f_exist.strMessage = "โปรแกรมเปิดทำงานอยู่แล้ว";
                f_exist.strStatus = "Error";
                f_exist.ShowDialog(); 

                //System.Windows.Forms.MessageBox.Show("โปรแกรมเปิดทำงานอยู่แล้ว");
                Environment.Exit(0);
            }
            else
            {
                //
            }
        }

        public static bool CheckPOC_ONLINE(string chkPrinterName)  //  POS-80C หรือ XP-80C  *C-หมายถึง Cut
        {
            string defaultPrinterName = "";
            PrinterSettings settings = new PrinterSettings();
            defaultPrinterName = settings.PrinterName.ToString();

            bool IsReady = false;
            string printerName = "";
            System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Printer");
            foreach (System.Management.ManagementObject printer in searcher.Get())
            {
                printerName = printer["Name"].ToString().ToLower();
                if (printerName.Equals(defaultPrinterName.ToString().ToLower()))
                {
                    Console.WriteLine("Printer =  " + printer["Name"]);
                    if (printer["WorkOffline"].ToString().ToLower().Equals("true"))
                    {
                        Console.WriteLine("Printer is Off");
                    }
                    else
                    {
                        Console.WriteLine("Printer is On");
                        IsReady = true;
                        break;
                    }
                }
            }

            return IsReady;
        }


        public static void CHECK_EXIST_IN_FOR_PUB(string citizenId, string fullname, string strBirthDate)
        {
            if (classGlobal.userId == "")
            {
                //+++++++++++++++++++++++++++++++++//
                clsXML clsxmlP = new clsXML();
                string readStr = clsxmlP.GetReadXML("root", "checkage", classGlobal.config).ToLower().ToString();
                if (readStr == "false")
                    classGlobal.CheckAge = 0;
                else if (readStr == "true")
                    classGlobal.CheckAge = 20;
                else
                    classGlobal.CheckAge = Int32.Parse(clsxmlP.GetReadXML("root", "checkage", classGlobal.config));
                clsxmlP = null;
                //+++++++++++++++++++++++++++++++++//

                if (classGlobal.CheckAge > 0)
                {
                    int age = 0;
                    string[] txtBD = new string[0];
                    try
                    {
                        txtBD = strBirthDate.Split('/');
                        age = classGlobal.Age_Calculate(new DateTime(Int32.Parse(txtBD[2]), Int32.Parse(txtBD[1]), Int32.Parse(txtBD[0])));
                    }
                    catch
                    {
                        age = 0;
                    }

                    if (age > 0)
                    {                                               
                        txtBD = strBirthDate.Split('/');
                        txtBD[2] = (Int32.Parse(txtBD[2]) + 543).ToString();

                        string query = "";
                        DataTable dt = new DataTable("dt");
                        if (classGlobal.databaseType == "acc")
                        {
                            query = "SELECT  TOP 1 t2.id, t1.str_imagedocument, t1.str_imagewebcamera, t1.status_in, t1.status_out " +
                                           "FROM tbl_visitor t1 " +
                                           "INNER JOIN tbl_personal t2 ON t1.id = t2.id " +
                                           "WHERE t2.id_number='" + citizenId + "'" + " " + "ORDER BY t2.id DESC";

                            OleDbDataAdapter ad = new OleDbDataAdapter(query, classGlobal.conn);                            
                            ad.Fill(dt);
                            ad.Dispose();
                            ad = null;
                        }
                        else if (classGlobal.databaseType == "psql")
                        {
                            query = "SELECT   t2.id, t1.str_imagedocument, t1.str_imagewebcamera, t1.status_in, t1.status_out " +
                                           "FROM tbl_visitor t1 " +
                                           "INNER JOIN tbl_personal t2 ON t1.id = t2.id " +
                                           "WHERE t2.id_number='" + citizenId + "'" + " " + "ORDER BY t2.id DESC LIMIT 1";

                            NpgsqlDataAdapter ad = new NpgsqlDataAdapter(query, classGlobal.connP);
                            ad.Fill(dt);
                            ad.Dispose();
                            ad = null;
                        }
                        else
                        {
                            // do nothing
                        }


                        frmAgeForPub fage = new frmAgeForPub();
                        if (dt.Rows.Count > 0)
                        {                            
                            fage.citizenId = citizenId;
                            fage.citizenName = fullname;
                            fage.citizenAge = age.ToString();
                            fage.citizenCheckIn = "";
                            fage.base64PhotoImage = dt.Rows[0][1].ToString();
                            fage.base64CameraImage = dt.Rows[0][2].ToString();

                            string[] spltSpace = dt.Rows[0][3].ToString().Split(' ');
                            string[] spltDash = spltSpace[0].ToString().Split('-');
                            string[] spltColon = spltSpace[1].ToString().Split(':');
                            DateTime dtNow = DateTime.Now;
                            DateTime dtCheckIn = new DateTime(Int32.Parse(spltDash[0]), Int32.Parse(spltDash[1]), Int32.Parse(spltDash[2]),
                                                                    Int32.Parse(spltColon[0]), Int32.Parse(spltColon[1]), Int32.Parse(spltColon[2]));
                            TimeSpan ts = dtNow - dtCheckIn;
                            int hoursDiff = (Int32)ts.TotalHours;
                            if (hoursDiff >= 24) // เกิน 1 วัน ถือว่าไม่เข้าซ้ำ
                            {
                                fage.strStatus = "Information";
                                fage.citizenCheckIn = "";
                            }
                            else  // ภายใน 1 วัน ถือว่าเข้าซ้ำ!!!
                            {
                                fage.strStatus = "Error";
                                fage.citizenCheckIn = spltDash[2] + "/" + spltDash[1] + "/" + (Int32.Parse(spltDash[0]) + 543).ToString() + " " + spltSpace[1];                                
                            }                            
                        }
                        else
                        {
                            fage.strStatus = "Information";
                            fage.citizenId = citizenId;
                            fage.citizenName = fullname;
                            fage.citizenAge = age.ToString();
                            fage.citizenCheckIn = "";
                            fage.base64PhotoImage = "";
                            fage.base64CameraImage = "";                            
                            fage.citizenCheckIn = "";                           
                        }

                        fage.ShowDialog();

                    }

                }
            }
        }
        public static int Age_Calculate(DateTime birthdate)
        {
             System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");           
            // Save today's date.
            var today = DateTime.Today;
            // Calculate the age.
            var age = today.Year - birthdate.Year;
            // Go back to the year in which the person was born in case of a leap year
            if (birthdate.Date > today.AddYears(-age)) age--;

            return age; 
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                //if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_') {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static DataTable GetDataFromXLSX(string path, bool hasHeader = true)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column{0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {

                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                return tbl;
            }
        }
        public static DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            return dTable;
        }

        public static bool ProgramIsRunning(string FullPath)
        {
            string FilePath = Path.GetDirectoryName(FullPath);
            string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
            bool isRunning = false;

            Process[] pList = Process.GetProcessesByName(FileName);

            foreach (Process p in pList)
            {
                if (p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    isRunning = true;
                    break;
                }
            }

            return isRunning;
        }

        public static void CHECK_DATABASE_SIZE()
        {
            #region เช็คฐานข้อมุลว่าใกล้เต็มรึยัง
            long n = (1024 * 1024 * 1024);
            long TWO_G = n * 2;
            long MARGIN = (8 * 1024 * 1024);
            string pathToMonsterMdb = DATABASELOCATION + DATABASENANE + ".mdb";
            FileInfo mdb = new FileInfo(pathToMonsterMdb);
            long len = mdb.Length;
            if (len > (TWO_G - MARGIN))
                System.Windows.Forms.MessageBox.Show("ฐานข้อมูลใกล้จะเต็มแล้ว กรุณา backup ข้อมูล!", "VISITOR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            #endregion
        }
    }
}


