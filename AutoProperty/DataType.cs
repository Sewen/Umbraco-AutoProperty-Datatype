using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sewen.DataType.AutoProperty
{
    public class DataType : umbraco.cms.businesslogic.datatype.BaseDataType, umbraco.interfaces.IDataType {
    
        #region IDataType Members

        private umbraco.interfaces.IDataEditor _Editor;
		private umbraco.interfaces.IData _baseData;
		private umbraco.interfaces.IDataPrevalue _prevalueeditor;

		public override umbraco.interfaces.IDataEditor DataEditor 
		{
			get
			{
                if (_Editor == null)
                {
                    string[] config = ((PrevalueEditor)PrevalueEditor).Configuration.Split('|');
                    if (config.Count() > 1)
                        _Editor = new DataEditor(Data, config[0], config[1]);
                    else
                        _Editor = new DataEditor(Data);
                }
				return _Editor;
			}
		}

		public override umbraco.interfaces.IData Data 
		{
			get 
			{
				if (_baseData == null)
					_baseData = new umbraco.cms.businesslogic.datatype.DefaultData(this);
				return _baseData;
			}
		}
		public override Guid Id 
		{
			get {return new Guid("ec3378f6-f14e-4a97-b705-186af646080f");}
		}

		public override string DataTypeName 
		{
			get {return "AutoProperty Textbox";}
		}

		public override umbraco.interfaces.IDataPrevalue PrevalueEditor 
		{
			get 
			{
				if (_prevalueeditor == null)
					_prevalueeditor = new PrevalueEditor(this);
				return _prevalueeditor;
			}
		}

        #endregion
    }
}

