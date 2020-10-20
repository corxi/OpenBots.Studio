﻿using System.ComponentModel;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace OpenBots.Commands.Excel
{
    [Serializable]
    [Category("Excel Commands")]
    [Description("This command runs a macro in an Excel Workbook.")]

    public class ExcelRunMacroCommand : ScriptCommand
    {
        [DisplayName("Excel Instance Name")]
        [Description("Enter the unique instance that was specified in the **Create Application** command.")]
        [SampleUsage("MyExcelInstance")]
        [Remarks("Failure to enter the correct instance or failure to first call the **Create Application** command will cause an error.")]
        public string v_InstanceName { get; set; }

        [DisplayName("Macro Name")]
        [Description("Enter the name of the macro as it exists in the Worksheet.")]
        [SampleUsage("Macro1 || {vMacro}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_MacroName { get; set; }

        public ExcelRunMacroCommand()
        {
            CommandName = "ExcelRunMacroCommand";
            SelectionName = "Run Macro";
            CommandEnabled = true;
            
            v_InstanceName = "DefaultExcel";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vMacro = v_MacroName.ConvertUserVariableToString(engine);
            var excelObject = v_InstanceName.GetAppInstance(engine);
            var excelInstance = (Application)excelObject;
            excelInstance.Run(vMacro);           
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_InstanceName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_MacroName", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [Run Macro '{v_MacroName}' - Instance Name '{v_InstanceName}']";
        }
    }
}