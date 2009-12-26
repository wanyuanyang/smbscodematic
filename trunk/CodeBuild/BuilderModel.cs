using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LTP.IDBO;
using LTP.CodeHelper;
using LTP.Utility;
namespace LTP.CodeBuild
{
    /// <summary>
    /// Model��s
    /// </summary>
    public class BuilderModel
    {
        #region ˽�б���
        protected IDbObject dbobj;
        #endregion


        #region ��������

        protected string _dbname;
        protected string _tablename;
        protected string _modelname; //model����       
        protected string _namespace = "Maticsoft"; //���������ռ��� 
        protected string _folder; //�����ļ���
        protected string _modelpath;
        protected List<ColumnInfo> _fieldlist;

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
        public string ModelName
        {
            set { _modelname = value; }
            get { return _modelname; }
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
        /// <summary>
        /// ʵ����������ռ�
        /// </summary>
        public string Modelpath
        {
            set { _modelpath = value; }
            get { return _modelpath; }
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

        #endregion

        public BuilderModel()
        {            
        }
        public BuilderModel(IDbObject idbobj)
        {
            dbobj = idbobj;
        }
        public BuilderModel(IDbObject idbobj, string DbName, string TableName, string ModelName,
            string NameSpace, string Folder, string Modelpath, List<ColumnInfo> fieldlist)
        {
            dbobj = idbobj;
            _dbname = DbName;
            _tablename = TableName;
            _modelname = ModelName;
            _namespace = NameSpace;
            _folder = Folder;
            _modelpath = Modelpath;
            Fieldlist = fieldlist;

        }


        #region ������������Model��

        /// <summary>
        /// ������������Model��
        /// </summary>		
        public string CreatModel()
        {
            if (_modelname == "")
            {
                _modelname = TableName;
            }
            StringPlus strclass = new StringPlus();
            StringPlus strclass1 = new StringPlus();
            StringPlus strclass2 = new StringPlus();
            strclass.AppendLine("using System;");

            strclass.AppendLine("namespace " + Modelpath);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// ʵ����" + _modelname + " ��(����˵���Զ���ȡ���ݿ��ֶε�������Ϣ)");
            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpaceLine(1, "public class " + _modelname);
            strclass.AppendSpaceLine(1, "{");
            strclass.AppendSpaceLine(2, "public " + _modelname + "()");
            strclass.AppendSpaceLine(2, "{}");
            strclass.AppendLine(CreatModelMethod());
            strclass.AppendSpaceLine(1, "}");
            strclass.AppendLine("}");
            strclass.AppendLine("");

            return strclass.ToString();
        }
        #endregion

        #region ����Model���Բ���
        /// <summary>
        /// ����ʵ���������
        /// </summary>
        /// <returns></returns>
        public string CreatModelMethod()
        {

            StringPlus strclass = new StringPlus();
            StringPlus strclass1 = new StringPlus();
            StringPlus strclass2 = new StringPlus();

            strclass.AppendSpaceLine(2, "#region Model");
                        
            foreach (ColumnInfo field in Fieldlist)
            {
                string columnName = field.ColumnName;
                string columnType = field.TypeName;
                bool IsIdentity = field.IsIdentity;
                bool ispk=field.IsPK;
                bool cisnull=field.cisNull;
                
                string deText = field.DeText;

                columnType = CodeCommon.DbTypeToCS(columnType);
                string isnull = "";
                if (CodeCommon.isValueType(columnType))
                {
                    if ((!IsIdentity) && (!ispk) && (cisnull))
                    {
                        isnull = "?";
                    }
                }
                strclass1.AppendSpaceLine(2, "private " + columnType + isnull + " _" + columnName.ToLower() + ";");//˽�б���
                strclass2.AppendSpaceLine(2, "/// <summary>");
                strclass2.AppendSpaceLine(2, "/// " + deText);
                strclass2.AppendSpaceLine(2, "/// </summary>");
                strclass2.AppendSpaceLine(2, "public " + columnType + isnull + " " + columnName);//����
                strclass2.AppendSpaceLine(2, "{");
                strclass2.AppendSpaceLine(3, "set{" + " _" + columnName.ToLower() + "=value;}");
                strclass2.AppendSpaceLine(3, "get{return " + "_" + columnName.ToLower() + ";}");
                strclass2.AppendSpaceLine(2, "}");
            }

            strclass.Append(strclass1.Value);
            strclass.Append(strclass2.Value);
            strclass.AppendSpaceLine(2, "#endregion Model");

            return strclass.ToString();
        }

        #endregion

    }
}
