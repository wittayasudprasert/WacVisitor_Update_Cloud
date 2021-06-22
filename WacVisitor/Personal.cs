using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.IO;
//using System.Windows.Media.Imaging;
using System.Drawing;

namespace WacVisitor
{
    public class Personal
    {
        public string Address { get; set; }
        public string AddressHouseNo { get; set; }
        public string AddressVillageNo { get; set; }
        public string AddressLane { get; set; }
        public string AddressSoi { get; set; }
        public string AddressRoad { get; set; }
        public string AddressSubDistrict { get; set; }
        public string AddressDistrict { get; set; }
        public string AddressProvince { get; set; }
        //public string AddrSubDistrict { get; set; }        
        public string CardType { get; set; }
        public string CitizenID { get; set; }
        public string EnBirthDate { get; set; }
        public string EnExpiryDate { get; set; }
        public string EnFirstName { get; set; }
        public string EnIssueDate { get; set; }
        public string EnLastName { get; set; }
        public string EnMidName { get; set; }
        public string EnPreName { get; set; }
        public string ImageNumber { get; set; }
        public string IssueCode { get; set; }
        public string IssuePlace { get; set; }
        public Bitmap PhotoBitmap { get; set; }
        public byte[] PhotoByte { get; set; }
        public string ReqCardNumber { get; set; }
        public string Sex { get; set; }
        public string ThBirthDate { get; set; }
        public string ThExpiryDate { get; set; }
        public string ThFirstName { get; set; }
        public string ThIssueDate { get; set; }
        public string ThLastName { get; set; }
        public string ThMidName { get; set; }
        public string ThPreName { get; set; }
        public string Version { get; set; }

    }
}
