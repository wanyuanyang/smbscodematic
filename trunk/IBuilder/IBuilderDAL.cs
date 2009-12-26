using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using LTP.IDBO;
using LTP.CodeHelper;
namespace LTP.IBuilder
{
    /// <summary>
    /// DAL���빹�����ӿ�
    /// </summary>
    public interface IBuilderDAL
    { 

        #region ��������
        IDbObject DbObject
        {
            set;
            get;
        }
        /// <summary>
        /// ����
        /// </summary>
        string DbName
        {
            set;
            get;
        }
        /// <summary>
        /// ����
        /// </summary>
        string TableName
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
        /// �����������ֶ��б�
        /// </summary>
        List<ColumnInfo> Keys
        {
            set;
            get;
        }
        /// <summary>
        /// ���������ռ���
        /// </summary>
        string NameSpace
        {
            set;
            get;
        }
        /// <summary>
        /// �����ļ���
        /// </summary>
        string Folder
        {
            set;
            get;
        }

        /// <summary>
        /// ʵ����������ռ�
        /// </summary>
        string Modelpath
        {
            set;
            get;
        }
        /// <summary>
        /// model����
        /// </summary>
        string ModelName
        {
            set;
            get;
        }
        ///// <summary>
        ///// ʵ��������������ռ� + ����
        ///// </summary>
        //string ModelSpace
        //{
        //    set;
        //    get;
        //}
        
        /// <summary>
        /// ���ݲ�������ռ�
        /// </summary>
        string DALpath
        {
            set;
            get;
        }
        /// <summary>
        /// ���ݲ������
        /// </summary>
        string DALName
        {
            set;
            get;
        }

        /// <summary>
        /// �ӿڵ������ռ�
        /// </summary>
        string IDALpath
        {
            set;
            get;
        }
        /// <summary>
        /// �ӿ�����
        /// </summary>
        string IClass
        {
            set;
            get;
        }


        /// <summary>
        /// ���ݿ��������
        /// </summary>
        string DbHelperName
        {
           set;
            get;
        }        
        /// <summary>
        /// �洢����ǰ׺ 
        /// </summary>       
        string ProcPrefix
        {
            set;
            get;
        }
        #endregion

        string GetDALCode(bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List);

        /// <summary>
        /// �õ�GetMaxID()�ķ�������
        /// </summary>
        string CreatGetMaxID();

        /// <summary>
        /// �õ�Exists()�����Ĵ���
        /// </summary>
        string CreatExists();
                   
        /// <summary>
        /// �õ�Add()�Ĵ���
        /// </summary>
        string CreatAdd();       

        /// <summary>
        /// �õ�Update()�Ĵ���
        /// </summary>        
        string CreatUpdate();
        
        /// <summary>
        /// �õ�Delete()�Ĵ���
        /// </summary>
        string CreatDelete();
        
        /// <summary>
        /// �õ�GetModel()�Ĵ���
        /// </summary>
        string CreatGetModel();
        
        /// <summary>
        /// �õ�GetList()�Ĵ���
        /// </summary> 
        string CreatGetList();

    }
}
