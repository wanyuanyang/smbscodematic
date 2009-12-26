using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using LTP.IDBO;
using LTP.Utility;
using LTP.CodeHelper;
namespace LTP.CodeBuild
{
    /// <summary>
    /// ��ܴ������
    /// </summary>
    public class BuilderFrame
    {
        #region  ˽�б���
        protected IDbObject dbobj;
        protected string _dbtype;     
        protected string _key = "ID";//Ĭ�ϵ�һ�������ֶ�		
        protected string _keyType = "int";//Ĭ�ϵ�һ����������

        #endregion

        #region  ��������
        private string _dbname;
        private string _tablename;
        private string _modelname;//model���� 
        private string _bllname;//bll����    
        private string _dalname;//dal����    
        private string _namespace = "Maticsoft";//���������ռ���
        private string _folder;//�����ļ���
        private string _dbhelperName;//���ݿ��������
        private List<ColumnInfo> _keys; //�����������ֶ��б�
        private List<ColumnInfo> _fieldlist;

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
        //ѡ����ֶμ����ַ���
        public string Fields
        {
            get
            {
                StringPlus _fields = new StringPlus();
                foreach (object obj in Fieldlist)
                {
                    _fields.Append("'" + obj.ToString() + "',");
                }
                _fields.DelLastComma();
                return _fields.Value;
            }
        }

        #endregion

        #region ��������
        private string _modelpath;        
        private string _dalpath;           
        private string _idalpath;       
        private string _bllpath;        
        //private string _factoryclass;

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
            get
            {
                string strdbtype = _dbtype;
                if (_dbtype == "SQL2000" || _dbtype == "SQL2005")
                {
                    strdbtype = "SQLServer";
                }
                _dalpath = _namespace + "." + strdbtype + "DAL";
                if (_folder.Trim() != "")
                {
                    _dalpath += "." + _folder;
                }
                return _dalpath;
            }
            set { _dalpath = value; }
        }        
        /// <summary>
        /// ���ݲ�Ĳ��������ƶ���
        /// </summary>
        public string DALSpace
        {            
            get
            {
                return DALpath + "." + DALName;
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
                //if (_folder.Trim() != "")
                //{
                //    factorypath += "." + _folder;
                //}
                return factorypath;
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
                    case "MySQL":
                        return "MySql";                        
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
                    case "MySQL":
                        return "MySqlDbType";
                    case "OleDb":
                        return "OleDbType";
                    default:
                        return "SqlDbType";
                }
            }
        }
        /// <summary>
        /// �ֶε� select �б�
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
        /// <summary>
        /// ������ʶ�ֶ�
        /// </summary>
        public string Key
        {
            get
            {
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
                return _key;
            }
        }
        #endregion	

        /// <summary>
        /// �õ����������ֶβ��������б�
        /// </summary>
        /// <param name="Keys"></param>
        /// <returns></returns>
        public string GetkeyParalist(Hashtable Keys)
        {
            StringPlus strlist = new StringPlus();
            foreach (DictionaryEntry key in Keys)
            {
                strlist.Append(CodeCommon.DbTypeToCS(key.Value.ToString()) + " " + key.Key.ToString() + ",");
            }
            if (strlist.Value.IndexOf(",") > 0)
            {
                strlist.DelLastComma();
            }            
            return strlist.Value;
        }
        
        /// <summary>
        /// �õ��������������ʽ
        /// </summary>
        /// <param name="Keys"></param>
        /// <returns></returns>
        public string GetkeyWherelist(Hashtable Keys)
        {
            StringPlus strlist = new StringPlus();
            int n = 0;
            foreach (DictionaryEntry key in Keys)
            {
                n++;
                if (CodeCommon.IsAddMark(key.Value.ToString()))
                {
                    strlist.Append(key.Key.ToString() + "='\"+" + key.Key.ToString() + "+\"'\"");
                }
                else
                {
                    strlist.Append(key.Key.ToString() + "=\"+" + key.Key.ToString() + "+\"");
                    if (n == Keys.Count)
                    {
                        strlist.Append("\"");
                    }
                }                
                strlist.Append(" and ");
            }

            if (strlist.Value.IndexOf("and") > 0)
            {
                strlist.DelLastChar("and");
            }            
            return strlist.Value;
        }
        
        /// <summary>
        /// �õ��������������ʽ(�洢���̲��� )
        /// </summary>
        /// <param name="Keys"></param>
        /// <returns></returns>
        public string GetkeyWherelistProc(Hashtable Keys)
        {
            StringPlus strlist = new StringPlus();
            foreach (DictionaryEntry key in Keys)
            {               
                strlist.Append(key.Key.ToString() + "=@" + key.Key.ToString() );               
                strlist.Append(" and ");
            }
            if (strlist.Value.IndexOf("and") > 0)
            {
                strlist.DelLastChar("and");
            }
            return strlist.Value;
        }

        /// <summary>
        /// �õ��ֶ��б�
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string GetFieldslist(DataTable dt)
        {
            StringPlus strclass = new StringPlus();
            foreach (DataRow row in dt.Rows)
            {
                string columnName = row["ColumnName"].ToString();
                strclass.Append("[" + columnName + "],");
            }
            strclass.DelLastComma();
            return strclass.Value;
        }

        public string Space(int num)
        {
            StringBuilder str = new StringBuilder();
            for (int n = 0; n < num; n++)
            {
                str.Append("\t");
            }
            return str.ToString();
        }
    }
}
