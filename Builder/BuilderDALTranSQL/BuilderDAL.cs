using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Data.SqlClient;
using LTP.Utility;
using LTP.IDBO;
using LTP.CodeHelper;
namespace LTP.BuilderDALTranSQL
{
    /// <summary>
    /// ���ݷ��ʲ���빹������SQL��ʽ��
    /// </summary>
    public class BuilderDAL : LTP.IBuilder.IBuilderDAL
    {

        #region ��������
        IDbObject dbobj;
        private string _dbname;
        private string _tablenameparent;
        private string _tablenameson;
        private List<ColumnInfo> _fieldlistparent;
        private List<ColumnInfo> _keysparent; // �����������ֶ��б�      
        private List<ColumnInfo> _fieldlistson;
        private List<ColumnInfo> _keysson; // �����������ֶ��б�                
        private string _namespace; //���������ռ���
        private string _folder; //�����ļ���               
        private string _modelpath;
        private string _modelnameparent;
        private string _modelnameson;
        private string _dalpath;
        private string _dalnameparent;
        private string _dalnameson;
        private string _idalpath;
        private string _iclass;
        private string _dbhelperName;//���ݿ��������    
        private string _procprefix;

        public IDbObject DbObject
        {
            set { dbobj = value; }
            get { return dbobj; }
        }
        /// <summary>
        /// ����
        /// </summary>
        public string DbName
        {
            set { _dbname = value; }
            get { return _dbname; }
        }
        /// <summary>
        /// ����
        /// </summary>
        public string TableNameParent
        {
            set { _tablenameparent = value; }
            get { return _tablenameparent; }
        }
        /// <summary>
        /// ����
        /// </summary>
        public string TableNameSon
        {
            set { _tablenameson = value; }
            get { return _tablenameson; }
        }

        /// <summary>
        /// ѡ����ֶμ���
        /// </summary>
        public List<ColumnInfo> FieldlistParent
        {
            set { _fieldlistparent = value; }
            get { return _fieldlistparent; }
        }
        /// <summary>
        /// ѡ����ֶμ���
        /// </summary>
        public List<ColumnInfo> FieldlistSon
        {
            set { _fieldlistson = value; }
            get { return _fieldlistson; }
        }
        /// <summary>
        /// �����������ֶεļ���
        /// </summary>
        public List<ColumnInfo> KeysParent
        {
            set { _keysparent = value; }
            get { return _keysparent; }
        }
        /// <summary>
        /// �����������ֶεļ���
        /// </summary>
        public List<ColumnInfo> KeysSon
        {
            set { _keysson = value; }
            get { return _keysson; }
        }
        /// <summary>
        /// ���������ռ���
        /// </summary>
        public string NameSpace
        {
            set { _namespace = value; }
            get { return _namespace; }
        }
        /// <summary>
        /// �����ļ���
        /// </summary>
        public string Folder
        {
            set { _folder = value; }
            get { return _folder; }
        }

