using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Data;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Maticsoft.AddInManager;
namespace Codematic.UserControls
{
    /// <summary>
    /// ���������
    /// </summary>
    public partial class UcAddInManage : UserControl
    {
        private UcCodeView codeview;
        AddIn addin = new AddIn();
        public UcAddInManage()
        {
            InitializeComponent();
            codeview = new UcCodeView();
            tabPage_Code.Controls.Add(codeview);
            CreatView();
            BindlistView();
            LoadAddinFile();
        }

        #region  ΪlistView� �� ����

        private void CreatView()
        {
            //�����б�
            this.listView1.Columns.Clear();
            this.listView1.Items.Clear();
            //this.listView1.LargeImageList = imglistView;
            //this.listView1.SmallImageList = imglistView;
            this.listView1.View = View.Details;
            this.listView1.GridLines = true;
            //this.listView1.CheckBoxes = true;
            this.listView1.FullRowSelect = true;

            listView1.Columns.Add("���", 60, HorizontalAlignment.Left);
            listView1.Columns.Add("����", 150, HorizontalAlignment.Left);            
            listView1.Columns.Add("�汾", 50, HorizontalAlignment.Left);
            listView1.Columns.Add("˵��", 150, HorizontalAlignment.Left);
            listView1.Columns.Add("����", 100, HorizontalAlignment.Left);
        }

        private void BindlistView()
        {
            DataSet ds = addin.GetAddInList();
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null)
                    {
                        listView1.Items.Clear();
                        foreach (DataRow row in dt.Rows)
                        {
                            string Name = row["Name"].ToString();
                            string Decription = row["Decription"].ToString();
                            string Assembly = row["Assembly"].ToString();
                            string Classname = row["Classname"].ToString();
                            string Version = row["Version"].ToString();
                            string Guid = row["Guid"].ToString();

                            ListViewItem item1 = new ListViewItem(Guid, 0);
                            //item1.ImageIndex = 4;
                            item1.SubItems.Add(Name);                            
                            item1.SubItems.Add(Version);
                            item1.SubItems.Add(Decription);
                            item1.SubItems.Add(Assembly);
                            //item1.SubItems.Add(Guid);

                            listView1.Items.AddRange(new ListViewItem[] { item1 });

                        }
                    }
                }
            }



        }
        #endregion

        #region �����ļ�����
        private void LoadAddinFile()
        {
            string xmlfile = addin.LoadFile();
            codeview.SettxtContent("XML", xmlfile);
        }
        #endregion

        #region  ɾ��
        private void menu_ShowMain_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.listView1.SelectedItems.Count < 1)
                {
                    MessageBox.Show(this, "����ѡ�������", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                int count = 0;
                foreach (ListViewItem item in this.listView1.SelectedItems)
                {
                    string Guid = item.SubItems[0].Text;
                    addin.DeleteAddIn(Guid);
                    listView1.Items.Remove(listView1.SelectedItems[0]);
                    count++;
                }
                BindlistView();
                LoadAddinFile();
                MessageBox.Show(this, count + "����ɾ����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch(System.Exception ex)
            {
                MessageBox.Show(this, "ɾ��ʧ�ܣ������ԣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogInfo.WriteLog(ex);
            }
        }

        #endregion

        #region ��ť
        private void btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                if ((txtClassname.Text.Trim() == "") || (txtAssembly.Text.Trim() == "") || (txtName.Text.Trim() == ""))
                {
                    MessageBox.Show(this, "�����Ϣ����������������д��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string filename = txtFile.Text;
                if (File.Exists(filename))
                {
                    int n = filename.LastIndexOf("\\");
                    if (n > 1)
                    {
                        string name = filename.Substring(n + 1);
                        if (File.Exists(Application.StartupPath + "\\" + name))
                        {
                            DialogResult dr = MessageBox.Show("������ļ��Ѿ����ڣ��Ƿ񸲸�ԭ�����", "ͬ����ʾ", MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {
                                File.Copy(filename, Application.StartupPath + "\\" + name, true);
                            }
                        }
                        else
                        {
                            File.Copy(filename, Application.StartupPath + "\\" + name, true);
                        }
                    }

                    addin.Guid = Guid.NewGuid().ToString().ToUpper();
                    addin.Classname = txtClassname.Text;
                    addin.Assembly = txtAssembly.Text;
                    addin.Decription = txtDecr.Text;
                    addin.Name = txtName.Text;
                    addin.Version = txtVersion.Text;
                    addin.AddAddIn();
                    BindlistView();
                    LoadAddinFile();
                    MessageBox.Show(this, "�����ӳɹ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tabControlAddIn.SelectedIndex = 1;
                }
                else
                {
                    MessageBox.Show(this, "��ѡ����ļ������ڣ�" , "����", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(this, "������ʧ�ܣ�\r\n�������ֹ����Ƹ��ļ�����װĿ¼�����޸������ļ����ɡ�\r\n" + ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogInfo.WriteLog(ex);
            }

        }
        /// <summary>
        /// �Ƿ��Ǳ�׼�ӿڵĳ���
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool IsValidPlugin(Type t)
        {
            bool ret = false;
            Type[] interfaces = t.GetInterfaces();
            foreach (Type theInterface in interfaces)
            {
                if ((theInterface.FullName == "LTP.IBuilder.IBuilderDAL") ||
                    (theInterface.FullName == "LTP.IBuilder.IBuilderDALTran")||
                    (theInterface.FullName == "LTP.IBuilder.IBuilderWeb") ||
                    (theInterface.FullName == "LTP.IBuilder.IBuilderBLL"))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }        
        private void btnBrow_Click(object sender, EventArgs e)
        {
            try
            {

                OpenFileDialog sqlfiledlg = new OpenFileDialog();
                sqlfiledlg.Title = "ѡ������ļ�";
                sqlfiledlg.Filter = "DLL Files (*.dll)|*.dll|All files (*.*)|*.*";
                DialogResult result = sqlfiledlg.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    string filename = sqlfiledlg.FileName;
                    txtFile.Text = filename;

                    Assembly assembly = Assembly.LoadFile(filename);
                    AssemblyName assemblyName = assembly.GetName();
                    Version version = assemblyName.Version;

                    txtAssembly.Text = assemblyName.Name;
                    txtVersion.Text = version.Major + "." + version.MajorRevision;

                    bool isValid = false;
                    Type[] types = assembly.GetTypes();
                    foreach (Type t in types)
                    {
                        //PlugInAttribute pluginAttr = null;
                        //if (IsValidPlugin(t, out pluginAttr))
                        //{
                        //    mPlugs.Add(t.FullName, tmp);
                        //}
                        if (IsValidPlugin(t))
                        {
                            isValid = true;
                            txtClassname.Text = t.FullName;
                        }
                    }
                    if (!isValid)
                    {
                        MessageBox.Show(this, "�Ǳ�׼�������ɲ����������ѡ����д�ļ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    //ѭ�����س��򼯵��ڴ��С�plugins.Add(pluginAssembly.CreateInstance(plugingType.FullName)); 
                }


            }
            catch (System.Exception ex)
            {
                MessageBox.Show(this, "�������ʧ�ܣ�\r\n���������Ƿ���Ͻӿڱ�׼���ļ��Ƿ���ȷ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogInfo.WriteLog(ex);
            }
            

        }
        #endregion

    }
}
