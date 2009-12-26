using System;
using System.Collections.Generic;
using System.Data;
using LTP.CodeHelper;
using System.Data.SqlClient;
namespace LTP.IDBO
{
	/// <summary>
	/// ��ȡ���ݿ���Ϣ��Ľӿڶ��塣
	/// </summary>
	public interface IDbObject
	{
        /// <summary>
        ///  ���ݿ�����
        /// </summary>
        string DbType
        {            
            get;
        }
        /// <summary>
        /// ���ݿ������ַ���
        /// </summary>
        string DbConnectStr
		{
			set;get;
		}

		#region db����

		int ExecuteSql(string DbName,string SQLString);
		DataSet Query(string DbName,string SQLString);

		#endregion
		
		#region �õ����ݿ�������б� GetDBList()
		/// <summary>
		/// �õ����ݿ�������б�
		/// </summary>
		/// <returns></returns>
		List<string> GetDBList();
		#endregion

		#region  �õ����ݿ�������û����� GetTables(string DbName)
		/// <summary>
		/// �õ����ݿ�����б���
		/// </summary>
		/// <param name="DbName"></param>
		/// <returns></returns>
        List<string> GetTables(string DbName);
		DataTable GetVIEWs(string DbName);
		/// <summary>
		/// �õ����ݿ�����б����ͼ��
		/// </summary>
		/// <param name="DbName">���ݿ�</param>
		/// <returns></returns>
		DataTable GetTabViews(string DbName);
        DataTable GetProcs(string DbName);
		#endregion
		
		#region  �õ����ݿ�����б����ϸ��Ϣ GetTablesInfo(string DbName)

		/// <summary>
		/// �õ����ݿ�����б����ϸ��Ϣ
		/// </summary>
		/// <param name="DbName">���ݿ�</param>
		/// <returns></returns>
        List<TableInfo> GetTablesInfo(string DbName);
        List<TableInfo> GetVIEWsInfo(string DbName);
		/// <summary>
		/// �õ����ݿ�����б����ͼ����ϸ��Ϣ
		/// </summary>
		/// <param name="DbName">���ݿ�</param>
		/// <returns></returns>
        List<TableInfo> GetTabViewsInfo(string DbName);
        List<TableInfo> GetProcInfo(string DbName);

		#endregion

        #region �õ����������
        /// <summary>
        /// �õ���ͼ��洢���̵Ķ������
        /// </summary>  
        string GetObjectInfo(string DbName, string objName);
        #endregion

        #region �õ�(����)���ݿ��������������� GetColumnList(string DbName,string TableName)
        /// <summary>
		/// �õ����ݿ���������������
		/// </summary>
		/// <param name="DbName"></param>
		/// <param name="TableName"></param>
		/// <returns></returns>
        List<ColumnInfo> GetColumnList(string DbName, string TableName);
		#endregion

		#region �õ����ݿ������е���ϸ��Ϣ GetColumnInfoList(string DbName,string TableName)
		/// <summary>
		/// �õ����ݿ������е���ϸ��Ϣ
		/// </summary>
		/// <param name="DbName"></param>
		/// <param name="TableName"></param>
		/// <returns></returns>
        List<ColumnInfo> GetColumnInfoList(string DbName, string TableName);
		#endregion

		#region �õ����ݿ��������� GetKeyName(string DbName,string TableName)
		/// <summary>
		/// �õ����ݿ���������
		/// </summary>
		/// <param name="DbName"></param>
		/// <param name="TableName"></param>
		/// <returns></returns>
		DataTable GetKeyName(string DbName,string TableName);
		#endregion

		#region �õ����ݱ�������� GetTabData(string DbName,string TableName)

		/// <summary>
		/// �õ����ݱ��������
		/// </summary>
		/// <param name="DbName"></param>
		/// <param name="TableName"></param>
		/// <returns></returns>
		DataTable GetTabData(string DbName,string TableName);

		#endregion

		#region ���ݿ����Բ���

		/// <summary>
		/// �޸����ݿ�����
		/// </summary>
		/// <param name="OldName"></param>
		/// <param name="NewName"></param>
		/// <returns></returns>
		bool RenameTable(string DbName,string OldName,string NewName);
		/// <summary>
		/// ɾ����
		/// </summary>	
		bool DeleteTable(string DbName,string TableName);

		string GetVersion();

		#endregion
	}
}
