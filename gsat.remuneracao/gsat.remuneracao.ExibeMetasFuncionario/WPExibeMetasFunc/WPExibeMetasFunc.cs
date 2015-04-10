using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Cit.Globosat.Common;

namespace Globosat.Remuneracao.ExibeMetasFuncionario.WPExibeMetasFuncionario
{
    public enum SiteLists { Ano_2012, Ano_2013, Ano_2014, Ano_2012_2013, Ano_2013_2014 };

    [ToolboxItemAttribute(false)]
    public class WPExibeMetasFunc : WebPart
    {
        [WebBrowsable(true), 
        WebDisplayName("Selecione o Ano:"),
        WebDescription("Filtro a ser realizado no preenchimento dos dados."),
        Personalizable(PersonalizationScope.Shared), Category(Constants.GroupEditWebPart),
        DefaultValue(SiteLists.Ano_2013_2014)]
        public SiteLists Ano { get; set; }

        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Globosat.Remuneracao.ExibeMetasFuncionario/WPExibeMetasFunc/WPExibeMetasFuncUC.ascx";
        protected override void CreateChildControls()
        {
            WPExibeMetasFuncUC control = (WPExibeMetasFuncUC)Page.LoadControl(_ascxPath);
            control.Ano = this.Ano;
            Controls.Add(control);
        }
    }
}

