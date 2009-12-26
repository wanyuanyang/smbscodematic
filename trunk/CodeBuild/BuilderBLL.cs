using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using LTP.Utility;
using LTP.IDBO;
using LTP.DBFactory;
using LTP.CodeHelper;
namespace LTP.CodeBuild
{
    /// <summary>
    /// ҵ��㷽������
    /// </summary>
    class BuilderBLL
    {
        #region ˽�б���
        protected string _key = "ID";//Ĭ�ϵ�һ�������ֶ�		
        protected string _keyType = "int";//Ĭ�ϵ�һ����������
        #endregion

        #region ��������

        private List<ColumnInfo> _keys; //�����������ֶ��б�       
        private string _modelspace;
        private bool isHasIdentity;
        private string _modelname;//model����      

        public List<ColumnInfo> Keys
        {
            set { _keys = value; }
            get { return _keys; }
        }       
        /// <summary>
        /// ʵ��������������ռ� + ����
        /// </summary>
        public string ModelSpace
        {
            set { _modelspace = value; }
            get
            { return _modelspace; }
        }
        public string ModelName
        {
            set { _modelname = value; }
            get { return _modelname; }
        }
        public bool IsHasIdentity
        {
            set { isHasIdentity = value; }
            get { return isHasIdentity; }
        }
               
