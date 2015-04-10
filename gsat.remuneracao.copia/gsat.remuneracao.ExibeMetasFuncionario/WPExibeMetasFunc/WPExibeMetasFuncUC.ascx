<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPExibeMetasFuncUC.ascx.cs" Inherits="Globosat.Remuneracao.ExibeMetasFuncionario.WPExibeMetasFuncionario.WPExibeMetasFuncUC" %>
<table>
    <tr>
        <td>
            <b>Selecione o Centro de Custo:</b>
            <asp:DropDownList ID="ddlCentroCusto" runat="server" />
            &nbsp;&nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <b>Ano:</b>
            <asp:DropDownList ID="ddlAno" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td align="right">
            <asp:ImageButton ID="btnImprimir" runat="server" OnClientClick="javascript:window.print()" ImageUrl="~/_layouts/images/EvolucaoSalarial/print_icon.jpg" />
            <asp:ImageButton ID="btnEnviar" runat="server" OnClick="Onclick_btnEnviar" ImageUrl="~/_layouts/images/EvolucaoSalarial/mail_icon.jpg" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" ID="lbltabela" />
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblErro" runat="server" ForeColor="Red" Text="" />
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblFrase" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblAnexos" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblArquivoMeta" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblLink" runat="server" Text="" />
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblMensagem" runat="server"></asp:Label>
        </td>
    </tr>
</table>
<br />
<br />
<asp:Label ID="lblMessagem" runat="server" Visible="false" Font-Bold="true" Text="No pagamento será acrescentado o Participe e descontado o adiantamento de julho/12 
"></asp:Label>
