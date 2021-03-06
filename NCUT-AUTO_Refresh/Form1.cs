﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCUT_AUTO_Refresh
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;//取消线程间的安全检查
        }

        bool IsPic = false;


        private string GetHttpWebRequest(string url)
        {
            HttpWebResponse result;
            string strHTML = string.Empty;
            try
            {
                Uri uri = new Uri(url);
                WebRequest webReq = WebRequest.Create(uri);
                WebResponse webRes = webReq.GetResponse();

                HttpWebRequest myReq = (HttpWebRequest)webReq;
                myReq.UserAgent = "User-Agent:Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705";
                myReq.Accept = "*/*";
                myReq.KeepAlive = true;
                myReq.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                result = (HttpWebResponse)myReq.GetResponse();
                Stream receviceStream = result.GetResponseStream();
                StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("gb2312"));
                strHTML = readerOfStream.ReadToEnd();
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
                readerOfStream.Close();
                receviceStream.Close();
                result.Close();
            }
            catch
            {
                Uri uri = new Uri(url);
                WebRequest webReq = WebRequest.Create(uri);
                HttpWebRequest myReq = (HttpWebRequest)webReq;
                myReq.UserAgent = "User-Agent:Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705";
                myReq.Accept = "*/*";
                myReq.KeepAlive = true;
                myReq.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                //result = (HttpWebResponse)myReq.GetResponse(); 
                try
                {
                    result = (HttpWebResponse)myReq.GetResponse();
                }
                catch (WebException ex)
                {
                    result = (HttpWebResponse)ex.Response;
                }
                try
                {
                    Stream receviceStream = result.GetResponseStream();
                    StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("gb2312"));
                    strHTML = readerOfStream.ReadToEnd();
                    readerOfStream.Close();
                    receviceStream.Close();
                    result.Close();

                }
                catch (Exception ex)
                {
                    //MessageBox.Show("没有连接到NCUT！", "未连接");
                    Console.WriteLine(ex.Message);
                    pictureBox1.Hide();
                    pictureBox2.Show();
                    pictureBox3.Hide();
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    button4.Enabled = true;
                    button5.Enabled = true;
                    button6.Enabled = true;

                    return "您没有连接到网络，或者没有连接到NCUT！                                      ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;";
                    //throw;
                }


            }
            return strHTML;
        }
        static string GET(string input)
        {
            string result = "";
            int temp = input.Length;
            for (int i = 1; i < input.Length; i++)
            {
                if (char.IsNumber(input, temp - i))
                {
                    for (int j = input.Length - i; j >= 0; j--)
                    {
                        if (char.IsNumber(input, j))
                        {
                            result += input[j];
                        }
                        else
                        {
                            return result;
                        }
                    }
                }

            }
            return result;
        }
        static string fanzhuan(string result)
        {
            string a = "";
            for (int i = result.Length - 1; i >= 0; i--)
            {
                a = a + result[i];
            }
            return a;
        }
        private void Fun()
        {
            string str;
            this.textBox1.Text = "正在连接到NCUT，如果长时间没有反应，请检查网络连接！";
            str = GetHttpWebRequest("http://ip.ncut.edu.cn/");
            this.textBox1.Text = "已经连接到NCUT(ip.ncut.edu.cn)！";
            string[] a = str.Split(';');
            int res;
            try
            {
                string b = a[3];
                string result2;
                result2 = GET(b);
                result2 = fanzhuan(result2);
                int.TryParse(result2, out res);

                string Usetime = a[2];
                int time;
                result2 = GET(Usetime);
                Usetime = fanzhuan(result2);
                int.TryParse(Usetime, out time);
                TimeSpan ts = new TimeSpan(0, time, 0);
                label8.Text = "有效使用时长*:  " + ts.Days + " 天 " + ts.Hours + " 小时 " + ts.Minutes + "分钟";

                string name = a[27];
                Regex reg = new Regex("[\u4e00-\u9fa5]+");
                foreach (Match RealName in reg.Matches(name))
                    name = RealName.ToString();
                //Console.WriteLine(RealName);

                string id = a[12];
                string id1;
                string id2;
                id1 = GET(id);
                id2 = fanzhuan(id1);
                label7.Text = "ID:  " + name + "  (" + id2 + ")";

                int flow, flow1, flow0;
                flow = res;
                flow0 = flow % 1024;
                flow1 = flow - flow0;
                flow0 = flow0 * 1000;
                flow0 = flow0 - flow0 % 1024;
                string flow3 = ".";
                string detail;
                detail = "已使用流量 : " + flow1 / 1024 + flow3 + flow0 / 1024 + " M";
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                label3.Text = detail;

                string TextFlag = a[200];//未登录时数组会越界

                //if (res == 0 && name == "基本参数")
                //{
                //    pictureBox1.Hide();
                //    pictureBox2.Hide();
                //    pictureBox3.Show();

                //    timer2.Stop();
                //    timer4.Stop();
                //    timer3.Stop();

                //    label7.Text = "-------------------";
                //    label8.Text = "-------------------";
                //    label3.Text = "-------------------";

                //    notifyIcon1.BalloonTipTitle = "注意";
                //    notifyIcon1.BalloonTipText = "您可能并未登陆NCUT-AUTO，请您打开浏览器重新登陆。";
                //    notifyIcon1.ShowBalloonTip(3000);

                //}

            }
            catch (Exception ex)
            {
                pictureBox1.Hide();
                pictureBox2.Hide();
                pictureBox3.Show();

                timer2.Stop();
                timer4.Stop();
                timer3.Stop();

                label7.Text = "-------------------";
                label8.Text = "-------------------";
                this.label3.Text = "-------------------";

                notifyIcon1.BalloonTipTitle = "注意";
                notifyIcon1.BalloonTipText = "您可能并未登陆NCUT-AUTO，请您打开浏览器重新登陆。";
                notifyIcon1.ShowBalloonTip(3000);

                Console.WriteLine(ex.Message);

            }
        }
        public void ConnectTest()
        {
            textBox1.Text = "正在连接到NCUT，如果长时间没有反应，请检查网络连接！\r\n\r\n";
            pictureBox1.Show();
            pictureBox2.Hide();
            pictureBox3.Hide();
            string str;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            str = GetHttpWebRequest("http://ip.ncut.edu.cn/");
            //Console.WriteLine(str);
            //MessageBox.Show(str);
            textBox1.Text = "确保绿色指示灯闪烁一次，其它颜色指示灯关闭。\r\n\r\n\r\n" + str;
            //           Regex r = new Regex("flow='");
            string[] a = str.Split(';');
            //string[] name  = str.Split(';');
            //           Match m = r.Match(str);
            //           if (m.Success)
            //           {
            //               Console.WriteLine("Found match at position " + m.Index); //输入匹配字符的位置
            //          }
            int res;
            //int idnum;
            try
            {
                string b = a[3];
                string result2;
                string result1;
                result2 = GET(b);
                result1 = fanzhuan(result2);
                int.TryParse(result1, out res);

                string Usetime = a[2];
                int time;
                result2 = GET(Usetime);
                Usetime = fanzhuan(result2);
                int.TryParse(Usetime, out time);
                TimeSpan ts = new TimeSpan(0, time, 0);
                //label8.Text = "有效使用时长*: " + ts.TotalMinutes + "小时 " ;
                label8.Text = "有效使用时长*:  " + ts.Days + " 天 " + ts.Hours + " 小时 " + ts.Minutes + "分钟";

                string name = a[27];
                Regex reg = new Regex("[\u4e00-\u9fa5]+");
                foreach (Match RealName in reg.Matches(name))
                    name = RealName.ToString();
                //Console.WriteLine(RealName);

                string id = a[12];
                string id1;
                string id2;
                id1 = GET(id);
                id2 = fanzhuan(id1);
                label7.Text = "ID:  " + name + "  (" + id2 + ")";


                //res = int.Parse(b);
                //MessageBox.Show(b);
                int flow, flow1, flow0;
                flow = res;
                flow0 = flow % 1024;
                flow1 = flow - flow0;
                flow0 = flow0 * 1000;
                flow0 = flow0 - flow0 % 1024;
                string flow3 = ".";
                string detail;
                detail = "已使用流量 : " + flow1 / 1024 + flow3 + flow0 / 1024 + " M";
                //MessageBox.Show(detail);
                //textBox2.Text = detail;
                label3.Text = detail;
                toolStripStatusLabel2.Text = "确保绿色指示灯闪烁一次，其它颜色指示灯关闭。";

                string TextFlag = a[200];

                if (res == 0 && name == "基本参数")
                {
                    toolStripStatusLabel2.Text = "连接测试：连接失败请检查网络是否连接，或者是否登陆NCUT！";
                    pictureBox1.Hide();
                    pictureBox2.Hide();
                    pictureBox3.Show();

                    label8.Text = "ID";
                    label7.Text = "有效使用时长*:";
                    label3.Text = "已使用流量 :";

                    try
                    {
                        Ping usable = new Ping();
                        Console.WriteLine(usable.Send("192.168.254.251").Status);
                        DialogResult dr = MessageBox.Show("Ping：" + usable.Send("192.168.254.251").Status + "！\r\n您可能并未登陆NCUT-AUTO，是否打开浏览器为您登陆？", "登陆提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start("http://ip.ncut.edu.cn/");
                        }
                    }
                    catch
                    {
                        DialogResult dr = MessageBox.Show("Ping：Fail" + "！\r\n您可能并未连接到NCUT-AUTO，请您打开网络和Internet设置。", "连接提示");
                    }
                   
                }

                pictureBox1.Hide();
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = "连接测试：连接失败请检查网络是否连接，或者是否登陆NCUT！";
                Console.WriteLine(ex.Message);
                label8.Text = "ID";
                label7.Text = "有效使用时长*:";
                label3.Text = "已使用流量 :";
                pictureBox1.Hide();
                pictureBox2.Hide();
                pictureBox3.Show();
                try
                {
                    Ping usable = new Ping();
                    Console.WriteLine(usable.Send("192.168.254.251").Status);
                    DialogResult dr = MessageBox.Show("Ping：" + usable.Send("192.168.254.251").Status + "！\r\n您可能并未登陆NCUT-AUTO，是否打开浏览器为您登陆？", "登陆提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("http://ip.ncut.edu.cn/");
                    }
                }
                catch
                {
                    DialogResult dr = MessageBox.Show("Ping：Fail" + "！\r\n您可能并未连接到NCUT-AUTO，请您打开网络和Internet设置。", "连接提示");
                }
            }
        }
        static private string GetIpAddress()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
                                                                    //IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            IPAddress localaddr = localhost.AddressList[0];

            return localaddr.ToString();
        }
        public static string GetMacByNetworkInterface()
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface ni in interfaces)
                {
                    return BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes());
                }
            }
            catch (Exception)
            {
            }
            return "00-00-00-00-00-00";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            toolStripStatusLabel1.Text = DateTime.Now.ToString();
            toolStripStatusLabel2.Text = "完成初始化。";
            timer1.Start();
            MessageBox.Show("如果您在连接到非NCUT的外部网络中使用“连接测试”按钮，程序会假死40s左右，不建议您这样尝试！","我想对你说句真心话");
            //button5_Click(sender,e);
        }



        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.BalloonTipTitle = "注意";
            notifyIcon1.BalloonTipText = "双击托盘图标打开程序。";
            notifyIcon1.ShowBalloonTip(3000);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            this.Show();
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = DateTime.Now.ToString();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            toolStripStatusLabel2.Text = "正在进行间隔2s的刷新。";

            pictureBox2.Hide();
            pictureBox3.Hide();
            if (IsPic)
            {
                pictureBox1.Hide();
                IsPic = !IsPic;
            }
            else
            {
                pictureBox1.Show();
                IsPic = !IsPic;
            }
            Thread childThread = new Thread(Fun);
            childThread.Start();
            textBox1.Text += "\r\n";
        }

        private void timer3_Tick(object sender, EventArgs e)
        {

            toolStripStatusLabel2.Text = "正在进行间隔3s的刷新。";
            
            pictureBox2.Hide();
            pictureBox3.Hide();
            if (IsPic)
            {
                pictureBox1.Hide();
                IsPic = !IsPic;
            }
            else
            {
                pictureBox1.Show();
                IsPic = !IsPic;
            }
            Thread childThread = new Thread(Fun);
            childThread.Start();
            textBox1.Text += "\r\n";
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text = "正在进行极速刷新。";

            pictureBox2.Hide();
            pictureBox3.Hide();
            if (IsPic)
            {
                pictureBox1.Hide();
                IsPic = !IsPic;
            }
            else
            {
                pictureBox1.Show();
                IsPic = !IsPic;
            }
            Thread childThread = new Thread(Fun);
            childThread.Start();
            textBox1.Text += "\r\n";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            button5_Click( sender, e);
            pictureBox1.Hide();
            pictureBox2.Hide();
            pictureBox3.Hide();
            toolStripStatusLabel2.Text = "正在进行间隔2s的刷新。";
            timer2.Start();
            timer4.Stop();
            timer3.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button5_Click(sender, e);
            pictureBox1.Hide();
            pictureBox2.Hide();
            pictureBox3.Hide();
            toolStripStatusLabel2.Text = "正在进行间隔3s的刷新。";
            timer3.Start();
            timer4.Stop();
            timer2.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button5_Click(sender, e);
            pictureBox1.Hide();
            pictureBox2.Hide();
            pictureBox3.Hide();
            toolStripStatusLabel2.Text = "正在进行极速刷新。";
            timer4.Start();
            timer2.Stop();
            timer3.Stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            timer4.Stop();
            timer3.Stop();
            pictureBox1.Show();
            pictureBox2.Show();
            pictureBox3.Show();
            label7.Text = "";
            toolStripStatusLabel2.Text = "您已经停止刷新。";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Thread childThread = new Thread(ConnectTest);
            childThread.Start();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pictureBox1.Show();
            pictureBox2.Show();
            pictureBox3.Show();
            System.Environment.Exit(0);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

            MessageBox.Show("本机的IP地址为："+GetIpAddress(), "本机的IP地址");
        }

        private void label5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("本机的MAC地址为："+GetMacByNetworkInterface(), "本机的MAC地址");
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void timer5_Tick(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://ip.ncut.edu.cn/");
        }
    }
}