        /// <summary>
        /// ʵ����������ռ�
        /// </summary>
        public string Modelpath
        {
            set { _modelpath = value; }
            get { return _modelpath; }
        }
        /// <summary>
        /// Model����(����)
        /// </summary>
        public string ModelNameParent
        {
            set { _modelnameparent = value; }
            get { return _modelnameparent; }
        }
        /// <summary>
        ///  Model����(����)
        /// </summary>
        public string ModelNameSon
        {
            set { _modelnameson = value; }
            get { return _modelnameson; }
        }
        /// <summary>
        /// ʵ��������������ռ� + ����
        /// </summary>
        public string ModelSpaceParent
        {
            get { return Modelpath + "." + ModelNameParent; }
        }
        /// <summary>
        /// ʵ��������������ռ� + ����
        /// </summary>
        public string ModelSpaceSon
        {
            get { return Modelpath + "." + ModelNameSon; }
        }
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
        /// <summary>
        /// ���ݲ������(����)
        /// </summary>
        public string DALNameParent
        {
            set { _dalnameparent = value; }
            get { return _dalnameparent; }
        }
        /// <summary>
        /// ���ݲ������(����)
        /// </summary>
        public string DALNameSon
        {
            set { _dalnameson = value; }
            get { return _dalnameson; }
        }

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
        /// <summary>
        /// ���ݿ��������
        /// </summary>
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
        /// ��ͬ���ݿ����ǰ׺
        /// </summary>
        public string DbParaHead
        {
            get
            {
                switch (dbobj.DbType)
                {
                    case "SQL2000":
                    case "SQL2005":
                        return "Sql";
                    case "Oracle":
                        return "Oracle";
                    case "OleDb":
                        return "OleDb";
                    default:
                        return "Sql";
                }
            }

        }
        /// <summary>
        ///  ��ͬ���ݿ��ֶ�����
        /// </summary>
        public string DbParaDbType
        {
            get
            {
                switch (dbobj.DbType)
                {
                    case "SQL2000":
                    case "SQL2005":
                        return "SqlDbType";
                    case "Oracle":
                        return "OracleType";
                    case "OleDb":
                        return "OleDbType";
                    default:
                        return "SqlDbType";
                }
            }
        }

