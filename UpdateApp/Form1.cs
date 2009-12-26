using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Threading;
using UpdateApp.UpdateService;
namespace UpdateApp
{
	/// <summary>
	/// Form1 ��ժҪ˵����
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label StatusLabel1;
		private System.Windows.Forms.ProgressBar progressBar1;
        delegate void SetStatusCallback(string text);
        delegate void SetProBar1MaxCallback(int val);
        delegate void SetProBar1ValCallback(int val);
		Thread mythread;
		Mutex mutex;
        private PictureBox pictureBox1;
        string logfilename = Application.StartupPath + "\\logInfo.txt";

		#region 
		public Form1()
		{			
			InitializeComponent();
			mutex = new Mutex(false, "UpdateApp_MUTEX");
			if (!mutex.WaitOne(0, false)) 
			{
				mutex.Close();
				mutex = null;
			}	
		}

		/// <summary>
		/// ������������ʹ�õ���Դ��
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows ������������ɵĴ���
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.StatusLabel1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // StatusLabel1
            // 
            this.StatusLabel1.AutoSize = true;
            this.StatusLabel1.Location = new System.Drawing.Point(21, 73);
            this.StatusLabel1.Name = "StatusLabel1";
            this.StatusLabel1.Size = new System.Drawing.Size(155, 12);
            this.StatusLabel1.TabIndex = 0;
            this.StatusLabel1.Text = "���ӷ���������������...";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(0, 110);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(292, 19);
            this.progressBar1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(19, 15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(292, 131);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.StatusLabel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ϵͳ����";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Ӧ�ó��������ڵ㡣
		/// </summary>
		[STAThread]
		static void Main() 
		{			
			Application.EnableVisualStyles();
			Application.DoEvents(); 
						
			Form1 app = new Form1();
			if (app.mutex != null) 
			{
				Application.Run(app);				
			}
			else
			{
				DialogResult dia=MessageBox.Show(app,"�����Ѿ������У���ֹ��ǰӦ����","ϵͳ��ʾ",MessageBoxButtons.YesNo,MessageBoxIcon.Information);
				if(dia==DialogResult.Yes)
				{
					Application.Exit();
				}
			}
		}

        /// <summary>
        /// ѭ����ַ�������ֵ
        /// </summary>
        /// <param name="val"></param>
        public void SetprogressBar1Max(int val)
        {
            if (this.progressBar1.InvokeRequired)
            {
                SetProBar1MaxCallback d = new SetProBar1MaxCallback(SetprogressBar1Max);
                this.Invoke(d, new object[] { val });
            }
            else
            {
                this.progressBar1.Maximum = val;

            }
        }
        /// <summary>
        /// ѭ����ַ����
        /// </summary>
        /// <param name="val"></param>
        public void SetprogressBar1Val(int val)
        {
            if (this.progressBar1.InvokeRequired)
            {
                SetProBar1ValCallback d = new SetProBar1ValCallback(SetprogressBar1Val);
                this.Invoke(d, new object[] { val });
            }
            else
            {
                this.progressBar1.Value = val;

            }
        }

		#endregion

        /// <summary>
        /// д��־
        /// </summary>
        /// <param name="loginfo"></param>
        public void WriteLog(string loginfo)
        {            
            StreamWriter sw = new StreamWriter(logfilename, true);
            sw.WriteLine(DateTime.Now.ToString() + ":" + loginfo);
            sw.Close();
        }

		private void Form1_Load(object sender, System.EventArgs e)
		{
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
			{
				this.Close();
			}
			try
			{
                mythread = new Thread(new ThreadStart(ProcUpdate));
                mythread.Start();
                //ProcUpdate();
			}
			catch (System.Exception er)
			{
				MessageBox.Show(er.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		#region �������������ļ�

		void ProcUpdate()
		{
			this.StatusLabel1.Text = "���ӷ���������������...";
            bool iserr = false;
            //����������
            int linecount = 0;
            int linesucc = 0;//�ɹ��� 
			try
			{                
				UpServer upser = new UpServer();				
				if (upser.GetFileList() != "")
				{
					string currpath = Application.StartupPath;
					string serverurl = upser.GetServerUrl();
					string folder = upser.GetNewFilePath();
					
                    //����[���ļ��б�]�ļ�
					SetMessage("�����ļ��б�...");
					string fileUrl = serverurl + "/" + upser.GetFileList();
					string filelistname = currpath + "\\Updatelist.ini";
					DownFile(fileUrl, filelistname);
                                        
                    StreamReader srline = File.OpenText(filelistname);
                    while (srline.ReadLine()!=null)
                    {
                        linecount++;
                    }
                    srline.Close();
                    SetprogressBar1Max(linecount);

					//��ʼ���б��ļ� ��ʼ ���س����ļ�					
					StreamReader sr = File.OpenText(filelistname);
                    int currline = 1;
					String strfile;
					while ((strfile = sr.ReadLine()) != null)
					{
                        try
                        {
                            SetMessage("�������� " + strfile);
                            string fileR = serverurl + "/" + folder + "/" + strfile;
                            string fileL = currpath + "\\" + strfile;
                            DownFile(fileR, fileL);
                            linesucc++;
                        }
                        catch (System.Exception ex)
                        {
                            iserr = true;
                            WriteLog(strfile+":"+ex.Message);
                        }
                        SetprogressBar1Val(currline);
                        currline++;
					}                    
                    SetMessage("������ɡ�");
					sr.Close();					
				}				
			}
			catch (System.Exception ex)
			{
				string s = ex.Message;
                iserr = true;
                WriteLog(s);
			}
			finally
			{}
            //����ʧ��
            if (iserr)
            {
                try
                {
                    string errinfo = "�������ʧ�ܣ�����ֱ���ڹٷ���վ�������°汾��װ��";
                    int fileerr = linecount - linesucc;
                    if (fileerr > 0)
                    {
                        errinfo = "�������ʧ��"+fileerr.ToString()+"���ļ�δ���³ɹ�������ֱ���ڹٷ���վ�������°汾��װ��";
                    }
                    DialogResult dia = MessageBox.Show(this, errinfo, "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dia == DialogResult.Yes)
                    {
                        Process proc = new Process();
                        Process.Start("IExplore.exe", "http://www.maticsoft.com/softdown.aspx");
                    }

                }
                catch
                {
                    MessageBox.Show("����ʣ�http://www.maticsoft.com", "���", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }            
			StartApp();
		}

        //�ļ�����
        public void DownFile(string fileurl, string localFilename)
        {
            WebClient myclient = new WebClient();
            myclient.DownloadFile(fileurl, localFilename);
            myclient.Dispose();
        }
        //�ļ���Ϣ
        private void SetMessage(string msg)
        {
            if (this.StatusLabel1.InvokeRequired)
            {
                SetStatusCallback d = new SetStatusCallback(SetMessage);
                this.Invoke(d, new object[] { msg });
            }
            else
            {
                this.StatusLabel1.Text = msg;

            }
        }
		#endregion

        #region ������ϣ�����Ӧ�ó���
        private void StartApp()
		{
			this.Hide();
			DialogResult dia=MessageBox.Show(this,"������³ɹ�����رյ�ǰ�������´�Ӧ�ó���\n�������ڴ���","ϵͳ��ʾ",MessageBoxButtons.YesNo,MessageBoxIcon.Information);
			if(dia==DialogResult.Yes)
			{
				string strapp = Application.StartupPath + @"\Codematic.exe";
				if (File.Exists(strapp))
				{
					Process.Start(strapp);
				}
			}
			Application.Exit();
        }
        #endregion

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            //if ((mythread.ThreadState == System.Threading.ThreadState.Running) || (mythread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            //{
            //    e.Cancel = true;
            //}
		}
	}
}
