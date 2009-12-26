using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Data.SqlClient;
using LTP.Utility;
using LTP.IDBO;
using LTP.DBFactory;
using LTP.CodeHelper;
namespace LTP.CodeBuild
{
    /// <summary>
    /// C# ���������ࡣ
    /// </summary>
    public class CodeBuilders
    {
        #region  ˽�б���
        IDbObject dbobj;
        IBuilder.IBuilderWeb ibw;
        #endregion

        #region  ��������

        private string _dbtype;
        private string _dbconnectStr;
        private string _dbname;
        private string _tablename;
        private string _modelname;//model����    
        private string _bllname;//bll����    
        private string _dalname;//dal����    
        private string _namespace = "Maticsoft";//���������ռ���
        private string _folder;//�����ļ���
        private string _dbhelperName = "DbHelperSQL";//���ݿ��������
        private List<ColumnInfo> _keys; //�����������ֶ��б�
        private List<ColumnInfo> _fieldlist ;
        private string _procprefix;
        
        public string DbType
        {
            set { _dbtype = value; }
            get { return _dbtype; }
        }
        public string DbConnectStr
        {
            set { _dbconnectStr = value; }
            get { return _dbconnectStr; }
        }
        public string DbName
        {
            set { _dbname = value; }
            get { return _dbname; }
        }
        public string TableName
        {
            set { _tablename = value; }
            get { return _tablename; }
        }
        
                            
        public string NameSpace
        {
            set { _namespace = value; }
            get { return _namespace; }
        }
        public string Folder
        {
            set { _folder = value; }
            get { return _folder; }
        }
        public string ModelName
        {
            set { _modelname = value; }
            get { return _modelname; }
        }
        public string BLLName
        {
            set { _bllname = value; }
            get { return _bllname; }
        }
        public string DALName
        {
            set { _dalname = value; }
            get { return _dalname; }
        }  


        public string DbHelperName
        {
            set { _dbhelperName = value; }
            get { return _dbhelperName; }
        }

        /// <summary>
        /// ѡ����ֶμ���
        /// </summary>
        public List<ColumnInfo> Fieldlist
        {
            set { _fieldlist = value; }
            get { return _fieldlist; }
        }
        public List<ColumnInfo> Keys
        {
            set { _keys = value; }
            get { return _keys; }
        }
        /// <summary>
        /// �洢����ǰ׺ 
        /// </summary>       
        public string ProcPrefix
        {
            set { _procprefix = value; }
            get { return _procprefix; }
        }
        #endregion

        #region ��������

        private string _modelpath;
        private string _dalpath;
        private string _idalpath;
        private string _bllpath;       
        private string _factoryclass;    


        /// <summary>
        /// ʵ����������ռ�
        /// </summary>
        public string Modelpath
        {
            set { _modelpath = value; }
            get
            {
                _modelpath = _namespace + "." + "Model";
                if (_folder.Trim() != "")
                {
                    _modelpath += "." + _folder;
                }
                return _modelpath;
            }
        }
        /// <summary>
        /// ʵ��������������ռ�+����
        /// </summary>
        public string ModelSpace
        {            
            get
            {
                return Modelpath + "." + ModelName; 
            }
        }


        /// <summary>
        /// ���ݲ�������ռ�
        /// </summary>
        public string DALpath
        {
            set { _dalpath = value; }
            get
            {
                string dalpath = _namespace + "." + _dbtype + "DAL";
                if (_folder.Trim() != "")
                {
                    dalpath += "." + _folder;
                }
                return dalpath;
            }
        }


        /// <summary>
        /// �ӿڵ������ռ�
        /// </summary>
        public string IDALpath
        {
            get
            {
                _idalpath = _namespace + "." + "IDAL";
                if (_folder.Trim() != "")
                {
                    _idalpath += "." + _folder;
                }
                return _idalpath;
            }
        }
        /// <summary>
        /// �ӿ���
        /// </summary>
        public string IClass
        {
            get
            {
                return "I" + DALName;
            }
        }


        /// <summary>
        /// ҵ���߼���Ĳ��������ƶ���
        /// </summary>
        public string BLLpath
        {
            set { _bllpath = value; }
            get
            {
                string bllpath = _namespace + "." + "BLL";
                if (_folder.Trim() != "")
                {
                    bllpath += "." + _folder;
                }
                return bllpath;
            }
        }
        /// <summary>
        /// ҵ���߼���Ĳ��������ƶ���
        /// </summary>
        public string BLLSpace
        {
            get
            {
                return BLLpath + "." + BLLName;
            }
        }
        
