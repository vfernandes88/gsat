using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Linq;
using Microsoft.SharePoint.WebControls;
using System.Text;
using Microsoft.SharePoint.Utilities;
using System.IO;
using Microsoft.Office.Server.UserProfiles;
using Globosat.Library.Servicos;
using CIT.Sharepoint.Util;
using System.Diagnostics;
using Globosat.Library.Entidades;
using System.Data;
using System.Globalization;


namespace MelhoriaMetas.WPExibeMetas
{
    public partial class WPExibeMetasUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                DataTable dtCentrosCusto = new DataTable();


                #region Teste
                //ddlCentroCusto.Items.Add(new ListItem("Selecione...", "0"));
                //ddlCentroCusto.Items.Add(new ListItem("RH", "RHV"));
                //ddlCentroCusto.Items.Add(new ListItem("Comercial", "ComercialV"));
                //ddlCentroCusto.Items.Add(new ListItem("Administrativo", "AdministrativoV"));
                //ddlCentroCusto.AutoPostBack = true;

                #endregion


                #region Produção
                ddlCentroCusto.AutoPostBack = true;
                //Get Centros de Custo do Usuário Logado
                dtCentrosCusto = PreencheCentroCustosUsuario();
                ddlCentroCusto.Items.Add(new ListItem("Selecione...", "0"));

