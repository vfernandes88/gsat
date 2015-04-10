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
    [Guid("EC3BA40C-EEDA-41e3-8C76-B83B608BEA9F")]
    public class CargoFieldControl : LinkedFieldRenderingControl
    {
        #region Atributos
        protected DropDownList cargoDropDownList;
        protected Label cargoLblAviso;
        #endregion

        #region Propriedades
        protected override string DefaultTemplateName
        {
            get
            {
                return "CargoFieldControl";
            }
        }

        public override string DisplayTemplateName
        {
            get
            {
                return "CargoFieldControlForDisplay";
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

                return cargoDropDownList.SelectedValue;
            }

            set
            {
                EnsureChildControls();
                cargoDropDownList.SelectedValue = (string)this.ItemFieldValue;
                this.PopulaControle("cargoDropDownList", cargoDropDownList.SelectedValue);
            }
        }
        #endregion

        #region Eventos
        protected override void OnLoad(EventArgs e)
        {
            cargoLblAviso = (Label)TemplateContainer.FindControl("cargoLblAviso");

            if (cargoLblAviso != null)
                cargoLblAviso.Visible = false;

            base.OnLoad(e);
        }

        protected override void CreateChildControls()
        {
            if (Field == null) return;
            base.CreateChildControls();

            if (ControlMode == Microsoft.SharePoint.WebControls.SPControlMode.Display)
                return;

            cargoDropDownList = (DropDownList)TemplateContainer.FindControl("cargoDropDownList");

            if (cargoDropDownList == null)
                throw new ArgumentException("Lista de cargos é nula.");

            cargoDropDownList.TabIndex = TabIndex;
            cargoDropDownList.CssClass = CssClass;
            cargoDropDownList.ToolTip = "Cargos de um Centro de Custo";
            cargoDropDownList.SelectedIndexChanged += new EventHandler(cargoDropDownList_SelectedIndexChanged);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter output)
        {
            if (ControlMode == Microsoft.SharePoint.WebControls.SPControlMode.Display)
            {
                try
                {
                    string valor = ParseString(this.Value);

                    if (this.Value != null && !string.IsNullOrEmpty(valor))
                    {
                        SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;

                        SPSecurity.RunWithElevatedPrivileges(delegate
                        {
                            using (SPSite site = new SPSite(SPContext.Current.Site.ID, sysToken))
                            {
                                using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                                {
                                    SPList spList = web.Lists["Biblioteca Cargos"];

                                    if (spList.Fields.ContainsField("CentroCusto"))
                                    {
                                        SPField spField = spList.Fields.GetFieldByInternalName("CentroCusto");

                                        string codCargo = ParseString(this.Value);
                                        string codCentroCusto = string.Empty;

                                        if (Item["CentroCusto"] != null)
                                        {
                                            codCentroCusto = Item["CentroCusto"].ToString();

                                            DataTable dtCargo = ManipularDados.BuscaCargo(codCentroCusto, codCargo);

                                            if (dtCargo != null)
                                            {
                                                foreach (DataRow cargo in dtCargo.Rows)
                                                {
                                                    if (codCentroCusto == cargo["CODCENTROCUSTO"].ToString() && codCargo == cargo["CODFUNCAO"].ToString())
                                                    {
                                                        output.Write(cargo["NOME"].ToString());
                                                    }
                                                }
                                            }
                                        }
                                        //SPListItem item = spList.GetItemById((int)this.Value);
                                    }
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(string.Format("Render: {0} ({1})", ex.Message, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error, 2, 2);
                }
            }

            base.Render(output);
        }
        #endregion

        #region Métodos
        public override void SetDataSource(string parentSelectedValue)
        {
            try
            {
                this.cargoDropDownList.Items.Clear();
                
                DataTable dtCargo = ManipularDados.BuscaCargo(parentSelectedValue);
                if (dtCargo != null)
                {
                    this.cargoDropDownList.Items.Insert(0, new ListItem("Selecione...", "Selecione..."));
                    this.cargoDropDownList.Items.Insert(1, new ListItem("A SER PREENCHIDO PELO RH", "A SER PREENCHIDO PELO RH"));

                    foreach (DataRow cargo in dtCargo.Rows)
                    {
                        cargoDropDownList.Items.Add(new ListItem(cargo["NOME"].ToString(), cargo["CODFUNCAO"].ToString()));
                    }
                }

                if (cargoDropDownList.Items.Count <= 1)
                    cargoLblAviso.Text = "Não existe um Cargo associado a este centro de custo.";
                else
                    cargoLblAviso.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Erro ao preencher campo Cargo no SetDataSource: {0} ({1})", ex.Message, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error, 2, 2);
            }
        }

        protected void cargoDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopulaControle("cargoDropDownList", cargoDropDownList.SelectedValue);
        }

        public override void Focus()
        {
            EnsureChildControls();
            cargoDropDownList.Focus();
        }

        public static string ParseString(object valor)
        {
            if (valor != null)
                return valor.ToString();
            return null;
        }
        #endregion
    }
}
