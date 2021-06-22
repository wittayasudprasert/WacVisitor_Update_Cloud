using GemCard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WacVisitor
{
    public partial class ThaiIDReader : Form
    {
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
       (
           int nLeftRect,     // x-coordinate of upper-left corner
           int nTopRect,      // y-coordinate of upper-left corner
           int nRightRect,    // x-coordinate of lower-right corner
           int nBottomRect,   // y-coordinate of lower-right corner
           int nWidthEllipse, // height of ellipse
           int nHeightEllipse // width of ellipse
       );

        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();


        delegate void EnableProgrssBarDelegate(ProgressBar pg, int val);
        private CardBase m_iCard = null;

        #region WinSCard Lib
        private IntPtr _handleContext;
        private IntPtr _handleCard;
        private uint _activeProtocol;
        private byte[] _buffer;
        private ushort GetData(ushort tag, int length)
        {
            byte[] apdu = new byte[5];
            apdu[0] = 0x80;
            apdu[1] = 0xca;
            apdu[2] = Convert.ToByte(tag >> 8);
            apdu[3] = Convert.ToByte(tag & 0xff);
            apdu[4] = Convert.ToByte(length);

            byte[] response = sendAPDU(apdu);

            if (response != null && response.Length >= 2)
            {
                ushort sw = response[response.Length - 2];
                sw <<= 8;
                sw += response[response.Length - 1];

                if (sw == 0x9000)
                {
                    _buffer = new byte[response.Length - 2];
                    Array.Copy(response, 0, _buffer, 0, response.Length - 2);
                }

                return sw;
            }

            return 0;
        }
        public void Connect()
        {
            uint pdwActiveProtocol = 0;
            uint dwShareMode = 2;           // SCARD_SHARE_SHARED
            uint dwPreferredProtocols = 3;  // SCARD_PROTOCOL_T1 || SCARD_PROTOCOL_T2
            pdwActiveProtocol = 0;

            WinSCard.SCardConnect(
                _handleContext,
                comboReader.Text,
                dwShareMode,
                dwPreferredProtocols,
                out _handleCard,
                out pdwActiveProtocol);

            _activeProtocol = pdwActiveProtocol;
        }
        private void Disconnect()
        {
            WinSCard.SCardDisconnect(_handleCard, (uint)2);
            _handleCard = IntPtr.Zero;
        }
        private byte[] sendAPDU(byte[] apdu)
        {
            IntPtr SCARD_PCI_T0 = WinSCard.GetPciT0();

            uint recvLength = 257;
            byte[] recvBytes = new byte[recvLength];
            WinSCard.SCARD_IO_REQUEST recvPci = new WinSCard.SCARD_IO_REQUEST();
            recvPci.dwProtocol = _activeProtocol;         // SCARD_PROTOCOL_T0
            recvPci.cbPciLength = 255;


            WinSCard.SCARD_IO_REQUEST sendPci = new WinSCard.SCARD_IO_REQUEST();
            sendPci.dwProtocol = _activeProtocol;         // SCARD_PROTOCOL_T0
            sendPci.cbPciLength = 8;

            int err = WinSCard.SCardTransmit(_handleCard,
                                             SCARD_PCI_T0,
                                             apdu,
                                             (uint)apdu.Length,
                                             null,
                                             recvBytes,
                                             ref recvLength);

            if (err == 0)
            {
                byte[] response = new byte[recvLength];
                Array.Copy(recvBytes, 0, response, 0, recvLength);
                return response;
            }

            return null;
        }
        #endregion
        public ThaiIDReader()
        {
            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

           

            SelectICard();
            SetupReaderList();
        }

        Personal personal = new Personal();
        protected void EnableProgrssBar(ProgressBar pg, int val)
        {
            pg.Value = val;
        }
        private void ThaiIDReader_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 20;
        }

        private void ThaiIDReader_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void SelectICard()
        {
            try
            {
                if (m_iCard != null)
                    m_iCard.Disconnect(DISCONNECT.Unpower);

                m_iCard = new CardNative();

                m_iCard.OnCardInserted += new CardInsertedEventHandler(m_iCard_OnCardInserted);
                m_iCard.OnCardRemoved += new CardRemovedEventHandler(m_iCard_OnCardRemoved);

            }
            catch (Exception ex)
            {
                //btnConnect.Enabled = false;
                //btnDisconnect.Enabled = false;

            }
        }

        protected void EnableButton(Button btn, bool enable)
        {
            btn.Enabled = enable;
        }
        private void m_iCard_OnCardInserted(string reader)
        {
            personal = READ_CARDINFO();

            if (File.Exists(@"cid.ini") == true)
                File.Delete(@"cid.ini");

            if (personal != null)
            {

                string fileName;
                StreamWriter objWriter;

                fileName = "cid.ini";
                objWriter = new StreamWriter(fileName);
                objWriter.WriteLine(personal.CitizenID);
                objWriter.WriteLine(personal.ThPreName);
                objWriter.WriteLine(personal.ThFirstName);
                objWriter.WriteLine(personal.ThLastName);
                objWriter.WriteLine(personal.EnPreName);
                objWriter.WriteLine(personal.EnFirstName);
                objWriter.WriteLine(personal.EnLastName);
                objWriter.WriteLine(personal.ThBirthDate);
                objWriter.WriteLine(personal.Sex);
                objWriter.WriteLine(personal.AddressHouseNo);
                objWriter.WriteLine(personal.AddressVillageNo);
                objWriter.WriteLine(personal.AddressLane);
                objWriter.WriteLine(personal.AddressRoad);
                objWriter.WriteLine(personal.AddressSubDistrict);
                objWriter.WriteLine(personal.AddressDistrict);
                objWriter.WriteLine(personal.AddressProvince);
                objWriter.WriteLine(personal.ThIssueDate);
                objWriter.WriteLine(personal.ThExpiryDate);
                objWriter.WriteLine(Convert.ToBase64String(personal.PhotoByte));
                objWriter.Close();


                if (progressBar1.InvokeRequired)
                {
                    //progressBar1.BeginInvoke(new MethodInvoker(delegate { progressBar1.Visible = false; })); 
                    this.BeginInvoke(new MethodInvoker(delegate { this.Close(); }));
                }
                else
                {
                    //progressBar1.Visible = false;
                    this.Close();
                }

            }

        }
        private void m_iCard_OnCardRemoved(string reader)
        {
            CLEAR_LOGS();
        }

        private void SetupReaderList()
        {
            try
            {
                string[] sListReaders = m_iCard.ListReaders();
                comboReader.Items.Clear();

                if (sListReaders != null)
                {
                    for (int nI = 0; nI < sListReaders.Length; nI++)
                        comboReader.Items.Add(sListReaders[nI]);

                    comboReader.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void comboReader_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_iCard.StopCardEvents();

                // Get the current selection
                int idx = comboReader.SelectedIndex;
                if (idx != -1)
                {
                    // Start waiting for a card
                    string reader = (string)comboReader.SelectedItem;
                    m_iCard.StartCardEvents(reader);

                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
        }


        int sleep = 5;
        private Personal READ_CARDINFO()
        {
            personal = new Personal();

            int val = 0;
            progressBar1.Value = 0;
            UTF8Encoding TIS = new UTF8Encoding();
            Encoding objEncoding;

            //string strChipId = READ_CHIPID();            
            //string strLaserId = READ_LASERID();

            WinSCard.SCardEstablishContext(2, IntPtr.Zero, IntPtr.Zero, out _handleContext);
            Connect();

            byte[] apdu1;
            byte[] apdu2;
            byte[] apdu3;

            // Select Card
            apdu1 = new byte[13] { 0x00, 0xa4, 0x04, 0x00, 0x08, 0xa0, 0x00, 0x00, 0x00, 0x54, 0x48, 0x00, 0x01 };
            byte[] response = sendAPDU(apdu1);

            if (response != null && response.Length >= 2)
            {
                ushort sw = response[response.Length - 2];
                sw <<= 8;
                sw += response[response.Length - 1];

                if ((sw & 0xff00) == 0x6100 || (sw & 0xf000) == 0x9000)
                {
                    // 1.CID
                    System.Threading.Thread.Sleep(sleep);
                    apdu2 = new byte[7] { 0x80, 0xb0, 0x00, 0x04, 0x02, 0x00, 0x0d };
                    response = sendAPDU(apdu2);
                    if (response != null && response.Length >= 2)
                    {
                        apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x0d };  // GET RESPONSE
                        response = sendAPDU(apdu3);
                        byte[] buffer = new byte[response.Length - 1];
                        Array.Copy(response, 0, buffer, 0, response.Length - 1);
                        progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                        personal.CitizenID = Encoding.ASCII.GetString(buffer, 0, 13);
                    }

                    // 2.TH Fullname
                    System.Threading.Thread.Sleep(sleep);
                    apdu2 = new byte[7] { 0x80, 0xb0, 0x00, 0x11, 0x02, 0x00, 0x64 };
                    response = sendAPDU(apdu2);
                    if (response != null && response.Length >= 2)
                    {
                        apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x64 };  // GET RESPONSE
                        response = sendAPDU(apdu3);
                        byte[] buffer = new byte[response.Length - 1];
                        Array.Copy(response, 0, buffer, 0, response.Length - 1);
                        TIS = new UTF8Encoding();
                        objEncoding = Encoding.GetEncoding("TIS-620");
                        progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                        string[] fullNameTh = objEncoding.GetString(buffer, 0, 100).Replace(" ", "").Split('#');
                        personal.ThPreName = fullNameTh[0];
                        personal.ThFirstName = fullNameTh[1];
                        personal.ThMidName = fullNameTh[2];
                        personal.ThLastName = fullNameTh[3];
                    }

                    // 3.En Fullname
                    System.Threading.Thread.Sleep(sleep);
                    apdu2 = new byte[7] { 0x80, 0xb0, 0x00, 0x75, 0x02, 0x00, 0x64 };
                    response = sendAPDU(apdu2);
                    if (response != null && response.Length >= 2)
                    {
                        apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x64 };  // GET RESPONSE
                        response = sendAPDU(apdu3);
                        byte[] buffer = new byte[response.Length - 1];
                        Array.Copy(response, 0, buffer, 0, response.Length - 1);
                        progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                        string[] fullNameEn = Encoding.Default.GetString(buffer, 0, 100).Replace(" ", "").Split('#');
                        personal.EnPreName = fullNameEn[0];
                        personal.EnFirstName = fullNameEn[1];
                        personal.EnMidName = fullNameEn[2];
                        personal.EnLastName = fullNameEn[3];
                    }

                    // 4.Date of Birth
                    System.Threading.Thread.Sleep(sleep);
                    apdu2 = new byte[7] { 0x80, 0xb0, 0x00, 0xd9, 0x02, 0x00, 0x08 };
                    response = sendAPDU(apdu2);
                    if (response != null && response.Length >= 2)
                    {
                        apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x08 };  // GET RESPONSE
                        response = sendAPDU(apdu3);
                        byte[] buffer = new byte[response.Length - 1];
                        Array.Copy(response, 0, buffer, 0, response.Length - 1);
                        progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                        string dateOfBirth = Encoding.Default.GetString(buffer, 0, 8).Replace(" ", "");
                        string day = dateOfBirth.Substring(6, 2);
                        string month = dateOfBirth.Substring(4, 2);
                        string year = dateOfBirth.Substring(0, 4);
                        string yearEn = (Int32.Parse(year) - 543).ToString();
                        personal.ThBirthDate = day + "/" + month + "/" + year;
                        personal.EnBirthDate = day + "/" + month + "/" + yearEn;
                    }

                    // 5.Gender
                    System.Threading.Thread.Sleep(sleep);
                    apdu2 = new byte[7] { 0x80, 0xb0, 0x00, 0xe1, 0x02, 0x00, 0x01 };
                    response = sendAPDU(apdu2);
                    if (response != null && response.Length >= 2)
                    {
                        apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x01 };  // GET RESPONSE
                        response = sendAPDU(apdu3);
                        byte[] buffer = new byte[response.Length - 1];
                        Array.Copy(response, 0, buffer, 0, response.Length - 1);
                        progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                        string gender = Encoding.Default.GetString(buffer, 0, 1).ToString().Replace("1", "M").Replace("2", "F");
                        personal.Sex = gender;
                    }

                    // 6.Card Issuer
                    System.Threading.Thread.Sleep(sleep);
                    apdu2 = new byte[7] { 0x80, 0xb0, 0x00, 0xf6, 0x02, 0x00, 0x64 };
                    response = sendAPDU(apdu2);
                    if (response != null && response.Length >= 2)
                    {
                        apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x64 };  // GET RESPONSE
                        response = sendAPDU(apdu3);
                        byte[] buffer = new byte[response.Length - 1];
                        Array.Copy(response, 0, buffer, 0, response.Length - 1);
                        TIS = new UTF8Encoding();
                        objEncoding = Encoding.GetEncoding("TIS-620");
                        progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                        string Card_Issuer = objEncoding.GetString(buffer, 0, 100).Replace(" ", "");
                        personal.IssuePlace = Card_Issuer;
                    }

                    // 7.Issue Date
                    System.Threading.Thread.Sleep(sleep);
                    apdu2 = new byte[7] { 0x80, 0xb0, 0x01, 0x67, 0x02, 0x00, 0x08 };
                    response = sendAPDU(apdu2);
                    if (response != null && response.Length >= 2)
                    {
                        apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x08 };  // GET RESPONSE
                        response = sendAPDU(apdu3);
                        byte[] buffer = new byte[response.Length - 1];
                        Array.Copy(response, 0, buffer, 0, response.Length - 1);
                        progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                        string IssueDate = Encoding.Default.GetString(buffer, 0, 8).Replace(" ", "");
                        string day = IssueDate.Substring(6, 2);
                        string month = IssueDate.Substring(4, 2);
                        string year = IssueDate.Substring(0, 4);
                        string yearEn = (Int32.Parse(year) - 543).ToString();
                        personal.ThIssueDate = day + "/" + month + "/" + year;
                        personal.EnIssueDate = day + "/" + month + "/" + yearEn;
                    }

                    // 8.Expire Date
                    System.Threading.Thread.Sleep(sleep);
                    apdu2 = new byte[7] { 0x80, 0xb0, 0x01, 0x6f, 0x02, 0x00, 0x08 };
                    response = sendAPDU(apdu2);
                    if (response != null && response.Length >= 2)
                    {
                        apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x08 };  // GET RESPONSE
                        response = sendAPDU(apdu3);
                        byte[] buffer = new byte[response.Length - 1];
                        Array.Copy(response, 0, buffer, 0, response.Length - 1);
                        progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                        string ExpireDate = Encoding.Default.GetString(buffer, 0, 8).Replace(" ", "");
                        string day = ExpireDate.Substring(6, 2);
                        string month = ExpireDate.Substring(4, 2);
                        string year = ExpireDate.Substring(0, 4);
                        string yearEn = (Int32.Parse(year) - 543).ToString();
                        personal.ThExpiryDate = day + "/" + month + "/" + year;
                        personal.EnExpiryDate = day + "/" + month + "/" + yearEn;
                    }

                    // 9.Address
                    System.Threading.Thread.Sleep(sleep);
                    apdu2 = new byte[7] { 0x80, 0xb0, 0x15, 0x79, 0x02, 0x00, 0x64 };
                    response = sendAPDU(apdu2);
                    if (response != null && response.Length >= 2)
                    {
                        apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x64 };  // GET RESPONSE
                        response = sendAPDU(apdu3);
                        byte[] buffer = new byte[response.Length - 1];
                        Array.Copy(response, 0, buffer, 0, response.Length - 1);
                        TIS = new UTF8Encoding();
                        objEncoding = Encoding.GetEncoding("TIS-620");
                        progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                        string Address = objEncoding.GetString(buffer, 0, 100).Replace(" ", "").Replace("#", " ");
                        personal.Address = Address;

                        string[] spltAddr = objEncoding.GetString(buffer, 0, 100).Replace(" ", "").Split('#');
                        personal.AddressHouseNo = spltAddr[0];
                        personal.AddressVillageNo = spltAddr[1];
                        personal.AddressLane = spltAddr[2];
                        personal.AddressSoi = spltAddr[3];
                        personal.AddressRoad = spltAddr[4];
                        personal.AddressSubDistrict = spltAddr[5];
                        personal.AddressDistrict = spltAddr[6];
                        personal.AddressProvince = spltAddr[7];
                    }

                    //// 10.ImageNumber
                    //System.Threading.Thread.Sleep(sleep);
                    //apdu2 = new byte[7] { 0x80, 0xb0, 0x16, 0x19, 0x02, 0x00, 0x0e };
                    //response = sendAPDU(apdu2);
                    //if (response != null && response.Length >= 2)
                    //{
                    //    apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x0e };  // GET RESPONSE
                    //    response = sendAPDU(apdu3);
                    //    byte[] buffer = new byte[response.Length - 1];
                    //    Array.Copy(response, 0, buffer, 0, response.Length - 1);
                    //    progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                    //    personal.ImageNumber = Encoding.Default.GetString(buffer, 0, 14).Replace(" ", "");
                    //}

                    //// 11.ISSUE_PLACE
                    //System.Threading.Thread.Sleep(sleep);
                    //apdu2 = new byte[7] { 0x80, 0xb0, 0x00, 0xf6, 0x02, 0x00, 0x64 };
                    //response = sendAPDU(apdu2);
                    //if (response != null && response.Length >= 2)
                    //{
                    //    apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x64 };  // GET RESPONSE
                    //    response = sendAPDU(apdu3);
                    //    byte[] buffer = new byte[response.Length - 1];
                    //    Array.Copy(response, 0, buffer, 0, response.Length - 1);
                    //    TIS = new UTF8Encoding();
                    //    objEncoding = Encoding.GetEncoding("TIS-620");
                    //    progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                    //    personal.IssuePlace = objEncoding.GetString(buffer, 0, 100).Replace(" ", "");
                    //}

                    //// 12.ISSUE_CODE
                    //System.Threading.Thread.Sleep(sleep);
                    //apdu2 = new byte[7] { 0x80, 0xb0, 0x01, 0x5a, 0x02, 0x00, 0x0d };
                    //response = sendAPDU(apdu2);
                    //if (response != null && response.Length >= 2)
                    //{
                    //    apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x0d };  // GET RESPONSE
                    //    response = sendAPDU(apdu3);
                    //    byte[] buffer = new byte[response.Length - 1];
                    //    Array.Copy(response, 0, buffer, 0, response.Length - 1);
                    //    progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                    //    personal.IssueCode = Encoding.Default.GetString(buffer, 0, 13).Replace(" ", "");
                    //}

                    //// 13.CMD_REQ_VERSION
                    //System.Threading.Thread.Sleep(sleep);
                    //apdu2 = new byte[7] { 0x80, 0xb0, 0x00, 0x00, 0x02, 0x00, 0x04 };
                    //response = sendAPDU(apdu2);
                    //if (response != null && response.Length >= 2)
                    //{
                    //    apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x04 };  // GET RESPONSE
                    //    response = sendAPDU(apdu3);
                    //    byte[] buffer = new byte[response.Length - 1];
                    //    Array.Copy(response, 0, buffer, 0, response.Length - 1);
                    //    progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                    //    personal.Version = Encoding.Default.GetString(buffer, 0, 4).Replace(" ", "");
                    //}

                    //// 14.CMD_REQ_CARD_NUMBER
                    //System.Threading.Thread.Sleep(sleep);
                    //apdu2 = new byte[7] { 0x80, 0xb0, 0x00, 0xe2, 0x02, 0x00, 0x14 };
                    //response = sendAPDU(apdu2);
                    //if (response != null && response.Length >= 2)
                    //{
                    //    apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x14 };  // GET RESPONSE
                    //    response = sendAPDU(apdu3);
                    //    byte[] buffer = new byte[response.Length - 1];
                    //    Array.Copy(response, 0, buffer, 0, response.Length - 1);
                    //    progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                    //    personal.ReqCardNumber = Encoding.Default.GetString(buffer, 0, 20).Replace(" ", "");
                    //}

                    //// 15.CMD_REQ_CARD_TYPE
                    //System.Threading.Thread.Sleep(sleep);
                    //apdu2 = new byte[7] { 0x80, 0xb0, 0x01, 0x77, 0x02, 0x00, 0x02 };
                    //response = sendAPDU(apdu2);
                    //if (response != null && response.Length >= 2)
                    //{
                    //    apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0x02 };  // GET RESPONSE
                    //    response = sendAPDU(apdu3);
                    //    byte[] buffer = new byte[response.Length - 1];
                    //    Array.Copy(response, 0, buffer, 0, response.Length - 1);
                    //    progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, val += 1 });

                    //    personal.CardType = Encoding.Default.GetString(buffer, 0, 2).Replace(" ", "");
                    //}


                    // 16.Photo
                    System.Threading.Thread.Sleep(sleep);
                    string base64String = "";
                    for (int r1 = 0; r1 <= 20; r1++)
                    {
                        apdu2 = new byte[] {
                                   (byte) 0x80,
                                   (byte) 0xB0,
                                   (byte) ((byte) 0x01+r1),
                                   (byte) ((byte) 0x7B-r1),
                                   (byte) 0x02,
                                   (byte) 0x00,
                                   (byte) 0xFF };

                        response = sendAPDU(apdu2);
                        if (response != null && response.Length >= 2)
                        {
                            apdu3 = new byte[5] { 0x00, 0xc0, 0x00, 0x00, 0xff };  // GET RESPONSE
                            response = sendAPDU(apdu3);
                            byte[] buffer = new byte[255];
                            Array.Copy(response, 0, buffer, 0, response.Length - 2);
                            base64String = base64String + Convert.ToBase64String(buffer, 0, buffer.Length);
                        }

                        System.Threading.Thread.Sleep(5);
                    }

                    personal.PhotoBitmap = (Bitmap)BASESTRING_TO_IMAGE(base64String);
                    personal.PhotoByte = Convert.FromBase64String(base64String);

                    progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, 20 });

                }
            }


            Disconnect();
            WinSCard.SCardReleaseContext(_handleContext);

            return personal;
        }
        private Image BASESTRING_TO_IMAGE(string base64String)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64String);
                using (var ms = new MemoryStream(bytes))
                {
                    return Image.FromStream(ms);
                }
            }
            catch
            {
                return null;
            }

        }

        private void CLEAR_LOGS()
        {
            if (progressBar1.InvokeRequired)
                progressBar1.Invoke(new EnableProgrssBarDelegate(EnableProgrssBar), new object[] { progressBar1, 0 });
            else
                progressBar1.Value = 0;

            this.Close();
        }

        private void ThaiIDReader_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                m_iCard.Disconnect(DISCONNECT.Unpower);
                m_iCard.StopCardEvents();
            }
            catch
            {
            }
        }
    }
}