        /// <summary>
        /// ������������ռ�
        /// </summary>
        public string Factorypath
        {
            get
            {
                string factorypath = _namespace + "." + "DALFactory";
                if (_folder.Trim() != "")
                {
                    factorypath += "." + _folder;
                }
                return factorypath;
            }
        }
        /// <summary>
        /// �����������
        /// </summary>
        public string FactoryClass
        {
            get
            {
                _factoryclass = _namespace + "." + "DALFactory";
                if (_folder.Trim() != "")
                {
                    _factoryclass += "." + _folder;
                }
                _factoryclass += "." + _modelname;
                return _factoryclass;
            }
        }
        /// <summary>
        /// �����Ƿ��б�ʶ��
        /// </summary>
        public bool IsHasIdentity
        {
            get
            {
                bool isid = false;
                if (Keys.Count > 0)
                {
                    foreach (ColumnInfo key in Keys)
                    {
                        if (key.IsIdentity)
                        {
                            isid = true;
                        }
                    }
                }
                return isid;
            }
        }
        #endregion	
               

        public CodeBuilders(IDbObject idbobj)
        {           
            dbobj = idbobj;
            DbType = idbobj.DbType;
            if (_dbhelperName == "")
            {
                switch (DbType)
                {
                    case "SQL2000":
                    case "SQL2005":
                        _dbhelperName = "DbHelperSQL";
                        break;
                    case "Oracle":
                        _dbhelperName = "DbHelperOra";
                        break;
                    case "MySQL":
                        _dbhelperName = "DbHelperMySQL";
                        break;
                    case "OleDb":
                        _dbhelperName = "DbHelperOleDb";
                        break;
                }
            }
        }

        #region  ���ɵ���ṹ����
        /// <summary>
        /// ���ɵ���ṹ����
        /// </summary>     
        public string GetCodeFrameOne(string DALtype, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List)
        {
            BuilderFrameOne cfo = new BuilderFrameOne(dbobj, DbName, TableName, ModelName,Fieldlist, Keys, NameSpace,Folder, DbHelperName);
            string strcode = cfo.GetCode(DALtype, Maxid, Exists, Add, Update, Delete, GetModel, List, ProcPrefix);            
            return strcode;
        }
        #endregion

        #region ���ɼ�����ṹ����
        
        /// <summary>
        /// ���ɼ�����ṹ���롪��Model
        /// </summary>
        /// <returns></returns>
        public string GetCodeFrameS3Model()
        {
            BuilderFrameS3 s3 = new BuilderFrameS3(dbobj, DbName, TableName, ModelName, BLLName, DALName, Fieldlist, Keys, NameSpace, Folder, DbHelperName);
            string strcode = s3.GetModelCode();
            return strcode;
        }
       
        /// <summary>
        /// ���ɼ�����ṹ���롪��DAL���ݷ��ʲ�
        /// </summary>        
        public string GetCodeFrameS3DAL(string AssemblyGuid, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List)
        {
            BuilderFrameS3 s3 = new BuilderFrameS3(dbobj, DbName, TableName, ModelName, BLLName, DALName, Fieldlist, Keys, NameSpace, Folder, DbHelperName);
            return s3.GetDALCode(AssemblyGuid, Maxid, Exists, Add, Update, Delete, GetModel, List, ProcPrefix);           
        }
       
        /// <summary>
        /// ���ɼ�����ṹ���롪��BLL���ݷ��ʲ�
        /// </summary>     
        public string GetCodeFrameS3BLL(string AssemblyGuid, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool GetModelByCache, bool List)
        {
            BuilderFrameS3 s3 = new BuilderFrameS3(dbobj, DbName, TableName, ModelName, BLLName, DALName, Fieldlist, Keys, NameSpace, Folder, DbHelperName);
            string strcode = s3.GetBLLCode(AssemblyGuid,Maxid, Exists, Add, Update, Delete, GetModel, GetModelByCache, List);
            return strcode;
        }

        #endregion

        #region ����ģʽ����
        /// <summary>
        /// ���ɹ���ģʽ����ṹ���롪��Model
        /// </summary>
        /// <returns></returns>
        public string GetCodeFrameF3Model()
        {
            BuilderFrameF3 f3 = new BuilderFrameF3(dbobj, DbName, TableName, ModelName,BLLName,DALName, Fieldlist, Keys,  NameSpace, Folder, DbHelperName);
            string strcode = f3.GetModelCode();
            return strcode;
        }

