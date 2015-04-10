<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPTabelaRemuneracaoVariavelUserControl.ascx.cs" Inherits="WPTabelaRemuneracaoVariavel.WPTabelaRemuneracaoVariavel.WPTabelaRemuneracaoVariavelUserControl" %>
<b>Selecione o ano:</b> <asp:DropDownList ID="ddlAno" runat="server" /><br><br>
    <div align="right">
            <asp:ImageButton ID="btnImprimir" runat="server" OnClientClick="javascript:window.print()" ImageUrl="~/_layouts/images/EvolucaoSalarial/print_icon.jpg" />
            <asp:ImageButton ID="btnEnviar" runat="server" OnClick="Onclick_btnEnviar" ImageUrl="~/_layouts/images/EvolucaoSalarial/mail_icon.jpg" />
    </div>
<asp:Label runat="server" ID="lblMain" />
<asp:Label runat="server" ID="lblTeste" />
<%--<table width="100%">
    <tr align="center">
        <td width="10%" >
            Classe Salarial
        </td>
        <td width="90%" >
            Nº SALÁRIOS</td>
    </tr>
</table>
<table width="100%">
    <tr align="center">
        <td width="10%">            
        </td>
        <td width="22,5%">
            PARTICIPE</td>
        <td width="22,5%">
            PARTICIPE VARIÁVEL</td>
        <td width="22,5%">
            BÔNUS</td>
        <td width="22,5%">
            TOTAL</td>
    </tr>
</table>--%>
