<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPPremiosUserControl.ascx.cs" Inherits="Cit.Globosat.Premios.WPPremios.WPPremiosUserControl" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<script type="text/javascript" src="/_layouts/Cit.Globosat.Base/Scripts/jquery-1.10.1.min.js"></script>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $('#<%= dropDownListCentroCusto.ClientID %>').change(function () {
            var waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('Aguarde!', 'Processando...', 76, 330);
            return true;
        });

        $('#<%= dropDownListFuncionarios.ClientID %>').change(function () {
            
            if ($(this).val() === '0') {
                var waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('Aguarde!', 'Processando...', 76, 330);
            } else {
                var waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('Aguarde!', 'O relatório está sendo gerado.<br /> Isto poderá levar alguns instantes...', 70, 310);
            }

            return true;
        });

        $('#<%= dropDownListEventos.ClientID %>').change(function () {

            var waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('Aguarde!', 'O relatório está sendo gerado.<br /> Isto poderá levar alguns instantes...', 70, 310);
            return true;
        });
    });
</script>
<br />
<table align="left">
    <tr>        
        <td>
            <asp:Label runat="server" ID="labelCentroCusto" Text="Centro de Custo:" Font-Bold="true"></asp:Label>
            <br />
            <asp:DropDownList runat="server" ID="dropDownListCentroCusto" Width="300px" AutoPostBack="true" 
               ToolTip="Escolha o centro de custo." OnSelectedIndexChanged="dropDownListCentroCusto_SelectedIndexChanged" />
        </td>
        <td>
            <asp:Label runat="server" ID="labelFuncionarios" Text="Funcionários:" Font-Bold="true"></asp:Label>
            <br />
            <asp:DropDownList runat="server" ID="dropDownListFuncionarios" Width="300px" AutoPostBack="true" 
                ToolTip="Escolha o funcionário." OnSelectedIndexChanged="dropDownListFuncionarios_SelectedIndexChanged" />
        </td>
        <td runat="server" id="tdGraficoEvento" visible="false">
            <asp:Label runat="server" ID="labelGraficoEvento" Text="Gráfico de Eventos:" Font-Bold="true"></asp:Label>
            <br />
            <asp:DropDownList runat="server" ID="dropDownListEventos" Width="300px" AutoPostBack="true" 
                ToolTip="Escolha o evento." OnSelectedIndexChanged="dropDownListEventos_SelectedIndexChanged" />
        </td>
    </tr>
    <tr>
        <td align="right" colspan="3">
            <asp:ImageButton ID="btnImprimir" runat="server" OnClientClick="javascript:window.print()" ImageUrl="~/_layouts/images/EvolucaoSalarial/print_icon.jpg" />
        </td>
    </tr>
</table>
<table align="left" width="100%">
    <tr>
        <td>
            <rsweb:ReportViewer ID="reportViewerPremios" runat="server" AsyncRendering="False" SizeToReportContent="False" ZoomMode="FullPage" ShowZoomControl="false" 
                OnReportRefresh="reportViewerPremios_ReportRefresh" Height="850px" Width="1050px" InteractivityPostBackMode="SynchronousOnDrillthrough">
            </rsweb:ReportViewer>
        </td>
    </tr>
    <tr>
		<td>
			<asp:Label runat="server" ID="labelMessage" Text="É preciso definir o caminho do report nas propriedades da webpart." Font-Bold="true" Visible="false"></asp:Label>
		</td>
	</tr>
</table>