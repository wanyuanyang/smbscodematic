using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace Codematic.UserControls
{
    /// <summary>
    /// ��������ؼ�
    /// </summary>
    public partial class UcCodeView : UserControl
    {
        public UcCodeView()
        {
            InitializeComponent();
            SettxtContent("CS", "");
        }
        private void menu_Save_Click(object sender, EventArgs e)
        {            
            SaveFileDialog sqlsavedlg = new SaveFileDialog();
            sqlsavedlg.Title = "���浱ǰ����";
            string text = "";
            if (txtContent_CS.Visible)
            {
                sqlsavedlg.Filter = "C# files (*.cs)|*.cs|All files (*.*)|*.*";
                text = txtContent_CS.Text;
            }
            if (txtContent_SQL.Visible)
            {
                sqlsavedlg.Filter = "SQL files (*.sql)|*.cs|All files (*.*)|*.*";
                text = txtContent_SQL.Text;
            }
            if (txtContent_Web.Visible)
            {
                sqlsavedlg.Filter = "Aspx files (*.aspx)|*.cs|All files (*.*)|*.*";
                text = txtContent_Web.Text;
            }            
            DialogResult dlgresult = sqlsavedlg.ShowDialog(this);
            if (dlgresult == DialogResult.OK)
            {
                string filename = sqlsavedlg.FileName;                
                StreamWriter sw = new StreamWriter(filename, false, Encoding.Default);//,false);
                sw.Write(text);
                sw.Flush();//�ӻ�����д����������ļ���
                sw.Close();
            }            
        }

        //���ô���ؼ�����
        public void SettxtContent(string Type, string strContent)
        {
            switch (Type)
            {
                case "SQL":
                    {
                        this.txtContent_SQL.Visible = true;
                        this.txtContent_CS.Visible = false;
                        this.txtContent_Web.Visible = false;
                        this.txtContent_XML.Visible = false;
                        this.txtContent_SQL.Dock = DockStyle.Fill;
                        this.txtContent_SQL.Text = strContent;

                    }
                    break;
                case "CS":
                    {
                        this.txtContent_SQL.Visible = false;
                        this.txtContent_CS.Visible = true;
                        this.txtContent_Web.Visible = false;
                        this.txtContent_XML.Visible = false;
                        this.txtContent_CS.Dock = DockStyle.Fill;
                        this.txtContent_CS.Text = strContent;
                    }
                    break;
                case "Aspx":
                    {
                        this.txtContent_SQL.Visible = false;
                        this.txtContent_CS.Visible = false;
                        this.txtContent_XML.Visible = false;
                        this.txtContent_Web.Visible = true;
                        this.txtContent_Web.Dock = DockStyle.Fill;
                        this.txtContent_Web.Text = strContent;
                    }
                    break;
                case "XML":
                    {
                        this.txtContent_SQL.Visible = false;
                        this.txtContent_CS.Visible = false;
                        this.txtContent_Web.Visible = false;                        
                        this.txtContent_XML.Visible = true;
                        this.txtContent_XML.Dock = DockStyle.Fill;
                        this.txtContent_XML.Text = strContent;
                    }
                    break;
            }           

        }
    }
}
