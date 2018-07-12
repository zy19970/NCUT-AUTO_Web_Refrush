using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCUT_AUTO_Refresh
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
                StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("utf-8"));
                strHTML = readerOfStream.ReadToEnd();
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
                    MessageBox.Show("没有连接到NCUT！", "未连接");
                    Console.WriteLine(ex.Message);

                    pictureBox1.Hide();
                    pictureBox2.Show();
                    pictureBox3.Hide();

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
            str = GetHttpWebRequest("http://192.168.254.251/");
            this.textBox1.Text = "已经连接到NCUT(192.168.254.251)！";
            string[] a = str.Split(';');
            int res;
            try
            {
                string b = a[54];
                string result2;
                result2 = GET(b);
                result2 = fanzhuan(result2);
                int.TryParse(result2, out res);
                int flow, flow1, flow0;
                flow = res;
                flow0 = flow % 1024;
                flow1 = flow - flow0;
                flow0 = flow0 * 1000;
                flow0 = flow0 - flow0 % 1024;
                string flow3 = ".";
                string detail;
                detail = "已使用流量 : " + flow1 / 1024 + flow3 + flow0 / 1024 + " M";
                this.textBox2.Text = detail;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                pictureBox1.Hide();
                pictureBox2.Hide();
                pictureBox3.Show();
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = DateTime.Now.ToString();
            toolStripStatusLabel2.Text = "完成初始化。";
            timer1.Start();
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
            Fun();
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
            textBox1.Text += "\r\n";
        }

        private void timer3_Tick(object sender, EventArgs e)
        {

            toolStripStatusLabel2.Text = "正在进行间隔3s的刷新。";
            Fun();
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
            textBox1.Text += "\r\n";
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text = "正在进行间隔5s的刷新。";
            Fun();
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
            textBox1.Text += "\r\n";
        }


        private void button1_Click(object sender, EventArgs e)
        {
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
            pictureBox1.Hide();
            pictureBox2.Hide();
            pictureBox3.Hide();
            toolStripStatusLabel2.Text = "正在进行间隔5s的刷新。";
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
            toolStripStatusLabel2.Text = "您已经停止刷新。";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox1.Show();
            pictureBox2.Hide();
            pictureBox3.Hide();
            string str;
            textBox1.Text = "正在连接到NCUT，如果长时间没有反应，请检查网络连接！";
            str = GetHttpWebRequest("http://192.168.254.251/");
            //Console.WriteLine(str);
            //MessageBox.Show(str);
            textBox1.Text = str;
            //           Regex r = new Regex("flow='");
            string[] a = str.Split(';');
            //           Match m = r.Match(str);
            //           if (m.Success)
            //           {
            //               Console.WriteLine("Found match at position " + m.Index); //输入匹配字符的位置
            //          }
            int res;
            try
            {
                string b = a[54];
                string result2;
                string result1;
                result2 = GET(b);
                result1 = fanzhuan(result2);
                int.TryParse(result1, out res);
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
                textBox2.Text = detail;
                toolStripStatusLabel2.Text = "连接测试：已经连接到NCUT。";

                pictureBox1.Hide();
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = "连接测试：连接失败请检查网络是否连接，或者是否登陆NCUT！";
                Console.WriteLine(ex.Message);
                pictureBox1.Hide();
                pictureBox2.Show();
                pictureBox3.Hide();
                DialogResult dr = MessageBox.Show("您可能并未登陆NCUT-AUTO，是否打开浏览器为您登陆？", "登陆提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("http://192.168.254.251/");
                }
            }

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
    }
}
