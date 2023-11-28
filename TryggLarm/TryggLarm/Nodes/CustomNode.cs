using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryggLarm.Admin;
using TryggLarm.NodeEditors;

namespace TryggLarm.Nodes
{
    [Serializable]
    public class CustomNode : TreeNode, ISerializable
    {
        public NodeEditorManager NodeMng;
        protected NodeEditorBase myNodeEditor;
        private bool _isEnabled;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if(value)
                {
                    this.ForeColor = Color.Black;
                    _isEnabled = value;
                }
                else
                {
                    this.ForeColor = Color.Red;
                    _isEnabled = value;
                }
            }
        }

        public List<CustomNode> ChildNodes = new List<CustomNode>();

        public string DisplayName
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Use the AddValue method to specify serialized values.
            //info.AddValue("props", myProperty_value, typeof(string));
            info.AddValue("Name", DisplayName);
            info.AddValue("IsEnabled", IsEnabled);

            var myNodes = new List<CustomNode>();
            
            //List<CustomNode> myNodes = new List<CustomNode>();
            foreach(CustomNode node in Nodes)
            {
                myNodes.Add(node);
            }
            info.AddValue("ChildNodes", myNodes);
        }

        public virtual void OnNodeCreated(NodeEditorManager nodeMng) { }

        public CustomNode(string name)
        {
            DisplayName = name;
            IsEnabled = true;
            Initialize();
            //AddChildNodes();
        }

        public void RefreshMyEditor()
        {
            if (myNodeEditor != null)
                myNodeEditor.LoadValues(this);
        }

        internal virtual void DisableNodeClick(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                IsEnabled = false;
                ContextMenu.MenuItems[0].Text = "Enable";
            }
            else
            {
                IsEnabled = true;
                ContextMenu.MenuItems[0].Text = "Disable";
            }

            NodeMng.OnUserChange(sender, e);
        }

        public virtual void Initialize() { }

        // The special constructor is used to deserialize values.
        public CustomNode(SerializationInfo info, StreamingContext context)
        {
            foreach(SerializationEntry entry in info)
            {
                switch(entry.Name)
                {
                    case "Name":
                        DisplayName = (string)entry.Value;
                        break;
                    case "ChildNodes":
                        ChildNodes = (List<CustomNode>)entry.Value;
                        break;
                    case "IsEnabled":
                        IsEnabled = (bool)entry.Value;
                        break;
                }
            }
            Initialize();
        }

        public void DisposeEditor()
        {
            myNodeEditor.Dispose();
            myNodeEditor = null;
        }

        public void AddChildNodes(NodeEditorManager nodeMng)
        {
            foreach(CustomNode node in ChildNodes)
            {
                node.NodeMng = nodeMng;
                node.AddChildNodes(NodeMng);
                Nodes.Add(node);
            }
            //ChildNodes.Clear();
        }

        public NodeEditorBase GetNodeEditor()
        {
            return myNodeEditor;
        }

        /// <summary>
        /// Initialize myNodeEditor
        /// </summary>
        public virtual void InitializeEditor(/*NodeEditorManager nodeMng*/)
        {
            if (myNodeEditor == null)
                return;

            myNodeEditor.LoadValues(this);
            //return myNodeEditor;
        }
    }
}
