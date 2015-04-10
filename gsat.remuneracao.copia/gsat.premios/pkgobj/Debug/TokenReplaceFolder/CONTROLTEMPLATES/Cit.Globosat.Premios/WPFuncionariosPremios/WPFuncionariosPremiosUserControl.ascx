<%@ Assembly Name="Cit.Globosat.Premios, Version=1.0.0.0, Culture=neutral, PublicKeyToken=49c2cd8d8f741a50" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPFuncionariosPremiosUserControl.ascx.cs"
    Inherits="Cit.Globosat.Premios.WPFuncionariosPremios.WPFuncionariosPremiosUserControl" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<script type="text/javascript" src="/_layouts/Cit.Globosat.Base/Scripts/jquery-1.10.1.min.js"></script>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $('#<%= dropDownListCentroCusto.ClientID %>').change(function () {
            var waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('Aguarde!', 'Processando...', 76, 330);
            return true;
        });

        $('#<%= dropDownListEventos.ClientID %>').change(function () {
            var waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('Aguarde!', 'Processando...', 76, 330);
            return true;
        });

        $('#<%= buttonGerarRelatorio.ClientID %>').click(function () {
            var waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('Aguarde!', 'O relatório está sendo gerado.<br /> Isto poderá levar alguns instantes...', 70, 310);
            return true;
        });

        // Botão selecionar todas
        $('#<% =buttonSelecionar.ClientID %>').click(function () {
            $('[id*=checkBoxID]').prop('checked', true);
            $('[id*=checkBoxID]').closest('tr').find('td').css('background-color', '#ccc');
            return false;
        });

        // Botão desmarcar todas
        $('#<% =buttonDesmarcar.ClientID %>').click(function () {
            $('[id*=checkBoxID]').prop('checked', false);
            $('[id*=checkBoxID]').closest('tr').find('td').css('background-color', '#F5F5F5');
            return false;
        });

        $('[id*=checkBoxID]').click(function () {
            if ($(this).prop('checked')) {
                $(this).closest('tr').find('td').css('background-color', '#ccc');
            } else {
                $(this).closest('tr').find('td').css('background-color', '#F5F5F5');
            }
        });

    });
</script>
<style type="text/css">
    .GridView
    {
        border: 1px solid #e0e0e0;
    }
    
    .GridViewHeader
    {
        background-image: url(/_layouts/images/bgximg.png);
        background-repeat: repeat-x;
        background-position: left;
        height: 25px;
        border: 1px solid #e0e0e0;
    }
    
    .GridViewHeader th
    {
        border: 1px solid #e0e0e0;
    }
    
    .GridViewFooter
    {
    }
    
    .GridViewRow
    {
    }
    
    .GridViewRow td
    {
        border-bottom: 1px solid #DFDFDF !important;
        border-top: 1px solid white !important;
        border: 1px solid #e0e0e0;
        background-color: #ccc;
    }
    
    .GridViewRow th
    {
        border: 1px solid #e0e0e0;
    }
    
    .GridViewAlternating
    {
        border-bottom: 1px solid #DFDFDF !important;
        border-top: 1px solid white !important;
    }
    
    .GridViewAlternating td
    {
        border-bottom: 1px solid #DFDFDF !important;
        border-top: 1px solid white !important;
        background-color: #ccc;
        border: 1px solid #e0e0e0;
    }
    
    .GridViewRowSelected
    {
        font-size: 1em;
        font-family: Arial,Verdana;
        background-color: #000000 !important;
        color: white;
    }
    
    .GridViewPager td
    {
        background: transparent !important;
        background-color: transparent !important;
        border: none !important;
    }
    
    .GridViewPager td table
    {
        margin-left: auto;
        margin-right: auto;
    }
</style>
<table align="left">
    <tr>
        <td>
            <asp:Label runat="server" ID="labelCentroCusto" Text="Centro de Custo:" Font-Bold="true"></asp:Label>
            <br />
            <asp:DropDownList runat="server" ID="dropDownListCentroCusto" Width="300px" AutoPostBack="true" ToolTip="Escolha o centro de custo."
                OnSelectedIndexChanged="dropDownListCentroCusto_SelectedIndexChanged" />
        </td>
        <td runat="server">
            <asp:Label runat="server" ID="labelGraficoEvento" Text="Escolha o Evento:" Font-Bold="true" Visible="false"></asp:Label>
            <br />
            <asp:DropDownList runat="server" ID="dropDownListEventos" Width="300px" ToolTip="Escolha o evento." AutoPostBack="true"
                Visible="false" OnSelectedIndexChanged="dropDownListEventos_SelectedIndexChanged" />
        </td>
    </tr>
    <tr>
        <td align="right" colspan="3">
            <asp:ImageButton ID="btnImprimir" runat="server" OnClientClick="javascript:window.print()" ImageUrl="~/_layouts/images/EvolucaoSalarial/print_icon.jpg" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Label runat="server" ID="labelFuncionarios" Text="Funcionários:" Font-Bold="true" Visible="false"></asp:Label>
            <br />
            <asp:GridView runat="server" ID="gridViewFuncionarios" AutoGenerateColumns="false" Width="600px" Visible="false"
                PageSize="10" AllowPaging="false" OnRowDataBound="gridViewFuncionarios_RowDataBound" OnPageIndexChanging="gridViewFuncionarios_PageIndexChanging">
                <FooterStyle CssClass="GridViewFooter" />
                <PagerStyle CssClass="GridViewPager" />
                <HeaderStyle CssClass="GridViewHeader" />
                <RowStyle CssClass="GridViewRow" HorizontalAlign="Center" VerticalAlign="Middle" />
                <SelectedRowStyle CssClass="GridViewRowSelected" />
                <AlternatingRowStyle CssClass="GridViewAlternating" />
                <EmptyDataRowStyle HorizontalAlign="Center" />
                <EmptyDataTemplate>
                    <asp:Label runat="server" ID="labelGridViewNoData" Text="Não existem funcionários! Escolha outro centro de custo." />
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField>
                        <HeaderStyle Width="1%" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="checkBoxID" AutoPostBack="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CHAPA" HeaderText="MATRÍCULA">
                        <HeaderStyle Width="10%" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="NOME" HeaderText="NOME">
                        <HeaderStyle Width="89%" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="left">
            <table>
                <tr>
                    <td>
                        <asp:Button runat="server" ID="buttonSelecionar" Text="Selecionar Todos" Width="130px" Visible="false" />
                    </td>
                    <td>
                        <asp:Button runat="server" ID="buttonDesmarcar" Text="Desmarcar Todos" Width="130px" Visible="false" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="right">
            <asp:Button runat="server" ID="buttonGerarRelatorio" OnClick="buttonGerarRelatorio_Click" Text="Gerar Relatório"
                Enabled="false" Width="130px" />
        </td>
    </tr>
</table>
<table align="left" width="100%">
    <tr>
        <td>
            <rsweb:ReportViewer ID="reportViewerFuncPremios" runat="server" AsyncRendering="False" SizeToReportContent="True"
                ZoomMode="FullPage" ShowZoomControl="false" OnReportRefresh="reportViewerPremios_ReportRefresh" Width="600px">
            </rsweb:ReportViewer>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" ID="labelMessage" Text="É preciso definir o caminho do report nas propriedades da webpart."
                Font-Bold="true" Visible="false"></asp:Label>
        </td>
    </tr>
</table>
<%--<asp:GridView runat="server" ID="gridviewPremios" AutoGenerateColumns="true">
</asp:GridView>--%>
