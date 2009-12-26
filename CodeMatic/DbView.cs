using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading;
using System.Text;
using System.IO;
using LTP.CodeHelper;
using System.Windows.Forms;
namespace Codematic
{
    //���ݿ������
    public partial class DbView : Form
    {
        #region ϵͳ����

        MainForm mainfrm;
        public static bool isMdb = false;//�Ƿ���mdb���ݿ�        
        LTP.IDBO.IDbObject dbobj;
        string path = Application.StartupPath;
        LoginForm logo = new LoginForm();
        LoginOra logoOra = new LoginOra();
        LoginOledb logoOledb = new LoginOledb();
        LoginMySQL loginMysql = new LoginMySQL();
        LoginSQLite loginSQLite = new LoginSQLite();
        DbTypeSel dbsel = new DbTypeSel();

        TreeNode TreeClickNode;//�Ҽ��˵������Ľڵ�
        TreeNode serverlistNode;
        private bool m_bLayoutCalled = false;
        LTP.CmConfig.ModuleSettings setting;

        #endregion

        delegate void AddTreeNodeCallback(TreeNode ParentNode, TreeNode Node);
        delegate void SetTreeNodeFontCallback(TreeNode Node, Font nodeFont);
        

        //delegate void SetTreeCallback(TreeNode serverNode, string dbtype, string ServerIp);

        public DbView(Form mdiParentForm)
        {
            mainfrm = (MainForm)mdiParentForm;
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            backgroundWorkerCon.DoWork += new DoWorkEventHandler(DoConnect);
            backgroundWorkerCon.ProgressChanged += new ProgressChangedEventHandler(ProgessChangedCon);
            backgroundWorkerCon.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompleteWorkCon);

            backgroundWorkerReg.DoWork += new DoWorkEventHandler(RegServer);
            backgroundWorkerReg.ProgressChanged += new ProgressChangedEventHandler(ProgessChangedReg);
            backgroundWorkerReg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompleteWorkReg);

