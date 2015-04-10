<%@ Assembly Name="Globosat.Remuneracao.FaleConosco, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d9eb8068188b8854" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPFaleConoscoUserControl.ascx.cs"
    Inherits="Globosat.Remuneracao.FaleConosco.WPFaleConosco.WPFaleConoscoUserControl" %>
<table width="100%">
    <tr>
        <td align="center">
            <table>
                <tr>
                <td>
                <h4>Escreva sua dúvida ou comentário e clique em enviar.</h4><br />
                </td>
                </tr>
                <tr>
                    <td>
                    <SharePoint:InputFormTextBox RichTextMode="FullHtml" runat="server" ID="tbComentario" Wrap="true" Width="400px" Height="200px" TextMode="MultiLine"/>
                    </td>
                </tr>
                <tr>
                <td>
                <br />
                <asp:Button ID="btnEnviar" Text="Enviar" runat="server" OnClick="btnEnviar_Click" />
                </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
