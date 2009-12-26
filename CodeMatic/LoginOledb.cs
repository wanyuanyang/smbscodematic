using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
namespace Codematic
{
	/// <summary>
	/// LoginForm ��ժҪ˵����
	/// </summary>
	public class LoginOledb : System.Windows.Forms.Form
	{
		public System.Windows.Forms.GroupBox groupBox1;
		public System.Windows.Forms.Label label2;
		public System.Windows.Forms.Label label3;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ToolTip toolTip1;
		public System.Windows.Forms.TextBox txtServer;
		public System.Windows.Forms.TextBox txtUser;
		public System.Windows.Forms.TextBox txtPass;
		private System.ComponentModel.IContainer components;
		private WiB.Pinkie.Controls.ButtonXP btn_Ok;
		private WiB.Pinkie.Controls.ButtonXP btn_Cancel;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private WiB.Pinkie.Controls.ButtonXP btn_SelDb;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radBtn_DB;
		private System.Windows.Forms.RadioButton radBtn_Constr;
		public System.Windows.Forms.TextBox txtConstr;

		LTP.CmConfig.DbSettings dbobj=new LTP.CmConfig.DbSettings();
		

		public LoginOledb()
		{			
			InitializeComponent();			
		}

		#region Windows Form Designer generated code
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

		
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		public void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LoginOledb));
			this.txtServer = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.txtUser = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtPass = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.btn_Ok = new WiB.Pinkie.Controls.ButtonXP();
			this.btn_Cancel = new WiB.Pinkie.Controls.ButtonXP();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radBtn_DB = new System.Windows.Forms.RadioButton();
			this.btn_SelDb = new WiB.Pinkie.Controls.ButtonXP();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtConstr = new System.Windows.Forms.TextBox();
			this.radBtn_Constr = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtServer
			// 
			this.txtServer.Location = new System.Drawing.Point(104, 24);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(216, 21);
			this.txtServer.TabIndex = 1;
			this.txtServer.Text = "";
			this.txtServer.TextChanged += new System.EventHandler(this.txtServer_TextChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkBox1);
			this.groupBox1.Controls.Add(this.txtUser);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.txtPass);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.pictureBox1);
			this.groupBox1.Location = new System.Drawing.Point(24, 80);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(368, 106);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "��¼���ݿ⣺";
			// 
			// checkBox1
			// 
			this.checkBox1.Checked = true;
			this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox1.Location = new System.Drawing.Point(72, 77);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(88, 24);
			this.checkBox1.TabIndex = 4;
			this.checkBox1.Text = "�հ�����";
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// txtUser
			// 
			this.txtUser.Location = new System.Drawing.Point(136, 24);
			this.txtUser.Name = "txtUser";
			this.txtUser.Size = new System.Drawing.Size(120, 21);
			this.txtUser.TabIndex = 3;
			this.txtUser.Text = "";
			this.txtUser.TextChanged += new System.EventHandler(this.txtUser_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(56, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "��¼��(&L)��";
			// 
			// txtPass
			// 
			this.txtPass.Enabled = false;
			this.txtPass.Location = new System.Drawing.Point(136, 48);
			this.txtPass.Name = "txtPass";
			this.txtPass.PasswordChar = '*';
			this.txtPass.Size = new System.Drawing.Size(120, 21);
			this.txtPass.TabIndex = 3;
			this.txtPass.Text = "";
			this.txtPass.TextChanged += new System.EventHandler(this.txtPass_TextChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(72, 50);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 16);
			this.label3.TabIndex = 2;
			this.label3.Text = "����(&P)��";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(16, 24);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(38, 35);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			// 
			// btn_Ok
			// 
			this.btn_Ok._Image = null;
			this.btn_Ok.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(236)), ((System.Byte)(233)), ((System.Byte)(216)));
			this.btn_Ok.DefaultScheme = false;
			this.btn_Ok.DialogResult = System.Windows.Forms.DialogResult.None;
			this.btn_Ok.Image = null;
			this.btn_Ok.Location = new System.Drawing.Point(104, 240);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Scheme = WiB.Pinkie.Controls.ButtonXP.Schemes.Blue;
			this.btn_Ok.Size = new System.Drawing.Size(75, 24);
			this.btn_Ok.TabIndex = 19;
			this.btn_Ok.Text = "ȷ��(&O):";
			this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
			// 
			// btn_Cancel
			// 
			this.btn_Cancel._Image = null;
			this.btn_Cancel.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(236)), ((System.Byte)(233)), ((System.Byte)(216)));
			this.btn_Cancel.DefaultScheme = false;
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Image = null;
			this.btn_Cancel.Location = new System.Drawing.Point(224, 240);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Scheme = WiB.Pinkie.Controls.ButtonXP.Schemes.Blue;
			this.btn_Cancel.Size = new System.Drawing.Size(75, 24);
			this.btn_Cancel.TabIndex = 20;
			this.btn_Cancel.Text = "ȡ��(&C):";
			this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.txtServer);
			this.groupBox2.Controls.Add(this.radBtn_DB);
			this.groupBox2.Controls.Add(this.btn_SelDb);
			this.groupBox2.Location = new System.Drawing.Point(24, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(368, 60);
			this.groupBox2.TabIndex = 21;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "ѡ�����ݿ�";
			// 
			// radBtn_DB
			// 
			this.radBtn_DB.Checked = true;
			this.radBtn_DB.Location = new System.Drawing.Point(16, 22);
			this.radBtn_DB.Name = "radBtn_DB";
			this.radBtn_DB.TabIndex = 20;
			this.radBtn_DB.TabStop = true;
			this.radBtn_DB.Text = "���ݿ�(&D)��";
			this.radBtn_DB.Click += new System.EventHandler(this.radBtn_DB_Click);
			// 
			// btn_SelDb
			// 
			this.btn_SelDb._Image = null;
			this.btn_SelDb.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(236)), ((System.Byte)(233)), ((System.Byte)(216)));
			this.btn_SelDb.DefaultScheme = false;
			this.btn_SelDb.DialogResult = System.Windows.Forms.DialogResult.None;
			this.btn_SelDb.Image = null;
			this.btn_SelDb.Location = new System.Drawing.Point(320, 22);
			this.btn_SelDb.Name = "btn_SelDb";
			this.btn_SelDb.Scheme = WiB.Pinkie.Controls.ButtonXP.Schemes.Blue;
			this.btn_SelDb.Size = new System.Drawing.Size(40, 24);
			this.btn_SelDb.TabIndex = 19;
			this.btn_SelDb.Text = "...";
			this.btn_SelDb.Click += new System.EventHandler(this.btn_SelDb_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.txtConstr);
			this.groupBox3.Controls.Add(this.radBtn_Constr);
			this.groupBox3.Location = new System.Drawing.Point(24, 192);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(368, 40);
			this.groupBox3.TabIndex = 22;
			this.groupBox3.TabStop = false;
			// 
			// txtConstr
			// 
			this.txtConstr.Enabled = false;
			this.txtConstr.Location = new System.Drawing.Point(104, 12);
			this.txtConstr.Name = "txtConstr";
			this.txtConstr.Size = new System.Drawing.Size(256, 21);
			this.txtConstr.TabIndex = 0;
			this.txtConstr.Text = "";
			// 
			// radBtn_Constr
			// 
			this.radBtn_Constr.Location = new System.Drawing.Point(12, 10);
			this.radBtn_Constr.Name = "radBtn_Constr";
			this.radBtn_Constr.TabIndex = 1;
			this.radBtn_Constr.Text = "�����ַ�����";
			this.radBtn_Constr.Click += new System.EventHandler(this.radBtn_Constr_Click);
			// 
			// LoginOledb
			// 
			this.AcceptButton = this.btn_Ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.btn_Cancel;
			this.ClientSize = new System.Drawing.Size(416, 272);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.btn_Cancel);
			this.Controls.Add(this.btn_Ok);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginOledb";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "��¼���ݿ�....";
			this.Load += new System.EventHandler(this.LoginForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
//		protected override void OnClosing(CancelEventArgs e)
//		{				
//			if(this.DialogResult==DialogResult.Cancel)
//			{					
//				this.Close();
//			}	
//			else
//			{
//				e.Cancel = true;
//			}
//			// otherwise, let the framework close the app
//		}


		#endregion

		private void LoginForm_Load(object sender, System.EventArgs e)
		{			
			this.toolTip1.SetToolTip(this.txtUser,"�뱣֤���û�����ÿ�����ݿ�ķ���Ȩ��");
            //try
            //{
            //    dbobj=LTP.CmConfig.DbConfig.GetSetting("OleDb");
            //    if(dbobj!=null)
            //    {
            //        txtServer.Text=dbobj.Server;
            //        txtUser.Text=dbobj.Uid;
            //        txtPass.Text=dbobj.Password;					
            //    }

            //}
            //catch
            //{
            //    MessageBox.Show("��ȡ�����ļ�ʧ��!");
            //}
		}


		#region ��¼

		private void btn_Ok_Click(object sender, System.EventArgs e)
		{
			try
			{							
				string server=this.txtServer.Text.Trim();
				string user=this.txtUser.Text.Trim();
				string pass=this.txtPass.Text.Trim();
								
				if(this.radBtn_DB.Checked)
				{
                    GetConstr();
					if(server=="")
					{
						MessageBox.Show(this,"���ݿⲻ��Ϊ�գ�","����",MessageBoxButtons.OK,MessageBoxIcon.Information);
						return;
					}
				}

				if(this.radBtn_Constr.Checked)
				{
                    if (txtConstr.Text == "")
					{
						MessageBox.Show(this,"���ݿⲻ��Ϊ�գ�","����",MessageBoxButtons.OK,MessageBoxIcon.Information);
						return;
					}
				}
                string constr = this.txtConstr.Text;

                //��������
				OleDbConnection myCn = new OleDbConnection(constr);
				try
				{
                    this.Text = "�����������ݿ⣬���Ժ�...";
					myCn.Open();
				}
				catch
				{
                    this.Text = "�������ݿ�ʧ�ܣ�";
                    MessageBox.Show(this, "�������ݿ�ʧ�ܣ��������ݿ��ַ���û��������Ƿ���ȷ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);					
					return;					
				}				
				myCn.Close();
                this.Text = "�������ݿ�ɹ���";

				if(dbobj==null)
					dbobj=new LTP.CmConfig.DbSettings();

				//����ǰ����д�������ļ�
				dbobj.DbType="OleDb";
				dbobj.Server=server;
                dbobj.ConnectStr = constr;
                dbobj.DbName = "";              
                LTP.CmConfig.DbConfig.AddSettings(dbobj);
								              
				this.DialogResult=DialogResult.OK;
				this.Close();	
				
			}
			catch(System.Exception ex)
			{				
				MessageBox.Show(this,ex.Message,"����",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                LogInfo.WriteLog(ex);
			}
			
		}
		#endregion
		
		private void btn_Cancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
        }


        #region ѡ���ļ�
        private void btn_SelDb_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog sqlfiledlg=new OpenFileDialog();
			sqlfiledlg.Title="ѡ�����ݿ��ļ�";
            sqlfiledlg.Filter = "Access files (*.mdb;*.accdb)|*.mdb;*.accdb|�����ļ� (*.*)|*.*";
			DialogResult result=sqlfiledlg.ShowDialog(this);			
			if(result==DialogResult.OK)
			{
				this.txtServer.Text=sqlfiledlg.FileName;
                GetConstr();
                //this.txtConstr.Text=@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+txtServer.Text+";Persist Security Info=False";	
			}
        }

        #endregion

        #region ���������ַ���
        private void GetConstr()
		{
            FileInfo file = new FileInfo(txtServer.Text);
            string ext = file.Extension;
            switch (ext.ToLower().Trim())
            {
                case ".mdb":
                    txtConstr.Text = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + txtServer.Text + ";Persist Security Info=False";
                    break;
                case ".accdb":
                    txtConstr.Text = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + txtServer.Text + ";Persist Security Info=False";
                    break;
                default:
                    txtConstr.Text = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + txtServer.Text + ";Persist Security Info=False";		
                    break;
            }				
		}

		private void txtServer_TextChanged(object sender, System.EventArgs e)
		{
			GetConstr();			
		}
		private void txtUser_TextChanged(object sender, System.EventArgs e)
		{
			GetConstr();
		}
		private void txtPass_TextChanged(object sender, System.EventArgs e)
		{
			GetConstr();
        }
        #endregion

        #region  �ؼ�ѡ��
        private void radBtn_DB_Click(object sender, System.EventArgs e)
		{
			this.radBtn_Constr.Checked=false;
			this.checkBox1.Enabled=true;
			this.txtServer.Enabled=true;
			this.txtPass.Enabled=true;
			this.txtUser.Enabled=true;
			this.txtConstr.Enabled=false;		
		}

		private void radBtn_Constr_Click(object sender, System.EventArgs e)
		{
			this.radBtn_DB.Checked=false;
			this.checkBox1.Enabled=false;
			this.txtServer.Enabled=false;
			this.txtPass.Enabled=false;
			this.txtUser.Enabled=false;
			this.txtConstr.Enabled=true;
        }
        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                this.txtPass.Text = "";
                this.txtPass.Enabled = false;
            }
            else
            {
                this.txtPass.Enabled = true;
            }
        }

        #endregion


    }
}
