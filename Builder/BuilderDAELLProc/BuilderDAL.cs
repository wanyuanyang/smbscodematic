using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using LTP.Utility;
using LTP.IDBO;
using LTP.CodeHelper;
namespace LTP.BuilderDALELProc
{
    /// <summary>
    /// ���ݷ��ʲ���빹�������洢���̷�ʽ��
    /// </summary>
    public class BuilderDAL : LTP.IBuilder.IBuilderDAL
    {
        #region ˽�б���
        protected string _key = "ID";//��ʶ�У��������ֶ�		
        protected string _keyType = "int";//��ʶ�У��������ֶ�����        
        #endregion

        #region ��������
        IDbObject dbobj;
        private string _dbname;
        private string _tablename;
        private string _modelname; //model����
        private string _dalname;//dal����    
        private List<ColumnInfo> _fieldlist;
        private List<ColumnInfo> _keys; //�����������ֶ��б�        
        private string _namespace; //���������ռ���
        private string _folder; //�����ļ���
        private string _dbhelperName;//���ݿ��������           
        private string _modelpath;
        private string _dalpath;
        private string _idalpath;
        private string _iclass;
        private string _procprefix;

        public IDbObject DbObject
        {
            set { dbobj = value; }
            get { return dbobj; }
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
        
        /// <summary>
        /// ѡ����ֶμ���
        /// </summary>
        public List<ColumnInfo> Fieldlist
        {
            set { _fieldlist = value; }
            get { return _fieldlist; }
        }
        /// <summary>
        /// �����������ֶ�
        /// </summary>
        public List<ColumnInfo> Keys
        {
            set { _keys = value; }
            get { return _keys; }
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
        /*============================*/
        /// <summary>
        /// ʵ����������ռ�
        /// </summary>
        public string Modelpath
        {
            set { _modelpath = value; }
            get { return _modelpath; }
        }
        public string ModelName
        {
            set { _modelname = value; }
            get { return _modelname; }
        }
        /// <summary>
        /// ʵ��������������ռ� + ������������ Modelpath+ModelName
        /// </summary>
        public string ModelSpace
        {
            get { return Modelpath + "." + ModelName; }
        }
        
        /*============================*/

        /// <summary>
        /// ���ݲ�������ռ�
        /// </summary>
        public string DALpath
        {
            set { _dalpath = value; }
            get
            {
                return _dalpath;
            }
        }
        public string DALName
        {
            set { _dalname = value; }
            get { return _dalname; }
        }
        /*============================*/
        
        /// <summary>
        /// �ӿڵ������ռ�
        /// </summary>
        public string IDALpath
        {
            set { _idalpath = value; }
            get
            {
                return _idalpath;
            }
        }
        /// <summary>
        /// �ӿ�����
        /// </summary>
        public string IClass
        {
            set { _iclass = value; }
            get
            {
                return _iclass;
            }
        }
        /*============================*/

        public string DbHelperName
        {
            set { _dbhelperName = value; }
            get { return _dbhelperName; }
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

        /// <summary>
        /// ѡ����ֶμ��ϵ�-�ַ���
        /// </summary>
        public string Fields
        {
            get
            {
                StringPlus _fields = new StringPlus();
                foreach (ColumnInfo obj in Fieldlist)
                {
                    _fields.Append("'" + obj.ColumnName + "',");
                }
                _fields.DelLastComma();
                return _fields.Value;
            }
        }
        /// <summary>
        /// �ֶε� select * �б�
        /// </summary>
        public string Fieldstrlist
        {
            get
            {
                StringPlus _fields = new StringPlus();
                foreach (ColumnInfo obj in Fieldlist)
                {
                    _fields.Append(obj.ColumnName + ",");
                }
                _fields.DelLastComma();
                return _fields.Value;
            }
        }
        
         /// <summary>
        ///  ��ͬ���ݿ��ֶ�����
        /// </summary>
        public string DbParaDbType
        {
            get
            {
                return "DbType";
            }
        }
        

        /// <summary>
        /// �洢���̲��� ���÷���@
        /// </summary>
        public string preParameter
        {
            get
            {               
               return "@";
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
                if (_keys.Count > 0)
                {
                    foreach (ColumnInfo key in _keys)
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

        #region  ��������Ϣ �õ��������б�

        /// <summary>
        /// �õ�Where������� - Parameter��ʽ (���磺����Exists  Delete  GetModel ��where)
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public string GetWhereExpression(List<ColumnInfo> keys)
        {
            StringPlus strclass = new StringPlus();
            foreach (ColumnInfo key in keys)
            {
                strclass.Append(key.ColumnName + "=" + preParameter + key.ColumnName + " and ");
            }
            strclass.DelLastChar("and");
            return strclass.Value;
        }

        /// <summary>
        /// ����sql����еĲ����б�(���磺����Add  Exists  Update Delete  GetModel �Ĳ�������)
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public string GetPreParameter(List<ColumnInfo> keys)
        {
            StringPlus strclass = new StringPlus();
            foreach (ColumnInfo key in keys)
            {
                strclass.AppendSpaceLine(3, "db.AddInParameter(dbCommand, \"" + key.ColumnName + "\", DbType." + CSToProcType(key.TypeName) + "," + key.ColumnName + ");");
            }
            return strclass.Value;
        }

        #endregion


        #region ���캯��

        public BuilderDAL()
        {
        }
        public BuilderDAL(IDbObject idbobj)
        {
            dbobj = idbobj;
        }

        public BuilderDAL(IDbObject idbobj, string dbname, string tablename, string modelname, string dalName, 
            List<ColumnInfo> fieldlist, List<ColumnInfo> keys, string namepace,
            string folder, string dbherlpername, string modelpath, string modelspace,
            string dalpath, string idalpath, string iclass)
        {
            dbobj = idbobj;
            _dbname = dbname;
            _tablename = tablename;
            _modelname = modelname;
            _dalname = dalName;
            _namespace = namepace;
            _folder = folder;
            _dbhelperName = dbherlpername;
            _modelpath = modelpath;
            
            _dalpath = dalpath;
            _idalpath = idalpath;
            _iclass = iclass;
            Fieldlist = fieldlist;
            Keys = keys;
            foreach (ColumnInfo key in _keys)
            {
                _key = key.ColumnName;
                _keyType = key.TypeName;
                if (key.IsIdentity)
                {
                    _key = key.ColumnName;
                    _keyType = CodeCommon.DbTypeToCS(key.TypeName);
                    break;
                }
            }
        }

        #endregion        
        

        #region ���ݲ�(������)

        public string GetDALCode(bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List)
        {
            StringPlus strclass = new StringPlus();
            strclass.AppendLine("using System;");
            strclass.AppendLine("using System.Data;");
            strclass.AppendLine("using System.Text;");
            strclass.AppendLine("using System.Collections.Generic;");
            strclass.AppendLine("using Microsoft.Practices.EnterpriseLibrary.Data;");
            strclass.AppendLine("using Microsoft.Practices.EnterpriseLibrary.Data.Sql;");
            strclass.AppendLine("using System.Data.Common;");
            if (IDALpath != "")
            {
                strclass.AppendLine("using " + IDALpath + ";");
            }
            strclass.AppendLine("using Maticsoft.DBUtility;//�����������");
            strclass.AppendLine("namespace " + DALpath);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// ���ݷ�����" + DALName + "��");
            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpace(1, "public class " + DALName);
            if (IClass != "")
            {
                strclass.Append(":" + IClass);
            }
            strclass.AppendLine("");
            strclass.AppendSpaceLine(1, "{");
            strclass.AppendSpaceLine(2, "public " + DALName + "()");
            strclass.AppendSpaceLine(2, "{}");
            strclass.AppendSpaceLine(2, "#region  ��Ա����");

            #region  ��������
            if (Maxid)
            {
                strclass.AppendLine(CreatGetMaxID());
            }
            if (Exists)
            {
                strclass.AppendLine(CreatExists());
            }
            if (Add)
            {
                strclass.AppendLine(CreatAdd());
            }
            if (Update)
            {
                strclass.AppendLine(CreatUpdate());
            }
            if (Delete)
            {
                strclass.AppendLine(CreatDelete());
            }
            if (GetModel)
            {
                strclass.AppendLine(CreatGetModel());
            }
            if (List)
            {
                strclass.AppendLine(CreatGetList());
                strclass.AppendLine(CreatGetListByPageProc());
                strclass.AppendLine(CreatGetListArray());
                strclass.AppendLine(CreatReaderBind());
            }
            #endregion

            strclass.AppendSpaceLine(2, "#endregion  ��Ա����");
            strclass.AppendSpaceLine(1, "}");
            strclass.AppendLine("}");
            strclass.AppendLine("");

            return strclass.ToString();
        
        }

        #endregion


        #region ���ݲ�(ʹ�ô洢����ʵ��)

        /// <summary>
        /// �õ����ID�ķ�������
        /// </summary>
        /// <param name="TabName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string CreatGetMaxID()
        {
            StringPlus strclass = new StringPlus();
            if (_keys.Count > 0)
            {
                string keyname = "";
                foreach (ColumnInfo obj in _keys)
                {
                    if (CodeCommon.DbTypeToCS(obj.TypeName) == "int")
                    {
                        keyname = obj.ColumnName;
                        if (obj.IsPK)
                        {
                            strclass.AppendLine("");
                            strclass.AppendSpaceLine(2, "/// <summary>");
                            strclass.AppendSpaceLine(2, "/// �õ����ID");
                            strclass.AppendSpaceLine(2, "/// </summary>");
                            strclass.AppendSpaceLine(2, "public int GetMaxId()");
                            strclass.AppendSpaceLine(2, "{");
                            strclass.AppendSpaceLine(3, "string strsql = \"select max(" + keyname + ")+1 from " + _tablename + "\";");
                            strclass.AppendSpaceLine(3, "Database db = DatabaseFactory.CreateDatabase();");
                            strclass.AppendSpaceLine(3, "object obj = db.ExecuteScalar(CommandType.Text, strsql);");
                            strclass.AppendSpaceLine(3, "if (obj != null && obj != DBNull.Value)");
                            strclass.AppendSpaceLine(3, "{");
                            strclass.AppendSpaceLine(4, "return int.Parse(obj.ToString());");
                            strclass.AppendSpaceLine(3, "}");
                            strclass.AppendSpaceLine(3, "return 1;");
                            strclass.AppendSpaceLine(2, "}");
                            break;
                        }
                    }
                }
            }
            return strclass.ToString();
        }

        /// <summary>
        /// �õ�Exists�����Ĵ���
        /// </summary>
        /// <param name="_tablename"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public string CreatExists()
        {
            StringPlus strclass = new StringPlus();
            if (_keys.Count > 0)
            {
                strclass.AppendSpaceLine(2, "/// <summary>");
                strclass.AppendSpaceLine(2, "/// �Ƿ���ڸü�¼");
                strclass.AppendSpaceLine(2, "/// </summary>");
                strclass.AppendSpaceLine(2, "public bool Exists(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
                strclass.AppendSpaceLine(2, "{");

                strclass.AppendSpaceLine(3, "Database db = DatabaseFactory.CreateDatabase();");
                strclass.AppendSpaceLine(3, "DbCommand dbCommand = db.GetStoredProcCommand(\"" + ProcPrefix + _tablename + "_Exists\");");
                strclass.Append(GetPreParameter(Keys));
                strclass.AppendSpaceLine(3, "int result;");
                strclass.AppendSpaceLine(3, "object obj = db.ExecuteScalar(dbCommand);");
                strclass.AppendSpaceLine(3, "int.TryParse(obj.ToString(),out result);");
                strclass.AppendSpaceLine(3, "if(result==1)");
                strclass.AppendSpaceLine(3, "{");
                strclass.AppendSpaceLine(4, "return true;");
                strclass.AppendSpaceLine(3, "}");
                strclass.AppendSpaceLine(3, "else");
                strclass.AppendSpaceLine(3, "{");
                strclass.AppendSpaceLine(4, "return false;");
                strclass.AppendSpaceLine(3, "}");
                strclass.AppendSpaceLine(2, "}");
            }
            return strclass.Value;
        }

        /// <summary>
        /// �õ�Add()�Ĵ���
        /// </summary>
        /// <param name="ExistsMaxId">�Ƿ���GetMaxId()��������</param>	
        public string CreatAdd()
        {

            //if (ModelSpace == "")
            //{
            //    ModelSpace = "ModelClassName"; ;
            //}
            StringPlus strclass = new StringPlus();
            StringPlus strclass2 = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "///  ����һ������");
            strclass.AppendSpaceLine(2, "/// </summary>");
            string strretu = "void";
            if (IsHasIdentity)
            {
                strretu = "int";
            }
            //��������ͷ
            string strFun = CodeCommon.Space(2) + "public " + strretu + " Add(" + ModelSpace + " model)";
            strclass.AppendLine(strFun );                        
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "Database db = DatabaseFactory.CreateDatabase();");
            strclass.AppendSpaceLine(3, "DbCommand dbCommand = db.GetStoredProcCommand(\"" + ProcPrefix + _tablename + "_ADD\");");

            string nkey = string.Empty; ;
           
            foreach (ColumnInfo field in Fieldlist)
                {
                    string columnName = field.ColumnName;
                    string columnType = field.TypeName;
                    bool IsIdentity = field.IsIdentity;
                    string Length = field.Length;

                    if (field.IsIdentity)
                    {                       
                        strclass.AppendSpaceLine(3, "db.AddOutParameter(dbCommand, \"" + columnName + "\", DbType." + CSToProcType(columnType) + ", " + Length + ");");
                        nkey = columnName;
                    }
                    else
                    {
                        strclass.AppendSpaceLine(3, "db.AddInParameter(dbCommand, \"" + columnName + "\", DbType." + CSToProcType(columnType) + ", model." + columnName + ");");
                    }                 
            }

            strclass.AppendSpaceLine(3, "db.ExecuteNonQuery(dbCommand);"); 
          
  
            //���¶��巽��ͷ
            if (IsHasIdentity)
            {
                strclass.AppendSpaceLine(3, "return (" + _keyType + ")db.GetParameterValue(dbCommand, \"" + nkey + "\");");
            }            
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;

        }

        /// <summary>
        /// �õ�Update�����Ĵ���
        /// </summary>
        /// <param name="DbName"></param>
        /// <param name="_tablename"></param>
        /// <param name="_key"></param>
        /// <param name="ModelName"></param>
        /// <returns></returns>
        public string CreatUpdate()
        {            
            //if (ModelSpace == "")
            //{
            //    ModelSpace = "ModelClassName"; ;
            //}
            StringPlus strclass = new StringPlus();
            StringPlus strclass2 = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "///  ����һ������");
            strclass.AppendSpaceLine(2, "/// </summary>");

            strclass.AppendSpaceLine(2, "public void Update(" + ModelSpace + " model)");
            strclass.AppendSpaceLine(2, "{");

            strclass.AppendSpaceLine(3, "Database db = DatabaseFactory.CreateDatabase();");
            strclass.AppendSpaceLine(3, "DbCommand dbCommand = db.GetStoredProcCommand(\"" + ProcPrefix + _tablename + "_Update\");");

            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                string Length = field.Length;

                strclass.AppendSpaceLine(3, "db.AddInParameter(dbCommand, \"" + columnName + "\", DbType." + CSToProcType(columnType) + ", model." + columnName + ");");
            }
            strclass.AppendSpaceLine(3, "db.ExecuteNonQuery(dbCommand);");             
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;

        }
        /// <summary>
        /// �õ�Delete�Ĵ���
        /// </summary>
        /// <param name="_tablename"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public string CreatDelete()
        {
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ɾ��һ������");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public void Delete(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{");

            strclass.AppendSpaceLine(3, "Database db = DatabaseFactory.CreateDatabase();");
            strclass.AppendSpaceLine(3, "DbCommand dbCommand = db.GetStoredProcCommand(\"" + ProcPrefix + _tablename + "_Delete\");");

            strclass.AppendLine(GetPreParameter(Keys));
            strclass.AppendSpaceLine(3, "db.ExecuteNonQuery(dbCommand);");  
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;

        }

        /// <summary>
        /// �õ�GetModel()�Ĵ���
        /// </summary>
        /// <param name="DbName"></param>
        /// <param name="_tablename"></param>
        /// <param name="_key"></param>
        /// <param name="ModelName"></param>
        /// <returns></returns>
        public string CreatGetModel()
        {
            //if (ModelSpace == "")
            //{
            //    ModelSpace = "ModelClassName"; ;
            //}
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// �õ�һ������ʵ��");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public " + ModelSpace + " GetModel(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{");

            strclass.AppendSpaceLine(3, "Database db = DatabaseFactory.CreateDatabase();");
            strclass.AppendSpaceLine(3, "DbCommand dbCommand = db.GetStoredProcCommand(\"" + ProcPrefix + _tablename + "_GetModel\");");

            strclass.AppendLine(GetPreParameter(Keys));
            strclass.AppendSpaceLine(3, "" + ModelSpace + " model=null;");
            strclass.AppendSpaceLine(3, "using (IDataReader dataReader = db.ExecuteReader(dbCommand))");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "if(dataReader.Read())");
            strclass.AppendSpaceLine(4, "{");
            strclass.AppendSpaceLine(5, "model=ReaderBind(dataReader);");
            strclass.AppendSpaceLine(4, "}");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "return model;");
            strclass.AppendSpaceLine(2, "}");

            return strclass.Value;
        }

