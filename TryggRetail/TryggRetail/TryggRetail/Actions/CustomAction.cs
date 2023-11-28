using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryggRetail.Actions
{
    [Serializable]
    public class CustomAction
    {
        public string DisplayText { get; set; }

        public Color ButtonBackColor { get; set; }
        public Color ButtonForeColor { get; set; }

        public override string ToString()
        {
            return DisplayText;
            //return base.ToString();
        }

        public CustomAction()
        {

        }

        public virtual void Execute()
        {

        }
    }
}
