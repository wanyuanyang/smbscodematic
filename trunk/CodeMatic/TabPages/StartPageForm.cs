using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Threading;

namespace Codematic
{
    public partial class StartPageForm : Form
    {
        MainForm mainfrm = null;
        private Thread thread;
        private string tempOpmlfile = Application.StartupPath + @"\tempopml.xml"; //��ʱRSS
        private string tempRssfile = Application.StartupPath + @"\temprss.xml"; //��ʱRSS       
        private string RssPath = "http://blog.csdn.net/litp/rss.aspx";	//rss��ַ
        string cmcfgfile = Application.StartupPath + @"\cmcfg.ini";
        LTP.Utility.INIFile cfgfile;
        delegate void SetTextCallback(string text);
        delegate void SetListCallback();


        public StartPageForm(Form mdiParentForm, string strRsspath)
        {
            InitializeComponent();
            mainfrm = (MainForm)mdiParentForm;
            RssPath = strRsspath;
        }

        private void StartPageForm_Load(object sender, EventArgs e)
        {
            try
            {
                //thread.IsBackground = true;
                this.thread = new Thread(new ThreadStart(LoadRss));
                this.thread.Start();

                //LoadRss();
            }
            catch //(Exception ex)
            {
                // MessageBox.Show(ex.ToString());
            }
        }

        #region LoadRss����Rss

        private void LoadRss()
        {
            try
            {
                CreatListview();
                LoadRss(RssPath);
            }
            catch (System.Exception ex)
            {
                //SetText("��ȡ��վ��Ϣʧ�ܣ��������������Ƿ��������Ժ����ԡ�");
                MessageBox.Show("��ȡ��վ��Ϣʧ�ܣ��������������Ƿ��������Ժ����ԡ�" + ex.Message);
                LogInfo.WriteLog(ex);
            }
        }
        private void LoadRss(string RssPath)
        {
            if (!IsHasLoaded())
            {
                SetText("���ڻ�ȡ��վ������Ϣ�����Ժ�");
                this.LoadXml2Coach(RssPath, tempRssfile);
                LoadedRssMarker();
            }

            SetText("���ڶ�ȡ������Ϣ��");
            SetListText();

            SetText("���");
        }
        #endregion

        #region ��������
        private void CreatListview()
        {
            //�����б�
            this.listView1.Columns.Clear();
            this.listView1.Items.Clear();
           
            this.listView1.View = View.Details;
            this.listView1.FullRowSelect = true;
            this.listView1.BackColor = Color.FromArgb(235, 241, 255);

            listView1.Columns.Add("", 300, HorizontalAlignment.Left);
            
        }

        private void SetText(string text)
        {
            if (this.lblTip.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.lblTip.Text = text;
            }
        }
        private void SetListText()
        {
            if (this.listView1.InvokeRequired)
            {
                SetListCallback d = new SetListCallback(SetListText);
                this.Invoke(d, null);
            }
            else
            {
                LoadItem();
            }
        }
        
