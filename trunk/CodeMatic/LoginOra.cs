using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data.OracleClient;
using System.Xml;

namespace Codematic
{
    /// <summary>
    /// FormLogin ��ժҪ˵����
    /// </summary>
    public class LoginOra : System.Windows.Forms.Form
    {
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private WiB.Pinkie.Controls.ButtonXP BtnOk;
        private WiB.Pinkie.Controls.ButtonXP BtnCancle;
        public System.Windows.Forms.TextBox txtUser;
        public System.Windows.Forms.TextBox txtPass;
        public System.Windows.Forms.TextBox txtServer;

        LTP.CmConfig.DbSettings dbobj = new LTP.CmConfig.DbSettings();
        public string constr = "";
        public string dbname = "";
        public CheckBox chk_Simple;

        #region system
        /// <summary>
        /// ����������������
        /// </summary>
        private System.ComponentModel.Container components = null;

        public LoginOra()
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows ������������ɵĴ���
        /// <summary>
        /// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
        /// �˷��������ݡ�
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginOra));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.BtnOk = new WiB.Pinkie.Controls.ButtonXP();
            this.BtnCancle = new WiB.Pinkie.Controls.ButtonXP();
            this.chk_Simple = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(166, 276);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.label1.Location = new System.Drawing.Point(173, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 1);
            this.label1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(168, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = " ��¼�����ݿ�";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(176, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 23);
            this.label3.TabIndex = 3;
            this.label3.Text = "�û�����";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(176, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 23);
            this.label4.TabIndex = 4;
            this.label4.Text = "����(P&)��";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(176, 148);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 23);
            this.label5.TabIndex = 5;
            this.label5.Text = "����";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(184, 248);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(280, 23);
            this.label6.TabIndex = 6;
            this.label6.Text = "��Ȩ����(C) 2009 SMBSOFT ����������Ȩ����";
            this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtUser
            // 
            this.txtUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUser.Location = new System.Drawing.Point(288, 80);
            this.txtUser.MaxLength = 30;
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(168, 21);
            this.txtUser.TabIndex = 7;
            // 
            // txtPass
            // 
            this.txtPass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPass.Location = new System.Drawing.Point(288, 112);
            this.txtPass.MaxLength = 25;
            this.txtPass.Name = "txtPass";
            this.txtPass.PasswordChar = '*';
            this.txtPass.Size = new System.Drawing.Size(168, 21);
            this.txtPass.TabIndex = 7;
            // 
            // txtServer
            // 
            this.txtServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServer.Location = new System.Drawing.Point(288, 144);
            this.txtServer.MaxLength = 30;
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(168, 21);
            this.txtServer.TabIndex = 7;
            // 
            // BtnOk
            // 
            this.BtnOk._Image = null;
            this.BtnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.BtnOk.DefaultScheme = true;
            this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnOk.Image = null;
            this.BtnOk.Location = new System.Drawing.Point(196, 208);
            this.BtnOk.Name = "BtnOk";
            this.BtnOk.Scheme = WiB.Pinkie.Controls.ButtonXP.Schemes.Blue;
            this.BtnOk.Size = new System.Drawing.Size(70, 24);
            this.BtnOk.TabIndex = 8;
            this.BtnOk.Text = "ȷ ��";
            this.BtnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // BtnCancle
            // 
            this.BtnCancle._Image = null;
            this.BtnCancle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.BtnCancle.DefaultScheme = true;
            this.BtnCancle.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancle.Image = null;
            this.BtnCancle.Location = new System.Drawing.Point(292, 208);
            this.BtnCancle.Name = "BtnCancle";
            this.BtnCancle.Scheme = WiB.Pinkie.Controls.ButtonXP.Schemes.Blue;
            this.BtnCancle.Size = new System.Drawing.Size(70, 24);
            this.BtnCancle.TabIndex = 9;
            this.BtnCancle.Text = "ȡ ��";
            this.BtnCancle.Click += new System.EventHandler(this.BtnCancle_Click);
            // 
            // chk_Simple
            // 
            this.chk_Simple.AutoSize = true;
            this.chk_Simple.Checked = true;
            this.chk_Simple.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Simple.Location = new System.Drawing.Point(288, 173);
            this.chk_Simple.Name = "chk_Simple";
            this.chk_Simple.Size = new System.Drawing.Size(96, 16);
            this.chk_Simple.TabIndex = 37;
            this.chk_Simple.Text = "��Ч����ģʽ";
            this.chk_Simple.UseVisualStyleBackColor = true;
            // 
            // LoginOra
            // 
            this.AcceptButton = this.BtnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.ClientSize = new System.Drawing.Size(480, 273);
            this.Controls.Add(this.chk_Simple);
            this.Controls.Add(this.BtnCancle);
            this.Controls.Add(this.BtnOk);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.txtServer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginOra";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "��¼";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion




        #endregion


        private void FormLogin_Load(object sender, System.EventArgs e)
        {
            //try
            //{
            //    dbobj=LTP.CmConfig.DbConfig.GetSetting("Oracle");
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

        private void BtnCancle_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
        {
            try
            {
                string user = this.txtUser.Text.Trim();
                string pass = this.txtPass.Text.Trim();
                string server = this.txtServer.Text.Trim();

                if ((user.Trim() == "") || (server.Trim() == ""))
                {
                    MessageBox.Show(this, "�û��������벻��Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                constr = "Data Source=" + server + "; user id=" + user + ";password=" + pass;

                //��������
                OracleConnection myCn = new OracleConnection(constr);
                try
                {
                    this.Text = "�������ӷ����������Ժ�...";
                    myCn.Open();
                }
                catch
                {
                    this.Text = "���ӷ�����ʧ�ܣ�";
                    myCn.Close();
                    MessageBox.Show(this, "���ӷ�����ʧ�ܣ������������ַ���û��������Ƿ���ȷ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                myCn.Close();
                this.Text = "���ӷ������ɹ���";


                if (dbobj == null)
                    dbobj = new LTP.CmConfig.DbSettings();

                //����ǰ����д�������ļ�
                dbobj.DbType = "Oracle";
                dbobj.Server = server;
                dbobj.ConnectStr = constr;
                dbobj.DbName = "";
                dbobj.ConnectSimple = chk_Simple.Checked; 
                bool succ = LTP.CmConfig.DbConfig.AddSettings(dbobj);
                if (!succ)
                {
                    MessageBox.Show(this, "�÷������Ѿ����ڣ��������������ַ���������Ƿ���ȷ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                //����ǰ���ݿ�ϵͳ����д������
                //MainForm.setting.DbType="Oracle";
                //LTP.CmConfig.ModuleConfig.SaveSettings(MainForm.setting);
                this.DialogResult = DialogResult.OK;
                this.Close();

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(this, ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogInfo.WriteLog(ex);
            }
        }


    }
}
