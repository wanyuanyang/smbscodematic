using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using LTP.IDBO;
using LTP.Utility;
using LTP.DBFactory;
using LTP.IBuilder;
using LTP.CodeHelper;
namespace LTP.CodeBuild
{
    public class BuilderFrameF3 : BuilderFrame
    {
        #region  ˽�б���
        IBuilderDAL idal;
        IBuilderBLL ibll;
        IBuilderDALTran idaltran;        
        #endregion


        #region ����
        public BuilderFrameF3(IDbObject idbobj, string dbName, string tableName, string modelName, string bllName, string dalName, 
            List<ColumnInfo> fieldlist, List<ColumnInfo> keys, 
            string nameSpace, string folder, string dbHelperName)
        {
            dbobj = idbobj;
            _dbtype = idbobj.DbType;
            DbName = dbName;
            TableName = tableName;
            ModelName = modelName;
            BLLName = bllName;
            DALName = dalName;
            NameSpace = nameSpace;
            DbHelperName = dbHelperName;
            Folder = folder;            
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
        }
        public BuilderFrameF3(IDbObject idbobj, string dbName,string nameSpace, string folder, string dbHelperName)
        {
            dbobj = idbobj;
            _dbtype = idbobj.DbType;
            DbName = dbName;
            NameSpace = nameSpace;
            DbHelperName = dbHelperName;
            Folder = folder;

        }
        #endregion



        #region ����Model
        /// <summary>
		/// �õ�Model��
		/// </summary>		
        public string GetModelCode()
        {            
            //return model.CreatModel();
            LTP.BuilderModel.BuilderModel model = new LTP.BuilderModel.BuilderModel();
            model.ModelName = ModelName;
            model.NameSpace = NameSpace;
            model.Fieldlist = Fieldlist;
            model.Modelpath = Modelpath;
            return model.CreatModel();
        }

        /// <summary>
        /// �õ����ӱ�Model
        /// </summary>		
        public string GetModelCode(string tableNameParent, string modelNameParent, List<ColumnInfo> FieldlistP,
                       string tableNameSon, string modelNameSon, List<ColumnInfo> FieldlistS)
        {
            if (modelNameParent == "")
            {
                modelNameParent = tableNameParent;
            }
            if (modelNameSon == "")
            {
                modelNameSon = tableNameSon;
            }
            StringPlus strclass = new StringPlus();
            StringPlus strclass1 = new StringPlus();
            StringPlus strclass2 = new StringPlus();
            strclass.AppendLine("using System;");
            strclass.AppendLine("using System.Collections.Generic;");
            strclass.AppendLine("namespace " + Modelpath);
            strclass.AppendLine("{");

            //����
            //LTP.BuilderModel.BuilderModelT modelP = new LTP.BuilderModel.BuilderModelT(dbobj, DbName, tableNameParent, modelNameParent, FieldlistP,
            //    tableNameSon, modelNameSon, FieldlistS, NameSpace, Folder, Modelpath);
            LTP.BuilderModel.BuilderModelT modelP = new LTP.BuilderModel.BuilderModelT();
            modelP.ModelName = modelNameParent;
            modelP.NameSpace = NameSpace;
            modelP.Fieldlist = FieldlistP;
            modelP.Modelpath = Modelpath;
            modelP.ModelNameSon = modelNameSon;

            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// ʵ����" + modelNameParent + " ��(����˵���Զ���ȡ���ݿ��ֶε�������Ϣ)");
            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpaceLine(1, "public class " + modelNameParent);
            strclass.AppendSpaceLine(1, "{");
            strclass.AppendSpaceLine(2, "public " + modelNameParent + "()");
            strclass.AppendSpaceLine(2, "{}");
            strclass.AppendLine(modelP.CreatModelMethodT());
            strclass.AppendSpaceLine(1, "}");

            //����
            //LTP.BuilderModel.BuilderModel modelS = new LTP.BuilderModel.BuilderModel(dbobj, DbName, tableNameSon, modelNameSon, NameSpace, Folder, Modelpath, FieldlistS);
            LTP.BuilderModel.BuilderModel modelS = new LTP.BuilderModel.BuilderModel();
            modelS.ModelName = modelNameSon;
            modelS.NameSpace = NameSpace;
            modelS.Fieldlist = FieldlistS;
            modelS.Modelpath = Modelpath;

            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// ʵ����" + modelNameSon + " ��(����˵���Զ���ȡ���ݿ��ֶε�������Ϣ)");
            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpaceLine(1, "public class " + modelNameSon);
            strclass.AppendSpaceLine(1, "{");
            strclass.AppendSpaceLine(2, "public " + modelNameSon + "()");
            strclass.AppendSpaceLine(2, "{}");
            strclass.AppendLine(modelS.CreatModelMethod());
            strclass.AppendSpaceLine(1, "}");

            strclass.AppendLine("}");
            strclass.AppendLine("");

            return strclass.ToString();
        }

