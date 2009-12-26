using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Codematic.UserControls;
namespace Codematic
{
    public partial class OptionFrm : Form
    {

        private UcOptionsEnviroments optionsEnviroments ;
        private UcOptionEditor optionEditor ;
        private UcOptionsQuerySettings optionsQuerySettings ;
        private UcOptionStartUp optionStartUp ;
        //private UcDBSet ucdbset;
        private UcCSSet uccsset;
        private UcCSNameSet ucnameset;
        private MainForm mainForm;
        UcAddInManage ucAddin;
        UcDatatype ucDatatype;
        UcSysManage ucSysmanage;

        LTP.CmConfig.AppSettings appsettings;
        LTP.CmConfig.ModuleSettings setting;

        public OptionFrm(MainForm mainform)
        {
            InitializeComponent();
            mainForm = mainform;
            appsettings = LTP.CmConfig.AppConfig.GetSettings();
            setting = LTP.CmConfig.ModuleConfig.GetSettings();

            optionsEnviroments = new UcOptionsEnviroments();
            optionEditor = new UcOptionEditor();
            optionsQuerySettings = new UcOptionsQuerySettings();
            optionStartUp = new UcOptionStartUp(appsettings);
            //ucdbset = new UcDBSet();
            uccsset = new UcCSSet();
            ucnameset = new UcCSNameSet();
            ucAddin = new UcAddInManage();
            ucDatatype = new UcDatatype();
            ucSysmanage = new UcSysManage();

        }
        private void OptionFrm_Load(object sender, EventArgs e)
        {
            InitTreeView();
        }
        private void InitTreeView()
        {
            // Top
            TreeNode tnEnviroment = new TreeNode("����", 0, 1);
            TreeNode tnCodeSet = new TreeNode("������������", 0, 1);
            TreeNode tnAddIn = new TreeNode("�������", 0, 1);
            TreeNode tnDbo = new TreeNode("ϵͳ����", 0, 1);

            #region ����
            TreeNode tnEditor = new TreeNode("�༭", 2, 3);
            TreeNode tnQuerySettings = new TreeNode("����", 2, 3);
            TreeNode StartPage = new TreeNode("����", 2, 3);
            tnEnviroment.Nodes.Add(StartPage);
            //tnEnviroment.Nodes.Add(tnEditor);
            //tnEnviroment.Nodes.Add(tnQuerySettings);
            #endregion

            #region  �������
            TreeNode tnDB = new TreeNode("���ݿ�ű�", 2, 3);
            TreeNode tnCS = new TreeNode("��������", 2, 3);
            TreeNode tnNameRule = new TreeNode("����������", 2, 3);
            TreeNode tnWeb = new TreeNode("Webҳ��", 2, 3);
            TreeNode tnDatatype = new TreeNode("�ֶ�����ӳ��", 2, 3);
            //tnCodeSet.Nodes.Add(tnDB);
            tnCodeSet.Nodes.Add(tnCS);
            tnCodeSet.Nodes.Add(tnNameRule);
            //tnCodeSet.Nodes.Add(tnWeb);
            tnCodeSet.Nodes.Add(tnDatatype);
            #endregion

            #region  �������
            //TreeNode tnaddin = new TreeNode("DAL�������", 2, 3);
            //TreeNode tnaddinbll = new TreeNode("BLL�������", 2, 3);
            //TreeNode tnproc = new TreeNode("�洢���̴�����", 2, 3);            
            
            //tnAddIn.Nodes.Add(tnaddin);
            //tnAddIn.Nodes.Add(tnaddinbll);
            #endregion

            this.treeView1.Nodes.Add(tnEnviroment);
            this.treeView1.Nodes.Add(tnCodeSet);
            this.treeView1.Nodes.Add(tnAddIn);
            this.treeView1.Nodes.Add(tnDbo);
            tnEnviroment.Expand();
            tnCodeSet.Expand();

            this.UserControlContainer.Controls.Add(optionsEnviroments);//����
            this.UserControlContainer.Controls.Add(optionEditor);//�༭
            this.UserControlContainer.Controls.Add(optionsQuerySettings);//����
            this.UserControlContainer.Controls.Add(optionStartUp);//����
            //this.UserControlContainer.Controls.Add(ucdbset);
            this.UserControlContainer.Controls.Add(uccsset);//�������ɻ�������
            this.UserControlContainer.Controls.Add(ucnameset);//�������ɻ�������
            this.UserControlContainer.Controls.Add(ucAddin);//�������
            this.UserControlContainer.Controls.Add(ucDatatype);//�ֶ�����ӳ��
            this.UserControlContainer.Controls.Add(ucSysmanage);//ϵͳ����

            ActivateOptionControl(optionsEnviroments);

        }
        private void ActivateOptionControl(System.Windows.Forms.UserControl optionControl)
        {
            foreach (UserControl uc in this.UserControlContainer.Controls)
                uc.Hide();
            optionControl.Show();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = this.treeView1.SelectedNode;
            if (selectedNode != null)
            {
                switch (selectedNode.Text)
                {
                    case "����":
                        ActivateOptionControl(optionsEnviroments);
                        break;
                    case "�༭":
                        ActivateOptionControl(optionEditor);
                        break;
                    case "����":
                        ActivateOptionControl(optionsQuerySettings);
                        break;
                    case "����":
                        ActivateOptionControl(optionStartUp);
                        break;
                    //case "���ݿ�ű�":
                    //    ActivateOptionControl(ucdbset);
                    //    break;
                    case "������������":                       
                    case "��������":
                        ActivateOptionControl(uccsset);
                        break;
                    case "����������":
                        ActivateOptionControl(ucnameset);
                        break;
                    case "�ֶ�����ӳ��":
                        ActivateOptionControl(ucDatatype);
                        break;
                    case "�������":
                    case "DAL������":
                        ActivateOptionControl(ucAddin);
                        break;                    
                    case "ϵͳ����":
                        ActivateOptionControl(ucSysmanage);
                        break;
                }
            }
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            #region ����
            switch (optionStartUp.cmb_StartUpItem.SelectedIndex)
            {
                case 0:
                    appsettings.AppStart = "startuppage";
                    appsettings.StartUpPage = optionStartUp.txtStartUpPage.Text;
                    break;
                case 1:
                    appsettings.AppStart = "blank";
                    break;
                case 2:
                    appsettings.AppStart = "homepage";
                    appsettings.HomePage = optionStartUp.txtStartUpPage.Text;
                    break;
            }
            LTP.CmConfig.AppConfig.SaveSettings(appsettings);
            #endregion

            #region ������������

            if (uccsset.radbtn_Frame_One.Checked)
            {
                setting.AppFrame = "One";
            }
            if (uccsset.radbtn_Frame_S3.Checked)
            {
                setting.AppFrame = "S3";
            }
            if (uccsset.radbtn_Frame_F3.Checked)
            {
                setting.AppFrame = "F3";
            }

            setting.DALType = uccsset.GetDALType();
            setting.BLLType = uccsset.GetBLLType();
            setting.WebType = uccsset.GetWebType(); 

            setting.Namepace = uccsset.txtNamepace.Text.Trim();
            setting.DbHelperName = uccsset.txtDbHelperName.Text.Trim();
            setting.ProjectName = uccsset.txtProjectName.Text.Trim();
            setting.ProcPrefix = uccsset.txtProcPrefix.Text.Trim();

            setting.BLLPrefix = ucnameset.txtBLL_Prefix.Text.Trim();
            setting.BLLSuffix = ucnameset.txtBLL_Suffix.Text.Trim();
            setting.DALPrefix = ucnameset.txtDAL_Prefix.Text.Trim();
            setting.DALSuffix = ucnameset.txtDAL_Suffix.Text.Trim();
            setting.ModelPrefix = ucnameset.txtModel_Prefix.Text.Trim();
            setting.ModelSuffix = ucnameset.txtModel_Suffix.Text.Trim();

            #region ����������
            if (ucnameset.radbtn_Same.Checked)
            {
                setting.TabNameRule = "same";
            }
            if (ucnameset.radbtn_Lower.Checked)
            {
                setting.TabNameRule = "lower";
            }
            if (ucnameset.radbtn_Upper.Checked)
            {
                setting.TabNameRule = "upper";
            }
            if (ucnameset.radbtn_firstUpper.Checked)
            {
                setting.TabNameRule = "firstupper";
            }
            #endregion

            LTP.CmConfig.ModuleConfig.SaveSettings(setting);
            #endregion

            #region �ֶ�����ӳ��
            //�����ֶ�ӳ�������ļ�
            ucDatatype.SaveIni();
            ucSysmanage.SaveDBO();
            #endregion
      
            Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}