        #endregion		

       
        public BuilderBLL(List<ColumnInfo> keys, string modelspace)
        {
            _modelspace = modelspace;
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
        public string Space(int num)
        {
            StringBuilder str = new StringBuilder();
            for (int n = 0; n < num; n++)
            {
                str.Append("\t");
            }
            return str.ToString();
        }
        
        #region ҵ��㷽��
        
        //public string GetBLLCode(bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List)
        //{
        //    StringBuilder strclass = new StringBuilder();

        //    strclass.Append("using System;\r\n");
        //    strclass.Append("using System.Data;\r\n");
        //    if (GetModelByCache)
        //    {
        //        strclass.Append("using LTP.Common;\r\n");
        //    }
        //    strclass.Append("using " + Modelpath + ";\r\n");
        //    strclass.Append("namespace " + BLLpath + "\r\n");
        //    strclass.Append("{" + "\r\n");
        //    strclass.Append(Space(1) + "/// <summary>" + "\r\n");
        //    strclass.Append(Space(1) + "/// ҵ���߼���" + ModelName + " ��ժҪ˵����" + "\r\n");
        //    strclass.Append(Space(1) + "/// </summary>" + "\r\n");
        //    strclass.Append(Space(1) + "public class " + ModelName + "\r\n");
        //    strclass.Append(Space(1) + "{" + "\r\n");
        //    strclass.Append(Space(2) + "private readonly " + DALSpace + " dal=" + "new " + DALSpace + "();" + "\r\n");
        //    strclass.Append(Space(2) + "public " + ModelName + "()" + "\r\n");
        //    strclass.Append(Space(2) + "{}" + "\r\n");
        //    strclass.Append(Space(2) + "#region  ��Ա����" + "\r\n");

        //    #region  ��������
        //    if (Maxid)
        //    {
        //        if (Keys.Count > 0)
        //        {
        //            foreach (ColumnInfo obj in Keys)
        //            {
        //                if (CodeCommon.DbTypeToCS(obj.TypeName) == "int")
        //                {
        //                    if (obj.IsPK)
        //                    {
        //                        strclass.Append(bll.CreatBLLGetMaxID() + "\r\n");
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (Exists)
        //    {
        //        strclass.Append(bll.CreatBLLExists() + "\r\n");
        //    }
        //    if (Add)
        //    {
        //        strclass.Append(bll.CreatBLLADD() + "\r\n");
        //    }
        //    if (Update)
        //    {
        //        strclass.Append(bll.CreatBLLUpdate() + "\r\n");
        //    }
        //    if (Delete)
        //    {
        //        strclass.Append(bll.CreatBLLDelete() + "\r\n");
        //    }
        //    if (GetModel)
        //    {
        //        strclass.Append(bll.CreatBLLGetModel() + "\r\n");
        //    }
        //    if (GetModelByCache)
        //    {
        //        strclass.Append(bll.CreatBLLGetModelByCache(ModelName) + "\r\n");
        //    }
        //    if (List)
        //    {
        //        strclass.Append(bll.CreatBLLGetList() + "\r\n");
        //        strclass.Append(bll.CreatBLLGetAllList() + "\r\n");
        //        strclass.Append(bll.CreatBLLGetListByPage() + "\r\n");
        //    }

        //    #endregion
        //    strclass.Append(Space(2) + "#endregion  ��Ա����" + "\r\n");
        //    strclass.Append(Space(1) + "}" + "\r\n");
        //    strclass.Append("}" + "\r\n");
        //    strclass.Append("\r\n");

        //    return strclass.ToString();
        //}

        

        public string CreatBLLGetMaxID()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.Append("\r\n");
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// �õ����ID" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            strclass.Append(Space(2) + "public int GetMaxId()" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            strclass.Append(Space(3) + "return dal.GetMaxId();" + "\r\n");
            strclass.Append(Space(2) + "}\r\n");
            return strclass.ToString();
        }
        public string CreatBLLExists()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// �Ƿ���ڸü�¼" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            strclass.Append(Space(2) + "public bool Exists(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            strclass.Append(Space(3) + "return dal.Exists(" + LTP.CodeHelper.CodeCommon.GetFieldstrlist(Keys) + ");" + "\r\n");
            strclass.Append(Space(2) + "}\r\n");
            return strclass.ToString();
        }
        public string CreatBLLADD()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// ����һ������" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            string strretu = "void";
            if (IsHasIdentity)
            {
                strretu = "int ";
            }            
            strclass.Append(Space(2) + "public " + strretu + " Add(" + ModelSpace + " model)" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            if (strretu == "void")
            {
                strclass.Append(Space(3) + "dal.Add(model);" + "\r\n");
            }
            else
            {
                strclass.Append(Space(3) + "return dal.Add(model);" + "\r\n");
            }
            strclass.Append(Space(2) + "}\r\n");
            return strclass.ToString();
        }
        public string CreatBLLUpdate()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// ����һ������" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            strclass.Append(Space(2) + "public void Update(" + ModelSpace + " model)" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            strclass.Append(Space(3) + "dal.Update(model);" + "\r\n");
            strclass.Append(Space(2) + "}\r\n");
            return strclass.ToString();
        }
        public string CreatBLLDelete()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// ɾ��һ������" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            strclass.Append(Space(2) + "public void Delete(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            strclass.Append(Space(3) + "dal.Delete(" + LTP.CodeHelper.CodeCommon.GetFieldstrlist(Keys) + ");" + "\r\n");
            strclass.Append(Space(2) + "}\r\n");
            return strclass.ToString();
        }
        public string CreatBLLGetModel()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// �õ�һ������ʵ��" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            strclass.Append(Space(2) + "public " + ModelSpace + " GetModel(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            strclass.Append(Space(3) + "return dal.GetModel(" + LTP.CodeHelper.CodeCommon.GetFieldstrlist(Keys) + ");" + "\r\n");
            strclass.Append(Space(2) + "}\r\n");
            return strclass.ToString();

        }
        public string CreatBLLGetModelByCache(string ModelName)
        {
            StringPlus strclass = new StringPlus();            
            strclass.AppendSpaceLine(2, "/// <summary>" );
            strclass.AppendSpaceLine(2, "/// �õ�һ������ʵ�壬�ӻ����С�");
            strclass.AppendSpaceLine(2, "/// </summary>" );
            strclass.AppendSpaceLine(2, "public " + ModelSpace + " GetModelByCache(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ")");
            strclass.AppendSpaceLine(2, "{" );
            strclass.AppendSpaceLine(3, "string CacheKey = \"" + ModelName + "Model-\" + " + _key + ";");
            strclass.AppendSpaceLine(3, "object objModel = LTP.Common.DataCache.GetCache(CacheKey);");
            strclass.AppendSpaceLine(3, "if (objModel == null)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "try");
            strclass.AppendSpaceLine(4, "{");
            strclass.AppendSpaceLine(5, "objModel = dal.GetModel(" + LTP.CodeHelper.CodeCommon.GetFieldstrlist(Keys) + ");");
            strclass.AppendSpaceLine(5, "if (objModel != null)");
            strclass.AppendSpaceLine(5, "{");
            strclass.AppendSpaceLine(6, "int ModelCache = LTP.Common.ConfigHelper.GetConfigInt(\"ModelCache\");");
            strclass.AppendSpaceLine(6, "LTP.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);");
            strclass.AppendSpaceLine(5, "}");
            strclass.AppendSpaceLine(4, "}");
            strclass.AppendSpaceLine(4, "catch{}");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "return (" + ModelSpace + ")objModel;");
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;

        }
        public string CreatBLLGetList()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// ��������б�" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            strclass.Append(Space(2) + "public DataSet GetList(string strWhere)" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            strclass.Append(Space(3) + "return dal.GetList(strWhere);" + "\r\n");
            strclass.Append(Space(2) + "}\r\n");

            return strclass.ToString();

        }
        public string CreatBLLGetAllList()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// ��������б�" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            strclass.Append(Space(2) + "public DataSet GetAllList()" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            strclass.Append(Space(3) + "return GetList(\"\");" + "\r\n");
            strclass.Append(Space(2) + "}\r\n");

            return strclass.ToString();

        }
        public string CreatBLLGetListByPage()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// ��������б�" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            strclass.Append(Space(2) + "//public DataSet GetList(int PageSize,int PageIndex,string strWhere)" + "\r\n");
            strclass.Append(Space(2) + "//{" + "\r\n");
            strclass.Append(Space(3) + "//return dal.GetList(PageSize,PageIndex,strWhere);" + "\r\n");
            strclass.Append(Space(2) + "//}\r\n");

            return strclass.ToString();

        }
        
        #endregion
    }
}