            this.treeView1.ExpandAll();
        }

        #region FormLoad

        private void DbView_Load(object sender, EventArgs e)
        {
            LoadServer();
            setting = LTP.CmConfig.ModuleConfig.GetSettings();
            mainfrm = (MainForm)Application.OpenForms["MainForm"];
        }

        #endregion

        #region ��������

        // ����TabPage
        private void AddTabPage(string pageTitle, Control ctrForm)
        {
            if (mainfrm.tabControlMain.Visible == false)
            {
                mainfrm.tabControlMain.Visible = true;
            }
            Crownwood.Magic.Controls.TabPage page = new Crownwood.Magic.Controls.TabPage();
            page.Title = pageTitle;
            page.Control = ctrForm;
            //MainForm mainfrm = (MainForm)Application.OpenForms["MainForm"];
            mainfrm.tabControlMain.TabPages.Add(page);
            mainfrm.tabControlMain.SelectedTab = page;
        }

        // ����TabPage
        private void AddTabPage(string pageTitle, Control ctrForm, MainForm mainfrm)
        {
            if (mainfrm.tabControlMain.Visible == false)
            {
                mainfrm.tabControlMain.Visible = true;
            }
            Crownwood.Magic.Controls.TabPage page = new Crownwood.Magic.Controls.TabPage();
            page.Title = pageTitle;
            page.Control = ctrForm;
            mainfrm.tabControlMain.TabPages.Add(page);
            mainfrm.tabControlMain.SelectedTab = page;
        }

        // �����µ�Ψһ����ҳ���������ظ��ģ�
        private void AddSinglePage(Control control, string Title)
        {
            if (mainfrm.tabControlMain.Visible == false)
            {
                mainfrm.tabControlMain.Visible = true;
            }
            bool showed = false;
            Crownwood.Magic.Controls.TabPage currPage = null;
            foreach (Crownwood.Magic.Controls.TabPage page in mainfrm.tabControlMain.TabPages)
            {
                if (page.Control.Name == control.Name)
                {
                    showed = true;
                    currPage = page;
                }
            }
            if (!showed)//������
            {
                AddTabPage(Title, control);
            }
            else
            {
                mainfrm.tabControlMain.SelectedTab = currPage;
            }
        }

        /// <summary>
        /// �첽�߳���Ϊ�����ӽڵ�
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="Node"></param>
        public void AddTreeNode(TreeNode ParentNode, TreeNode Node)
        {
            if (this.treeView1.InvokeRequired)
            {
                AddTreeNodeCallback d = new AddTreeNodeCallback(AddTreeNode);
                this.Invoke(d, new object[] { ParentNode, Node });
            }
            else
            {
                ParentNode.Nodes.Add(Node);
            }
        }
        
        /// <summary>
        /// �첽�߳������ýڵ�����
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="Node"></param>
        public void SetTreeNodeFont(TreeNode Node,Font nodeFont)
        {
            if (this.treeView1.InvokeRequired)
            {
                SetTreeNodeFontCallback d = new SetTreeNodeFontCallback(SetTreeNodeFont);
                this.Invoke(d, new object[] { Node, nodeFont });
            }
            else
            {
                Node.NodeFont = nodeFont;
            }
        }

      
        /// <summary>
        /// �õ�ѡ�еķ�����������Ϣ
        /// </summary>
        /// <returns></returns>
        public string GetLongServername()
        {
            TreeNode SelNode = treeView1.SelectedNode;
            if (SelNode == null)
                return "";
            string loneServername = "";
            switch (SelNode.Tag.ToString())
            {
                case "serverlist":
                    return "";
                case "server":
                    {
                        loneServername = SelNode.Text;
                    }
                    break;
                case "db":
                    {
                        loneServername = SelNode.Parent.Text;

                    }
                    break;
                case "tableroot":
                case "viewroot":
                    {
                        loneServername = SelNode.Parent.Parent.Text;

                    }
                    break;
                case "table":
                case "view":
                    {
                        loneServername = SelNode.Parent.Parent.Parent.Text;
                    }
                    break;
                case "column":
                    loneServername = SelNode.Parent.Parent.Parent.Parent.Text;
                    break;
            }

            return loneServername;
        }

        //���ݷ��������õõ��������ڵ��ַ���
        private string GetserverNodeText(string servername, string dbtype, string dbname)
        {
            string str = servername + "(" + dbtype + ")";
            if ((dbname.Trim() != "") && (dbname.Trim() != "master"))
            {
                str += "(" + dbname + ")";
            }
            return str;
        }

        #endregion

        #region ��ʼ����������

        private void LoadServer()
        {
            this.treeView1.Nodes.Clear();
            serverlistNode = new TreeNode("������");
            serverlistNode.Tag = "serverlist";
            serverlistNode.ImageIndex = 0;
            serverlistNode.SelectedImageIndex = 0;
            treeView1.Nodes.Add(serverlistNode);

            LTP.CmConfig.DbSettings[] dbs = LTP.CmConfig.DbConfig.GetSettings();
            if (dbs != null)
            {
                foreach (LTP.CmConfig.DbSettings db in dbs)
                {
                    string servername = db.Server;
                    string dbtype = db.DbType;
                    string dbname = db.DbName;
                    TreeNode serverNode = new TreeNode(GetserverNodeText(servername, dbtype, dbname));
                    serverNode.ImageIndex = 1;
                    serverNode.SelectedImageIndex = 1;
                    serverNode.Tag = "server";
                    serverlistNode.Nodes.Add(serverNode);
                }
                serverlistNode.Expand();
            }
        }

        #endregion

        #region ������
        private void toolbtn_AddServer_Click(object sender, EventArgs e)
        {
            backgroundWorkerReg.RunWorkerAsync();
        }
        private void toolbtn_Connect_Click(object sender, EventArgs e)
        {
            if ((TreeClickNode == null) || (TreeClickNode.Tag.ToString() != "server"))
                return;
            if ((TreeClickNode.Tag.ToString() == "server") & (TreeClickNode.Nodes.Count > 0))
                return;

            backgroundWorkerCon.RunWorkerAsync();
        }

        private void toolbtn_unConnect_Click(object sender, EventArgs e)
        {
            if ((TreeClickNode == null) || (TreeClickNode.Tag.ToString() != "server"))
                return;

            try
            {
                if ((TreeClickNode.Tag.ToString() == "server") & (TreeClickNode.Nodes.Count > 0))
                {
                    TreeClickNode.Nodes.Clear();
                }
            }
            catch (System.Exception ex)
            {
                LogInfo.WriteLog(ex);
                MessageBox.Show("����ʧ�ܣ�" + ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void toolbtn_Refrush_Click(object sender, EventArgs e)
        {

        }


        #endregion

        #region treeview �����

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                TreeNode SelNode = this.treeView1.SelectedNode;
                string selstr = SelNode.Text;
                string Nodetype = SelNode.Tag.ToString().ToLower();
                mainfrm = (MainForm)Application.OpenForms["MainForm"];

                DbBrowser dbrowfrm = (DbBrowser)Application.OpenForms["DbBrowser"];
                if (dbrowfrm != null)
                {
                    mainfrm.StatusLabel1.Text = "���ڼ������ݿ�.....";
                    dbrowfrm.SetListView(this);
                }

                CodeMaker codemakerfrm = (CodeMaker)Application.OpenForms["CodeMaker"];
                if (codemakerfrm != null)
                {
                    mainfrm.StatusLabel1.Text = "���ڼ������ݿ�.....";
                    codemakerfrm.SetListView(this);
                }
                CodeMakerM codemakermfrm = (CodeMakerM)Application.OpenForms["CodeMakerM"];
                if (codemakermfrm != null)
                {
                    mainfrm.StatusLabel1.Text = "���ڼ������ݿ�.....";
                    codemakermfrm.SetListView(this);
                }

                CodeTemplate codetempfrm = (CodeTemplate)Application.OpenForms["CodeTemplate"];
                if (codetempfrm != null)
                {
                    mainfrm.StatusLabel1.Text = "���ڼ������ݿ�.....";
                    codetempfrm.SetListView(this);
                }

                #region  ѡ��ĳ���ͽڵ�
                switch (Nodetype)
                {
                    case "serverlist":
                        {
                            mainfrm.toolComboBox_DB.Items.Clear();
                            mainfrm.toolComboBox_Table.Items.Clear();
                            mainfrm.toolComboBox_DB.Visible = false;
                            mainfrm.toolComboBox_Table.Visible = false;

                            mainfrm.����ToolStripMenuItem.Visible = false;
                        }
                        break;
                    case "server":
                        {
                            mainfrm.toolComboBox_DB.Visible = true;
                            mainfrm.toolComboBox_Table.Visible = false;

                            mainfrm.����ToolStripMenuItem.Visible = false;
                        }
                        break;
                    case "db":
                        {
                            #region
                            mainfrm.toolComboBox_DB.Visible = true;
                            mainfrm.toolComboBox_Table.Visible = true;
                            mainfrm.toolComboBox_DB.Text = SelNode.Parent.Text;

                            mainfrm.����ToolStripMenuItem.Visible = false;
                            #endregion
                        }
                        break;
                    case "tableroot":
                    case "viewroot":
                        {
                            #region
                            mainfrm.toolComboBox_DB.Visible = true;
                            mainfrm.toolComboBox_Table.Visible = true;
                            mainfrm.toolComboBox_DB.Text = SelNode.Parent.Text;

                            mainfrm.����ToolStripMenuItem.Visible = false;
                            #endregion

                        }
                        break;
                    case "table":
                        {
                            #region
                            mainfrm.toolComboBox_DB.Visible = true;
                            mainfrm.toolComboBox_Table.Visible = true;
                            mainfrm.toolComboBox_DB.Text = SelNode.Parent.Parent.Text;
                            mainfrm.toolComboBox_Table.Text = selstr;

                            mainfrm.����ToolStripMenuItem.Visible = true;
                            mainfrm.������ToolStripMenuItem.Visible = false;
                            mainfrm.toolStripMenuItem17.Visible = false;
                            mainfrm.���ɴ洢����ToolStripMenuItem.Visible = true;
                            mainfrm.�������ݽű�ToolStripMenuItem.Visible = true;

                            #endregion
                        }
                        break;
                    case "view":
                        {
                            #region
                            mainfrm.toolComboBox_DB.Visible = true;
                            mainfrm.toolComboBox_Table.Visible = true;
                            mainfrm.toolComboBox_DB.Text = SelNode.Parent.Parent.Text;
                            mainfrm.toolComboBox_Table.Text = selstr;

                            mainfrm.����ToolStripMenuItem.Visible = true;
                            mainfrm.������ToolStripMenuItem.Visible = true;
                            mainfrm.toolStripMenuItem17.Visible = true;
                            mainfrm.���ɴ洢����ToolStripMenuItem.Visible = false;
                            mainfrm.�������ݽű�ToolStripMenuItem.Visible = false;

                            #endregion
                        }
                        break;
                    case "proc":
                        {
                            mainfrm.����ToolStripMenuItem.Visible = true;
                            mainfrm.������ToolStripMenuItem.Visible = true;
                            mainfrm.toolStripMenuItem17.Visible = true;
                            mainfrm.���ɴ洢����ToolStripMenuItem.Visible = false;
                            mainfrm.�������ݽű�ToolStripMenuItem.Visible = false;
                        }
                        break;
                    default:
                        {
                            mainfrm.����ToolStripMenuItem.Visible = false;
                        }
                        break;
                }
                #endregion

                mainfrm.StatusLabel1.Text = "����";
            }
            catch (System.Exception ex)
            {
                LogInfo.WriteLog(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                Point mpt = new Point(e.X, e.Y);
                TreeClickNode = this.treeView1.GetNodeAt(mpt);
                this.treeView1.SelectedNode = TreeClickNode;
                if (TreeClickNode != null)
                {
                    CreatMenu(TreeClickNode.Tag.ToString());
                    if (e.Button == MouseButtons.Right)
                    {
                        this.DbTreeContextMenu.Show(this.treeView1, mpt);
                        //this.treeView1.ContextMenu.Show(treeView1,mpt);
                    }
                }
                else
                {
                    DbTreeContextMenu.Items.Clear();
                }
            }
            catch(System.Exception ex)
            {
                LogInfo.WriteLog(ex);
            }
        }

        #endregion

        #region ����treeview �Ҽ��˵�
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));

        private void CreatMenu(string NodeType)
        {
            this.DbTreeContextMenu.Items.Clear();
            switch (NodeType)
            {
                case "serverlist":
                    {
                        #region
                        ToolStripMenuItem ��ӷ�����Item = new ToolStripMenuItem();
                        ��ӷ�����Item.Image = ((System.Drawing.Image)(resources.GetObject("toolbtn_AddServer.Image")));
                        //��ӷ�����Item.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
                        //��ӷ�����Item.ImageTransparentColor = System.Drawing.Color.Magenta;
                        ��ӷ�����Item.Name = "��ӷ�����Item";
                        ��ӷ�����Item.Text = "��ӷ�����";
                        ��ӷ�����Item.Click += new System.EventHandler(��ӷ�����Item_Click);

                        ToolStripMenuItem ���ݷ���������Item = new ToolStripMenuItem();
                        ���ݷ���������Item.Name = "���ݷ���������Item";
                        ���ݷ���������Item.Text = "���ݷ���������";
                        ���ݷ���������Item.Click += new System.EventHandler(���ݷ���������Item_Click);

                        ToolStripMenuItem �������������Item = new ToolStripMenuItem();
                        �������������Item.Name = "�������������Item";
                        �������������Item.Text = "�������������";
                        �������������Item.Click += new System.EventHandler(�������������Item_Click);

                        ToolStripMenuItem ˢ��Item = new ToolStripMenuItem();
                        ˢ��Item.Name = "ˢ��Item";
                        ˢ��Item.Text = "ˢ��";
                        ˢ��Item.Click += new System.EventHandler(ˢ��Item_Click);

                        ToolStripMenuItem ����Item = new ToolStripMenuItem();
                        ����Item.Name = "����Item";
                        ����Item.Text = "����";
                        ����Item.Click += new System.EventHandler(����Item_Click);

                        DbTreeContextMenu.Items.AddRange(
                            new System.Windows.Forms.ToolStripItem[] { 
                                ��ӷ�����Item, 
                                ���ݷ���������Item,
                                �������������Item,
                                ˢ��Item                                
                            }
                            );
                        #endregion
                    }
                    break;
                case "server":
                    {
                        #region
                        ToolStripMenuItem ���ӷ�����Item = new ToolStripMenuItem();
                        ���ӷ�����Item.Image = ((System.Drawing.Image)(resources.GetObject("toolbtn_Connect.Image")));
                        //���ӷ�����Item.ImageTransparentColor = System.Drawing.Color.Magenta;
                        ���ӷ�����Item.Name = "���ӷ�����Item";
                        ���ӷ�����Item.Text = "���ӷ�����";
                        ���ӷ�����Item.Click += new System.EventHandler(���ӷ�����Item_Click);

                        ToolStripMenuItem ע��������Item = new ToolStripMenuItem();
                        ע��������Item.Name = "ע��������Item";
                        ע��������Item.Text = "ע��������";
                        ע��������Item.Click += new System.EventHandler(ע��������Item_Click);

                        ToolStripMenuItem ����Item = new ToolStripMenuItem();
                        ����Item.Name = "����Item";
                        ����Item.Text = "ˢ��";
                        ����Item.Click += new System.EventHandler(server����Item_Click);

                        DbTreeContextMenu.Items.AddRange(
                            new System.Windows.Forms.ToolStripItem[] { 
                                ���ӷ�����Item, 
                                ע��������Item,
                                ����Item
                            }
                            );
                        #endregion
                    }
                    break;
                case "db":
                    {
                        #region
                        ToolStripMenuItem ������ݿ�Item = new ToolStripMenuItem();
                        ������ݿ�Item.Image = ((System.Drawing.Image)(resources.GetObject("���ݿ������ToolStripMenuItem.Image")));
                        ������ݿ�Item.Name = "������ݿ�Item";
                        ������ݿ�Item.Text = "������ݿ�";
                        ������ݿ�Item.Click += new System.EventHandler(������ݿ�Item_Click);


                        ToolStripMenuItem �½���ѯItem = new ToolStripMenuItem();
                        �½���ѯItem.Image = ((System.Drawing.Image)(resources.GetObject("��ѯ������ToolStripMenuItem.Image")));
                        �½���ѯItem.Name = "�½���ѯItem";
                        �½���ѯItem.Text = "�½���ѯ";
                        �½���ѯItem.Click += new System.EventHandler(�½���ѯItem_Click);

                        ToolStripMenuItem �½�NET��ĿItem = new ToolStripMenuItem();
                        �½�NET��ĿItem.Image = ((System.Drawing.Image)(resources.GetObject("toolBtn_NewProject.Image")));
                        �½�NET��ĿItem.Name = "�½�NET��ĿItem";
                        �½�NET��ĿItem.Text = "�½�NET��Ŀ";
                        �½�NET��ĿItem.Click += new System.EventHandler(�½�NET��ĿItem_Click);

                        ToolStripSeparator Separator1 = new ToolStripSeparator();
                        Separator1.Name = "Separator1";

                        ToolStripMenuItem ���ɴ洢����dbItem = new ToolStripMenuItem();
                        ���ɴ洢����dbItem.Name = "���ɴ洢����Item";
                        ���ɴ洢����dbItem.Text = "���ɴ洢����";
                        ���ɴ洢����dbItem.Click += new System.EventHandler(���ɴ洢����dbItem_Click);

                        ToolStripMenuItem �������ݽű�dbItem = new ToolStripMenuItem();
                        �������ݽű�dbItem.Image = ((System.Drawing.Image)(resources.GetObject("dB�ű�������ToolStripMenuItem.Image")));
                        �������ݽű�dbItem.Name = "�������ݽű�Item";
                        �������ݽű�dbItem.Text = "�������ݽű�";
                        �������ݽű�dbItem.Click += new System.EventHandler(�������ݽű�dbItem_Click);

                        #region  �����ļ�

                        ToolStripMenuItem �����ļ�dbItem = new ToolStripMenuItem();
                        �����ļ�dbItem.Name = "�����ļ�Item";
                        �����ļ�dbItem.Text = "�����ļ�";

                        ToolStripMenuItem �洢����dbItem = new ToolStripMenuItem();
                        �洢����dbItem.Name = "�洢����Item";
                        �洢����dbItem.Text = "�洢����";
                        �洢����dbItem.Click += new System.EventHandler(�洢����dbItem_Click);

                        ToolStripMenuItem ���ݽű�dbItem = new ToolStripMenuItem();
                        ���ݽű�dbItem.Name = "���ݽű�Item";
                        ���ݽű�dbItem.Text = "���ݽű�";
                        ���ݽű�dbItem.Click += new System.EventHandler(���ݽű�dbItem_Click);

                        ToolStripMenuItem ������dbItem = new ToolStripMenuItem();
                        ������dbItem.Name = "������Item";
                        ������dbItem.Text = "������";
                        ������dbItem.Click += new System.EventHandler(������dbItem_Click);

                        #endregion

                        �����ļ�dbItem.DropDownItems.AddRange(
                            new System.Windows.Forms.ToolStripItem[] { 
                                �洢����dbItem, 
                                ���ݽű�dbItem,
                                ������dbItem                                
                            }
                            );

                        ToolStripSeparator Separator2 = new ToolStripSeparator();
                        Separator1.Name = "Separator2";

                        ToolStripMenuItem ���ӱ��������dbItem = new ToolStripMenuItem();
                        ���ӱ��������dbItem.Name = "���ӱ��������Item";
                        ���ӱ��������dbItem.Text = "���ӱ��������(����)";
                        ���ӱ��������dbItem.Click += new System.EventHandler(���ӱ��������dbItem_Click);

                        ToolStripMenuItem ������������dbItem = new ToolStripMenuItem();
                        ������������dbItem.Image = ((System.Drawing.Image)(resources.GetObject("�����Զ������ToolStripMenuItem.Image")));
                        ������������dbItem.Name = "������������dbItem";
                        ������������dbItem.Text = "������������";
                        ������������dbItem.Click += new System.EventHandler(������������dbItem_Click);

                        ToolStripSeparator Separatordb2 = new ToolStripSeparator();
                        Separator1.Name = "Separatordb2";

                        ToolStripMenuItem ˢ��dbItem = new ToolStripMenuItem();
                        ˢ��dbItem.Name = "ˢ��Item";
                        ˢ��dbItem.Text = "ˢ��";
                        ˢ��dbItem.Click += new System.EventHandler(ˢ��dbItem_Click);


                        DbTreeContextMenu.Items.AddRange(
                            new System.Windows.Forms.ToolStripItem[] { 
                                ������ݿ�Item,�½���ѯItem,�½�NET��ĿItem,Separator1,
                                ���ɴ洢����dbItem,
                                �������ݽű�dbItem,
                                �����ļ�dbItem,
                                Separator2,        
                                ���ӱ��������dbItem,
                                ������������dbItem,
                                Separatordb2,
                                ˢ��dbItem
                            }
                            );
                        #endregion

                    }
                    break;
                case "tableroot":
                    break;
                case "viewroot":
                    break;
                case "procroot":
                    break;
                case "table":
                    {
                        #region

                        ToolStripMenuItem ����SQL���Item = new ToolStripMenuItem();
                        ����SQL���Item.Name = "����SQL���Item";
                        ����SQL���Item.Text = "����SQL��䵽";

                        #region ����SQL��䵽

                        ToolStripMenuItem SELECTItem = new ToolStripMenuItem();
                        SELECTItem.Name = "SELECTItem";
                        SELECTItem.Text = "SELECT(&S)";
                        SELECTItem.Click += new System.EventHandler(SELECTItem_Click);

                        ToolStripMenuItem UPDATEItem = new ToolStripMenuItem();
                        UPDATEItem.Name = "UPDATEItem";
                        UPDATEItem.Text = "UPDATE(&U)";
                        UPDATEItem.Click += new System.EventHandler(UPDATEItem_Click);

                        ToolStripMenuItem DELETEItem = new ToolStripMenuItem();
                        DELETEItem.Name = "DELETEItem";
                        DELETEItem.Text = "DELETE(&D)";
                        DELETEItem.Click += new System.EventHandler(DELETEItem_Click);

                        ToolStripMenuItem INSERTItem = new ToolStripMenuItem();
                        INSERTItem.Name = "INSERTItem";
                        INSERTItem.Text = "INSERT(&I)";
                        INSERTItem.Click += new System.EventHandler(INSERTItem_Click);

                        ����SQL���Item.DropDownItems.AddRange(
                            new System.Windows.Forms.ToolStripItem[] { 
                                SELECTItem, 
                                UPDATEItem,
                                DELETEItem,
                                INSERTItem
                            }
                            );

                        #endregion

                        ToolStripMenuItem �鿴������tabItem = new ToolStripMenuItem();
                        �鿴������tabItem.Name = "�鿴������tabItem";
                        �鿴������tabItem.Text = "���������";
                        �鿴������tabItem.Click += new System.EventHandler(�鿴������tabItem_Click);

                        ToolStripMenuItem �������ݽű�tabItem = new ToolStripMenuItem();
                        �������ݽű�tabItem.Image = ((System.Drawing.Image)(resources.GetObject("dB�ű�������ToolStripMenuItem.Image")));
                        �������ݽű�tabItem.Name = "�������ݽű�tabItem";
                        �������ݽű�tabItem.Text = "�������ݽű�";
                        �������ݽű�tabItem.Click += new System.EventHandler(�������ݽű�tabItem_Click);

                        ToolStripMenuItem ���ɴ洢����tabItem = new ToolStripMenuItem();
                        ���ɴ洢����tabItem.Name = "���ɴ洢����tabItem";
                        ���ɴ洢����tabItem.Text = "���ɴ洢����";
                        ���ɴ洢����tabItem.Click += new System.EventHandler(���ɴ洢����tabItem_Click);

                        #region �����ļ�Item
                        ToolStripMenuItem �����ļ�tabItem = new ToolStripMenuItem();
                        �����ļ�tabItem.Name = "�����ļ�tabItem";
                        �����ļ�tabItem.Text = "�����ļ�";

                        ToolStripMenuItem �洢����tabItem = new ToolStripMenuItem();
                        �洢����tabItem.Name = "�洢����tabItem";
                        �洢����tabItem.Text = "�洢����";
                        �洢����tabItem.Click += new System.EventHandler(�洢����tabItem_Click);

                        ToolStripMenuItem ���ݽű�tabItem = new ToolStripMenuItem();
                        ���ݽű�tabItem.Name = "���ݽű�tabItem";
                        ���ݽű�tabItem.Text = "���ݽű�";
                        ���ݽű�tabItem.Click += new System.EventHandler(���ݽű�tabItem_Click);

                        ToolStripMenuItem ������tabItem = new ToolStripMenuItem();
                        ������tabItem.Name = "������tabItem";
                        ������tabItem.Text = "������";
                        ������tabItem.Click += new System.EventHandler(������tabItem_Click);

                        �����ļ�tabItem.DropDownItems.AddRange(
                            new System.Windows.Forms.ToolStripItem[] { 
                                �洢����tabItem, 
                                ���ݽű�tabItem,
                                ������tabItem                                
                            }
                            );
                        #endregion


                        ToolStripSeparator Separator1 = new ToolStripSeparator();
                        Separator1.Name = "Separator1";

                        #region ��������Item

                        ToolStripMenuItem ��������Item = new ToolStripMenuItem();
                        ��������Item.Image = ((System.Drawing.Image)(resources.GetObject("����������ToolStripMenuItem.Image")));
                        ��������Item.Name = "��������Item";
                        ��������Item.Text = "����������";
                        ��������Item.Click += new System.EventHandler(��������Item_Click);

                        ToolStripMenuItem ģ���������Item = new ToolStripMenuItem();
                        ģ���������Item.Name = "ģ���������Item";
                        ģ���������Item.Text = "ģ���������";
                        ģ���������Item.Click += new System.EventHandler(ģ���������Item_Click);

                        ToolStripMenuItem ���ɵ���ṹItem = new ToolStripMenuItem();
                        ���ɵ���ṹItem.Name = "���ɵ���ṹItem";
                        ���ɵ���ṹItem.Text = "���ɵ���ṹ";
                        ���ɵ���ṹItem.Click += new System.EventHandler(���ɵ���ṹItems_Click);

                        ToolStripMenuItem ����ModelItem = new ToolStripMenuItem();
                        ����ModelItem.Name = "����ModelItem";
                        ����ModelItem.Text = "����Model";
                        ����ModelItem.Click += new System.EventHandler(����ModelItem_Click);

                        ToolStripSeparator Separator3 = new ToolStripSeparator();
                        Separator3.Name = "Separator3";


                        ToolStripMenuItem ������Item = new ToolStripMenuItem();
                        ������Item.Name = "������Item";
                        ������Item.Text = "������";

                        #region ������

                        ToolStripMenuItem ����DALS3Item = new ToolStripMenuItem();
                        ����DALS3Item.Name = "����DALS3Item";
                        ����DALS3Item.Text = "����DAL";
                        ����DALS3Item.Click += new System.EventHandler(����DALS3Item_Click);

                        ToolStripMenuItem ����BLLS3Item = new ToolStripMenuItem();
                        ����BLLS3Item.Name = "����BLLS3Item";
                        ����BLLS3Item.Text = "����BLL";
                        ����BLLS3Item.Click += new System.EventHandler(����BLLS3Item_Click);

                        ToolStripMenuItem ����ȫ��S3Item = new ToolStripMenuItem();
                        ����ȫ��S3Item.Name = "����ȫ��S3";
                        ����ȫ��S3Item.Text = "����ȫ��";
                        ����ȫ��S3Item.Click += new System.EventHandler(����ȫ��S3Item_Click);

                        ������Item.DropDownItems.AddRange(
                            new System.Windows.Forms.ToolStripItem[] { 
                                ����DALS3Item, 
                                ����BLLS3Item,
                                ����ȫ��S3Item                                
                            }
                            );

                        #endregion


                        ToolStripMenuItem ����ģʽ����Item = new ToolStripMenuItem();
                        ����ģʽ����Item.Name = "����ģʽ����Item";
                        ����ģʽ����Item.Text = "����ģʽ����";

                        #region ����ģʽ����

                        ToolStripMenuItem ����DALF3Item = new ToolStripMenuItem();
                        ����DALF3Item.Name = "����DALF3Item";
                        ����DALF3Item.Text = "����DAL";
                        ����DALF3Item.Click += new System.EventHandler(����DALF3Item_Click);

                        ToolStripMenuItem ����IDALItem = new ToolStripMenuItem();
                        ����IDALItem.Name = "����IDALItem";
                        ����IDALItem.Text = "����DAL";
                        ����IDALItem.Click += new System.EventHandler(����IDALItem_Click);

                        ToolStripMenuItem ����DALFactoryItem = new ToolStripMenuItem();
                        ����DALFactoryItem.Name = "����DALFactoryItem";
                        ����DALFactoryItem.Text = "����DALFactory";
                        ����DALFactoryItem.Click += new System.EventHandler(����DALFactoryItem_Click);

                        ToolStripMenuItem ����BLLF3Item = new ToolStripMenuItem();
                        ����BLLF3Item.Name = "����BLLF3Item";
                        ����BLLF3Item.Text = "����BLL";
                        ����BLLF3Item.Click += new System.EventHandler(����BLLF3Item_Click);

                        ToolStripMenuItem ����ȫ��F3Item = new ToolStripMenuItem();
                        ����ȫ��F3Item.Name = "����ȫ��F3Item";
                        ����ȫ��F3Item.Text = "����ȫ��";
                        ����ȫ��F3Item.Click += new System.EventHandler(����ȫ��F3Item_Click);

                        ����ģʽ����Item.DropDownItems.AddRange(
                            new System.Windows.Forms.ToolStripItem[] { 
                                ����DALF3Item, 
                                ����IDALItem,
                                ����DALFactoryItem,
                                ����BLLF3Item,
                                ����ȫ��F3Item
                            }
                            );


                        #endregion

                        ToolStripSeparator Separator5 = new ToolStripSeparator();
                        Separator5.Name = "Separator5";

                        ToolStripMenuItem ����ҳ��Item = new ToolStripMenuItem();
                        ����ҳ��Item.Name = "����ҳ��Item";
                        ����ҳ��Item.Text = "����ҳ��";
                        ����ҳ��Item.Click += new System.EventHandler(����ҳ��Item_Click);

                        //��������Item.DropDownItems.AddRange(
                        //    new System.Windows.Forms.ToolStripItem[] { 
                        //        ���ɵ���ṹItem,
                        //        ����ModelItem, 
                        //        Separator3,
                        //        ������Item,
                        //        ����ģʽ����Item,
                        //        Separator5,
                        //        ����ҳ��Item
                        //    }
                        //    );

                        #endregion

                        ToolStripSeparator Separator2 = new ToolStripSeparator();
                        Separator2.Name = "Separator2";


                        ToolStripMenuItem ���ӱ��������Item = new ToolStripMenuItem();
                        ���ӱ��������Item.Name = "���ӱ��������Item";
                        ���ӱ��������Item.Text = "���ӱ��������(����)";
                        ���ӱ��������Item.Click += new System.EventHandler(���ӱ��������Item_Click);

                        ToolStripMenuItem ������������Item = new ToolStripMenuItem();
                        ������������Item.Image = ((System.Drawing.Image)(resources.GetObject("�����Զ������ToolStripMenuItem.Image")));
                        ������������Item.Name = "������������";
                        ������������Item.Text = "������������";
                        ������������Item.Click += new System.EventHandler(������������Item_Click);

                        ToolStripSeparator Separator4 = new ToolStripSeparator();
                        Separator4.Name = "Separator4";

                        ToolStripMenuItem ������tabItem = new ToolStripMenuItem();
                        ������tabItem.Name = "������tabItem";
                        ������tabItem.Text = "������";
                        ������tabItem.Click += new System.EventHandler(������tabItem_Click);

                        ToolStripMenuItem ɾ��tabItem = new ToolStripMenuItem();
                        ɾ��tabItem.Name = "ɾ��tabItem";
                        ɾ��tabItem.Text = "ɾ��";
                        ɾ��tabItem.Click += new System.EventHandler(ɾ��tabItem_Click);

                        DbTreeContextMenu.Items.AddRange(
                            new System.Windows.Forms.ToolStripItem[] {                                 
                                ����SQL���Item,
                                �鿴������tabItem,
                                �������ݽű�tabItem,
                                ���ɴ洢����tabItem,                               
                                �����ļ�tabItem,
                                Separator1,
                                ��������Item,
                                ���ӱ��������Item,
                                ������������Item,
                                ģ���������Item,                                
                                Separator4,
                                ������tabItem,
                                ɾ��tabItem
                            }
                            );

                        #endregion
                    }
                    break;
                case "view":
                    {
                        #region
                        ToolStripMenuItem �ű�Item = new ToolStripMenuItem();
                        �ű�Item.Name = "�ű�Item";
                        �ű�Item.Text = "�ű�";

                        ToolStripMenuItem SELECTviewItem = new ToolStripMenuItem();
                        SELECTviewItem.Name = "SELECTItem";
                        SELECTviewItem.Text = "SELECT(&S)";
                        //SELECTviewItem.Click += new System.EventHandler(SELECTviewItem_Click);

                        ToolStripMenuItem AlterItem = new ToolStripMenuItem();
                        AlterItem.Name = "AlterItem";
                        AlterItem.Text = "ALTER(&U)";
                        //AlterItem.Click += new System.EventHandler(AlterItem_Click);

                        ToolStripMenuItem DropItem = new ToolStripMenuItem();
                        DropItem.Name = "DropItem";
                        DropItem.Text = "DROP(&D)";
                        //DropItem.Click += new System.EventHandler(DropItem_Click);


                        �ű�Item.DropDownItems.AddRange(
                            new System.Windows.Forms.ToolStripItem[] { 
                                SELECTviewItem, 
                                AlterItem,
                                DropItem                               
                            }
                            );


                        ToolStripMenuItem ������Item = new ToolStripMenuItem();
                        ������Item.Name = "������Item";
                        ������Item.Text = "������";
                        ������Item.Click += new System.EventHandler(������Item_Click);

                        ToolStripMenuItem �鿴������tabItem = new ToolStripMenuItem();
                        �鿴������tabItem.Name = "�鿴������tabItem";
                        �鿴������tabItem.Text = "���������";
                        �鿴������tabItem.Click += new System.EventHandler(�鿴������tabItem_Click);

                        ToolStripSeparator Separatorv1 = new ToolStripSeparator();
                        Separatorv1.Name = "Separatorv1";


                        ToolStripMenuItem ��������Item = new ToolStripMenuItem();
                        ��������Item.Name = "��������Item";
                        ��������Item.Text = "����������";
                        ��������Item.Click += new System.EventHandler(��������Item_Click);


                        DbTreeContextMenu.Items.AddRange(
                            new System.Windows.Forms.ToolStripItem[] { 
                                �ű�Item, 
                                ������Item,
                                �鿴������tabItem,
                                Separatorv1,
                                ��������Item
                            }
                            );
                        #endregion
                    }
                    break;
                case "proc":
                    {
                        #region
                        ToolStripMenuItem �ű�Item = new ToolStripMenuItem();
                        �ű�Item.Name = "�ű�Item";
                        �ű�Item.Text = "�ű�";

                        ToolStripMenuItem AlterItem = new ToolStripMenuItem();
                        AlterItem.Name = "AlterItem";
                        AlterItem.Text = "ALTER(&U)";
                        //AlterItem.Click += new System.EventHandler(AlterItem_Click);

                        ToolStripMenuItem DropItem = new ToolStripMenuItem();
                        DropItem.Name = "DropItem";
                        DropItem.Text = "DROP(&D)";
                        //DropItem.Click += new System.EventHandler(DropItem_Click);

                        ToolStripMenuItem EXECItem = new ToolStripMenuItem();
                        EXECItem.Name = "EXECItem";
                        EXECItem.Text = "EXEC(&I)";
                        //EXECItem.Click += new System.EventHandler(EXECItem_Click);

                        �ű�Item.DropDownItems.AddRange(
                            new System.Windows.Forms.ToolStripItem[] {                                 
                                AlterItem,
                                DropItem,
                                EXECItem
                            }
                            );

                        ToolStripMenuItem ������Item = new ToolStripMenuItem();
                        ������Item.Name = "������Item";
                        ������Item.Text = "������";
                        ������Item.Click += new System.EventHandler(������Item_Click);


                        DbTreeContextMenu.Items.AddRange(
                            new System.Windows.Forms.ToolStripItem[] { 
                                �ű�Item, 
                                ������Item                                
                            }
                            );
                        #endregion
                    }
                    break;
                case "column":
                    break;
            }
        }

        #endregion

        #region  treeview �Ҽ��˵��¼�

        #region serverlist_click

        private void ��ӷ�����Item_Click(object sender, EventArgs e)
        {
            backgroundWorkerReg.RunWorkerAsync();
        }
        private void ���ݷ���������Item_Click(object sender, EventArgs e)
        {
            SaveFileDialog sqlsavedlg = new SaveFileDialog();
            sqlsavedlg.Title = "�������������";
            sqlsavedlg.Filter = "DB Serverlist(*.config)|*.config|All files (*.*)|*.*";
            DialogResult dlgresult = sqlsavedlg.ShowDialog(this);
            if (dlgresult == DialogResult.OK)
            {
                string filename = sqlsavedlg.FileName;
                DataSet ds = LTP.CmConfig.DbConfig.GetSettingDs();
                ds.WriteXml(filename);
            }
        }
        private void �������������Item_Click(object sender, EventArgs e)
        {
            OpenFileDialog sqlfiledlg = new OpenFileDialog();
            sqlfiledlg.Title = "ѡ������������ļ�";
            sqlfiledlg.Filter = "DB Serverlist(*.config)|*.config|All files (*.*)|*.*";
            DialogResult result = sqlfiledlg.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                try
                {
                    string filename = sqlfiledlg.FileName;
                    DataSet ds = new DataSet();
                    if (File.Exists(filename))
                    {
                        ds.ReadXml(filename);
                        string fileNamelocal = Application.StartupPath + "\\DbSetting.config";
                        ds.WriteXml(fileNamelocal);
                    }
                    LoadServer();
                }
                catch(System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show("��ȡ�����ļ�ʧ�ܣ�", "��ʾ");
                }
            }
        }
        private void ˢ��Item_Click(object sender, EventArgs e)
        {
            LoadServer();
        }
        private void ����Item_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("ˢ��Item");
        }
        #endregion

        #region server_click

        private void ���ӷ�����Item_Click(object sender, EventArgs e)
        {
            backgroundWorkerCon.RunWorkerAsync();
        }

        private void ע��������Item_Click(object sender, EventArgs e)
        {
            try
            {
                if (TreeClickNode != null)
                {
                    string nodetext = TreeClickNode.Text;
                    LTP.CmConfig.DbSettings dbset = LTP.CmConfig.DbConfig.GetSetting(nodetext);
                    if (dbset != null)
                    {
                        LTP.CmConfig.DbConfig.DelSetting(dbset.DbType, dbset.Server, dbset.DbName);
                    }
                    serverlistNode.Nodes.Remove(TreeClickNode);
                }
            }
            catch
            {
                MessageBox.Show("ע��������ʧ�ܣ���رպ����´����ԡ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        //ˢ�·�����
        private void server����Item_Click(object sender, EventArgs e)
        {
            backgroundWorkerCon.RunWorkerAsync();
        }
        #endregion

        #region db_click
        private void ������ݿ�Item_Click(object sender, EventArgs e)
        {
            AddSinglePage(new DbBrowser(), "ժҪ");

        }
        private void �½���ѯItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode != null)
            {
                //��comboxѡ�е�ǰ��
                string dbname = TreeClickNode.Text;
                string server = TreeClickNode.Parent.Text;
                string title = server + "." + dbname + "��ѯ.sql";
                MainForm mainfrm = (MainForm)Application.OpenForms["MainForm"];
                AddTabPage(title, new DbQuery(mainfrm, ""), mainfrm);
                mainfrm.toolComboBox_DB.Text = dbname;
            }

        }
        private void �½�NET��ĿItem_Click(object sender, EventArgs e)
        {
            NewProject newpro = new NewProject();
            newpro.ShowDialog(mainfrm);
        }
        private void ���ɴ洢����dbItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode != null)
            {
                string longservername = TreeClickNode.Parent.Text;
                string dbname = TreeClickNode.Text;
                LTP.IDBO.IDbScriptBuilder dsb = ObjHelper.CreatDsb(longservername);
                string strSQL = dsb.GetPROCCode(dbname);
                string title = dbname + "�洢����.sql";
                AddTabPage(title, new DbQuery(mainfrm, strSQL));
            }
        }
        private void �������ݽű�dbItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode != null)
            {
                string longservername = TreeClickNode.Parent.Text;
                string dbname = TreeClickNode.Text;
                DbToScript dts = new DbToScript(longservername, dbname);
                dts.ShowDialog(this);
            }
        }
        private void �洢����dbItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sqlsavedlg = new SaveFileDialog();
            sqlsavedlg.Title = "���浱ǰ�ű�";
            sqlsavedlg.Filter = "sql files (*.sql)|*.sql|All files (*.*)|*.*";
            DialogResult dlgresult = sqlsavedlg.ShowDialog(this);
            if (dlgresult == DialogResult.OK)
            {
                if (TreeClickNode != null)
                {
                    string longservername = TreeClickNode.Parent.Text;
                    string dbname = TreeClickNode.Text;
                    LTP.IDBO.IDbScriptBuilder dsb = ObjHelper.CreatDsb(longservername);
                    string strSQL = dsb.GetPROCCode(dbname);

                    string filename = sqlsavedlg.FileName;
                    StreamWriter sw = new StreamWriter(filename, false, Encoding.Default);//,false);
                    sw.Write(strSQL);
                    sw.Flush();//�ӻ�����д����������ļ���
                    sw.Close();
                    MessageBox.Show("�ű����ɳɹ���", "���", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }


        }
        private void ���ݽű�dbItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode != null)
            {
                string longservername = TreeClickNode.Parent.Text;
                string dbname = TreeClickNode.Text;
                DbToScript dts = new DbToScript(longservername, dbname);
                dts.ShowDialog(this);
            }

        }
        private void ������dbItem_Click(object sender, EventArgs e)
        {

        }

        //����������
        private void ��������dbItem_Click(object sender, EventArgs e)
        {
            string longservername = TreeClickNode.Parent.Text;
            if (longservername == "")
                return;
            mainfrm.AddSinglePage(new CodeMaker(), "����������");
        }

        private void ���ӱ��������dbItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode != null)
            {
                string longservername = TreeClickNode.Parent.Text;
                string dbname = TreeClickNode.Text;
                if (longservername == "")
                    return;
                mainfrm.AddSinglePage(new CodeMakerM(dbname), "���ӱ��������");
            }
        }

        private void ������������dbItem_Click(object sender, EventArgs e)
        {
            string longservername = TreeClickNode.Parent.Text;
            if (longservername == "")
                return;
            CodeExport ce = new CodeExport(longservername);
            ce.ShowDialog(this);
        }
        private void ˢ��dbItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = TreeClickNode;
            string servercfg = tn.Parent.Text;
            LTP.CmConfig.DbSettings dbset = LTP.CmConfig.DbConfig.GetSetting(servercfg);
            string server = dbset.Server;
            string dbtype = dbset.DbType;
            //string dbname = dbset.DbName;  

            tn.Nodes.Clear();

            LTP.IDBO.IDbObject dbobj2 = LTP.DBFactory.DBOMaker.CreateDbObj(dbtype);
            dbobj2.DbConnectStr = dbset.ConnectStr;

            string dbname = tn.Text;
            mainfrm.StatusLabel1.Text = "�������ݿ�" + dbname + "...";

            TreeNode tabNode = new TreeNode("��");
            tabNode.ImageIndex = 3;
            tabNode.SelectedImageIndex = 4;
            tabNode.Tag = "tableroot";
            tn.Nodes.Add(tabNode);

            TreeNode viewNode = new TreeNode("��ͼ");
            viewNode.ImageIndex = 3;
            viewNode.SelectedImageIndex = 4;
            viewNode.Tag = "viewroot";
            tn.Nodes.Add(viewNode);

            TreeNode procNode = new TreeNode("�洢����");
            procNode.ImageIndex = 3;
            procNode.SelectedImageIndex = 4;
            procNode.Tag = "procroot";
            tn.Nodes.Add(procNode);

            #region ��

            try
            {
                List<string> tabNames = dbobj2.GetTables(dbname);
                if (tabNames.Count > 0)
                {
                    //DataRow[] dRows = dt.Select("", "name ASC");
                    foreach (string tabname in tabNames)
                    {
                        TreeNode tbNode = new TreeNode(tabname);
                        tbNode.ImageIndex = 5;
                        tbNode.SelectedImageIndex = 5;
                        tbNode.Tag = "table";
                        tabNode.Nodes.Add(tbNode);

                        //���ֶ���Ϣ
                        List<ColumnInfo> collist = dbobj2.GetColumnList(dbname, tabname);
                        if ((collist != null) && (collist.Count > 0))
                        {
                            foreach (ColumnInfo col in collist)
                            {
                                string columnName = col.ColumnName;
                                string columnType = col.TypeName;
                                TreeNode colNode = new TreeNode(columnName + "[" + columnType + "]");
                                colNode.ImageIndex = 7;
                                colNode.SelectedImageIndex = 7;
                                colNode.Tag = "column";
                                tbNode.Nodes.Add(colNode);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogInfo.WriteLog(ex);
                MessageBox.Show(this, "��ȡ���ݿ�" + dbname + "�ı���Ϣʧ�ܣ�" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion

            #region	��ͼ

            try
            {
                DataTable dtv = dbobj2.GetVIEWs(dbname);
                if (dtv != null)
                {
                    DataRow[] dRows = dtv.Select("", "name ASC");
                    foreach (DataRow row in dRows)//ѭ��ÿ����
                    {
                        string tabname = row["name"].ToString();
                        TreeNode tbNode = new TreeNode(tabname);
                        tbNode.ImageIndex = 6;
                        tbNode.SelectedImageIndex = 6;
                        tbNode.Tag = "view";
                        viewNode.Nodes.Add(tbNode);

                        //���ֶ���Ϣ
                        List<ColumnInfo> collist = dbobj2.GetColumnList(dbname, tabname);
                        if ((collist != null) && (collist.Count > 0))
                        {
                            foreach (ColumnInfo col in collist)
                            {
                                string columnName = col.ColumnName;
                                string columnType = col.TypeName;
                                TreeNode colNode = new TreeNode(columnName + "[" + columnType + "]");
                                colNode.ImageIndex = 7;
                                colNode.SelectedImageIndex = 7;
                                colNode.Tag = "column";
                                tbNode.Nodes.Add(colNode);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogInfo.WriteLog(ex);
                MessageBox.Show(this, "��ȡ���ݿ�" + dbname + "����ͼ��Ϣʧ�ܣ�" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion

            #region �洢����
            try
            {
                DataTable dtp = dbobj2.GetProcs(dbname);
                if (dtp != null)
                {
                    DataRow[] dRows = dtp.Select("", "name ASC");
                    foreach (DataRow row in dRows)//ѭ��ÿ����
                    {
                        string tabname = row["name"].ToString();
                        TreeNode tbNode = new TreeNode(tabname);
                        tbNode.ImageIndex = 8;
                        tbNode.SelectedImageIndex = 8;
                        tbNode.Tag = "proc";
                        procNode.Nodes.Add(tbNode);

                        //���ֶβ�����Ϣ
                        List<ColumnInfo> collist = dbobj2.GetColumnList(dbname, tabname);
                        if ((collist != null) && (collist.Count > 0))
                        {
                            foreach (ColumnInfo col in collist)
                            {
                                string columnName = col.ColumnName;
                                string columnType = col.TypeName;
                                TreeNode colNode = new TreeNode(columnName + "[" + columnType + "]");
                                colNode.ImageIndex = 9;
                                colNode.SelectedImageIndex = 9;
                                colNode.Tag = "column";
                                tbNode.Nodes.Add(colNode);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogInfo.WriteLog(ex);
                MessageBox.Show(this, "��ȡ���ݿ�" + dbname + "����ͼ��Ϣʧ�ܣ�" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion

            mainfrm.StatusLabel1.Text = "����";

        }
        #endregion

        #region table_click

        private void SELECTItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode != null)
            {
                string longservername = TreeClickNode.Parent.Parent.Parent.Text;
                string dbname = TreeClickNode.Parent.Parent.Text;
                string tabname = TreeClickNode.Text;
                LTP.IDBO.IDbScriptBuilder dsb = ObjHelper.CreatDsb(longservername);
                string strSQL = dsb.GetSQLSelect(dbname, tabname);
                string title = tabname + "��ѯ.sql";

                AddTabPage(title, new DbQuery(mainfrm, strSQL));
            }
        }
        private void UPDATEItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode != null)
            {

                string longservername = TreeClickNode.Parent.Parent.Parent.Text;
                string dbname = TreeClickNode.Parent.Parent.Text;
                string tabname = TreeClickNode.Text;
                LTP.IDBO.IDbScriptBuilder dsb = ObjHelper.CreatDsb(longservername);
                string strSQL = dsb.GetSQLUpdate(dbname, tabname);
                string title = tabname + "��ѯ.sql";
                //MainForm frm = (MainForm)MdiParentForm;
                AddTabPage(title, new DbQuery(mainfrm, strSQL));
            }

        }
        private void DELETEItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode != null)
            {
                string longservername = TreeClickNode.Parent.Parent.Parent.Text;
                string dbname = TreeClickNode.Parent.Parent.Text;
                string tabname = TreeClickNode.Text;
                LTP.IDBO.IDbScriptBuilder dsb = ObjHelper.CreatDsb(longservername);
                string strSQL = dsb.GetSQLDelete(dbname, tabname);
                string title = tabname + "��ѯ.sql";
                //MainForm frm = (MainForm)MdiParentForm;
                AddTabPage(title, new DbQuery(mainfrm, strSQL));
            }
        }
        private void INSERTItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode != null)
            {
                string longservername = TreeClickNode.Parent.Parent.Parent.Text;
                string dbname = TreeClickNode.Parent.Parent.Text;
                string tabname = TreeClickNode.Text;
                LTP.IDBO.IDbScriptBuilder dsb = ObjHelper.CreatDsb(longservername);
                string strSQL = dsb.GetSQLInsert(dbname, tabname);
                string title = tabname + "��ѯ.sql";
                AddTabPage(title, new DbQuery(mainfrm, strSQL));
            }

        }

        private void �鿴������tabItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode != null)
            {
                string longservername = TreeClickNode.Parent.Parent.Parent.Text;
                string dbname = TreeClickNode.Parent.Parent.Text;
                string tabname = TreeClickNode.Text;
                LTP.IDBO.IDbObject dbobj = ObjHelper.CreatDbObj(longservername);
                DataList dl = new DataList(dbobj, dbname, tabname);
                AddTabPage(tabname, dl, mainfrm);
            }
        }

        private void ���ɴ洢����tabItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode != null)
            {
                string longservername = TreeClickNode.Parent.Parent.Parent.Text;
                string dbname = TreeClickNode.Parent.Parent.Text;
                string tabname = TreeClickNode.Text;
                LTP.IDBO.IDbScriptBuilder dsb = ObjHelper.CreatDsb(longservername);

                dsb.DbName = dbname;
                dsb.TableName = tabname;
                dsb.ProjectName = setting.ProjectName;
                dsb.ProcPrefix = setting.ProcPrefix;
                dsb.Keys = new List<LTP.CodeHelper.ColumnInfo>();
                dsb.Fieldlist = new List<LTP.CodeHelper.ColumnInfo>();

                string strSQL = dsb.GetPROCCode(dbname, tabname);
                string title = tabname + "�洢����.sql";
                AddTabPage(title, new DbQuery(mainfrm, strSQL));
            }

        }
        private void �������ݽű�tabItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(this, "����ñ��������ϴ�ֱ�����ɽ���Ҫ�Ƚϳ���ʱ�䣬\r\nȷʵ��Ҫֱ��������\r\n(������ýű����������ɡ�)", "��ʾ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                if (TreeClickNode != null)
                {
                    string longservername = TreeClickNode.Parent.Parent.Parent.Text;
                    string dbname = TreeClickNode.Parent.Parent.Text;
                    string tabname = TreeClickNode.Text;
                    LTP.IDBO.IDbScriptBuilder dsb = ObjHelper.CreatDsb(longservername);
                    dsb.Fieldlist = new List<LTP.CodeHelper.ColumnInfo>();
                    string strSQL = dsb.CreateTabScript(dbname, tabname);
                    string title = tabname + "�ű�.sql";
                    AddTabPage(title, new DbQuery(mainfrm, strSQL));
                }
            }
            if (dr == DialogResult.No)
            {
                if (TreeClickNode != null)
                {
                    string longservername = TreeClickNode.Parent.Parent.Parent.Text;
                    string dbname = TreeClickNode.Parent.Parent.Text;
                    string tabname = TreeClickNode.Text;
                    DbToScript dts = new DbToScript(longservername, dbname);
                    dts.ShowDialog(this);
                }
            }


        }

        //�����ļ�
        private void �洢����tabItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sqlsavedlg = new SaveFileDialog();
            sqlsavedlg.Title = "���浱ǰ�ű�";
            sqlsavedlg.Filter = "sql files (*.sql)|*.sql|All files (*.*)|*.*";
            DialogResult dlgresult = sqlsavedlg.ShowDialog(this);
            if (dlgresult == DialogResult.OK)
            {
                if (TreeClickNode == null)
                {
                    return;
                }
                string longservername = TreeClickNode.Parent.Parent.Parent.Text;
                string dbname = TreeClickNode.Parent.Parent.Text;
                string tabname = TreeClickNode.Text;
                LTP.IDBO.IDbScriptBuilder dsb = ObjHelper.CreatDsb(longservername);
                dsb.DbName = dbname;
                dsb.TableName = tabname;
                dsb.ProjectName = setting.ProjectName;
                dsb.ProcPrefix = setting.ProcPrefix;
                dsb.Keys = new List<LTP.CodeHelper.ColumnInfo>();
                dsb.Fieldlist = new List<LTP.CodeHelper.ColumnInfo>();

                string strSQL = dsb.GetPROCCode(dbname, tabname);

                string filename = sqlsavedlg.FileName;
                StreamWriter sw = new StreamWriter(filename, false, Encoding.Default);//,false);
                sw.Write(strSQL);
                sw.Flush();//�ӻ�����д����������ļ���
                sw.Close();
                MessageBox.Show("�ű����ɳɹ���", "���", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        //�����ļ�
        private void ���ݽű�tabItem_Click(object sender, EventArgs e)
        {
            string longservername = TreeClickNode.Parent.Parent.Parent.Text;
            string dbname = TreeClickNode.Parent.Parent.Text;
            DbToScript dts = new DbToScript(longservername, dbname);
            dts.ShowDialog(this);
        }
        //�����ļ�
        private void ������tabItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode == null)
            {
                return;
            }
            string longservername = TreeClickNode.Parent.Parent.Parent.Text;
            string dbname = TreeClickNode.Parent.Parent.Text;
            string tabname = TreeClickNode.Text;
            LTP.IDBO.IDbObject dbobj = ObjHelper.CreatDbObj(longservername);

        }


        //����������
        private void ��������Item_Click(object sender, EventArgs e)
        {
            string longservername = TreeClickNode.Parent.Parent.Parent.Text;
            if (longservername == "")
                return;
            mainfrm.AddSinglePage(new CodeMaker(), "����������");
        }

        private void ���ӱ��������Item_Click(object sender, EventArgs e)
        {
            if (TreeClickNode != null)
            {
                string longservername = TreeClickNode.Parent.Parent.Parent.Text;
                string dbname = TreeClickNode.Parent.Parent.Text;
                if (longservername == "")
                    return;
                mainfrm.AddSinglePage(new CodeMakerM(dbname), "���ӱ��������");
            }
        }

        private void ������������Item_Click(object sender, EventArgs e)
        {
            string longservername = TreeClickNode.Parent.Parent.Parent.Text;
            if (longservername == "")
                return;
            CodeExport ce = new CodeExport(longservername);
            ce.ShowDialog(this);
        }


        private void ģ���������Item_Click(object sender, EventArgs e)
        {
            string longservername = TreeClickNode.Parent.Parent.Parent.Text;
            if (longservername == "")
                return;
            mainfrm.AddSinglePage(new CodeTemplate(mainfrm), "ģ�����������");
        }

        private void ����ModelItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode == null)
            {
                return;
            }
            string longservername = TreeClickNode.Parent.Parent.Parent.Text;
            string dbname = TreeClickNode.Parent.Parent.Text;
            string tabname = TreeClickNode.Text;
            LTP.CodeBuild.CodeBuilders cb = ObjHelper.CreatCB(longservername);
            cb.DbName = dbname;
            cb.TableName = tabname;
            string strSQL = cb.GetCodeFrameS3Model();
            string title = tabname;
            AddTabPage(title, new DbQuery(mainfrm, strSQL));
        }

        private void ���ɵ���ṹItems_Click(object sender, EventArgs e)
        {
        }
        private void ����DALS3Item_Click(object sender, EventArgs e)
        {
        }
        private void ����BLLS3Item_Click(object sender, EventArgs e)
        {
        }

        private void ����ȫ��S3Item_Click(object sender, EventArgs e)
        {
        }
        private void ����DALF3Item_Click(object sender, EventArgs e)
        {
        }
        private void ����IDALItem_Click(object sender, EventArgs e)
        {
        }
        private void ����DALFactoryItem_Click(object sender, EventArgs e)
        {
        }

        private void ����BLLF3Item_Click(object sender, EventArgs e)
        {
        }
        private void ����ȫ��F3Item_Click(object sender, EventArgs e)
        {
        }
        private void ����ҳ��Item_Click(object sender, EventArgs e)
        {
        }

        
        private void ������tabItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode == null)
            {
                return;
            }
            string longservername = TreeClickNode.Parent.Parent.Parent.Text;
            string dbname = TreeClickNode.Parent.Parent.Text;
            string tabname = TreeClickNode.Text;
            LTP.IDBO.IDbObject dbobj = ObjHelper.CreatDbObj(longservername);

            RenameFrm rnfrm = new RenameFrm();
            rnfrm.txtName.Text = tabname;
            DialogResult result = rnfrm.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                string newName = rnfrm.txtName.Text.Trim();
                bool succ = dbobj.RenameTable(dbname, tabname, newName);
                if (succ)
                {
                    TreeClickNode.Text = newName;
                    MessageBox.Show(this, "�����޸ĳɹ���", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, "�����޸�ʧ�ܣ����Ժ����ԣ�", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void ɾ��tabItem_Click(object sender, EventArgs e)
        {
            if (TreeClickNode == null)
            {
                return;
            }
            string longservername = TreeClickNode.Parent.Parent.Parent.Text;
            string dbname = TreeClickNode.Parent.Parent.Text;
            string tabname = TreeClickNode.Text;
            LTP.IDBO.IDbObject dbobj = ObjHelper.CreatDbObj(longservername);

            DialogResult result = MessageBox.Show(this, "��ȷ��Ҫɾ���ı���", "ϵͳ��ʾ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                bool succ = dbobj.DeleteTable(dbname, tabname);
                if (succ)
                {
                    TreeClickNode.Remove();
                    MessageBox.Show(this, "��" + tabname + "ɾ���ɹ���", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, "��" + tabname + "ɾ��ʧ�ܣ����Ժ����ԣ�", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }

        #endregion

        #region view_click

        #endregion

        #region proc_click

        private void ������Item_Click(object sender, EventArgs e)
        {
            if (TreeClickNode == null)
            {
                return;
            }
            string longservername = TreeClickNode.Parent.Parent.Parent.Text;
            string dbname = TreeClickNode.Parent.Parent.Text;
            string name = TreeClickNode.Text;
            LTP.IDBO.IDbObject dbobj = ObjHelper.CreatDbObj(longservername);
            string str = dbobj.GetObjectInfo(dbname, name);
            string title = name + "����.sql";
            AddTabPage(title, new DbQuery(mainfrm, str));
        }

        #endregion

        #endregion


        #region ע�������RegServer
        
        public void RegServer(object sender, DoWorkEventArgs e)
        {
            try
            {
                Application.DoEvents();
                string dbtype = "SQL2005";                
                DialogResult dbselresult = dbsel.ShowDialog(this);
                if (dbselresult == DialogResult.OK)
                {
                    dbtype = dbsel.dbtype;
                    switch (dbtype)
                    {
                        case "SQL2000":
                        case "SQL2005":
                            LoginServer(e);
                            break;
                        case "Oracle":
                            LoginServerOra(e);
                            break;
                        case "OleDb":
                            LoginServerOledb(e);
                            break;
                        case "MySQL":
                            LoginServerMySQL(e);
                            break;
                        case "SQLite":
                            LoginServerSQLite(e);
                            break;
                        default:
                            LoginServer(e);
                            break;
                    }
                    if (serverlistNode != null)
                    {
                        serverlistNode.Expand();
                    }
                }
                                
            }
            catch(System.Exception ex)
            {
                MessageBox.Show("���ӷ�����ʧ�ܣ���رպ����´����ԡ�\r\n"+ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            e.Result = -1;

        }
        #endregion

        #region ��¼������LoginServer

        #region SQL
        private void LoginServer(DoWorkEventArgs e)
        {
            mainfrm = (MainForm)Application.OpenForms["MainForm"];
            DialogResult result = logo.ShowDialog(this);
            if (result == DialogResult.OK)
            {                
                Application.DoEvents();
                                
                string ServerIp = logo.comboBoxServer.Text;
                string dbname = logo.dbname;
                string constr = logo.constr;
                string dbtype = logo.GetSelVer();

                try
                {                    
                    mainfrm.StatusLabel1.Text = "������֤�����ӷ�����.....";
                    CreatTree(dbtype, ServerIp, constr, dbname,e);                 

                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show(this, ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //LTP.SplashScrForm.SplashScreen.SetStatus("����ϵͳģ��...");

            }

        }
        #endregion

        #region Oracle
        private void LoginServerOra(DoWorkEventArgs e)
        {
            mainfrm = (MainForm)Application.OpenForms["MainForm"];
            DialogResult result = logoOra.ShowDialog(this);
            if (result == DialogResult.OK)
            {                
                Application.DoEvents();             
                string ServerIp = logoOra.txtServer.Text;
                string constr = logoOra.constr;
                string dbname = logoOra.dbname;
                try
                {                   
                    mainfrm.StatusLabel1.Text = "������֤�����ӷ�����.....";
                    CreatTree("Oracle", ServerIp, constr, dbname,e);
                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show(this, ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //LTP.SplashScrForm.SplashScreen.SetStatus("����ϵͳģ��...");
            }
        }

        #endregion

        #region Oledb

        private void LoginServerOledb(DoWorkEventArgs e)
        {
            DialogResult result = logoOledb.ShowDialog(this);

            switch (result)
            {
                case DialogResult.OK:
                    {                        
                        Application.DoEvents();
                                             
                        string ServerIp = logoOledb.txtServer.Text;
                        string user = logoOledb.txtUser.Text;
                        string pass = logoOledb.txtPass.Text;
                        string constr = logoOledb.txtConstr.Text;

                        //string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ServerIp + ";Persist Security Info=False";

                        GetConstr(ServerIp, constr);
                        try
                        {                            
                            mainfrm.StatusLabel1.Text = "������֤�����ӷ�����.....";
                            CreatTree("OleDb", ServerIp, constr, "",e);                         
                        }
                        catch (System.Exception ex)
                        {
                            LogInfo.WriteLog(ex);
                            MessageBox.Show(this, ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //isSuccess = false;
                            break;
                        }
                    }
                    //LTP.SplashScrForm.SplashScreen.SetStatus("����ϵͳģ��...");

                    break;
                case DialogResult.Cancel:
                    break;
            }

        }

        private string GetConstr(string ServerIp, string txtConstr)
        {
            string constr = "";
            if (ServerIp != "")
            {
                constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ServerIp + ";Persist Security Info=False";
                if (constr.ToLower().IndexOf(".mdb") > 0) 
                {
                    isMdb = true;
                }
                else
                    if(constr.ToLower().IndexOf(".accdb") > 0)
                    {
                        isMdb = true;
                        constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ServerIp + ";Persist Security Info=False";
                    }                   
                else
                {
                    isMdb = false;
                }
            }
            else
            {
                constr = txtConstr;
                if ((constr.ToLower().IndexOf(".mdb") > 0) || (constr.ToLower().IndexOf(".accdb") > 0))
                {
                    isMdb = true;
                }
                else
                {
                    isMdb = false;
                }

            }
            return constr;
        }

        #endregion

        #region MySQL
        private void LoginServerMySQL(DoWorkEventArgs e)
        {
            mainfrm = (MainForm)Application.OpenForms["MainForm"];
            DialogResult result = loginMysql.ShowDialog(this);
            if (result == DialogResult.OK)
            {                
                Application.DoEvents();
             
                string ServerIp = loginMysql.comboBoxServer.Text;
                string constr = loginMysql.constr;
                string dbname = loginMysql.dbname;
                try
                {                    
                    mainfrm.StatusLabel1.Text = "������֤�����ӷ�����.....";
                    CreatTree("MySQL", ServerIp, constr, dbname,e);
                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show(this, ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //LTP.SplashScrForm.SplashScreen.SetStatus("����ϵͳģ��...");
            }

        }
        #endregion

        #region SQLite
        private void LoginServerSQLite(DoWorkEventArgs e)
        {
            mainfrm = (MainForm)Application.OpenForms["MainForm"];
            DialogResult result = loginSQLite.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                Application.DoEvents();

                string ServerIp = loginSQLite.txtServer.Text;
                string pass = loginSQLite.txtPass.Text;
                string constr = loginSQLite.txtConstr.Text;
                constr = "Data Source=" + ServerIp;
                if (pass != "")
                {
                    constr += ";Password=" + pass;
                }
                try
                {
                    mainfrm.StatusLabel1.Text = "������֤�����ӷ�����.....";
                    CreatTree("SQLite", ServerIp, constr, "", e);
                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show(this, ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //LTP.SplashScrForm.SplashScreen.SetStatus("����ϵͳģ��...");
            }

        }
        #endregion

        #endregion

        #region ��������������ڵ� CreatTree

        private void CreatTree(string dbtype, string ServerIp, string constr, string Dbname, DoWorkEventArgs e)
        {
            dbobj = LTP.DBFactory.DBOMaker.CreateDbObj(dbtype);

            TreeNode serverNode = new TreeNode(GetserverNodeText(ServerIp, dbtype, Dbname));
            serverNode.Tag = "server";            
            AddTreeNode(serverlistNode, serverNode);

            serverNode.ImageIndex = 1;
            serverNode.SelectedImageIndex = 1;

            //0 serverlist
            //1 server
            //2 db
            //3 folderclose
            //4 folderopen
            //5 table
            //6 view
            //7 fild
            this.treeView1.SelectedNode = serverNode;
            mainfrm.StatusLabel1.Text = "�������ݿ���...";
            Application.DoEvents();
            
            dbobj.DbConnectStr = constr;

            #region SQLSERVER ���ݿ���Ϣ
            if ((dbtype == "SQL2000") || (dbtype == "SQL2005"))
            {
                try
                {
                    if ((logo.dbname == "master") || (logo.dbname == ""))
                    {
                        List<string> dblist = dbobj.GetDBList();
                        if (dblist != null)
                        {
                            if (dblist.Count > 0)
                            {
                                mainfrm.toolComboBox_DB.Items.Clear();
                                foreach (string dbname in dblist)
                                {
                                    TreeNode dbNode = new TreeNode(dbname);
                                    dbNode.ImageIndex = 2;
                                    dbNode.SelectedImageIndex = 2;
                                    dbNode.Tag = "db";                                    
                                    AddTreeNode(serverNode, dbNode);
                                    mainfrm.toolComboBox_DB.Items.Add(dbname);
                                }
                                if (mainfrm.toolComboBox_DB.Items.Count > 0)
                                {
                                    mainfrm.toolComboBox_DB.SelectedIndex = 0;
                                }
                            }
                        }

                    }
                    else
                    {
                        string dbname = logo.dbname;
                        TreeNode dbNode = new TreeNode(dbname);
                        dbNode.ImageIndex = 2;
                        dbNode.SelectedImageIndex = 2;
                        dbNode.Tag = "db";                        
                        AddTreeNode(serverNode, dbNode);
                        mainfrm.toolComboBox_DB.Items.Clear();
                        mainfrm.toolComboBox_DB.Items.Add(dbname);

                        //������2
                        DataTable dto = dbobj.GetTabViews(dbname);
                        if (dto != null)
                        {
                            mainfrm.toolComboBox_Table.Items.Clear();
                            foreach (DataRow row in dto.Rows)//ѭ��ÿ����
                            {
                                string tabname = row["name"].ToString();
                                mainfrm.toolComboBox_Table.Items.Add(dbname);
                            }
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    throw new Exception("��ȡ���ݿ�ʧ�ܣ�" + ex.Message);
                }
            }

            #endregion

            #region Oracle ���ݿⵥ������
            if (dbtype == "Oracle")
            {
                string dbname = ServerIp;
                TreeNode dbNode = new TreeNode(dbname);
                dbNode.ImageIndex = 2;
                dbNode.SelectedImageIndex = 2;
                dbNode.Tag = "db";                
                AddTreeNode(serverNode, dbNode);
                mainfrm.toolComboBox_DB.Items.Add(dbname);

                //������2
                DataTable dto = dbobj.GetTabViews(dbname);
                if (dto != null)
                {
                    mainfrm.toolComboBox_Table.Items.Clear();
                    foreach (DataRow row in dto.Rows)//ѭ��ÿ����
                    {
                        string tabname = row["name"].ToString();
                        mainfrm.toolComboBox_Table.Items.Add(dbname);
                    }
                    if (mainfrm.toolComboBox_Table.Items.Count > 0)
                    {
                        mainfrm.toolComboBox_Table.SelectedIndex = 0;
                    }
                }

            }
            #endregion

            #region MySQL ���ݿⵥ������
            if (dbtype == "MySQL")
            {
                try
                {
                    if ((this.loginMysql.dbname == "mysql") || (loginMysql.dbname == ""))
                    {
                        List<string> dblist = dbobj.GetDBList();
                        if (dblist != null)
                        {
                            if (dblist.Count > 0)
                            {
                                mainfrm.toolComboBox_DB.Items.Clear();
                                foreach (string dbname in dblist)
                                {
                                    TreeNode dbNode = new TreeNode(dbname);
                                    dbNode.ImageIndex = 2;
                                    dbNode.SelectedImageIndex = 2;
                                    dbNode.Tag = "db";                                    
                                    AddTreeNode(serverNode, dbNode);
                                    mainfrm.toolComboBox_DB.Items.Add(dbname);
                                }
                                if (mainfrm.toolComboBox_DB.Items.Count > 0)
                                {
                                    mainfrm.toolComboBox_DB.SelectedIndex = 0;
                                }
                            }
                        }

                    }
                    else
                    {
                        string dbname = loginMysql.dbname;
                        TreeNode dbNode = new TreeNode(dbname);
                        dbNode.ImageIndex = 2;
                        dbNode.SelectedImageIndex = 2;
                        dbNode.Tag = "db";                        
                        AddTreeNode(serverNode, dbNode);
                        mainfrm.toolComboBox_DB.Items.Clear();
                        mainfrm.toolComboBox_DB.Items.Add(dbname);

                        //������2
                        DataTable dto = dbobj.GetTabViews(dbname);
                        if (dto != null)
                        {
                            mainfrm.toolComboBox_Table.Items.Clear();
                            foreach (DataRow row in dto.Rows)//ѭ��ÿ����
                            {
                                string tabname = row["name"].ToString();
                                mainfrm.toolComboBox_Table.Items.Add(dbname);
                            }
                            if (mainfrm.toolComboBox_Table.Items.Count > 0)
                            {
                                mainfrm.toolComboBox_Table.SelectedIndex = 0;
                            }
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    throw new Exception("��ȡ���ݿ�ʧ�ܣ�" + ex.Message);
                }

            }
            #endregion

            #region OleDb ���ݿⵥ������
            if (dbtype == "OleDb")
            {
                string dbname = ServerIp.Substring(ServerIp.LastIndexOf("\\") + 1);
                TreeNode dbNode = new TreeNode(dbname);
                dbNode.ImageIndex = 2;
                dbNode.SelectedImageIndex = 2;
                dbNode.Tag = "db";                
                AddTreeNode(serverNode, dbNode);
                mainfrm.toolComboBox_DB.Items.Add(dbname);

                //������2
                DataTable dto = dbobj.GetTabViews(dbname);
                if (dto != null)
                {
                    mainfrm.toolComboBox_Table.Items.Clear();
                    foreach (DataRow row in dto.Rows)//ѭ��ÿ����
                    {
                        string tabname = row["name"].ToString();
                        mainfrm.toolComboBox_Table.Items.Add(dbname);
                    }
                    if (mainfrm.toolComboBox_Table.Items.Count > 0)
                    {
                        mainfrm.toolComboBox_Table.SelectedIndex = 0;
                    }
                }

            }
            #endregion

            #region SQLite ���ݿⵥ������
            if (dbtype == "SQLite")
            {
                string dbname = ServerIp.Substring(ServerIp.LastIndexOf("\\") + 1);
                TreeNode dbNode = new TreeNode(dbname);
                dbNode.ImageIndex = 2;
                dbNode.SelectedImageIndex = 2;
                dbNode.Tag = "db";
                AddTreeNode(serverNode, dbNode);
                mainfrm.toolComboBox_DB.Items.Add(dbname);

                //������2
                DataTable dto = dbobj.GetTabViews(dbname);
                if (dto != null)
                {
                    mainfrm.toolComboBox_Table.Items.Clear();
                    foreach (DataRow row in dto.Rows)//ѭ��ÿ����
                    {
                        string tabname = row["name"].ToString();
                        mainfrm.toolComboBox_Table.Items.Add(dbname);
                    }
                    if (mainfrm.toolComboBox_Table.Items.Count > 0)
                    {
                        mainfrm.toolComboBox_Table.SelectedIndex = 0;
                    }
                }

            }
            #endregion

            serverNode.ExpandAll();

            #region  ѭ�����ݿ⣬��������Ϣ
            
            foreach (TreeNode tn in serverNode.Nodes)
            {
                string dbname = tn.Text;

                mainfrm.StatusLabel1.Text = "�������ݿ� " + dbname + "...";
                SetTreeNodeFont(tn, new Font("����", 9, FontStyle.Bold));
                TreeNode tabNode = new TreeNode("��");
                tabNode.ImageIndex = 3;
                tabNode.SelectedImageIndex = 4;
                tabNode.Tag = "tableroot";                
                AddTreeNode(tn, tabNode);

                TreeNode viewNode = new TreeNode("��ͼ");
                viewNode.ImageIndex = 3;
                viewNode.SelectedImageIndex = 4;
                viewNode.Tag = "viewroot";                
                AddTreeNode(tn, viewNode);

                TreeNode procNode = new TreeNode("�洢����");
                procNode.ImageIndex = 3;
                procNode.SelectedImageIndex = 4;
                procNode.Tag = "procroot";                
                AddTreeNode(tn, procNode);

                #region ��

                try
                {
                    List<string> tabNames = dbobj.GetTables(dbname);
                    if (tabNames.Count > 0)
                    {
                        int pi = 1;
                        foreach (string tabname in tabNames)//ѭ��ÿ����
                        {
                            if (backgroundWorkerReg.CancellationPending)
                            {
                                e.Cancel = true;
                            }
                            else
                            {
                                backgroundWorkerReg.ReportProgress(pi);
                            }
                            pi++;

                            mainfrm.StatusLabel1.Text = "�������ݿ� " + dbname + "�ı� " + tabname;
                            TreeNode tbNode = new TreeNode(tabname);
                            tbNode.ImageIndex = 5;
                            tbNode.SelectedImageIndex = 5;
                            tbNode.Tag = "table";                            
                            AddTreeNode(tabNode, tbNode);

                            #region  ���ֶ���Ϣ
                            if (!logo.chk_Simple.Checked)
                            {
                                List<ColumnInfo> collist = dbobj.GetColumnList(dbname, tabname);
                                if ((collist != null) && (collist.Count > 0))
                                {
                                    foreach (ColumnInfo col in collist)
                                    {
                                        string columnName = col.ColumnName;
                                        string columnType = col.TypeName;
                                        TreeNode colNode = new TreeNode(columnName + "[" + columnType + "]");
                                        colNode.ImageIndex = 7;
                                        colNode.SelectedImageIndex = 7;
                                        colNode.Tag = "column";                                        
                                        AddTreeNode(tbNode, colNode);
                                    }
                                }
                            }
                            #endregion

                        }
                    }

                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show(this, "��ȡ���ݿ�" + dbname + "�ı���Ϣʧ�ܣ�" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                #endregion

                #region	��ͼ

                try
                {
                    DataTable dtv = dbobj.GetVIEWs(dbname);
                    if (dtv != null)
                    {
                        DataRow[] dRows = dtv.Select("", "name ASC");
                        foreach (DataRow row in dRows)//ѭ��ÿ����
                        {
                            string tabname = row["name"].ToString();                            
                            mainfrm.StatusLabel1.Text = "�������ݿ� " + dbname + "����ͼ " + tabname;
                            TreeNode tbNode = new TreeNode(tabname);
                            tbNode.ImageIndex = 6;
                            tbNode.SelectedImageIndex = 6;
                            tbNode.Tag = "view";                            
                            AddTreeNode(viewNode, tbNode);

                            #region  ���ֶ���Ϣ
                            if (!logo.chk_Simple.Checked)
                            {
                                List<ColumnInfo> collist = dbobj.GetColumnList(dbname, tabname);
                                if ((collist != null) && (collist.Count > 0))
                                {
                                    foreach (ColumnInfo col in collist)
                                    {
                                        string columnName = col.ColumnName;
                                        string columnType = col.TypeName;
                                        TreeNode colNode = new TreeNode(columnName + "[" + columnType + "]");
                                        colNode.ImageIndex = 7;
                                        colNode.SelectedImageIndex = 7;
                                        colNode.Tag = "column";
                                        //tbNode.Nodes.Add(colNode);
                                        AddTreeNode(tbNode, colNode);
                                    }
                                }
                            }
                            #endregion

                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show(this, "��ȡ���ݿ�" + dbname + "����ͼ��Ϣʧ�ܣ�" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                #endregion

                #region �洢����
                try
                {
                    DataTable dtp = dbobj.GetProcs(dbname);
                    if (dtp != null)
                    {
                        DataRow[] dRows = dtp.Select("", "name ASC");
                        foreach (DataRow row in dRows)//ѭ��ÿ����
                        {
                            string tabname = row["name"].ToString();                            
                            mainfrm.StatusLabel1.Text = "�������ݿ� " + dbname + "�Ĵ洢���� " + tabname;
                            TreeNode tbNode = new TreeNode(tabname);
                            tbNode.ImageIndex = 8;
                            tbNode.SelectedImageIndex = 8;
                            tbNode.Tag = "proc";                            
                            AddTreeNode(procNode, tbNode);

                            #region  ���ֶ���Ϣ
                            if (!logo.chk_Simple.Checked)
                            {
                                List<ColumnInfo> collist = dbobj.GetColumnList(dbname, tabname);
                                if ((collist != null) && (collist.Count > 0))
                                {
                                    foreach (ColumnInfo col in collist)
                                    {
                                        string columnName = col.ColumnName;
                                        string columnType = col.TypeName;

                                        TreeNode colNode = new TreeNode(columnName + "[" + columnType + "]");
                                        colNode.ImageIndex = 9;
                                        colNode.SelectedImageIndex = 9;
                                        colNode.Tag = "column";                                        
                                        AddTreeNode(tbNode, colNode);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show(this, "��ȡ���ݿ�" + dbname + "����ͼ��Ϣʧ�ܣ�" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                #endregion

                SetTreeNodeFont(tn, new Font("����", 9, FontStyle.Regular));

            }
            #endregion

            #region ѡ�и��ڵ�
            foreach (TreeNode node in this.treeView1.Nodes)
            {
                if (node.Text == ServerIp)
                {
                    this.treeView1.SelectedNode = node;
                    //node.BackColor=Color.FromArgb(10,36,106);
                    //node.ForeColor=Color.White;
                }
            }
            #endregion

        }

        #endregion

        #region ���ӷ�����ConnectServer

        public void DoConnect(object sender, DoWorkEventArgs e)
        {
            if (TreeClickNode != null)
            {
                string nodetext = TreeClickNode.Text;
                LTP.CmConfig.DbSettings dbset = LTP.CmConfig.DbConfig.GetSetting(nodetext);
                if (dbset == null)
                {
                    MessageBox.Show("�÷�������Ϣ�Ѿ������ڣ���ر����Ȼ�����ԡ�" , "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string server = dbset.Server;
                string dbtype = dbset.DbType;
                string dbname = dbset.DbName;
                bool connectSimple = dbset.ConnectSimple;

                try
                {                    
                    ConnectServer(TreeClickNode, dbtype, server, dbname, connectSimple,e);
                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show("�������ݿ�ʧ�ܣ���رպ����´����ԡ�\r\n"+ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            e.Result = -1;
        }

        public void ProgessChangedCon(object sender, ProgressChangedEventArgs e)
        {
            //this.progressBar1.Maximum = 1000;
            //this.progressBar1.Value = e.ProgressPercentage;
        }
        public void CompleteWorkCon(object sender, RunWorkerCompletedEventArgs e)
        {
            if (mainfrm != null)
            {
                mainfrm.StatusLabel1.Text = "���";
            }
        }

        public void ProgessChangedReg(object sender, ProgressChangedEventArgs e)
        {
            //this.progressBar1.Maximum = 1000;
            //this.progressBar1.Value = e.ProgressPercentage;
        }
        public void CompleteWorkReg(object sender, RunWorkerCompletedEventArgs e)
        {
            if (mainfrm != null)
            {
                mainfrm.StatusLabel1.Text = "���";
            }
        }
        
        //���ݷ������ڵ㴴�������ͱ�ڵ�
        private void ConnectServer(TreeNode serverNode, string dbtype, string ServerIp, string DbName, bool ConnectSimple, DoWorkEventArgs e)
        {
            LTP.IDBO.IDbObject dbobj2 = LTP.DBFactory.DBOMaker.CreateDbObj(dbtype);

            mainfrm.StatusLabel1.Text = "�������ݿ���...";            
            Application.DoEvents();
            
            LTP.CmConfig.DbSettings dbset = LTP.CmConfig.DbConfig.GetSetting(dbtype, ServerIp, DbName);
            dbobj2.DbConnectStr = dbset.ConnectStr;

            serverNode.Nodes.Clear();

            #region SQLSERVER ���ݿ���Ϣ
            if ((dbtype == "SQL2000") || (dbtype == "SQL2005"))
            {
                try
                {
                    if ((dbset.DbName == "master") || (dbset.DbName == ""))
                    {
                        List<string> dblist = dbobj2.GetDBList();
                        if (dblist.Count > 0)
                        {
                            mainfrm.toolComboBox_DB.Items.Clear();
                            foreach (string dbname in dblist)
                            {
                                TreeNode dbNode = new TreeNode(dbname);
                                dbNode.ImageIndex = 2;
                                dbNode.SelectedImageIndex = 2;
                                dbNode.Tag = "db";                                
                                AddTreeNode(serverNode, dbNode);
                                mainfrm.toolComboBox_DB.Items.Add(dbname);
                            }
                            if (mainfrm.toolComboBox_DB.Items.Count > 0)
                            {
                                mainfrm.toolComboBox_DB.SelectedIndex = 0;
                            }
                        }
                    }
                    else
                    {
                        TreeNode dbNode = new TreeNode(dbset.DbName);
                        dbNode.ImageIndex = 2;
                        dbNode.SelectedImageIndex = 2;
                        dbNode.Tag = "db";                        
                        AddTreeNode(serverNode, dbNode);
                        mainfrm.toolComboBox_DB.Items.Clear();
                        mainfrm.toolComboBox_DB.Items.Add(dbset.DbName);

                        //������2
                        DataTable dto = dbobj2.GetTabViews(dbset.DbName);
                        if (dto != null)
                        {
                            mainfrm.toolComboBox_Table.Items.Clear();
                            foreach (DataRow row in dto.Rows)//ѭ��ÿ����
                            {
                                string tabname = row["name"].ToString();
                                mainfrm.toolComboBox_Table.Items.Add(tabname);
                            }
                            if (mainfrm.toolComboBox_Table.Items.Count > 0)
                            {
                                mainfrm.toolComboBox_Table.SelectedIndex = 0;
                            }
                        }
                    }

                }
                catch(System.Exception ex)
                {
                    // throw new Exception("��ȡ���ݿ�ʧ�ܣ�" + ex.Message);
                    LogInfo.WriteLog(ex);
                    MessageBox.Show(this, "���ӷ�����ʧ�ܣ�����������Ƿ��Ѿ���������������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            #endregion

            #region Oracle ���ݿⵥ������
            if (dbtype == "Oracle")
            {
                TreeNode dbNode = new TreeNode(ServerIp);
                dbNode.ImageIndex = 2;
                dbNode.SelectedImageIndex = 2;
                dbNode.Tag = "db";                
                AddTreeNode(serverNode, dbNode);
                mainfrm.toolComboBox_DB.Items.Add(ServerIp);
                mainfrm.toolComboBox_DB.SelectedIndex = 0;

                //������2
                DataTable dto = dbobj2.GetTabViews(ServerIp);
                if (dto != null)
                {
                    mainfrm.toolComboBox_Table.Items.Clear();
                    foreach (DataRow row in dto.Rows)//ѭ��ÿ����
                    {
                        string tabname = row["name"].ToString();
                        mainfrm.toolComboBox_Table.Items.Add(tabname);
                    }
                    if (mainfrm.toolComboBox_Table.Items.Count > 0)
                    {
                        mainfrm.toolComboBox_Table.SelectedIndex = 0;
                    }
                }

            }
            #endregion

            #region MySQL ���ݿⵥ������
            if (dbtype == "MySQL")
            {
                try
                {
                    string dbname = dbset.DbName;

                    if ((dbname == "mysql") || (dbname == ""))
                    {
                        List<string> dblist = dbobj2.GetDBList();
                        if (dblist.Count > 0)
                        {
                            mainfrm.toolComboBox_DB.Items.Clear();
                            foreach (string db in dblist)
                            {
                                TreeNode dbNode = new TreeNode(db);
                                dbNode.ImageIndex = 2;
                                dbNode.SelectedImageIndex = 2;
                                dbNode.Tag = "db";                                
                                AddTreeNode(serverNode, dbNode);
                                mainfrm.toolComboBox_DB.Items.Add(db);
                            }
                            if (mainfrm.toolComboBox_DB.Items.Count > 0)
                            {
                                mainfrm.toolComboBox_DB.SelectedIndex = 0;
                            }
                        }

                    }
                    else
                    {                        
                        TreeNode dbNode = new TreeNode(dbname);
                        dbNode.ImageIndex = 2;
                        dbNode.SelectedImageIndex = 2;
                        dbNode.Tag = "db";                        
                        AddTreeNode(serverNode, dbNode);
                        mainfrm.toolComboBox_DB.Items.Clear();
                        mainfrm.toolComboBox_DB.Items.Add(dbname);

                        //������2
                        DataTable dto = dbobj2.GetTabViews(dbname);
                        if (dto != null)
                        {
                            mainfrm.toolComboBox_Table.Items.Clear();
                            foreach (DataRow row in dto.Rows)//ѭ��ÿ����
                            {
                                string tabname = row["name"].ToString();
                                mainfrm.toolComboBox_Table.Items.Add(dbname);
                            }
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show(this, "���ӷ�����ʧ�ܣ�����������Ƿ��Ѿ���������������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            #endregion

            #region OleDb ���ݿⵥ������
            if (dbtype == "OleDb")
            {
                string dbname = ServerIp.Substring(ServerIp.LastIndexOf("\\") + 1);
                TreeNode dbNode = new TreeNode(dbname);
                dbNode.ImageIndex = 2;
                dbNode.SelectedImageIndex = 2;
                dbNode.Tag = "db";                
                AddTreeNode(serverNode, dbNode);
                mainfrm.toolComboBox_DB.Items.Add(dbname);
                mainfrm.toolComboBox_DB.SelectedIndex = 0;

                //������2
                DataTable dto = dbobj2.GetTabViews(dbname);
                if (dto != null)
                {
                    mainfrm.toolComboBox_Table.Items.Clear();
                    foreach (DataRow row in dto.Rows)//ѭ��ÿ����
                    {
                        string tabname = row["name"].ToString();
                        mainfrm.toolComboBox_Table.Items.Add(tabname);
                    }
                    if (mainfrm.toolComboBox_Table.Items.Count > 0)
                    {
                        mainfrm.toolComboBox_Table.SelectedIndex = 0;
                    }
                }

            }
            #endregion


            #region OleDb ���ݿⵥ������
            if (dbtype == "SQLite")
            {
                string dbname = ServerIp.Substring(ServerIp.LastIndexOf("\\") + 1);
                TreeNode dbNode = new TreeNode(dbname);
                dbNode.ImageIndex = 2;
                dbNode.SelectedImageIndex = 2;
                dbNode.Tag = "db";
                AddTreeNode(serverNode, dbNode);
                mainfrm.toolComboBox_DB.Items.Add(dbname);
                mainfrm.toolComboBox_DB.SelectedIndex = 0;

                //������2
                DataTable dto = dbobj2.GetTabViews(dbname);
                if (dto != null)
                {
                    mainfrm.toolComboBox_Table.Items.Clear();
                    foreach (DataRow row in dto.Rows)//ѭ��ÿ����
                    {
                        string tabname = row["name"].ToString();
                        mainfrm.toolComboBox_Table.Items.Add(tabname);
                    }
                    if (mainfrm.toolComboBox_Table.Items.Count > 0)
                    {
                        mainfrm.toolComboBox_Table.SelectedIndex = 0;
                    }
                }

            }
            #endregion


            serverNode.ExpandAll();

            #region ѭ�����ݿ⣬��������Ϣ
            foreach (TreeNode tn in serverNode.Nodes)
            {
                string dbname = tn.Text;
                mainfrm.StatusLabel1.Text = "�������ݿ� " + dbname + "...";
                SetTreeNodeFont(tn, new Font("����", 9, FontStyle.Bold));
                TreeNode tabNode = new TreeNode("��");
                tabNode.ImageIndex = 3;
                tabNode.SelectedImageIndex = 4;
                tabNode.Tag = "tableroot";                
                AddTreeNode(tn, tabNode);

                TreeNode viewNode = new TreeNode("��ͼ");
                viewNode.ImageIndex = 3;
                viewNode.SelectedImageIndex = 4;
                viewNode.Tag = "viewroot";                
                AddTreeNode(tn, viewNode);

                TreeNode procNode = new TreeNode("�洢����");
                procNode.ImageIndex = 3;
                procNode.SelectedImageIndex = 4;
                procNode.Tag = "procroot";                
                AddTreeNode(tn, procNode);

                #region ��

                try
                {
                    List<string> tabNames = dbobj2.GetTables(dbname);
                    if (tabNames.Count > 0)
                    {
                        int pi = 1;
                        foreach (string tabname in tabNames)//ѭ��ÿ����
                        {
                            if (backgroundWorkerCon.CancellationPending)
                            {
                                e.Cancel = true;
                            }
                            else
                            {
                                backgroundWorkerCon.ReportProgress(pi);
                            } 
                            pi++;

                            mainfrm.StatusLabel1.Text = "�������ݿ� " + dbname + "�ı� " + tabname;
                            TreeNode tbNode = new TreeNode(tabname);
                            tbNode.ImageIndex = 5;
                            tbNode.SelectedImageIndex = 5;
                            tbNode.Tag = "table";                           
                            AddTreeNode(tabNode, tbNode);

                            #region  ���ֶ���Ϣ
                            if (!ConnectSimple)
                            {
                                List<ColumnInfo> collist = dbobj2.GetColumnList(dbname, tabname);
                                if ((collist != null) && (collist.Count > 0))
                                {
                                    foreach (ColumnInfo col in collist)
                                    {
                                        string columnName = col.ColumnName;
                                        string columnType = col.TypeName;
                                        TreeNode colNode = new TreeNode(columnName + "[" + columnType + "]");
                                        colNode.ImageIndex = 7;
                                        colNode.SelectedImageIndex = 7;
                                        colNode.Tag = "column";                                        
                                        AddTreeNode(tbNode, colNode);
                                    }
                                }
                            }
                            #endregion

                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show(this, "��ȡ���ݿ�" + dbname + "�ı���Ϣʧ�ܣ�" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                #endregion

                #region	��ͼ

                try
                {
                    DataTable dtv = dbobj2.GetVIEWs(dbname);
                    if (dtv != null)
                    {
                        DataRow[] dRows = dtv.Select("", "name ASC");
                        foreach (DataRow row in dRows)//ѭ��ÿ����
                        {
                            string tabname = row["name"].ToString();
                            mainfrm.StatusLabel1.Text = "�������ݿ� " + dbname + "����ͼ " + tabname;                          
                            TreeNode tbNode = new TreeNode(tabname);
                            tbNode.ImageIndex = 6;
                            tbNode.SelectedImageIndex = 6;
                            tbNode.Tag = "view";
                            //viewNode.Nodes.Add(tbNode);
                            AddTreeNode(viewNode, tbNode);

                            #region  ���ֶ���Ϣ
                            if (!ConnectSimple)
                            {
                                List<ColumnInfo> collist = dbobj2.GetColumnList(dbname, tabname);
                                if ((collist != null) && (collist.Count > 0))
                                {
                                    foreach (ColumnInfo col in collist)//ѭ��ÿ����
                                    {
                                        string columnName = col.ColumnName;
                                        string columnType = col.TypeName;
                                        TreeNode colNode = new TreeNode(columnName + "[" + columnType + "]");
                                        colNode.ImageIndex = 7;
                                        colNode.SelectedImageIndex = 7;
                                        colNode.Tag = "column";
                                        //tbNode.Nodes.Add(colNode);
                                        AddTreeNode(tbNode, colNode);
                                    }
                                }
                            }
                            #endregion

                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show(this, "��ȡ���ݿ�" + dbname + "����ͼ��Ϣʧ�ܣ�" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                #endregion

                #region �洢����
                try
                {
                    DataTable dtp = dbobj2.GetProcs(dbname);
                    if (dtp != null)
                    {
                        DataRow[] dRows = dtp.Select("", "name ASC");
                        foreach (DataRow row in dRows)//ѭ��ÿ����
                        {
                            string tabname = row["name"].ToString();                            
                            mainfrm.StatusLabel1.Text = "�������ݿ� " + dbname + "�Ĵ洢���� " + tabname;    
                            TreeNode tbNode = new TreeNode(tabname);
                            tbNode.ImageIndex = 8;
                            tbNode.SelectedImageIndex = 8;
                            tbNode.Tag = "proc";                            
                            AddTreeNode(procNode, tbNode);

                            #region  ���ֶ���Ϣ
                            if (!ConnectSimple)
                            {
                                List<ColumnInfo> collist = dbobj2.GetColumnList(dbname, tabname);
                                if ((collist != null) && (collist.Count > 0))
                                {
                                    foreach (ColumnInfo col in collist)//ѭ��ÿ����
                                    {
                                        string columnName = col.ColumnName;
                                        string columnType = col.TypeName;
                                        TreeNode colNode = new TreeNode(columnName + "[" + columnType + "]");
                                        colNode.ImageIndex = 9;
                                        colNode.SelectedImageIndex = 9;
                                        colNode.Tag = "column";
                                        //tbNode.Nodes.Add(colNode);
                                        AddTreeNode(tbNode, colNode);
                                    }
                                }
                            }
                            #endregion

                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogInfo.WriteLog(ex);
                    MessageBox.Show(this, "��ȡ���ݿ�" + dbname + "����ͼ��Ϣʧ�ܣ�" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                #endregion

                SetTreeNodeFont(tn, new Font("����", 9, FontStyle.Regular));
            }
            #endregion
                        
            #region ѡ�и��ڵ�
            foreach (TreeNode node in serverlistNode.Nodes)
            {
                if (node.Text == ServerIp)
                {
                    this.treeView1.SelectedNode = node;
                    //node.BackColor=Color.FromArgb(10,36,106);
                    //node.ForeColor=Color.White;
                }
            }
            #endregion

            //mainfrm.StatusLabel1.Text = "����";
        }

        #endregion

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode node = (TreeNode)e.Item;          
            if ((node.Tag.ToString() == "table") || (node.Tag.ToString() == "view") || (node.Tag.ToString() == "column"))
            {
                DoDragDrop(node, System.Windows.Forms.DragDropEffects.Copy);
            }
        }

        private void DbView_Layout(object sender, LayoutEventArgs e)
        {
            if (m_bLayoutCalled == false)
            {
                m_bLayoutCalled = true;              
                //LTP.SplashScrForm.SplashScreen.CloseForm();
                this.Activate();
            }
        }



    }
}