using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Data;
using LTP.Utility;
namespace Maticsoft.AddInManager
{
    /// <summary>
    /// ������ù���
    /// </summary>
    public class AddIn
    {
        string fileAddin = Application.StartupPath + "\\CodeDAL.addin";
        Cache cache = new Cache();
        #region ����
        private string _guid;
        private string _name;
        private string _desc;
        private string _assembly;
        private string _classname;
        private string _version;

        public string Guid
        {
            set { _guid = value; }
            get { return _guid; }
        }
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }
        public string Decription
        {
            set { _desc = value; }
            get { return _desc; }
        }
        public string Assembly
        {
            set { _assembly = value; }
            get { return _assembly; }
        }
        public string Classname
        {
            set { _classname = value; }
            get { return _classname; }
        }
        public string Version
        {
            set { _version = value; }
            get { return _version; }
        }

        #endregion
        public AddIn()
        {
        }
        /// <summary>
        /// ����һ�������Ϣ
        /// </summary>
        /// <param name="AssemblyGuid"></param>
        public AddIn(string AssemblyGuid)
        {
            object obj = cache.GetObject(AssemblyGuid);
            if (obj == null)
            {
                try
                {
                    obj = GetAddIn(AssemblyGuid);
                    if (obj != null)
                    {
                        cache.SaveCache(AssemblyGuid, obj);// д�뻺��

                        DataRow row = (DataRow)obj;
                        _guid = row["Guid"].ToString();
                        _name = row["Name"].ToString();
                        _desc = row["Decription"].ToString();
                        _assembly = row["Assembly"].ToString();
                        _classname = row["Classname"].ToString();
                        _version = row["Version"].ToString();
                    }

                }
                catch (System.Exception ex)
                {
                    string str = ex.Message;// ��¼������־
                }
            }
            
        }

        /// <summary>
        /// �õ����в��
        /// </summary>
        /// <returns></returns>
        public DataSet GetAddInList()
        {
            try
            {
                DataSet dsAddin = new DataSet();
                if (File.Exists(fileAddin))
                {
                    dsAddin.ReadXml(fileAddin);
                    if (dsAddin.Tables.Count > 0)
                    {
                        return dsAddin;
                    }
                }
                return null;
            }
            catch (SystemException ex)
            {
                string err = ex.Message;
                return null;
            }
        }
        /// <summary>
        /// �õ�ĳ�ֽӿ��µ����в��
        /// </summary>
        /// <returns></returns>
        public DataSet GetAddInList(string InterfaceName)
        {
            try
            {
                DataSet dsAddin = new DataSet();
                if (File.Exists(fileAddin))
                {
                    dsAddin.ReadXml(fileAddin);
                    if (dsAddin.Tables.Count > 0)
                    {
                        List<DataRow> rowdels = new List<DataRow>();
                        foreach (DataRow row in dsAddin.Tables[0].Rows)
                        {
                            string assem = row["Assembly"].ToString();
                            Assembly assembly = System.Reflection.Assembly.Load(assem);
                            Type[] types = assembly.GetTypes();
                            bool ret = false;
                            foreach (Type t in types)
                            {                                
                                Type[] interfaces = t.GetInterfaces();
                                foreach (Type theInterface in interfaces)
                                {
                                    if (theInterface.FullName == InterfaceName) 
                                    {
                                        ret = true;
                                    }
                                }                                
                            }
                            if (!ret)
                            {
                                rowdels.Add(row);
                                
                            }
                        }
                        foreach (DataRow r in rowdels)
                        {
                            dsAddin.Tables[0].Rows.Remove(r);
                        }
                        return dsAddin;
                    }
                }
                return null;
            }
            catch (SystemException ex)
            {
                string err = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// �õ�һ���������Ϣ
        /// </summary>
        /// <returns></returns>
        private DataRow GetAddIn(string AssemblyGuid)
        {
            DataSet dsAddin = GetAddInList();
            if (dsAddin.Tables.Count > 0)
            {
                DataRow[] drs = dsAddin.Tables[0].Select("Guid='" + AssemblyGuid + "'");
                if (drs.Length > 0)
                {
                    DataRow row = drs[0];
                    return row;
                }
            }
            return null;
        }
        
        /// <summary>
        /// �õ�һ�������Ϣ(����)
        /// </summary>
        /// <param name="AssemblyGuid"></param>
        /// <returns></returns>
        public DataRow GetAddInByCache(string AssemblyGuid)
        {
            object obj = cache.GetObject(AssemblyGuid);
            if (obj == null)
            {
                try
                {
                    obj = GetAddIn(AssemblyGuid);
                    cache.SaveCache(AssemblyGuid, obj);// д�뻺��
                }
                catch (System.Exception ex)
                {
                    string str = ex.Message;// ��¼������־
                }
            }
            return (DataRow)obj;
        }

        /// <summary>
        /// ���Ӳ����Ϣ
        /// </summary>
        public void AddAddIn()
        {
            DataSet dsAddin = new DataSet();
            if (File.Exists(fileAddin))
            {
                dsAddin.ReadXml(fileAddin);
                if (dsAddin.Tables.Count > 0)
                {
                    DataRow rown = dsAddin.Tables[0].NewRow();
                    rown["Guid"] = _guid;
                    rown["Name"] = _name;
                    rown["Decription"] = _desc;
                    rown["Assembly"] = _assembly;
                    rown["Classname"] = _classname;
                    rown["Version"] = _version;
                    dsAddin.Tables[0].Rows.Add(rown);
                    //dsAddin.WriteXml(fileAddin);

                    XmlTextWriter xtw = new XmlTextWriter(fileAddin, Encoding.Default);
                    xtw.WriteStartDocument();
                    dsAddin.WriteXml(xtw);
                    xtw.Close();

                }
            }
        }
        /// <summary>
        /// ɾ��һ�����
        /// </summary>
        /// <param name="AssemblyGuid"></param>
        public void DeleteAddIn(string AssemblyGuid)
        {
            DataSet dsAddin = GetAddInList();
            if (dsAddin.Tables.Count > 0)
            {
                dsAddin.Tables[0].Select("Guid='" + AssemblyGuid + "'")[0].Delete();
                dsAddin.WriteXml(fileAddin);
            }		
				
        }
        /// <summary>
        /// ��ȡ�ļ�����
        /// </summary>
        /// <returns></returns>
        public string LoadFile()
        {
            StreamReader srFile = new StreamReader(fileAddin, Encoding.Default);
            string strContents = srFile.ReadToEnd();
            srFile.Close();
            return strContents;
        }



    }
}
