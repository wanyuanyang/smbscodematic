using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
namespace uninstall
{
	/// <summary>
	/// Form1 ��ժҪ˵����
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Windows ���������֧���������
			//
			InitializeComponent();

			//
			// TODO: �� InitializeComponent ���ú�����κι��캯������
			//
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
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(192, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "��ȷ��Ҫж�ظ������?";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(292, 157);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.ShowInTaskbar = false;
			this.Text = "ж��";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Ӧ�ó��������ڵ㡣
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			try
			{
				Process myProcess =new Process();
				string s=System.Environment.OSVersion.ToString();
				if(System.Environment.OSVersion.ToString().IndexOf("NT 5")>0)
				{
					myProcess.StartInfo.FileName="msiexec.exe";
                    myProcess.StartInfo.Arguments = "/X{B5A043B4-BCE9-4607-9928-73492209AD1D}";
					myProcess.Start();
				}
				myProcess.Close();
			}
			catch
			{}
			Application.Exit();		
		}
	}
}
