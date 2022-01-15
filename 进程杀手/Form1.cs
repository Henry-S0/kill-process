using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace 进程杀手
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }




        private void IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            //WindowsBuiltInRole可以枚举出很多权限，例如系统用户、User、Guest等等
            if (windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                this.Invoke(new EventHandler(delegate
                {
                    label4.Text = "管理员模式";
                }));
            }
            else
            {
                this.Invoke(new EventHandler(delegate
                {
                    label4.Text = "非管理员模式";
                }));
            }
        }

        bool is_start = false;

        //开始按钮按下时
        private void button1_Click(object sender, EventArgs e)
        {
            if (is_start == false)
            {
                //判断是否选择
                if (comboBox1.Text == "" || comboBox1.Text == "请选择")
                {
                    return;
                }

                string filePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

                if (comboBox1.Text == "进程杀手" || comboBox1.Text == System.IO.Path.GetFileNameWithoutExtension(filePath))
                {
                    MessageBox.Show("不可以，这样子不可以。");
                    return;
                }

                //
                is_start = true;
                button1.Text = "停止";

                label5.Text = "已阻止0次";
                //创建线程，开始killer
                Thread thread = new Thread(new ParameterizedThreadStart(KillProcess));
                thread.Start(comboBox1.Text);

                //选项按钮属性变更
                label3.Visible = true;
                comboBox1.Enabled = false;
                label5.Visible = true;
            }
            else
            {
                //
                is_start = false;
                button1.Text = "开始";

                //选项按钮属性变更
                label3.Visible = false;
                comboBox1.Enabled = true;
                label5.Visible = false;
            }

        }

        //kill主函数
        public void KillProcess(object kill_p)
        {
            int i = 0;

            while (button1.Text == "停止")
            {
                Process[] pro = Process.GetProcessesByName((string)kill_p);
                foreach (Process item in pro)
                {
                    try
                    {
                        item.Kill();
                        i++;
                        this.Invoke(new EventHandler(delegate
                        {
                            label5.Text = "已阻止" + i.ToString() + "次";
                        }));
                    }
                    catch
                    {

                    }
                }
            }
        }

        //Dictionary<int, string> dic = new Dictionary<int, string>();

        public int[] id;
        public string[] name;

        List<string> list=new List<string>();


        private void get_list()
        {
            Process[] pro = Process.GetProcesses();


            //获取数据长度
            int pid_len = pro.Length;

            //创建对应长度的数组
            id = new int[pid_len];
            name = new string[pid_len];


            for (int i = 0; i < pid_len; i++)
            {
                id[i] = pro[i].Id;
                name[i] = pro[i].ProcessName;
            }


            //dic.Clear();
            //foreach (var p in Process.GetProcesses())
            //{
            //    dic.Add(p.Id, p.ProcessName);
            //}
            //dic = dic.OrderBy(o => o.Value).ToDictionary(p => p.Key, o => o.Value);


            this.Invoke(new EventHandler(delegate
            {
                comboBox1.DataSource = name;
            }));


            //foreach (var i in dic)
            //{
            //    this.Invoke(new EventHandler(delegate
            //    {
            //        comboBox1.Items.Add(i.Value);
            //    }));
            //}
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(IsAdministrator);
            thread.Start();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = false;
            //string str = this.GetType().Assembly.Location;
            //Process.Start(@str);
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {

            Thread thread = new Thread(get_list);
            thread.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int i = 0;
            //foreach (int key in dic.Keys)
            //{
            //    if (i == comboBox1.SelectedIndex)
            //    {
            //        label2.Text = key.ToString();
            //    }
            //    i++;
            //}

            label2.Text = id[comboBox1.SelectedIndex].ToString();

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }


    }
}