        /// <summary>
        /// �Ƿ��Ѿ����ع�RSS����
        /// </summary>
        /// <returns></returns>
        private bool IsHasLoaded()
        {
            if (File.Exists(cmcfgfile))
            {
                cfgfile = new LTP.Utility.INIFile(cmcfgfile);
                string Contents = cfgfile.IniReadValue("updaterss", "today");
                if (Contents == DateTime.Today.ToString("yyyyMMdd"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ��ǽ����Ѿ����ع�RSS����
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="loginfo"></param>
        private void LoadedRssMarker()
        {
            cfgfile.IniWriteValue("updaterss", "today", DateTime.Today.ToString("yyyyMMdd"));
        }

        #endregion

        #region LoadXml2Coach��ȡRSS���ڱ���
        
        /// <summary>
        /// ��ȡRSS���ڱ���
        /// </summary>        
        private void LoadXml2Coach(string url, string tempfile)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(url);
                doc.Save(tempfile);
            }
            catch//(System.Exception ex)
            {
                //MessageBox.Show(ex.Message);
                //return;
            }
        }
        #endregion


        #region LoadItem����RSS��Ŀ

        /// <summary>
        /// ����RSS��Ŀ
        /// </summary>
        private void LoadItem()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(this.tempRssfile);
            XmlNodeList nodeList;
            nodeList = doc.SelectNodes("/rss/channel/item");
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");

            this.listView1.Items.Clear();
            try
            {
                foreach (XmlNode objNode in nodeList)//ѭ��ÿ��item��
                {
                    if (objNode.HasChildNodes == true)
                    {
                        string title = "";
                        string link = "";
                        string creator = "";
                        string author = "";
                        string pubDate = DateTime.Now.ToString();
                        XmlNodeList objItemsChild = objNode.ChildNodes;
                        foreach (XmlNode objNodeChild in objItemsChild)//ÿ��item���µĽڵ�ֵ
                        {
                            switch (objNodeChild.Name)
                            {
                                case "title":
                                    title = objNodeChild.InnerText;
                                    break;
                                case "link":
                                    link = objNodeChild.InnerText;
                                    break;
                                case "dc:creator":
                                    creator = objNodeChild.InnerText;
                                    break;
                                case "author":
                                    author = objNodeChild.InnerText;
                                    break;
                                case "pubDate":
                                    pubDate = objNodeChild.InnerText;
                                    pubDate = DateTime.Parse(pubDate).ToString();
                                    break;

                            }
                        }

                        ListViewItem item1 = new ListViewItem(title, 0);
                        item1.SubItems.Add(pubDate);
                        item1.SubItems.Add(link);
                        listView1.Items.AddRange(new ListViewItem[] { item1 });
                    }

                }
                listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.Dock = System.Windows.Forms.DockStyle.Fill;
                this.lblTip.Visible = false;
                pictureBox1.Visible = false;
                listView1.Visible = true;
            }
            catch//(Exception ex) 
            {
                //MessageBox.Show(ex.ToString()); 
            }
        }

        #endregion

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                string selstr = this.listView1.SelectedItems[0].Text;
                string link = this.listView1.SelectedItems[0].SubItems[2].Text;
                //��ʼҳ
                Crownwood.Magic.Controls.TabPage page = new Crownwood.Magic.Controls.TabPage();
                page.Title = selstr;
                page.Control = new IEView(mainfrm, link);
                //MainForm mainfrm = (MainForm)Application.OpenForms["MainForm"];
                mainfrm.tabControlMain.TabPages.Add(page);
                mainfrm.tabControlMain.SelectedTab = page;
            }
        }

        #region ���ò����¼�

        private void lblAddServer_Click(object sender, EventArgs e)
        {
            DbView dbview = new DbView(mainfrm);
            dbview.backgroundWorkerReg.RunWorkerAsync();
        }

        private void lblDBBrowser_Click(object sender, EventArgs e)
        {
            mainfrm.AddSinglePage(new DbBrowser(), "ժҪ");
        }

        private void lblDBScript_Click(object sender, EventArgs e)
        {
            string longservername = FormCommon.GetDbViewSelServer();
            if (longservername == "")
            {
                MessageBox.Show("û�п��õ����ݿ����ӣ������������ݿ��������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DbToScript ce = new DbToScript(longservername);
            ce.ShowDialog(this);
        }

        private void lblWord_Click(object sender, EventArgs e)
        {
            string longservername = FormCommon.GetDbViewSelServer();
            if (longservername == "")
            {
                MessageBox.Show("û�п��õ����ݿ����ӣ������������ݿ��������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DbToWord dbtoword = new DbToWord(longservername);
            dbtoword.Show();
        }

        private void lblNewPro_Click(object sender, EventArgs e)
        {
            NewProject newpro = new NewProject();
            newpro.ShowDialog(mainfrm);
        }

        private void lblCodeMaker_Click(object sender, EventArgs e)
        {
            string longservername = FormCommon.GetDbViewSelServer();
            if (longservername == "")
            {
                MessageBox.Show("û�п��õ����ݿ����ӣ������������ݿ��������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            mainfrm.AddSinglePage(new CodeMaker(), "��������");
        }

        private void lblCodeExport_Click(object sender, EventArgs e)
        {
            string longservername = FormCommon.GetDbViewSelServer();
            if (longservername == "")
            {
                MessageBox.Show("û�п��õ����ݿ����ӣ������������ݿ��������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            CodeExport ce = new CodeExport(longservername);
            ce.ShowDialog(this);
        }
        private void lblOption_Click(object sender, EventArgs e)
        {
            OptionFrm of = new OptionFrm(mainfrm);
            of.Show();
        }
        #endregion

        

    }
}