        #endregion

        #region ���ݷ��ʲ����

        public string GetDALCode(string AssemblyGuid, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List, string procPrefix)
        {
            idal = BuilderFactory.CreateDALObj(AssemblyGuid);
            if (idal == null)
            {
                return "//��ѡ����Ч�����ݲ����������ͣ�";
            }
            idal.DbObject = dbobj;
            idal.DbName = DbName;
            idal.TableName = TableName;
            idal.Fieldlist = Fieldlist;
            idal.Keys = Keys;
            idal.NameSpace = NameSpace;            
            idal.Folder = Folder;            
            idal.Modelpath = Modelpath;
            idal.ModelName = ModelName;            
            idal.DALpath = DALpath;
            idal.DALName = DALName;
            idal.IDALpath = IDALpath;
            idal.IClass = IClass;
            idal.DbHelperName = DbHelperName;
            idal.ProcPrefix = procPrefix;
            return idal.GetDALCode(Maxid, Exists, Add, Update, Delete, GetModel, List);

        }
        /// <summary>
        /// ���ɸ��ӱ��������
        /// </summary>
        public string GetDALCodeTran(string AssemblyGuid, bool Maxid, bool Exists, bool Add, bool Update, bool Delete,
            bool GetModel, bool List, string procPrefix, string tableNameParent, string tableNameSon, string modelNameParent, string modelNameSon,
            List<ColumnInfo> fieldlistParent, List<ColumnInfo> fieldlistSon, List<ColumnInfo> keysParent, List<ColumnInfo> keysSon,
            string DALNameParent, string DALNameSon)
        {
            idaltran = BuilderFactory.CreateDALTranObj(AssemblyGuid);
            if (idaltran == null)
            {
                return "//��ѡ����Ч�����ݲ����������ͣ�";
            }
            idaltran.DbObject = dbobj;
            idaltran.DbName = DbName;
            idaltran.TableNameParent = tableNameParent;
            idaltran.TableNameSon = tableNameSon;
            idaltran.FieldlistParent = fieldlistParent;
            idaltran.FieldlistSon = fieldlistSon;
            idaltran.KeysParent = keysParent;
            idaltran.KeysSon = keysSon;
                        
            idaltran.NameSpace = NameSpace;            
            idaltran.Folder = Folder;            
            idaltran.Modelpath = Modelpath;
            idaltran.ModelNameParent = modelNameParent;
            idaltran.ModelNameSon = modelNameSon;
            idaltran.DALpath = DALpath;
            idaltran.DALNameParent = DALNameParent;
            idaltran.DALNameSon = DALNameSon;

            idaltran.IDALpath = IDALpath;
            idaltran.IClass = IClass;
            idaltran.DbHelperName = DbHelperName;
            idaltran.ProcPrefix = procPrefix;

            return idaltran.GetDALCode(Maxid, Exists, Add, Update, Delete, GetModel, List);

        }
        #endregion

        #region  �ӿڲ����

