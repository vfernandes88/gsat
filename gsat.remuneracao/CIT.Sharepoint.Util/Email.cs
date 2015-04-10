using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;

namespace CIT.Sharepoint.Util
{
    public class Email
    {
        /// <summary>
        /// Envia email
        /// </summary>
        /// <param name="to">Destinatário</param>
        /// <param name="subject">Assunto</param>
        /// <param name="body">Corpo do email</param>
        public static void EnvioEmail(string to, string subject, string body)
        {
            SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;

            using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
            {

                using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                {
                    SPUtility.SendEmail(web, false, false, to, subject, body);
                }

            }           
        }
    }
}