        /// <summary>
        /// ���ɹ���ģʽ����ṹ���롪��DAL���ݷ��ʲ�
        /// </summary>        
        public string GetCodeFrameF3DAL(string AssemblyGuid, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List)
        {
            BuilderFrameF3 f3 = new BuilderFrameF3(dbobj, DbName, TableName, ModelName, BLLName, DALName, Fieldlist, Keys, NameSpace, Folder, DbHelperName);
            return f3.GetDALCode(AssemblyGuid, Maxid, Exists, Add, Update, Delete, GetModel, List, ProcPrefix);            
        }

        /// <summary>
        /// ���ɹ���ģʽ����ṹ���롪��IDAL
        /// </summary>
        /// <returns></returns>
        public string GetCodeFrameF3IDAL(bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List, bool ListProc)
        {
            BuilderFrameF3 f3 = new BuilderFrameF3(dbobj, DbName, TableName, ModelName, BLLName, DALName, Fieldlist, Keys, NameSpace, Folder, DbHelperName);
            string strcode = f3.GetIDALCode(Maxid, Exists, Add, Update, Delete, GetModel, List, ListProc);
            return strcode;
        }

        /// <summary>
        /// ���ɹ���ģʽ����ṹ���롪��DALFactory
        /// </summary>
        /// <returns></returns>
        public string GetCodeFrameF3DALFactory()
        {
            BuilderFrameF3 f3 = new BuilderFrameF3(dbobj, DbName, TableName, ModelName, BLLName, DALName, Fieldlist, Keys, NameSpace, Folder, DbHelperName);
            string strcode = f3.GetDALFactoryCode();
            return strcode;
        }
        /// <summary>
        /// ���ɹ���ģʽ����ṹ���롪��DALFactory�еķ�������
        /// </summary>
        /// <returns></returns>
        public string GetCodeFrameF3DALFactoryMethod()
        {
            BuilderFrameF3 f3 = new BuilderFrameF3(dbobj, DbName, TableName, ModelName, BLLName, DALName, Fieldlist, Keys, NameSpace, Folder, DbHelperName);
            string strcode = f3.GetDALFactoryMethodCode();
            return strcode;
        }

        /// <summary>
        /// ���ɹ���ģʽ����ṹ���롪��BLL���ݷ��ʲ�
        /// </summary>     
        public string GetCodeFrameF3BLL(string AssemblyGuid, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool GetModelByCache, bool List)
        {
            BuilderFrameF3 f3 = new BuilderFrameF3(dbobj, DbName, TableName, ModelName, BLLName, DALName, Fieldlist, Keys, NameSpace, Folder, DbHelperName);
            string strcode = f3.GetBLLCode(AssemblyGuid,Maxid, Exists, Add, Update, Delete, GetModel, GetModelByCache, List, List);
            return strcode;
        }

        #endregion

        #region ����web
        /// <summary>
        /// ����BuilderWeb�ӿڶ���
        /// </summary>
        /// <param name="AssemblyGuid"></param>
        /// <returns></returns>
        public IBuilder.IBuilderWeb CreatBuilderWeb(string AssemblyGuid)
        {
            ibw = BuilderFactory.CreateWebObj(AssemblyGuid);                   
            ibw.NameSpace = NameSpace;
            ibw.Fieldlist = Fieldlist;
            ibw.Keys = Keys;
            ibw.ModelName = ModelName;
            ibw.Folder = Folder;
            ibw.BLLName = BLLName;
            return ibw;
        }

        public string GetAddAspx()
        {
            //IBuilder.IBuilderWeb bw = CreatBuilderWeb();
            if (ibw == null)
            {
                return "//��ѡ����Ч�ı�ʾ����������";
            }
            return ibw.GetAddAspx();            
        }
        public string GetAddAspxCs()
        {
            //IBuilder.IBuilderWeb bw = CreatBuilderWeb();
            if (ibw == null)
            {
                return "//��ѡ����Ч�ı�ʾ����������";
            }
            string cs = ibw.GetAddAspxCs();
            StringPlus strcode = new StringPlus();
            //strcode.AppendSpaceLine(2, "protected void Page_LoadComplete(object sender, EventArgs e)");
            //strcode.AppendSpaceLine(2, "{");
            //strcode.AppendSpaceLine(3, "(Master.FindControl(\"lblTitle\") as Label).Text = \"��Ϣ���\";");
            //strcode.AppendSpaceLine(2, "}");
            strcode.AppendSpaceLine(2, "protected void btnAdd_Click(object sender, EventArgs e)");
            strcode.AppendSpaceLine(2, "{");
            strcode.AppendSpaceLine(3, cs);
            strcode.AppendSpaceLine(2, "}");
            return strcode.ToString();

        }
        public string GetAddDesigner()
        {
            //IBuilder.IBuilderWeb bw = CreatBuilderWeb();
            if (ibw == null)
            {
                return "//��ѡ����Ч�ı�ʾ����������";
            }
            return ibw.GetAddDesigner();
        }