        /// <summary>
        /// �õ�GetList()�Ĵ���
        /// </summary>
        /// <param name="_tablename"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public string CreatGetList()
        {
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ��������б�");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetList(string strWhere)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpace(3, "strSql.Append(\"select ");
            strclass.AppendLine(Fieldstrlist + " \");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" FROM " + TableName + " \");");
            strclass.AppendSpaceLine(3, "if(strWhere.Trim()!=\"\")");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "strSql.Append(\" where \"+strWhere);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "Database db = DatabaseFactory.CreateDatabase();");
            strclass.AppendSpaceLine(3, "return db.ExecuteDataSet(CommandType.Text, strSql.ToString());");
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;

        }
        /// <summary>
        /// �õ�GetList()�Ĵ���
        /// </summary>
        /// <param name="_tablename"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public string CreatGetListByPageProc()
        {
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/*");
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ��ҳ��ȡ�����б�");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetList(int PageSize,int PageIndex,string strWhere)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "Database db = DatabaseFactory.CreateDatabase();");
            strclass.AppendSpaceLine(3, "DbCommand dbCommand = db.GetStoredProcCommand(\"UP_GetRecordByPage\");");
            strclass.AppendSpaceLine(3, "db.AddInParameter(dbCommand, \"tblName\", DbType.AnsiString, \"" + TableName + "\");");
            strclass.AppendSpaceLine(3, "db.AddInParameter(dbCommand, \"fldName\", DbType.AnsiString, \"" + _key + "\");");
            strclass.AppendSpaceLine(3, "db.AddInParameter(dbCommand, \"PageSize\", DbType.Int32, PageSize);");
            strclass.AppendSpaceLine(3, "db.AddInParameter(dbCommand, \"PageIndex\", DbType.Int32, PageIndex);");
            strclass.AppendSpaceLine(3, "db.AddInParameter(dbCommand, \"IsReCount\", DbType.Boolean, 0);");
            strclass.AppendSpaceLine(3, "db.AddInParameter(dbCommand, \"OrderType\", DbType.Boolean, 0);");
            strclass.AppendSpaceLine(3, "db.AddInParameter(dbCommand, \"strWhere\", DbType.AnsiString, strWhere);");
            strclass.AppendSpaceLine(3, "return db.ExecuteDataSet(dbCommand);");
            strclass.AppendSpaceLine(2, "}*/");
            return strclass.Value;
        }


