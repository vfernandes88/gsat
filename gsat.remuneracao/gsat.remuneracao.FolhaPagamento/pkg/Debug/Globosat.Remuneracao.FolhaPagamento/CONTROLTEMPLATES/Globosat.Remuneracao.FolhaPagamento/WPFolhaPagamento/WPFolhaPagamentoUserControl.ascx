<%@ Assembly Name="Globosat.Remuneracao.FolhaPagamento, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c353ebd6b305333e" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPFolhaPagamentoUserControl.ascx.cs"
    Inherits="Globosat.Remuneracao.FolhaPagamento.WPFolhaPagamento.WPFolhaPagamentoUserControl" %>
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
        <td align="left"><b>Selecione o Centro de Custo:&nbsp;</b>
            <asp:DropDownList ID="ddlCentroCusto" runat="server" AutoPostBack="true" />
        </td>
    </tr>
    <tr>
        <td align="right">
            <asp:ImageButton ID="btnImprimir" runat="server" OnClientClick="javascript:window.print()" ImageUrl="~/_layouts/images/EvolucaoSalarial/print_icon.jpg" />
            <asp:ImageButton ID="btnEnviar" runat="server" OnClick="Onclick_btnEnviar" ImageUrl="~/_layouts/images/EvolucaoSalarial/mail_icon.jpg" />
        </td>
    </tr>
    <tr>
        <td align="center">
        <asp:Panel ID="pnlDados" runat="server">
            <asp:Label ID="lblCentroCusto" runat="server" CssClass="CentroCusto" Text='<%# Eval("DESCRICAO") %>' />
            <br />
            <table class="sample" width="930px" runat="server" id="tableHeader" visible="false">
                <tr>
                    <td style="width: 115px" align="center">
                        <span class="labelsHeader"><b>Imagem</b></span>
                    </td>
                    <td style="width: 70px" align="center">
                        <span class="labelsHeader"><b>Matrícula</b></span>
                    </td>
                    <td style="width: 200px" align="center">
                        <span class="labelsHeader"><b>Nome</b></span>
                    </td>
                    <td style="width: 200px" align="center">
                        <span class="labelsHeader"><b>Função</b></span>
                    </td>
                    <td style="width: 85px" align="center">
                        <span class="labelsHeader"><b>Salário</b></span>
                    </td>
                    <td style="width: 50px" align="center">
                        <span class="labelsHeader"><b>Nível</b></span>
                    </td>
                    <td style="width: 50px" align="center">
                        <span class="labelsHeader"><b>Classe</b></span>
                    </td>
                    <td style="width: 75px" align="center">
                        <span class="labelsHeader"><b>Admissão</b></span>
                    </td>
                    <td style="width: 85px" align="center">
                        <span class="labelsHeader"><b>Nascimento</b></span>
                    </td>
                </tr>
            </table>
            <br />
            <asp:Repeater runat="server" ID="rptColaboradores">
                <ItemTemplate>
                    <table class="sample" width="930px">
                        <tr>
                            <td style="width: 115px">
                                <asp:Image runat="server" ID="imgColaborador" ImageUrl='<%# Eval("Foto") %>' />
                            </td>
                            <td style="width: 70px">
                                <asp:Label CssClass="labels" runat="server" ID="lblMatriculaColaborador" Text='<%# Eval("Matricula") %>' />
                            </td>
                            <td style="width: 200px">
                                <asp:Label CssClass="labels" runat="server" ID="lblNomeColaborador" Text='<%# Eval("Nome") %>' />
                            </td>
                            <td style="width: 200px">
                                <asp:Label CssClass="labels" runat="server" ID="lblCargoColaborador" Text='<%# Eval("Funcao") %>' />
                            </td>
                            <td style="width: 85px">
                                <asp:Label CssClass="labels" runat="server" ID="lblSalario" Text='<%# Eval("Salario") %>' />
                            </td>
                            <td style="width: 50px">
                                <asp:Label CssClass="labels" runat="server" ID="lblClasse" Text='<%# Eval("Nivel") %>' />
                            </td>
                            <td style="width: 50px">
                                <asp:Label CssClass="labels" runat="server" ID="lblNivel" Text='<%# Eval("Classe") %>' />
                            </td>
                            <td style="width: 75px">
                                <asp:Label CssClass="labels" runat="server" ID="lblAdmissao" Text='<%# Eval("Admissao") %>' />
                            </td>
                            <td style="width: 85px">
                                <asp:Label CssClass="labels" runat="server" ID="lblDtNascimento"  Text='<%# Eval("DtNascimento") %>' />
                            </td>
                        </tr>
                    </table>
                    <br />
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:Label ID="lblErroMsg" runat="server" Visible="false" />
        </td>
    </tr>
</table>
