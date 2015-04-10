using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using CIT.Sharepoint.Util;
using System.Diagnostics;

namespace Globosat.Remuneracao.LieAceito.WPLieAceito
{
    public partial class WPLieAceitoUC : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
                //Se usuário logado ainda não deu o aceite.
//            if (!VerificaStatus(SPContext.Current.Web.CurrentUser))
//            {
//                string strUrl = SPContext.Current.Site.RootWeb.Url + "/_layouts/Globosat.Remuneracao.LieAceito/TermoAceite.aspx";
//                string strScript = @"
//                function openModalDialog() {
//    var options = SP.UI.$create_DialogOptions();
//    options.width = 500;
//    options.allowMaximize = false;
//    options.height = 250;
//    options.url = '" + strUrl + @"'
//    options.dialogReturnValueCallback =
//        Function.createDelegate(null, whenMyModalDialogCloses);
//    SP.UI.ModalDialog.showModalDialog(options);
//
//} 
//
//function whenMyModalDialogCloses() {
//    
//}
//";


//                if (!Page.IsPostBack)
//                {
//                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), strScript, true);

//                    OpenMySweetModalDialog();
//                }
//            }//Fim !VerificaStatus

        }
        protected void OpenMySweetModalDialog()
        {
            var script = string.Format(
                @"function reallyOpenDialogForRealYouGuys() {{ 
            openModalDialog(); 
        }}; 
        SP.SOD.executeOrDelayUntilScriptLoaded(reallyOpenDialogForRealYouGuys, ""sp.ui.dialog.js""); ");

            Page.ClientScript.RegisterStartupScript(
                this.GetType(), Guid.NewGuid().ToString(), script, true);
        }
       

        /// <summary>
        /// Verifica se usuário logado já leu o termo de aceite.
        /// Para isso, consulta-se a lista "LieConcordoStatus"
        /// </summary>
        /// <param name="sPUser"></param>
        private bool VerificaStatus(SPUser sPUser, string centroCusto)
        {
            bool lido = false;
            SPSite site = new SPSite(SPContext.Current.Site.ID);
            SPWeb web = site.OpenWeb(SPContext.Current.Web.ID);

            try
            {
                #region Elevando permissões
                SPUserToken sysToken = site.SystemAccount.UserToken;
                using (SPSite siteAdmin = new SPSite(site.ID, sysToken))
                {
                    using (SPWeb webAdmin = siteAdmin.OpenWeb(web.ID))
                    {
                        SPList oList = webAdmin.Lists["LiEConcordoStatus"];
                        SPQuery query = new SPQuery();
                        query.Query = string.Format(@"<Where>
                            <And>
                            <Eq>
                                <FieldRef Name='Usuario' LookupId='TRUE' />
                                    <Value Type='Int'>{0}</Value>
                            </Eq>
                            <Eq>
                                <FieldRef Name='CentroCusto' />
                                    <Value Type='Text'>{1}</Value>
                            </Eq>
                            </And>
                        </Where>", sPUser.ID, centroCusto);

                        SPListItemCollection itemCollection = oList.GetItems(query);
                        if (itemCollection.Count > 0)
                            lido = true;
                        else
                            lido = false;
                    }
                }
                #endregion
                
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Erro ao buscar status do usuário '{0}': {1}.",sPUser.LoginName , ex.Message + ex.StackTrace), EventLogEntryType.Error, 2, 2);
            }
            finally
            {

                web.Dispose();
                site.Dispose();
            }
            return lido;
        }
    }
}
