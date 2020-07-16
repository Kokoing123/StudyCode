using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Basler.Pylon;
using HalconDotNet;

namespace PylonLiveViewer
{
    public partial class Form1 : Form
    {
        BaslerCam mCamera1;
        BaslerCam mCamera2;
        

        long count1 = 0;
        long count2 = 0;

        bool grab1 = false;
        bool grab2 = false;
        

        HObject hImageSave1 = null;
        HObject hImageSave2 = null;
        HObject hImageLoad1 = null;
        HObject hImageLoad2 = null;

        string Imagepath1 = null;

        List<int> usedlist = new List<int>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            UpdateDeviceList();
            usedlist.Add(-1);
            SetBottonEnable1(false, false, false);
            SetBottonEnable2(false, false, false);
            groupBox3.Enabled = false;
            groupBox4.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;

        }

        // 计算相机1采集图像计数
        private void computeGrabTime1(long time)
        {
            ++count1;
            lblCount1.Text = "[  Count1 : " + count1 + "  ]";
        }
        // 计算相机2采集图像计数
        private void computeGrabTime2(long time)
        {
            ++count2;
            lblCount2.Text = "[  Count2 : " + count2 + "  ]";
        }


        // 相机1 halcon窗体显示图像
        private void processHImage1(HObject hImg)
        {
            //hWindowControl1.HalconWindow.ClearWindow();
            hWindowControl1.HalconWindow.SetPart(0, 0, -1, -1);
            hWindowControl1.HalconWindow.DispObj(hImg);

            HOperatorSet.GenEmptyObj(out hImageSave1);
            hImageSave1.Dispose();
            HOperatorSet.CopyImage(hImg, out hImageSave1);
        }
        // 相机2 halcon窗体显示图像
        private void processHImage2(HObject hImg)
        {
            //hWindowControl2.HalconWindow.ClearWindow();
            hWindowControl2.HalconWindow.SetPart(0, 0, -1, -1);
            hWindowControl2.HalconWindow.DispObj(hImg);

            HOperatorSet.GenEmptyObj(out hImageSave2);
            hImageSave2.Dispose();
            HOperatorSet.CopyImage(hImg, out hImageSave2);
        }


        // 相机1单张采集
        private void btnSingleFrame1_Click(object sender, EventArgs e)
        {
            mCamera1.GrabOne();
        }
        // 相机1开始采集
        private void btnStart1_Click(object sender, EventArgs e)
        {
            if (mCamera1.StartGrabbing())
            {
                grab1 = true;
                SetBottonEnable1(false, true, false);

            }
        }
        // 相机1停止采集
        private void btnStop1_Click(object sender, EventArgs e)
        {
            mCamera1.StopGrabbing();
            SetBottonEnable1(true, false, true);
            grab1 = false;
        }

        // 相机2单张采集
        private void btnSingleFrame2_Click(object sender, EventArgs e)
        {
            mCamera2.GrabOne();

        }
        // 相机2开始采集
        private void btnStart2_Click(object sender, EventArgs e)
        {
            if (mCamera2.StartGrabbing())
            {
                grab2 = true;
                SetBottonEnable2(false, true, false);
            }
        }
        // 相机2停止采集
        private void btnStop2_Click(object sender, EventArgs e)
        {
            mCamera2.StopGrabbing();
            SetBottonEnable2(true, false, true);
            grab2 = false;
        }

        // 设置相机１为Freerun模式
        private void rdbFreerun1_CheckedChanged(object sender, EventArgs e)
        {
            mCamera1.SetFreerun();
            btnExecute1.Enabled = false;
        }
        // 设置相机１为软触发模式
        private void rdbSWTrigger1_CheckedChanged(object sender, EventArgs e)
        {
            mCamera1.SetSoftwareTrigger();
            btnExecute1.Enabled = true;
        }
        // 设置相机１为外触发模式
        private void rdbHWTrigger1_CheckedChanged(object sender, EventArgs e)
        {
            mCamera1.SetExternTrigger();
            btnExecute1.Enabled = false;
        }
        // 执行相机1软触发命令
        private void btnExecute1_Click(object sender, EventArgs e)
        {
            mCamera1.SendSoftwareExecute();
        }


