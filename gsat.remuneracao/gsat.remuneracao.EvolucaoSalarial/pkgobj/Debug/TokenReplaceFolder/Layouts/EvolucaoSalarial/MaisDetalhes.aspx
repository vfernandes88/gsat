<%@ Assembly Name="Globosat.Remuneracao.EvolucaoSalarial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c757b7f67732b175" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaisDetalhes.aspx.cs" Inherits="Globosat.Remuneracao.EvolucaoSalarial.Layouts.EvolucaoSalarial.MaisDetalhes" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="/_layouts/Cit.Globosat.Base/Scripts/jquery-1.10.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/Cit.Globosat.Base/Scripts/log4javascript_production.js"></script>
    <script type="text/javascript" src="/_layouts/Cit.Globosat.Base/Scripts/Cit.Globosat.Base.js"></script>
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            $('#<%= cbAcordoColetivo.ClientID %>').change(function () {
                showMessageModal();
                return true;
            });

            $('#<%= ddlFuncionarios.ClientID %>').change(function () {
                showMessageModal();
                return true;
            });
        });

        var showMessageModal = function () {
            /*When our page is in dialog mode, then window.parent is the parent page of the dialog. When it is NOT
            in dialog mode, then window.parent is just the current page. window.parent is never null.
 
            When our page is in dialog mode, we need to execute showWaitScreenWithNoClose in the context of the parent page. This
            is because when the postBack completes on our dialog, the context of our dialog will be destroyed, and calling close
            on the wait screen object will result in a "Can't execute code from a freed script" error in IE (works OK in Chrome though).
 
            When our page is NOT in dialog mode, the wait screen will be removed when the postBack completes and the page is reloaded
            anyway, i.e. we don't have to worry about it.
 
            We also need to store the waitDialog variable in a place that will survive the postBack. This is the parent page
            when our page is in dialog mode. When our page is NOT in dialog mode, then we don't really care - so just put it anywhere.
 
            Because we are calling eval under the parent page's context, the waitDialog below is stored in the correct place.
            */
            window.parent.eval("window.waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('Aguarde!', 'Processando...', 76, 330);");
        }

        var printPDF = function () {
            showMessageModal();
            
            // Post Back does not work after writing files to response in ASP.NET.
            setTimeout(function () {
                _spFormOnSubmitCalled = false; 

                if (window.frameElement != null) {
                    if (window.parent.waitDialog != null) {
                        window.parent.waitDialog.close();
                    }
                }
            }, 3000);
            return true;
        }

        var sendMail = function () {
            showMessageModal();
            return true;
        }
    </script>
    <style type="text/css">
        .gridView
        {
            padding: "4px";
            text-transform: capitalize;
        }
        .marginImage
        {
            margin: 0 10px 0 0;
        }
    </style>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table width="100%" border="0" style="overflow: hidden">
        <tr>
            <td align="right">
                <asp:ImageButton ID="btnPDF" runat="server" OnClick="btnPDF_Click" OnClientClick="javascript:return printPDF();" ImageUrl="~/_layouts/images/EvolucaoSalarial/pdf_icon.jpg" />
                <asp:ImageButton ID="btnImprimir" runat="server" OnClientClick="javascript:window.print()" ImageUrl="~/_layouts/images/EvolucaoSalarial/print_icon.jpg" />
                <asp:ImageButton ID="btnEmail" runat="server" OnClick="btnEmail_Click" OnClientClick="javascript:return sendMail();" CssClass="marginImage" ImageUrl="~/_layouts/images/EvolucaoSalarial/mail_icon.jpg" />
            </td>
        </tr>
    </table>
    <div id="tablePDF">
        <table width="100%" border="0" style="overflow: hidden">
            <tr>
                <td align="center">
                    <h2>
                        Evolução Salarial</h2>
                    <br />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:Label ID="lblFuncionarios" runat="server" Text="Selecione o funcionário:"></asp:Label>
                    &nbsp;
                    <asp:DropDownList ID="ddlFuncionarios" runat="server" OnSelectedIndexChanged="dllFuncionarios_SelectedIndexChanged" />
                    <br />
                    <br />
                    <asp:CheckBox ID="cbAcordoColetivo" runat="server" Text="Mostrar acordo coletivo" Checked="false" AutoPostBack="true" />
                    <br />
                    <br />
                </td>
            </tr>
            <tr>
                <td align="center">
                    <table style="border: 1px solid #8A9095" width="70%">
                        <tr>
                            <td align="center" style="border-right: 1px solid #8A9095;">
                                <h2 style="margin: 0">
                                    <asp:Label ID="lblMatricula" runat="server" /></h2>
                            </td>
                            <td align="center">
                                <h2 style="margin: 0">
                                    <asp:Label ID="lblNome" runat="server" /></h2>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:GridView runat="server" CssClass="gridView" ID="GVMaisInf" AutoGenerateColumns="false" EmptyDataText="Não possuem informações disponíveis" HorizontalAlign="Center" HeaderStyle-BackColor="#999999"
                        HeaderStyle-ForeColor="Black" Width="850px" RowStyle-Height="30" HeaderStyle-Font-Size="12pt" RowStyle-Font-Size="10pt" RowStyle-HorizontalAlign="Center">
                        <Columns>
                            <asp:BoundField HeaderText="Data" DataField="Data">
                                <ItemStyle CssClass="gridView" />
                            </asp:BoundField>
                        </Columns>
                        <Columns>
                            <asp:BoundField HeaderText="Salário" DataField="Salario">
                                <ItemStyle CssClass="gridView" />
                            </asp:BoundField>
                        </Columns>
                        <Columns>
                            <asp:BoundField HeaderText="%" DataField="Percentual">
                                <ItemStyle CssClass="gridView" />
                            </asp:BoundField>
                        </Columns>
                        <Columns>
                            <asp:BoundField HeaderText="Motivo" DataField="Motivo">
                                <ItemStyle CssClass="gridView" />
                            </asp:BoundField>
                        </Columns>
                        <Columns>
                            <asp:BoundField HeaderText="Função" DataField="Funcao">
                                <ItemStyle CssClass="gridView" />
                            </asp:BoundField>
                        </Columns>
                        <Columns>
                            <asp:BoundField HeaderText="Classe" DataField="Classe">
                                <ItemStyle CssClass="gridView" />
                            </asp:BoundField>
                        </Columns>
                        <Columns>
                            <asp:BoundField HeaderText="Nível" DataField="Nivel">
                                <ItemStyle CssClass="gridView" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Chart ID="ChartEvolucao" runat="server" Width="850px">
                        <Series>
                            <asp:Series Name="Evolução Salarial (Últimos 10 anos)" XValueMember="Data" YValueMembers="PercentualNumber" IsVisibleInLegend="true">
                            </asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea1" Area3DStyle-Enable3D="true">
                                <AxisX LineColor="DarkGray">
                                    <MajorGrid LineColor="LightGray" />
                                </AxisX>
                                <AxisY LineColor="DarkGray">
                                    <MajorGrid LineColor="LightGray" />
                                </AxisY>
                            </asp:ChartArea>
                        </ChartAreas>
                        <Legends>
                            <asp:Legend>
                            </asp:Legend>
                        </Legends>
                    </asp:Chart>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblError" runat="server" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Application Page
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    My Application Page
</asp:Content>
