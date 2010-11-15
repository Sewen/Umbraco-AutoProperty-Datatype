using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sewen.DataType.AutoProperty
{
    public class DataEditor : System.Web.UI.WebControls.TextBox, umbraco.interfaces.IDataEditor
    {
        private umbraco.interfaces.IData data;
        private string startNode;
        private string nodeTypeAlias;
        
        public DataEditor(umbraco.interfaces.IData Data)
        {
            data = Data;
            startNode = "";
            nodeTypeAlias = "";
        }

        public DataEditor(umbraco.interfaces.IData Data, string StartNode, string NodeTypeAlias)
        {
            data = Data;
            startNode = StartNode;
            nodeTypeAlias = NodeTypeAlias;
        }

        #region IDataEditor Members

        public System.Web.UI.Control Editor
        {
            get { return this; }
        }

        public void Save()
        {
            data.Value = this.Text;
        }

        public bool ShowLabel
        {
            get { return true; }
        }

        public bool TreatAsRichTextEditor
        {
            get { return false; }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            this.CssClass = "umbEditorTextField autoPropertyTextField " +this.ID;
            //this.Attributes.Add("rel", propertyName);

            if (data != null && data.Value != null)
            {
                Text = data.Value.ToString();
            }

            RegisterJavascript(this.ID);

            base.OnInit(e);
        }

        private void RegisterJavascript(string propertyName)
        {
            string autocompleteUrl = "/scripts/jquery/autocomplete/jquery.autocomplete.min.js";
            string autocompleteCss = "/scripts/jquery/autocomplete/jquery.autocomplete.css";
            string scriptUrl = "/scripts/AutoProperty/AutoProperty.js";

            System.Web.UI.ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "AutoCompleteInclude", autocompleteUrl);
            System.Web.UI.ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "AutoPropertyInclude", scriptUrl);
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AutoPropertyScript" + propertyName, string.Format("setAutoProperty('{0}', {1}, '{2}', '{3}');", propertyName, getStartNode(), nodeTypeAlias, useParent().ToString()), true);
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AutoPropertyCss", string.Format("includeCSS('{0}');", autocompleteCss), true);
        }

        private string getStartNode()
        {
            if (string.IsNullOrEmpty(startNode))
            {
                return System.Web.HttpContext.Current.Request["id"];
            }
            else
            {
                return startNode;
            }
        }

        private bool useParent()
        {
            return string.IsNullOrEmpty(startNode);
        }
    }
}
