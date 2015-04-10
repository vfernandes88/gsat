using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text;
using Microsoft.SharePoint.Utilities;
using System.IO;
using System.Collections.Generic;
using Globosat.Library;
using CIT.Sharepoint.Util;
using Globosat.Library.Entidades;
using Globosat.Library.Servicos;
using Microsoft.Office.Server.UserProfiles;
using System.Data;
using System.Diagnostics;


namespace WPTabelaRemuneracaoVariavel.WPTabelaRemuneracaoVariavel
{
    public partial class WPTabelaRemuneracaoVariavelUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            try
            {
                if (!this.IsPostBack)
                {
                    #region Preenche DropDownList de Ano
                    DataTable dtAnos = new DataTable();
                    dtAnos = PreencheAno();
                    ddlAno.Items.Add(new ListItem("Selecione...", "0"));
                    ddlAno.AutoPostBack = true;
                    if (dtAnos != null)
                    {
                        foreach (DataRow ano in dtAnos.Rows)
                            ddlAno.Items.Add(new ListItem(ano["Ano"].ToString().Trim(), ano["Ano"].ToString().Trim()));
                    }
                    ddlAno.SelectedValue = (DateTime.Now.Year - 1).ToString();
                    #endregion

                    MontaTabela();

                }
                else
                    MontaTabela();
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao gerar Tabela de Remuneração Variável: " + ex.Message + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 3, 1);

