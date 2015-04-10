<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BuscaCargo.aspx.cs" Inherits="Globosat.Remuneracao.CustomPages.Layouts.Globosat.Remuneracao.CustomPages.BuscaCargo" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script src="/_layouts/Globosat.Remuneracao.CustomPages/scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="/_layouts/Globosat.Remuneracao.CustomPages/scripts/BuscaCargo.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:TextBox runat="server" ID="Dialogvalue" CssClass="modalhiddenfield" onchange="checkTextChange();" Style="display: none; visibility: hidden;"></asp:TextBox>
    <div style="font-family: Calibri; font-size: 16px">
        <div>
            <!--Inicio-->
            <table width="100%" border="0">
                <tr valign="middle">
                    <td style="width: 335px">
                        <b>Digite o cargo:</b>
                        <asp:TextBox ID="SearchBox" runat="server" Width="220px" ToolTip="Digite o cargo..."></asp:TextBox>
                    </td>
                    <td style="width: 110px; font-family: Calibri; font-size: 14px" valign="middle">
                        &nbsp;&nbsp;<asp:RadioButtonList RepeatLayout="Table" ID="rbLocaltrabalho" RepeatDirection="Horizontal" runat="server" Font-Bold="true" Enabled="false">
                            <asp:ListItem Text="RJ" Value="RJ" />
                            <asp:ListItem Text="SP" Value="SP" />
                        </asp:RadioButtonList>
                    </td>
                    <td>
                        <asp:ImageButton ID="Dosearch" runat="server" ImageUrl="~/_layouts/images/searchlogo.png" ImageAlign="Top" OnClick="Dosearch_Click" Width="20px" AlternateText="search" ToolTip="Buscar cargo" />
                    </td>
                </tr>
            </table>
            <div style="padding-top: 10px">
                <div style="padding-bottom: 10px">
                    <asp:Label runat="server" Visible="false" ID="ResultCount"></asp:Label>
                </div>
                <SharePoint:SPGridView ID="ResultGrid" runat="server" AutoGenerateColumns="false" ShowHeader="true" RowStyle-BackColor="#EBEBEB" HeaderStyle-BackColor="#C3C3C3" AlternatingRowStyle-BackColor="#F6F6F6"
                    EnableTheming="true" ShowHeaderWhenEmpty="true" AutoGenerateSelectButton="true" SelectedRowStyle-BackColor="#EDE275" AllowSorting="true">
                    <Columns>
                        <asp:BoundField HeaderText="Cargo" DataField="CODNOME" ShowHeader="true" HeaderStyle-BackColor="#C3C3C3" HeaderStyle-ForeColor="#000000" />
                        <asp:BoundField HeaderText="Jornada" DataField="JORNADA" ShowHeader="true" HeaderStyle-BackColor="#C3C3C3" HeaderStyle-ForeColor="#000000" />
                        <asp:BoundField HeaderText="Nível" DataField="NIVEL" ShowHeader="true" HeaderStyle-BackColor="#C3C3C3" HeaderStyle-ForeColor="#000000" />
                        <asp:BoundField HeaderText="Faixa" DataField="FAIXA" ShowHeader="true" HeaderStyle-BackColor="#C3C3C3" HeaderStyle-ForeColor="#000000" />
                        <asp:BoundField HeaderText="Salário" DataField="SALARIO" ShowHeader="true" HeaderStyle-BackColor="#C3C3C3" HeaderStyle-ForeColor="#000000" />
                    </Columns>
                </SharePoint:SPGridView>
            </div>
        </div>
        <!-- Insert the modal dialog box OK and Cancel buttons here--->
        <div style="padding-top: 10px">
            &nbsp;&nbsp;
            <input type="button" name="BtnCancel" id="btnModalCancel" value="Cancelar" onclick="ModalCancel_click();" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Application Page
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Busca de cargo
</asp:Content>
