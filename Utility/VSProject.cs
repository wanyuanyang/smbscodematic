using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
namespace LTP.Utility
{
    /// <summary>
    /// vs��Ŀ�ļ��޸�
    /// </summary>
    public class VSProject
    {

        #region �޸���Ŀ�ļ�

        /// <summary>
        /// �Զ�ѡ�����ͽ������
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="classname"></param>
        public void AddClass(string filename, string classname)
        {
            System.Xml.XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            string name = doc.DocumentElement.FirstChild.Name;
            switch (name)            
            {
                case "CSHARP":
                    AddClass2003(filename, classname);
                    break;
                case "PropertyGroup":
                    AddClass2005(filename, classname);
                    break;         
                default:
                    break;
            }
        }

        /// <summary>
        /// vs2003 ��ʽ��Ŀ׷���ļ�
        /// </summary>
        /// <param name="filename">��Ŀ�ļ���</param>
        /// <param name="classname">���ļ���</param>
        public void AddClass2003(string filename, string classname)
        {
            System.Xml.XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            foreach (XmlElement xmlFirstElement in doc.DocumentElement.ChildNodes)//���������
            {
                foreach (XmlElement xmlSecondElement in xmlFirstElement)
                {
                    if (xmlSecondElement.Name == "Files")
                    {
                        foreach (XmlElement xmlThreeElement in xmlSecondElement)
                        {
                            if (xmlThreeElement.Name == "Include")
                            {
                                XmlElement elem = doc.CreateElement("File", doc.DocumentElement.NamespaceURI);
                                elem.SetAttribute("RelPath", classname);
                                elem.SetAttribute("SubType", "Code");
                                elem.SetAttribute("BuildAction", "Compile");
                                xmlThreeElement.AppendChild(elem);
                                break;
                            }
                        }
                    }
                }
            }
            doc.Save(filename);
        }


        /// <summary>
        /// vs2005��ʽ��Ŀ׷���ļ�
        /// </summary>
        /// <param name="filename">��Ŀ�ļ���</param>
        /// <param name="classname">���ļ���</param>
        public void AddClass2005(string filename, string classname)
        {
            System.Xml.XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            foreach (XmlElement xmlFirstElement in doc.DocumentElement.ChildNodes)//���������
            {
                if (xmlFirstElement.Name == "ItemGroup")
                {
                    string code = xmlFirstElement.ChildNodes[0].InnerText; //xmlFirstElement.InnerText;
                    string type = xmlFirstElement.ChildNodes[0].Name;
                    if (type == "Compile") 
                    {
                        XmlElement elem = doc.CreateElement("Compile", doc.DocumentElement.NamespaceURI);
                        elem.SetAttribute("Include", classname);
                        xmlFirstElement.AppendChild(elem);
                        break;
                    }
                }
            }
            doc.Save(filename);
        }

        /// <summary>
        /// vs2008��ʽ��Ŀ׷���ļ�
        /// </summary>
        /// <param name="filename">��Ŀ�ļ���</param>
        /// <param name="classname">���ļ���</param>
        public void AddClass2008(string filename, string classname)
        {
            System.Xml.XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            foreach (XmlElement xmlFirstElement in doc.DocumentElement.ChildNodes)//���������
            {
                if (xmlFirstElement.Name == "ItemGroup")
                {
                    string code = xmlFirstElement.ChildNodes[0].InnerText; //xmlFirstElement.InnerText;
                    string type = xmlFirstElement.ChildNodes[0].Name;
                    if (type == "Compile")
                    {
                        XmlElement elem = doc.CreateElement("Compile", doc.DocumentElement.NamespaceURI);
                        elem.SetAttribute("Include", classname);
                        xmlFirstElement.AppendChild(elem);
                        break;
                    }
                }
            }
            doc.Save(filename);
        }

        /// <summary>
        /// vs2005��ʽ��Ŀ׷���ļ�
        /// </summary>
        /// <param name="filename">��Ŀ�ļ���</param>
        /// <param name="classname">���ļ���</param>
        public void AddClass2005Aspx(string filename, string aspxname)
        {
            System.Xml.XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            foreach (XmlElement xmlFirstElement in doc.DocumentElement.ChildNodes)//���������
            {
                if (xmlFirstElement.Name == "ItemGroup")
                {
                    string code = xmlFirstElement.ChildNodes[0].InnerText; //xmlFirstElement.InnerText;
                    string type = xmlFirstElement.ChildNodes[0].Name;
                    if (type == "Compile")
                    {
                        XmlElement elem = doc.CreateElement("Compile", doc.DocumentElement.NamespaceURI);
                        elem.SetAttribute("Include", aspxname);
                        xmlFirstElement.AppendChild(elem);
                        break;
                    }
                }
            }
            doc.Save(filename);
        }
        #endregion

        #region �����ļ����ӷ���

        /// <summary>
        /// �����ļ����ӷ���
        /// </summary>
        /// <param name="ClassFile">���ļ�</param>
        /// <param name="strContent">��������</param>
        public void AddMethodToClass(string ClassFile, string strContent)
        {
            if (File.Exists(ClassFile))
            {
                string strcontent = File.ReadAllText(ClassFile, Encoding.Default);
                if (strcontent.IndexOf(" class ") > 0)
                {
                    int n1 = strcontent.LastIndexOf("}");
                    string temp=strcontent.Substring(0,n1-1);
                    int n2 = temp.LastIndexOf("}");

                    string sss = strcontent.Substring(0, n2 - 1);

                    string lastStr = sss + "\r\n" + strContent + "\r\n" + "}" + "\r\n" + "}";
                    
                    StreamWriter sw = new StreamWriter(ClassFile, false, Encoding.Default);
                    sw.Write(lastStr);
                    sw.Flush();
                    sw.Close();
                }
            }
        }
        #endregion



    }
}
