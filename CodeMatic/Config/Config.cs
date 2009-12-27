using System;
using System.IO;
using System.Web;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;
namespace Codematic
{
	//����������

	#region ���ö���ģ���� ModuleSettings

	/// <summary>
	/// ���õ�modul�ࣨע��������Ե�������[XmlElement]��
	/// use:ModuleSettings settings=ModuleConfig.GetSettings();
	/// </summary>
	public class ModuleSettings
	{        
        private string _procprefix;
        private string _projectname;
		private string _namespace="Maticsoft";
		private string _folder="Folder";
		private string _appframe="f3";//������(s3)����������(f3)���Զ���(custom)
		private string _daltype="sql";//ֱ��дsql���(sql)�����ǵ��ô洢����(Proc);
        private string _blltype = "sql";//ֱ��дsql���(sql)�����ǵ��ô洢����(Proc);
		private string _editfont="������";
		private float _editfontsize=9;
        private string _dbHelperName = "DbHelperSQL";
      		
        
        /// <summary>
        /// �洢����ǰ׺ 
        /// </summary>
        [XmlElement]
        public string ProcPrefix
        {
            set { _procprefix = value; }
            get { return _procprefix; }
        }
        /// <summary>
        /// ��Ŀ���� 
        /// </summary>
        [XmlElement]
        public string ProjectName
        {
            set { _projectname = value; }
            get { return _projectname; }
        }
		/// <summary>
		/// Ĭ�϶��������ռ���
		/// </summary>
		[XmlElement]
		public string Namepace
		{
            set { _namespace = value; }
            get { return _namespace; }
		}
		/// <summary>
		/// Ĭ��ҵ���߼����ڵ��ļ���
		/// </summary>
		[XmlElement]
		public string Folder
		{
			set{ _folder=value; }
			get{ return _folder; }
		}
		/// <summary>
		/// ���õļܹ�
		/// </summary>
		[XmlElement]
		public string AppFrame
		{
			set{ _appframe=value; }
			get{ return _appframe; }
		}
		/// <summary>
		/// ���ݲ����� 
		/// </summary>
		[XmlElement]
		public string DALType
		{
			set{ _daltype=value; }
			get{ return _daltype; }
		}
        /// <summary>
        /// ҵ������� 
        /// </summary>
        [XmlElement]
        public string BLLType
        {
            set { _blltype = value; }
            get { return _blltype; }
        }
		/// <summary>
		/// ��ǰ�༭��ʹ�õ�������
		/// </summary>
		[XmlElement]
		public string EditFont
		{
			set{ _editfont=value; }
			get{ return _editfont; }
		}
		/// <summary>
		/// ��ǰ�༭��ʹ�õ�����Ĵ�С
		/// </summary>
		[XmlElement]
		public float EditFontSize
		{
			set{ _editfontsize=value; }
			get{ return _editfontsize; }
		}
        /// <summary>
        /// ���ݷ������� 
        /// </summary>
        [XmlElement]
        public string DbHelperName
        {
            set { _dbHelperName = value; }
            get { return _dbHelperName; }
        }
 
               
		
	}
	#endregion 


	#region  ���õĲ����� ModuleConfig

	/// <summary>
	/// ���õĲ�����ModuleConfig��
	/// </summary>
	public class ModuleConfig
	{        		
		public static ModuleSettings GetSettings()
		{			
			ModuleSettings data = null;
			XmlSerializer serializer = new XmlSerializer(typeof(ModuleSettings));
			try
			{
				string apppath=Application.StartupPath;
				string fileName = apppath+"\\config.xml";
				FileStream fs = new FileStream(fileName, FileMode.Open);					
				data = (ModuleSettings)serializer.Deserialize(fs);
				fs.Close();				
			}
			catch
			{	
				data = new ModuleSettings();
			}			
			return data;
		}
        
		public static void SaveSettings(ModuleSettings data)
		{
			string apppath=Application.StartupPath;
			string fileName = apppath+"\\config.xml";
			XmlSerializer serializer = new XmlSerializer (typeof(ModuleSettings));
        
			// serialize the object
			FileStream fs = new FileStream(fileName, FileMode.Create);
			serializer.Serialize(fs, data);
			fs.Close();
		}
	}

	#endregion



}
