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
    /// ���ӱ��model����
    /// </summary>
    public class BuilderModelT : BuilderModel
    {
        #region  ��������
        private string _tablenameson;
        private string _modelnameson; //model����       
        private string _namespaceson = "Maticsoft"; //���������ռ��� 
        private string _folderson; //�����ļ���
        private string _modelpathson;
        private List<ColumnInfo> _fieldlistson;
               
        public string TableNameSon
        {
            set { _tablenameson = value; }
            get { return _tablenameson; }
        }
        public string ModelNameSon
        {
            set { _modelnameson = value; }
            get { return _modelnameson; }
        }
        public string NameSpaceSon
        {
            set { _namespaceson = value; }
            get { return _namespaceson; }
        }
        public string FolderSon
        {
            set { _folderson = value; }
            get { return _folderson; }
        }
        /// <summary>
        /// ʵ����������ռ�
        /// </summary>
        public string ModelpathSon
        {
            set { _modelpathson = value; }
            get { return _modelpathson; }
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
        /// ѡ����ֶμ��ϵ�-�ַ���
        /// </summary>
        public string FieldsSon
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
               
        public BuilderModelT(IDbObject idbobj, string DbName, string tableNameParent, string modelNameParent,List<ColumnInfo> FieldlistP,
                       string tableNameSon,string modelNameSon,List<ColumnInfo> FieldlistS,string NameSpace, string Folder, string Modelpath)
        {
            dbobj = idbobj;
            _dbname = DbName;
            _tablename = TableName;
            _modelname = ModelName;
            _tablenameson = tableNameSon;
            _modelnameson = modelNameSon;
            _namespace = NameSpace;
            _folder = Folder;
            _modelpath = Modelpath;
            Fieldlist = FieldlistP;
            _fieldlistson = FieldlistS;

        }

        #region ������������Model��

        /// <summary>
        /// ������������Model��
        /// </summary>		
        public string CreatModelMethodT()
        {
            
            StringPlus strclass = new StringPlus();
         
            strclass.AppendLine(CreatModelMethod());

            strclass.AppendSpaceLine(2, "private List<" + ModelNameSon + "> _" + ModelNameSon.ToLower() + "s;");//˽�б���
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// ���� ");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public List<" + ModelNameSon + "> " + ModelNameSon + "s");//����
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "set{" + " _" + ModelNameSon.ToLower() + "s=value;}");
            strclass.AppendSpaceLine(3, "get{return " + "_" + ModelNameSon.ToLower() + "s;}");
            strclass.AppendSpaceLine(2, "}");
                        
            return strclass.ToString();
        }
        #endregion
                
    }
}
