<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermoAceite.aspx.cs" Inherits="Globosat.Remuneracao.LieAceito.Layouts.Globosat.Remuneracao.LieAceito.TermoAceite" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">


<style type="text/css">
    
.ms-dialog BODY #s4-workspace { 

overflow:hidden !important;
min-width:300px;

}
.ms-dlgOverlay
{
    position:static !important
}


.s4-ca s4-ca-dlgNoRibbon
{
    width:330px;
    overflow:hidden !important;
}    
.Geral
{
    width:320px;
    overflow:hidden !important;
}
    
body { 
overflow:hidden !important;
width: 320px;
display:table !important;
}
ms-bodyareacell
{
    width:330px;
    overflow:hidden !important;
}
.ms-bodyareacell
{
    width:330px;
    overflow:hidden !important;
}
.ms-dlgContent 
{ 
overflow:hidden !important;
} 

</style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<div style="overflow:hidden;width:320px;">
<table border="0">
        <tr>
            <td>
                <asp:Label ID="lblTermo" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
            </br>
            </br>                
            </br>
            </td>
        </tr>
        <tr align="left">
        	<td>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:ImageButton ID="btnSalvar" ImageUrl="/_layouts/Images/Globosat.Remuneracao.LieAceito/Ciente.jpg" runat="server" />
                &nbsp;&nbsp;
                <asp:ImageButton ID="btnCancelar" ImageUrl="/_layouts/Images/Globosat.Remuneracao.LieAceito/Cancelar.jpg" runat="server" OnClientClick="SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.cancel, 'Cancelled');" />
        	</td>
        </tr>
</table>
</div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Termo de Aceite
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Termo de Aceite
</asp:Content>