                if (dtCentrosCusto != null)
                {
                    foreach (DataRow centroCusto in dtCentrosCusto.Rows)
                        ddlCentroCusto.Items.Add(new ListItem(centroCusto["CODSECAO"].ToString() + " - " + centroCusto["DESCRICAO"].ToString(), centroCusto["CODSECAO"].ToString()));
                }
                #endregion
            }
            else
            {
                if (ddlCentroCusto.SelectedItem.Value != null && ddlCentroCusto.SelectedValue != "0")
                {
                    #region Monta Layout de Meta

                    #region Limpa Página
                    //Limpa página
                    lbltabela.Text = string.Empty;
                    lblErro.Text = string.Empty;
                    lblLink.Text = string.Empty;
                    #endregion

                    //Monta HTML Template
                    string strTemplate = SelecionaTemplate(ddlCentroCusto.SelectedItem.Text);
                    string listItemIDArquitvo = string.Empty;
                    if (!string.IsNullOrEmpty(strTemplate))
                    {
                        string[] s = strTemplate.Split(';');
                        listItemIDArquitvo = s[1].ToString();
                        strTemplate = s[0].ToString();

                    }

                    //Valida strTemplate
                    if (string.IsNullOrEmpty(strTemplate))
                    {
                        lblErro.Text = "Não há um <b>template</b> associado ao <b>centro de custo</b> selecionado.<br>"
                            + "Para fazer esta associação, <a href='/Remuneracoes/Lists/Templates%20e%20Centros%20de%20Custo/AllItems.aspx' target='_blank'>clique aqui</a>.";
                        lbltabela.Text = string.Empty;
                    }
                    else
                        lblErro.Text = string.Empty;

                    using (SPSite site = new SPSite(SPContext.Current.Site.Url))
                    {
                        using (SPWeb web = site.OpenWeb("Remuneracoes"))
                        {

                            #region Monta HTML Template
                            SPList List = web.Lists["Metas"];
                            SPQuery oQuery = new SPQuery();
                            oQuery.Query = @" 
                     <Where>
                      <And>
                         <Eq>
                            <FieldRef Name='Template' />
                            <Value Type='Lockup'>" + strTemplate + @"</Value>
                         </Eq>
                         <Eq>
                            <FieldRef Name='Exibir' />
                            <Value Type='Boolean'>1</Value>
                         </Eq>
                      </And>
                   </Where>
                   <OrderBy>
                      <FieldRef Name='Ordem' Ascending='True' />
                   </OrderBy>";

                            SPListItemCollection lista = List.GetItems(oQuery);
                            if (lista.Count.Equals(0))
                                return;
                            StringBuilder sbParticipe = new StringBuilder();
                            StringBuilder sbBonus = new StringBuilder();
                            StringBuilder sbValorB = new StringBuilder();
                            StringBuilder sbValorP = new StringBuilder();
                            StringBuilder sbBaseParticipe = new StringBuilder();
                            StringBuilder sbBaseValor = new StringBuilder();
                            StringBuilder sbResultadoInternet = new StringBuilder();
                            StringBuilder sbValorResultado = new StringBuilder();
                            StringBuilder sbBaseResultado = new StringBuilder();

                            lbltabela.Text += "<style type='text/css'>" +
                                                    ".style1 {" +
                                                        "text-align: center; }" +
                                                    ".style2 {" +
                                                        "background-color: #FFFF9F;}" +
                                                    ".style3 {" +
                                                        "background-color: #0073CE; color:white;}" +
                                                   ".style4 {" +
                                                        "border-left-width:0px; border-left-style:none;}" +
                                                        "</style>" +
                                                    "<table style='width: 100%' class='style3' border='1'>" +
                                                        "<tr>" +
                                                            "<td class='style1' style='height: 30px'><strong><span class='style3'>METAS " + lista[0]["Ano"] +
                                                            "</span></strong></td>" +
                                                        "</tr>" +
                                                    "</table>" +
                                                    "<p>&nbsp;</p>" +
                                                    "<table style='width: 100%' border='1' style='border-color:black'>" +
                                                        "<tr>" +
                                                            "<td class='style1'><strong>Participe Variavel</strong></td>" +
                                                            "<td colspan='2' class='style1'><strong>Bônus</strong></td>" +
                                                        "</tr>";
                            bool resultado = false;
                            foreach (SPListItem item in lista)
                            {

                                //style='color: #008000; font-weight: bold;'

                                if (item["Categoria"].Equals("PV"))
                                {
                                    sbParticipe.Append("<td height='85px' width='130px' align='center' class='style2' >" + item["Descricao"] + "</td>");
                                    sbValorP.Append("<td height='30px' align='center'>" + item["Valor"] + "%" + "</td>");
                                    sbBaseParticipe.Append("<td height='20px' class='style2' align='center' style='color: #008000;'>" + item["valorMonetario"] + "</td>");
                                }
                                else
                                {
                                    if (item["Descricao"].Equals("Visitantes Únicos no Subdomínio") || item["Descricao"].Equals("Visitantes Únicos nas prop. canal") || item["Descricao"].Equals("Receitas Líquidas de Internet") || item["Descricao"].Equals("VideoViews de íntegras em prod. p/ ass."))
                                    {
                                        resultado = true;
                                        sbResultadoInternet.Append("<td height='40px' width='130px' align='center' class='style2' >" + item["Descricao"] + "</td>");
                                        sbValorResultado.Append("<td height='30px' align='center'>" + item["Valor"] + "%" + "</td>");
                                        sbBaseResultado.Append("<td height='20px' class='style2' align='center' style='color: #008000;'>" + item["valorMonetario"] + "</td>");
                                    }
                                    else
                                    {
                                        sbBonus.Append("<td  height='85px' width='130px' align='center' class='style2'>" + item["Descricao"] + "</td>");
                                        sbValorB.Append("<td height='30px' align='center'>" + item["Valor"] + "%" + "</td>");
                                        sbBaseValor.Append("<td height='20px' class='style2' align='center' style='color: #008000;'>" + item["valorMonetario"] + "</td>");
                                    }
                                }

                            }
                            if (resultado)
                            {
                                lbltabela.Text += "<tr><td><table><tr><td><table cellpadding='10px' cellspacing='5px'><tr>" + sbParticipe.ToString() + "</tr><tr>" + sbValorP.ToString() + "</tr><tr>" + sbBaseParticipe.ToString() + "</tr></table></td></tr></table></td>" + "<td><table cellpadding='10px' cellspacing='5px'><tr><td height='10px' align='center' colspan='4' class='style2'>Resultado de Internet</td></tr><tr>" + sbResultadoInternet.ToString() + "</tr><tr>" + sbValorResultado.ToString() + "</tr><tr>" + sbBaseResultado.ToString() + "</tr></table></td>" + "<td class='style4'><table cellpadding='10px' cellspacing='5px'><tr></tr><tr>" + sbBonus.ToString() + "</tr><tr>" + sbValorB.ToString() + "</tr><tr>" + sbBaseValor.ToString() + "</tr></table></td>" + "</tr></table>";
                            }
                            else
                            {
                                lbltabela.Text += "<tr><td><table cellpadding='1px' cellspacing='5px'><tr><td><tr>" + sbParticipe.ToString() + "</tr><tr>" + sbValorP.ToString() + "</tr><tr>" + sbBaseParticipe.ToString() + "</tr></td></tr></table></td><td><table cellpadding='1px' cellspacing='5px'><tr><td><tr>" + sbBonus.ToString() + "</tr><tr>" + sbValorB.ToString() + "</tr><tr>" + sbBaseValor.ToString() + "</tr></td></tr></table></td></tr></table>";
                            }



                            if (lista.Count > 0)
                            {
                                //Varre a lista dos resultados obtidos
                                //foreach (SPListItem item in lista)
                                //{
                                //se o centro de custo for igual ao do usuario ele continua a "montar" a web Part
                                //if (strTemplate == item.Title)
                                //{

                                if (ExistePasta(listItemIDArquitvo, web))
                                {
                                    //Pega a pasta de anexos dentra lista de arquivo    
                                    SPFolder folder = web.Folders["Lists"].SubFolders["Templates e Centros de Custo"].SubFolders["Attachments"].SubFolders[listItemIDArquitvo];

                                    if (folder.Files.Count > 0)
                                    {
                                        lblLink.Text = "<b>Clique no arquivo abaixo para visualizar os projetos negociados da sua área:</b><br><br>";
                                        //Varre os arquivos na pasta dos anexos                
                                        foreach (SPFile file in folder.Files)
                                        {
                                            //Verifica a extensão do arquivo
                                            string strFileExtension = new FileInfo(file.Name).Extension;
                                            strFileExtension = tipoArquivo(strFileExtension);


                                            if (strFileExtension == "arquivo")
                                            {
                                                lblLink.Visible = true;
                                                lblLink.Text += @"                                                        <table>
	<tr>
		<td>
			 <img src='http://rj2k8shp01/remuneracoes/SiteAssets/logopdf.png' width='29' height='39'/>
		</td>
		<td>";

                                                lblLink.Text += "<a href=\"";
                                                lblLink.Text += web.Url + "/" + file.Url + "\"";
                                                lblLink.Text += " target='_blank'>" + file.Name + "</a>";
                                                lblLink.Text += @"</td>
	</tr>
</table>";
                                            }

                                        }
                                    }

                                }

                                //}

                                // }

                            }
                            #endregion

                        }
                    } 
                    #endregion
                }
            }
        }

        private DataTable PreencheCentroCustosUsuario()
        {
      

            #region Produção
            string login = string.Empty;
            Gerente dadosProfile = null;
            DataTable dtCentrosCusto = null;

            dadosProfile = new Gerente();

            login = SPContext.Current.Web.CurrentUser.LoginName;

            try
            {
                //Busca matrícula e coligada do usuário atual.
                dadosProfile = ManipularDados.BuscaMatriculaColigada(login);

                SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;
                using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
                {
                    using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        if (PossuiAcessoTotal(login))
                            dtCentrosCusto = ManipularDados.BuscaTodosCentrosCusto();
                        else
                            dtCentrosCusto = ManipularDados.BuscaCentroCusto(dadosProfile.Matricula, dadosProfile.Coligada);

                        return dtCentrosCusto;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao preencher campo Centro de Custo: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                return null;
            } 
            #endregion
        }

        private bool PossuiAcessoTotal(string login)
        {
            bool possuiAcesso = false;

            SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;

            using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
            {

                using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                {
                    SPUser userLogado = SPContext.Current.Web.CurrentUser;
                    SPGroup grupoAdministrador = web.Groups["Grupo_Remuneração_Administradores"];
                   

                    foreach (SPUser administrador in grupoAdministrador.Users)
                    {
                        if (administrador.LoginName.Equals(login))
                            possuiAcesso = true;

                    }
                   
                }
            }
            return possuiAcesso;
        }
        private string SelecionaTemplate(string strCentroCusto)
        {
            string strTemplate = string.Empty;

            using(SPSite site = new SPSite(SPContext.Current.Site.Url))
            {
                using (SPWeb web = site.OpenWeb("Remuneracoes"))
                {
                    SPList listTemplate = web.Lists["Templates e Centros de Custo"];
                    SPQuery query = new SPQuery();
                    query.Query = @"<Where><Eq><FieldRef Name='Centros_x0020_de_x0020_custo' /><Value Type='Lookup'>"
                                    + strCentroCusto + @"</Value></Eq></Where>";

                    SPListItemCollection lista = listTemplate.GetItems(query);

                    foreach (SPListItem listITem in lista)
                    {
                        strTemplate = listITem["Template"].ToString() + ";" + listITem.ID.ToString();
                    }

                }
            }

          
            return strTemplate;
        }

        private bool ExistePasta(string p, SPWeb web)
        {
            bool existe = false;
            foreach (SPFolder folder in web.Folders["Lists"].SubFolders["Templates e Centros de Custo"].SubFolders["Attachments"].SubFolders)
            {
                if (folder.Name.Equals(p))
                    existe = true;
            }
            return existe;

        }

        //Metodo que verifica a extensão dos arquivos
        public static string tipoArquivo(string strFileExtension)
        {
            string arquivo = null;
            //verifica se  é uma imagem
            if (strFileExtension == ".xlsx" || strFileExtension == ".xlsm" || strFileExtension == ".xls" || strFileExtension == ".xlt" || strFileExtension == ".xla" || strFileExtension == ".ods" || strFileExtension == ".doc" || strFileExtension == ".docx" || strFileExtension == ".odt" || strFileExtension == ".xltx" || strFileExtension == ".pdf")
            {
                arquivo = "arquivo";
            }
            else
            {
                arquivo = "outro";
            }

            //Retorna de acordo com a verificação
            return arquivo;
        }
        //Metodo que busca o centro de custo do usuario
        public static String BuscaCentroCusto(string propriedade)
        {

            //Cria uma variavel que armazena o profile e outra que guardara o centro de custo
            UserProfile u = null;
            string xpto = null;

            //Execução com privilegios elevados por tratar de informações de usuarios
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = SPContext.Current.Site)
                {
                    //Verifica se existe um usuario e Captura o usuario corrente
                    string userLogin = SPContext.Current.Web.CurrentUser.LoginName;
                    SPServiceContext ctx = SPServiceContext.GetContext(site);
                    UserProfileManager upm = new UserProfileManager(ctx);

                    if (upm.UserExists(userLogin))
                    {
                        //Captura e armazena o centro de custo de acordo com o user
                        u = upm.GetUserProfile(userLogin);
                        xpto = u[propriedade].ToString();
                    }
                    else
                    {
                        //Se nao houver centro de custo retorna null
                        xpto = null;
                    }
                }
            });
            //Se capturou retorna o centro de custo atraves de uma string
            return xpto;
        }

        
        
    }
}

