using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Security;
using System.Security.Permissions;

namespace Globosat.Remuneracao.CustomFields
{
    public class CentroCusto: SPFieldText
    {
        public CentroCusto(SPFieldCollection fields, string fname)
            : base(fields, fname) { }

        public CentroCusto(SPFieldCollection fields, string fname, string dname)
            : base(fields, fname, dname) { }

        public override BaseFieldControl FieldRenderingControl
        {
            [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
            get
            {
                BaseFieldControl fieldControl = new CentroCustoFieldControl();
                fieldControl.FieldName = this.InternalName;
                return fieldControl;
            }
        }
    }
}
