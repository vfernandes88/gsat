using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Web.UI.WebControls;
using System.Data;
using System.Diagnostics;
using System.IO;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Security;

using CIT.Sharepoint.Util;
using Globosat.Library.AcessoDados;
using Globosat.Library.Servicos;
using Globosat.Library.Entidades;

namespace Globosat.Remuneracao.FieldType.CCustoCargo
{
    [CLSCompliant(false)]
    [Guid("0F360096-A100-482f-B1FA-9EA31F5D096D")]
    public class CentroCustoCargoFieldControl : LinkedFieldRenderingControl
    {
        #region Atributos
        protected DropDownList centroCustoCargoDropDownList;
        protected Label centroCustoCargoLblAviso;
        #endregion

        #region Propriedades
        protected override string DefaultTemplateName
        {
            get
            {
                return "CentroCustoCargoFieldControl";
            }
        }

        public override string DisplayTemplateName
        {
            get
            {
                return "CentroCustoCargoFieldControlForDisplay";
            }
            set
            {
                base.DisplayTemplateName = value;
            }
        }

        public override object Value
        {
            get
            {
                EnsureChildControls();

                if (ControlMode == SPControlMode.Display)
                    return this.ItemFieldValue;

                return centroCustoCargoDropDownList.SelectedValue;
            }

            set
            {
                EnsureChildControls();
                centroCustoCargoDropDownList.SelectedValue = (string)this.ItemFieldValue;
                this.PopulaControle("centroCustoCargoDropDownList", centroCustoCargoDropDownList.SelectedValue);
            }
        }
        #endregion

        #region Eventos
        protected override void OnLoad(EventArgs e)
        {
            centroCustoCargoLblAviso = (Label)TemplateContainer.FindControl("centroCustoCargoLblAviso");

            if (centroCustoCargoLblAviso != null)
                centroCustoCargoLblAviso.Visible = false;

            base.OnLoad(e);
        }

        public override void Focus()
        {
            EnsureChildControls();
            centroCustoCargoDropDownList.Focus();
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            if (Field == null) 
                return;

            if (ControlMode == Microsoft.SharePoint.WebControls.SPControlMode.Display)
                return;

            centroCustoCargoDropDownList = (DropDownList)TemplateContainer.FindControl("centroCustoCargoDropDownList");
            centroCustoCargoLblAviso = (Label)TemplateContainer.FindControl("centroCustoCargoLblAviso");

            if (centroCustoCargoDropDownList == null)
                throw new ArgumentException("Lista de Centro de Custo é nula.");

            centroCustoCargoDropDownList.TabIndex = TabIndex;
            centroCustoCargoDropDownList.CssClass = CssClass;
            centroCustoCargoDropDownList.ToolTip = "Centro de Custo";
            centroCustoCargoDropDownList.AutoPostBack = true;

            centroCustoCargoDropDownList.SelectedIndexChanged += new EventHandler(centroCustoCargoDropDownList_SelectedIndexChanged);

            DataTable dtCentrosCusto = new DataTable();
            dtCentrosCusto = ListaCentroCustosUsuario();

            if (ControlMode == SPControlMode.New)
            {
                this.centroCustoCargoDropDownList.Items.Insert(0, new ListItem("Selecione...", "Selecione..."));

                if (dtCentrosCusto != null)
                {
                    foreach (DataRow centroCusto in dtCentrosCusto.Rows)
                    {
                        centroCustoCargoDropDownList.Items.Add(new ListItem(centroCusto["CODSECAO"].ToString() + " - " + centroCusto["DESCRICAO"].ToString(), centroCusto["CODSECAO"].ToString()));
                    }
                }

                if (centroCustoCargoDropDownList.Items.Count <= 1)
                    centroCustoCargoLblAviso.Text = "Não existe um Centro de Custo associado ao seu usuário.<BR>Por favor, contate o administrador.";
                else
                    centroCustoCargoLblAviso.Text = string.Empty;
            }
            else if (ControlMode == SPControlMode.Edit)
            {
                if (dtCentrosCusto != null)
                {
                    int count = 0;
                    int index = 0;

                    foreach (DataRow centroCusto in dtCentrosCusto.Rows)
                    {
                        centroCustoCargoDropDownList.Items.Add(new ListItem(centroCusto["CODSECAO"].ToString() + " - " + centroCusto["DESCRICAO"].ToString(), centroCusto["CODSECAO"].ToString()));

                        if (this.ItemFieldValue != null)
                        {
                            if (this.ItemFieldValue.ToString() == centroCusto["CODSECAO"].ToString())
                                index = count;
                        }

                        count++;
                    }

                    centroCustoCargoDropDownList.SelectedIndex = index;
                }
            }
        }

        /*
        protected override void Render(System.Web.UI.HtmlTextWriter output)
        {
            if (ControlMode == Microsoft.SharePoint.WebControls.SPControlMode.Display)
            {
                try
                {
                    string valor = Convert.ToString(this.Value);

                    if (this.Value != null && !string.IsNullOrEmpty(valor))
                    {
                        Rossi.Entidades.Cidade cidade = Rossi.Negocio.Servicos.Cidade.Obtem(int.Parse(valor));
                        if (cidade != null)
                        {
                            output.Write(cidade.Nome);
                        }
                    }

                    this.ChamarBaseRender = false;
                }
                catch (Exception ex)
                {
                    this.LogaMensagem(string.Format("Render: {0} ({1})", ex.Message, ex.StackTrace));
                }

                this.ChamarBaseRender = false;
            }

            base.Render(output);
        }*/
        #endregion

        #region Métodos
        private DataTable ListaCentroCustosUsuario()
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

        protected void centroCustoCargoDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopulaControle("centroCustoCargoDropDownList", centroCustoCargoDropDownList.SelectedValue);
        }
        #endregion
    }
}
