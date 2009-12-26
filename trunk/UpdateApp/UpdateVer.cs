using System;
using System.IO;
using System.Web;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;
namespace UpdateApp
{
    //����������

    #region ���ö���ģ���� UpdateSettings

    /// <summary>
    /// ���õ�modul��
    /// </summary>
    public class UpdateSettings
    {
        private string _version="1.0";
        private string _description;
        private string _serverurl;

        /// <summary>
        /// ��ǰ����汾 
        /// </summary>
        [XmlElement]
        public string Version
        {
            set { _version = value; }
            get { return _version; }
        }
        /// <summary>
        /// ��������
        /// </summary>
        [XmlElement]
        public string Description
        {
            set { _description = value; }
            get { return _description; }
        }
        /// <summary>
        /// ������������ַ
        /// </summary>
        [XmlElement]
        public string ServerUrl
        {
            set { _serverurl = value; }
            get { return _serverurl; }
        }

    }
    #endregion


    #region  ���õĲ�����UpdateConfig
    /// <summary>
    /// ���õĲ�����ModuleConfig��
    /// </summary>
    public class UpdateConfig
    {
        public static UpdateSettings GetSettings()
        {
            UpdateSettings data = null;
            XmlSerializer serializer = new XmlSerializer(typeof(UpdateSettings));
            try
            {
                string fileName = Application.StartupPath + @"\UpdateVer.xml";
                FileStream fs = new FileStream(fileName, FileMode.Open);
                data = (UpdateSettings)serializer.Deserialize(fs);
                fs.Close();
            }
            catch
            {
                data = new UpdateSettings();
            }
            return data;
        }
        public static void SaveSettings(UpdateSettings data)
        {
            string fileName = Application.StartupPath + @"\UpdateVer.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(UpdateSettings));

            // serialize the object
            FileStream fs = new FileStream(fileName, FileMode.Create);
            serializer.Serialize(fs, data);
            fs.Close();
        }
    }

    #endregion

}
