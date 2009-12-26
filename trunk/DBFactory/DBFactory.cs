using System;
using System.Reflection;
using LTP.Utility;
using LTP.IDBO;
namespace LTP.DBFactory
{
	/// <summary>
	/// ���ݿ���Ϣ��ʵ������,���÷��䶯̬��������
	/// </summary>
    public class DBOMaker
	{
        private static Cache cache = new Cache();

        #region ͬһ�����ڷ���

        //#region Cache-CreateObject
        //public static object CreateObject(string TypeName)
        //{
        //    object obj = cache.GetObject(TypeName);
        //    if (obj == null)
        //    {
        //        try
        //        {
        //            Type objType = Type.GetType(TypeName, true);
        //            obj = Activator.CreateInstance(objType);
        //            cache.SaveCaech(TypeName, obj);// д�뻺��
        //        }
        //        catch//(System.Exception ex)
        //        {
        //            //string str=ex.Message;// ��¼������־
        //        }
        //    }
        //    return obj;
        //}
        //#endregion

        ///// <summary>
        ///// �������ݿ���Ϣ��ӿ�
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static IDbObject CreateDbObj(string obj)
        //{		
        //    //�ӳ��򼯴�������ʵ��
        //    //string objpath = ConfigHelper.GetConfigString("DbObject");
        //    //return (IDbObject)Assembly.Load(objpath).CreateInstance(objpath+".DbObject");

        //    // ʹ����ָ������ƥ��̶���ߵĹ��캯��������ָ�����͵�ʵ��
        //    ////string obj = ConfigHelper.GetConfigString("DbObject");
        //    //string TypeName="LTP.CodeBuild."+obj+".DbObject";
        //    //Type objType = Type.GetType(TypeName,true);
        //    //return (IDbObject)Activator.CreateInstance(objType);

        //    string TypeName = "LTP.CodeBuild." + obj + ".DbObject";
        //    object objType = CreateObject(TypeName);
        //    return (IDbObject)objType;
			
        //}

        ///// <summary>
        ///// �������ݿ�ű�������ӿ�
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static IDbScriptBuilder CreateScript(string obj)
        //{			
        //    //string TypeName="LTP.CodeBuild."+obj+".DbScriptBuilder";
        //    //Type objType = Type.GetType(TypeName,true);
        //    //return (IDbScriptBuilder)Activator.CreateInstance(objType);

        //    string TypeName = "LTP.CodeBuild." + obj + ".DbScriptBuilder";
        //    object objType = CreateObject(TypeName);
        //    return (IDbScriptBuilder)objType;
        //}

        #endregion


        #region ��ͬ���򼯷���

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
                catch(System.Exception ex)
                {
                    string str=ex.Message;// ��¼������־
                }
            }
            return obj;
        }
        /// <summary>
        /// �������ݿ���Ϣ��ӿ�
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDbObject CreateDbObj(string dbTypename)
        {
            //�ӳ��򼯴�������ʵ��
            //string objpath = ConfigHelper.GetConfigString("DbObject");
            //return (IDbObject)Assembly.Load(objpath).CreateInstance(objpath+".DbObject");

            string TypeName = "LTP.DbObjects." + dbTypename + ".DbObject";
            object objType = CreateObject("LTP.DbObjects", TypeName);
            return (IDbObject)objType;
        }

        /// <summary>
        /// �������ݿ�ű�������ӿ�
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDbScriptBuilder CreateScript(string dbTypename)
        {
            string TypeName = "LTP.DbObjects." + dbTypename + ".DbScriptBuilder";                       
            object objType = CreateObject("LTP.DbObjects", TypeName);
            return (IDbScriptBuilder)objType;
        }

        #endregion


    }
}
