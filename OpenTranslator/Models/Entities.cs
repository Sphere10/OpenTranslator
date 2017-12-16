using Omu.AwesomeMvc;
using OpenTranslator.Models.Input;
using System;
using System.Collections.Generic;

namespace OpenTranslator.Models
{
    public class Entity
    {
        public string TextId { get; set; }
		public string Text { get; set; }
		public bool System { get; set; }

	}	

	public class GridFormatData
	{
		public List<GridArrayRow> GridRows {get;set; }
		public List<Column> GridColumn { get;set;}
	}

	public enum Translationmodes
    {
        Open = 0,
        Selected = 1,
		Locked = 2
    }
	
	
}