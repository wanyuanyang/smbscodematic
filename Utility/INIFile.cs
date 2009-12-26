using System;
using System.Runtime.InteropServices;
using System.Text;
namespace LTP.Utility
{
    /// <summary>
    /// INIFile ��ժҪ˵����
    /// </summary>
    public class INIFile
    {
        public string path;

        public INIFile(string INIPath)
        {
            path = INIPath;
        }

        #region ����

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);
        #endregion


        #region  дINI

        /// <summary>
        /// дINI�ļ�
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }
        #endregion

        #region ɾ��ini����

        /// <summary>
        /// ɾ��ini�ļ������ж���
        /// </summary>
        public void ClearAllSection()
        {
            IniWriteValue(null, null, null);
        }
        /// <summary>
        /// ɾ��ini�ļ���personal�����µ����м�
        /// </summary>
        /// <param name="Section"></param>
        public void ClearSection(string Section)
        {
            IniWriteValue(Section, null, null);
        }
        #endregion

        #region ��ȡINI
        /// <summary>
        /// ��ȡINI�ļ�
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }
        public byte[] IniReadValues(string section, string key)
        {
            byte[] temp = new byte[255];
            int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
            return temp;
        }
        /// <summary>
        /// ��ȡini�ļ������ж�����
        /// </summary>    
        public string[] IniReadValues()
        {
            byte[] allSection = IniReadValues(null, null);
            return ByteToString(allSection);

        }
        /// <summary>
        /// ת��byte[]����Ϊstring[]�������� 
        /// </summary>
        /// <param name="sectionByte"></param>
        /// <returns></returns>
        private string[] ByteToString(byte[] sectionByte)
        {                  
            ASCIIEncoding ascii = new ASCIIEncoding();           
            //��������key��string����
            string sections = ascii.GetString(sectionByte);
            //��ȡkey������
            string[] sectionList = sections.Split(new char[1] { '\0' });
            return sectionList;
        }

        /// <summary>
        /// ��ȡini�ļ���ĳ���������м���
        /// </summary>    
        public string[] IniReadValues(string Section)
        {
            byte[] sectionByte = IniReadValues(Section, null);
            return ByteToString(sectionByte);
        }

        #endregion

    }


}
