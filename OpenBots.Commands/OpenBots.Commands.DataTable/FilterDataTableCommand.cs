﻿using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Data = System.Data;

namespace OpenBots.Commands.DataTable
{
	[Serializable]
	[Category("DataTable Commands")]
	[Description("This command filters specific rows from a DataTable into a new Datatable.")]
	public class FilterDataTableCommand : ScriptCommand
	{
		[Required]
		[DisplayName("Input DataTable")]
		[Description("Enter the DataTable to filter through.")]
		[SampleUsage("{vDataTable}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_DataTable { get; set; }

		[Required]
		[DisplayName("Filter Tuple")]
		[Description("Enter a tuple containing the column name and item you would like to filter by.")]
		[SampleUsage("(ColumnName1,Item1),(ColumnName2,Item2) || ({vColumn1},{vItem1}),({vCloumn2},{vItem2}) || {vFilterTuple}")]
		[Remarks("DataRows must match all provided tuples to be included in the filtered DataTable.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_SearchItem { get; set; }

		[Required]
		[Editable(false)]
		[DisplayName("Output Filtered DataTable Variable")]
		[Description("Create a new variable or select a variable from the list.")]
		[SampleUsage("{vUserVariable}")]
		[Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
		public string v_OutputUserVariableName { get; set; }

		public FilterDataTableCommand()
		{
			CommandName = "FilterDataTableCommand";
			SelectionName = "Filter DataTable";
			CommandEnabled = true;          
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			var vSearchItem = v_SearchItem.ConvertUserVariableToString(engine);

			Data.DataTable Dt = (Data.DataTable)v_DataTable.ConvertUserVariableToObject(engine);
			var t = new List<Tuple<string, string>>();
			var listPairs = vSearchItem.Split(')');
			int i = 0;

			listPairs = listPairs.Take(listPairs.Count() - 1).ToArray();
			foreach (string item in listPairs)
			{
				string temp;
				temp = item.Trim().TrimStart(',').TrimStart('(');
				var tempList = temp.Split(',');
				t.Insert(i, Tuple.Create(tempList[0], tempList[1]));
				i++;
			}

			List<DataRow> templist = new List<DataRow>();

			foreach (Tuple<string, string> tuple in t)
			{
				foreach (DataRow row in Dt.Rows)
				{
					if (row[tuple.Item1] != null)
					{
						if (row[tuple.Item1].ToString() == tuple.Item2.ToString() && !templist.Contains(row))
						{
							//outputDT.Rows.Remove(row);
							templist.Add(row);
						}
					}
				}
			}

			Data.DataTable outputDT = new Data.DataTable();
			int x = 0;

			foreach(DataColumn column in Dt.Columns)
			{
				outputDT.Columns.Add(Dt.Columns[x].ToString());
				x++;
			}

			foreach (DataRow item in templist)
				outputDT.Rows.Add(item.ItemArray);

			outputDT.StoreInUserVariable(engine, v_OutputUserVariableName);
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_DataTable", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_SearchItem", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue()+ $" [Filter Rows With '{v_SearchItem}' From '{v_DataTable}' - Store Filtered DataTable in '{v_OutputUserVariableName}']";
		}       
	}
}