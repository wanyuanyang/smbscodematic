using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
namespace Codematic
{
    public partial class NewProject : Form
    {
        Thread mythread;
        string cmcfgfile = Application.StartupPath + @"\cmcfg.ini";
        LTP.Utility.INIFile cfgfile;
        string folder1 = "";//ԴĿ¼
        string folder2 = "";
        string ProName = "";




        public NewProject()
        {
            InitializeComponent();
            InitTreeView();
            InitListView();
            btn_ok.Enabled = false;            
        }
        private void NewProject_Load(object sender, EventArgs e)
        {
            if (File.Exists(cmcfgfile))
            {
                cfgfile = new LTP.Utility.INIFile(cmcfgfile);
                string lastpath = cfgfile.IniReadValue("Project", "lastpath");
                if (lastpath.Trim() != "")
                {
                    txtProPath.Text = lastpath;
                }
            }
        }

        #region listview
        private void InitTreeView()
        {
            TreeNode tnEnviroment = new TreeNode("����", 0, 1);
            //TreeNode tnEditor = new TreeNode("ģ��ܹ�", 2, 3);

            this.treeView1.Nodes.Add(tnEnviroment);
            //this.treeView1.Nodes.Add(tnEditor);

        }
        private void InitListView()
        {
            this.listView1.Columns.Clear();
            this.listView1.Items.Clear();
            this.listView1.LargeImageList = imageList1;
            //this.listView1.SmallImageList = imglistView;
            this.listView1.View = View.LargeIcon;
            listView1.HideSelection = false;

            ListViewGroup listViewGroup1 = new ListViewGroup("Codematic �Ѱ�װ��ģ��", HorizontalAlignment.Left);
            listViewGroup1.Header = "Codematic �Ѱ�װ��ģ��";
            listViewGroup1.Name = "listViewGroup1";
            this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1});

            string strname = "����ṹ";
            //ListViewItem item1 = new ListViewItem(strname, 0);
            //item1.SubItems.Add(strname);
            //item1.ImageIndex = 0;
            //item1.Group = listViewGroup1;
            //listView1.Items.AddRange(new ListViewItem[] { item1 });

            strname = "������ṹ";
            ListViewItem item2 = new ListViewItem(strname, 0);
            item2.SubItems.Add(strname);
            item2.ImageIndex = 1;
            item2.Group = listViewGroup1;            
            listView1.Items.AddRange(new ListViewItem[] { item2 });

            strname = "����ģʽ�ṹ";
            ListViewItem item3 = new ListViewItem(strname, 0);
            item3.SubItems.Add(strname);
            item3.ImageIndex = 2;
            item3.Group = listViewGroup1;
            listView1.Items.AddRange(new ListViewItem[] { item3 });

            strname = "������ṹ(����)";
            ListViewItem item4 = new ListViewItem(strname, 0);
            item4.SubItems.Add(strname);
            item4.ImageIndex = 1;
            item4.Group = listViewGroup1;
            listView1.Items.AddRange(new ListViewItem[] { item4 });
                        
