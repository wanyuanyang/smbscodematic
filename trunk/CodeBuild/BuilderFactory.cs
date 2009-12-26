using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Maticsoft.AddInManager;
using LTP.Utility;
using System.Data;
namespace LTP.CodeBuild
{
    /// <summary>
    /// �������ɶ��� ����
    /// </summary>
    public class BuilderFactory
    {
        private static Cache cache = new Cache();

        #region ���򼯷���

        private static object CreateObject(string path, string TypeName)
        {
            object obj = cache.GetObject(TypeName);
            if (obj == null)
            {
                try
                {
                    obj = Assembly.Load(path).CreateInstance(TypeName);
                    cache.SaveCache(TypeName, obj);// д�뻺��
                }
                catch (System.Exception ex)
                {
                    string str = ex.Message;// ��¼������־
                }
            }
            return obj;
        }
        #endregion

        #region �������ݷ��ʲ� �������ɶ���

        /// <summary>
        /// �������ݷ��ʲ� �������ɶ���
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static LTP.IBuilder.IBuilderDAL CreateDALObj(string AssemblyGuid)
        {
            try
            {
                if (AssemblyGuid == "")
                {
                    return null;
                }
                AddIn addin = new AddIn(AssemblyGuid);                
                string Assembly = addin.Assembly;
                string Classname = addin.Classname;

                object objType = CreateObject(Assembly, Classname);
                return (LTP.IBuilder.IBuilderDAL)objType;
            }
            catch (SystemException ex)
            {
                string err = ex.Message;
                return null;
            }
        }
        /// <summary>
        /// �������ݷ��ʲ� �������ɶ���
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static LTP.IBuilder.IBuilderDALTran CreateDALTranObj(string AssemblyGuid)
        {
            try
            {
                if (AssemblyGuid == "")
                {
                    return null;
                }
                AddIn addin = new AddIn(AssemblyGuid);
                string Assembly = addin.Assembly;
                string Classname = addin.Classname;

                object objType = CreateObject(Assembly, Classname);
                return (LTP.IBuilder.IBuilderDALTran)objType;
            }
            catch (SystemException ex)
            {
                string err = ex.Message;
                return null;
            }
        }

        #endregion

        #region ����ҵ��� �������ɶ���

        /// <summary>
        /// ����ҵ��� �������ɶ���
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static LTP.IBuilder.IBuilderBLL CreateBLLObj(string AssemblyGuid)
        {
            try
            {
                if (AssemblyGuid == "")
                {
                    return null;
                }
                AddIn addin = new AddIn(AssemblyGuid);
                string Assembly = addin.Assembly;
                string Classname = addin.Classname;

                object objType = CreateObject(Assembly, Classname);
                return (LTP.IBuilder.IBuilderBLL)objType;
            }
            catch (SystemException ex)
            {
                string err = ex.Message;
                return null;
            }
        }
  

        #endregion

        #region ����Model�� �������ɶ���

        /// <summary>
        /// ����ҵ��� �������ɶ���
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static LTP.IBuilder.IBuilderModel CreateModelObj(string AssemblyGuid)
        {
            try
            {
                if (AssemblyGuid == "")
                {
                    return null;
                }
                AddIn addin = new AddIn(AssemblyGuid);
                string Assembly = addin.Assembly;
                string Classname = addin.Classname;

                object objType = CreateObject(Assembly, Classname);
                return (LTP.IBuilder.IBuilderModel)objType;
            }
            catch (SystemException ex)
            {
                string err = ex.Message;
                return null;
            }
        }


        #endregion

        #region ����WEB�� �������ɶ���

        /// <summary>
        /// ����ҵ��� �������ɶ���
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static LTP.IBuilder.IBuilderWeb CreateWebObj(string AssemblyGuid)
        {
            try
            {
                if (AssemblyGuid == "")
                {
                    return null;
                }
                AddIn addin = new AddIn(AssemblyGuid);
                string Assembly = addin.Assembly;
                string Classname = addin.Classname;

                object objType = CreateObject(Assembly, Classname);
                return (LTP.IBuilder.IBuilderWeb)objType;
            }
            catch (SystemException ex)
            {
                string err = ex.Message;
                return null;
            }
        }


        #endregion


    }
}
