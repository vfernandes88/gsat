using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Cit.Globosat.Common;

namespace Cit.Globosat.Premios.WPFuncionariosPremios
{
    [ToolboxItemAttribute(false)]
    public class WPFuncionariosPremios : WebPart
    {
        [WebBrowsable(true),
        WebDisplayName("Caminho do relatório"),
        WebDescription("Local onde está o arquivo rdlc do relatório."),
        Personalizable(PersonalizationScope.Shared), Category(Constants.GroupEditWebPart)]
        public string ReportPath { get; set; }

        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Cit.Globosat.Premios/WPFuncionariosPremios/WPFuncionariosPremiosUserControl.ascx";

        protected override void CreateChildControls()
        {
            WPFuncionariosPremiosUserControl control = (WPFuncionariosPremiosUserControl)Page.LoadControl(_ascxPath);
            control.ReportPath = this.ReportPath;
            Controls.Add(control);
        }
    }
}