        /// <summary>
        /// �õ��ӿڲ����
        /// </summary>
        /// <param name="ID">����</param>
        /// <param name="ModelName">����</param>
        /// <returns></returns>
        public string GetIDALCode(bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List, bool ListProc)
        {
            StringBuilder strclass = new StringBuilder();
            strclass.Append("using System;\r\n");
            strclass.Append("using System.Data;\r\n");

            strclass.Append("namespace " + IDALpath + "\r\n");
            strclass.Append("{" + "\r\n");
            strclass.Append("	/// <summary>" + "\r\n");
            strclass.Append("	/// �ӿڲ�" + IClass + " ��ժҪ˵����" + "\r\n");
            strclass.Append("	/// </summary>" + "\r\n");
            strclass.Append("	public interface " + IClass + "\r\n");
            strclass.Append("	{\r\n");
            strclass.Append(Space(2) + "#region  ��Ա����" + "\r\n");

            if (Maxid)
            {
                if (Keys.Count > 0)
                {
                    foreach (ColumnInfo obj in Keys)
                    {
                        if (CodeCommon.DbTypeToCS(obj.TypeName) == "int")
                        {
                            if (obj.IsPK)
                            {
                                strclass.Append(Space(2) + "/// <summary>" + "\r\n");
                                strclass.Append(Space(2) + "/// �õ����ID" + "\r\n");
                                strclass.Append(Space(2) + "/// </summary>" + "\r\n");
                                strclass.Append("		int GetMaxId();" + "\r\n");
                                break;
                            }
                        }
                    }
                }
            }
            if (Exists)
            {
                if (Keys.Count > 0)
                {
                    strclass.Append(Space(2) + "/// <summary>" + "\r\n");
                    strclass.Append(Space(2) + "/// �Ƿ���ڸü�¼" + "\r\n");
                    strclass.Append(Space(2) + "/// </summary>" + "\r\n");
                    strclass.Append(Space(2) + "bool Exists(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ");" + "\r\n");
                }
            }
            if (Add)
            {
                strclass.Append(Space(2) + "/// <summary>" + "\r\n");
                strclass.Append(Space(2) + "/// ����һ������" + "\r\n");
                strclass.Append(Space(2) + "/// </summary>" + "\r\n");
                string strretu = "void";
                if (IsHasIdentity)
                {
                    strretu = "int";
                }                
                strclass.Append(Space(2)  + strretu + " Add(" + ModelSpace + " model);" + "\r\n");
                
            }
            if (Update)
            {
                strclass.Append(Space(2) + "/// <summary>" + "\r\n");
                strclass.Append(Space(2) + "/// ����һ������" + "\r\n");
                strclass.Append(Space(2) + "/// </summary>" + "\r\n");
                strclass.Append(Space(2) + "void Update(" + ModelSpace + " model);" + "\r\n");
            }
            if (Delete)
            {
                strclass.Append(Space(2) + "/// <summary>" + "\r\n");
                strclass.Append(Space(2) + "/// ɾ��һ������" + "\r\n");
                strclass.Append(Space(2) + "/// </summary>" + "\r\n");
                strclass.Append(Space(2) + "void Delete(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ");" + "\r\n");
            }
            if (GetModel)
            {
                strclass.Append(Space(2) + "/// <summary>" + "\r\n");
                strclass.Append(Space(2) + "/// �õ�һ������ʵ��" + "\r\n");
                strclass.Append(Space(2) + "/// </summary>" + "\r\n");
                strclass.Append(Space(2) + ModelSpace + " GetModel(" + LTP.CodeHelper.CodeCommon.GetInParameter(Keys) + ");" + "\r\n");
            }
            if (List)
            {
                strclass.Append(Space(2) + "/// <summary>" + "\r\n");
                strclass.Append(Space(2) + "/// ��������б�" + "\r\n");
                strclass.Append(Space(2) + "/// </summary>" + "\r\n");
                strclass.Append(Space(2) + "DataSet GetList(string strWhere);" + "\r\n");

                if ((dbobj.DbType == "SQL2000") ||
                (dbobj.DbType == "SQL2005") ||
                (dbobj.DbType == "SQL2008"))
                {
                    strclass.Append(Space(2) + "/// <summary>" + "\r\n");
                    strclass.Append(Space(2) + "/// ���ǰ��������" + "\r\n");
                    strclass.Append(Space(2) + "/// </summary>" + "\r\n");
                    strclass.Append(Space(2) + "DataSet GetList(int Top,string strWhere,string filedOrder);" + "\r\n");
                }
            }
            if (ListProc)
            {
                //strclass.Append(Space(2) + "/// <summary>" + "\r\n");
                //strclass.Append(Space(2) + "/// ��������б�" + "\r\n");
                //strclass.Append(Space(2) + "/// </summary>" + "\r\n");
                //strclass.Append("		DataSet GetList();" + "\r\n");

                strclass.Append(Space(2) + "/// <summary>" + "\r\n");
                strclass.Append(Space(2) + "/// ���ݷ�ҳ��������б�" + "\r\n");
                strclass.Append(Space(2) + "/// </summary>" + "\r\n");
                strclass.Append(Space(2) + "//DataSet GetList(int PageSize,int PageIndex,string strWhere);" + "\r\n");
            }
            strclass.Append(Space(2) + "#endregion  ��Ա����" + "\r\n");
            strclass.Append("	}\r\n");
            strclass.Append("}" + "\r\n");
            return strclass.ToString();
        }
        #endregion

        #region ���ݹ���

        public string GetDALFactoryCode()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.Append("using System;\r\n");
            strclass.Append("using System.Reflection;\r\n");
            strclass.Append("using System.Configuration;\r\n");
            strclass.Append("using " + IDALpath + ";\r\n");
            strclass.Append("namespace " + Factorypath + "\r\n");
            strclass.Append("{" + "\r\n");
            strclass.Append(Space(1) + "/// <summary>" + "\r\n");
            strclass.Append(Space(1) + "/// ���󹤳�ģʽ����DAL��" + "\r\n");
            strclass.Append(Space(1) + "/// web.config ��Ҫ�������ã�(���ù���ģʽ+�������+�������,ʵ�ֶ�̬������ͬ�����ݲ����ӿ�)  \r\n");
            strclass.Append(Space(1) + "/// DataCache���ڵ���������ļ�����\r\n");            
            strclass.Append(Space(1) + "/// <appSettings>  \r\n");
            strclass.Append(Space(1) + "/// <add key=\"DAL\" value=\"" + DALpath + "\" /> (����������ռ����ʵ���������Ϊ�Լ���Ŀ�������ռ�)\r\n");
            strclass.Append(Space(1) + "/// </appSettings> \r\n");
            strclass.Append(Space(1) + "/// </summary>" + "\r\n");
            strclass.Append(Space(1) + "public sealed class DataAccess\r\n");
            strclass.Append(Space(1) + "{" + "\r\n");

