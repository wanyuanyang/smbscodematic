using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using MySql.Data.MySqlClient;
namespace Codematic
{
    public partial class LoginMySQL : Form
    {
        LTP.CmConfig.DbSettings dbobj = new LTP.CmConfig.DbSettings();
        public string constr;       
        public string dbname = "mysql";

        public LoginMySQL()
        {
            InitializeComponent();
        }

        private void btn_ConTest_Click(object sender, EventArgs e)
        {
            try
            {
                string server = this.comboBoxServer.Text.Trim();
                string user = this.txtUser.Text.Trim();
                string pass = this.txtPass.Text.Trim();
                string port = this.textBox1.Text.Trim();
                if ((user == "") || (server == ""))
                {
                    MessageBox.Show(this, "���������û�������Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                constr = String.Format("server={0};uid={1}; Port={2};pwd={3}; database=mysql; pooling=false", server, user, port, pass);
                try
                {
                    this.Text = "�������ӷ����������Ժ�...";

                    LTP.IDBO.IDbObject dbobj;                    
                    dbobj = LTP.DBFactory.DBOMaker.CreateDbObj("MySQL");

                    dbobj.DbConnectStr = constr;
                    List<string> dblist = dbobj.GetDBList();
                    this.cmbDBlist.Enabled = true;
                    this.cmbDBlist.Items.Clear();
                    this.cmbDBlist.Items.Add("ȫ����");
                    if (dblist != null)
                    {
                        if (dblist.Count > 0)
                        {
                            foreach (string dbname in dblist)
                            {
                                this.cmbDBlist.Items.Add(dbname);
                            }
                        }
                    }
                    this.cmbDBlist.SelectedIndex = 0;
                    this.Text = "���ӷ������ɹ���";

                }
                catch(System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    this.Text = "���ӷ��������ȡ������Ϣʧ�ܣ�";
                    DialogResult drs = MessageBox.Show(this, "���ӷ��������ȡ������Ϣʧ�ܣ�\r\n�����������ַ���û��������Ƿ���ȷ���鿴�����ļ��԰�����������⣿", "��ʾ", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    if (drs == DialogResult.OK)
                    {
                        try
                        {
                            Process proc = new Process();
                            Process.Start("IExplore.exe", "http://help.maticsoft.com");
                        }
                        catch
                        {
                            MessageBox.Show("����ʣ�http://www.maticsoft.com", "���", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    return;
                }

            }
            catch (System.Exception ex)
            {
                LogInfo.WriteLog(ex);
                MessageBox.Show(this, ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);                
            }
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            try
            {
                string server = this.comboBoxServer.Text.Trim();
                string user = this.txtUser.Text.Trim();
                string pass = this.txtPass.Text.Trim();
                string port = this.textBox1.Text.Trim();
                if ((user == "") || (server == ""))
                {
                    MessageBox.Show(this, "���������û�������Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (this.cmbDBlist.SelectedIndex > 0)
                {
                    dbname = cmbDBlist.Text;
                }
                else
                {
                    dbname = "mysql";
                }                
                constr = String.Format("server={0};user id={1}; Port={2};password={3}; database={4}; pooling=false", server, user, port,pass, dbname);               
                //��������
                MySqlConnection myCn = new MySqlConnection(constr);
                try
                {
                    this.Text = "�������ӷ����������Ժ�...";
                    myCn.Open();
                }
                catch(System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    this.Text = "���ӷ�����ʧ�ܣ�";
                    MessageBox.Show(this, "���ӷ�����ʧ�ܣ������������ַ���û��������Ƿ���ȷ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    myCn.Close();
                }
                this.Text = "���ӷ������ɹ���";
                if (dbobj == null)
                {
                    dbobj = new LTP.CmConfig.DbSettings();
                }                              
                string strtype = "MySQL";
                //����ǰ����д�������ļ�
                dbobj.DbType = strtype;
                dbobj.Server = server;
                dbobj.ConnectStr = constr;
                dbobj.DbName = dbname;
                dbobj.ConnectSimple = chk_Simple.Checked; 
                bool succ = LTP.CmConfig.DbConfig.AddSettings(dbobj);
                if (!succ)
                {
                    MessageBox.Show(this, "�÷������Ѿ����ڣ��������������ַ���������Ƿ���ȷ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(this, ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogInfo.WriteLog(ex);
            }		
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoginMySQL_Load(object sender, EventArgs e)
        {
            //comboBoxServerVer.SelectedIndex = 0;
            //comboBox_Verified.SelectedIndex = 0;
        }
    }
}