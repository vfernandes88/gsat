using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.WebControls;
using CIT.Sharepoint.Util;

namespace Globosat.Remuneracao.FieldType.CCustoCargo
{
    public abstract class LinkedFieldRenderingControl : BaseFieldControl
    {
        public string CamposAcionados
        {
            get { return Field.GetCustomProperty("CamposAcionados") == null ? string.Empty : (string)Field.GetCustomProperty("CamposAcionados"); }
        }

        public virtual void SetDataSource(string parentSelectedValue)
        {
            return;
        }

        public void PopulaControle(string controleAcionador, string valor)
        {
            try
            {
                string[] infoControles = this.CamposAcionados.Split(';');

                foreach (string controle in infoControles)
                {
                    LinkedFieldRenderingControl child = (LinkedFieldRenderingControl)Controles.EncontraControleRecursivo(this.Page, controle).Parent.Parent;
                    child.SetDataSource(valor);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("PopulaControle ({0}_SelectedIndexChanged): {1} ({2})", controleAcionador, ex.Message, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error, 2, 2);                
            }
        }
    }
}
