using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Codematic.UserControls;
namespace Codematic
{
    public partial class TempView : Form
    {
        TempNode TreeClickNode; //�Ҽ��˵������Ľڵ�
        DataSet ds ; //�˵�����
        LTP.CmConfig.AppSettings settings;
        string tempfilepath = "temptree.xml"; //�˵����ļ�
        
        public TempView()
        {
            InitializeComponent();
            settings = LTP.CmConfig.AppConfig.GetSettings();
            LoadTreeview();
        }

        #region ��ʼ��Treeview��

        private void LoadTreeview()
        {
            ds = new DataSet(); //�˵�����
            treeView1.Nodes.Clear();
            TempNode rootNode = new TempNode("����ģ��");
            rootNode.NodeType = "root";
            rootNode.ImageIndex = 0;
            rootNode.SelectedImageIndex = 0;
            rootNode.Expand();
            treeView1.Nodes.Add(rootNode);
                        
            ds.ReadXml(tempfilepath);
            DataTable dt=ds.Tables[0];
            
            DataRow[] drs = dt.Select("ParentID= " + 0); //ѡ������һ���ڵ�	
            foreach (DataRow r in drs)
            {
                string nodeid = r["NodeID"].ToString();
                string text = r["Text"].ToString();
                string filepath = r["FilePath"].ToString();
                string nodetype = r["NodeType"].ToString();
                
                TempNode node = new TempNode(text);
                node.NodeID = nodeid;
                node.NodeType = nodetype;
                node.FilePath = filepath;
                if (nodetype == "folder")
                {
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 1;
                }
                else
                {
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                }                
                rootNode.Nodes.Add(node);
                
                int sonparentid = int.Parse(nodeid); // or =location
                CreateNode(sonparentid,node,dt);
            }
            

        }

        //�����ڵ�
        public void CreateNode(int parentid, TreeNode parentnode, DataTable dt)
        {
            DataRow[] drs = dt.Select("ParentID= " + parentid); //ѡ�������ӽڵ�			
            foreach (DataRow r in drs)
            {
                string nodeid = r["NodeID"].ToString();
                string text = r["Text"].ToString();
                string filepath = r["FilePath"].ToString();
                string nodetype = r["NodeType"].ToString();
                
                TempNode node = new TempNode(text);
                node.NodeID = nodeid;
                node.NodeType = nodetype;
                node.FilePath = filepath;
                if (nodetype == "folder")
                {
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 1;
                }
                else
                {
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                }                        

                parentnode.Nodes.Add(node);

                int sonparentid = int.Parse(nodeid);// or =location
                CreateNode(sonparentid, node, dt);

            }//endforeach		

        }


        #endregion

