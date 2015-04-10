using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using CIT.Sharepoint.Util;

namespace Globosat.Remuneracao.ERBibCargos.EventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class EventReceiver : SPItemEventReceiver
    {
        //TODO: Fazer Code Review otimizando o código
        private string PegaNomeArquivo(string p)
        {
            string nomeArquivo = p.Substring(p.LastIndexOf('/') + 1);
            return nomeArquivo.Substring(0, nomeArquivo.LastIndexOf('.')).Trim();
        }

        public override void ItemUpdating(SPItemEventProperties properties)
        {
            Logger.Write("ENTROU NO ITEM UPDATING", System.Diagnostics.EventLogEntryType.SuccessAudit, 3, 1);
            base.ItemUpdating(properties);
            string centroCusto = string.Empty;
            string nomeArquivo = string.Empty;
            try
            {

                if (((properties.BeforeProperties["vti_sourcecontrolcheckoutby"] != null) && (properties.AfterProperties["vti_sourcecontrolcheckedoutby"] == null)) ||
                    ((properties.AfterProperties["vti_sourcecontrolcheckedoutby"] == null) && (properties.BeforeProperties["vti_sourcecontrolcheckedoutby"] == null)) &&
                    properties.AfterProperties["CentroCusto"] != null)
                {
                    string beforeURL = properties.BeforeUrl;
                    //Instancia a lista em questão
                    SPList list = properties.OpenWeb().Lists[properties.ListId];

                    //Pega somente o nome do arquivo, pois o beforeURL possui a URL completa do arquivo
                    nomeArquivo = PegaNomeArquivo(properties.BeforeUrl);
                    SPField field = list.Fields["Centro de Custo"];
                    //centroCusto = properties.AfterProperties[field.InternalName].ToString();

                    centroCusto = (properties.AfterProperties[field.InternalName] == null ? "" : properties.AfterProperties[field.InternalName].ToString());

                    #region Verifica se arquivo já existe

                    bool jaExiste = false;

                    foreach (SPListItem item in list.Items)
                    {
                        //Verifica se o arquivo possui o mesmo centro de custo
                        if (PegaNomeArquivo(item.File.Name).Equals(nomeArquivo) && item[field.InternalName].ToString() != centroCusto)
                        {
                            jaExiste = true;
                            break;
                        }

                    }
                    #endregion

                    if (jaExiste)
                    {
                        properties.Cancel = true;
                        properties.ErrorMessage = string.Format("<b>Nomenclatura de cargo existente, favor complementar com o nome de sua área/canal ou do ocupante do cargo!</b>");
                        properties.RedirectUrl = "/_Layouts/Globosat.Remuneracao.EventReceiverBibliotecaCargos/ItemExistente.aspx";
                    }
                }
            }
            catch (Exception ex)
            {
                properties.Cancel = true;
                properties.ErrorMessage = string.Format("Ocorreu o seguinte erro: {0}, Trace: {1}", ex.Message, ex.StackTrace);
                properties.RedirectUrl = "/_Layouts/Globosat.Remuneracao.EventReceiverBibliotecaCargos/ItemException.aspx";
            }
        }

        private bool JaExiste(SPItemEventProperties properties)
        {
            string beforeURL = properties.BeforeUrl;

            //Pega somente o nome do arquivo, pois o beforeURL possui a URL completa do arquivo
            string nomeArquivo = PegaNomeArquivo(properties.BeforeUrl);

            #region Verifica se arquivo já existe
            SPList list = properties.OpenWeb().Lists[properties.ListId];
            bool jaExiste = false;
            foreach (SPListItem item in list.Items)
            {
                //Verifica se arquivo já existe na biblioteca
                if (PegaNomeArquivo(item.File.Name).Equals(nomeArquivo))
                {
                    jaExiste = true;
                    break;
                }
            }
            #endregion
            return jaExiste;
        }

        public override void ItemAdding(SPItemEventProperties properties)
        {
            base.ItemAdding(properties);
            try
            {
                Logger.Write("ENTROU NO ITEM ADDING", System.Diagnostics.EventLogEntryType.SuccessAudit, 3, 1);

                if (JaExiste(properties))
                {
                    properties.Status = SPEventReceiverStatus.CancelWithRedirectUrl;
                    properties.Cancel = true;
                    //properties.ErrorMessage = "<b>Nomenclatura de cargo existente, favor complementar com o nome de sua área/canal ou do ocupante do cargo!</b>";
                    properties.RedirectUrl = "/_Layouts/Globosat.Remuneracao.CustomPages/ItemExistente.aspx";
                }
            }
            catch (Exception ex)
            {
                properties.Status = SPEventReceiverStatus.CancelWithRedirectUrl;
                properties.Cancel = true;
                //properties.ErrorMessage = string.Format("Ocorreu o seguinte erro (ItemAdding): {0}, Trace: {1}", ex.Message, ex.StackTrace);
                properties.RedirectUrl = string.Format("/_Layouts/Globosat.Remuneracao.CustomPages/ItemException.aspx?msg={0}", ex.Message);
            }
        }

        /// <summary>
        /// An item is being deleted.
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            base.ItemDeleting(properties);
        }


    }
}
