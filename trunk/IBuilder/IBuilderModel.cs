using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using LTP.IDBO;
using LTP.CodeHelper;
namespace LTP.IBuilder
{
    /// <summary>
    /// Model代码构造器接口
    /// </summary>
    public interface IBuilderModel
    {
        #region 公有属性   
        /// <summary>
        /// model类名
        /// </summary>
        string ModelName
        {
            set;
            get;
        }
        /// <summary>
        /// 顶级命名空间名 
        /// </summary>
        string NameSpace
        {
            set;
            get;
        }
        /// <summary>
        /// 实体类的命名空间
        /// </summary>
        string Modelpath
        {
            set;
            get;
        }
        /// <summary>
        /// 选择的字段集合
        /// </summary>
        List<ColumnInfo> Fieldlist
        {
            set;
            get;
        }
        #endregion
        
        #region 生成完整单个Model类
        /// <summary>
        /// 生成完整单个Model类
        /// </summary>		
        string CreatModel();       
        #endregion

        #region 生成Model属性部分
        /// <summary>
        /// 生成实体类的属性
        /// </summary>
        /// <returns></returns>
        string CreatModelMethod();      
        #endregion
    }
}