        /// <summary>
        /// �洢���̲��� ���÷���@
        /// </summary>
        public string preParameter
        {
            get
            {
                switch (dbobj.DbType)
                {
                    case "SQL2000":
                    case "SQL2005":
                        return "@";
                    case "Oracle":
                        return ":";
                    //case "OleDb":
                    // break;
                    default:
                        return "@";

                }
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

        #region ���캯��

        public BuilderDAL()
        {            
        }
        public BuilderDAL(IDbObject idbobj)
        {
            dbobj = idbobj;            
        }
        public BuilderDAL(IDbObject idbobj, string dbname, string tablename, string modelname, List<ColumnInfo> fieldlist, List<ColumnInfo> keys, string namepace,
            string folder, string dbherlpername, string modelpath, string modelspace,
            string dalpath, string idalpath, string iclass)
        {
            dbobj = idbobj;            
            _dbname = dbname;
            _tablename = tablename;
            _modelname = modelname;
            _namespace = namepace;
            _folder = folder;
            _dbhelperName = dbherlpername;            
            _modelpath = modelpath;
            _modelspace = modelspace;
            _dalpath = dalpath;
            _idalpath = idalpath;
            _iclass = iclass;
            Fieldlist = fieldlist;
            Keys = keys;
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
            switch (dbobj.DbType)
            {
                case "SQL2005":
                    strclass.AppendLine("using System.Data.SqlClient;");
                    break;
                case "SQL2000":
                    strclass.AppendLine("using System.Data.SqlClient;");
                    break;
                case "Oracle":
                    strclass.AppendLine("using System.Data.OracleClient;");
                    break;
                case "OleDb":
                    strclass.AppendLine("using System.Data.OleDb;");
                    break;
            }
            if (IDALpath != "")
            {
                strclass.AppendLine("using " + IDALpath + ";");
            }
            strclass.AppendLine("using Maticsoft.DBUtility;//�����������");
            strclass.AppendLine("namespace " + DALpath);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// ���ݷ�����" + DALNameParent + "��");
            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpace(1, "public class " + DALNameParent);
            if (IClass != "")
            {
                strclass.Append(":" + IClass);
            }
            strclass.AppendLine("");
            strclass.AppendSpaceLine(1, "{");
            strclass.AppendSpaceLine(2, "public " + DALNameParent + "()");
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
            }
            #endregion

            strclass.AppendSpaceLine(2, "#endregion  ��Ա����");
            strclass.AppendSpaceLine(1, "}");
            strclass.AppendLine("}");
            strclass.AppendLine("");

            return strclass.ToString();
        }

        #endregion


        #region ���ݲ�(����)

        /// <summary>
        /// �õ�ĳ�ֶ����ֵ�ķ�������(ֻ��������int�͵����������)
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
                    if (CodeCommon.DbTypeToCS(obj.TypeName)=="int")
                    {
                        keyname = obj.ColumnName;
                        if (obj.IsPK)
                        {
                            strclass.AppendLine("");
                            strclass.AppendSpaceLine(2, "/// <summary>");
                            strclass.AppendSpaceLine(2, "/// �õ����ID");
                            strclass.AppendSpaceLine(2, "/// </summary>");
                            strclass.AppendSpaceLine(2, "public int GetMaxId()");
                            strclass.AppendSpaceLine(2, "{" );
                            strclass.AppendSpaceLine(2, "return " + DbHelperName + ".GetMaxID(\"" + keyname + "\", \"" + _tablename + "\"); ");
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
        /// <param name="ID"></param>
        /// <returns></returns>
        public string CreatExists()
        {
            StringPlus strclass = new StringPlus();
            if (_keys.Count > 0)
            {
                strclass.AppendLine("");
                strclass.AppendSpaceLine(2, "/// <summary>");
                strclass.AppendSpaceLine(2, "/// �Ƿ���ڸü�¼");
                strclass.AppendSpaceLine(2, "/// </summary>");
                strclass.AppendSpaceLine(2, "public bool Exists(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
                strclass.AppendSpaceLine(2, "{");
                strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
                strclass.AppendSpaceLine(3, "strSql.Append(\"select count(1) from " + _tablename + "\");");
                strclass.AppendSpaceLine(3, "strSql.Append(\" where " + LTP.CodeHelper.CodeCommon.GetWhereExpression(Keys) + "\");");                
                strclass.AppendSpaceLine(3, "return " + DbHelperName + ".Exists(strSql.ToString());");
                strclass.AppendSpace(2, "}");
            }
            return strclass.ToString();
        }
        /// <summary>
        /// �õ�Add()�Ĵ���
        /// </summary>        
        public string CreatAdd()
        {
            if (ModelSpace == "")
            {
                ModelSpace = "ModelClassName"; ;
            }
            StringPlus strclass = new StringPlus();
            StringPlus strclass1 = new StringPlus();
            StringPlus strclass2 = new StringPlus();
            strclass.AppendLine("");
            strclass.AppendSpaceLine(2,"/// <summary>");
            strclass.AppendSpaceLine(2, "/// ����һ������" );
            strclass.AppendSpaceLine(2, "/// </summary>");
            string strretu = "void";
            if ((dbobj.DbType == "SQL2000" || dbobj.DbType == "SQL2005") && (IsHasIdentity))
            {
                strretu = "int";
            }
            //��������ͷ
            string strFun = CodeCommon.Space(2) + "public " + strretu + " Add(" + ModelSpace + " model)";
            strclass.AppendLine(strFun);
            strclass.AppendSpaceLine(2,"{" );
            strclass.AppendSpaceLine(3,"StringBuilder strSql=new StringBuilder();" );
            strclass.AppendSpaceLine(3,"strSql.Append(\"insert into " + _tablename + "(\");" );
            strclass1.AppendSpace(3,"strSql.Append(\"");
            
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                bool IsIdentity = field.IsIdentity;

                if (IsIdentity) 
                {                    
                    continue;
                }                   
                strclass1.Append(columnName + ",");
                if (CodeCommon.IsAddMark(columnType.Trim()))
                {
                    strclass2.AppendSpaceLine(3,"strSql.Append(\"'\"+model." + columnName + "+\"',\");" );
                }
                else
                {
                    strclass2.AppendSpaceLine(3,"strSql.Append(\"\"+model." + columnName + "+\",\");");
                }
            }
            
            //ȥ�����Ķ���
            strclass1.DelLastComma();
            strclass2.Remove(strclass2.Value.Length - 6, 1);

            strclass1.AppendLine("\");" );
            strclass.Append(strclass1.ToString());

            strclass.AppendSpaceLine(3, "strSql.Append(\")\");");
            strclass.AppendSpaceLine(3,"strSql.Append(\" values (\");" );
            strclass.Append(strclass2.ToString());
            strclass.AppendSpaceLine(3,"strSql.Append(\")\");");

            //���¶��巽��ͷ
            if ((dbobj.DbType == "SQL2000" || dbobj.DbType == "SQL2005") && (IsHasIdentity))
            {
                strclass.AppendSpaceLine(3, "strSql.Append(\";select @@IDENTITY\");" );
                strclass.AppendSpaceLine(3, "object obj = " + DbHelperName + ".GetSingle(strSql.ToString());");
                strclass.AppendSpaceLine(3, "if (obj == null)" );
                strclass.AppendSpaceLine(3, "{" );
                strclass.AppendSpaceLine(4, "return 1;" );
                strclass.AppendSpaceLine(3, "}" );
                strclass.AppendSpaceLine(3, "else" );
                strclass.AppendSpaceLine(3, "{" );
                strclass.AppendSpaceLine(4, "return Convert.ToInt32(obj);" );
                strclass.AppendSpaceLine(3, "}" );
            }
            else
            {
                strclass.AppendSpaceLine(3,"" + DbHelperName + ".ExecuteSql(strSql.ToString());");                
            }

            strclass.AppendSpace(2, "}");
            return strclass.ToString();
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
            if (ModelSpace == "")
            {
                ModelSpace = "ModelClassName"; ;
            }
            StringPlus strclass = new StringPlus();
            strclass.AppendLine("");
            strclass.AppendSpaceLine(2, "/// <summary>" );
            strclass.AppendSpaceLine(2, "/// ����һ������" );
            strclass.AppendSpaceLine(2, "/// </summary>" );
            strclass.AppendSpaceLine(2, "public void Update(" + ModelSpace + " model)" );
            strclass.AppendSpaceLine(2, "{" );
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();" );
            strclass.AppendSpaceLine(3, "strSql.Append(\"update " + _tablename + " set \");" );
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                string Length = field.Length;
                bool IsIdentity = field.IsIdentity;
                bool isPK = field.IsPK;

                if (field.IsIdentity || field.IsPK || (Keys.Contains(field)))
                {
                    continue;
                }      
                if (CodeCommon.IsAddMark(columnType.Trim()))
                {
                    strclass.AppendSpaceLine(3, "strSql.Append(\"" + columnName + "='\"+model." + columnName + "+\"',\");" );
                }
                else
                {
                    strclass.AppendSpaceLine(3, "strSql.Append(\"" + columnName + "=\"+model." + columnName + "+\",\");" );
                }
            }            

            //ȥ�����Ķ���			
            strclass.Remove(strclass.Value.Length - 6, 1);
            strclass.AppendSpaceLine(3, "strSql.Append(\" where " + LTP.CodeHelper.CodeCommon.GetModelWhereExpression(Keys) + "\");");
            strclass.AppendSpaceLine(3, "" + DbHelperName + ".ExecuteSql(strSql.ToString());" );
            strclass.AppendSpace(2, "}");
            return strclass.ToString();
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
            strclass.AppendLine("");
            strclass.AppendSpaceLine(2,"/// <summary>" );
            strclass.AppendSpaceLine(2,"/// ɾ��һ������" );
            strclass.AppendSpaceLine(2,"/// </summary>" );
            strclass.AppendSpaceLine(2,"public void Delete(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")" );
            strclass.AppendSpaceLine(2,"{" );
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();" );
            if (dbobj.DbType != "OleDb")
            {
                strclass.AppendSpaceLine(3, "strSql.Append(\"delete " + _tablename + " \");" );
            }
            else
            {
                strclass.AppendSpaceLine(3, "strSql.Append(\"delete from " + _tablename + " \");" );
            }
            strclass.AppendSpaceLine(3, "strSql.Append(\" where " + LTP.CodeHelper.CodeCommon.GetWhereExpression(Keys) + "\" );" );

            strclass.AppendSpaceLine(3, DbHelperName + ".ExecuteSql(strSql.ToString());" );
            strclass.AppendSpace(2,"}");
            return strclass.ToString();
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
            if (ModelSpace == "")
            {
                ModelSpace = "ModelClassName"; ;
            }
            StringPlus strclass = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>" );
            strclass.AppendSpaceLine(2, "/// �õ�һ������ʵ��" );
            strclass.AppendSpaceLine(2, "/// </summary>" );
            strclass.AppendSpaceLine(2, "public " + ModelSpace + " GetModel(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")" );
            strclass.AppendSpaceLine(2, "{" );
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();" );
            strclass.AppendSpaceLine(3, "strSql.Append(\"select  " + " \");" );
            strclass.AppendSpaceLine(3, "strSql.Append(\" " + Fieldstrlist + " \");" );
            strclass.AppendSpaceLine(3, "strSql.Append(\" from " + _tablename + " \");" );
            strclass.AppendSpaceLine(3, "strSql.Append(\" where " + LTP.CodeHelper.CodeCommon.GetWhereExpression(Keys) + "\" );" );
            strclass.AppendSpaceLine(3,  ModelSpace + " model=new " + ModelSpace + "();" );
            strclass.AppendSpaceLine(3, "DataSet ds=" + DbHelperName + ".Query(strSql.ToString());" );            
            strclass.AppendSpaceLine(3, "if(ds.Tables[0].Rows.Count>0)" );
            strclass.AppendSpaceLine(3, "{" );
      
                foreach (ColumnInfo field in Fieldlist)
                {
                    string columnName = field.ColumnName;
                    string columnType = field.TypeName;
                    switch (CodeCommon.DbTypeToCS(columnType))
                    {
                        case "int":
                            {
                                strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" );
                                strclass.AppendSpaceLine(4, "{" );
                                strclass.AppendSpaceLine(5, "model." + columnName + "=int.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());" );
                                strclass.AppendSpaceLine(4, "}" );
                            }
                            break;
                        case "long":
                            {
                                strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                                strclass.AppendSpaceLine(4, "{");
                                strclass.AppendSpaceLine(5, "model." + columnName + "=long.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                                strclass.AppendSpaceLine(4, "}");
                            }
                            break;
                        case "decimal":
                            {
                                strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" );
                                strclass.AppendSpaceLine(4, "{" );
                                strclass.AppendSpaceLine(5, "model." + columnName + "=decimal.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());" );
                                strclass.AppendSpaceLine(4, "}" );
                            }
                            break;
                        case "float":
                            {
                                strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                                strclass.AppendSpaceLine(4, "{");
                                strclass.AppendSpaceLine(5, "model." + columnName + "=float.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                                strclass.AppendSpaceLine(4, "}");
                            }
                            break;
                        case "DateTime":
                            {
                                strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" );
                                strclass.AppendSpaceLine(4, "{" );
                                strclass.AppendSpaceLine(5, "model." + columnName + "=DateTime.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());" );
                                strclass.AppendSpaceLine(4, "}" );
                            }
                            break;
                        case "string":
                            {
                                strclass.AppendSpaceLine(4, "model." + columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();" );
                            }
                            break;
                        case "bool":
                            {
                                strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" );
                                strclass.AppendSpaceLine(4, "{" );
                                strclass.AppendSpaceLine(5, "if((ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()==\"1\")||(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString().ToLower()==\"true\"))" );
                                strclass.AppendSpaceLine(5, "{" );
                                strclass.AppendSpaceLine(6, "model." + columnName + "=true;" );
                                strclass.AppendSpaceLine(5, "}" );
                                strclass.AppendSpaceLine(5, "else" );
                                strclass.AppendSpaceLine(5, "{" );
                                strclass.AppendSpaceLine(6, "model." + columnName + "=false;" );
                                strclass.AppendSpaceLine(5, "}" );
                                strclass.AppendSpaceLine(4, "}"  );
                            }
                            break;
                        case "byte[]":
                            {
                                strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" );
                                strclass.AppendSpaceLine(4, "{" );
                                strclass.AppendSpaceLine(5, "model." + columnName + "=(byte[])ds.Tables[0].Rows[0][\"" + columnName + "\"];" );
                                strclass.AppendSpaceLine(4, "}" );
                            }
                            break;
                        case "Guid":
                            {
                                strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" );
                                strclass.AppendSpaceLine(4, "{" );
                                strclass.AppendSpaceLine(5, "model." + columnName + "=new Guid(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());" );
                                strclass.AppendSpaceLine(4, "}" );
                            }
                            break;
                        default:
                            strclass.AppendSpaceLine(4, "//model." + columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();" );
                            break;
                    }
                }            
            strclass.AppendSpaceLine(4, "return model;" );
            strclass.AppendSpaceLine(3, "}" );
            strclass.AppendSpaceLine(3, "else" );
            strclass.AppendSpaceLine(3, "{" );
            strclass.AppendSpaceLine(4, "return null;" );
            strclass.AppendSpaceLine(3, "}" );
            strclass.AppendSpace(2, "}");
            return strclass.ToString();
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
            strclass.AppendSpaceLine(3, "{" );
            strclass.AppendSpaceLine(4, "strSql.Append(\" where \"+strWhere);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "return " + DbHelperName + ".Query(strSql.ToString());");
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
            //strclass.AppendSpaceLine(2, "/// <summary>");
            //strclass.AppendSpaceLine(2, "/// ��ҳ��ȡ�����б�");
            //strclass.AppendSpaceLine(2, "/// </summary>");
            //strclass.AppendSpaceLine(2, "public DataSet GetList(int PageSize,int PageIndex,string strWhere)");
            //strclass.AppendSpaceLine(2, "{");
            //strclass.AppendSpaceLine(3, "" + DbParaHead + "Parameter[] parameters = {");
            //strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"" + preParameter + "tblName\", " + DbParaDbType + ".VarChar, 255),");
            //strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"" + preParameter + "fldName\", " + DbParaDbType + ".VarChar, 255),");
            //strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"" + preParameter + "PageSize\", " + DbParaDbType + "." + CodeCommon.CSToProcType(dbobj.DbType, "int") + "),");
            //strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"" + preParameter + "PageIndex\", " + DbParaDbType + "." + CodeCommon.CSToProcType(dbobj.DbType, "int") + "),");
            //strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"" + preParameter + "IsReCount\", " + DbParaDbType + "." + CodeCommon.CSToProcType(dbobj.DbType, "bit") + "),");
            //strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"" + preParameter + "OrderType\", " + DbParaDbType + "." + CodeCommon.CSToProcType(dbobj.DbType, "bit") + "),");
            //strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"" + preParameter + "strWhere\", " + DbParaDbType + ".VarChar,1000),");
            //strclass.AppendSpaceLine(5, "};");
            //strclass.AppendSpaceLine(3, "parameters[0].Value = \"" + TableName + "\";");
            //strclass.AppendSpaceLine(3, "parameters[1].Value = \"" + _key + "\";");
            //strclass.AppendSpaceLine(3, "parameters[2].Value = PageSize;");
            //strclass.AppendSpaceLine(3, "parameters[3].Value = PageIndex;");
            //strclass.AppendSpaceLine(3, "parameters[4].Value = 0;");
            //strclass.AppendSpaceLine(3, "parameters[5].Value = 0;");
            //strclass.AppendSpaceLine(3, "parameters[6].Value = strWhere;	");
            //strclass.AppendSpaceLine(3, "return " + DbHelperName + ".RunProcedure(\"UP_GetRecordByPage\",parameters,\"ds\");");
            //strclass.AppendSpaceLine(2, "}");
            strclass.AppendSpaceLine(2, "*/");
            return strclass.Value;
        }
        #endregion//���ݲ�


    }
}