        #region treeView1����

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {            
        }
        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                Point mpt = new Point(e.X, e.Y);
                TreeClickNode = (TempNode)this.treeView1.GetNodeAt(mpt);
                this.treeView1.SelectedNode = TreeClickNode;
                if (TreeClickNode != null)
                {                   
                    if (e.Button == MouseButtons.Right)
                    {
                        CreatMenu(TreeClickNode.NodeType);
                        contextMenuStrip1.Show(treeView1, mpt);
                    }
                }
            }
            catch
            {
            }
        }

        #region ����treeview �Ҽ��˵�

        private void CreatMenu(string NodeType)
        {
            //this.contextMenuStrip1.Items.Clear();
            switch (NodeType)
            {
                case "folder":
                    ��ToolStripMenuItem.Enabled = false;
                    �½�ToolStripMenuItem.Visible = true;
                    break;
                case "file":
                    ��ToolStripMenuItem.Enabled = true;
                    �½�ToolStripMenuItem.Visible = false;
                    break;
            }
        }

        #endregion

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            //TreeNode node = (TreeNode)e.Item;          
            //if ((node.Tag.ToString() == "table") || (node.Tag.ToString() == "view") || (node.Tag.ToString() == "column"))
            //{
            //    DoDragDrop(node, DragDropEffects.Copy);
            //}
        }

        #endregion

        private string GetMaxNodeID(DataTable dt)
        {
            int maxval = 1;
            foreach (DataRow row in dt.Rows)
            {
                string nodeid = row["NodeID"].ToString();
                if (maxval < int.Parse(nodeid))
                {
                    maxval = int.Parse(nodeid);
                }
            }            
            return (maxval+1).ToString();

            //object obj = dt.Compute("max(CONVERT(int,NodeID))+1", "");
            //object obj = dt.Compute("max(NodeID)+1", "");
            //return obj.ToString();
        }     

        #region �Ҽ��˵��¼�

        private void ��ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TempNode SelNode = (TempNode)this.treeView1.SelectedNode;
                string selstr = SelNode.Text;
                string filepath = SelNode.FilePath;
                if (filepath.Trim() != "")
                {
                    CodeTemplate codetempfrm = (CodeTemplate)Application.OpenForms["CodeTemplate"];
                    if (codetempfrm != null)
                    {
                        codetempfrm.SettxtTemplate(filepath);
                    }
                    else
                    {
                        MessageBox.Show("��δ��ģ��������ɱ༭����","��ʾ",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("��ѡ�ļ��Ѿ������ڣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //�½��ļ���
        private void �ļ���ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TempNode SelNode = (TempNode)this.treeView1.SelectedNode;
                string nodeid = SelNode.NodeID;

                int orderid = 1;
                if (SelNode != null)
                {
                    orderid = SelNode.Nodes.Count + 1;
                }

                string text = SelNode.Text;
                string filepath = SelNode.FilePath;
                string nodetype = SelNode.NodeType;                
                string newNodeid=GetMaxNodeID(ds.Tables[0]);

                //��ds����һ������              
                DataRow newRow = ds.Tables[0].NewRow();
                newRow["NodeID"] = newNodeid; 
                newRow["Text"] = "�½��ļ���";
                newRow["FilePath"] = "";
                newRow["NodeType"] = "folder";
                newRow["ParentID"] = nodeid;
                newRow["OrderID"] = orderid;
                ds.Tables[0].Rows.Add(newRow);
                ds.Tables[0].AcceptChanges();
                ds.WriteXml(tempfilepath);

                //��treeview������һ���ڵ�
                TempNode node = new TempNode("�½��ļ���");
                node.NodeID = newNodeid;
                node.ParentID = nodeid;
                node.NodeType = "folder";               
                node.ImageIndex = 0;
                node.SelectedImageIndex = 1;                
                SelNode.Nodes.Add(node);
                SelNode.Expand();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //�½�ģ���ļ�
        private void ģ��ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TempNode SelNode = (TempNode)this.treeView1.SelectedNode;
                string nodeid = SelNode.NodeID;

                int orderid = 1;
                if (SelNode != null)
                {
                    orderid = SelNode.Nodes.Count + 1;
                }
                string newNodeid = GetMaxNodeID(ds.Tables[0]);
                //��ds����һ������
                DataRow newRow = ds.Tables[0].NewRow();
                newRow["NodeID"] = newNodeid;
                newRow["Text"] = "�½�ģ��";
                string filepath = "�½�ģ��.cmt";
                if ((settings.TemplateFolder != null) && (settings.TemplateFolder != ""))
                {
                    filepath = settings.TemplateFolder + "\\" + filepath;
                }
                else
                {
                    filepath = "Template\\�½�ģ��.cmt";
                }

                // �Ƿ��Ѿ�����
                //while (File.Exists(filepath))
                //{
                //}

                newRow["FilePath"] = filepath;
                newRow["NodeType"] = "file";
                newRow["ParentID"] = nodeid;
                newRow["OrderID"] = orderid;
                ds.Tables[0].Rows.Add(newRow);
                ds.Tables[0].AcceptChanges();
                ds.WriteXml(tempfilepath);
                                
                File.Create(filepath);                               

                //��treeview������һ���ڵ�
                TempNode node = new TempNode("�½�ģ��");
                node.NodeID = newNodeid;
                node.ParentID = nodeid;
                node.FilePath = filepath;
                node.NodeType = "file";
                node.ImageIndex = 2;
                node.SelectedImageIndex = 2;
                SelNode.Nodes.Add(node);
                SelNode.Expand();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ˢ��ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTreeview();
        }
        private void ɾ��ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TempNode SelNode = (TempNode)this.treeView1.SelectedNode;
                string nodeid = SelNode.NodeID;
                string text = SelNode.Text;
                string filepath = SelNode.FilePath;
                string nodetype = SelNode.NodeType;
                
                //��dsɾ��һ������
                foreach (DataRow row in ds.Tables[0].Select("NodeID='" + nodeid + "'"))
                {
                    ds.Tables[0].Rows.Remove(row);
                }
                ds.Tables[0].AcceptChanges();
                ds.WriteXml(tempfilepath);
                if (nodetype == "file")
                {
                    if (File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }
                }

                //ɾ���ڵ�
                treeView1.Nodes.Remove(SelNode);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TempNode SelNode = (TempNode)this.treeView1.SelectedNode;
                if (SelNode != null && SelNode.Parent != null)
                {
                    treeView1.SelectedNode = SelNode;
                    treeView1.LabelEdit = true;
                    if (!SelNode.IsEditing)
                    {
                        SelNode.BeginEdit();
                    }
                }
                else
                {
                    MessageBox.Show("û��ѡ��ڵ��ýڵ��Ǹ��ڵ�.\n" , "��Чѡ��");
                }
                                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                if (e.Label.Length > 0)
                {
                    if (e.Label.IndexOfAny(new char[] { '@', '.', ',', '!' }) == -1)
                    {
                        e.Node.EndEdit(false);
                        TempNode SelNode = (TempNode)e.Node;
                        string nodeid = SelNode.NodeID;
                        string nodetype=SelNode.NodeType;
                        string newNodetext = e.Label;
                        string filepath = SelNode.FilePath;
                        string newfilepath = filepath;
                        if (nodetype == "file")
                        {
                            int end = filepath.LastIndexOf("\\");
                            newfilepath = filepath.Substring(0, filepath.Length - end-1) + newNodetext + ".cmt";

                            File.Move(filepath, newfilepath);
                        }                     
                        
                        foreach (DataRow row in ds.Tables[0].Select("NodeID='" + nodeid + "'"))
                        {
                            row["Text"] = newNodetext;
                            row["FilePath"] = newfilepath;
                        }
                        ds.Tables[0].AcceptChanges();
                        ds.WriteXml(tempfilepath);

                    }
                    else
                    {
                        e.CancelEdit = true;
                        MessageBox.Show("��Ч�ڵ����Ч�ַ�: '@','.', ',', '!'","�ڵ�༭");
                        e.Node.BeginEdit();
                    }
                }
                else
                {
                    e.CancelEdit = true;
                    MessageBox.Show("��Ч�ڵ��ڵ����Ʋ���Ϊ�գ�", "�ڵ�༭");
                    e.Node.BeginEdit();
                }
                this.treeView1.LabelEdit = false;
            }

        }
        #endregion

        

    }
}