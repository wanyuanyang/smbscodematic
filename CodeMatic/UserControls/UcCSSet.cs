using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Maticsoft.AddInManager;
namespace Codematic.UserControls
{
    /// <summary>
    /// ��������
    /// </summary>
    public partial class UcCSSet : UserControl
    {
        LTP.CmConfig.ModuleSettings setting;
        DALTypeAddIn cm_daltype;
        DALTypeAddIn cm_blltype;
        DALTypeAddIn cm_webtype;

        public UcCSSet()
        {
            InitializeComponent();

            setting = LTP.CmConfig.ModuleConfig.GetSettings();
            switch (setting.AppFrame)
            {
                case "One":
                    radbtn_Frame_One.Checked = true;
                    break;
                case "S3":
                    radbtn_Frame_S3.Checked = true;
                    break;
                case "F3":
                    radbtn_Frame_F3.Checked = true;
                    break;
            }
  
            #region ���ز��

            cm_daltype = new DALTypeAddIn("LTP.IBuilder.IBuilderDAL");
            cm_daltype.Title = "DAL";
            groupBox5.Controls.Add(cm_daltype);
            cm_daltype.Location = new System.Drawing.Point(10, 18);
            cm_daltype.SetSelectedDALType(setting.DALType.Trim());

            cm_blltype = new DALTypeAddIn("LTP.IBuilder.IBuilderBLL");
            cm_blltype.Title = "BLL";
            groupBox5.Controls.Add(cm_blltype);
            cm_blltype.Location = new System.Drawing.Point(10, 42);
            cm_blltype.SetSelectedDALType(setting.BLLType.Trim());

            cm_webtype = new DALTypeAddIn("LTP.IBuilder.IBuilderWeb");
            cm_webtype.Title = "Web";
            groupBox5.Controls.Add(cm_webtype);
            cm_webtype.Location = new System.Drawing.Point(10, 66);
            cm_webtype.SetSelectedDALType(setting.WebType.Trim()); 

            #endregion

            txtDbHelperName.Text = setting.DbHelperName;
            txtNamepace.Text = setting.Namepace;
            txtProjectName.Text = setting.ProjectName;
            txtProcPrefix.Text = setting.ProcPrefix;
            
        }


        //�õ����ݲ�����
        public string GetDALType()
        {
            string daltype = "";
            daltype = cm_daltype.AppGuid;
            if ((daltype == "") && (daltype == "System.Data.DataRowView"))
            {
                MessageBox.Show("ѡ������ݲ�����������رպ����ԣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
            return daltype;
        }

        //�õ����ݲ�����
        public string GetBLLType()
        {
            string blltype = "";
            blltype = cm_blltype.AppGuid;
            if ((blltype == "") && (blltype == "System.Data.DataRowView"))
            {
                MessageBox.Show("ѡ������ݲ�����������رպ����ԣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
            return blltype;
        }

        //�õ����ݲ�����
        public string GetWebType()
        {
            string webtype = "";
            webtype = cm_webtype.AppGuid;
            if ((webtype == "") && (webtype == "System.Data.DataRowView"))
            {
                MessageBox.Show("ѡ��ı�ʾ������������رպ����ԣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
            return webtype;
        }

       
    }
}
