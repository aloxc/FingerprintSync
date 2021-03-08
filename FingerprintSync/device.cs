using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerprintSync
{
    class device
    {
    }
    class DeviceUtil
    {
        private static int iMachineNumber = 1;


        public static List<Attendance> getAttendanceList(zkemkeeper.CZKEMClass axCZKEM1, string ip, int port, string deviceSN)
        {
            //Console.WriteLine("aaaaa");
            int idwErrorCode = 0;
            List<Attendance> list = new List<Attendance>();
            string sdwEnrollNumber = "";
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;
            int iMachineNumber = 1;
            bool bIsConnected = axCZKEM1.Connect_Net(ip, port);

            //axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM1.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
            {
                while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                            out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {
                    Attendance att = new Attendance();
                    att.userNo = sdwEnrollNumber;
                    att.deviceSerialNo = deviceSN;
                    if (idwVerifyMode == 1)
                    {
                        att.attendanceMode = "FINGERPRINT";
                    }
                    else if (idwVerifyMode == 15)
                    {
                        att.attendanceMode = "FACE";
                    }
                    else if (idwVerifyMode == 0)
                    {
                        att.attendanceMode = "PASSWORD";
                    }
                    else
                    {
                        att.attendanceMode = "PASSWORD";
                    }
                    string month = idwMonth.ToString();
                    if (month.Length == 1)
                    {
                        month = "0" + month;
                    }
                    string day = idwDay.ToString();
                    if (day.Length == 1)
                    {
                        day = "0" + day;
                    }
                    string date = idwYear.ToString() + "-" + month + "-" + day +
                        " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":00";
                    att.createDate = date;
                    list.Add(att);
                }

            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    axCZKEM1.Disconnect();
                    throw new Exception("[" + ip + "]读取考勤数据发生错误，错误代码: " + idwErrorCode.ToString());
                }
            }
            axCZKEM1.Disconnect();
            return list;
        }

    }
}
