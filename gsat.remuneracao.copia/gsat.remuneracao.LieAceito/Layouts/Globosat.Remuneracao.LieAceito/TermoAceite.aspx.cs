using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using CIT.Sharepoint.Util;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using Microsoft.Office.Server.UserProfiles;
using Globosat.Library.Servicos;

namespace Globosat.Remuneracao.LieAceito.Layouts.Globosat.Remuneracao.LieAceito
{
    public partial class TermoAceite : LayoutsPageBase
    {
        //protected CheckBox cbTermo;
        //protected Button btnSalvar;
        protected ImageButton btnSalvar;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            btnSalvar.Click += new ImageClickEventHandler(btnSalvar_Click);

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            lblTermo.Text = BuscaTermoAceite("termoaceite");
        }

        void btnSalvar_Click(object sender, EventArgs e)
        {
            string centroCusto = string.Empty;
            HttpContext context = HttpContext.Current;
            if(context.Request.QueryString["CENTROCUSTO"] != null)
                centroCusto = context.Request.QueryString["CENTROCUSTO"];

            GravaAceite(SPContext.Current.Web.CurrentUser, centroCusto);
        }

        /// <summary>
        /// Armazena em lista o usuário do contexto incluindo/alterando o status para lido!
        /// </summary>
        /// <param name="sPUser"></param>
        private void GravaAceite(SPUser sPUser, string centroCusto)
        {
            //Verificar se usuário já existe. Se existir anterar STatus para "Lido".
            //Se não existir, incluir na lista como lido.
            string[] userName = sPUser.LoginName.Split('\\');
            
            SPSite site = new SPSite(SPContext.Current.Site.ID);
            SPWeb web = site.OpenWeb("Remuneracoes");

            try
            {
                
                #region Elevando permissões
                SPUserToken sysToken = site.SystemAccount.UserToken;
                using (SPSite siteAdmin = new SPSite(site.ID, sysToken))
                {
                    using (SPWeb webAdmin = siteAdmin.OpenWeb(web.ID))
                    {
                        
                        SPList oList = webAdmin.Lists["LiEConcordoStatus"];

                        #region Busca dados do UserProfile
                        //Instancia contexto para busca em profile
                        SPServiceContext serviceContext = SPServiceContext.GetContext(SPContext.Current.Site);
                        // Inicializa o usuário gerenciador de perfis
                        UserProfileManager upm = new UserProfileManager(serviceContext);
                        UserProfile profile = null;

                        //Busca a foto do colaborador
                        if ((upm.UserExists(sPUser.LoginName)) && centroCusto != string.Empty) 
                        {
                            profile = upm.GetUserProfile(sPUser.LoginName);

                                webAdmin.AllowUnsafeUpdates = true;
                                SPListItem i = oList.AddItem();
                                i["Usuario"] = sPUser;
                                i["Title"] = sPUser.Name;
                                i["Status"] = "Lido";
                                i["Data"] = System.DateTime.Now;
                                i["Cargo"] = profile["SPS-JobTitle"].Value != null ? profile["SPS-JobTitle"].Value.ToString() : string.Empty;
                                i["Matricula"] = profile["Matricula"].Value != null ? profile["Matricula"].Value.ToString() : string.Empty;
                                i["Coligada"] = profile["Coligada"].Value != null ? profile["Coligada"].Value.ToString() : string.Empty;
                                i["Empresa"] = profile["Empresa"].Value != null ? profile["Empresa"].Value.ToString() : string.Empty;
                                i["CentroCusto"] = centroCusto;
                                i.Update();
                                webAdmin.AllowUnsafeUpdates = false;
                        }
                        
                        #endregion
                        
                        FechaPopup();
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Erro ao buscar na lista 'LiEConcordoStatus' pelo usuário '{0}': {1}.", sPUser.LoginName, ex.Message + ex.StackTrace), EventLogEntryType.Error, 2, 2);
            }
            finally
            {
                web.Dispose();
                site.Dispose();
                
            }

        }

        private void FechaPopup()
        {
            HttpContext context = HttpContext.Current;
            if (context.Request.QueryString["IsDlg"] != null)
            {
                context.Response.Write("<script type='text/javascript'>window.frameElement.commitPopup()</script>");
                context.Response.Flush();
                context.Response.End();
            }

        }

        
        //void cbTermo_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (cbTermo.Checked)
        //        btnSalvar.Enabled = true;
        //    else
        //        btnSalvar.Enabled = false;
        //}
        
       

        /// <summary>
        /// Busca o termo de aceite cadastrado na lista LiEConcordoConfig.
        /// </summary>
        /// <returns></returns>
        private string BuscaTermoAceite(string strChave)
        {

            SPSite site = new SPSite(SPContext.Current.Site.ID);
            SPWeb web = site.OpenWeb("Remuneracoes");

            try
            {
                #region Elevando permissões
                SPUserToken sysToken = site.SystemAccount.UserToken;
                using (SPSite siteAdmin = new SPSite(site.ID, sysToken))
                {
                    using (SPWeb webAdmin = siteAdmin.OpenWeb(web.ID))
                    {
                        SPList oList = webAdmin.Lists["Remuneracoes_config"];
                        SPQuery query = new SPQuery();
                        query.Query =string.Format(@"<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where>", strChave);

                        SPListItemCollection itemCollection = oList.GetItems(query);

                        if (itemCollection.Count > 0)
                            return itemCollection[0]["Valor"].ToString();
                        else
                        {
                            return "<div style='color:red'><b>Não há termo de aceite cadastrado. Favor entrar em contato com o Administrador.</b></div>";
                        }
                    }
                }
                #endregion
                
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Erro ao buscar na lista 'Remuneracoes_config' pela chave '{0}' : {1}.", strChave, ex.Message + ex.StackTrace), EventLogEntryType.Error, 2, 2);
            }
            finally
            {
                web.Dispose();
                site.Dispose();
            }
            return null;
        }
    }
}