        //--------------------------------------------------------------

        public string GetUpdateAspx()
        {
            //IBuilder.IBuilderWeb bw = CreatBuilderWeb();
            if (ibw == null)
            {
                return "//��ѡ����Ч�ı�ʾ����������";
            }
            return ibw.GetUpdateAspx();
        }
        public string GetUpdateAspxCs()
        {
            //IBuilder.IBuilderWeb bw = CreatBuilderWeb();
            if (ibw == null)
            {
                return "//��ѡ����Ч�ı�ʾ����������";
            }
            string cs = ibw.GetUpdateAspxCs();
            string cs2 = ibw.GetUpdateShowAspxCs();
            StringPlus strcode = new StringPlus();
            //strcode.AppendSpaceLine(2, "protected void Page_LoadComplete(object sender, EventArgs e)");
            //strcode.AppendSpaceLine(2, "{");
            //strcode.AppendSpaceLine(3, "(Master.FindControl(\"lblTitle\") as Label).Text = \"��Ϣ�޸�\";");
            //strcode.AppendSpaceLine(2, "}");

            strcode.AppendSpaceLine(2, "protected void Page_Load(object sender, EventArgs e)");
            strcode.AppendSpaceLine(2, "{");
            strcode.AppendSpaceLine(3, "if (!Page.IsPostBack)");
            strcode.AppendSpaceLine(3, "{");
            strcode.AppendSpaceLine(4, "if (Request.Params[\"id\"] != null && Request.Params[\"id\"].Trim() != \"\")");
            strcode.AppendSpaceLine(4, "{");
            strcode.AppendSpaceLine(5, "string id = Request.Params[\"id\"];");
            if (_keys.Count > 0)
            {
                strcode.AppendSpaceLine(5, "//ShowInfo(" + LTP.CodeHelper.CodeCommon.GetFieldstrlist(Keys) + ");");
            }
            else
            {
                strcode.AppendSpaceLine(5, "ShowInfo();");
            }
            strcode.AppendSpaceLine(4, "}");
            strcode.AppendSpaceLine(3, "}");
            strcode.AppendSpaceLine(2, "}");
            strcode.AppendSpaceLine(3, cs2);
            strcode.AppendSpaceLine(2, "public void btnUpdate_Click(object sender, EventArgs e)");
            strcode.AppendSpaceLine(2, "{");
            strcode.AppendSpaceLine(3, cs);
            strcode.AppendSpaceLine(2, "}");
            return strcode.ToString();
        }
        public string GetUpdateDesigner()
        {
            //IBuilder.IBuilderWeb bw = CreatBuilderWeb();
            if (ibw == null)
            {
                return "//��ѡ����Ч�ı�ʾ����������";
            }
            return ibw.GetUpdateDesigner();
        }
        /// <summary>
        /// �õ��޸Ĵ���Ĵ���
        /// </summary>       
        public string GetUpdateShowAspxCs()
        {
            return ibw.GetUpdateShowAspxCs();
        }

