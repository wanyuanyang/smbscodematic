using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using LTP.CodeHelper;
namespace LTP.IDBO
{
	/// <summary>
	/// IDbScriptBuilder ��ժҪ˵����
	/// </summary>
	public interface IDbScriptBuilder
	{

		#region ����
		string DbConnectStr
		{
			get;
			set;
		}

		string DbName
		{
			get;
			set;
		}

		string TableName
		{
			get;
			set;
		}
        	

        string ProcPrefix
        {
            set;
            get;
        }       
        string ProjectName
        {
            set;
            get;
        }
        /// <summary>
        /// ѡ����ֶμ���
        /// </summary>
        List<ColumnInfo> Fieldlist
        {
            set;
            get;
        }
        /// <summary>
        /// ѡ����ֶμ����ַ���
        /// </summary>
        string Fields
        {
            get;
        }
        /// <summary>
        /// �����������ֶ�
        /// </summary>
        List<ColumnInfo> Keys
        {
            set;
            get;
        }
		#endregion
	

		#region �������ݿ�����ű�
        /// <summary>
        /// �������ݿ����б�Ĵ����ű�
        /// </summary>
        /// <returns></returns>
        string CreateDBTabScript(string dbname);
		/// <summary>
		/// �������ݿ�����ű�
		/// </summary>
		/// <returns></returns>
		string CreateTabScript(string dbname,string tablename);

        /// <summary>
        /// ����SQL��ѯ��� �������ݴ����ű�
        /// </summary>
        /// <returns></returns>
        string CreateTabScriptBySQL(string dbname, string strSQL);
        /// <summary>
        /// �������ݿ�����ű����ļ�
        /// </summary>
        /// <param name="dbname"></param>
        /// <param name="tablename"></param>
        /// <param name="filename"></param>
        /// <param name="progressBar"></param>
		void CreateTabScript(string dbname,string tablename,string filename,System.Windows.Forms.ProgressBar progressBar);
        		
		#endregion

		#region �����洢����

		string CreatPROCGetMaxID();
        string CreatPROCIsHas();
		string CreatPROCADD();
		string CreatPROCUpdate();
        string CreatPROCDelete();
        string CreatPROCGetModel();
		string CreatPROCGetList();
		
		/// <summary>
		/// �õ�ĳ����Ĵ洢���̣�ѡ�����ɵķ�����
		/// </summary>
		/// <param name="Maxid"></param>
		/// <param name="Ishas"></param>
		/// <param name="Add"></param>
		/// <param name="Update"></param>
		/// <param name="Delete"></param>
		/// <param name="GetModel"></param>
		/// <param name="List"></param>
		/// <param name="dtColumn">�����������Ϣ</param>
		/// <returns></returns>
        string GetPROCCode(bool Maxid, bool Ishas, bool Add, bool Update, bool Delete, bool GetModel, bool List);
		
		/// <summary>
		/// �õ�ĳ����Ĵ洢����
		/// </summary>
		/// <param name="dbname">����</param>
		/// <param name="tablename">����</param>
		/// <returns></returns>
		string GetPROCCode(string dbname,string tablename);
		/// <summary>
		/// �õ�һ���������б�Ĵ洢����
		/// </summary>
		/// <param name="DbName"></param>
		/// <returns></returns>
		string GetPROCCode(string dbname);
		#endregion

        #region ����SQL��ѯ���

        /// <summary>
        /// ����Select��ѯ���
        /// </summary>
        /// <param name="dbname">����</param>
        /// <param name="tablename">����</param>
        /// <returns></returns>
        string GetSQLSelect(string dbname, string tablename);
       

        /// <summary>
        /// ����update��ѯ���
        /// </summary>
        /// <param name="dbname">����</param>
        /// <param name="tablename">����</param>
        /// <returns></returns>
        string GetSQLUpdate(string dbname, string tablename);
        
        /// <summary>
        /// ����update��ѯ���
        /// </summary>
        /// <param name="dbname">����</param>
        /// <param name="tablename">����</param>
        /// <returns></returns>
        string GetSQLDelete(string dbname, string tablename);

        /// <summary>
        /// ����INSERT��ѯ���
        /// </summary>
        /// <param name="dbname">����</param>
        /// <param name="tablename">����</param>
        /// <returns></returns>
        string GetSQLInsert(string dbname, string tablename);
        #endregion
	}
}
