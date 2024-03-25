using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Profinet.Panasonic;
using Microsoft.Win32;

namespace TestSignal
{
    public partial class Form1 : Form
    {
        PanasonicMewtocol panasonicMewtocol1 = new PanasonicMewtocol();
        public Form1()
        {
            InitializeComponent();
            LoadCom();
            GetPassCode();
        }

        void LoadCom()
        {
            // 获取本机所有COM LIST
            using (RegistryKey local = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM", false))
            {
                string[] k = local.GetValueNames();
                if (k.Length > 0)
                {
                    string[] ports = new string[local.ValueCount];
                    for (int i = 0; i < k.Length; i++)
                    {
                        ports[i] = local.GetValue(k[i]) as string;
                        this.comboBox1.Items.Add(ports[i]);
                        this.comboBox1.SelectedIndex = 0;
                        this.comboBox2.SelectedIndex = 1;
                    }
                }
            }
        }

        void GetPassCode()
        {
            if (!HslCommunication.Authorization.SetAuthorizationCode("f562cc4c-4772-4b32-bdcd-f3e122c534e3"))
            {
                MessageBox.Show("授权失败，当前程序只能使用8小时");
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!panasonicMewtocol1.IsOpen())
                return;

            OperateResult<string> operateResult1;
            OperateResult<bool> operateResult2;

            if (this.comboBox2.Text == "String")
            {
                operateResult1 = panasonicMewtocol1.ReadString(textBox1.Text, ushort.Parse(textBox2.Text));
                this.textBox3.Text = operateResult1.Content.ToString();
            }
            else
            {
                operateResult2 = panasonicMewtocol1.ReadBool(textBox1.Text);
                this.textBox3.Text = operateResult2.Content.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 配置COM参数
            panasonicMewtocol1.SerialPortInni("COM1", 9600);
            panasonicMewtocol1.Open();
            if (!panasonicMewtocol1.IsOpen())
            {
                MessageBox.Show("连接PLC失败！");
            }
            this.button2.Enabled = false;
            this.button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (panasonicMewtocol1.IsOpen())
            {
                panasonicMewtocol1.Close();
                this.button2.Enabled = true;
                this.button3.Enabled = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            panasonicMewtocol1.Dispose();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox2.SelectedIndex == 1)
            {
                this.textBox2.Enabled = false;
            }
            else if(this.comboBox2.SelectedIndex == 0)
            {
                this.textBox2.Enabled = true;
            } 
            else
            {
                this.textBox2.Enabled = false;
            }
        }
    }
}