        // 设置相机2为Freerun模式
        private void rdbFreerun2_CheckedChanged(object sender, EventArgs e)
        {
            mCamera2.SetFreerun();
            btnExecute2.Enabled = false;
        }
        // 设置相机2为软触发模式
        private void rdbSWTrigger2_CheckedChanged(object sender, EventArgs e)
        {
            mCamera2.SetSoftwareTrigger();
            btnExecute2.Enabled = true;
        }
        // 设置相机2为外触发模式
        private void rdbHWTrigger2_CheckedChanged(object sender, EventArgs e)
        {
            mCamera2.SetExternTrigger();
            btnExecute2.Enabled = false;
        }
        // 执行相机2软触发命令
        private void btnExecute2_Click(object sender, EventArgs e)
        {
            mCamera2.SendSoftwareExecute();
        }


        private void tkbExposure1_Scroll(object sender, EventArgs e)
        {
            int value = tkbExposure1.Value;
            mCamera1.SetExposureTime(value);
            txtBExposure1.Text = Convert.ToString(value);
        }

        private void tkbGain1_Scroll(object sender, EventArgs e)
        {
            int value = tkbGain1.Value;
            mCamera1.SetGain(value);
            txtBGain1.Text = Convert.ToString(value);
        }

        private void tkbExposure2_Scroll(object sender, EventArgs e)
        {
            int value = tkbExposure2.Value;
            mCamera2.SetExposureTime(value);
            txtBExposure2.Text = Convert.ToString(value);
        }

        private void tkbGain2_Scroll(object sender, EventArgs e)
        {
            int value = tkbGain2.Value;
            mCamera2.SetGain(value);
            txtBGain2.Text = Convert.ToString(value);
        }

        private void txtBExposure1_TextChanged(object sender, EventArgs e)
        {
            int value = 0;
            int.TryParse(txtBExposure1.Text, out value);
            mCamera1.SetExposureTime(value);
        }

        private void txtBGain1_TextChanged(object sender, EventArgs e)
        {
            int value = 0;
            int.TryParse(txtBGain1.Text, out value);
            mCamera1.SetGain(value);
        }

        private void txtBExposure2_TextChanged(object sender, EventArgs e)
        {
            int value = 0;
            int.TryParse(txtBExposure2.Text, out value);
            mCamera2.SetExposureTime(value);
        }

