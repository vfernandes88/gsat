<%@ Assembly Name="MelhoriaMetas, Version=1.0.0.0, Culture=neutral, PublicKeyToken=a863ef5b32b85a5b" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPExibeMetasUserControl.ascx.cs" Inherits="MelhoriaMetas.WPExibeMetas.WPExibeMetasUserControl" %>
<table>

<tr><td><b>Selecione o Centro de Custo:</b> <asp:DropDownList ID="ddlCentroCusto" runat="server" />&nbsp;&nbsp; 
    </td></tr>

        <tr><td>&nbsp;</td></tr>
        <tr>
        <td align="right">
            <asp:ImageButton ID="btnImprimir" runat="server" OnClientClick="javascript:window.print()"
                    ImageUrl="~/_layouts/images/EvolucaoSalarial/print_icon.jpg" />
       </td>
    </tr>
<tr><td><asp:Label runat="server" ID="lbltabela" /></td></tr>


 <tr><td>&nbsp;</td></tr>

    <tr>
    <td>
    <asp:Label ID="lblErro" runat="server" ForeColor="Red" text="" /></td>
    </tr>
     <tr><td>&nbsp;</td></tr>
    <tr>
        <td>
            <asp:Label ID="lblLink" runat="server" Text="" />
        </td>
    </tr>
</table>
