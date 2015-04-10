using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.IO;
using System.Collections;
using Globosat.Library.AcessoDados;
using System.Data;
using Globosat.Library.Servicos;
using Globosat.Library.Entidades;
using CIT.Sharepoint.Util;
using System.Diagnostics;

namespace Globosat.Remuneracao.CustomFields
{
    class CentroCustoFieldControl : BaseFieldControl
    {
        protected DropDownList CentroCustoDropDownList;
        protected Label CentroCustoLabel;

        protected override string DefaultTemplateName
        {
            get
            {
                return "CentroCustoFieldControl";
            }
        }

        public override string DisplayTemplateName
        {
            get
            {
                return "CentroCustoFieldControlForDisplay";
            }
            set
            {
                base.DisplayTemplateName = value;
            }
        }
        protected override void CreateChildControls()
        {
            DataTable dtCentrosCusto = null;
            base.CreateChildControls();

            #region Teste
            if (Field == null)
                return;
            if (ControlMode == SPControlMode.Display)
                return;

            CentroCustoDropDownList = (DropDownList)TemplateContainer.FindControl("CentroCustoDropDownList");
            CentroCustoLabel = (Label)TemplateContainer.FindControl("CentroCustoLabel");

            if (CentroCustoDropDownList == null)
                throw new ArgumentException("Valor Inválido!");

            CentroCustoDropDownList.TabIndex = TabIndex;
            CentroCustoDropDownList.CssClass = CssClass;
            CentroCustoDropDownList.ToolTip = "Centros de Custos do Usuário";

            if (ControlMode == SPControlMode.New)
            {
                dtCentrosCusto = new DataTable();

                //Get Centros de Custo do Usuário Logado
                dtCentrosCusto = PreencheCentroCustosUsuario();
                //#region Teste
                //CentroCustoDropDownList.Items.Add(new ListItem("Selecione...", "0"));
                //CentroCustoDropDownList.Items.Add(new ListItem(SPContext.Current.Web.CurrentUser.LoginName, SPContext.Current.Web.CurrentUser.Name));
                //CentroCustoDropDownList.Items.Add(new ListItem("RH", "RHV"));
                //CentroCustoDropDownList.Items.Add(new ListItem("Comercial", "ComercialV"));
                //CentroCustoDropDownList.Items.Add(new ListItem("Administrativo", "AdministrativoV"));

                //#endregion

                CentroCustoDropDownList.Items.Add(new ListItem("Selecione...", "0"));

                if (dtCentrosCusto != null)
                {
                    foreach (DataRow centroCusto in dtCentrosCusto.Rows)
                        CentroCustoDropDownList.Items.Add(new ListItem(centroCusto["CODSECAO"].ToString() + " - " + centroCusto["DESCRICAO"].ToString(), centroCusto["CODSECAO"].ToString()));
                    //CentroCustoDropDownList.Items.Add(new ListItem(centroCusto["CODSECAO"].ToString() + " - " + centroCusto["DESCRICAO"].ToString(), centroCusto["CODSECAO"].ToString() + " - " + centroCusto["DESCRICAO"].ToString()));
                }

                if (CentroCustoDropDownList.Items.Count <= 1)
                    CentroCustoLabel.Text = "Não existe um Centro de Custo associado ao seu usuário.<BR>Por favor, contate o administrador.";
                //throw new SPFieldValidationException("<b>Não existe um Centro de Custo associado ao seu usuário!</b>.<BR><b>Por favor, contate o administrador.</b>");
                else
                    CentroCustoLabel.Text = string.Empty;
            }
            else if (ControlMode == SPControlMode.Edit)
            {
                dtCentrosCusto = new DataTable();

                dtCentrosCusto = PreencheCentroCustosUsuario();

                if (dtCentrosCusto != null)
                {
                    int count = 0;
                    int index = 0;
                    foreach (DataRow centroCusto in dtCentrosCusto.Rows)
                    {
                        CentroCustoDropDownList.Items.Add(new ListItem(centroCusto["CODSECAO"].ToString() + " - " + centroCusto["DESCRICAO"].ToString(), centroCusto["CODSECAO"].ToString()));

                        if (Item["C Custo"] != null)
                        {
                            if (Item["C Custo"].ToString() == centroCusto["CODSECAO"].ToString())
                            {
                                index = count;
                            }
                        }
                        count++;
                    }
                        CentroCustoDropDownList.SelectedIndex = index;
                }
            }

            #endregion
        }

        private DataTable PreencheCentroCustosUsuario()
        {
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
                        if (ManipularDados.VerificaLogin(web, login, "Administradores Remuneração"))
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
        }

        public override object Value
        {
            get
            {
                EnsureChildControls();
                //return CentroCustoDropDownList.Text.Trim();
                return CentroCustoDropDownList.SelectedValue;
            }
            set
            {
                EnsureChildControls();
                base.Value = value;
            }
        }
    }
}
