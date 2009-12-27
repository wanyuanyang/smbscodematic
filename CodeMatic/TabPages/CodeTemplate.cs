using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Collections;
using LTP.CodeBuild;
using System.Threading;
using LTP.CodeHelper;
using LTP.Utility;
namespace Codematic
{
    /// <summary>
    /// ģ���������
    /// </summary>
    public partial class CodeTemplate : Form
    {
        MainForm mainfrm;
        LTP.IDBO.IDbObject dbobj;
        string servername;
        string dbname;
        string tablename;
        private Thread thread;
        //private Thread threadCode;
        delegate void SetListCallback();

        public CodeTemplate()
        {
            InitializeComponent();
        }
        public CodeTemplate(Form mdiParentForm)
        {
            InitializeComponent();
            mainfrm = (MainForm)mdiParentForm;            
            CreatView();
            mainfrm.toolBtn_Run.Visible = true;

            DbView dbviewfrm = (DbView)Application.OpenForms["DbView"];
            if (dbviewfrm != null)
            {
                //SetListView(dbviewfrm);
                try
                {
                    this.thread = new Thread(new ThreadStart(Showlistview));
                    this.thread.Start();
                }
                catch
                {
                }
            }
        }


        #region ����listview

        void Showlistview()
        {
            DbView dbviewfrm = (DbView)Application.OpenForms["DbView"];
            //SetListView(dbviewfrm);
            if (dbviewfrm.treeView1.InvokeRequired)
            {
                SetListCallback d = new SetListCallback(Showlistview);
                dbviewfrm.Invoke(d, null);
            }
            else
            {
                SetListView(dbviewfrm);
            }
        }
        public void SetListView(DbView dbviewfrm)
        {
            #region �õ����Ͷ���            
            TreeNode SelNode = dbviewfrm.treeView1.SelectedNode;
            if (SelNode == null)
                return;
            switch (SelNode.Tag.ToString())
            {
                case "table":
                case "view":
                    {
                        servername = SelNode.Parent.Parent.Parent.Text;
                        dbname = SelNode.Parent.Parent.Text;
                        tablename = SelNode.Text;
                        dbobj = ObjHelper.CreatDbObj(servername);
                        BindlistViewCol(dbname, tablename);
                    }
                    break;
                case "column":
                    {
                        servername = SelNode.Parent.Parent.Parent.Parent.Text;
                        dbname = SelNode.Parent.Parent.Parent.Text;
                        tablename = SelNode.Parent.Text;
                        dbobj = ObjHelper.CreatDbObj(servername);
                        BindlistViewCol(dbname, tablename);

                    }
                    break;
                default:
                    {
                        this.listView1.Items.Clear();
                    }
                    break;
            }

            #endregion

        }

        #endregion

        #region ����ģ���ı���
        public void SettxtTemplate(string Filename)
        {
            StreamReader srFile = new StreamReader(Filename, Encoding.Default);
            string strContents = srFile.ReadToEnd();
            srFile.Close();
            txtTemplate.Text = strContents;
        }
        #endregion

        #region  ΪlistView� �� ����

        private void CreatView()
        {
            //�����б�
            this.listView1.Columns.Clear();
            this.listView1.Items.Clear();
            this.listView1.LargeImageList = imglistView;
            this.listView1.SmallImageList = imglistView;
            this.listView1.View = View.Details;
            this.listView1.GridLines = true;
            this.listView1.CheckBoxes = true;
            this.listView1.FullRowSelect = true;

            listView1.Columns.Add("���", 60, HorizontalAlignment.Center);
            listView1.Columns.Add("����", 110, HorizontalAlignment.Left);
            listView1.Columns.Add("��������", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("����", 40, HorizontalAlignment.Left);
            listView1.Columns.Add("С��", 40, HorizontalAlignment.Left);
            listView1.Columns.Add("��ʶ", 40, HorizontalAlignment.Center);
            listView1.Columns.Add("����", 40, HorizontalAlignment.Center);
            listView1.Columns.Add("�����", 60, HorizontalAlignment.Center);
            listView1.Columns.Add("Ĭ��ֵ", 100, HorizontalAlignment.Left);
            //listView1.Columns.Add("�ֶ�˵��", 100, HorizontalAlignment.Left);
        }

        private void BindlistViewCol(string Dbname, string TableName)
        {
            List<ColumnInfo> collist = dbobj.GetColumnInfoList(Dbname, TableName);
            if ((collist!=null)&&(collist.Count > 0))
            {
                listView1.Items.Clear();
                list_KeyField.Items.Clear();
                foreach (ColumnInfo col in collist)
                {
                    string order = col.Colorder;
                    string columnName = col.ColumnName;
                    string columnType = col.TypeName;        
                    string Length = col.Length;
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
                    if ((ispk == "��") && (isnull.Trim() == ""))//���������ǿ�
                    {
                        this.list_KeyField.Items.Add(columnName + "(" + columnType + ")");
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
            btn_SelAll_Click(null, null);            
        }
        #endregion

        #region ѡ���ֶΰ�ť
        private void btn_SelAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                if (!item.Checked)
                {
                    item.Checked = true;
                }
            }
        }

        private void btn_SelI_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = !item.Checked;
            }
        }