        private void txtBGain2_TextChanged(object sender, EventArgs e)
        {
            int value = 0;
            int.TryParse(txtBGain2.Text, out value);
            mCamera2.SetGain(value);
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hImageSave1 == null)
            {
                MessageBox.Show("当前窗体图片为空！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    HOperatorSet.WriteImage(hImageSave1, "bmp", 0, (HTuple)saveFileDialog1.FileName);
                }
            }
        }

        private void 加载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                HOperatorSet.ReadImage(out hImageLoad1, (HTuple)openFileDialog1.FileName);
                hWindowControl1.HalconWindow.ClearWindow();
                hWindowControl1.HalconWindow.SetPart(0, 0, -1, -1);
                hWindowControl1.HalconWindow.DispObj(hImageLoad1);

                HOperatorSet.GenEmptyObj(out hImageSave1);
                hImageSave1.Dispose();
                HOperatorSet.CopyImage(hImageLoad1, out hImageSave1);
            }
        }

        private void 保存ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (hImageSave2 == null)
            {
                MessageBox.Show("当前窗体图片为空！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    HOperatorSet.WriteImage(hImageSave2, "bmp", 0, (HTuple)saveFileDialog1.FileName);
                }
            }
        }

        private void 加载ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                HOperatorSet.ReadImage(out hImageLoad2, (HTuple)openFileDialog1.FileName);
                hWindowControl2.HalconWindow.ClearWindow();
                hWindowControl2.HalconWindow.SetPart(0, 0, -1, -1);
                hWindowControl2.HalconWindow.DispObj(hImageLoad2);

                HOperatorSet.GenEmptyObj(out hImageSave2);
                hImageSave2.Dispose();
                HOperatorSet.CopyImage(hImageLoad2, out hImageSave2);
            }
        }

        private void SetBottonEnable1(bool startEnable, bool stopEnable, bool singleEnable)
        {
            btnStart1.Enabled = startEnable;
            btnStop1.Enabled = stopEnable;
            btnSingleFrame1.Enabled = singleEnable;
        }
        private void SetBottonEnable2(bool startEnable, bool stopEnable, bool singleEnable)
        {
            btnStart2.Enabled = startEnable;
            btnStop2.Enabled = stopEnable;
            btnSingleFrame2.Enabled = singleEnable;
        }

        // 异常信息显示
        private void ShowException(Exception exception)
        {
            MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        //窗口关闭事件
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (grab1)
            {
                mCamera1.StopGrabbing();
            }
            
            if (grab2)
            {
                mCamera2.StopGrabbing();
            }

            if (usedlist.Contains(0))
            {
                mCamera1.CloseCam();
            }
            if (usedlist.Contains(1))
            {
                mCamera2.CloseCam();
            }
            
        }

        private void hWindowControl1_HMouseMove(object sender, HMouseEventArgs e)
        {

        }

        #region old update list
        //private string[] UpdateDeviceList()
        //{
        //    try
        //    {
        //        // Ask the camera finder for a list of camera devices.
        //        List<ICameraInfo> allCameras = CameraFinder.Enumerate();
        //        var SerialList = new List<string>();//创建了一个空列表
        //        // Loop over all cameras found.
        //        foreach (ICameraInfo cameraInfo in allCameras)
        //        {
        //            string name = cameraInfo[CameraInfoKey.FriendlyName];
        //            Regex rex = new Regex(@"\(([^)]*)\)");
        //            string serial = rex.Match(name).Groups[1].ToString();
        //            SerialList.Add(serial);
        //        }
        //        string[] Serials = SerialList.ToArray();
        //        return Serials;
        //    }
        //    catch (Exception exception)
        //    {
        //        ShowException(exception);
        //        return null;
        //    }

        //}
        //加载图片
        //更新可用设备列表
        #endregion

        //更新相机列表
        private void UpdateDeviceList()
        {
            try
            {
                // Ask the camera finder for a list of camera devices.
                List<ICameraInfo> allCameras = CameraFinder.Enumerate();

                ListView.ListViewItemCollection items = devicelist.Items;

                // Loop over all cameras found.
                foreach (ICameraInfo cameraInfo in allCameras)
                {
                    // Loop over all cameras in the list of cameras.
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        ICameraInfo tag = item.Tag as ICameraInfo;

                        // Is the camera found already in the list of cameras?
                        if (tag[CameraInfoKey.FullName] == cameraInfo[CameraInfoKey.FullName])
                        {
                            tag = cameraInfo;
                            newitem = false;
                            break;
                        }
                    }

                    // If the camera is not in the list, add it to the list.
                    if (newitem)
                    {
                        // Create the item to display.
                        ListViewItem item = new ListViewItem(cameraInfo[CameraInfoKey.FriendlyName]);

                        // Create the tool tip text.
                        string toolTipText = "";
                        foreach (KeyValuePair<string, string> kvp in cameraInfo)
                        {
                            toolTipText += kvp.Key + ": " + kvp.Value + "\n";
                        }
                        item.ToolTipText = toolTipText;

                        // Store the camera info in the displayed item.
                        item.Tag = cameraInfo;

                        // Attach the device data.
                        devicelist.Items.Add(item);
                    }
                }



                // Remove old camera devices that have been disconnected.
                foreach (ListViewItem item in items)
                {
                    bool exists = false;

                    // For each camera in the list, check whether it can be found by enumeration.
                    foreach (ICameraInfo cameraInfo in allCameras)
                    {
                        if (((ICameraInfo)item.Tag)[CameraInfoKey.FullName] == cameraInfo[CameraInfoKey.FullName])
                        {
                            exists = true;
                            break;
                        }
                    }
                    // If the camera has not been found, remove it from the list view.
                    if (!exists)
                    {
                        devicelist.Items.Remove(item);
                    }
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }
        //选择图片
        private void button1_Click(object sender, EventArgs e)
        {
            if (!grab1)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = false;   //是否允许多选
                dialog.Title = "请选择要处理的文件";  //窗口title
                dialog.Filter = "图像|*.*";   //可选择的文件类型
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Imagepath1 = dialog.FileName;  //获取文件路径
                    path1.Text = Imagepath1.ToString();
                }
                button2.Enabled = true;
            }

            else
            {
                MessageBox.Show("先停止相机采集");
            }

        }
        //设置图片
        private void button2_Click(object sender, EventArgs e)
        {
            List<int> paramters = new List<int>();
            paramters.Add(Convert.ToInt16(textBox1.Text));
            paramters.Add(Convert.ToInt16(textBox2.Text));
            int[] paras = paramters.ToArray();
            mCamera1.SetTestImage(Imagepath1, paras);
            status1.Text = "设置成功，可以采集！";
        }
        //手动刷新
        private void updatelist_Click(object sender, EventArgs e)
        {
            UpdateDeviceList();
        }
        //列表点击事件
        private void devicelist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (devicelist.SelectedItems.Count == 0)
                return;
            else
            {
                //List<string> list = new List<string>();
                string devicetxt= devicelist.SelectedItems[0].Text.ToString();
                //正则匹配相机序列号
                Regex rex = new Regex(@"\(([^)]*)\)");
                string serial = rex.Match(devicetxt).Groups[1].ToString();
                //实例化相机
                if (devicelist.Items[devicelist.SelectedIndices[0]].Index==0)
                {
                    //防止重复
                    if (!usedlist.Contains(0))
                    {
                        button1.Enabled = true;
                        groupBox3.Enabled = true;
                        mCamera1 = new BaslerCam(serial);
                        mCamera1.OpenCam();
                        //mCamera1.SetHeartBeatTime(5000);                      // 设置相机1心跳时间，网口相机
                        mCamera1.eventComputeGrabTime += computeGrabTime1;    // 注册计算采集图像时间回调函数
                        mCamera1.eventProcessImage += processHImage1;         // 注册halcon显示回调函数
                        //mCamera1.numWindowIndex = 0;                          // 相机1 PYLON自带窗体显示窗体序号

                        tkbExposure1.Minimum = (int)mCamera1.minExposureTime;
                        tkbExposure1.Maximum = (int)mCamera1.maxExposureTime;
                        tkbGain1.Minimum = (int)mCamera1.minGain;
                        tkbGain1.Maximum = (int)mCamera1.maxGain;
                        // 设置相机1按钮使能
                        SetBottonEnable1(true, false, true);
                        //
                        usedlist.Add(0);
                        //
                        lblGrabTime1.Text = "[  连接状态 : " + "在线" + "  ]";
                    }

                }
                else
                {
                    if (!usedlist.Contains(1))
                    {
                        groupBox4.Enabled = true;
                        mCamera2 = new BaslerCam(serial);
                        mCamera2.OpenCam();
                        //mCamera2.SetHeartBeatTime(5000);                     // 设置相机2心跳时间，网口相机
                        mCamera2.eventComputeGrabTime += computeGrabTime2;   // 注册计算采集图像时间回调函数
                        mCamera2.eventProcessImage += processHImage2;        // 注册halcon显示回调函数
                        //mCamera2.numWindowIndex = 1;                         // 相机2 PYLON自带窗体显示窗体序号

                        tkbExposure2.Minimum = (int)mCamera2.minExposureTime;
                        tkbExposure2.Maximum = (int)mCamera2.maxExposureTime;
                        tkbGain2.Minimum = (int)mCamera2.minGain;
                        tkbGain2.Maximum = (int)mCamera2.maxGain;
                        // 设置相机2按钮使能
                        SetBottonEnable2(true, false, true);
                        //
                        usedlist.Add(1);
                        //
                        lblGrabTime2.Text = "[  连接状态 : " + "在线" + "  ]";
                    }

                }
                
            }
        }
    }
}
