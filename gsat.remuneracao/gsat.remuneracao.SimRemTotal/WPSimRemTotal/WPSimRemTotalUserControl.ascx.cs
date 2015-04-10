using System;
using Microsoft.SharePoint.WebControls;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections.Generic;
using CIT.Sharepoint.Util;
using System.Diagnostics;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using Globosat.Library.Servicos;
using Globosat.Library.Entidades;


namespace Globosat.Remuneracao.SimRemTotal.WPSimRemTotal
{
    public partial class WPSimRemTotalUserControl : UserControl
    {
        private void Page_Load(object sender, EventArgs e)
        {
            string strEbitda = "100";

            ManipularDados manipularDados = new ManipularDados();
            if (manipularDados.GetFaixaSalarial(SPContext.Current.Web.CurrentUser.LoginName) > 16)
            {
                rblEbitda.Visible = true;
            }
            else
            {
                rblEbitda.Visible = false;
            }

            if(rblEbitda.Visible)
                strEbitda = rblEbitda.SelectedValue;
            
            

            Gerente dadosUsuarioLogado = ManipularDados.BuscaMatriculaColigada(SPContext.Current.Web.CurrentUser.LoginName);

            Simulador sim = new Simulador();
            sim = manipularDados.BuscaSimulador(dadosUsuarioLogado.Matricula, dadosUsuarioLogado.Coligada, strEbitda);

            lblTabelaSimulador.Text += manipularDados.CriaTabelaSimulador(dadosUsuarioLogado.Matricula, dadosUsuarioLogado.Coligada, sim.RemuneracaoVariavel, sim.PlanoSaude, sim.PlanoOdontologico);
            
            criaGrafico(sim);
        }
        public void criaGrafico(Simulador sim)
        {
            //decimal percentual = 0;
            try
            {
                List<EixosGrafico> eixos = new List<EixosGrafico>();
                EixosGrafico eixosGrafico1 = new EixosGrafico();
                eixosGrafico1.Y = sim.RemuneracaoFixa;
                eixosGrafico1.X = "Rem. Fixa";
                eixos.Add(eixosGrafico1);
                EixosGrafico eixo2 = new EixosGrafico();
                eixo2.Y = sim.RemuneracaoDireta;
                eixo2.X = "Rem. Direta";
                eixos.Add(eixo2);
                EixosGrafico eixo3 = new EixosGrafico();
                eixo3.Y = sim.RemuneracaoTotal;
                eixo3.X = "Rem. Total";
                eixos.Add(eixo3);
                
                ChartSimulador.DataSource = eixos;
                ChartSimulador.DataBind();

                ChartSimulador.ChartAreas[0].AxisX.LabelStyle.Angle = 30;
                ChartSimulador.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 8);
                ChartSimulador.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 9);
                ChartSimulador.AntiAliasing = AntiAliasingStyles.All;

                ChartSimulador.ChartAreas[0].AxisY.LabelStyle.Format = "{0.##}";

                if (eixos.Count > 1)
                    ChartSimulador.ChartAreas[0].AxisX.LabelStyle.Interval = 1;


                for (int i = 0; i <= eixos.Count - 1; i++)
                {
                    ChartSimulador.Series[0].Points[i].Label = string.Format("R$ {0}", eixos[i].Y.ToString("N2"));
                }

                ChartSimulador.Series[0].Points[1].Color = ColorTranslator.FromHtml("#FFCC00");
                ChartSimulador.Series[0].Points[2].Color = ColorTranslator.FromHtml("#339966");
              
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao criar gráfico do simulador: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
 
            }
        }

       
        
    }
}