        private void btn_SelClear_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = false;
            }
        }
        #endregion

        #region  ��������

        //�趨������listbox
        private void btn_SetKey_Click(object sender, EventArgs e)
        {
            this.list_KeyField.Items.Clear();
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    this.list_KeyField.Items.Add(item.SubItems[1].Text + "(" + item.SubItems[2].Text + ")");
                }
            }

        }

        ////�õ������Ĺ�����
        //private Hashtable GetKeyFields()
        //{
        //    Hashtable keys = new Hashtable();
        //    foreach (object obj in this.list_KeyField.Items)
        //    {
        //        string str = obj.ToString();
        //        int n = str.IndexOf("(");
        //        string k = str.Substring(0, n);
        //        string v = str.Substring(n + 1, str.Length - k.Length - 2);
        //        keys.Add(k, v);
        //    }
        //    return keys;
        //}

        ////�õ�ѡ����ֶμ���
        //private ArrayList GetFieldlist()
        //{
        //    ArrayList fieldlist = new ArrayList();
        //    foreach (ListViewItem item in listView1.Items)
        //    {
        //        if (item.Checked)
        //        {
        //            fieldlist.Add(item.SubItems[1].Text);
        //        }
        //    }
        //    return fieldlist;
        //}

        /// <summary>
        /// �õ������Ķ�����Ϣ
        /// </summary>
        /// <returns></returns>
        private List<ColumnInfo> GetKeyFields()
        {
            //DataTable dt = dbobj.GetColumnInfoList(dbname, tablename);
            List<ColumnInfo> collist = dbobj.GetColumnInfoList(dbname, tablename);
            DataTable dt = LTP.CodeHelper.CodeCommon.GetColumnInfoDt(collist);
            StringPlus Fields = new StringPlus();
            foreach (object obj in list_KeyField.Items)
            {
                Fields.Append("'" + obj.ToString() + "',");
            }
            Fields.DelLastComma();
            if (dt != null)
            {
                DataRow[] dtrows;
                if (Fields.Value.Length > 0)
                {
                    dtrows = dt.Select("ColumnName in (" + Fields.Value + ")", "colorder asc");
                }
                else
                {
                    dtrows = dt.Select();
                }
                List<ColumnInfo> keys = new List<ColumnInfo>();
                ColumnInfo key;
                foreach (DataRow row in dtrows)
                {
                    string Colorder = row["Colorder"].ToString();
                    string ColumnName = row["ColumnName"].ToString();
                    string TypeName = row["TypeName"].ToString();
                    string isIdentity = row["IsIdentity"].ToString();
                    string IsPK = row["IsPK"].ToString();
                    string Length = row["Length"].ToString();
                    string Preci = row["Preci"].ToString();
                    string Scale = row["Scale"].ToString();
                    string cisNull = row["cisNull"].ToString();
                    string DefaultVal = row["DefaultVal"].ToString();
                    string DeText = row["DeText"].ToString();

                    key = new ColumnInfo();
                    key.Colorder = Colorder;
                    key.ColumnName = ColumnName;
                    key.TypeName = TypeName;
                    key.IsIdentity = (isIdentity == "��") ? true : false;
                    key.IsPK = (IsPK == "��") ? true : false;
                    key.Length = Length;
                    key.Preci = Preci;
                    key.Scale = Scale;
                    key.cisNull = (cisNull == "��") ? true : false;
                    key.DefaultVal = DefaultVal;
                    key.DeText = DeText;
                    keys.Add(key);

                }
                return keys;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// �õ�ѡ����ֶμ���
        /// </summary>
        /// <returns></returns>
        private List<ColumnInfo> GetFieldlist()
        {
            List<ColumnInfo> collist = dbobj.GetColumnInfoList(dbname, tablename);
            DataTable dt = LTP.CodeHelper.CodeCommon.GetColumnInfoDt(collist);
            StringPlus Fields = new StringPlus();
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    Fields.Append("'" + item.SubItems[1].Text + "',");
                }
            }
            Fields.DelLastComma();
            if (dt != null)
            {
                DataRow[] dtrows;
                if (Fields.Value.Length > 0)
                {
                    dtrows = dt.Select("ColumnName in (" + Fields.Value + ")", "colorder asc");
                }
                else
                {
                    dtrows = dt.Select();
                }
                List<ColumnInfo> keys = new List<ColumnInfo>();
                ColumnInfo key;
                foreach (DataRow row in dtrows)
                {
                    string Colorder = row["Colorder"].ToString();
                    string ColumnName = row["ColumnName"].ToString();
                    string TypeName = row["TypeName"].ToString();
                    string isIdentity = row["IsIdentity"].ToString();
                    string IsPK = row["IsPK"].ToString();
                    string Length = row["Length"].ToString();
                    string Preci = row["Preci"].ToString();
                    string Scale = row["Scale"].ToString();
                    string cisNull = row["cisNull"].ToString();
                    string DefaultVal = row["DefaultVal"].ToString();
                    string DeText = row["DeText"].ToString();

                    key = new ColumnInfo();
                    key.Colorder = Colorder;
                    key.ColumnName = ColumnName;
                    key.TypeName = TypeName;
                    key.IsIdentity = (isIdentity == "��") ? true : false;
                    key.IsPK = (IsPK == "��") ? true : false;
                    key.Length = Length;
                    key.Preci = Preci;
                    key.Scale = Scale;
                    key.cisNull = (cisNull == "��") ? true : false;
                    key.DefaultVal = DefaultVal;
                    key.DeText = DeText;
                    keys.Add(key);

                }
                return keys;
            }
            else
            {
                return null;
            }
        }


        #endregion

        #region ���ɴ���

        private void btn_Run_Click(object sender, EventArgs e)
        {
            try
            {
                Run();
                //threadCode = new Thread(new ThreadStart(Run));
                //threadCode.Start();
            }
            catch
            {
            }
        }

        public void Run()
        {
            StatusLabel1.Text = "��������...";
            //Hashtable keys = GetKeyFields();
            //ArrayList fieldlist = GetFieldlist();
            string tempxslt = @"Template\temp.xslt"; //��ʱxslt�ļ�
            try
            {
                string strContent = txtTemplate.Text;
                if (strContent.Trim() == "")
                {
                    MessageBox.Show("ģ������Ϊ�գ�������ģ���������ѡ��ģ�棡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    StatusLabel1.Text = "����";
                    return;
                }
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(strContent);
                xdoc.Save(tempxslt);
            }
            catch (System.Exception ex)
            {
                StatusLabel1.Text = "ģ���ʽ����" + ex.Message;
                return;
            }

            string strcode = "";
            try
            {
                BuilderTemp bt = new BuilderTemp(dbobj, dbname, tablename, GetFieldlist(), GetKeyFields(), tempxslt);
                strcode = bt.GetCode();

                //ɾ����ʱ�ļ�
                if (File.Exists(tempxslt))
                {
                    File.Delete(tempxslt);
                }
                string xmlDoc = @"Template\temp.xml"; //��ʱ�����ļ�temp.xml
                if (File.Exists(xmlDoc))
                {
                    File.Delete(xmlDoc);
                }
            }
            catch (System.Exception ex)
            {
                StatusLabel1.Text = "����ת��ʧ�ܣ�" + ex.Message;
                return;
            }
            txtCode.Text = strcode;
            this.tabControl1.SelectedIndex = 1;

            //string filename = @"Template\tempcode.cs";
            //StreamWriter sw = new StreamWriter(filename, false, Encoding.Default);//,false);
            //sw.Write(strcode);
            //sw.Flush();
            //sw.Close();

            StatusLabel1.Text = "���ɳɹ���";
            //mainfrm.AddSinglePage(new CodeEditor(filename), "Class1.cs");

        }

        #endregion

        private void txtTemplate_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                string s = sender.ToString();
            }
        }

        
    }
}