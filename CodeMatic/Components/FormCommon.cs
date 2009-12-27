using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
namespace Codematic
{
    /// <summary>
    /// ���干�ò�����
    /// </summary>
    public static class FormCommon
    {
        /// <summary>
        /// ��ǰ���ݿ����������
        /// </summary>
        public static DbView DbViewForm
        {
            get
            {
                if (Application.OpenForms["DbView"] == null)
                {
                    return null;
                }
                return (DbView)Application.OpenForms["DbView"];
            }
        }

        /// <summary>
        /// �õ���ǰ���ݿ������ѡ�еķ���������
        /// </summary>        
        public static string GetDbViewSelServer()
        {
            if (Application.OpenForms["DbView"] == null)
            {
                return "";
            }
            DbView dbviewfrm1 = (DbView)Application.OpenForms["DbView"];
            TreeNode SelNode = dbviewfrm1.treeView1.SelectedNode;
            if (SelNode == null)
                return "";
            string longservername = "";
            switch (SelNode.Tag.ToString())
            {
                case "serverlist":
                    return "";
                case "server":
                    {
                        longservername = SelNode.Text;
                    }
                    break;
                case "db":
                    {
                        longservername = SelNode.Parent.Text;
                    }
                    break;
                case "tableroot":
                case "viewroot":
                    {
                        longservername = SelNode.Parent.Parent.Text;
                    }
                    break;
                case "table":
                case "view":
                    {
                        longservername = SelNode.Parent.Parent.Parent.Text;
                    }
                    break;
                case "column":
                    longservername = SelNode.Parent.Parent.Parent.Parent.Text;
                    break;
            }

            return longservername;
        }
    }
}
