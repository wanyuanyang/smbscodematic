using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;
using LTP.Utility;
using LTP.IDBO;
using LTP.DBFactory;
using LTP.IBuilder;
using LTP.CodeHelper;
namespace LTP.CodeBuild
{
    /// <summary>
    /// ������
    /// </summary>
    public class BuilderFrameS3 : BuilderFrame
    {

        #region  ˽�б���
        IBuilderDAL idal;
        IBuilderDALTran idaltran;
        IBuilderBLL ibll;
                
        /// <summary>
        /// ���ݲ�������ռ�
        /// </summary>
        public new string DALpath
        {
            get
            {
                string _dalpath = NameSpace + "." + "DAL";
                if (Folder.Trim() != "")
                {
                    _dalpath += "." + Folder;
                }
                return _dalpath;
            }            
        }
        public new string DALSpace
        {            
            get
            {
                return DALpath + "." + DALName;
            }
        }    

        #endregion


        #region ����
        public BuilderFrameS3(IDbObject idbobj, string dbName, string tableName, string modelName, string bllName, string dalName, 
            List<ColumnInfo> fieldlist, List<ColumnInfo> keys, 
            string nameSpace, string folder,string dbHelperName)
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
            //model = new BuilderModel(dbobj, DbName, TableName, ModelName, NameSpace, Folder, Modelpath, fieldlist);   
         
        }

        public BuilderFrameS3(IDbObject idbobj, string dbName, string nameSpace, string folder, string dbHelperName)
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
        public string GetModelCode()
        {
            LTP.BuilderModel.BuilderModel model = new LTP.BuilderModel.BuilderModel();
            model.ModelName = ModelName;
            model.NameSpace = NameSpace;
            model.Fieldlist = Fieldlist;
            model.Modelpath = Modelpath;
            return model.CreatModel();
        }
        /// <summary>
		/// �õ�Model��
		/// </summary>		
        //public string GetModelCode(string AssemblyGuid)
        //{
        //    //return model.CreatModel();
        //    imodel = BuilderFactory.CreateModelObj(AssemblyGuid);
        //    if (imodel == null)
        //    {
        //        return "��ѡ����Ч��Model�����������ͣ�";
        //    }
        //    imodel.ModelName = ModelName;
        //    imodel.NameSpace = NameSpace;
        //    imodel.Fieldlist = Fieldlist;
        //    //imodel.Keys = Keys;
        //    imodel.Modelpath = Modelpath;
        //    //imodel.ModelSpace = ModelSpace;
        //    return imodel.CreatModel();
        //}

        /// <summary>
        /// �õ����ӱ�Model
        /// </summary>		
        public string GetModelCode(string tableNameParent, string modelNameParent,List<ColumnInfo> FieldlistP,
                       string tableNameSon,string modelNameSon,List<ColumnInfo> FieldlistS)
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
            //    tableNameSon, modelNameSon, FieldlistS,NameSpace, Folder, Modelpath);

            LTP.BuilderModel.BuilderModelT modelP = new LTP.BuilderModel.BuilderModelT();
            modelP.ModelName = modelNameParent;
            modelP.NameSpace = NameSpace;
            modelP.Fieldlist = FieldlistP;
            modelP.Modelpath = Modelpath;
            modelP.ModelNameSon = modelNameSon;
            //modelP.FieldlistSon = FieldlistS;

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
            idal.IDALpath = "";
            idal.IClass = "";
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
            
            idaltran.IDALpath = "";
            idaltran.IClass = "";
            idaltran.DbHelperName = DbHelperName;
            idaltran.ProcPrefix = procPrefix;

            return idaltran.GetDALCode(Maxid, Exists, Add, Update, Delete, GetModel, List);

        }

        #endregion
                
        #region ҵ���
        public string GetBLLCode(string AssemblyGuid, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool GetModelByCache, bool List)
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
            ibll.Factorypath = "";
            ibll.IDALpath = "";
            ibll.IClass = "";
            ibll.DALpath = DALpath;
            ibll.DALName = DALName;
            ibll.IsHasIdentity = IsHasIdentity;
            ibll.DbType = dbobj.DbType;

            return ibll.GetBLLCode(Maxid, Exists, Add, Update, Delete, GetModel,GetModelByCache, List);

           

        }
        
        #endregion


    }
}
