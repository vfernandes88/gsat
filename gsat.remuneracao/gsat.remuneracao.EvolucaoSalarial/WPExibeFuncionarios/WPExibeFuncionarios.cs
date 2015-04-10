using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Globosat.Remuneracao.EvolucaoSalarial.WPExibeFuncionarios
{
    [ToolboxItemAttribute(false)]
    public class WPExibeFuncionarios : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Globosat.Remuneracao.EvolucaoSalarial/WPExibeFuncionarios/WPExibeFuncionariosUserControl.ascx";

        protected override void CreateChildControls()
        {
            this.ChromeType = PartChromeType.None;
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
