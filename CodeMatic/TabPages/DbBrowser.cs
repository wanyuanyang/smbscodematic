using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LTP.CodeHelper;
namespace Codematic
{
    public partial class DbBrowser : Form
    {
        LTP.IDBO.IDbObject dbobj;
        public DbBrowser()
        {
            InitializeComponent();
            DbView dbviewfrm = (DbView)Application.OpenForms["DbView"];
            SetListView(dbviewfrm);
        }

        public void SetListView(DbView dbviewfrm)
        {
            #region �õ����Ͷ���
            TreeNode SelNode = dbviewfrm.treeView1.SelectedNode;
            if (SelNode == null)
                return;
            string servername = "";
            switch (SelNode.Tag.ToString())
            {
                case "serverlist":
                    return;
                case "server":
                    {
                        servername = SelNode.Text;
                        CreatDbObj(servername);
                        #region listView1

                        this.lblViewInfo.Text = " ��������" + servername;
                        this.lblNum.Text = SelNode.Nodes.Count.ToString() + "��";
                        this.listView1.Columns.Clear();
                        this.listView1.Items.Clear();
                        this.listView1.LargeImageList =this.imglistDB;
                        //this.listView1.SmallImageList = imglistView;
                        this.listView1.View = View.LargeIcon;
                        foreach (TreeNode node in SelNode.Nodes)
                        {
                            string dbname = node.Text;
                            ListViewItem item1 = new ListViewItem(dbname, 0);
                            item1.SubItems.Add(dbname);
                            item1.ImageIndex = 0;
                            listView1.Items.AddRange(new ListViewItem[] { item1 });
                        }
                        SetListViewMenu("db");
                        #endregion
                    }
                    break;
                case "db":
                    {
                        servername = SelNode.Parent.Text;
                        CreatDbObj(servername);
                        #region
                        this.lblViewInfo.Text = " ���ݿ⣺" + SelNode.Text;
                        this.lblNum.Text = SelNode.Nodes.Count.ToString() + "��";                        
                        SetListViewMenu("table");
                        BindlistViewTab(SelNode.Text, SelNode.Tag.ToString());
                        #endregion
                    }
                    break;
                case "tableroot":
                case "viewroot":
                case "procroot":
                    {
                        servername = SelNode.Parent.Parent.Text;
                        string dbname = SelNode.Parent.Text;
                        CreatDbObj(servername);

                        #region
                        this.lblViewInfo.Text = " ���ݿ⣺" + dbname;
                        this.lblNum.Text = SelNode.Nodes.Count.ToString() + "��";
                                                                
                        SetListViewMenu("table");
                        BindlistViewTab(dbname, SelNode.Tag.ToString());
                        #endregion
                    }
                    break;                    
                case "table":
                case "view":
                    {
                        servername = SelNode.Parent.Parent.Parent.Text;
                        string dbname = SelNode.Parent.Parent.Text;
                        string tabname = SelNode.Text;
                        CreatDbObj(servername);                       

                        #region

                        this.lblViewInfo.Text = " ��" + tabname;
                        this.lblNum.Text = SelNode.Nodes.Count.ToString() + "��";

                        SetListViewMenu("column");
                        BindlistViewCol(dbname, tabname);

                        #endregion
                    }
                    break;
                case "proc":
                    {
                        servername = SelNode.Parent.Parent.Parent.Text;
                        string dbname = SelNode.Parent.Parent.Text;
                        string tabname = SelNode.Text;
                        CreatDbObj(servername);

                        #region

                        this.lblViewInfo.Text = " �洢���̣�" + tabname;
                        this.lblNum.Text = SelNode.Nodes.Count.ToString() + "��";

                        //SetListViewMenu("column");
                        //BindlistViewCol(dbname, tabname);
                        //this.listView1.Columns.Clear();
                        this.listView1.Items.Clear();

                        #endregion
                    }
                    break;
                case "column":
                    servername = SelNode.Parent.Parent.Parent.Parent.Text;
                    break;
            }
            

            #endregion
            
        }

        private void CreatDbObj(string servername)
        {
            LTP.CmConfig.DbSettings dbset = LTP.CmConfig.DbConfig.GetSetting(servername);
            dbobj = LTP.DBFactory.DBOMaker.CreateDbObj(dbset.DbType);
            dbobj.DbConnectStr = dbset.ConnectStr;
        }

       