            strclass.Append(Space(2) + "private static readonly string AssemblyPath = ConfigurationManager.AppSettings[\"DAL\"];\r\n");

            //CreateObject
            strclass.Append(Space(2) + "/// <summary>" + "\r\n");
            strclass.Append(Space(2) + "/// ���������ӻ����ȡ" + "\r\n");
            strclass.Append(Space(2) + "/// </summary>" + "\r\n");
            strclass.Append(Space(2) + "public static object CreateObject(string AssemblyPath,string ClassNamespace)" + "\r\n");
            strclass.Append(Space(2) + "{" + "\r\n");
            strclass.Append(Space(3) + "object objType = DataCache.GetCache(ClassNamespace);//�ӻ����ȡ" + "\r\n");
            strclass.Append(Space(3) + "if (objType == null)" + "\r\n");
            strclass.Append(Space(3) + "{" + "\r\n");
            strclass.Append(Space(4) + "try" + "\r\n");
            strclass.Append(Space(4) + "{" + "\r\n");
            strclass.Append(Space(5) + "objType = Assembly.Load(AssemblyPath).CreateInstance(ClassNamespace);//���䴴��" + "\r\n");
            strclass.Append(Space(5) + "DataCache.SetCache(ClassNamespace, objType);// д�뻺��" + "\r\n");
            strclass.Append(Space(4) + "}" + "\r\n");
            strclass.Append(Space(4) + "catch" + "\r\n");
            strclass.Append(Space(4) + "{}" + "\r\n");
            strclass.Append(Space(3) + "}" + "\r\n");
            strclass.Append(Space(3) + "return objType;" + "\r\n");
            strclass.Append(Space(2) + "}" + "\r\n");
            
            strclass.Append(GetDALFactoryMethodCode());

            strclass.Append(Space(1) + "}" + "\r\n");
            strclass.Append("}" + "\r\n");
            strclass.Append("\r\n");
            return strclass.ToString();
        }

        /// <summary>
        /// �õ������У�����ӿڴ�����������
        /// </summary>
        /// <returns></returns>
        public string GetDALFactoryMethodCode()
        {
            StringPlus strclass = new StringPlus();
            strclass.AppendSpaceLine(2, "/// <summary>" );
            strclass.AppendSpaceLine(2, "/// ����" + DALName + "���ݲ�ӿ�" );
            strclass.AppendSpaceLine(2, "/// </summary>" );
            strclass.AppendSpaceLine(2, "public static " + IDALpath + "." + IClass + " Create" + DALName + "()");
            strclass.AppendSpaceLine(2, "{\r\n");
            if (Folder != "")
            {
                strclass.AppendSpaceLine(3, "string ClassNamespace = AssemblyPath +\"." + Folder + "." + DALName + "\";");
            }
            else
            {
                strclass.AppendSpaceLine(3, "string ClassNamespace = AssemblyPath +\"" + "." + DALName + "\";");
            }
            strclass.AppendSpaceLine(3, "object objType=CreateObject(AssemblyPath,ClassNamespace);");
            strclass.AppendSpaceLine(3, "return (" + IDALpath + "." + IClass + ")objType;");
            strclass.AppendSpaceLine(2, "}");
            return strclass.Value;
        }

        #endregion

        #region ҵ��� 
        public string GetBLLCode(string AssemblyGuid, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool GetModelByCache, bool List, bool ListProc)
        {
            ibll = BuilderFactory.CreateBLLObj(AssemblyGuid);
            if (ibll == null)
            {
                return "//��ѡ����Ч��ҵ������������ͣ�";
            }
            ibll.Fieldlist = Fieldlist;
            ibll.Keys = Keys;
            ibll.NameSpace = NameSpace;            
            ibll.Modelpath = Modelpath;
            ibll.ModelName = ModelName;
            ibll.BLLpath = BLLpath;
            ibll.BLLName = BLLName;
            ibll.Factorypath = Factorypath;
            ibll.IDALpath = IDALpath;
            ibll.IClass = IClass;
            ibll.DALpath = DALpath;
            ibll.DALName = DALName;
            ibll.IsHasIdentity = IsHasIdentity;
            ibll.DbType = dbobj.DbType;

            return ibll.GetBLLCode(Maxid, Exists, Add, Update, Delete, GetModel, GetModelByCache, List);

           

        }
        #endregion


    }
}
