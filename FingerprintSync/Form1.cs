using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.Win32;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Drawing;
namespace FingerprintSync
{
    public partial class Form1 : Form
    {
        private int iMachineNumber = 1;
        private static Form1 THIS = null;
        public static zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
        private List<AttendanceUser> allUserList = new List<AttendanceUser>();
        public Form1()
        {
            InitializeComponent();
            this.fromTo.Text = Config.getFrom() + "-->" + Config.getTo();
            btnCheckAll.Enabled = false;
            this.btnSync.Enabled = false;

            THIS = this;

        }
        private void btnSync_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(delegate()
            {
                btnSync_Click();
            }
            );
            t1.IsBackground = true; //当主线程退出时，后台线程会被CLR调用Abort()来彻底终止程序
            t1.Start();
        }
        private void btnSync_Click()
        {
            string from = Config.getFrom();
            string to = Config.getTo();
            if (from != "" && to != "")
            {
                string[] ds = from.Split(new char[] { ',' });
                bool bIsConnected = axCZKEM1.Connect_Net(ds[0], Convert.ToInt32(ds[1]));
                axCZKEM1.ReadAllTemplate(iMachineNumber);//读取所有的指纹数据到pc寄存器中，加快处理速度
                Dictionary<string, AttendanceUser> map = new Dictionary<string, AttendanceUser>();
                for (int i = 0; i < allUserList.Count; i++)
                {
                    map.Add(allUserList[i].userNo, allUserList[i]);
                }
                for (int i = 0; i < cblAllUserList.Items.Count; i++)
                {
                    if (cblAllUserList.GetItemChecked(i))
                    {
                        logBox.BeginInvoke((MethodInvoker)delegate()
                        {
                            addLog(cblAllUserList.Items[i].ToString(), Color.Black);
                        });
                        string[] userInfos = Regex.Split(cblAllUserList.Items[i].ToString(), "\t\t");
                        string userNo = userInfos[0];
                        string userName = userInfos[1];
                        List<Fingerprint> fingerList = getFingerprintListFromDevice(userNo, from, axCZKEM1);
                        logBox.BeginInvoke((MethodInvoker)delegate()
                        {
                            addLog("\t\t指纹数量 : " + fingerList.Count, Color.Gray);
                        });
                        Thread.Sleep(10);
                        string face = getUserFaceFromDevice(userNo, from, axCZKEM1);
                        if (face != null && face != "")
                        {
                            map[userNo].face = face;
                            logBox.BeginInvoke((MethodInvoker)delegate()
                            {
                                addLog("\t\t面部数据 ", Color.Gray);
                            });
                        }
                        map[userNo].fingerprintList = fingerList;
                    }
                }
                axCZKEM1.Disconnect();
                if (map.Count > 0)
                {
                    ds = to.Split(new char[] { ';' });
                    for (int jj = 0; jj < ds.Length; jj++)
                    {
                        string[] dd = ds[jj].Split(new char[] { ',' });
                        bIsConnected = axCZKEM1.Connect_Net(dd[0], Convert.ToInt32(dd[1]));
                        if (bIsConnected)
                        {
                            for (int i = 0; i < cblAllUserList.Items.Count; i++)
                            {
                                if (cblAllUserList.GetItemChecked(i))
                                {
                                    string[] userInfos = Regex.Split(cblAllUserList.Items[i].ToString(), "\t\t");
                                    string userNo = userInfos[0];
                                    try
                                    {
                                        logBox.BeginInvoke((MethodInvoker)delegate()
                                        {
                                            addLog("设置[" + ds[jj] + "]\t" + map[userNo].name, Color.Gray);
                                        });
                                        axCZKEM1.SSR_SetUserInfo(1, map[userNo].userNo, map[userNo].name, map[userNo].password, map[userNo].manager ? Config.Privilege_Administrator : Config.Privilege_General, true);
                                        if (map[userNo].face != null)
                                        {
                                            //第三参数只能填写50
                                            axCZKEM1.SetUserFaceStr(1, map[userNo].userNo, 50, map[userNo].face, map[userNo].face.Length);
                                        }
                                        if (map[userNo].fingerprintList != null)
                                        {
                                            for (int j = 0; j < map[userNo].fingerprintList.Count; j++)
                                            {
                                                //addLog("设置指纹" + u.name, false);
                                                axCZKEM1.SetUserTmpExStr(1, map[userNo].userNo, map[userNo].fingerprintList[j].index, 1, map[userNo].fingerprintList[j].fingerprint);
                                            }
                                        }
                                    }
                                    catch (Exception exxx)
                                    {
                                        logBox.BeginInvoke((MethodInvoker)delegate()
                                        {
                                            addLog("设置[" + ds[jj] + "]\t" + map[userNo].name + "\t发生异常", Color.Black);
                                        });
                                    }

                                }
                            }

                        }
                        axCZKEM1.Disconnect();
                    }
                    logBox.BeginInvoke((MethodInvoker)delegate()
                    {
                        addLog("已恢复用户数据", Color.Black);
                    });
                }
                else
                {
                    MessageBox.Show("请检查考勤机配置文件", "请确认配置文件中devices节点\n配置文件 " + Config.getConfigPath(), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
        }

        /// <summary>
        /// 从考勤机读取用户列表
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        private void readUserListFromDevice(string device, zkemkeeper.CZKEMClass axCZKEM1)
        {
            int idwErrorCode = 0;
            string sdwEnrollNumber = "";
            string sdwName = "";
            string sdwPassword = "";
            int sdwPrivilege = 0;
            bool sdwEnabled = false;
            if (axCZKEM1.ReadAllUserID(iMachineNumber))
            {
                while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sdwEnrollNumber, out sdwName, out sdwPassword, out sdwPrivilege, out sdwEnabled))
                {
                    AttendanceUser user = new AttendanceUser();
                    user.userNo = sdwEnrollNumber;
                    string name = sdwName;
                    int index = name.IndexOf('\0');
                    if (index > 0)
                    {
                        name = name.Substring(0, index);
                    }
                    user.name = name;
                    user.password = sdwPassword;
                    user.manager = sdwPrivilege == Config.Privilege_Administrator;
                    allUserList.Add(user);
                }
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                if (idwErrorCode != 0)
                {
                    addLog("从考勤机读取用户列表异常,错误代码: " + idwErrorCode.ToString(), Color.Black);
                }
                else
                {
                    addLog("考勤机中没有用户数据!" + device, Color.Black);
                }
            }

        }
        /// <summary>
        /// 从考勤机读取指纹列表
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private List<Fingerprint> getFingerprintListFromDevice(string userNo, string device, zkemkeeper.CZKEMClass axCZKEM1)
        {
            List<Fingerprint> fingerList = new List<Fingerprint>();
            int idwFingerIndex = 0;
            int iTmpLength = 0;
            int iFlag = 0;
            string sTmpData = "";

            for (idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
            {
                if (axCZKEM1.GetUserTmpExStr(iMachineNumber, userNo, idwFingerIndex, out iFlag, out sTmpData, out iTmpLength))
                {
                    Fingerprint finger = new Fingerprint();
                    finger.index = idwFingerIndex;
                    finger.fingerprint = sTmpData;
                    fingerList.Add(finger);
                }
            }
            return fingerList;

        }

        /// <summary>
        /// 从考勤机读取面部识别
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private string getUserFaceFromDevice(string userNo, string device, zkemkeeper.CZKEMClass axCZKEM1)
        {
            string[] ds = device.Split(new char[] { ',' });
            //bool bIsConnected = axCZKEM1.Connect_Net(ds[0], Convert.ToInt32(ds[1]));
            int idwErrorCode = 0;
            int iTmpLength = 0;
            int iFaceIndex = 50;//the only possible parameter value
            string face = "";
            //addLog(userNo, false);
            if (axCZKEM1.GetUserFaceStr(iMachineNumber, userNo, iFaceIndex, ref face, ref iTmpLength))
            {
                return face;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                if (idwErrorCode != 0)
                {
                    if (idwErrorCode != -4993)
                    {
                        addLog("从考勤机读取用户" + userNo + "的面部数据异常,错误代码: " + idwErrorCode.ToString(), Color.Black);
                    }
                }
                else
                {
                    addLog("考勤机中用户" + userNo + "的面部数据数据!" + device, Color.Black);
                }
            }
            //axCZKEM1.Disconnect();
            return face;

        }


        /// <summary>
        /// 写日志到控制台及日志文件
        /// </summary>
        /// <param name="info">日志内容</param>
        /// <param name="bWriteLog">是否写入到日志文件，true：写，false：不写</param>
        public void addLog(string info, Color color)
        {
            String content = DateTime.Now.ToString("MM-dd HH:mm:ss") + "  " + info + "\r\n";
            int len = logBox.TextLength;
            logBox.AppendText(content);
            if (color != null)
            {
                // logBox.Select(len, len + content.Length);
                // logBox.SelectionColor = color;

            }
        }

        private void btnReadAll_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(delegate()
            {
                readAll_Click();
            }
            );
            t1.IsBackground = true; //当主线程退出时，后台线程会被CLR调用Abort()来彻底终止程序
            t1.Start();

        }
        public void readAll_Click()
        {
            btnReadAll.BeginInvoke((MethodInvoker)delegate()
            {
                string from = Config.getFrom();
                if (from != "")
                {
                    string[] ds = from.Split(new char[] { ',' });
                    bool bIsConnected = axCZKEM1.Connect_Net(ds[0], Convert.ToInt32(ds[1]));
                    if (bIsConnected)
                    {
                        //addLog("开始读取员工列表", false);
                        logBox.AppendText(DateTime.Now.ToString("MM-dd HH:mm:ss") + " 开始读取员工列表 \r\n");
                        readUserListFromDevice(from, axCZKEM1);
                        if (allUserList.Count > 0)
                        {
                            allUserList.Reverse();
                            axCZKEM1.ReadAllTemplate(iMachineNumber);//读取所有的指纹数据到pc寄存器中，加快处理速度
                            foreach (AttendanceUser user in allUserList)
                            {
                                cblAllUserList.Items.Add(user.userNo + "\t\t" + user.name);
                                this.btnCheckAll.Enabled = true;
                                this.btnSync.Enabled = true;
                            }
                            logBox.AppendText(DateTime.Now.ToString("MM-dd HH:mm:ss") + " 员工数量: " + allUserList.Count + "  \r\n");

                            //logBox.AppendText(DateTime.Now.ToString("MM-dd HH:mm:ss") + "  " + info + "\r\n");
                            //addLog("员工数量" + allUserList.Count + "个用户", true);

                        }
                        axCZKEM1.Disconnect();
                    }
                    else
                    {
                        MessageBox.Show("请检查考勤机配置文件", "请确认配置文件中devices节点\n配置文件 " + Config.getConfigPath(), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                }
            });
        }

        private void btnDeleteLeave_Click(object sender, EventArgs e)
        {
            string returnValue = "";
            List<string> leaveList = new List<string>();
            try
            {
                returnValue = HttpUtil.sendRequest(Config.getLeaveUrl());
                JsonResponse res = JsonConvert.DeserializeObject<JsonResponse>(returnValue);
                if (res.code == 10000)
                {
                    JArray data = (JArray)(res.data);
                    foreach (var item in data)
                    {
                        string userNo = (string)item["userNo"];
                        leaveList.Add(userNo);
                        addLog((String)userNo, Color.Black);
                    }
                    //leaveList.Add("8600091");
                    if (leaveList.Count > 0)
                    {
                        string from = Config.getFrom();
                        string to = Config.getTo();
                        string[] ds = from.Split(new char[] { ',' });
                        bool bIsConnected = axCZKEM1.Connect_Net(ds[0], Convert.ToInt32(ds[1]));
                        if (bIsConnected)
                        {
                            for (int i = 0; i < leaveList.Count; i++)
                            {
                                bool rf = axCZKEM1.DelUserFace(1, leaveList[i], 50);
                                axCZKEM1.SetUserInfo(1, int.Parse(leaveList[i]), "aaa", "", 0, true);
                                axCZKEM1.ModifyPrivilege(1, int.Parse(leaveList[i]), 1, 1, 0);
                                for (int k = 0; k < 10; k++)
                                {
                                    axCZKEM1.SSR_DelUserTmp(1, leaveList[i], k);
                                }
                                bool ru2 = axCZKEM1.SSR_DeleteEnrollData(1, leaveList[i], 10);
                                bool ru1 = axCZKEM1.SSR_DeleteEnrollData(1, leaveList[i], 11);
                                bool ru = axCZKEM1.SSR_DeleteEnrollData(1, leaveList[i], 12);
                                bool r = axCZKEM1.DeleteUserInfoEx(1, int.Parse(leaveList[i]));
                                addLog("删除离职  " + leaveList[i], Color.Black);
                            }
                        }
                        axCZKEM1.Disconnect();


                        ds = to.Split(new char[] { ';' });
                        for (int jj = 0; jj < ds.Length; jj++)
                        {
                            string[] dd = ds[jj].Split(new char[] { ',' });
                            bIsConnected = axCZKEM1.Connect_Net(dd[0], Convert.ToInt32(dd[1]));
                            if (bIsConnected)
                            {
                                for (int i = 0; i < leaveList.Count; i++)
                                {
                                    bool rf = axCZKEM1.DelUserFace(1, leaveList[i], 50);
                                    axCZKEM1.SetUserInfo(1, int.Parse(leaveList[i]), "aaa", "", 0, true);
                                    axCZKEM1.ModifyPrivilege(1, int.Parse(leaveList[i]), 1, 1, 0);
                                    for (int k = 0; k < 10; k++)
                                    {
                                        axCZKEM1.SSR_DelUserTmp(1, leaveList[i], k);
                                    }
                                    bool ru2 = axCZKEM1.SSR_DeleteEnrollData(1, leaveList[i], 10);
                                    bool ru1 = axCZKEM1.SSR_DeleteEnrollData(1, leaveList[i], 11);
                                    bool ru = axCZKEM1.SSR_DeleteEnrollData(1, leaveList[i], 12);
                                    bool r = axCZKEM1.DeleteUserInfoEx(1, int.Parse(leaveList[i]));
                                    addLog("删除离职  " + leaveList[i], Color.Black);
                                }
                                axCZKEM1.Disconnect();
                            }
                        }
                    }
                }
                else
                {
                    addLog("OA服务端异常!" + returnValue, Color.Black);
                }
            }
            catch (Exception ex)
            {
                returnValue = ex.Message + ex.StackTrace;
                addLog("OA响应异常!" + returnValue, Color.Black);
            }
        }

        private void allUserList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            if (this.cblAllUserList.Items.Count == 0)
            {
                return;
            }
            for (int i = 0; i < this.cblAllUserList.Items.Count; i++)
            {
                this.cblAllUserList.SetItemChecked(i, true);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.cblAllUserList.Items.Count == 0)
            {
                return;
            }
            for (int i = 0; i < this.cblAllUserList.Items.Count; i++)
            {
                this.cblAllUserList.SetItemChecked(i, false);
            }
        }

        private void btnManulDeleteLeave_Click(object sender, EventArgs e)
        {
            string from = Config.getFrom();
            string to = Config.getTo();
            List<string> leaveList = new List<string>();
            for (int i = 0; i < cblAllUserList.Items.Count; i++)
            {
                if (cblAllUserList.GetItemChecked(i))
                {
                    leaveList.Add(cblAllUserList.Items[i].ToString().Split(new char[] { '\t' })[0]);
                    logBox.BeginInvoke((MethodInvoker)delegate()
                    {
                        //addLog(cblAllUserList.Items[i].ToString().Split(new char[]{'\t'})[0], Color.Black);
                    });
                     
                }
            }
            if(leaveList.Count == 0){
                return;
            }
            bool bIsConnected = false;
            string[] ds = null;
            if(to != "")
            {
                ds = to.Split(new char[] { ';' });
                for (int jj = 0; jj < ds.Length; jj++)
                {
                    string[] dd = ds[jj].Split(new char[] { ',' });
                    bIsConnected = axCZKEM1.Connect_Net(dd[0], Convert.ToInt32(dd[1]));
                    if (bIsConnected)
                    {
                        for (int i = 0; i < leaveList.Count; i++)
                        {
                            bool rf = axCZKEM1.DelUserFace(1, leaveList[i], 50);
                            axCZKEM1.SetUserInfo(1, int.Parse(leaveList[i]), "aaa", "", 0, true);
                            axCZKEM1.ModifyPrivilege(1, int.Parse(leaveList[i]), 1, 1, 0);
                            for (int k = 0; k < 10; k++)
                            {
                                axCZKEM1.SSR_DelUserTmp(1, leaveList[i], k);
                            }
                            bool ru2 = axCZKEM1.SSR_DeleteEnrollData(1, leaveList[i], 10);
                            bool ru1 = axCZKEM1.SSR_DeleteEnrollData(1, leaveList[i], 11);
                            bool ru = axCZKEM1.SSR_DeleteEnrollData(1, leaveList[i], 12);
                            bool r = axCZKEM1.DeleteUserInfoEx(1, int.Parse(leaveList[i]));
                            addLog("删除离职  " + leaveList[i], Color.Black);
                        }
                        axCZKEM1.Disconnect();
                    }
                }
            }
            if (from != "" )
            {
                ds = from.Split(new char[] { ',' });
                bIsConnected = axCZKEM1.Connect_Net(ds[0], Convert.ToInt32(ds[1]));
                if (bIsConnected)
                {
                    for (int i = 0; i < leaveList.Count; i++)
                    {
                        bool rf = axCZKEM1.DelUserFace(1, leaveList[i], 50);
                        axCZKEM1.SetUserInfo(1, int.Parse(leaveList[i]), "aaa", "", 0, true);
                        axCZKEM1.ModifyPrivilege(1, int.Parse(leaveList[i]), 1, 1, 0);
                        for (int k = 0; k < 10; k++)
                        {
                            axCZKEM1.SSR_DelUserTmp(1, leaveList[i], k);
                        }
                        bool ru2 = axCZKEM1.SSR_DeleteEnrollData(1, leaveList[i], 10);
                        bool ru1 = axCZKEM1.SSR_DeleteEnrollData(1, leaveList[i], 11);
                        bool ru = axCZKEM1.SSR_DeleteEnrollData(1, leaveList[i], 12);
                        bool r = axCZKEM1.DeleteUserInfoEx(1, int.Parse(leaveList[i]));
                        addLog("删除离职  " + leaveList[i], Color.Black);
                    }
                }
                axCZKEM1.Disconnect();

             }
             logBox.BeginInvoke((MethodInvoker)delegate()
            {
                addLog("已删除用户数据", Color.Black);
            });
        }
    }
}
