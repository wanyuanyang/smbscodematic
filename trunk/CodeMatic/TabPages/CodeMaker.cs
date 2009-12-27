using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Codematic.UserControls;
using LTP.CodeBuild;
using LTP.Utility;
using LTP.CodeHelper;
using System.Threading;
using Maticsoft.AddInManager;
namespace Codematic
{
    public partial class CodeMaker : Form
    {
        public UcCodeView codeview;
        LTP.CmConfig.ModuleSettings setting;
        LTP.IDBO.IDbObject dbobj;
        LTP.CodeBuild.CodeBuilders cb;//�������ɶ���
        string servername;
        string dbname;
        string tablename;
        private Thread thread;
        delegate void SetListCallback();        
        DALTypeAddIn cm_daltype ;
        DALTypeAddIn cm_blltype;
        DALTypeAddIn cm_webtype;
        NameRule namerule = new NameRule();//����������

        public CodeMaker()
        {
            InitializeComponent();
            list_KeyField.Height = 22;
            
            codeview = new UcCodeView();
            tabPage2.Controls.Add(codeview);

            SetListViewMenu("colum");
            CreatView();
            SetFormConfig();
            DbView dbviewfrm = (DbView)Application.OpenForms["DbView"];
            if (dbviewfrm != null)
            {                
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
        private void CodeMaker_Load(object sender, EventArgs e)
        {
            
        }

        void Showlistview()
        {
            DbView dbviewfrm = (DbView)Application.OpenForms["DbView"];            
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


        #region ��ʼ����������
        private void SetFormConfig()
        {
            this.panel1.Height = 150;//�����ֶ��������߶�
            radbtn_Type_Click(null, null);

            setting = LTP.CmConfig.ModuleConfig.GetSettings();
            txtProjectName.Text = setting.ProjectName;
            txtNameSpace.Text = setting.Namepace;
            txtNameSpace2.Text = setting.Folder;
            txtProcPrefix.Text = setting.ProcPrefix;
            switch (setting.AppFrame)
            {
                case "One":
                    radbtn_Frame_One.Checked = true;
                    break;
                case "S3":
                    radbtn_Frame_S3.Checked = true;
                    break;
                case "F3":
                    radbtn_Frame_F3.Checked = true;
                    break;
            }
            radbtn_Type_Click(null, null);
            radbtn_Frame_Click(null, null);
            radbtn_F3_Click(null, null);
            
            #region ���ز��
            //cm_daltype = new DALTypeAddIn();
            cm_daltype = new DALTypeAddIn("LTP.IBuilder.IBuilderDAL");
            cm_daltype.Title = "DAL";
            groupBox_DALType.Controls.Add(cm_daltype);
            cm_daltype.Location = new System.Drawing.Point(30, 18);
            cm_daltype.SetSelectedDALType(setting.DALType.Trim());

            cm_blltype = new DALTypeAddIn("LTP.IBuilder.IBuilderBLL");
            cm_blltype.Title = "BLL";
            groupBox_DALType.Controls.Add(cm_blltype);
            cm_blltype.Location = new System.Drawing.Point(30, 18);
            cm_blltype.SetSelectedDALType(setting.BLLType.Trim());

            cm_webtype = new DALTypeAddIn("LTP.IBuilder.IBuilderWeb");
            cm_webtype.Title = "Web";
            groupBox_DALType.Controls.Add(cm_webtype);
            cm_webtype.Location = new System.Drawing.Point(30, 18);
            cm_webtype.SetSelectedDALType(setting.WebType.Trim()); 
            #endregion

            this.tabControl1.SelectedIndex = 0;
        }

        
        #endregion

        #region  ѡ������

        #region ����ѡ��
        private void radbtn_Type_Click(object sender, EventArgs e)
        {
            if (this.radbtn_Type_DB.Checked)
            {
                this.groupBox_DB.Visible = true;
                this.groupBox_AppType.Visible = false;
                this.groupBox_DALType.Visible = false;
                this.groupBox_Method.Visible = false;
                this.groupBox_FrameSel.Visible = false;
                this.groupBox_F3.Visible = false;
                this.groupBox_Web.Visible = false;
            }
            if (this.radbtn_Type_CS.Checked)
            {
                this.groupBox_DB.Visible = false;
                this.groupBox_AppType.Visible = true;
                this.groupBox_DALType.Visible = true;
                this.groupBox_Method.Visible = true;
                this.groupBox_FrameSel.Visible = true;
                this.groupBox_F3.Visible = true;
                this.groupBox_Web.Visible = false;
                
                //this.groupBox_FrameSel.Top = 188;
                //this.groupBox_F3.Top = 235;
                //this.groupBox_DALType.Top = 286;
                //this.groupBox_Method.Top = 337;
                //this.groupBox_AppType.Top = 401;
                
                radbtn_Frame_Click(sender, e);

            }
            if (this.radbtn_Type_Web.Checked)
            {
                this.groupBox_DB.Visible = false;
                this.groupBox_AppType.Visible = false;
                this.groupBox_DALType.Visible = true;
                this.groupBox_Method.Visible = false;
                this.groupBox_FrameSel.Visible = false;
                this.groupBox_F3.Visible = false;
                this.groupBox_Web.Visible = true;

                cm_daltype.Visible = false;
                cm_blltype.Visible = false;
                cm_webtype.Visible = true;

                //this.groupBox_Web.Top = 188;
            }


        }
        #endregion

        #region �ܹ�ѡ��
        private void radbtn_Frame_Click(object sender, EventArgs e)
        {
            if (this.radbtn_Frame_One.Checked)
            {
                //this.groupBox_DALType.Top = 342;
                //this.groupBox_Method.Top = 393;

                this.groupBox_DB.Visible = false;
                this.groupBox_F3.Visible = false;
                this.groupBox_DALType.Visible = true;
                this.groupBox_Method.Visible = true;
                this.groupBox_AppType.Visible = false;
                this.groupBox_Web.Visible = false;

            }
            if (this.radbtn_Frame_S3.Checked)
            {
                //this.groupBox_F3.Top = 343;
                //this.groupBox_DALType.Top = 393;
                //this.groupBox_Method.Top = 444;

                this.groupBox_DB.Visible = false;
                this.groupBox_F3.Visible = true;
                this.groupBox_DALType.Visible = true;
                this.groupBox_Method.Visible = true;
                this.groupBox_AppType.Visible = false;
                this.groupBox_Web.Visible = false;

                this.radbtn_F3_IDAL.Visible = false;
                this.radbtn_F3_DALFactory.Visible = false;

                radbtn_F3_Click(sender, e);

            }
            if (this.radbtn_Frame_F3.Checked)
            {
                this.groupBox_DB.Visible = false;
                this.groupBox_F3.Visible = true;
                this.groupBox_DALType.Visible = true;
                this.groupBox_Method.Visible = true;
                this.groupBox_AppType.Visible = true;
                this.groupBox_Web.Visible = false;

                this.radbtn_F3_IDAL.Visible = true;
                this.radbtn_F3_DALFactory.Visible = true;

                //this.groupBox_F3.Top = 343;
                //this.groupBox_DALType.Top = 393;
                //this.groupBox_Method.Top = 444;
                //this.groupBox_AppType.Top = 499;

                radbtn_F3_Click(sender, e);
            }

        }
        #endregion

        #region ��ѡ��(����ģʽ��)
        private void radbtn_F3_Click(object sender, EventArgs e)
        {
            if (this.radbtn_F3_Model.Checked)
            {
                this.groupBox_DALType.Visible = false;
                this.groupBox_Method.Visible = false;
                this.groupBox_AppType.Visible = false;
                this.groupBox_Web.Visible = false;
            }
            if (this.radbtn_F3_DAL.Checked)
            {
                this.groupBox_DALType.Visible = true;
                cm_daltype.Visible = true;
                cm_blltype.Visible = false;
                cm_webtype.Visible = false;
                this.groupBox_Method.Visible = true;
                this.groupBox_AppType.Visible = false;
                this.groupBox_Web.Visible = false;
            }
            if (this.radbtn_F3_IDAL.Checked)
            {
                this.groupBox_DALType.Visible = false;
                this.groupBox_Method.Visible = true;
                this.groupBox_AppType.Visible = false;
                this.groupBox_Web.Visible = false;
            }
            if (this.radbtn_F3_DALFactory.Checked)
            {
                this.groupBox_DALType.Visible = false;
                this.groupBox_Method.Visible = false;
                this.groupBox_AppType.Visible = true;
                this.groupBox_Web.Visible = false;
            }
            if (this.radbtn_F3_BLL.Checked)
            {
                this.groupBox_DALType.Visible = true;
                cm_daltype.Visible = false;
                cm_blltype.Visible = true;
                cm_webtype.Visible = false;
                this.groupBox_Method.Visible = true;
                this.groupBox_AppType.Visible = false;
                this.groupBox_Web.Visible = false;
            }
        }
        #endregion

        private void radbtn_DBSel_Click(object sender, EventArgs e)
        {
            if (this.radbtn_DB_Proc.Checked)
            {
                chk_DB_GetMaxID.Visible = true;
                chk_DB_Exists.Visible = true;
                chk_DB_Add.Visible = true;
                chk_DB_Update.Visible = true;
                chk_DB_Delete.Visible = true;
                chk_DB_GetModel.Visible = true;
                chk_DB_GetList.Visible = true;
                txtProcPrefix.Visible=true;
                txtTabname.Visible=true;
                label5.Visible = true;
                label6.Visible = true;
                label7.Visible = true;
                
            }
            else
            {
                chk_DB_GetMaxID.Visible = false;
                chk_DB_Exists.Visible = false;
                chk_DB_Add.Visible = false;
                chk_DB_Update.Visible = false;
                chk_DB_Delete.Visible = false;
                chk_DB_GetModel.Visible = false;
                chk_DB_GetList.Visible = false;
                txtProcPrefix.Visible = false;
                txtTabname.Visible = false;
                label5.Visible = false;
                label6.Visible = false;
                label7.Visible = false;
            }
        }
        #endregion

        #region ����listview

        public void SetListView(DbView dbviewfrm)
        {
            #region �õ����Ͷ���

            TreeNode SelNode = dbviewfrm.treeView1.SelectedNode;
            if (SelNode == null)
                return;
            switch (SelNode.Tag.ToString())
            {
                case "table":
                    {
                        servername = SelNode.Parent.Parent.Parent.Text;
                        dbname = SelNode.Parent.Parent.Text;
                        tablename = SelNode.Text;
                        dbobj = ObjHelper.CreatDbObj(servername);
                        SetListViewMenu("column");
                        BindlistViewCol(dbname, tablename);
                    }
                    break;
                case "view":
                    {
                        servername = SelNode.Parent.Parent.Parent.Text;
                        dbname = SelNode.Parent.Parent.Text;
                        tablename = SelNode.Text;
                        dbobj = ObjHelper.CreatDbObj(servername);
                        SetListViewMenu("column");
                        BindlistViewCol(dbname, tablename);
                    }
                    break;
                case "column":
                    {
                        servername = SelNode.Parent.Parent.Parent.Parent.Text;
                        dbname = SelNode.Parent.Parent.Parent.Text;
                        tablename = SelNode.Parent.Text;
                        dbobj = ObjHelper.CreatDbObj(servername);
                        SetListViewMenu("column");
                        BindlistViewCol(dbname, tablename);

                    }
                    break;
                default:
                    {
                        this.listView1.Items.Clear();
                    }
                    break;
            }
            if (dbobj != null)
            {
                cb = new CodeBuilders(dbobj);
            }
            #endregion

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
            this.chk_CS_GetMaxID.Checked = true;
            List<ColumnInfo> collist = dbobj.GetColumnInfoList(Dbname, TableName);
            if ((collist!=null)&&(collist.Count > 0))
            {
                listView1.Items.Clear();
                list_KeyField.Items.Clear();
                this.chk_CS_GetMaxID.Enabled = true;
                foreach (ColumnInfo col in collist)
                {
                    string order = col.Colorder;
                    string columnName = col.ColumnName;
                    string columnType = col.TypeName;
                    string Length = col.Length;
                    if (columnType == "nvarchar")
                    {
                        Length = (int.Parse(Length) / 2).ToString();
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
                    if ((ispk == "��") && (isnull.Trim() == ""))//���������ǿ�
                    {
                        this.list_KeyField.Items.Add(columnName);
                        if (IsIdentity == "��")
                        {
                            this.chk_CS_GetMaxID.Checked = false;
                            this.chk_CS_GetMaxID.Enabled = false;
                            this.chk_DB_GetMaxID.Checked = false;
                            this.chk_DB_GetMaxID.Enabled = false;
                            //KeyIsIdentity = true;
                        }
                    }
                    else
                    {
                        ispk = "";
                        if (IsIdentity == "��")
                        {
                            this.list_KeyField.Items.Add(columnName);
                            this.chk_CS_GetMaxID.Checked = false;
                            this.chk_CS_GetMaxID.Enabled = false;
                            this.chk_DB_GetMaxID.Checked = false;
                            this.chk_DB_GetMaxID.Enabled = false;
                            //KeyIsIdentity = true;
                        }
                    }

                    item1.SubItems.Add(ispk);
                    item1.SubItems.Add(isnull);
                    item1.SubItems.Add(defaultVal);

                    listView1.Items.AddRange(new ListViewItem[] { item1 });

                }
            }
            btn_SelAll_Click(null, null);
            txtTabname.Text = TableName;
            txtClassName.Text = TableName;
            lblkeycount.Text = list_KeyField.Items.Count.ToString() + "������";
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

        #region  �趨����
        //�趨������listbox
        private void btn_SetKey_Click(object sender, EventArgs e)
        {
            this.list_KeyField.Items.Clear();
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    this.list_KeyField.Items.Add(item.SubItems[1].Text);
                }
            }
            lblkeycount.Text = list_KeyField.Items.Count.ToString() + "������";
        }
        
        #endregion

        #region ��������

        /// <summary>
        /// ���ô���ؼ�����
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="strContent"></param>
        private void SettxtContent(string Type, string strContent)
        {
            codeview.SettxtContent(Type, strContent);
            this.tabControl1.SelectedIndex = 1;

        }
        /// <summary>
        /// �õ������Ķ�����Ϣ
        /// </summary>
        /// <returns></returns>
        private List<ColumnInfo> GetKeyFields()
        {           
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
        ///// <summary>
        ///// �õ�ѡ����ֶμ���
        ///// </summary>
        ///// <returns></returns>
        //private List<ColumnInfo> GetFieldlist()
        //{            
        //    List<ColumnInfo> collist = dbobj.GetColumnInfoList(dbname, tablename);

        //    DataTable dt = LTP.CodeHelper.CodeCommon.GetColumnInfoDt(collist);
        //    StringPlus Fields = new StringPlus();
        //    foreach (ListViewItem item in listView1.Items)
        //    {
        //        if (item.Checked)
        //        {
        //            Fields.Append("'" + item.SubItems[1].Text + "',");
        //        }
        //    }
        //    Fields.DelLastComma();
        //    if (dt != null)
        //    {
        //        DataRow[] dtrows;
        //        if (Fields.Value.Length > 0)
        //        {                    
        //            dtrows = dt.Select("ColumnName in (" + Fields.Value + ")", "colorder asc");
                    
        //        }
        //        else
        //        {
        //            dtrows = dt.Select();
        //        }
        //        List<ColumnInfo> keys = new List<ColumnInfo>();
        //        ColumnInfo key;
        //        foreach (DataRow row in dtrows)
        //        {
        //            string Colorder = row["Colorder"].ToString();
        //            string ColumnName = row["ColumnName"].ToString();
        //            string TypeName = row["TypeName"].ToString();
        //            string isIdentity = row["IsIdentity"].ToString();
        //            string IsPK = row["IsPK"].ToString();
        //            string Length = row["Length"].ToString();
        //            string Preci = row["Preci"].ToString();
        //            string Scale = row["Scale"].ToString();
        //            string cisNull = row["cisNull"].ToString();
        //            string DefaultVal = row["DefaultVal"].ToString();
        //            string DeText = row["DeText"].ToString();

        //            key = new ColumnInfo();
        //            key.Colorder = Colorder;
        //            key.ColumnName = ColumnName;
        //            key.TypeName = TypeName;
        //            key.IsIdentity = (isIdentity == "��") ? true : false;
        //            key.IsPK = (IsPK == "��") ? true : false;
        //            key.Length = Length;
        //            key.Preci = Preci;
        //            key.Scale = Scale;
        //            key.cisNull = (cisNull == "��") ? true : false;
        //            key.DefaultVal = DefaultVal;
        //            key.DeText = DeText;
        //            keys.Add(key);

        //        }
        //        return keys;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// �õ�ѡ����ֶμ���
        /// </summary>
        /// <returns></returns>
        private List<ColumnInfo> GetFieldlist()
        {
            List<ColumnInfo> collist = dbobj.GetColumnInfoList(dbname, tablename);
            List<ColumnInfo> collistSel = new List<ColumnInfo>();
            foreach (ColumnInfo col in collist)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    if (col.ColumnName == item.SubItems[1].Text)
                    {
                        if (item.Checked)
                        {
                            collistSel.Add(col);
                        }
                        break;
                    }                    
                }
            }
            return collistSel;
        }


        //�õ�dal������
        private string GetDALType()
        {
            string daltype = "";
            daltype = cm_daltype.AppGuid;
            if ((daltype == "") && (daltype == "System.Data.DataRowView"))
            {
                MessageBox.Show("ѡ������ݲ�����������رպ����ԣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ""; 
            }
            return daltype;
        }

        //�õ�bll������
        private string GetBLLType()
        {
            string blltype = "";
            blltype = cm_blltype.AppGuid;
            if ((blltype == "") && (blltype == "System.Data.DataRowView"))
            {
                MessageBox.Show("ѡ������ݲ�����������رպ����ԣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
            return blltype;
        }
        //�õ�web������
        public string GetWebType()
        {
            string webtype = "";
            webtype = cm_webtype.AppGuid;
            if ((webtype == "") && (webtype == "System.Data.DataRowView"))
            {
                MessageBox.Show("ѡ��ı�ʾ������������رպ����ԣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
            return webtype;
        }

        #endregion

        #region �������� ��ť

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count < 1)
            {
                MessageBox.Show("û���κο������ɵ��", "��ѡ��", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (list_KeyField.Items.Count == 0)
            {
                DialogResult result = MessageBox.Show("û�������ֶκ������ֶΣ���ȷ��Ҫ�������ɣ�", "������ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            try
            {
                if (this.radbtn_Type_DB.Checked)
                {
                    CreatDB();
                }
                if (this.radbtn_Type_CS.Checked)
                {
                    CreatCS();
                }
                if (this.radbtn_Type_Web.Checked)
                {
                    CreatWeb();
                }
            }
            catch(System.SystemException ex)
            {
                MessageBox.Show("���ɴ���ʧ�ܣ���رպ����´����ԡ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogInfo.WriteLog(ex.Message);
            }

            #region ��������

            if (this.radbtn_Frame_One.Checked)
            {
                setting.AppFrame = "One";
            }
            if (this.radbtn_Frame_S3.Checked)
            {
                setting.AppFrame = "S3";
            }
            if (this.radbtn_Frame_F3.Checked)
            {
                setting.AppFrame = "F3";
            }

            setting.DALType = GetDALType();
            setting.BLLType = GetBLLType();
            setting.ProjectName = txtProjectName.Text;
            setting.Namepace = txtNameSpace.Text;
            setting.Folder = txtNameSpace2.Text;
            setting.ProcPrefix = txtProcPrefix.Text;
            LTP.CmConfig.ModuleConfig.SaveSettings(setting);
            #endregion
        }

        #endregion

        #region ���� ���ݿ�ű�
        private void CreatDB()
        {
            if (this.radbtn_DB_Proc.Checked)
            {
                CreatDBProc();
            }
            else
            {
                CreatDBScript();
            }
        }

        //�����洢����
        private void CreatDBProc()
        {
            string projectname = this.txtProjectName.Text;
            string snamespace = this.txtNameSpace.Text;
            string snamespace2 = this.txtNameSpace2.Text;
            string classname = this.txtClassName.Text;
            string procPrefix = this.txtProcPrefix.Text;
            string procTabname = this.txtTabname.Text;

            bool Maxid = this.chk_DB_GetMaxID.Checked;
            bool Exists = this.chk_DB_Exists.Checked;
            bool Add = this.chk_DB_Add.Checked;
            bool Update = this.chk_DB_Update.Checked;
            bool Delete = this.chk_DB_Delete.Checked;
            bool GetModel = this.chk_DB_GetModel.Checked;
            bool List = this.chk_DB_GetList.Checked;

            LTP.IDBO.IDbScriptBuilder dsb = ObjHelper.CreatDsb(servername);
            dsb.DbName = dbname;
            dsb.TableName = procTabname;// tablename;
            dsb.ProjectName = projectname;
            dsb.ProcPrefix = procPrefix;            
            dsb.Keys = GetKeyFields();
            dsb.Fieldlist = GetFieldlist();
            string strProc = dsb.GetPROCCode(Maxid, Exists, Add, Update, Delete, GetModel, List);
            SettxtContent("SQL", strProc);
        }
        //���ݿ�ű�
        private void CreatDBScript()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            LTP.IDBO.IDbScriptBuilder dsb = ObjHelper.CreatDsb(servername);
            dsb.Fieldlist = GetFieldlist();
            string strScript = dsb.CreateTabScript(dbname, tablename);
            SettxtContent("SQL", strScript);
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        #endregion

        #region ���� C#����
        //�ܹ�ѡ��
        private void CreatCS()
        {
            if (this.radbtn_Frame_One.Checked)
            {
                CreatCsOne();
            }
            if (this.radbtn_Frame_S3.Checked)
            {
                CreatCsS3();
            }
            if (this.radbtn_Frame_F3.Checked)
            {
                CreatCsF3();
            }
        }

        #region ����ṹ
        private void CreatCsOne()
        {
            string procPrefix = this.txtProcPrefix.Text;
            string namepace = this.txtNameSpace.Text.Trim();
            string namepace2 = this.txtNameSpace2.Text.Trim();
            if (namepace2.Trim() != "")
            {
                namepace = namepace + "." + namepace2;
            }
            string classname = this.txtClassName.Text;
            if (classname == "")
            {
                classname = tablename;
            }

            BuilderFrameOne cfo = new BuilderFrameOne(dbobj, dbname, tablename, classname, GetFieldlist(), GetKeyFields(), namepace,namepace2, setting.DbHelperName);

            string dALtype = GetDALType();
            string strCode = cfo.GetCode(dALtype,chk_CS_GetMaxID.Checked, chk_CS_Exists.Checked, chk_CS_Add.Checked,
                chk_CS_Update.Checked, chk_CS_Delete.Checked, chk_CS_GetModel.Checked, chk_CS_GetList.Checked, procPrefix);
            SettxtContent("CS", strCode);
        }
        #endregion

        #region ������

        private void CreatCsS3()
        {
            if (radbtn_F3_Model.Checked)
            {
                CreatCsS3Model();
            }
            if (radbtn_F3_DAL.Checked)
            {
                CreatCsS3DAL();
            }
            if (radbtn_F3_BLL.Checked)
            {
                CreatCsS3BLL();
            }
        }
        private void CreatCsS3Model()
        {            
            string namepace = this.txtNameSpace.Text.Trim();
            string namepace2 = this.txtNameSpace2.Text.Trim();
            string modelname = this.txtClassName.Text;
            if (modelname == "")
            {
                modelname = tablename;
            }
            string bllname = modelname;
            string dalname = modelname;
            //����������
            modelname = namerule.GetModelClass(modelname);
            bllname = namerule.GetBLLClass(bllname);
            dalname = namerule.GetDALClass(dalname);

            BuilderFrameS3 s3 = new BuilderFrameS3(dbobj, dbname, tablename, modelname,bllname,dalname, GetFieldlist(), GetKeyFields(),  namepace, namepace2, setting.DbHelperName);
            string strCode = s3.GetModelCode();
            SettxtContent("CS", strCode);
        }
        
        private void CreatCsS3DAL()
        {
            string procPrefix = this.txtProcPrefix.Text;
            string namepace = this.txtNameSpace.Text.Trim();
            string namepace2 = this.txtNameSpace2.Text.Trim();
            string modelname = this.txtClassName.Text;
            if (modelname == "")
            {
                modelname = tablename;
            }
            string bllname = modelname;
            string dalname = modelname;
            //����������
            modelname = namerule.GetModelClass(modelname);
            bllname = namerule.GetBLLClass(bllname);
            dalname = namerule.GetDALClass(dalname);

            BuilderFrameS3 s3 = new BuilderFrameS3(dbobj, dbname, tablename, modelname, bllname, dalname, GetFieldlist(), GetKeyFields(), namepace, namepace2, setting.DbHelperName);

            string dALtype = GetDALType();
            string strCode = s3.GetDALCode(dALtype, chk_CS_GetMaxID.Checked, chk_CS_Exists.Checked, chk_CS_Add.Checked, 
                chk_CS_Update.Checked, chk_CS_Delete.Checked, chk_CS_GetModel.Checked, chk_CS_GetList.Checked, procPrefix);
            SettxtContent("CS", strCode);
        }
        private void CreatCsS3BLL()
        {            
            string namepace = this.txtNameSpace.Text.Trim();
            string namepace2 = this.txtNameSpace2.Text.Trim();
            string modelname = this.txtClassName.Text;
            if (modelname == "")
            {
                modelname = tablename;
            }
            string bllname = modelname;
            string dalname = modelname;
            //����������
            modelname = namerule.GetModelClass(modelname);
            bllname = namerule.GetBLLClass(bllname);
            dalname = namerule.GetDALClass(dalname);

            BuilderFrameS3 s3 = new BuilderFrameS3(dbobj, dbname, tablename, modelname, bllname, dalname, GetFieldlist(), GetKeyFields(), namepace, namepace2, setting.DbHelperName);
            string blltype = GetBLLType();
            string strCode = s3.GetBLLCode(blltype, chk_CS_GetMaxID.Checked, chk_CS_Exists.Checked, chk_CS_Add.Checked, chk_CS_Update.Checked, chk_CS_Delete.Checked, chk_CS_GetModel.Checked, chk_CS_GetModelByCache.Checked, chk_CS_GetList.Checked);
            SettxtContent("CS", strCode);
        }
        #endregion

        #region ����ģʽ����
        private void CreatCsF3()
        {
            if (radbtn_F3_Model.Checked)
            {
                CreatCsF3Model();
            }
            if (radbtn_F3_DAL.Checked)
            {
                CreatCsF3DAL();
            }
            if (radbtn_F3_IDAL.Checked)
            {
                CreatCsF3IDAL();
            }
            if (radbtn_F3_DALFactory.Checked)
            {
                CreatCsF3DALFactory();
            }
            if (radbtn_F3_BLL.Checked)
            {
                CreatCsF3BLL();
            }
        }
        private void CreatCsF3Model()
        {            
            string namepace = this.txtNameSpace.Text.Trim();
            string namepace2 = this.txtNameSpace2.Text.Trim();
            string modelname = this.txtClassName.Text;
            if (modelname == "")
            {
                modelname = tablename;
            }
            string bllname = modelname;
            string dalname = modelname;

            //����������
            modelname = namerule.GetModelClass(modelname);
            bllname = namerule.GetBLLClass(bllname);
            dalname = namerule.GetDALClass(dalname);

            BuilderFrameF3 f3 = new BuilderFrameF3(dbobj, dbname, tablename, modelname, bllname, dalname, GetFieldlist(), GetKeyFields(), namepace, namepace2, setting.DbHelperName);
            string strCode = f3.GetModelCode();
            SettxtContent("CS", strCode);
        }
        private void CreatCsF3DAL()
        {
            string procPrefix = this.txtProcPrefix.Text;
            string namepace = this.txtNameSpace.Text.Trim();
            string namepace2 = this.txtNameSpace2.Text.Trim();
            string modelname = this.txtClassName.Text;
            if (modelname == "")
            {
                modelname = tablename;
            }
            string bllname = modelname;
            string dalname = modelname;
            //����������
            modelname = namerule.GetModelClass(modelname);
            bllname = namerule.GetBLLClass(bllname);
            dalname = namerule.GetDALClass(dalname);

            BuilderFrameF3 f3 = new BuilderFrameF3(dbobj, dbname, tablename, modelname, bllname, dalname, GetFieldlist(), GetKeyFields(), namepace, namepace2, setting.DbHelperName);

            string dALtype = GetDALType();
            string strCode = f3.GetDALCode(dALtype, chk_CS_GetMaxID.Checked, chk_CS_Exists.Checked, chk_CS_Add.Checked, 
                chk_CS_Update.Checked, chk_CS_Delete.Checked, chk_CS_GetModel.Checked, chk_CS_GetList.Checked, procPrefix);
            SettxtContent("CS", strCode);
        }
        private void CreatCsF3IDAL()
        {            
            string namepace = this.txtNameSpace.Text.Trim();
            string namepace2 = this.txtNameSpace2.Text.Trim();
            string modelname = this.txtClassName.Text;
            if (modelname == "")
            {
                modelname = tablename;
            }
            string bllname = modelname;
            string dalname = modelname;
            //����������
            modelname = namerule.GetModelClass(modelname);
            bllname = namerule.GetBLLClass(bllname);
            dalname = namerule.GetDALClass(dalname);

            BuilderFrameF3 f3 = new BuilderFrameF3(dbobj, dbname, tablename, modelname, bllname, dalname, GetFieldlist(), GetKeyFields(), namepace, namepace2, setting.DbHelperName);
            string strCode = f3.GetIDALCode(chk_CS_GetMaxID.Checked, chk_CS_Exists.Checked, chk_CS_Add.Checked, chk_CS_Update.Checked, chk_CS_Delete.Checked, chk_CS_GetModel.Checked, chk_CS_GetList.Checked, chk_CS_GetList.Checked);
            SettxtContent("CS", strCode);
        }
        private void CreatCsF3DALFactory()
        {            
            string namepace = this.txtNameSpace.Text.Trim();
            string namepace2 = this.txtNameSpace2.Text.Trim();
            string modelname = this.txtClassName.Text;
            if (modelname == "")
            {
                modelname = tablename;
            }
            string bllname = modelname;
            string dalname = modelname;
            //����������
            modelname = namerule.GetModelClass(modelname);
            bllname = namerule.GetBLLClass(bllname);
            dalname = namerule.GetDALClass(dalname);

            BuilderFrameF3 f3 = new BuilderFrameF3(dbobj, dbname, tablename, modelname, bllname, dalname, GetFieldlist(), GetKeyFields(), namepace, namepace2, setting.DbHelperName);
            string strCode = f3.GetDALFactoryCode();
            SettxtContent("CS", strCode);
        }
        private void CreatCsF3BLL()
        {            
            string namepace = this.txtNameSpace.Text.Trim();
            string namepace2 = this.txtNameSpace2.Text.Trim();
            string modelname = this.txtClassName.Text;
            if (modelname == "")
            {
                modelname = tablename;
            }
            string bllname = modelname;
            string dalname = modelname;
            //����������
            modelname = namerule.GetModelClass(modelname);
            bllname = namerule.GetBLLClass(bllname);
            dalname = namerule.GetDALClass(dalname);

            BuilderFrameF3 f3 = new BuilderFrameF3(dbobj, dbname, tablename, modelname, bllname, dalname, GetFieldlist(), GetKeyFields(), namepace, namepace2, setting.DbHelperName);
            string blltype = GetBLLType();
            string strCode = f3.GetBLLCode(blltype, chk_CS_GetMaxID.Checked, chk_CS_Exists.Checked, chk_CS_Add.Checked, chk_CS_Update.Checked, chk_CS_Delete.Checked, chk_CS_GetModel.Checked, chk_CS_GetModelByCache.Checked, chk_CS_GetList.Checked, chk_CS_GetList.Checked);
            SettxtContent("CS", strCode);
        }
        #endregion

        #endregion

        #region ���� Webҳ��
        private void CreatWeb()
        {            
            string namepace = this.txtNameSpace.Text.Trim();
            string folder = this.txtNameSpace2.Text.Trim();
            string modelname = this.txtClassName.Text;
            if (modelname == "")
            {
                modelname = tablename;
            }
            string bllname = modelname;            
            //����������
            modelname = namerule.GetModelClass(modelname);
            bllname = namerule.GetBLLClass(bllname);
           
            //LTP.BuilderWeb.BuilderWeb bw = new LTP.BuilderWeb.BuilderWeb();            
            //bw.NameSpace = namepace;
            //bw.Fieldlist = GetFieldlist();
            //bw.Keys = GetKeyFields();
            //bw.ModelName = modelname;
            //bw.BLLName = bllname;
            //bw.Folder = folder;

            cb.BLLName = bllname;
            cb.NameSpace = namepace;
            cb.Fieldlist = GetFieldlist();
            cb.Keys = GetKeyFields();
            cb.ModelName = modelname;
            cb.Folder = folder;

            string webtype = GetWebType();
            cb.CreatBuilderWeb(webtype);

            if (radbtn_Web_Aspx.Checked)
            {
                string strCode = cb.GetWebHtmlCode(chk_Web_HasKey.Checked, chk_Web_Add.Checked, chk_Web_Update.Checked, chk_Web_Show.Checked, true);
                SettxtContent("Aspx", strCode);
            }
            else
            {
                string strCode = cb.GetWebCode(chk_Web_HasKey.Checked, chk_Web_Add.Checked, chk_Web_Update.Checked, chk_Web_Show.Checked, true);
                SettxtContent("CS", strCode);
            }
        }
        #endregion


    }
}