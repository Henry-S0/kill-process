using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
                        //当异常时，说明进程不存在或kill失败
                    }
                }
            }
        }

        //Dictionary<int, string> dic = new Dictionary<int, string>();

        public int[] id;
        public string[] name;


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
            ProcessInit();

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

        private void button2_Click(object sender, EventArgs e)
        {
            if(label4.Text=="非管理员模式")
            {
                MessageBox.Show("注意：方案2必须在管理员下运行，否则无效");
                return;

            }


            ////Process.Start(Resource1.Backstab64.ToString());



            //string str_Normal = Convert.ToBase64String(Resource1.Backstab64);
            //byte[] ns = Convert.FromBase64String(str_Normal); // 转为byte
            //Assembly asm_n = Assembly.Load(ns);
            //MethodInfo info_n = asm_n.EntryPoint;
            //ParameterInfo[] parameters_n = info_n.GetParameters();
            //info_n.Invoke(null, null); // 执行

            //testFunc();

            FileUtil.ExtractResFile("进程杀手.Resources.Backstab.exe", ".test.exe");
            //string exe_path = "cmd.exe";  // 被调exe
            //string the_args = "cmd";   // 被调exe接受的参数

           
            StartProcess(label2.Text);

            string filePath = ".test.exe";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            //Thread thread = new Thread(new ParameterizedThreadStart(StartProcess));
            //thread.Start("111");

        }



        Process proc = new Process();
        /// <summary>
        /// ///////////////////////////////////////
        /// </summary>
        private void ProcessInit()
        {
            //启动Windows的cmd控制台
            proc.StartInfo.FileName = "cmd.exe";
            //启动进程时不使用 shell
            proc.StartInfo.UseShellExecute = false;
            //设置标准重定向输入
            proc.StartInfo.RedirectStandardInput = true;
            //设置标准重定向输出
            proc.StartInfo.RedirectStandardOutput = true;
            //设置标准重定向错误输出
            proc.StartInfo.RedirectStandardError = true;
            //设置不显示cmd控制台窗体
            proc.StartInfo.CreateNoWindow = true;
            //隐藏窗体
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            
            proc.Start();
            proc.BeginOutputReadLine();
            //设置回调函数，异步读取指令回复
            proc.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputHandler);
        }

        //设置回调，读取指令的返回值
        private void ProcessOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            try
            {
                //进程间通信，解决线程中调用控件错误
                Control.CheckForIllegalCrossThreadCalls = false;
                if (!String.IsNullOrEmpty(outLine.Data))
                {
                    ////将读取的指令标准输出显示在richtext控件上
                    //richTextBox1.AppendText(outLine.Data + Environment.NewLine);
                    //richTextBox1.Focus();
                    //richTextBox1.Select(this.richTextBox1.TextLength, 0);
                    //richTextBox1.ScrollToCaret();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        //指令发送函数，tclCommand为需要执行的cmd指令
        private void ExecuteTclCommand(string tclCommand)
        {
            proc.StandardInput.WriteLine(tclCommand);
            proc.StandardInput.AutoFlush = true;
        }

        /// <summary>
        /// ////////////////////////////////////////////////////
        /// </summary>
        /// <param name="obj"></param>

        public void StartProcess(string pid)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false; //必要参数
            process.StartInfo.RedirectStandardOutput = true;//输出参数设定
            process.StartInfo.RedirectStandardInput = true;//传入参数设定
            process.StartInfo.RedirectStandardError = true;//设置标准重定向错误输出

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Verb = "runas";

            process.StartInfo.FileName = ".test.exe";
            process.StartInfo.Arguments = "-p "+pid+" -k";


            process.Start(); //程序启动


            process.WaitForExit();//等待程序执行完退出进程    
            process.Close(); //关闭程序
            //process.Dispose();
            //Process myProcess = new Process();

            ////myProcess.StartInfo.UseShellExecute = false;
            //myProcess.StartInfo.FileName = runFilePath;
            ////myProcess.StartInfo.CreateNoWindow = true;

            ////myProcess.StartInfo.UseShellExecute = true;

            //myProcess.StartInfo.Arguments = "ping 192.168.0.1";
            ////

            //myProcess.Start();



        }


        [STAThread]//需要标记现场模型为STA，否则程序可能报错
        static void testFunc()
        {
            string resourceName = "进程杀手" + ".Resources.Backstab64.exe";//命名空间+内嵌资源名称

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);//内嵌资源转换成stream

            byte[] buffer = new byte[stream.Length];//资源数据缓存数组

            stream.Read(buffer, 0, (int)stream.Length);//读取数据到缓存数组

            Assembly asm = Assembly.Load(buffer); //加载数据

            MethodInfo pointInfo = asm.EntryPoint;//获取程序入口点

            pointInfo.Invoke(null, null);//运行

            Console.ReadLine();
        }


        class FileUtil
        {

            /// <summary>
            /// 从资源文件中抽取资源文件
            /// </summary>
            /// <param name="resFileName">资源文件名称（资源文件名称必须包含目录，目录间用“.”隔开,最外层是项目默认命名空间）</param>
            /// <param name="outputFile">输出文件</param>
            public static void ExtractResFile(string resFileName, string outputFile)
            {
                BufferedStream inStream = null;
                FileStream outStream = null;
                try
                {
                    Assembly asm = Assembly.GetExecutingAssembly(); //读取嵌入式资源
                    inStream = new BufferedStream(asm.GetManifestResourceStream(resFileName));
                    outStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);

                    byte[] buffer = new byte[1024];
                    int length;

                    while ((length = inStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outStream.Write(buffer, 0, length);
                    }
                    outStream.Flush();
                }
                finally
                {
                    if (outStream != null)
                    {
                        outStream.Close();
                    }
                    if (inStream != null)
                    {
                        inStream.Close();
                    }
                }
            }



        }

        private void button3_Click(object sender, EventArgs e)
        {


        }

        private void button4_Click(object sender, EventArgs e)
        {
            string path4 = System.AppDomain.CurrentDomain.BaseDirectory;
            //proc.StartInfo.Verb = "runas";
            //ExecuteTclCommand(textBox1.Text);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
