using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WacVisitor
{
    internal class WinSCard
    {
        public static string[] GetReaderLists()
        {
            IntPtr hContext = IntPtr.Zero;
            int nReturn = SCardEstablishContext(2, IntPtr.Zero, IntPtr.Zero, out hContext);
            if (nReturn != 0)
                return null;

            // Used to split null delimited strings into string arrays
            char[] delimiter = new char[1] { Convert.ToChar(0) };
            string[] readerLists = new string[0];

            // Get reader group lists
            IntPtr pmszGroups = IntPtr.Zero;
            uint pcchGroups = uint.MaxValue;
            nReturn = SCardListReaderGroups(hContext, ref pmszGroups, ref pcchGroups);
            if (nReturn == 0)
            {
                string text = Marshal.PtrToStringAuto(pmszGroups, (int)pcchGroups - 2);
                string[] readerGroups = text.Split(delimiter);
                nReturn = SCardFreeMemory(hContext, pmszGroups);

                // Get reader lists
                IntPtr pmszReader = IntPtr.Zero;
                uint pcchReaders = uint.MaxValue;
                nReturn = SCardListReaders(hContext, readerGroups[0], ref pmszReader, ref pcchReaders);
                if (nReturn != 0) return readerLists;

                text = Marshal.PtrToStringAuto(pmszReader, (int)pcchReaders - 2);
                readerLists = text.Split(delimiter);
                nReturn = SCardFreeMemory(hContext, pmszReader);
            }

            SCardReleaseContext(hContext);

            return readerLists;
        }
        public static bool GetCardStatus(string readerName)
        {
            IntPtr hContext = IntPtr.Zero;
            int nReturn = SCardEstablishContext(2, IntPtr.Zero, IntPtr.Zero, out hContext);
            if (nReturn != 0) return false;

            SCARD_READERSTATE[] aState = new SCARD_READERSTATE[1];
            aState[0].szReader = readerName;
            aState[0].pvUserData = IntPtr.Zero;
            aState[0].dwCurrentState = (uint)0;
            aState[0].dwEventState = 0;
            aState[0].cbAtr = 0;
            aState[0].rgbAtr = null;

            nReturn = SCardGetStatusChange(hContext, (uint)0, aState, (uint)1);

            bool state = false;
            if (nReturn == 0)
                if ((aState[0].dwEventState & 0x20) > 0) state = true;

            SCardReleaseContext(hContext);
            return state;
        }
        public static string GetSerialNumber(string readerName)
        {
            IntPtr hContext = IntPtr.Zero;
            int nReturn = SCardEstablishContext(2, IntPtr.Zero, IntPtr.Zero,
                out hContext);
            if (nReturn != 0) return string.Empty;

            // Do direct connect
            IntPtr hCard = IntPtr.Zero;
            uint dwShareMode = 3;           // SCARD_SHARE_DIRECT
            uint dwPreferredProtocols = 0;  // SCARD_PROTOCOL_T1 || SCARD_PROTOCOL_T2
            uint pdwActiveProtocol = 0;
            nReturn = SCardConnect(hContext,
                readerName,
                dwShareMode,
                dwPreferredProtocols,
                out hCard,
                out pdwActiveProtocol);

            string serial = String.Empty;

            if (nReturn == 0)
            {
                // Get smartcard reader serial no attributes
                uint attrID = 0x00010103;	        // SSCARD_ATTR_VENDOR_IFD_SERIAL_NO 
                IntPtr attrBytesPtr = IntPtr.Zero;
                uint attrStringSize = uint.MaxValue;	// SCARD_AUTOALLOCATE

                nReturn = SCardGetAttrib(hCard, attrID, ref attrBytesPtr, ref attrStringSize);
                if (nReturn == 0)
                {
                    serial = Marshal.PtrToStringAnsi(attrBytesPtr, (int)attrStringSize);
                    SCardFreeMemory(hContext, attrBytesPtr);
                }

                SCardDisconnect(hCard, 0);
            }

            SCardReleaseContext(hContext);

            return serial;
        }


        #region WINSCARD DLL Import APIS
        [DllImport("WINSCARD.DLL")]
        public static extern int SCardEstablishContext(uint dwScope, IntPtr pvReserved1, IntPtr pvReserved2, out IntPtr phContext);

        [DllImport("WINSCARD.DLL")]
        public static extern int SCardReleaseContext(IntPtr hContext);

        [DllImport("WINSCARD.DLL", CharSet = CharSet.Auto)]
        public static extern int SCardListReaders(IntPtr hContext, string mszGroups, ref IntPtr pmszReaders, ref uint pcchReaders);

        [DllImport("WINSCARD.DLL", CharSet = CharSet.Auto)]
        public static extern int SCardListReaderGroups(IntPtr hContext, ref IntPtr pmszGroups, ref uint pcchGroups);

        [DllImport("WINSCARD.DLL")]
        public static extern int SCardFreeMemory(IntPtr hContext, IntPtr pvMem);

        [DllImport("WINSCARD.DLL")]
        public static extern int SCardDisconnect(IntPtr hCard, uint dwDisposition);

        [DllImport("WINSCARD.DLL", CharSet = CharSet.Auto)]
        public static extern int SCardGetStatusChange(IntPtr hContext, uint dwTimeout, [In, Out] SCARD_READERSTATE[] rgReaderStates, uint cReaders);

        [DllImport("WINSCARD.DLL", CharSet = CharSet.Auto)]
        public static extern int SCardConnect(IntPtr hContext, string szReader, uint dwShareMode, uint dwPreferredProtocols, out IntPtr phCard, out uint pdwActiveProtocol);

        [DllImport("WINSCARD.DLL")]
        public static extern int SCardGetAttrib(IntPtr hCard, uint dwAttrId, ref IntPtr pbAttr, ref uint pcbAttrLen);

        [DllImport("WINSCARD.DLL")]
        public static extern int SCardTransmit(IntPtr hCard, IntPtr pioSendPci, [In] byte[] pbSendBuffer, uint cbSendLength, SCARD_IO_REQUEST pioRecvPci, [In, Out] byte[] pbRecvBuffer, ref uint pcbRecvLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll")]
        internal static extern void FreeLibrary(IntPtr handle);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetProcAddress(IntPtr handle, string procName);

        public static IntPtr GetPciT0()
        {
            IntPtr handle = LoadLibrary("winscard.dll");
            IntPtr pci = GetProcAddress(handle, "g_rgSCardT0Pci");
            FreeLibrary(handle);
            return pci;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SCARD_READERSTATE
        {
            internal string szReader;
            internal IntPtr pvUserData;
            internal uint dwCurrentState;
            internal uint dwEventState;
            internal uint cbAtr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x24, ArraySubType = UnmanagedType.U1)]
            internal byte[] rgbAtr;
        }

        // Private/Internal Types
        [StructLayout(LayoutKind.Sequential)]
        public class SCARD_IO_REQUEST
        {
            internal uint dwProtocol;
            internal uint cbPciLength;
            public SCARD_IO_REQUEST()
            {
                dwProtocol = 0;
            }
        }

        #endregion
    }
}