using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.Office.InfoPath.Server.Controls;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using CIT.Sharepoint.Util;
using System.Diagnostics;
using Globosat.Library;
using Globosat.Library.Servicos;
using System.Globalization;

namespace Globosat.Remuneracao.Forms.Webparts.InfoPathFormHost
{
    [ToolboxItemAttribute(false)]
    public class InfoPathFormHost : WebPart
    {
        // XmlFormView control object to render the browser-enabled InfoPath form.
        private XmlFormView xmlFormView = null;

        // Field to hold the return value from the modal dialog box.
        private TextBox hiddenText = null;
        private string receivedValue = null;

        // Property to store the InfoPath XSN URL.
        [WebBrowsable(true)]
        [WebDisplayName("URL do Formulário XSN")]
        [Description("URL do  arquivo InfoPath XSN.")]
        [Category("Configurações")]
        [Personalizable(PersonalizationScope.Shared)]
        public string FormXSNURL { get; set; }

        // Property to store the Infopath namespac.
        [WebBrowsable(true)]
        [WebDisplayName("Infopath Namespace")]
        [Description("Namespace do arquivo infopath para mapeamento dos campos")]
        [Category("Configurações")]
        [Personalizable(PersonalizationScope.Shared)]
        public string infopathNamespace { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Register scripts to load and display the modal dialog box.
            ScriptLink.Register(this.Page, "Globosat.Remuneracao.CustomPages/scripts/jquery-1.9.1.min.js", false);
            ScriptLink.Register(this.Page, "Globosat.Remuneracao.CustomPages/scripts/BuscaCargo.js", false);
        }

        protected override void CreateChildControls()
        {
            try
            {
                base.CreateChildControls();

                // Instantiate the XMlFormView object.
                this.xmlFormView = new XmlFormView();

                // Add a handler for the NotifyHost event.
                this.xmlFormView.NotifyHost += new EventHandler<NotifyHostEventArgs>(xmlFormView_NotifyHost);

                // Set the editing status to init on initial load.
                this.xmlFormView.EditingStatus = XmlFormView.EditingState.Init;

                // Add to the Web Part controls collection.
                this.Controls.Add(this.xmlFormView);

                this.hiddenText = new TextBox();

                // Hide the text box.
                this.hiddenText.Style.Add("display", "none");
                this.hiddenText.Style.Add("visibility", "hidden");

                // Assign a dummy class to the control so it can be found through jQuery functions.
                this.hiddenText.CssClass = "webparthiddenfield";

                // Add to the Web Part controls collection.
                this.Controls.Add(this.hiddenText);
            }
            catch(Exception ex)
            {
                Logger.Write("Erro ao carregar os controle da página: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                throw;
            }
        }

        // Handles the event that is raised when the button 
        // in the InfoPath form is clicked.
        void xmlFormView_NotifyHost(object sender, NotifyHostEventArgs e)
        {
            try
            {
                string url = SPContext.Current.Web.Url;
                string strNotification = e.Notification;
                // Check if the argument contains the XPath.
                if (!string.IsNullOrEmpty(e.Notification))
                {
                    if (e.Notification.Contains("&"))
                    {
                        string[] notification = e.Notification.Split('&');
                        url += "&" + notification[1];
                        strNotification = notification[0];
                    }
                    // Save the InfoPath field XPath in the view state so it can be used later.
                    //ViewState["fieldXPath"] = e.Notification;
                    ViewState["fieldXPath"] = strNotification;
                    
                    // Construct a JavaScript function to invoke the modal dialog box.
                    StringBuilder functionSyntax = new StringBuilder();
                    functionSyntax.AppendLine("function popupparams() {");

                    // Pass the current SharePoint web URL as an argument.
                    functionSyntax.AppendLine("var url ='" + url + "';");

                    // Call the JavaScript function to pop up the modal dialog box.
                    functionSyntax.AppendLine("popupmodalui(url);}");

                    // Ensure the function popupparams is called after the UI is finished loading.
                    functionSyntax.AppendLine("_spBodyOnLoadFunctionNames.push('popupparams');");

                    // Register the script on the page.
                    Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "ModalHostScript", functionSyntax.ToString(), true);
                }
            }
            catch(Exception ex)
            {
                Logger.Write("Erro ao abrir Popup (NotifyHost): " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                throw;
            }
        }


        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                // Determine whether the user has set the FormXSNURL property with InfoPath XSN location.
                if (!string.IsNullOrEmpty(this.FormXSNURL))
                {
                    // Configure the XmlFormView control to host the form.
                    this.xmlFormView.XsnLocation = this.FormXSNURL;

                    // Set the editing status to ensure the control displays the form.
                    this.xmlFormView.EditingStatus = XmlFormView.EditingState.Editing;

                    // Retrieve the return value from the modal dialog box.
                    this.receivedValue = this.hiddenText.Text;

                    // Determine whether the received value is not empty.
                    if (!string.IsNullOrEmpty(this.receivedValue))
                    {
                        // Update the form data source with the new value.
                        this.UpdateFormMainDataSource();
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Write("Erro ao carregar o Formulário: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                
            }
        }

        // Updates the form's datasource with the received value.
        private void UpdateFormMainDataSource()
        {
            if (ViewState["fieldXPath"] != null && ViewState["fieldXPath"].ToString() != string.Empty && !string.IsNullOrEmpty(this.receivedValue))
            {
                // Ensure the XMlFormView has access to the form's underlying data source.
                this.xmlFormView.DataBind();

                #region Campos utilizados pelo Formulário de Alteração Funcional
                string salarioAtual = string.Empty;
                string campoDiferenca = "/my:myFields/my:tb_Diferenca";
                string campoPorcentagemAumento = "/my:myFields/my:tb_PercentualAumento"; 
                string campoSalarioProposto = "tb_SalarioProposto";
                string salarioProposto = string.Empty;
                #endregion

                if (this.FormXSNURL.Contains("Funcional")) // Trata-se do Formulário de Alteração Funcional
                {
                   salarioAtual = ViewState["fieldXPath"].ToString().Substring(ViewState["fieldXPath"].ToString().LastIndexOf('#')+1);
                }

                string[] fieldForm = ViewState["fieldXPath"].ToString().Split('#');
                string[] received = this.receivedValue.Split(';');

                for (int i = 0; i < fieldForm.Length; i++)
                {
                    if (fieldForm[i].ToString().Contains(campoSalarioProposto))
                    {
                        salarioProposto = received[i];
                    }
                
                    // Pass the target InfoPath field XPath and the received values.
                    this.SetFormFieldValue(fieldForm[i], received[i], received[i]);

                    if (this.FormXSNURL.Contains("Funcional") && (i == (fieldForm.Length - 2))) // Trata-se do Formulário de Alteração Funcional. Portanto é hora de sair do loop.
                        i++;
                }
                
                if (this.FormXSNURL.Contains("Funcional")) // Trata-se do Formulário de Alteração Funcional
                {
                    decimal percentualAumentoSalarial = CalcularPercentualAumento(salarioProposto, salarioAtual);
                    decimal diferencaSalarial = CalcularDiferencaSalarial(salarioProposto, salarioAtual);

                    this.SetFormFieldValue(campoDiferenca, diferencaSalarial.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR")), diferencaSalarial.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR")));
                    this.SetFormFieldValue(campoPorcentagemAumento, percentualAumentoSalarial.ToString("00") + " %", percentualAumentoSalarial.ToString("00") + " %");
                }
            }
        }

