using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Globalization;

namespace Globosat.Remuneracao.TabelaSalarialLista.WPTabelaSalarialLista
{
    public partial class WPTabelaSalarialListaUC : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string StrRow = string.Empty;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int Contador = 1;
            int conttds = 10;
            int ContTr = 1;

            string StrCabecalho = " <tr class='Cabecalho'> " +
                                  "     <td rowspan='3'>                     " +
                                  "        <span> Classe Salarial  </span>   " +
                                  "     </td>                                " +
                                  "     <td>                                 " +
                                  "         <span>  </span>               " +
                                  "     </td>                                " +
                                  "     <td>                                 " +
                                  "         <span>  </span>               " +
                                  "     </td>                                " +
                                  "     <td>                                 " +
                                  "         <span>  </span>               " +
                                  "     </td>                                " +
                                  "     <td>                                 " +
                                  "         <span>  </span>               " +
                                  "     </td> </tr>                               " +
                                  "   <tr><td>                                 " +
                                  "         <span>  </span>               " +
                                  "     </td>                                " +
                                  "     <td>                                 " +
                                  "         <span>  </span>               " +
                                  "     </td>                                " +
                                  "     <td>                                 " +
                                  "         <span>  </span>               " +
                                  "     </td>                                " +
                                  "     <td>                                 " +
                                  "         <span>  </span>               " +
                                  "     </td>                                " +
                                  "     <td>                                 " +
                                  "         <span>  </span>               " +
                                  "     </td>                                " +
                                  " </tr>                                    ";

            string StrCabecalhoPerc = " <tr class='CabecalhoPercentual'>     " +
                                       "     <td>                            " +
                                       "         <span> Participe </span>          " +
                                       "     </td>                           " +
                                       "     <td>                            " +
                                       "         <span> Participe Variavel </span>          " +
                                       "     </td>                           " +
                                       "     <td>                            " +
                                       "         <span> Bônus </span>          " +
                                       "     </td>                           " +
                                       "     <td>                            " +
                                       "         <span> Total </span>          " +
                                       "     </td>                           " +
                                       " </tr>                               ";

            string StrCabecalhoClassi = " <tr class='CabecalhoClassificacao'>  " +
                                      "     <td colspan='4'>                " +
                                      "         <span> Min. </span>         " +
                                      "     </td>                           " +
                                      "     <td colspan='4'>                " +
                                      "         <span> Mediana </span>      " +
                                      "     </td>                           " +
                                      "     <td>                            " +
                                      "         <span> Max. </span>          " +
                                      "     </td>                           " +
                                      " </tr>                               ";
           
            using (SPSite site = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists["TabelaSalarial"]; //List which has lookupfield TechSkills
                    SPListItemCollection itemCollection;
                    itemCollection = list.Items;

                    sb.Append("<tr>");
                    foreach (SPListItem item in itemCollection)
                    {   
                        string Valor = Convert.ToDecimal(item["Valor"]).ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));

                        if (Contador == 1)
                        {   
                            if (ContTr % 2 == 0)
                                sb.Append("<tr class='RowPar'>");
                            else
                                sb.Append("<tr class='RowImpar'>");

                            sb.Append(RetornaRow(item["Classe"].ToString()));                            
                        }
                            
                        sb.Append(RetornaRow(Valor));
                        
                        if (Contador == 9)
                        { 
                            sb.Append("</tr>");
                            ContTr++;
                            Contador = 0;
                            conttds = 10;
                        }

                        Contador++;
                        conttds--;
                    }
                }
            }

            lblMain.Text = "<Table class='TabelaSalarial'>" + StrCabecalho + StrCabecalhoPerc + StrCabecalhoClassi + TratarColunasVazias(sb.ToString(), conttds) + "</Table>";
        }

        private string TratarColunasVazias(string StrTabela, int conttds)
        {
            for (int i = 0; i < conttds; i++)
                StrTabela += RetornaRow(string.Empty);

            StrTabela += "</tr>";
            return StrTabela;
        }

        private string RetornaRow(string Valor)
        {
            string StrTR = string.Empty;

            if (string.IsNullOrEmpty(Valor))
            {
                StrTR = "<td></td>";
            }
            else
            {
                StrTR = "<td>" + Valor + "</td>";
            }

            return StrTR;
        }

    }
}
