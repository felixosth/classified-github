using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;
using TryggLarm.Nodes;
using TryggLarm.NodeEditors;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TryggLarm.Admin
{
    public partial class NodeEditorManager : UserControl
    {
        internal event EventHandler ConfigurationChangedByUser;

        public NodeEditorManager()
        {
            InitializeComponent();
            NodeTreeView.LabelEdit = true;
        }

        internal String DisplayName
        {
            get { return (NodeTreeView.Nodes[0] as RootNode).DisplayName; }
            set { (NodeTreeView.Nodes[0] as RootNode).DisplayName = value; }
        }

        internal void FillContent(Item item)
        {
            SetupNodes(item);
        }

        internal void UpdateItem(Item item)
        {
            if (lastNodeEditor != null)
                lastNodeEditor.SetValues();

            item.Name = DisplayName;

            //List<CustomNode> cNodes = new List<CustomNode>();
            //foreach(CustomNode node in NodeTreeView.Nodes)
            //{
            //    cNodes.Add(node);
            //}
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, NodeTreeView.Nodes[0] as RootNode);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                item.Properties["treeNodes"] = Convert.ToBase64String(buffer);
            }
        }

        internal void ClearContent()
        {
            NodeTreeView.Nodes.Clear();
            splitContainer.Panel2.Controls.Clear();
        }

        internal void OnUserChange(object sender, EventArgs e)
        {
            if (ConfigurationChangedByUser != null)
                ConfigurationChangedByUser(this, new EventArgs());
        }

        private void NodeEditorManager_Load(object sender, EventArgs e)
        {

        }

        void SetupNodes(Item item)
        {
            NodeTreeView.Nodes.Clear();

            //List<CustomNode> nodeList = new List<CustomNode>();
            RootNode rootNode;
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(item.Properties["treeNodes"])))
            {
                BinaryFormatter bf = new BinaryFormatter();
                rootNode = (RootNode)bf.Deserialize(ms);
            }
            rootNode.NodeMng = this;
            rootNode.AddChildNodes(this);
            NodeTreeView.Nodes.Add(rootNode);
            //foreach (var node in nodeList)
            //{
            //    node.NodeMng = this;
            //    node.AddChildNodes(this);

            //    if (node is RootNode)
            //    {
            //        var rootNode = node as RootNode;
            //        //rootNode.NodeMng = this;
            //        NodeTreeView.Nodes.Add(rootNode);
            //    }
            //    else
            //        NodeTreeView.Nodes.Add(node);
            //}

            NodeTreeView.ExpandAll();
        }

        CustomNode lastNode;
        NodeEditorBase lastNodeEditor;
        private void nodeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(lastNodeEditor != null)
            {
                if(lastNodeEditor.ValuesAreModified)
                {
                    lastNodeEditor.SetValues();
                    lastNodeEditor.ValuesAreModified = false;
                    lastNode.DisposeEditor();
                }
            }

            splitContainer.Panel2.Controls.Clear();
            var node = e.Node as CustomNode;
            lastNode = node;
            node.InitializeEditor();
            var editor = node.GetNodeEditor();
            if(editor != null)
            {
                lastNodeEditor = editor;
                splitContainer.Panel2.Controls.Add(editor);
            }
        }


        public void ClearSettingsPanel()
        {
            splitContainer.Panel2.Controls.Clear();
        }


        /// <summary>
        /// Nodes that can be edited via the TreeView heirarchy
        /// </summary>
        List<Type> allowedEditTypes = new List<Type>()
        {
            typeof(RootNode),
            typeof(AlarmObjectNode),
            typeof(AlarmNode),
            typeof(SMSRecipientNode),
            typeof(EmailRecipientNode)
        };

        private void NodeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            bool found = false;
            foreach(var type in allowedEditTypes)
            {
                if (e.Node.GetType() == type)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                e.CancelEdit = true;
        }

        private void NodeTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && NodeTreeView.SelectedNode != null)
                NodeTreeView.SelectedNode.BeginEdit();
        }

        private void NodeTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null)
                return;
            (e.Node as CustomNode).DisplayName = e.Label;
            (e.Node as CustomNode).RefreshMyEditor();
            OnUserChange(sender, new EventArgs());
        }
    }
}