        #region ΪlistView� �� ����
        private void BindlistViewTab(string Dbname, string SelNodeType)
        {
           // SetListViewMenu("table");
           
            this.listView1.Columns.Clear();
            this.listView1.Items.Clear();
            this.listView1.LargeImageList = imglistView;
            this.listView1.SmallImageList = imglistView;
            this.listView1.View = View.Details;
            this.listView1.FullRowSelect = true;           
            listView1.Columns.Add("����", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("������", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("����", 60, HorizontalAlignment.Left);
            listView1.Columns.Add("��������", 200, HorizontalAlignment.Left);

            List<TableInfo> tablist = null;
            switch (SelNodeType)
            {
                case "db":
                    tablist = dbobj.GetTabViewsInfo(Dbname);
                    break;
                case "tableroot":
                    tablist = dbobj.GetTablesInfo(Dbname);
                    break;
                case "viewroot":
                    tablist = dbobj.GetVIEWsInfo(Dbname);
                    break;
                case "procroot":
                    tablist = dbobj.GetProcInfo(Dbname);
                    break;
            }
            if ((tablist!=null)&&(tablist.Count > 0))
            {
                foreach (TableInfo tab in tablist)
                {
                    string name = tab.TabName;
                    ListViewItem item1 = new ListViewItem(name, 0);

                    string user =tab.TabUser;
                    item1.SubItems.Add(user);

                    string type = tab.TabType;
                    switch (type.Trim())
                    {
                        case "S":
                            type = "ϵͳ";
                            break;
                        case "U":
                            type = "�û�";
                            item1.ImageIndex = 2;
                            break;
                        case "TABLE":
                            type = "��";
                            item1.ImageIndex = 2;
                            break;
                        case "V":
                        case "VIEW":
                            type = "��ͼ";
                            item1.ImageIndex = 3;
                            break;
                        case "P":
                            type = "�洢����";
                            item1.ImageIndex = 5;
                            break;
                        default:
                            type = "ϵͳ";
                            break;

                    }
                    item1.SubItems.Add(type);
                    string time = tab.TabDate;
                    item1.SubItems.Add(time);

                    listView1.Items.AddRange(new ListViewItem[] { item1 });

                }
            }
            //if (dt != null)
            //{
            //    DataRow[] dRows = dt.Select("", "type,name ASC");
            //    foreach (DataRow row in dRows)              
            //    {
            //        string name = row["name"].ToString();
            //        ListViewItem item1 = new ListViewItem(name, 0);

            //        string user = row["cuser"].ToString();
            //        item1.SubItems.Add(user);

            //        string type = row["type"].ToString();
            //        switch (type.Trim())
            //        {
            //            case "S":
            //                type = "ϵͳ";
            //                break;
            //            case "U":
            //                type = "�û�";
            //                item1.ImageIndex = 2;
            //                break;
            //            case "TABLE":
            //                type = "��";
            //                item1.ImageIndex = 2;
            //                break;
            //            case "V":
            //            case "VIEW":
            //                type = "��ͼ";
            //                item1.ImageIndex = 3;
            //                break;
            //            case "P":
            //                type = "�洢����";
            //                item1.ImageIndex = 5;
            //                break;
            //            default:
            //                type = "ϵͳ";
            //                break;

            //        }
            //        item1.SubItems.Add(type);
            //        string time = row["dates"].ToString();
            //        item1.SubItems.Add(time);

            //        listView1.Items.AddRange(new ListViewItem[] { item1 });

            //    }
            //}

        }

        #endregion

        #region  ΪlistView� �� ����

        private void BindlistViewCol(string Dbname, string TableName)
        {
            SetListViewMenu("colum");
            //�����б�
            this.listView1.Columns.Clear();
            this.listView1.Items.Clear();
            this.listView1.LargeImageList = imglistView;
            this.listView1.SmallImageList = imglistView;            
            this.listView1.View = View.Details;
            this.listView1.GridLines = true;
            this.listView1.FullRowSelect = true;
            
            listView1.Columns.Add("���", 60, HorizontalAlignment.Left);
            listView1.Columns.Add("����", 110, HorizontalAlignment.Left);
            listView1.Columns.Add("��������", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("����", 40, HorizontalAlignment.Left);
            listView1.Columns.Add("С��", 40, HorizontalAlignment.Left);
            listView1.Columns.Add("��ʶ", 40, HorizontalAlignment.Center);
            listView1.Columns.Add("����", 40, HorizontalAlignment.Center);
            listView1.Columns.Add("�����", 60, HorizontalAlignment.Center);
            listView1.Columns.Add("Ĭ��ֵ", 100, HorizontalAlignment.Left);
            //listView1.Columns.Add("�ֶ�˵��", 100, HorizontalAlignment.Left);

            List<ColumnInfo> collist = dbobj.GetColumnInfoList(Dbname, TableName);
            if ((collist!=null)&&(collist.Count > 0))
            {                
                foreach (ColumnInfo col in collist)
                {
                    string order = col.Colorder;
                    string columnName = col.ColumnName;
                    string columnType = col.TypeName;
                    string Length = col.Length;
                    switch (columnType)
                    {                        
                        case "varchar":
                        case "nvarchar":
                        case "char":
                        case "nchar":
                        case "varbinary":
                            {
                                Length = CodeCommon.GetDataTypeLenVal(columnType, Length);                                
                            }
                            break;
                        default:                          
                            break;
                    }

                    string Preci = col.Preci;
                    string Scale = col.Scale;
                    string defaultVal = col.DefaultVal;
                    string description = col.DeText;
                    string IsIdentity = (col.IsIdentity) ? "��" : "";
                    string ispk = (col.IsPK) ? "��" : "";
                    string isnull = (col.cisNull) ? "��" : "";

                    ListViewItem item1 = new ListViewItem(order, 0);
                    item1.ImageIndex = 4;
                    item1.SubItems.Add(columnName);
                    item1.SubItems.Add(columnType);
                    item1.SubItems.Add(Length);
                    item1.SubItems.Add(Scale);
                    item1.SubItems.Add(IsIdentity);
                    if ((ispk == "��") && (isnull.Trim() == ""))
                    {
                    }
                    else
                    {
                        ispk = "";
                    }
                    item1.SubItems.Add(ispk);
                    item1.SubItems.Add(isnull);
                    item1.SubItems.Add(defaultVal);

                    listView1.Items.AddRange(new ListViewItem[] { item1 });

                }
            }
        }
        #endregion
        
        #region �趨listview�Ҽ��˵�
        private void SetListViewMenu(string itemType)
        {
            switch (itemType.ToLower())
            {
                case "server":
                    {                        
                    }
                    break;
                case "db":
                    {
                    }
                    break;
                case "table":
                    {                        
                    }
                    break;
                case "column":
                    {
                    }
                    break;
                default:
                    {                       
                    }
                    break;
            }
        }
        #endregion

        

    }
}