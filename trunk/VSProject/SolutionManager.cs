using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace VSProject
{
    public class SolutionManager
    {
        private string XMLCsproj;//�����������ļ�

        public SolutionManager(string InXMlCsprojPathth) //�ⲿ����Ĺ����ļ���·��������
        {
            this.XMLCsproj = InXMlCsprojPathth;
        }

        /// <summary>
        /// ת����Ŀ�ļ�Ϊxml
        /// </summary>
        public void SaveXml()
        {
            System.IO.StreamReader obj_strRead = System.IO.File.OpenText(this.XMLCsproj);
            System.IO.StreamWriter obj_strWrite = System.IO.File.CreateText("C://aaa.xml");
            obj_strWrite.Write(obj_strRead.ReadToEnd());
            obj_strRead.Close();
            obj_strWrite.Close();

        }
        /// <summary>
        /// ��ȡ��Ŀ��Ϣ
        /// </summary>
        /// <returns></returns>
        public VisualStudioProject ExcludeCsproj()
        {
            VisualStudioProject obj_visualStudioProject = new VisualStudioProject();
            System.Xml.XmlDocument doc = new XmlDocument();
            this.SaveXml();
            doc.Load("c://aaa.xml");
            foreach (XmlElement xmlFirstElement in doc.DocumentElement.ChildNodes)//���������
            {
                obj_visualStudioProject.LastOpenVersion = xmlFirstElement.Attributes["LastOpenVersion"].InnerXml.ToString();//�Ѹ��ڵ�ĵ�һ���ӽڵ������LastOpenVersion������
                foreach (XmlElement xmlSecondElement in xmlFirstElement)//������һ���ӽڵ�������ӽڵ�
                {
                    if (xmlSecondElement.Name == "Build")//����ýڵ������ΪBuild��
                    {
                        foreach (XmlElement xmlThirdElement in xmlSecondElement) //����Build������ӽڵ�
                        {
                            if (xmlThirdElement.Name == "Settings")//����ڵ���Ϊsettings
                            {
                                obj_visualStudioProject.ReferencePath = xmlThirdElement.Attributes["ReferencePath"].InnerXml.ToString();//����Ӧ�����Դ�����
                                int i = xmlThirdElement.ChildNodes.Count; //��settings�ڵ���ӽڵ��������i����Ϊ�����ж��config������Ҫ�����浽һ���������棬���Ե���֪���������ڵ���
                                Config[] obj_config = new Config[i];
                                int j = 0;
                                foreach (XmlElement xmlFourElement in xmlThirdElement) //����settings�������ӽڵ�
                                {
                                    if (xmlFourElement.Name == "Config")
                                    {
                                        obj_config[j] = new Config(); //�����ڴ档����Ӧ������ֵ������
                                        obj_config[j].Name = xmlFourElement.Attributes["Name"].InnerXml.ToString();
                                        obj_config[j].EnableASPDebugging = xmlFourElement.Attributes["EnableASPDebugging"].InnerXml.ToString();
                                        obj_config[j].EnableASPXDebugging = xmlFourElement.Attributes["EnableASPXDebugging"].InnerXml.ToString();
                                        obj_config[j].EnableSQLServerDebugging = xmlFourElement.Attributes["EnableSQLServerDebugging"].InnerXml.ToString();
                                        obj_config[j].EnableUnmanagedDebugging = xmlFourElement.Attributes["EnableUnmanagedDebugging"].InnerXml.ToString();
                                        obj_config[j].RemoteDebugEnabled = xmlFourElement.Attributes["RemoteDebugEnabled"].InnerXml.ToString();
                                        obj_config[j].RemoteDebugMachine = xmlFourElement.Attributes["RemoteDebugMachine"].InnerXml.ToString();
                                        obj_config[j].StartAction = xmlFourElement.Attributes["StartAction"].InnerXml.ToString();
                                        obj_config[j].StartArguments = xmlFourElement.Attributes["StartArguments"].InnerXml.ToString();
                                        obj_config[j].StartPage = xmlFourElement.Attributes["StartPage"].InnerXml.ToString();
                                        obj_config[j].StartProgram = xmlFourElement.Attributes["StartProgram"].InnerXml.ToString();
                                        obj_config[j].StartURL = xmlFourElement.Attributes["StartURL"].InnerXml.ToString();
                                        obj_config[j].StartWorkingDirectory = xmlFourElement.Attributes["StartWorkingDirectory"].InnerXml.ToString();
                                        obj_config[j].StartWithIE = xmlFourElement.Attributes["StartWithIE"].InnerXml.ToString();
                                        j++;
                                    }
                                }
                                obj_visualStudioProject.ConfigObject = obj_config;
                            }
                        }
                    }
                    else if (xmlSecondElement.Name == "OtherProjectSettings")  //�жϽڵ��Ƿ����ƶ��ڵ�
                    {
                        OtherProjectSettings obj_OtherProjectSettings = new OtherProjectSettings();
                        obj_OtherProjectSettings.CopyProjectDestinationFolder = xmlSecondElement.Attributes["CopyProjectDestinationFolder"].InnerXml.ToString();
                        obj_OtherProjectSettings.CopyProjectOption = xmlSecondElement.Attributes["CopyProjectOption"].InnerXml.ToString();
                        obj_OtherProjectSettings.CopyProjectUncPath = xmlSecondElement.Attributes["CopyProjectUncPath"].InnerXml.ToString();
                        obj_OtherProjectSettings.ProjectTrust = xmlSecondElement.Attributes["ProjectTrust"].InnerXml.ToString();
                        obj_OtherProjectSettings.ProjectView = xmlSecondElement.Attributes["ProjectView"].InnerXml.ToString();

                        obj_visualStudioProject.OtherProjectSettingsObject = obj_OtherProjectSettings;

                    }
                }
            }

            return obj_visualStudioProject;
        }

        /// <summary>
        /// ������Ŀ�ļ�
        /// </summary>
        /// <param name="obj_VisualStudioProject"></param>
        public void SaveCsproj(VisualStudioProject obj_VisualStudioProject)
        {

            System.Xml.XmlWriter myXMLWrite = new XmlTextWriter(this.XMLCsproj, System.Text.Encoding.UTF8);
            myXMLWrite.WriteStartDocument(true);
            myXMLWrite.WriteStartElement("VisualStudioProject");
            myXMLWrite.WriteStartElement("CSHARP");
            myXMLWrite.WriteAttributeString("LastOpenVersion", obj_VisualStudioProject.LastOpenVersion);
            myXMLWrite.WriteStartElement("Build");
            myXMLWrite.WriteStartElement("Settings");
            myXMLWrite.WriteAttributeString("ReferencePath", obj_VisualStudioProject.ReferencePath);
            foreach (Config obj_config in obj_VisualStudioProject.ConfigObject)
            {
                myXMLWrite.WriteStartElement("Config");
                myXMLWrite.WriteAttributeString("Name", obj_config.Name);
                myXMLWrite.WriteAttributeString("EnableASPDebugging", obj_config.EnableASPDebugging);
                myXMLWrite.WriteAttributeString("EnableASPXDebugging", obj_config.EnableASPXDebugging);
                myXMLWrite.WriteAttributeString("EnableUnmanagedDebugging", obj_config.EnableUnmanagedDebugging);
                myXMLWrite.WriteAttributeString("EnableSQLServerDebugging", obj_config.EnableSQLServerDebugging);
                myXMLWrite.WriteAttributeString("RemoteDebugEnabled", obj_config.RemoteDebugEnabled);
                myXMLWrite.WriteAttributeString("RemoteDebugMachine", obj_config.RemoteDebugMachine);
                myXMLWrite.WriteAttributeString("StartAction", obj_config.StartAction);
                myXMLWrite.WriteAttributeString("StartArguments", obj_config.StartArguments);
                myXMLWrite.WriteAttributeString("StartPage", obj_config.StartPage);
                myXMLWrite.WriteAttributeString("StartProgram", obj_config.StartProgram);
                myXMLWrite.WriteAttributeString("StartURL", obj_config.StartURL);
                myXMLWrite.WriteAttributeString("StartWorkingDirectory", obj_config.StartWorkingDirectory);
                myXMLWrite.WriteAttributeString("StartWithIE", obj_config.StartWithIE);
                myXMLWrite.WriteEndElement();
            }
            myXMLWrite.WriteEndElement();
            myXMLWrite.WriteEndElement();
            myXMLWrite.WriteStartElement("OtherProjectSettings");
            myXMLWrite.WriteAttributeString("CopyProjectDestinationFolder", obj_VisualStudioProject.OtherProjectSettingsObject.CopyProjectDestinationFolder);
            myXMLWrite.WriteAttributeString("CopyProjectOption", obj_VisualStudioProject.OtherProjectSettingsObject.CopyProjectOption);
            myXMLWrite.WriteAttributeString("CopyProjectUncPath", obj_VisualStudioProject.OtherProjectSettingsObject.CopyProjectUncPath);
            myXMLWrite.WriteAttributeString("ProjectView", obj_VisualStudioProject.OtherProjectSettingsObject.ProjectView);
            myXMLWrite.WriteAttributeString("ProjectTrust", obj_VisualStudioProject.OtherProjectSettingsObject.ProjectTrust);
            myXMLWrite.WriteEndElement();
            myXMLWrite.WriteEndElement();
            myXMLWrite.WriteEndElement();
            myXMLWrite.WriteEndDocument();
            myXMLWrite.Close();

        }





    }
}
