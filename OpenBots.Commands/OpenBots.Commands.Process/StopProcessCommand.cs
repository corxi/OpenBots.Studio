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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Diagnostics = System.Diagnostics;

namespace OpenBots.Commands.Process
{
	[Serializable]
	[Category("Programs/Process Commands")]
	[Description("This command stops a program or process.")]
	public class StopProcessCommand : ScriptCommand
	{
		[Required]
		[DisplayName("Program Name or Path")]
		[Description("Provide a valid program name or enter a full path to the script/executable including the extension.")]
		[SampleUsage(@"notepad || excel || {vApp} || C:\temp\myapp.exe || {ProjectPath}\myapp.exe")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		[Editor("ShowFileSelectionHelper", typeof(UIAdditionalHelperType))]
		public string v_ProgramName { get; set; }

		[Required]
		[DisplayName("Stop Option")]
		[PropertyUISelectionOption("Close")]
		[PropertyUISelectionOption("Kill")]
		[Description("Indicate whether the program should be closed or killed.")]
		[SampleUsage("")]
		[Remarks("*Close* will close any open process windows while *Kill* will close all processes, including background ones.")]
		public string v_StopOption { get; set; }

		public StopProcessCommand()
		{
			CommandName = "StopProgramCommand";
			SelectionName = "Stop Process";
			CommandEnabled = true;
			
			v_StopOption = "Kill";
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			string vProgramName = v_ProgramName.ConvertUserVariableToString(engine);

			if (File.Exists(vProgramName))
				vProgramName = Path.GetFileNameWithoutExtension(vProgramName);

			var processes = Diagnostics.Process.GetProcessesByName(vProgramName);

			foreach (var prc in processes)
			{
				if (v_StopOption == "Close")
					prc.CloseMainWindow();
				else if (v_StopOption == "Kill")
					prc.Kill();
			}
					
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_ProgramName", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_StopOption", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [{v_StopOption} Process '{v_ProgramName}']";
		}
	}
}