using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Data;
using Globosat.Library.Servicos;
using Microsoft.Office.Server.UserProfiles;
using CIT.Sharepoint.Util;

namespace Globosat.Remuneracao.FiltroBibCargos.WPFiltroBibCargos
{
    public partial class WPFiltroBibCargos : Microsoft.SharePoint.WebPartPages.WebPart, IWebPartRow
    {
        private DropDownList ddlCentroCusto;
        private Button btnSelecionaCentroCusto;
        private DataTable table = new DataTable();
        private UserProfile up;
        private string matricula;
        private string coligada;
        private DataTable dtCentroCusto;
        private string login;

        public string CentroCusto
        {
            get { return (ViewState["CentroCusto"] == null) ? string.Empty : (string)ViewState["CentroCusto"]; }
            set { ViewState["CentroCusto"] = value; }
        }
        
        public WPFiltroBibCargos()
        {
            table.Columns.Add(new DataColumn("CentroCusto", typeof(string)));
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();
            
            #region DropDownList de Centro de Custo
            //DropDownList de centro de custo
            ddlCentroCusto = new DropDownList();
            ddlCentroCusto.Items.Add(new ListItem("Selecione...", ""));

            GetCentroCusto(ddlCentroCusto);
            Controls.Add(ddlCentroCusto);
            ddlCentroCusto.SelectedIndexChanged += new EventHandler(ddlCentroCusto_SelectedIndexChanged);
            ddlCentroCusto.AutoPostBack = true;
            #endregion

            #region Button de Seleção de Centro de Custo
            //Button de Seleção de Centro de custo
            btnSelecionaCentroCusto = new Button();
            btnSelecionaCentroCusto.Text = "Confirmar";
            Controls.Add(btnSelecionaCentroCusto);
            btnSelecionaCentroCusto.Click += new EventHandler(btnSelecionaCentroCusto_Click);
            btnSelecionaCentroCusto.Visible = false;
            #endregion
        }

        void ddlCentroCusto_SelectedIndexChanged(object sender, EventArgs e)
        {
            CentroCusto = ddlCentroCusto.SelectedValue;
        }
        
        public void btnSelecionaCentroCusto_Click(object sender, EventArgs e)
        {
            CentroCusto = ddlCentroCusto.SelectedValue;
        }

        public void GetRowData(RowCallback callback)
        {
            table.Rows.Clear();
            DataRow row = table.NewRow();
            row["CentroCusto"] = CentroCusto;
            table.Rows.Add(row);
            callback(table.DefaultView[0]);
        }

        public PropertyDescriptorCollection Schema
        {
            get
            {
                return TypeDescriptor.GetProperties(table.DefaultView[0]);
            }
        }

        [ConnectionProvider("Row Data")]
        public IWebPartRow GetProviderData()
        {
            return this;
        }

        private void GetCentroCusto(DropDownList ddl)
        {
            try
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        login = web.CurrentUser.LoginName;

                        if (ManipularDados.VerificaLogin(web, login, "Administradores Remuneração"))
                        {
                            dtCentroCusto = ManipularDados.BuscaTodosCentrosCusto();
                        }
                        else
                        {
                            up = ManipularDados.BuscaProfile(login);

                            if (up != null)
                            {
                                matricula = up["Matricula"].Value.ToString();

                                if (up["Coligada"].Value != null)
                                    coligada = up["Coligada"].Value.ToString();
                                else
                                    coligada = "";

                                dtCentroCusto = ManipularDados.BuscaCentroCusto(matricula, coligada);
                            }
                        }
                        if (dtCentroCusto != null)
                        {
                            foreach (DataRow centroCusto in dtCentroCusto.Rows)
                            {
                                ddl.Items.Add(new ListItem(centroCusto["CODSECAO"].ToString() + " - " + centroCusto["DESCRICAO"].ToString(), centroCusto["CODSECAO"].ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao buscar centro de Custo - WPFiltroBibCargos " + e.Message + e.StackTrace, System.Diagnostics.EventLogEntryType.Error, 1, 3);
                throw;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            string htmlCode = string.Empty;
            htmlCode = "<table>";
            htmlCode += "<tr>";
            htmlCode += "<td valign=\"middle\">";
            htmlCode += "<h4>Selecione o Centro de Custo:&nbsp;</h4>";
            htmlCode += "</td><td valign=\"middle\">";
            writer.Write(htmlCode);
            ddlCentroCusto.RenderControl(writer);
            htmlCode = "</td><td valign=\"middle\">&nbsp;";
            writer.Write(htmlCode);
            btnSelecionaCentroCusto.RenderControl(writer);
            htmlCode = "</td></tr>";
            htmlCode += "</table>";
            writer.Write(htmlCode);

            writer.Write("<br>");
        }
    }
}
