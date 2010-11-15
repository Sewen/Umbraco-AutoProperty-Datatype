using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using umbraco.DataLayer;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using System.Web.UI;

namespace Sewen.DataType.AutoProperty
{
	public class PrevalueEditor : System.Web.UI.WebControls.PlaceHolder, umbraco.interfaces.IDataPrevalue
	{
		// UI controls
		private umbraco.controls.ContentPicker _cpStartNode;
		private TextBox _textboxDocumentTypeAlias;
		private DropDownList _dropdownlist;
				
		// referenced datatype
		private umbraco.cms.businesslogic.datatype.BaseDataType _datatype;

		public static ISqlHelper SqlHelper
		{
			get { return Application.SqlHelper; }
		}

		public PrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType DataType) 
		{
			// state it knows its datatypedefinitionid
			_datatype = DataType;
			setupChildControls();

		}
		
		private void setupChildControls() 
		{
			_dropdownlist = new DropDownList();
			_dropdownlist.ID = "dbtype";
			_dropdownlist.Items.Add(DBTypes.Date.ToString());
			_dropdownlist.Items.Add(DBTypes.Integer.ToString());
			_dropdownlist.Items.Add(DBTypes.Ntext.ToString());
			_dropdownlist.Items.Add(DBTypes.Nvarchar.ToString());

            _cpStartNode = new umbraco.controls.ContentPicker();
			_cpStartNode.ID = "cpStartNode";

			_textboxDocumentTypeAlias = new TextBox();
			_textboxDocumentTypeAlias.ID = "tbDocumentTypeAlias";

			// put the childcontrols in context - ensuring that
			// the viewstate is persisted etc.
			this.Controls.Add(_dropdownlist);
			this.Controls.Add(_cpStartNode);
			this.Controls.Add(_textboxDocumentTypeAlias);

		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if (!Page.IsPostBack)
			{
				string[] config = Configuration.Split("|".ToCharArray());
				if (config.Length > 0) 
				{
					_cpStartNode.Value = config[0];
				}
                if (config.Length > 1)
                {
                    _textboxDocumentTypeAlias.Text = config[1];
                }
				_dropdownlist.SelectedValue = _datatype.DBType.ToString();
			}
		}
		
		public Control Editor 
		{
			get
			{
				return this;
			}
		}

		public void Save() 
		{
			_datatype.DBType = (umbraco.cms.businesslogic.datatype.DBTypes)Enum.Parse(typeof(umbraco.cms.businesslogic.datatype.DBTypes), _dropdownlist.SelectedValue, true);

			// Generate data-string
            string data = _cpStartNode.Text + "|" +_textboxDocumentTypeAlias.Text;

			// If the add new prevalue textbox is filled out - add the value to the collection.
			IParameter[] SqlParams = new IParameter[] {
										SqlHelper.CreateParameter("@value",data),
										SqlHelper.CreateParameter("@dtdefid",_datatype.DataTypeDefinitionId)};
			SqlHelper.ExecuteNonQuery("delete from cmsDataTypePreValues where datatypenodeid = @dtdefid",SqlParams);
			SqlHelper.ExecuteNonQuery("insert into cmsDataTypePreValues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'')",SqlParams);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteLine("<table>");
			writer.WriteLine("<tr><th>Database datatype</th><td>");
			_dropdownlist.RenderControl(writer);
			writer.Write("</td></tr>");
            writer.Write("<tr><th>Start node</th><td>");
            _cpStartNode.RenderControl(writer);
            writer.Write("</td></tr>");
            writer.Write("<tr><th>Document Type Alias</th><td>");
            _textboxDocumentTypeAlias.RenderControl(writer);
            writer.Write("</td></tr>");
            writer.Write("</table>");
		}

		public string Configuration 
		{
			get 
			{
				object configVal = SqlHelper.ExecuteScalar<object>("select value from cmsDataTypePreValues where datatypenodeid = @datatypenodeid", SqlHelper.CreateParameter("@datatypenodeid", _datatype.DataTypeDefinitionId));
				if (configVal != null)
					return configVal.ToString();
				else
					return "";
			}
		}
	}
}
