using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Security;

namespace Globosat.Remuneracao.FieldType.CCustoCargo
{
    [CLSCompliant(false)]
    [Guid("259CC70E-C02A-4c25-ACB6-CAB8AB5880A4")]
    public class CentroCustoCargoField : SPFieldText
    {
        public CentroCustoCargoField(SPFieldCollection fields, string fieldName) : base(fields, fieldName)
        {
        }

        public CentroCustoCargoField(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName)
        {
        }

        public override BaseFieldControl FieldRenderingControl
        {
            [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
            get
            {
                BaseFieldControl fieldControl = new CentroCustoCargoFieldControl();
                fieldControl.FieldName = this.InternalName;

                return fieldControl;
            }
        }

        public override string GetValidatedString(object value)
        {
            if (this.Required == true && string.IsNullOrEmpty(Convert.ToString(value)))
            {
                throw new SPFieldValidationException(this.Title
                    + " é obrigatório.");
            }
            else
            {
                return base.GetValidatedString(value);
            }
        }
    }
}
