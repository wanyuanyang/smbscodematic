using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Codematic
{
	/// <summary>
	/// ProjectExpadd ��ժҪ˵����
	/// </summary>
	public class ProjectExpadd : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox textBox1;
		private WiB.Pinkie.Controls.ButtonXP btn_OK;
		private WiB.Pinkie.Controls.ButtonXP btn_Cancel;
		/// <summary>
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProjectExpadd()
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
				if(components != null)
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btn_OK = new WiB.Pinkie.Controls.ButtonXP();
			this.btn_Cancel = new WiB.Pinkie.Controls.ButtonXP();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "����ļ����ͣ�";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(104, 16);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(168, 21);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "";
			// 
			// btn_OK
			// 
			this.btn_OK._Image = null;
			this.btn_OK.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(212)), ((System.Byte)(208)), ((System.Byte)(200)));
			this.btn_OK.DefaultScheme = false;
			this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btn_OK.Image = null;
			this.btn_OK.Location = new System.Drawing.Point(64, 56);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Scheme = WiB.Pinkie.Controls.ButtonXP.Schemes.Blue;
			this.btn_OK.Size = new System.Drawing.Size(65, 25);
			this.btn_OK.TabIndex = 46;
			this.btn_OK.Text = "ȷ ��";
			
			// 
			// btn_Cancel
			// 
			this.btn_Cancel._Image = null;
			this.btn_Cancel.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(212)), ((System.Byte)(208)), ((System.Byte)(200)));
			this.btn_Cancel.DefaultScheme = false;
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Image = null;
			this.btn_Cancel.Location = new System.Drawing.Point(160, 56);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Scheme = WiB.Pinkie.Controls.ButtonXP.Schemes.Blue;
			this.btn_Cancel.Size = new System.Drawing.Size(65, 25);
			this.btn_Cancel.TabIndex = 45;
			this.btn_Cancel.Text = "ȡ ��";

			// 
			// ProjectExpadd
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(292, 93);
			this.Controls.Add(this.btn_OK);
			this.Controls.Add(this.btn_Cancel);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProjectExpadd";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "����ļ�����";
			this.ResumeLayout(false);

		}
		#endregion

		
	}
}