        #region  ���ɶ���ʵ�������

        /// <summary>
        /// ���ɶ���ʵ�������
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string CreatReaderBind()
        {
            //if (ModelSpace == "")
            //{
            //    ModelSpace = "ModelClassName"; ;
            //}
            StringPlus strclass = new StringPlus();
            StringPlus strclass1 = new StringPlus();
            strclass.AppendLine("");
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ����ʵ�������");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public " + ModelSpace + " ReaderBind(IDataReader dataReader)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, ModelSpace + " model=new " + ModelSpace + "();");

            bool isobj = false;

            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                bool IsIdentity = field.IsIdentity;
                string Length = field.Length;

                switch (CodeCommon.DbTypeToCS(columnType))
                {
                    case "int":
                        {
                            isobj = true;
                            strclass1.AppendSpaceLine(3, "ojb = dataReader[\"" + columnName + "\"];");
                            strclass1.AppendSpaceLine(3, "if(ojb != null && ojb != DBNull.Value)");
                            strclass1.AppendSpaceLine(3, "{");
                            strclass1.AppendSpaceLine(4, "model." + columnName + "=(int)ojb;");
                            strclass1.AppendSpaceLine(3, "}");
                        }
                        break;
                    case "long":
                        {
                            isobj = true;
                            strclass1.AppendSpaceLine(3, "ojb = dataReader[\"" + columnName + "\"];");
                            strclass1.AppendSpaceLine(3, "if(ojb != null && ojb != DBNull.Value)");
                            strclass1.AppendSpaceLine(3, "{");
                            strclass1.AppendSpaceLine(4, "model." + columnName + "=(long)ojb;");
                            strclass1.AppendSpaceLine(3, "}");
                        }
                        break;
                    case "decimal":
                        {
                            isobj = true;
                            strclass1.AppendSpaceLine(3, "ojb = dataReader[\"" + columnName + "\"];");
                            strclass1.AppendSpaceLine(3, "if(ojb != null && ojb != DBNull.Value)");

                            strclass1.AppendSpaceLine(3, "{");
                            strclass1.AppendSpaceLine(4, "model." + columnName + "=(decimal)ojb;");
                            strclass1.AppendSpaceLine(3, "}");
                        }
                        break;
                    case "DateTime":
                        {
                            isobj = true;
                            strclass1.AppendSpaceLine(3, "ojb = dataReader[\"" + columnName + "\"];");
                            strclass1.AppendSpaceLine(3, "if(ojb != null && ojb != DBNull.Value)");
                            strclass1.AppendSpaceLine(3, "{");
                            strclass1.AppendSpaceLine(4, "model." + columnName + "=(DateTime)ojb;");
                            strclass1.AppendSpaceLine(3, "}");
                        }
                        break;
                    case "string":
                        {
                            strclass1.AppendSpaceLine(3, "model." + columnName + "=dataReader[\"" + columnName + "\"].ToString();");
                        }
                        break;
                    case "bool":
                        {
                            isobj = true;
                            strclass1.AppendSpaceLine(3, "ojb = dataReader[\"" + columnName + "\"];");
                            strclass1.AppendSpaceLine(3, "if(ojb != null && ojb != DBNull.Value)");

                            strclass1.AppendSpaceLine(3, "{");
                            strclass1.AppendSpaceLine(4, "model." + columnName + "=(bool)ojb;");
                            strclass1.AppendSpaceLine(3, "}");
                        }
                        break;
                    case "byte[]":
                        {
                            isobj = true;
                            strclass1.AppendSpaceLine(3, "ojb = dataReader[\"" + columnName + "\"];");
                            strclass1.AppendSpaceLine(3, "if(ojb != null && ojb != DBNull.Value)");

                            strclass1.AppendSpaceLine(3, "{");
                            strclass1.AppendSpaceLine(4, "model." + columnName + "=(byte[])ojb;");
                            strclass1.AppendSpaceLine(3, "}");
                        }
                        break;
                    case "Guid":
                        {
                            isobj = true;
                            strclass1.AppendSpaceLine(3, "ojb = dataReader[\"" + columnName + "\"];");
                            strclass1.AppendSpaceLine(3, "if(ojb != null && ojb != DBNull.Value)");
                            strclass1.AppendSpaceLine(3, "{");
                            strclass1.AppendSpaceLine(4, "model." + columnName + "= new Guid(ojb.ToString());");
                            strclass1.AppendSpaceLine(3, "}");
                        }
                        break;
                    default:
                        strclass1.AppendSpaceLine(3, "model." + columnName + "=dataReader[\"" + columnName + "\"].ToString();\r\n");
                        break;
                }
            }
            if (isobj)
            {
                strclass.AppendSpaceLine(3, "object ojb; ");
            } 
            strclass.Append(strclass1.ToString());
            strclass.AppendSpaceLine(3, "return model;");
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;
        }


