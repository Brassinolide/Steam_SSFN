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

        private void button1_Click(object sender, EventArgs e){
            if (File.Exists("temp")) {
                File.Delete("temp");
            }
            HttpDownloadFile("https://ssfnbox.com/download/" + textBox1.Text, "temp");
            string ssfnURL = GetBetweenStr(File.ReadAllLines("temp")[12], "window.location.assign(\"", "\"");
            if (string.IsNullOrEmpty(ssfnURL)) {
                MessageBox.Show("未找到SSFN", "错误");
            }
            else {
                if (File.Exists(textBox2.Text + "/steam.exe"))
                {
                    string[] searchfile = Directory.GetFiles(textBox2.Text, "ssfn*").Select(path => Path.GetFileName(path)).ToArray();

                    foreach (string ssfn in searchfile)
                    {
                        if (File.Exists(textBox2.Text + "\\" + ssfn))
                        {
                            File.Delete(textBox2.Text + "\\" + ssfn);
                        }
                    }
                    HttpDownloadFile("https://ssfnbox.com" + ssfnURL, "temp");
                    File.Move("temp", textBox2.Text + "/" + textBox1.Text);
                    if (checkBox1.Checked == true)
                    {
                        System.Diagnostics.Process.Start(textBox2.Text, "-noreactlogin");
                    }
                    MessageBox.Show("SSFN写入成功");
                }
                else {
                    MessageBox.Show("您确定您选择的Steam路径准确无误？","错误");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string steamPath = GetSteamPath();
            if (string.IsNullOrEmpty(steamPath))
            {
                MessageBox.Show("Steam路径获取失败，请手动获取","错误");
            }
            else {
                textBox2.Text = steamPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string FolderPath=string.Empty;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选择Steam目录";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FolderPath = dialog.SelectedPath;
            }
            textBox2.Text = FolderPath;
        }

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
    }
}
