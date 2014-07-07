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
    public partial class Foglio1
    {
        private void Foglio1_Startup(object sender, System.EventArgs e)
        {
            // See http://msdn.microsoft.com/en-us/library/d7f63219.aspx?appId=Dev10IDEF1&l=EN-US&k=k(MSDNSTART)&rd=true
            Microsoft.Office.Tools.Excel.NamedRange nr =
                this.Controls.AddNamedRange(this.Range["A2", missing], "NamedRange1");
            nr.Value2 = "This text was added by using code";

        }

        private void Foglio1_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(Foglio1_Startup);
            this.Shutdown += new System.EventHandler(Foglio1_Shutdown);
        }

        #endregion

    }
}
