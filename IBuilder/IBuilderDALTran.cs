using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using LTP.IDBO;
using LTP.CodeHelper;
namespace LTP.IBuilder
{
    /// <summary>
    /// ���빹�����ӿ�(��������ĸ��ӱ���)
    /// </summary>
    public interface IBuilderDALTran
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
        /// ������
        /// </summary>
        string TableNameParent
        {
            set;
            get;
        }
        /// <summary>
        /// �ӱ���
        /// </summary>
        string TableNameSon
        {
            set;
            get;
        }
        /// <summary>
        /// ѡ����ֶμ���
        /// </summary>
        List<ColumnInfo> FieldlistParent
        {
            set;
            get;
        }
        /// <summary>
        /// ѡ����ֶμ���
        /// </summary>
        List<ColumnInfo> FieldlistSon
        {
            set;
            get;
        }

        /// <summary>
        /// �����������ֶ��б�
        /// </summary>
        List<ColumnInfo> KeysParent
        {
            set;
            get;
        }
        /// <summary>
        /// �����������ֶ��б�
        /// </summary>
        List<ColumnInfo> KeysSon
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
        ///Model����(����)
        /// </summary>
        string ModelNameParent
        {
            set;
            get;
        }
        /// <summary>
        /// Model����(����)
        /// </summary>
        string ModelNameSon
        {
            set;
            get;
        }
       
        /// <summary>
        /// ���ݲ�������ռ�
        /// </summary>
        string DALpath
        {
            set;
            get;
        }
        /// <summary>
        /// ���ݲ������(����)
        /// </summary>
        string DALNameParent
        {
            set;
            get;
        }
        /// <summary>
        /// ���ݲ������(����)
        /// </summary>
        string DALNameSon
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