        //--------------------------------------------------------------
        public string GetShowAspx()
        {
            //LTP.BuilderWeb.BuilderWeb bw = CreatBuilderWeb();
            if (ibw == null)
            {
                return "//��ѡ����Ч�ı�ʾ����������";
            }
            return ibw.GetShowAspx();
        }
        public string GetShowAspxCs()
        {
            //LTP.BuilderWeb.BuilderWeb bw = CreatBuilderWeb();
            if (ibw == null)
            {
                return "//��ѡ����Ч�ı�ʾ����������";
            }
            string cs = ibw.GetShowAspxCs();
            StringPlus strcode = new StringPlus();
            //strcode.AppendSpaceLine(2, "protected void Page_LoadComplete(object sender, EventArgs e)");
            //strcode.AppendSpaceLine(2, "{");
            //strcode.AppendSpaceLine(3, "(Master.FindControl(\"lblTitle\") as Label).Text = \"��ϸ��Ϣ\";");
            //strcode.AppendSpaceLine(2, "}");
            strcode.AppendSpaceLine(2, "protected void Page_Load(object sender, EventArgs e)");
            strcode.AppendSpaceLine(2, "{");
            strcode.AppendSpaceLine(3, "if (!Page.IsPostBack)");
            strcode.AppendSpaceLine(3,"{");
            strcode.AppendSpaceLine(4, "if (Request.Params[\"id\"] != null && Request.Params[\"id\"].Trim() != \"\")");
            strcode.AppendSpaceLine(4, "{");
            strcode.AppendSpaceLine(5, "string id = Request.Params[\"id\"];");
            if (_keys.Count > 0)
            {
                strcode.AppendSpaceLine(5, "//ShowInfo(" + LTP.CodeHelper.CodeCommon.GetFieldstrlist(Keys) + ");");
            }
            else
            {
                strcode.AppendSpaceLine(5, "ShowInfo();");
            }
            strcode.AppendSpaceLine(4, "}");
            strcode.AppendSpaceLine(3, "}");

            strcode.AppendSpaceLine(2, "}");
            strcode.AppendSpaceLine(2, cs);
            return strcode.ToString();
        }
        public string GetShowDesigner()
        {
            //LTP.BuilderWeb.BuilderWeb bw = CreatBuilderWeb();
            if (ibw == null)
            {
                return "//��ѡ����Ч�ı�ʾ����������";
            }
            return ibw.GetShowDesigner();
        }


        /// <summary>
        /// ��ɾ��3��ҳ�����
        /// </summary>      
        public string GetWebHtmlCode(bool ExistsKey, bool AddForm, bool UpdateForm, bool ShowForm, bool SearchForm)
        {
            StringPlus strclass = new StringPlus();
            if (AddForm)
            {
                strclass.AppendLine(" <!--******************************����ҳ�����********************************-->");
                strclass.AppendLine(GetAddAspx());
            }
            if (UpdateForm)
            {
                strclass.AppendLine(" <!--******************************�޸�ҳ�����********************************-->");
                strclass.AppendLine(GetUpdateAspx());
            }
            if (ShowForm)
            {
                strclass.AppendLine("  <!--******************************��ʾҳ�����********************************-->");
                strclass.AppendLine(GetShowAspx());
            }
            return strclass.ToString();
        }

        /// <summary>
        /// ���ɱ�ʾ��ҳ���CS����
        /// </summary>
        /// <param name="ExistsKey"></param>
        /// <param name="AddForm">�Ƿ��������Ӵ���Ĵ���</param>
        /// <param name="UpdateForm">�Ƿ������޸Ĵ���Ĵ���</param>
        /// <param name="ShowForm">�Ƿ�������ʾ����Ĵ���</param>
        /// <param name="SearchForm">�Ƿ����ɲ�ѯ����Ĵ���</param>
        /// <returns></returns>
        public string GetWebCode(bool ExistsKey, bool AddForm, bool UpdateForm, bool ShowForm, bool SearchForm)
        {
            StringPlus strclass = new StringPlus();
            if (AddForm)
            {
                strclass.AppendLine("  /******************************���Ӵ������********************************/");
                strclass.AppendLine(GetAddAspxCs());
            }
            if (UpdateForm)
            {
                strclass.AppendLine("  /******************************�޸Ĵ������********************************/");
                //strclass.AppendLine("  /*�޸Ĵ���-��ʾ */");
                //strclass.AppendLine(GetUpdateShowAspxCs());
                strclass.AppendLine("  /*�޸Ĵ���-�ύ���� */");
                strclass.AppendLine(GetUpdateAspxCs());
            }
            if (ShowForm)
            {
                strclass.AppendLine("  /******************************��ʾ�������********************************/");
                strclass.AppendLine(GetShowAspxCs());
            }
            //if (DelForm)
            //{
            //    strclass.Append("  /******************************ɾ���������********************************/" );
            //    strclass.Append("");
            //    strclass.Append(CreatDeleteForm() );
            //}
            return strclass.Value;
        }
        #endregion

        #region MapXMLs
        //public string GetMapXMLs()
        //{
        //    LTP.BuilderWeb.BuilderWeb bw = CreatBuilderWeb();
        //    return bw.GetMapXMLs();
        //}

        #endregion



    }
}