            item2.Focused=true;
            item2.Selected = true;

        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1)
            {
                return;
            }
            string selstr = listView1.SelectedItems[0].Text;
            
            switch (selstr)
            {
                case "����ṹ":
                    lblTooltip.Text = "��������ṹ����Ŀ";
                    btn_ok.Enabled = false;   
                    break;
                case "������ṹ":
                    lblTooltip.Text = "����������ṹ����Ŀ(����VS2005)";
                    btn_ok.Enabled = true;   
                    break;
                case "������ṹ(����)":
                    lblTooltip.Text = "������������ϵͳ�����ܵļ�����ṹ����Ŀ(����VS2008)";
                    btn_ok.Enabled = true;
                    break;
                case "����ģʽ�ṹ":
                    lblTooltip.Text = "��������ģʽ�ṹ����Ŀ(����VS2005)";
                    btn_ok.Enabled = true;   
                    break;
            }
        }
        #endregion

        private void btn_browser_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            DialogResult result = folder.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.txtProPath.Text = folder.SelectedPath;
            }
        }

        #region ��ť
        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1)
            {
                return;
            }            
            string selstr = this.listView1.SelectedItems[0].Text;            
            string daltype = "S3";
            switch (selstr)
            {
                case "����ṹ":
                    {
                        daltype = "One";
                        MessageBox.Show("�ù����ݲ�֧�֣�����ѡ���������ܼ���ʹ�á�");
                    }
                    break;
                case "������ṹ":
                    {
                        folder1 = Application.StartupPath + "\\Template\\CodematicDemoS3";
                        daltype = "S3";                        
                    }
                    break;
                case "������ṹ(����)":
                    {
                        folder1 = Application.StartupPath + "\\Template\\CodematicDemoS3p";
                        daltype = "S3p";
                    }
                    break;
                case "����ģʽ�ṹ":
                    {
                        folder1 = Application.StartupPath + "\\Template\\CodematicDemoF3";
                        daltype = "F3";                        
                    }
                    break;
                default:
                    break;
            }
            
            #region ����Ŀ¼
            folder2 = txtProPath.Text.Trim();
            ProName = txtProName.Text.Trim();
            cfgfile.IniWriteValue("Project", "lastpath", folder2);
            if (ProName != "")
            {
                folder2 += "\\" + ProName;
            }
            else
            {
                MessageBox.Show("��������Ŀ���ƣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (txtProPath.Text.Trim() == "")
            {
                MessageBox.Show("��ѡ�����Ŀ¼��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string longservername = FormCommon.GetDbViewSelServer();
            if (longservername == "")
            {
                MessageBox.Show("����δѡ��Ҫ���ɴ�������ݿ���Ϣ��\r\n���Ƚ������ݿ����ӡ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #region Ŀ¼���
            //try
            //{
            //    DirectoryInfo source = new DirectoryInfo(folder1);
            //    DirectoryInfo target = new DirectoryInfo(folder2);
            //    if (!source.Exists)
            //    {
            //        MessageBox.Show("ԴĿ¼�Ѿ������ڣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //    if (!target.Exists)
            //    {
            //        try
            //        {
            //            target.Create();
            //        }
            //        catch
            //        {
            //            MessageBox.Show("Ŀ��Ŀ¼�����ڣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //            return;
            //        }
            //    }
            //}
            //catch
            //{
            //    MessageBox.Show("Ŀ¼��Ϣ����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            #endregion
                        
            //mythread = new Thread(new ThreadStart(ThreadWork));
            //mythread.Start();            

            NewProjectDB npb = new NewProjectDB(longservername, this, folder2, daltype, ProName);
            npb.Show();

            #endregion
            
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            
        }
        #endregion

        #region  ������Ŀ

        void ThreadWork()
        {
            CopyDirectory(folder1, folder2);
        }
        public void CopyDirectory(string SourceDirectory, string TargetDirectory)
        {
            DirectoryInfo source = new DirectoryInfo(SourceDirectory);
            DirectoryInfo target = new DirectoryInfo(TargetDirectory);
            //Check If we have valid source
            if (!source.Exists)
                return;
            if (!target.Exists)
                target.Create();
            //Copy Files
            FileInfo[] sourceFiles = source.GetFiles();
            int filescount = sourceFiles.Length;            
            for (int i = 0; i < filescount; ++i)
            {
                if ((sourceFiles[i].Extension == ".sln") || (sourceFiles[i].Extension == ".suo"))
                {
                    File.Copy(sourceFiles[i].FullName, target.FullName + "\\" + ProName + sourceFiles[i].Extension, true);
                }
                else
                {
                    File.Copy(sourceFiles[i].FullName, target.FullName + "\\" + sourceFiles[i].Name, true);
                }
            }
            //Copy directories
            DirectoryInfo[] sourceDirectories = source.GetDirectories();
            for (int j = 0; j < sourceDirectories.Length; ++j)
            {                
                CopyDirectory(sourceDirectories[j].FullName, target.FullName + "\\" + sourceDirectories[j].Name);
            }
        }

        #endregion

        
    }
}