<%@ Assembly Name="Globosat.Remuneracao.CustomPages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=59713f733f7cc09b" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AlteraSalarioProposto.aspx.cs" Inherits="Globosat.Remuneracao.CustomPages.Layouts.Globosat.Remuneracao.CustomPages.AlteraSalarioProposto"
    DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script src="/_layouts/Globosat.Remuneracao.CustomPages/scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="/_layouts/Globosat.Remuneracao.CustomPages/scripts/AlteraSalario.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        function MascaraMoeda(objTextBox, SeparadorMilesimo, SeparadorDecimal, e) {
            var sep = 0;
            var key = '';
            var i = j = 0;
            var len = len2 = 0;
            var strCheck = '0123456789';
            var aux = aux2 = '';

            if (navigator.appName == 'Microsoft Internet Explorer') {
                var whichCode = e.keyCode;
            } else if (navigator.appName == 'Netscape') {
                var whichCode = e.which;
            }


            key = String.fromCharCode(whichCode); // Valor para o código da Chave
            if (strCheck.indexOf(key) == -1) return false; // Chave inválida
            len = objTextBox.value.length;
            for (i = 0; i < len; i++)
                if ((objTextBox.value.charAt(i) != '0') && (objTextBox.value.charAt(i) != SeparadorDecimal)) break;
            aux = '';
            for (; i < len; i++)
                if (strCheck.indexOf(objTextBox.value.charAt(i)) != -1) aux += objTextBox.value.charAt(i);
            aux += key;
            len = aux.length;
            if (len == 0) objTextBox.value = '';
            if (len == 1) objTextBox.value = '0' + SeparadorDecimal + '0' + aux;
            if (len == 2) objTextBox.value = '0' + SeparadorDecimal + aux;
            if (len > 2) {
                aux2 = '';
                for (j = 0, i = len - 3; i >= 0; i--) {
                    if (j == 3) {
                        aux2 += SeparadorMilesimo;
                        j = 0;
                    }
                    aux2 += aux.charAt(i);
                    j++;
                }
                objTextBox.value = '';
                len2 = aux2.length;
                for (i = len2 - 1; i >= 0; i--)
                    objTextBox.value += aux2.charAt(i);
                objTextBox.value += SeparadorDecimal + aux.substr(len - 2, len);
            }
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:TextBox runat="server" ID="Dialogvalue" CssClass="modalhiddenfield" onchange="checkTextChange();" Style="display: none; visibility: hidden;"></asp:TextBox>
    <div style="font-family: Calibri; font-size: 16px">
        <div>
            <table width="100%" border="0">
                <tr valign="middle">
                    <td style="width: 335px">
                        <b>Digite o Salario R$:</b>
                        <asp:TextBox ID="SearchBox" onKeypress="return(MascaraMoeda(this,'.',',',event))" runat="server" Width="210px" ToolTip="Digite o Salário..."></asp:TextBox>
                    </td>
                    <td style="width: 110px; font-family: Calibri; font-size: 14px" valign="middle">
                        &nbsp;&nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <!-- Insert the modal dialog box OK and Cancel buttons here--->
        <div style="padding-top: 10px">
            <asp:Button runat="server" ID="btnConfirmar" Text="Confirmar" OnClick="Confirmar_Onclick" />
            <input type="button" name="BtnCancel" id="btnModalCancel" value="Cancelar" onclick="ModalCancel_click();" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Application Page
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Definir Salário
</asp:Content>