        private decimal CalcularDiferencaSalarial(string salarioProposto, string salarioAtual)
        {
            return ManipularDados.CalcularDiferencaSalario(salarioProposto, salarioAtual);
        }

        private decimal CalcularPercentualAumento(string salarioProposto, string salarioAtual)
        {
            return ManipularDados.CalcularPercentualDiferencaSalario(salarioProposto, salarioAtual);
        }
        // Sets the target InfoPath form field value with the received value.
        private void SetFormFieldValue(string xpath, string url, string value)
        {
            // Create an XPathNavigator positioned at the root of the form's main data source.
            XPathNavigator xNavMain = this.xmlFormView.XmlForm.MainDataSource.CreateNavigator();

            // Create an XmlNamespaceManager.
            XmlNamespaceManager xNameSpace = new XmlNamespaceManager(new NameTable());

            // Add the "my" namespace alias from the form's main data source.
            // Note: Replace the second argument with the correct namespace 
            // from the form template that you are using.


            if(!string.IsNullOrEmpty(this.infopathNamespace))
                xNameSpace.AddNamespace("my", this.infopathNamespace);
            else
                xNameSpace.AddNamespace("my", "http://schemas.microsoft.com/office/infopath/2003/myXSD/2013-01-05T12:48:00");

            // Create an XPathNavigator positioned on the target form field.
            XPathNavigator formfield = xNavMain.SelectSingleNode(xpath, xNameSpace);

            // Set the form's hyperlink field to the received document URL.
            formfield.SetValue(url);
            if (formfield.HasAttributes)
            {
                // Set the hyperlink's display text with document title.
                formfield.MoveToFirstAttribute();
                formfield.SetValue(value);
            }
        }
    }
}
