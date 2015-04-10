<%@ Assembly Name="RemVariavel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c54e82ac2c32471a" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPRemVariavelUserControl.ascx.cs" Inherits="RemVariavel.WPRemVariavel.WPRemVariavelUserControl" %>
<style type="text/css">
    .table
    {
        border: 1 solid black;
        border-collapse: collapse;
    }
    .td
    {
        border: 1 solid black;
    }
    
    table.sample
    {
        border-width: 2px;
        border-spacing: 1px;
        border-style: solid;
        border-color: black;
        border-collapse: collapse;
    }
    
    table.sample td
    {
        border-width: 1px;
        padding: 1px;
        border-style: solid;
        border-color: black;
    }
    .labels
    {
        font-size: 11px;
        font-family: Verdana;
    }
    .labelsHeader
    {
        font-size: 12px;
        font-family: Verdana;
    }
    .CentroCusto
    {
        font-size: 14px;
        font-family: Verdana;
        font-weight: bolder;
    }
</style>
<br />
<br />
<table width="100%">
    <tr>
        <td align="left">
            <table>
                <tr>
                    <td align="right" nowrap="nowrap">
                        <asp:Label runat="server" ID="lblSelCentroCusto" Text="Selecione o Centro de Custo:" Font-Bold="true"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:DropDownList ID="ddlCentroCusto" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCentroCusto_SelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label runat="server" ID="lblSelAno" Text="Selecione o Ano:" Font-Bold="true"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:DropDownList runat="server" ID="ddlAno" Enabled="false" AutoPostBack="true" OnSelectedIndexChanged="ddlAno_SelectedIndexChanged">
                            <asp:ListItem Value="0">Selecione...</asp:ListItem>
                            <asp:ListItem Value="2012">2012</asp:ListItem>
                            <asp:ListItem Value="2013">2013</asp:ListItem>
                            <asp:ListItem Value="2014">2014</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td align="right">
            <asp:ImageButton ID="btnImprimir" runat="server" OnClientClick="javascript:window.print()" ImageUrl="~/_layouts/images/EvolucaoSalarial/print_icon.jpg" />
            <asp:ImageButton  ID="btnEnviar" runat="server" ImageUrl="~/_layouts/images/EvolucaoSalarial/mail_icon.jpg" OnClick="Onclick_btnEnviar" />
        </td>
    </tr>
    <tr runat="server" id="trDados">
        <td style="font-family: Calibri; font-size: 8px;" align="center">
            <asp:Label ID="lblCentroCusto" runat="server" CssClass="CentroCusto" Text='<%# Eval("DESCRICAO") %>' />
            <br />
            <table style="font-family: Calibri;" class="sample" width="100%" runat="server" id="tableHeader" visible="false">
                <tr>
                    <td align="center" style="font-family: Calibri; width: 180px; min-width: 110px;">
                        <span class="labelsHeader"><b>Imagem</b></span>
                    </td>
                    <td style="font-family: Calibri; width: 180px; min-width: 110px;" align="center">
                        <span class="labelsHeader"><b>Nome</b></span>
                    </td>
                    <td style="font-family: Calibri; width: 150px; min-width: 110px;" align="center">
                        <span class="labelsHeader"><b>Função</b></span>
                    </td>
                    <td style="font-family: Calibri; width: 150px; min-width: 110px;" align="center">
                        <span class="labelsHeader"><b>Salário (<asp:Label runat="server" ID="lblMesSalario" />/<%= this.ddlAno.SelectedValue %>)</b></span>
                    </td>
                    <td style="font-family: Calibri; width: 110px; min-width: 110px;" align="center">
                        <span class="labelsHeader"><b>Pagamento</b></span>
                    </td>
                    <td style="font-family: Calibri; width: 110px; min-width: 110px;" align="center">
                        <span class="labelsHeader"><b>Participe</b></span>
                    </td>
                    <td style="font-family: Calibri; width: 110px; min-width: 110px;" align="center">
                        <span class="labelsHeader"><b>Part. Variável</b></span>
                    </td>
                    <td style="font-family: Calibri; width: 110px; min-width: 110px;" align="center" runat="server" id="tdColBonusTitulo">
                        <span class="labelsHeader"><b>Bônus</b></span>
                    </td>
                    <td style="font-family: Calibri; width: 110px; min-width: 110px;" align="center">
                        <span class="labelsHeader"><b>Totais</b></span>
                    </td>
                </tr>
            </table>
            <br />
            <asp:Repeater runat="server" ID="rptColaboradores" OnItemDataBound="rptColaboradores_ItemDataBound">
                <ItemTemplate>
                    <table style="font-family: Calibri; border-width: 2px; border-spacing: 1px; border-style: solid; border-color: black; border-collapse: collapse;" class="sample" width="100%">
                        <tr>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 180px; min-width: 110px;" rowspan="4" align="center">
                                <asp:Image runat="server" ID="imgColaborador" ImageUrl='<%# Eval("Foto") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 180px; min-width: 110px;" rowspan="4" align="center">
                                <asp:Label CssClass="labels" runat="server" ID="lblNomeColaborador" Text='<%# Eval("Nome") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 150px; min-width: 110px;" rowspan="4" align="center">
                                <asp:Label CssClass="labels" runat="server" ID="lblCargoColaborador" Text='<%# Eval("Funcao") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 150px; min-width: 110px;" rowspan="4" align="center">
                                <asp:Label CssClass="labels" runat="server" ID="lblSalario" Text='<%# Eval("Salario") %>' />
                            </td>
                            <td style="font-family: Calibri; font-size: 14px; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center">
                                Adiantamento (Julho)
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center">
                                <asp:Label CssClass="labels" runat="server" ID="lblParticipe7" Text='<%# Eval("Participe7", "{0:C}") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center">
                                <asp:Label CssClass="labels" runat="server" ID="lblParticipeVariavel7" Text='<%# Eval("ParticipeVariavel7", "{0:C}") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center" runat="server" id="tdColBonus7">
                                <asp:Label CssClass="labels" runat="server" ID="lblBonus7" Text='<%# Eval("Bonus7", "{0:C}") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center">
                                <asp:Label CssClass="labels" runat="server" ID="lblTotal7" Text='<%# Eval("Total7", "{0:C}") %>' />
                            </td>
                        </tr>
                        <tr style="background-color: #CCCCCC" align="center" runat="server" id="trParcelaJaneiro">
                            <td style="font-family: Calibri; font-size: 14px;">
                                2° parcela
                                <br />
                                (Janeiro)
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;">
                                <asp:Label CssClass="labels" runat="server" ID="Label1" Text='<%# Eval("Participe1", "{0:C}") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;">
                                <asp:Label CssClass="labels" runat="server" ID="Label2" Text='<%# Eval("ParticipeVariavel1", "{0:C}") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" runat="server" id="tdColBonus1">
                                <asp:Label CssClass="labels" runat="server" ID="Label3" Text='<%# Eval("Bonus1", "{0:C}") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;">
                                <asp:Label CssClass="labels" runat="server" ID="Label4" Text='<%# Eval("Total1", "{0:C}") %>' />
                            </td>
                        </tr>
                        <tr runat="server" id="trTotalAno">
                            <td style="font-family: Calibri; font-size: 14px; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center">
                                Total Ano ($)
                            </td>
                            <td align="center">
                                <asp:Label CssClass="labels" runat="server" ID="Label5" Text='<%# Eval("TotalParticipeAno", "{0:C}") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center">
                                <asp:Label CssClass="labels" runat="server" ID="Label6" Text='<%# Eval("TotalParticipeVariavelAno", "{0:C}") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center" runat="server" id="tdColTotalBonusAno">
                                <asp:Label CssClass="labels" runat="server" ID="Label7" Text='<%# Eval("TotalBonusAno", "{0:C}") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center">
                                <asp:Label CssClass="labels" runat="server" ID="Label8" Text='<%# Eval("TotalS", "{0:C}") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td style="font-family: Calibri; font-size: 14px; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center">
                                Total Ano
                                <br />
                                (N° Salários)
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center">
                                <asp:Label CssClass="labels" runat="server" ID="Label9" Text='<%# Eval("TotalParticipeAnoNSalarios", "{0:0.00}") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center">
                                <asp:Label CssClass="labels" runat="server" ID="Label10" Text='<%# Eval("TotalParticipeVariavelAnoNSalarios", "{0:0.00}") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center" runat="server" id="tdColTotalBonusAnoNSalarios">
                                <asp:Label CssClass="labels" runat="server" ID="Label11" Text='<%# Eval("TotalBonusAnoNSalarios", "{0:0.00}") %>' />
                            </td>
                            <td style="font-family: Calibri; border-width: 1px; padding: 1px; border-style: solid; border-color: black; width: 110px; min-width: 110px;" align="center">
                                <asp:Label CssClass="labels" runat="server" ID="Label12" Text='<%# Eval("TotalNSalarios", "{0:0.00}") %>' />
                            </td>
                        </tr>
                    </table>
                    <br />
                </ItemTemplate>
            </asp:Repeater>
        </td>
    </tr>
    <tr valign="middle">
        <td valign="middle">
            <asp:TextBox ID="txtLegenda" runat="server" BackColor="#CCCCCC" BorderColor="#CCCCCC" BorderWidth="0px" ForeColor="#CCCCCC" ReadOnly="True" Width="54px" BorderStyle="Solid" Visible="false"></asp:TextBox>
            &nbsp;
            <asp:Label CssClass="labels" ID="lblLegenda" runat="server" Visible="false" />
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:Label ID="lblErroMsg" runat="server" Visible="false" />
        </td>
    </tr>
</table>
