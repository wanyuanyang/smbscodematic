using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;
using LTP.Utility;
using System.Windows.Forms;
using LTP.IDBO;
using LTP.DBFactory;
using LTP.CodeHelper;
namespace LTP.CodeBuild
{
    /// <summary>
    /// ����ṹ
    /// </summary>
    public class BuilderFrameOne : BuilderFrame
    {
        string cmcfgfile = Application.StartupPath + @"\cmcfg.ini";
        LTP.Utility.INIFile cfgfile;

        #region ����

        private string _procprefix;
        
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
        /// �洢����ǰ׺ 
        /// </summary>       
        public string ProcPrefix
        {
            set { _procprefix = value; }
            get { return _procprefix; }
        }
        #endregion


        public BuilderFrameOne(IDbObject idbobj, string dbName, string tableName, string modelName, 
            List<ColumnInfo> fieldlist, List<ColumnInfo> keys,
            string nameSpace, string folder,string dbHelperName)
        {
            dbobj = idbobj;
            DbName = dbName;
            TableName = tableName;
            ModelName = modelName;
            NameSpace = nameSpace;
            Folder = folder;
            DbHelperName = dbHelperName;
            _dbtype = idbobj.DbType;            
            Fieldlist = fieldlist;
            Keys = keys;            
            foreach (ColumnInfo key in keys)
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
            //model = new BuilderModel(dbobj, DbName, TableName, ModelName, NameSpace, "", "", fieldlist);
        }
                

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
            StringPlus strclass2 = new StringPlus();
            strclass.AppendSpaceLine(3, "" + DbParaHead + "Parameter[] parameters = {");
            int n = 0;
            foreach (ColumnInfo key in keys)
            {
                strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"" + preParameter + "" + key.ColumnName + "\", " + DbParaDbType + "." + CodeCommon.DbTypeLength(dbobj.DbType, key.TypeName, "") + "),");
                strclass2.AppendSpaceLine(3, "parameters[" + n.ToString() + "].Value = " + key.ColumnName + ";");
                n++;
            }
            strclass.DelLastComma();
            strclass.AppendLine("};");
            strclass.Append(strclass2.Value);
            return strclass.Value;
        }

      

        #endregion

        #region ���ݷ��ʲ����

        public string GetCode(string DALtype, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List, string procPrefix)
        {
            cfgfile = new LTP.Utility.INIFile(cmcfgfile);
            DALtype = cfgfile.IniReadValue("BuilderOne", DALtype.Trim());
                
            ProcPrefix = procPrefix;
            StringPlus strclass = new StringPlus();
            strclass.AppendLine("using System;");
            strclass.AppendLine("using System.Data;");
            strclass.AppendLine("using System.Text;");
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
                case "MySQL":
                    strclass.AppendLine("using MySql.Data.MySqlClient;");                    
                    break;
                case "OleDb":
                    strclass.AppendLine("using System.Data.OleDb;");
                    break;
            }
            strclass.AppendLine("using Maticsoft.DBUtility;//�����������");
            strclass.AppendLine("namespace " + NameSpace);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// ��" + ModelName + "��");
            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpaceLine(1, "public class " + ModelName);
            
            strclass.AppendSpaceLine(1, "{");
            strclass.AppendSpaceLine(2, "public " + ModelName + "()");
            strclass.AppendSpaceLine(2, "{}");

            
            LTP.BuilderModel.BuilderModel model = new LTP.BuilderModel.BuilderModel();
            model.ModelName = ModelName;
            model.NameSpace = NameSpace;
            model.Fieldlist = Fieldlist;
            model.Modelpath = Modelpath;
            //return model.CreatModel();
            strclass.AppendLine(model.CreatModelMethod());


            strclass.AppendLine("");
            strclass.AppendSpaceLine(2, "#region  ��Ա����");
            switch (DALtype)
            {
                case "sql":
                    {
                        #region  SQL��������
                        strclass.AppendLine(CreatConstructorSQL());
                        if (Maxid)
                        {
                            strclass.AppendLine(CreatGetMaxIDSQL());
                        }
                        if (Exists)
                        {
                            strclass.AppendLine(CreatExistsSQL());
                        }
                        if (Add)
                        {
                            strclass.AppendLine(CreatAddSQL());
                        }
                        if (Update)
                        {
                            strclass.AppendLine(CreatUpdateSQL());
                        }
                        if (Delete)
                        {
                            strclass.AppendLine(CreatDeleteSQL());
                        }
                        if (GetModel)
                        {
                            
                            strclass.AppendLine(CreatGetModelSQL());
                        }
                        if (List)
                        {
                            strclass.AppendLine(CreatGetListSQL());
                        }
                        #endregion
                        break;
                    }
                case "Param":
                    {
                        #region  Parameter��������
                        strclass.Append(CreatConstructorParam() + "\r\n");
                        if (Maxid)
                        {
                            strclass.Append(CreatGetMaxIDParam() + "\r\n");
                        }
                        if (Exists)
                        {
                            strclass.Append(CreatExistsParam() + "\r\n");
                        }
                        if (Add)
                        {
                            strclass.Append(CreatAddParam() + "\r\n");
                        }
                        if (Update)
                        {
                            strclass.Append(CreatUpdateParam() + "\r\n");
                        }
                        if (Delete)
                        {
                            strclass.Append(CreatDeleteParam() + "\r\n");
                        }
                        if (GetModel)
                        {
                            strclass.Append(CreatGetModelParam() + "\r\n");
                        }
                        if (List)
                        {
                            strclass.Append(CreatGetListParam() + "\r\n");
                        }
                        #endregion
                        break;
                    }
                case "Proc":
                    {
                        #region  Proc��������
                        strclass.Append(CreatConstructorProc() + "\r\n");
                        if (Maxid)
                        {
                            strclass.Append(CreatGetMaxIDProc() + "\r\n");
                        }
                        if (Exists)
                        {
                            strclass.Append(CreatExistsProc() + "\r\n");
                        }
                        if (Add)
                        {
                            strclass.Append(CreatAddProc() + "\r\n");
                        }
                        if (Update)
                        {
                            strclass.Append(CreatUpdateProc() + "\r\n");
                        }
                        if (Delete)
                        {
                            strclass.Append(CreatDeleteProc() + "\r\n");
                        }
                        if (GetModel)
                        {
                            strclass.Append(CreatGetModelProc() + "\r\n");
                        }
                        if (List)
                        {
                            strclass.Append(CreatGetListProc() + "\r\n");
                        }
                        if (List)
                        {
                            strclass.Append(CreatGetListByPageProc() + "\r\n");
                        }
                        #endregion
                        break;
                    }
                default:
                    strclass.AppendSpaceLine(2, "//�ݲ�֧�ָ÷�ʽ��\r\n");                    
                    break;
            }
            strclass.AppendSpaceLine(2, "#endregion  ��Ա����");
            strclass.AppendSpaceLine(1, "}");
            strclass.AppendLine("}");
            strclass.AppendLine("");

            return strclass.ToString();
        }

        #endregion

        

        #region ����(ʹ��SQL���ʵ��)
        /// <summary>
        /// �õ����ID�ķ�������
        /// </summary>
        /// <param name="TabName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatGetMaxIDSQL()
        {

            StringPlus strclass = new StringPlus();
            if (Keys.Count > 0)
            {
                string keyname = "";
                foreach (ColumnInfo obj in Keys)
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
                            strclass.AppendSpaceLine(2, "{" + "\r\n");
                            strclass.AppendSpaceLine(2, "return " + DbHelperName + ".GetMaxID(\"" + keyname + "\", \"" + TableName + "\"); ");
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
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatExistsSQL()
        {
            StringPlus strclass = new StringPlus();
            if (Keys.Count > 0)
            {
                strclass.AppendLine("");
                strclass.AppendSpaceLine(2, "/// <summary>");
                strclass.AppendSpaceLine(2, "/// �Ƿ���ڸü�¼");
                strclass.AppendSpaceLine(2, "/// </summary>");
                strclass.AppendSpaceLine(2, "public bool Exists(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
                strclass.AppendSpaceLine(2, "{");
                strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
                strclass.AppendSpace(3, "strSql.Append(\"select count(1) from " + TableName);
                strclass.AppendSpaceLine(0, " where " + LTP.CodeHelper.CodeCommon.GetWhereExpression(Keys) + " );");
                strclass.AppendSpaceLine(3, "return " + DbHelperName + ".Exists(strSql.ToString());");
                strclass.AppendSpace(2, "}");
            }
            return strclass.ToString();
        }
        /// <summary>
        /// �õ�Add()�Ĵ���
        /// </summary>
        /// <param name="ExistsMaxId">�Ƿ���GetMaxId()��������</param>	
        private string CreatAddSQL()
        {
            //DataTable dt = dbobj.GetColumnInfoList(DbName, TableName);
            StringBuilder strclass = new StringBuilder();
            StringBuilder strclass1 = new StringBuilder();
            StringBuilder strclass2 = new StringBuilder();
            strclass.Append("\r\n");
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// ����һ������" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            string strretu = "void";
            if ((dbobj.DbType == "SQL2000" || dbobj.DbType == "SQL2005" || dbobj.DbType == "SQL2008") && (IsHasIdentity))
            {
                strretu = "int";
            }
            strclass.Append(Space(2) + "public " + strretu + " Add()" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            //if (ExistsMaxId)
            //{
            //    strclass.Append(Space(3) + _key + "=GetMaxId();" + "\r\n");
            //}
            strclass.Append(Space(3) + "StringBuilder strSql=new StringBuilder();" + "\r\n");
            strclass.Append(Space(3) + "strSql.Append(\"insert into [" + TableName + "](\");" + "\r\n");
            strclass1.Append(Space(3) + "strSql.Append(\"");
            foreach (ColumnInfo field in Fieldlist)
                {
                    string columnName = field.ColumnName;
                    string columnType = field.TypeName;
                    bool IsIdentity = field.IsIdentity;

                    if (field.IsIdentity)
                    {                        
                        strretu = CodeCommon.DbTypeToCS(columnType);
                        continue;
                    }                   
                    strclass1.Append(columnName + ",");
                    if (CodeCommon.IsAddMark(columnType.Trim()))
                    {
                        strclass2.Append(Space(3) + "strSql.Append(\"'\"+" + columnName + "+\"',\");" + "\r\n");
                    }
                    else
                    {
                        strclass2.Append(Space(3) + "strSql.Append(\"\"+" + columnName + "+\",\");" + "\r\n");
                    }

            }
            
            //ȥ�����Ķ���
            strclass1.Remove(strclass1.Length - 1, 1);
            strclass2.Remove(strclass2.Length - 6, 1);

            strclass1.Append("\");" + "\r\n");
            strclass.Append(strclass1.ToString());

            strclass.Append(Space(3) + "strSql.Append(\")\");" + "\r\n");
            strclass.Append(Space(3) + "strSql.Append(\" values (\");" + "\r\n");
            strclass.Append(strclass2.ToString());

            strclass.Append(Space(3) + "strSql.Append(\")\");" + "\r\n");

            if ((dbobj.DbType == "SQL2000" || dbobj.DbType == "SQL2005" || dbobj.DbType == "SQL2008") && (IsHasIdentity))
            {
                strclass.Append(CodeCommon.Space(3) + "strSql.Append(\";select @@IDENTITY\");" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "object obj = " + DbHelperName + ".GetSingle(strSql.ToString());" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "if (obj == null)" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "{" + "\r\n");
                strclass.Append(CodeCommon.Space(4) + "return 1;" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "}" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "else" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "{" + "\r\n");
                strclass.Append(CodeCommon.Space(4) + "return Convert.ToInt32(obj);" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "}" + "\r\n");

            }
            else
            {
                strclass.Append(CodeCommon.Space(3) + "" + DbHelperName + ".ExecuteSql(strSql.ToString());" + "\r\n");                
            }

            strclass.Append(Space(2) + "}");
            return strclass.ToString();
        }

        /// <summary>
        /// �õ�Update�����Ĵ���
        /// </summary>
        /// <param name="DbName"></param>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <param name="ModelName"></param>
        /// <returns></returns>
        private string CreatUpdateSQL()
        {

            StringPlus strclass = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2,"/// <summary>" );
            strclass.AppendSpaceLine(2, "/// ����һ������" );
            strclass.AppendSpaceLine(2, "/// </summary>" );
            strclass.AppendSpaceLine(2, "public void Update()" );
            strclass.AppendSpaceLine(2, "{" );
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();" );
            strclass.AppendSpaceLine(3, "strSql.Append(\"update " + TableName + " set \");" );
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


                    if ((dbobj.DbType == "Oracle") && (columnType.ToLower() == "date" || columnType.ToLower() == "datetime"))
                    {
                        strclass.AppendSpaceLine(3, "strSql.Append(\"" + columnName + "=to_date('\" + " + columnName + ".ToString() + \"','YYYY-MM-DD HH24:MI:SS'),\");");
                    }
                    else
                        if (columnType.ToLower() == "bit")
                        {
                            strclass.AppendSpaceLine(3, "strSql.Append(\"" + columnName + "=\"+ (" + columnName + "? 1 : 0) +\",\");");
                        }
                        else
                            if (CodeCommon.IsAddMark(columnType.Trim()))
                            {
                                strclass.AppendSpaceLine(3, "strSql.Append(\"" + columnName + "='\"+" + columnName + "+\"',\");");
                            }
                            else
                            {
                                strclass.AppendSpaceLine(3, "strSql.Append(\"" + columnName + "=\"+" + columnName + "+\",\");");
                            }


                    //if (CodeCommon.IsAddMark(columnType.Trim()))
                    //{
                    //    strclass.Append(Space(3) + "strSql.Append(\"" + columnName + "='\"+" + columnName + "+\"',\");" + "\r\n");
                    //}
                    //else
                    //{
                    //    strclass.Append(Space(3) + "strSql.Append(\"" + columnName + "=\"+" + columnName + "+\",\");" + "\r\n");
                    //}
            }
            

            //ȥ�����Ķ���			
            strclass.Remove(strclass.Value.Length - 1, 1);

            strclass.AppendSpaceLine(3, "strSql.Append(\" where " + LTP.CodeHelper.CodeCommon.GetWhereExpression(Keys) + "\");" );
            strclass.AppendSpaceLine(3, "" + DbHelperName + ".ExecuteSql(strSql.ToString());");
            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }
        /// <summary>
        /// �õ�Delete�Ĵ���
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatDeleteSQL()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.Append("\r\n");
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// ɾ��һ������" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            strclass.Append(Space(2) + "public void Delete(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            strclass.Append(Space(3) + "StringBuilder strSql=new StringBuilder();" + "\r\n");
            //if (_dbtype != "OleDb")
            //{
            //    strclass.Append(Space(3) + "strSql.Append(\"delete " + TableName + " \");" + "\r\n");
            //}
            //else
            //{
                strclass.Append(Space(3) + "strSql.Append(\"delete from " + TableName + " \");" + "\r\n");
            //}
            if (LTP.CodeHelper.CodeCommon.GetWhereExpression(Keys).Length > 0)
            {
                strclass.Append(Space(3) + "strSql.Append(\" where " + LTP.CodeHelper.CodeCommon.GetWhereExpression(Keys) + ");" + "\r\n");
            }
            else
            {
                strclass.Append(Space(3) + "//strSql.Append(\" where ����);" + "\r\n");
            }                      
            strclass.Append(Space(3) + "	" + DbHelperName + ".ExecuteSql(strSql.ToString());" + "\r\n");
            strclass.Append(Space(2) + "}");
            return strclass.ToString();
        }

        /// <summary>
        /// �õ����캯�� �Ĵ���
        /// </summary>    
        private string CreatConstructorSQL()
        {            
            StringBuilder strclass = new StringBuilder();
            strclass.Append("\r\n");
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// �õ�һ������ʵ��" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            strclass.Append(Space(2) + "public " + ModelName + "(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            strclass.Append(Space(3) + "StringBuilder strSql=new StringBuilder();" + "\r\n");
            strclass.Append(Space(3) + "strSql.Append(\"select " + " \");" + "\r\n");
            strclass.Append(Space(3) + "strSql.Append(\"" + Fieldstrlist + " \");" + "\r\n");
            strclass.Append(Space(3) + "strSql.Append(\" from " + TableName + " \");" + "\r\n");
            if (LTP.CodeHelper.CodeCommon.GetWhereExpression(Keys).Length > 0)
            {
                strclass.Append(Space(3) + "strSql.Append(\" where " + LTP.CodeHelper.CodeCommon.GetWhereExpression(Keys) + ");" + "\r\n");
            }
            else
            {
                strclass.Append(Space(3) + "//strSql.Append(\" where ����);" + "\r\n");
            }
            strclass.Append(Space(3) + "DataSet ds=" + DbHelperName + ".Query(strSql.ToString());" + "\r\n");

            strclass.Append(Space(3) + "if(ds.Tables[0].Rows.Count>0)" + "\r\n");
            strclass.Append(Space(3) + "{" + "\r\n");
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                switch (CodeCommon.DbTypeToCS(columnType))
                {
                    case "int":
                        {
                            strclass.Append(Space(4) + "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" + "\r\n");
                            strclass.Append(Space(4) + "{" + "\r\n");
                            strclass.Append(Space(5) + "" + columnName + "=int.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());" + "\r\n");
                            strclass.Append(Space(4) + "}" + "\r\n");
                        }
                        break;
                    case "decimal":
                        {
                            strclass.Append(Space(4) + "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" + "\r\n");
                            strclass.Append(Space(4) + "{" + "\r\n");
                            strclass.Append(Space(5) + "" + columnName + "=decimal.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());" + "\r\n");
                            strclass.Append(Space(4) + "}" + "\r\n");
                        }
                        break;
                    case "DateTime":
                        {
                            strclass.Append(Space(4) + "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" + "\r\n");
                            strclass.Append(Space(4) + "{" + "\r\n");
                            strclass.Append(Space(5) + "" + columnName + "=DateTime.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());" + "\r\n");
                            strclass.Append(Space(4) + "}" + "\r\n");
                        }
                        break;
                    case "string":
                        {
                            strclass.Append(Space(4) + "" + columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();" + "\r\n");
                        }
                        break;
                    case "bool":
                        {
                            strclass.Append(Space(4) + "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" + "\r\n");
                            strclass.Append(Space(4) + "{" + "\r\n");
                            strclass.Append(Space(5) + "if((ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()==\"1\")||(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString().ToLower()==\"true\"))" + "\r\n");
                            strclass.Append(Space(5) + "{" + "\r\n");
                            strclass.Append(Space(6) + "" + columnName + "=true;" + "\r\n");
                            strclass.Append(Space(5) + "}" + "\r\n");
                            strclass.Append(Space(5) + "else" + "\r\n");
                            strclass.Append(Space(5) + "{" + "\r\n");
                            strclass.Append(Space(6) + "" + columnName + "=false;" + "\r\n");
                            strclass.Append(Space(5) + "}" + "\r\n");
                            strclass.Append(Space(4) + "}" + "\r\n" + "\r\n");
                        }
                        break;
                    case "byte[]":
                        {
                            strclass.Append(Space(4) + "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" + "\r\n");
                            strclass.Append(Space(4) + "{" + "\r\n");
                            strclass.Append(Space(5) + "" + columnName + "=(byte[])ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();" + "\r\n");
                            strclass.Append(Space(4) + "}" + "\r\n");
                        }
                        break;
                    default:
                        strclass.Append(Space(4) + "" + columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();" + "\r\n");
                        break;
                }
            }

            strclass.Append(Space(3) + "}" + "\r\n");
            strclass.Append(Space(2) + "}");
            return strclass.ToString();
        }

        /// <summary>
        /// �õ�GetModel()�Ĵ���
        /// </summary>    
        private string CreatGetModelSQL()
        {
            StringPlus strclass = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>" );
            strclass.AppendSpaceLine(2, "/// �õ�һ������ʵ��" );
            strclass.AppendSpaceLine(2, "/// </summary>" );
            strclass.AppendSpaceLine(2, "public void GetModel(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{" + "\r\n");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();" );
            //strclass.Append(Space(3) + "strSql.Append(\"select " + " \");" + "\r\n");
            //strclass.Append(Space(3) + "strSql.Append(\"" + Fieldstrlist + " \");" + "\r\n");

            strclass.AppendSpace(3, "strSql.Append(\"select ");
            if (dbobj.DbType == "SQL2005" || dbobj.DbType == "SQL2000" || dbobj.DbType == "SQL2008")
            {
                strclass.Append(" top 1 ");
            }
            strclass.AppendLine(" \");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" " + Fieldstrlist + " \");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" from " + TableName + " \");");

            //strclass.Append(Space(3) + "strSql.Append(\" from " + TableName + " \");" + "\r\n");
            if (LTP.CodeHelper.CodeCommon.GetWhereExpression(Keys).Length > 0)
            {               
                strclass.AppendSpaceLine(3, "strSql.Append(\" where " + LTP.CodeHelper.CodeCommon.GetWhereExpression(Keys) + "\" );");
            }
            else
            {
                strclass.AppendSpaceLine(3, "//strSql.Append(\" where ����);" ); 
            }
            strclass.AppendSpaceLine(3, "DataSet ds=" + DbHelperName + ".Query(strSql.ToString());");

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
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, columnName + "=int.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "long":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5,  columnName + "=long.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "decimal":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5,  columnName + "=decimal.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "float":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5,  columnName + "=float.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "DateTime":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, columnName + "=DateTime.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "string":
                        {
                            strclass.AppendSpaceLine(4,  columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();");
                        }
                        break;
                    case "bool":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, "if((ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()==\"1\")||(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString().ToLower()==\"true\"))");
                            strclass.AppendSpaceLine(5, "{");
                            strclass.AppendSpaceLine(6,  columnName + "=true;");
                            strclass.AppendSpaceLine(5, "}");
                            strclass.AppendSpaceLine(5, "else");
                            strclass.AppendSpaceLine(5, "{");
                            strclass.AppendSpaceLine(6,  columnName + "=false;");
                            strclass.AppendSpaceLine(5, "}");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "byte[]":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5,  columnName + "=(byte[])ds.Tables[0].Rows[0][\"" + columnName + "\"];");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "Guid":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5,  columnName + "=new Guid(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    default:
                        strclass.AppendSpaceLine(4, "//" + columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();");
                        break;
                }
            }            
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");            
            return strclass.ToString();
        }

        /// <summary>
        /// �õ�GetList()�Ĵ���
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatGetListSQL()
        {            
            List<ColumnInfo> collist = dbobj.GetColumnList(DbName, TableName);
            DataTable dt = LTP.CodeHelper.CodeCommon.GetColumnInfoDt(collist);
            StringPlus strclass = new StringPlus();
            string strfields = GetFieldslist(dt);
            strclass.AppendSpaceLine(2, "/// <summary>" );
            strclass.AppendSpaceLine(2, "/// ��������б�" );
            strclass.AppendSpaceLine(2, "/// </summary>" );
            strclass.AppendSpaceLine(2, "public DataSet GetList(string strWhere)" );
            strclass.AppendSpaceLine(2, "{" );
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();" );
            strclass.AppendSpace(3, "strSql.Append(\"select ");
            strclass.AppendLine("* \");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" FROM " + TableName + " \");");
            strclass.AppendSpaceLine(3, "if(strWhere.Trim()!=\"\")" );
            strclass.AppendSpaceLine(3,"{" + "\r\n");
            strclass.AppendSpaceLine(4, "strSql.Append(\" where \"+strWhere);" );
            strclass.AppendSpaceLine(3, "}" );
            strclass.AppendSpaceLine(3, "return " + DbHelperName + ".Query(strSql.ToString());" );
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;
        }

        #endregion//���ݲ�

        #region ����(ʹ�ô洢����ʵ��)

        /// <summary>
        /// �õ����ID�ķ�������
        /// </summary>
        /// <param name="TabName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatGetMaxIDProc()
        {
            StringPlus strclass = new StringPlus();
            if (Keys.Count > 0)
            {
                string keyname = "";
                foreach (ColumnInfo obj in Keys)
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
                            strclass.AppendSpaceLine(2, "{" + "\r\n");
                            strclass.AppendSpaceLine(2, "return " + DbHelperName + ".GetMaxID(\"" + keyname + "\", \"" + TableName + "\"); ");
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
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatExistsProc()
        {
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// �Ƿ���ڸü�¼");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public bool Exists(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "int rowsAffected;");

            strclass.AppendLine(GetPreParameter(Keys));
            
            strclass.AppendSpaceLine(3, "int result= " + DbHelperName + ".RunProcedure(\"UP_" + TableName + "_Exists\",parameters,out rowsAffected);");
            strclass.AppendSpaceLine(3, "if(result==1)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return true;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "else");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return false;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;
        }
        /// <summary>
        /// �õ�Add()�Ĵ���
        /// </summary>
        /// <param name="ExistsMaxId">�Ƿ���GetMaxId()��������</param>	
        private string CreatAddProc()
        {           
            StringPlus strclass = new StringPlus();
            StringPlus strclass2 = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "///  ����һ������");
            strclass.AppendSpaceLine(2, "/// </summary>");
            string strretu = "void";
            if ((dbobj.DbType == "SQL2000" || dbobj.DbType == "SQL2005" || dbobj.DbType == "SQL2008") && (IsHasIdentity))
            {
                strretu = "int";
            }
            strclass.AppendSpaceLine(2, "public " + strretu + " Add()");
            strclass.AppendSpaceLine(2, "{");            
            strclass.AppendSpaceLine(3, "int rowsAffected;");
            strclass.AppendSpaceLine(3, "" + DbParaHead + "Parameter[] parameters = {");
            int nkey = 0;
            int n = 0;
            foreach (ColumnInfo field in Fieldlist)
                {
                    string columnName = field.ColumnName;
                    string columnType = field.TypeName;
                    bool IsIdentity = field.IsIdentity;
                    string Length = field.Length;

                    strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"" + preParameter + columnName + "\", " + DbParaDbType + "." + CodeCommon.DbTypeLength(_dbtype, columnType, Length) + "),");
                    if (field.IsIdentity)
                    {
                        //KeyIsIdentity = true;
                        nkey = n;
                        strclass2.AppendSpaceLine(3, "parameters[" + n + "].Direction = ParameterDirection.Output;");
                        n++;
                        continue;
                    }
                    strclass2.AppendSpaceLine(3, "parameters[" + n + "].Value = " + columnName + ";");
                    n++;
            }
            
            if (strclass.Value.IndexOf(",") > 0)
            {
                strclass.DelLastComma();
            }
            strclass.AppendLine("};");
            strclass.AppendLine(strclass2.Value);
            strclass.AppendSpaceLine(3, "" + DbHelperName + ".RunProcedure(\"" + ProcPrefix + TableName + "_ADD" + "\",parameters,out rowsAffected);");
            if ((dbobj.DbType == "SQL2000" || dbobj.DbType == "SQL2005" || dbobj.DbType == "SQL2008") && (IsHasIdentity))
            {
                strclass.AppendSpaceLine(3, _key+"= (" + _keyType + ")parameters[" + nkey + "].Value;");
            }
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;

        }

        /// <summary>
        /// �õ�Update�����Ĵ���
        /// </summary>
        /// <param name="DbName"></param>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <param name="ModelName"></param>
        /// <returns></returns>
        private string CreatUpdateProc()
        {        
          
            StringPlus strclass = new StringPlus();
            StringPlus strclass2 = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "///  ����һ������");
            strclass.AppendSpaceLine(2, "/// </summary>");

            strclass.AppendSpaceLine(2, "public void Update()");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "int rowsAffected;");
            strclass.AppendSpaceLine(3, "" + DbParaHead + "Parameter[] parameters = {");
            int n = 0;
            foreach (ColumnInfo field in Fieldlist)
                {
                    string columnName = field.ColumnName;
                    string columnType = field.TypeName;
                    string Length = field.Length;

                    strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"" + preParameter + columnName + "\", " + DbParaDbType + "." + CodeCommon.DbTypeLength(_dbtype, columnType, Length) + "),");
                    strclass2.AppendSpaceLine(3, "parameters[" + n + "].Value = " + columnName + ";");
                    n++;
                }
            
            if (strclass.Value.IndexOf(",") > 0)
            {
                strclass.DelLastComma();
            }
            strclass.AppendLine("};");
            strclass.AppendLine(strclass2.Value);
            strclass.AppendSpaceLine(3, "" + DbHelperName + ".RunProcedure(\"" + ProcPrefix + TableName + "_Update" + "\",parameters,out rowsAffected);");
            
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;

        }
        /// <summary>
        /// �õ�Delete�Ĵ���
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatDeleteProc()
        {
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ɾ��һ������");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public void Delete(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{" + "\r\n");
            strclass.AppendSpaceLine(3, "int rowsAffected;");
                        
            strclass.AppendLine(GetPreParameter(Keys));

            strclass.AppendSpaceLine(3, "" + DbHelperName + ".RunProcedure(\"" + ProcPrefix + TableName + "_Delete" + "\",parameters,out rowsAffected);");
            
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;

        }

        /// <summary>
        /// �õ�GetModel()�Ĵ���
        /// </summary>     
        private string CreatConstructorProc()
        {
            
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// �õ�һ������ʵ��");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public "+ModelName+"(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{");
           
            strclass.AppendLine(GetPreParameter(Keys));

            strclass.AppendSpaceLine(3, "DataSet ds= " + DbHelperName + ".RunProcedure(\"" + "UP_" + TableName + "_GetModel" + "\",parameters,\"ds\");");           
            strclass.AppendSpaceLine(3, "if(ds.Tables[0].Rows.Count>0)");
            strclass.AppendSpaceLine(3, "{");
            foreach (ColumnInfo field in Fieldlist)
                {
                    string columnName = field.ColumnName;
                    string columnType = field.TypeName;
                    switch (CodeCommon.DbTypeToCS(columnType))
                    {
                        case "int":
                            {
                                strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                                strclass.AppendSpaceLine(4, "{");
                                strclass.AppendSpaceLine(5, "" + columnName + "=int.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                                strclass.AppendSpaceLine(4, "}");
                            }
                            break;
                        case "decimal":
                            {
                                strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                                strclass.AppendSpaceLine(4, "{");
                                strclass.AppendSpaceLine(5, "" + columnName + "=decimal.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                                strclass.AppendSpaceLine(4, "}");
                            }
                            break;
                        case "DateTime":
                            {
                                strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                                strclass.AppendSpaceLine(4, "{" + "\r\n");
                                strclass.AppendSpaceLine(5, "" + columnName + "=DateTime.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                                strclass.AppendSpaceLine(4, "}");
                            }
                            break;
                        case "string":
                            {
                                strclass.AppendSpaceLine(4, "" + columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();");
                            }
                            break;
                        case "bool":
                            {
                                strclass.Append(Space(4) + "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" + "\r\n");
                                strclass.Append(Space(4) + "{" + "\r\n");
                                strclass.Append(Space(5) + "if((ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()==\"1\")||(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString().ToLower()==\"true\"))" + "\r\n");
                                strclass.Append(Space(5) + "{" + "\r\n");
                                strclass.Append(Space(6) + "" + columnName + "=true;" + "\r\n");
                                strclass.Append(Space(5) + "}" + "\r\n");
                                strclass.Append(Space(5) + "else" + "\r\n");
                                strclass.Append(Space(5) + "{" + "\r\n");
                                strclass.Append(Space(6) + "" + columnName + "=false;" + "\r\n");
                                strclass.Append(Space(5) + "}" + "\r\n");
                                strclass.Append(Space(4) + "}" + "\r\n" + "\r\n");
                            }
                            break;
                        case "byte[]":
                            {
                                strclass.Append(Space(4) + "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" + "\r\n");
                                strclass.Append(Space(4) + "{" + "\r\n");
                                strclass.Append(Space(5) + "" + columnName + "=(byte[])ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();" + "\r\n");
                                strclass.Append(Space(4) + "}" + "\r\n");
                            }
                            break;
                        default:
                            strclass.AppendSpaceLine(4, "" + columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();");
                            break;

                    }

                }
            
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;

        }


        /// <summary>
        /// �õ�GetModel()�Ĵ���
        /// </summary>     
        private string CreatGetModelProc()
        {
            
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// �õ�һ������ʵ��");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public void GetModel(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{");
           
            strclass.AppendLine(GetPreParameter(Keys));

            strclass.AppendSpaceLine(3, "DataSet ds= " + DbHelperName + ".RunProcedure(\"" + ProcPrefix + TableName + "_GetModel" + "\",parameters,\"ds\");");           
            strclass.AppendSpaceLine(3, "if(ds.Tables[0].Rows.Count>0)");
            strclass.AppendSpaceLine(3, "{");
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                switch (CodeCommon.DbTypeToCS(columnType))
                {
                    case "int":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, columnName + "=int.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "long":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, columnName + "=long.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "decimal":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, columnName + "=decimal.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "float":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, columnName + "=float.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "DateTime":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, columnName + "=DateTime.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "string":
                        {
                            strclass.AppendSpaceLine(4, columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();");
                        }
                        break;
                    case "bool":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, "if((ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()==\"1\")||(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString().ToLower()==\"true\"))");
                            strclass.AppendSpaceLine(5, "{");
                            strclass.AppendSpaceLine(6, columnName + "=true;");
                            strclass.AppendSpaceLine(5, "}");
                            strclass.AppendSpaceLine(5, "else");
                            strclass.AppendSpaceLine(5, "{");
                            strclass.AppendSpaceLine(6, columnName + "=false;");
                            strclass.AppendSpaceLine(5, "}");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "byte[]":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, columnName + "=(byte[])ds.Tables[0].Rows[0][\"" + columnName + "\"];");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "Guid":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, columnName + "=new Guid(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    default:
                        strclass.AppendSpaceLine(4, "//" + columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();");
                        break;
                }
            }
            
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;

        }

        /// <summary>
        /// �õ�GetList()�Ĵ���
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatGetListProc()
        {
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ��ȡ�����б�");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetList()");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "return " + DbHelperName + ".RunProcedure(\"" + ProcPrefix + TableName + "_GetList" + "\",new SqlParameter[] { null },\"ds\");");
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;
        }
        /// <summary>
        /// �õ�GetList()�Ĵ���
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatGetListByPageProc()
        {
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/*");
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ��ҳ��ȡ�����б�");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetList(int PageSize,int PageIndex,string strWhere)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "" + DbParaHead + "Parameter[] parameters = {");
            strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"@tblName\", " + DbParaDbType + ".VarChar, 255),");
            strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"@fldName\", " + DbParaDbType + ".VarChar, 255),");
            strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"@PageSize\", " + DbParaDbType + "." + CodeCommon.CSToProcType(_dbtype, "int") + "),");
            strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"@PageIndex\", " + DbParaDbType + "." + CodeCommon.CSToProcType(_dbtype, "int") + "),");
            strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"@IsReCount\", " + DbParaDbType + "." + CodeCommon.CSToProcType(_dbtype, "bit") + "),");
            strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"@OrderType\", " + DbParaDbType + "." + CodeCommon.CSToProcType(_dbtype, "bit") + "),");
            strclass.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"@strWhere\", " + DbParaDbType + ".VarChar,1000),");
            strclass.AppendSpaceLine(5, "};");
            strclass.AppendSpaceLine(3, "parameters[0].Value = \"" + this.TableName + "\";");
            strclass.AppendSpaceLine(3, "parameters[1].Value = \"" + this._key + "\";");
            strclass.AppendSpaceLine(3, "parameters[2].Value = PageSize;");
            strclass.AppendSpaceLine(3, "parameters[3].Value = PageIndex;");
            strclass.AppendSpaceLine(3, "parameters[4].Value = 0;");
            strclass.AppendSpaceLine(3, "parameters[5].Value = 0;");
            strclass.AppendSpaceLine(3, "parameters[6].Value = strWhere;	");
            strclass.AppendSpaceLine(3, "return " + DbHelperName + ".RunProcedure(\"UP_GetRecordByPage\",parameters,\"ds\");");
            strclass.AppendSpaceLine(2, "}*/");
            return strclass.Value;
        }

        #endregion//���ݲ�

        #region ����(ʹ��Parameterʵ��)

        /// <summary>
        /// �õ����ID�ķ�������
        /// </summary>
        /// <param name="TabName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatGetMaxIDParam()
        {
            StringPlus strclass = new StringPlus();
            if (Keys.Count > 0)
            {
                string keyname = "";
                foreach (ColumnInfo obj in Keys)
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
                            strclass.AppendSpaceLine(2, "{" + "\r\n");
                            strclass.AppendSpaceLine(2, "return " + DbHelperName + ".GetMaxID(\"" + keyname + "\", \"" + TableName + "\"); ");
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
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatExistsParam()
        {
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// �Ƿ���ڸü�¼");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public bool Exists(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpaceLine(3, "strSql.Append(\"select count(1) from " + TableName + "\");");

            if (GetWhereExpression(Keys).Length > 0)
            {
                strclass.AppendSpaceLine(3, "strSql.Append(\" where " + GetWhereExpression(Keys) + "\");" + "\r\n");
            }
            else
            {
                strclass.AppendSpaceLine(3, "//strSql.Append(\" where ����);" + "\r\n");
            }           
                   

            strclass.AppendLine(GetPreParameter(Keys));
            strclass.Append(CodeCommon.Space(3) + "return " + DbHelperName + ".Exists(strSql.ToString(),parameters);" + "\r\n");
            strclass.AppendSpaceLine(2, "}");

            return strclass.Value;
        }

        /// <summary>
        /// �õ�Add()�Ĵ���
        /// </summary>
        /// <param name="ExistsMaxId">�Ƿ���GetMaxId()��������</param>	
        private string CreatAddParam()
        {                   
            StringBuilder strclass = new StringBuilder();
            StringBuilder strclass1 = new StringBuilder();
            StringBuilder strclass2 = new StringBuilder();
            StringPlus strclass3 = new StringPlus();
            StringPlus strclass4 = new StringPlus();
            strclass.Append("\r\n");
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// ����һ������" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            string strretu = "void";
            if ((dbobj.DbType == "SQL2000" || dbobj.DbType == "SQL2005" || dbobj.DbType == "SQL2008") && (IsHasIdentity))
            {
                strretu = "int";
            }
            strclass.Append(Space(2) + "public " + strretu + " Add()" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
           
            strclass.Append(Space(3) + "StringBuilder strSql=new StringBuilder();" + "\r\n");
            strclass.Append(Space(3) + "strSql.Append(\"insert into " + TableName + "(\");" + "\r\n");
            strclass1.Append(Space(3) + "strSql.Append(\"");
            int n = 0;
            foreach (ColumnInfo field in Fieldlist)
                {
                    string columnName = field.ColumnName;
                    string columnType = field.TypeName;
                    bool IsIdentity = field.IsIdentity;
                    string Length = field.Length;
                    if (field.IsIdentity)
                    {                        
                        strretu = CodeCommon.DbTypeToCS(columnType);
                        continue;
                    }
                    strclass3.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"" + preParameter + columnName + "\", " + DbParaDbType + "." + CodeCommon.DbTypeLength(_dbtype, columnType, Length) + "),");
                    strclass1.Append(columnName + ",");
                    strclass2.Append(preParameter + columnName + ",");
                    strclass4.AppendSpaceLine(3, "parameters[" + n + "].Value = " + columnName + ";");
                    n++;
                }
            
            //ȥ�����Ķ���
            strclass1.Remove(strclass1.Length - 1, 1);
            strclass2.Remove(strclass2.Length - 1, 1);
            if (strclass3.Value.IndexOf(",")>0)
            {
                strclass3.DelLastComma();
            }
            strclass1.Append(")\");" + "\r\n");
            strclass.Append(strclass1.ToString());
                       
            strclass.Append(Space(3) + "strSql.Append(\" values (\");" + "\r\n");
            strclass.Append(Space(3) + "strSql.Append(\"" + strclass2.ToString() + ")\");" + "\r\n");
            if ((dbobj.DbType == "SQL2000" || dbobj.DbType == "SQL2005" || dbobj.DbType == "SQL2008") && (IsHasIdentity))
            {
                strclass.Append(CodeCommon.Space(3) + "strSql.Append(\";select @@IDENTITY\");" + "\r\n");
            }

            strclass.Append(Space(3) + "" + DbParaHead + "Parameter[] parameters = {" + "\r\n");
            strclass.Append(strclass3.Value);
            strclass.Append("};" + "\r\n");
            strclass.Append(strclass4.Value + "\r\n");

            if ((dbobj.DbType == "SQL2000" || dbobj.DbType == "SQL2005" || dbobj.DbType == "SQL2008") && (IsHasIdentity))
            {
                strclass.Append(CodeCommon.Space(3) + "object obj = " + DbHelperName + ".GetSingle(strSql.ToString(),parameters);" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "if (obj == null)" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "{" + "\r\n");
                strclass.Append(CodeCommon.Space(4) + "return 1;" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "}" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "else" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "{" + "\r\n");
                strclass.Append(CodeCommon.Space(4) + "return Convert.ToInt32(obj);" + "\r\n");
                strclass.Append(CodeCommon.Space(3) + "}" + "\r\n");

            }
            else
            {
                strclass.Append(CodeCommon.Space(3) + "" + DbHelperName + ".ExecuteSql(strSql.ToString(),parameters);" + "\r\n");           
            }


            strclass.Append(Space(2) + "}");
            return strclass.ToString();
        }

        /// <summary>
        /// �õ�Update�����Ĵ���
        /// </summary>
        /// <param name="DbName"></param>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <param name="ModelName"></param>
        /// <returns></returns>
        private string CreatUpdateParam()
        {           
            StringPlus strclass = new StringPlus();
            StringPlus strclass1 = new StringPlus();
            StringPlus strclass2 = new StringPlus();

            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ����һ������");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public void Update()");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpaceLine(3, "strSql.Append(\"update " + TableName + " set \");");
            int n = 0;
            foreach (ColumnInfo field in Fieldlist)
                {
                    string columnName = field.ColumnName;
                    string columnType = field.TypeName;
                    string Length = field.Length;
                    bool IsIdentity = field.IsIdentity;
                    bool isPK = field.IsPK;

                    strclass1.AppendSpaceLine(5, "new " + DbParaHead + "Parameter(\"" + preParameter + columnName + "\", " + DbParaDbType + "." + CodeCommon.DbTypeLength(_dbtype, columnType, Length) + "),");
                    strclass2.AppendSpaceLine(3, "parameters[" + n + "].Value = " + columnName + ";");
                    n++;

                    if (field.IsIdentity || field.IsPK || (Keys.Contains(field)))
                    {
                        continue;
                    }
                    strclass.AppendSpaceLine(3, "strSql.Append(\"" + columnName + "=" + preParameter + columnName + ",\");");
                    
                }
            

            if (strclass.Value.IndexOf(",") > 0)
            {
                strclass.DelLastComma();
            }
            strclass.AppendLine("\");");
            if (GetWhereExpression(Keys).Length > 0)
            {
                strclass.AppendSpace(3, "strSql.Append(\" where " + GetWhereExpression(Keys) + "\");" + "\r\n");
            }
            else
            {
                strclass.AppendSpace(3,"//strSql.Append(\" where ����);" + "\r\n");
            }
            strclass.AppendSpaceLine(3, "" + DbParaHead + "Parameter[] parameters = {");
            if (strclass1.Value.IndexOf(",") > 0)
            {
                strclass1.DelLastComma();
            }
            strclass.Append(strclass1.Value);
            strclass.AppendLine("};");
            strclass.AppendLine(strclass2.Value);
            strclass.AppendSpaceLine(3, "" + DbHelperName + ".ExecuteSql(strSql.ToString(),parameters);");
            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }
        /// <summary>
        /// �õ�Delete�Ĵ���
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatDeleteParam()
        {
            StringPlus strclass = new StringPlus();

            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ɾ��һ������");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public void Delete(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            //if (dbobj.DbType != "OleDb")
            //{
            //    strclass.AppendSpaceLine(3, "strSql.Append(\"delete " + TableName + " \");");
            //}
            //else
            //{
                strclass.AppendSpaceLine(3, "strSql.Append(\"delete from " + TableName + " \");");
            //}
         
            if (GetWhereExpression(Keys).Length > 0)
            {
                strclass.AppendSpace(3, "strSql.Append(\" where " + GetWhereExpression(Keys) + "\");" + "\r\n");
            }
            else
            {
                strclass.AppendSpace(3, "//strSql.Append(\" where ����);" + "\r\n");
            }
            
            strclass.AppendLine(GetPreParameter(Keys));

           
            
            strclass.AppendSpaceLine(3, "" + DbHelperName + ".ExecuteSql(strSql.ToString(),parameters);");
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;
        }

        /// <summary>
        /// �õ�GetModel()�Ĵ���
        /// </summary>      
        private string CreatConstructorParam()
        {            
            StringBuilder strclass = new StringBuilder();         
            strclass.Append("\r\n");
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// �õ�һ������ʵ��" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            strclass.Append(Space(2) + "public "+ModelName+"(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            strclass.Append(Space(3) + "StringBuilder strSql=new StringBuilder();" + "\r\n");

            strclass.Append(Space(3) + "strSql.Append(\"select ");
            strclass.Append(Fieldstrlist + " \");" + "\r\n");
            strclass.Append(Space(3) + "strSql.Append(\" FROM " + TableName + " \");" + "\r\n");

            if (GetWhereExpression(Keys).Length > 0)
            {
                strclass.Append(Space(3) + "strSql.Append(\" where " + GetWhereExpression(Keys) + "\");" + "\r\n");
            }
            else
            {
                strclass.Append(Space(3) + "//strSql.Append(\" where ����);" + "\r\n");
            }
            strclass.AppendLine(GetPreParameter(Keys));

            strclass.Append(Space(3) + "DataSet ds=" + DbHelperName + ".Query(strSql.ToString(),parameters);" + "\r\n");
            strclass.Append(Space(3) + "if(ds.Tables[0].Rows.Count>0)" + "\r\n");
            strclass.Append(Space(3) + "{" + "\r\n");
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                switch (CodeCommon.DbTypeToCS(columnType))
                {
                    case "int":
                        {
                            strclass.Append(Space(4) + "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" + "\r\n");
                            strclass.Append(Space(4) + "{" + "\r\n");
                            strclass.Append(Space(5) + "" + columnName + "=int.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());" + "\r\n");
                            strclass.Append(Space(4) + "}" + "\r\n");
                        }
                        break;
                    case "decimal":
                        {
                            strclass.Append(Space(4) + "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" + "\r\n");
                            strclass.Append(Space(4) + "{" + "\r\n");
                            strclass.Append(Space(5) + "" + columnName + "=decimal.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());" + "\r\n");
                            strclass.Append(Space(4) + "}" + "\r\n");
                        }
                        break;
                    case "DateTime":
                        {
                            strclass.Append(Space(4) + "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" + "\r\n");
                            strclass.Append(Space(4) + "{" + "\r\n");
                            strclass.Append(Space(5) + "" + columnName + "=DateTime.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());" + "\r\n");
                            strclass.Append(Space(4) + "}" + "\r\n");
                        }
                        break;
                    case "string":
                        {
                            strclass.Append(Space(4) + "" + columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();" + "\r\n");
                        }
                        break;
                    case "bool":
                        {
                            strclass.Append(Space(4) + "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" + "\r\n");
                            strclass.Append(Space(4) + "{" + "\r\n");
                            strclass.Append(Space(5) + "if((ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()==\"1\")||(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString().ToLower()==\"true\"))" + "\r\n");
                            strclass.Append(Space(5) + "{" + "\r\n");
                            strclass.Append(Space(6) + "" + columnName + "=true;" + "\r\n");
                            strclass.Append(Space(5) + "}" + "\r\n");
                            strclass.Append(Space(5) + "else" + "\r\n");
                            strclass.Append(Space(5) + "{" + "\r\n");
                            strclass.Append(Space(6) + "" + columnName + "=false;" + "\r\n");
                            strclass.Append(Space(5) + "}" + "\r\n");
                            strclass.Append(Space(4) + "}" + "\r\n" + "\r\n");
                        }
                        break;
                    case "byte[]":
                        {
                            strclass.Append(Space(4) + "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")" + "\r\n");
                            strclass.Append(Space(4) + "{" + "\r\n");
                            strclass.Append(Space(5) + "" + columnName + "=(byte[])ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();" + "\r\n");
                            strclass.Append(Space(4) + "}" + "\r\n");
                        }
                        break;
                    default:
                        strclass.Append(Space(4) + "" + columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();" + "\r\n");
                        break;

                }

            }

            strclass.Append(Space(3) + "}" + "\r\n");
            strclass.Append(Space(2) + "}");
            return strclass.ToString();
        }
        /// <summary>
        /// �õ�GetModel()�Ĵ���
        /// </summary>      
        private string CreatGetModelParam()
        {

            StringPlus strclass = new StringPlus();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>" );
            strclass.AppendSpaceLine(2, "/// �õ�һ������ʵ��" );
            strclass.AppendSpaceLine(2, "/// </summary>" );
            strclass.AppendSpaceLine(2, "public void GetModel(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")" );
            strclass.AppendSpaceLine(2, "{" );
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();" );

            strclass.AppendSpace(3, "strSql.Append(\"select ");
            if (dbobj.DbType == "SQL2005" || dbobj.DbType == "SQL2000" || dbobj.DbType == "SQL2008")
            {
                strclass.Append(" top 1 ");
            }
            strclass.AppendLine(Fieldstrlist + " \");" );
            strclass.AppendSpaceLine(3, "strSql.Append(\" FROM " + TableName + " \");" );

            if (GetWhereExpression(Keys).Length > 0)
            {
                strclass.AppendSpaceLine(3, "strSql.Append(\" where " + GetWhereExpression(Keys) + "\");" );
            }
            else
            {
                strclass.AppendSpaceLine(3, "//strSql.Append(\" where ����);" );               
            }
            strclass.AppendLine(GetPreParameter(Keys));

            strclass.AppendSpaceLine(3, "DataSet ds=" + DbHelperName + ".Query(strSql.ToString(),parameters);" );
            strclass.AppendSpaceLine(3, "if(ds.Tables[0].Rows.Count>0)" );
            strclass.AppendSpaceLine(3, "{" );
            #region �ֶθ�ֵ
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                switch (CodeCommon.DbTypeToCS(columnType))
                {
                    case "int":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, "model." + columnName + "=int.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
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
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, "model." + columnName + "=decimal.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
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
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, "model." + columnName + "=DateTime.Parse(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "string":
                        {
                            strclass.AppendSpaceLine(4, "model." + columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();");
                        }
                        break;
                    case "bool":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, "if((ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()==\"1\")||(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString().ToLower()==\"true\"))");
                            strclass.AppendSpaceLine(5, "{");
                            strclass.AppendSpaceLine(6, "model." + columnName + "=true;");
                            strclass.AppendSpaceLine(5, "}");
                            strclass.AppendSpaceLine(5, "else");
                            strclass.AppendSpaceLine(5, "{");
                            strclass.AppendSpaceLine(6, "model." + columnName + "=false;");
                            strclass.AppendSpaceLine(5, "}");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "byte[]":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, "model." + columnName + "=(byte[])ds.Tables[0].Rows[0][\"" + columnName + "\"];");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    case "Guid":
                        {
                            strclass.AppendSpaceLine(4, "if(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString()!=\"\")");
                            strclass.AppendSpaceLine(4, "{");
                            strclass.AppendSpaceLine(5, "model." + columnName + "=new Guid(ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString());");
                            strclass.AppendSpaceLine(4, "}");
                        }
                        break;
                    default:
                        strclass.AppendSpaceLine(4, "//model." + columnName + "=ds.Tables[0].Rows[0][\"" + columnName + "\"].ToString();");
                        break;
                }
            }
            #endregion       
            strclass.AppendSpaceLine(3, "}" );
            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }

        /// <summary>
        /// �õ�GetList()�Ĵ���
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string CreatGetListParam()
        {            
            StringPlus strclass = new StringPlus();            
            strclass.AppendSpaceLine(2, "/// <summary>" );
            strclass.AppendSpaceLine(2, "/// ��������б�");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetList(string strWhere)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpace(3, "strSql.Append(\"select ");
            strclass.AppendLine("* \");" );
            strclass.AppendSpaceLine(3, "strSql.Append(\" FROM " + TableName + " \");");           
            strclass.AppendSpaceLine(3, "if(strWhere.Trim()!=\"\")");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "strSql.Append(\" where \"+strWhere);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "return " + DbHelperName + ".Query(strSql.ToString());");
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;
        }

        #endregion

    }
}
