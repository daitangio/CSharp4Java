using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;

namespace ExcelWorkbookStudium
{
    public partial class Foglio3
    {
        private void Foglio3_Startup(object sender, System.EventArgs e)
        {
        }

        private void Foglio3_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(Foglio3_Startup);
            this.Shutdown += new System.EventHandler(Foglio3_Shutdown);
        }

        #endregion

    }
}