        public string CreatGetListArray()
        {
            string strList = "List<" + ModelSpace + ">";
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ��������б���DataSetЧ�ʸߣ��Ƽ�ʹ�ã�");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public " + strList + " GetListArray(string strWhere)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpace(3, "strSql.Append(\"select ");
            strclass.AppendLine(Fieldstrlist + " \");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" FROM " + TableName + " \");");
            strclass.AppendSpaceLine(3, "if(strWhere.Trim()!=\"\")");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "strSql.Append(\" where \"+strWhere);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, strList + " list = new " + strList + "();");
            strclass.AppendSpaceLine(3, "Database db = DatabaseFactory.CreateDatabase();");
            strclass.AppendSpaceLine(3, "using (IDataReader dataReader = db.ExecuteReader(CommandType.Text, strSql.ToString()))");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "while (dataReader.Read())");
            strclass.AppendSpaceLine(4, "{");
            strclass.AppendSpaceLine(5, "list.Add(ReaderBind(dataReader));");
            strclass.AppendSpaceLine(4, "}");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "return list;");
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;
        }


        #endregion

        #endregion//���ݲ�


        #region CSToProcType
        /// <summary>
        /// ��ҵ�����ݿ��ֶζ�Ӧ
        /// </summary>
        /// <param name="cstype"></param>
        /// <returns></returns>
        private static string CSToProcType(string cstype)
        {
            string ProcType = cstype;
            switch (cstype.Trim().ToLower())
            {

                case "string":
                case "nvarchar":
                case "nchar":
                case "ntext":
                    ProcType = "String";
                    break;
                case "text":
                case "char":
                case "varchar":
                    ProcType = "AnsiString";
                    break;
                case "datetime":
                case "smalldatetime":
                    ProcType = "DateTime";
                    break;
                case "smallint":
                    ProcType = "Int16";
                    break;
                case "tinyint":
                    ProcType = "Byte";
                    break;
                case "int":
                    ProcType = "Int32";
                    break;
                case "bigint":
                case "long":
                    ProcType = "Int64";
                    break;
                case "float":
                    ProcType = "Double";
                    break;
                case "real":
                case "numeric":
                case "decimal":
                    ProcType = "Decimal";
                    break;
                case "money":
                case "smallmoney":
                    ProcType = "Currency";
                    break;
                case "bool":
                case "bit":
                    ProcType = "Boolean";
                    break;
                case "binary":
                case "varbinary":
                    ProcType = "Binary";
                    break;
                case "image":
                    ProcType = "Image";
                    break;
                case "uniqueidentifier":
                    ProcType = "Guid";
                    break;
                case "timestamp":
                    ProcType = "String";
                    break;
                default:
                    ProcType = "String";
                    break;
            }
            return ProcType;
        }

        #endregion

        

    }
}
