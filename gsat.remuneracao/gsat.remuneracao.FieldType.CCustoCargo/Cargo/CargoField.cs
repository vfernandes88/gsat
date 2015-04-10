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
    [Guid("042D9C56-8111-459d-B26E-ADE37C49DC31")]
    public class CargoField : SPFieldText
    {
        public CargoField(SPFieldCollection fields, string fieldName) : base(fields, fieldName)
        {
        }

        public CargoField(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName)
        {
        }

        public override BaseFieldControl FieldRenderingControl
        {
            [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
            get
            {
                BaseFieldControl fieldControl = new CargoFieldControl();
                fieldControl.FieldName = this.InternalName;

                return fieldControl;
            }
        }

        public override object GetFieldValue(string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;
            return value;
        }

        public override void OnAdded(SPAddFieldOptions op)
        {
            base.OnAdded(op);
            Update();
        }

        public override string GetValidatedString(object value)
        {
            if (this.Required == true && string.IsNullOrEmpty(Convert.ToString(value)))
            {
                throw new SPFieldValidationException(this.Title + " é obrigatório.");
            }
             
            else
            {
                if(Convert.ToString(value).Equals("Selecione...", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SPFieldValidationException(this.Title + " é obrigatório.");
                }
                return base.GetValidatedString(value);
            }
        }
    }
}