                lblTeste.Text = "<b>Não foi possível gerar a tabela de Remuneração Variável devido a um erro. Favor entrar em contato com o administrador.</b>";
            }
        }

        private void MontaTabela()
        {
            //limpa tabela
            lblTeste.Text = string.Empty;
            string login = string.Empty;
            bool UsuarioAdministrador = false;
            bool UsuarioGerente = false;

            using (SPSite site = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    login = web.CurrentUser.LoginName;
                }
            }

            string userclass = "0";

            SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;
            using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
            {
                using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                {
                    UsuarioAdministrador = PossuiAcessoTotal(login);
                    UsuarioGerente = EhGerente(login);

                    //Se usuário é administrador ou Gerente
                    if (UsuarioAdministrador || UsuarioGerente)
                    {
                        UserProfile up = ManipularDados.BuscaProfile(login);
                        SPList list = web.Lists["Remuneracao Variavel"];
                        SPListItemCollection itemCollection;

                        SPQuery oQuery = new SPQuery();
                        //oQuery.Query = "<OrderBy><FieldRef Name=\"Classe\" Ascending=\"True\" /></OrderBy>";
                        oQuery.Query = string.Format("<Where><Eq><FieldRef Name='Ano' /><Value Type='Text'>{0}</Value></Eq></Where><OrderBy><FieldRef Name='Classe' Ascending='True' /></OrderBy>", ddlAno.SelectedValue.Trim());

                        itemCollection = list.GetItems(oQuery);

                        List<SPListItem> colecaoTabela1 = new List<SPListItem>();
                        List<SPListItem> colecaoTabela2 = new List<SPListItem>();


                        foreach (SPListItem item in itemCollection)
                        {
                            if (item["Classe"].ToString() == "1 a 10")
                            {
                                colecaoTabela1.Add(item);
                            }
                            else if (Convert.ToInt32(item["Classe"]) <= 16)
                            {
                                colecaoTabela1.Add(item);
                            }
                            else if (Convert.ToInt32(item["Classe"]) > 16)
                            {
                                colecaoTabela2.Add(item);
                            }
                        }

                        if (up != null)
                        {
                            if (up["FaixaSalarial"].Value != null)
                                userclass = up["FaixaSalarial"].Value.ToString();

                        }
                        if (UsuarioAdministrador)
                            userclass = "20";

                        if (userclass != null)
                        {
                            //#0073CE - Azul
                            //#DFA200 - Dourado
                            lblTeste.Text += "<style type='text/css'>" +
                                                        ".cssColuna {text-align: center; border-style: solid; border-width: 1px;}" +
                                                        ".cssSubTitulo {text-align: center; border: 2px solid #0073CE;}" +
                                                        ".cssTitulo {border-style: solid; border-color:#0073CE; background-color: #0073CE; color: #FFFFFF;text-align: center;	font-weight: 600;}" +
                                              "</style>" +
                                                  "<table width='100%' >" +
                                                  "<tr><td colspan='5' class='cssTitulo'> REMUNERAÇÃO VARIAVEL </td></tr>" +
                                                     "<tr>" +
                                                        "<td rowspan='2' class='cssSubTitulo'>Classe Salarial</td>" +
                                                            "<td colspan='4' class='cssSubTitulo'><b>Nº SALÁRIOS</b></td>" +
                                                     "</tr>" +
                                                     "<tr style='width:auto'>" +
                                                        "<td class='cssSubTitulo'>PARTICIPE</td>" +
                                                        "<td class='cssSubTitulo'>PARTICIPE VARIAVEL</td>";

                            if (ddlAno.SelectedValue.Trim().Equals("2012"))
                                lblTeste.Text += "<td class='cssSubTitulo'>BÔNUS</td>";
                            
                                lblTeste.Text += "<td class='cssSubTitulo'>TOTAL</td>";

                                                     lblTeste.Text += "</tr><tr>";
                            int classe = 0;
                            bool podeInserir = false;
                            foreach (SPListItem item in colecaoTabela1)
                            {
                                podeInserir = false;

                                if (item["Classe"].ToString() == "1 a 10")
                                {
                                    podeInserir = true;
                                }

                                if ((int.TryParse(item["Classe"].ToString(), out classe) && classe <= Convert.ToInt32(userclass)) || podeInserir)
                                {


                                    lblTeste.Text += "   <td class='cssColuna'>          " +
                                               "         <span>" + item["Classe"] + "</span> " +
                                               "     </td>                        " +
                                               "   <td class='cssColuna'> " +
                                               "         <span>" + item["Participe_100"] + "</span>" +
                                               "  </td>" +
                                               "  <td class='cssColuna'> " +
                                               "         <span>" + item["Participe Variavel_100"] + "</span>" +
                                               "  </td>";

                                    if (ddlAno.SelectedValue.Trim().Equals("2012"))
                                    {
                                        lblTeste.Text += "  <td class='cssColuna'> " +
                                    "         <span>" + item["Bonus_100"] + "</span>" +
                                    "  </td>";
                                    }
                                                                             lblTeste.Text += "  <td class='cssColuna'> " +
                                         "         <span>" + item["Total_100"] + "</span>" +
                                         "  </td></tr>";
                                }
                            }

                            lblTeste.Text += "</table>";


                            if (Convert.ToInt32(userclass) > 16 || UsuarioAdministrador)
                            {
                                lblTeste.Text += "<style type='text/css'>" +
                                                        ".cssColuna {text-align: center; border-style: solid; border-width: 1px;}" +
                                                        ".cssSubTitulo {text-align: center; border: 2px solid #0073CE;}" +
                                                        ".cssTitulo {border-style: solid; border-color:#0073CE; background-color: #0073CE; color: #FFFFFF;text-align: center;	font-weight: 600;}" +
                                                  "</style>" +
                                                  "</br><table width='100%'>" +
                                                   "<tr><td colspan='10' class='cssTitulo'> REMUNERAÇÃO VARIAVEL </td></tr>" +
                                                     "<tr>" +
                                                        "<td rowspan='2' class='cssSubTitulo'>Classe Salarial</td>";
                                if (ddlAno.SelectedValue.Trim().Equals("2012"))
                                {
                                    lblTeste.Text += "<td colspan='4' class='cssSubTitulo'><b>Nº SALÁRIOS - para 100% Ebtida</b></br></td>" +
                                    "<td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>" +
                                    "<td colspan='4' class='cssSubTitulo'><b>Nº SALÁRIOS - para 130% Ebtida</b></td>";
                                }
                                else
                                {
                                    lblTeste.Text += "<td colspan='3' class='cssSubTitulo'><b>Nº SALÁRIOS - para 100% Ebtida</b></br></td>" +
                                    "<td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>" +
                                    "<td colspan='3' class='cssSubTitulo'><b>Nº SALÁRIOS - para 130% Ebtida</b></td>";
                                }
                                                     lblTeste.Text += "</tr>" +
                                                     "<tr style='width:auto'>" +
                                                        "<td class='cssSubTitulo'>PARTICIPE</td>" +
                                                        "<td class='cssSubTitulo'>PARTICIPE VARIAVEL</td>";
                                if (ddlAno.SelectedValue.Trim().Equals("2012"))
                                    lblTeste.Text += "<td class='cssSubTitulo'>BÔNUS</td>";

                                lblTeste.Text += "<td class='cssSubTitulo'>TOTAL</td>" +
                                 "<td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>" +
                                 "<td class='cssSubTitulo'>PARTICIPE</td>" +
                                 "<td class='cssSubTitulo'>PARTICIPE VARIAVEL</td>";
                                if (ddlAno.SelectedValue.Trim().Equals("2012"))
                                    lblTeste.Text += "<td class='cssSubTitulo'>BÔNUS</td>";
                                 lblTeste.Text += "<td class='cssSubTitulo'>TOTAL</td>" +
                              "</tr><tr>";

                                foreach (SPListItem item in colecaoTabela2)
                                {
                                    if ((Convert.ToInt32(item["Classe"]) <= Convert.ToInt32(userclass)) || UsuarioAdministrador)
                                    {
                                        lblTeste.Text += "<td class='cssColuna'>          " +
                                              "         <span>" + item["Classe"] + "</span> " +
                                              "     </td>                        " +
                                              "   <td class='cssColuna'> " +
                                              "         <span>" + item["Participe_100"] + "</span>" +
                                              "  </td>" +
                                              "  <td class='cssColuna'> " +
                                              "         <span>" + item["Participe Variavel_100"] + "</span>" +
                                              "  </td>";
                                        if (ddlAno.SelectedValue.Trim().Equals("2012"))
                                        {
                                            lblTeste.Text += "  <td class='cssColuna'> " +
                                                   "         <span>" + item["Bonus_100"] + "</span>" +
                                                   "  </td>";
                                        }
                                        lblTeste.Text += "  <td class='cssColuna'> " +
                                        "         <span>" + item["Total_100"] + "</span>" +
                                        "  </td>" +
                                        "<td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>" +
                                        "   <td class='cssColuna'> " +
                                        "         <span>" + item["Participe_130"] + "</span>" +
                                        "  </td>" +
                                        "  <td class='cssColuna'> " +
                                        "         <span>" + item["Participe Variavel_130"] + "</span>" +
                                        "  </td>";
                                        if (ddlAno.SelectedValue.Trim().Equals("2012"))
                                        {
                                            lblTeste.Text += "  <td class='cssColuna'> " +
                                            "         <span>" + item["Bonus_130"] + "</span>" +
                                            "  </td>";
                                        }
                                                   lblTeste.Text += "  <td class='cssColuna'> " +
                                                   "         <span>" + item["Total_130"] + "</span>" +
                                                   "  </td></tr>";
                                    }
                                }

                                lblTeste.Text += "</table>";
                            }
                        }
                        else
                        {
                            lblTeste.Text = "<b>Não foi possível gerar a tabela de Remuneração Variável. Favor entrar em contato com o administrador.</b>";
                        }
                    }
                }

            }
        }

        private DataTable PreencheAno()
        {
            try
            {
                DataTable dtAnos = null;
                SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;
                using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
                {
                    using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPQuery query = new SPQuery();
                        query.Query = "<OrderBy><FieldRef Name='Ano' /></OrderBy>";
                        dtAnos = web.Lists["Remuneracao Variavel"].GetItems(query).GetDataTable();
                        DataView dtv = new DataView(dtAnos);
                        dtAnos = dtv.ToTable(true, "Ano");
                        return dtAnos;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Tabela Rem. Variável > Erro ao preencher campo Ano: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                return null;
            }
        }

        private bool EhGerente(string login)
        {
            bool possuiAcesso = false;

            SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;

            using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
            {

                using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                {
                    SPUser userLogado = SPContext.Current.Web.CurrentUser;
                    SPGroup grupoGerentes = web.Groups["Grupo_Remuneração_Gerentes"];

                    
                    foreach (SPUser gerente in grupoGerentes.Users)
                    {
                        if (gerente.LoginName.Equals(login))
                        {
                            possuiAcesso = true;
                            return possuiAcesso;
                        }
                    }
                }
            }
            return possuiAcesso;
        }
        //Verifica se usuário está presente no grupo de Administradores ou de Gerentes
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
                        {
                            possuiAcesso = true;
                            return possuiAcesso;
                        }
                    }
                }
            }
            return possuiAcesso;
        }

        protected void Onclick_btnEnviar(object sender, ImageClickEventArgs e)
        {
            StringBuilder email = new StringBuilder();
            Label lblConteudo = new Label();
            string centroCusto = string.Empty;
            try
            {
                centroCusto = ddlAno.SelectedItem.Text;
                email.Append("<br/>" + lblTeste.Text);

            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao enviar email em Mais Detalhes: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                SPUtility.TransferToErrorPage("Ocorreu um erro ao enviar o email.", null, null);
            }

            Email.EnvioEmail(SPContext.Current.Web.CurrentUser.Email, "Metas Funcionarios", email.ToString());
            SPUtility.TransferToSuccessPage("Email enviado com sucesso.", "/Paginas/metasFuncionarios.aspx", null, null);
        }
    }
}