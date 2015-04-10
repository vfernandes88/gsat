using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Cit.Globosat.Common;

namespace Cit.Globosat.Remuneracao.Formularios.WebParts.AltFuncCargoVWP
{
    [ToolboxItemAttribute(false)]
    public class AltFuncCargoVWP : WebPart
    {
        [WebBrowsable(true),
        WebDisplayName("Botão imprimir PDF habilitado"),
        WebDescription("Ativa e desativa o botão para realizar a exportação para o formato PDF."),
        Personalizable(PersonalizationScope.Shared), Category(Constants.GroupEditWebPart)]
        public bool PDFButtonVisible { get; set; }

        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Cit.Globosat.Remuneracao.Formularios.WebParts/AltFuncCargoVWP/AltFuncCargoVWPUserControl.ascx";

        protected override void CreateChildControls()
        {
            AltFuncCargoVWPUserControl control = (AltFuncCargoVWPUserControl)Page.LoadControl(_ascxPath);
            control.PDFButtonVisible = this.PDFButtonVisible;
            Controls.Add(control);
        }
    }
}
