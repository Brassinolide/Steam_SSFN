using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace SteamSSFN
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string GetBetweenStr(string str, string str1, string str2){
            int i1 = str.IndexOf(str1);
            if (i1 < 0) {
                return "";
            }
            int i2 = str.IndexOf(str2, i1 + str1.Length);
            if (i2 < 0) {
                return "";
            }
            return str.Substring(i1 + str1.Length, i2 - i1 - str1.Length);
        }

        public static string HttpDownloadFile(string url, string path){
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream responseStream = response.GetResponseStream();
            Stream stream = new FileStream(path, FileMode.Create);
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0){
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
            return path;
        }

        public static string GetSteamPath()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam");
            return (string)key.GetValue("SteamPath"); ;
        }

        //写入授权
        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists("temp")) File.Delete("temp");
            //获取ssfn下载地址
            HttpDownloadFile("http://124.222.242.85/home/ssfn/" + textBox1.Text, "temp");//抓包找到的大D的数据库
            //HttpDownloadFile("https://ssfnbox.com/download/" + textBox1.Text, "temp");
            string ssfnURL = GetBetweenStr(File.ReadAllLines("temp")[12], "window.location.assign(\"", "\"");
            if (string.IsNullOrEmpty(ssfnURL))
            {
                MessageBox.Show("未找到SSFN", "错误");
            }
            else
            {
                //判断steam目录是否正确
                if (File.Exists(textBox2.Text + "/steam.exe"))
                {
                    //在steam根目录下搜索并删除ssfn
                    string[] searchfile = Directory.GetFiles(textBox2.Text, "ssfn*").Select(path => Path.GetFileName(path)).ToArray();

                    foreach (string ssfn in searchfile)
                    {
                        if (File.Exists(textBox2.Text + "\\" + ssfn))
                        {
                            File.Delete(textBox2.Text + "\\" + ssfn);
                        }
                    }

                    //下载ssfn
                    HttpDownloadFile("https://ssfnbox.com" + ssfnURL, "temp");
                    //移动至steam根目录
                    File.Move("temp", textBox2.Text + "/" + textBox1.Text);
                    //启动steam
                    if (checkBox1.Checked == true)
                    {
                        System.Diagnostics.Process.Start(textBox2.Text, "-noreactlogin");
                    }
                    MessageBox.Show("SSFN写入成功");
                }
                else
                {
                    MessageBox.Show("您确定您选择的Steam路径准确无误？", "错误");
                }
            }
        }

        //启动时自动获取steam路径
        private void Form1_Load(object sender, EventArgs e)
        {
            string steamPath = GetSteamPath();
            if (string.IsNullOrEmpty(steamPath))
            {
                MessageBox.Show("Steam路径获取失败，请手动获取", "错误");
            }
            else
            {
                textBox2.Text = steamPath;
            }
        }

        //手动选择steam路径
        private void button3_Click(object sender, EventArgs e)
        {
            string FolderPath = string.Empty;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选择Steam目录";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FolderPath = dialog.SelectedPath;
            }
            textBox2.Text = FolderPath;
        }

        //运行steam
        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox2.Text + "/steam.exe"))
            {
                System.Diagnostics.Process.Start(textBox2.Text + "/steam.exe", "-noreactlogin");
            }
            else
            {
                MessageBox.Show("您确定您选择的Steam路径准确无误？", "错误");
            }
        }

        //一键上号
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (File.Exists("temp")) File.Delete("temp");
            //分割字符串
            string key = textBox3.Text;
            string[] keyarr = key.Split(new string[] { "----" }, StringSplitOptions.None);
            if (keyarr.Length >= 3)
            {
                string login = keyarr[0].ToString();
                string password = keyarr[1].ToString();
                string loginssfn = keyarr[2].ToString();
                //获取ssfn下载地址
                HttpDownloadFile("https://ssfnbox.com/download/" + loginssfn, "temp");
                string ssfnURL = GetBetweenStr(File.ReadAllLines("temp")[12], "window.location.assign(\"", "\"");
                if (string.IsNullOrEmpty(ssfnURL))
                {
                    MessageBox.Show("未找到SSFN", "错误");
                }
                else
                {
                    //判断steam目录是否正确
                    if (File.Exists(textBox2.Text + "/steam.exe"))
                    {
                        //在steam根目录下搜索并删除ssfn
                        string[] searchfile = Directory.GetFiles(textBox2.Text, "ssfn*").Select(path => Path.GetFileName(path)).ToArray();

                        foreach (string ssfn in searchfile)
                        {
                            if (File.Exists(textBox2.Text + "\\" + ssfn))
                            {
                                File.Delete(textBox2.Text + "\\" + ssfn);
                            }
                        }

                        //下载ssfn
                        HttpDownloadFile("https://ssfnbox.com" + ssfnURL, "temp");
                        //移动至steam根目录
                        File.Move("temp", textBox2.Text + "/" + loginssfn);
                        //启动steam
                        if (checkBox1.Checked == true)
                        {
                            System.Diagnostics.Process.Start(textBox2.Text + "/steam.exe", "-login " + login + " " + password + " -noreactlogin -rememberpassword -windowed -bigpicture ");
                        }
                        MessageBox.Show("一键上号成功");
                    }
                    else MessageBox.Show("您确定您选择的Steam路径准确无误？", "错误");
                }
            }
        }
    }
}
