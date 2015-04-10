<%@ Assembly Name="Cit.Globosat.Remuneracao.Formularios, Version=1.0.0.0, Culture=neutral, PublicKeyToken=dfe7308c061203c4" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReqPessoalEstagVWPUserControl.ascx.cs" Inherits="Cit.Globosat.Remuneracao.Formularios.WebParts.ReqPessoalEstagVWP.ReqPessoalEstagVWPUserControl" %>
<%@ Register Assembly="Cit.Globosat.Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=dfe7308c061203c4" Namespace="Cit.Globosat.Controls" TagPrefix="cc1" %>
<script type="text/javascript" src="/_layouts/Cit.Globosat.Base/Scripts/jquery-1.10.1.min.js"></script>
<script type="text/javascript" src="/_layouts/Cit.Globosat.Base/Scripts/log4javascript_production.js"></script>
<script type="text/javascript" src="/_layouts/Cit.Globosat.Base/Scripts/Cit.Globosat.Base.js"></script>
<script type="text/javascript" language="javascript"></script>
<script type="text/javascript" language="javascript">
    var printPDF = function () {

        // Para os raddionbuttons que não possuem postback é preciso adicionar 
        // o atributo de checked nos campos que o usuário selecionou.

        // Status da Vaga
        if ($('input[id*="<%= radioButtonListStatusVaga.ClientID %>"]:checked').val() !== undefined) {
            $('input:radio[value=' + $('input[id*="<%= radioButtonListStatusVaga.ClientID %>"]:checked').val() + ']').attr('checked', 'checked');
        }

        // Orçado
        if ($('input[id*="<%= radioButtonListOrcado.ClientID %>"]:checked').val() !== undefined) {
            $('input:radio[value=' + $('input[id*="<%= radioButtonListOrcado.ClientID %>"]:checked').val() + ']').attr('checked', 'checked');
        }

        // Post Back does not work after writing files to response in ASP.NET.
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);

        var printPDF = '<HTML><Head><Title></Title>';
        printPDF += '<link rel="stylesheet" type="text/css" href="/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/ReqPessoalEstag/ReqPessoalEstag.css" />';
        printPDF += '<link rel="stylesheet" type="text/css" href="/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/ReqPessoalEstag/PrintToPDF.css" />';
        printPDF += '</Head><Body>';
        printPDF += $('#divForm').html();
        printPDF += '</Body></HTML>';

        document.getElementById('<%= hiddenFieldPDF.ClientID %>').value = printPDF;

        return true;
    }
</script>
<link rel="stylesheet" type="text/css" href="/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/ReqPessoalEstag/ReqPessoalEstag.css" />
<asp:HiddenField runat="server" ID="hiddenFieldPDF" />
<div id="divForm">
    <div id="divBotoes">
        <table id="tableButtons" align="center" border="0" class="tableFormulario">
            <tr>
                <td style="text-align: right;">
                    <asp:ImageButton runat="server" ID="imageButtonVoltar" ImageUrl="~/_layouts/images/Cit.Globosat.Base/icon_back.jpg" ToolTip="Voltar" OnClick="imageButtonVoltar_Click" />
                    <asp:ImageButton runat="server" ID="imageButtonImprimir" ImageUrl="~/_layouts/images/Cit.Globosat.Base/print_icon_disable.jpg" OnClick="imageButtonImprimir_Click" />
                    <asp:ImageButton runat="server" ID="imageButtonGerarPDF" ImageUrl="~/_layouts/images/Cit.Globosat.Base/pdf_icon_disable.jpg" OnClick="imageButtonGerarPDF_Click" OnClientClick="javascript:printPDF();" />
                </td>
            </tr>
        </table>
    </div>
    <table id="tableHeader" align="center" border="1" class="tableFormulario">
        <tr>
            <td class="tdImage">
                <asp:Image runat="server" ID="imageLogo" style="border-width:0px;width: 113px;" />
            </td>
            <th>
                <asp:Label runat="server" ID="labelTitulo" Text="REQUISIÇÃO DE ESTAGIÁRIO" CssClass="titulo"></asp:Label>
            </th>
        </tr>
    </table>
    <table id="tableBody1" align="center" border="1" class="tableFormulario" style="border-bottom: 0px;">
        <tr valign="top">
            <td style="width: 100%">
                <asp:Label runat="server" ID="labelEstagioEm" Text="ESTÁGIO EM"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxEstagioEm" Width="100%" CssClass="input1"></asp:TextBox>
            </td>
            <td nowrap="nowrap">
                <asp:Label runat="server" ID="labelDataRequisicao" Text="DATA DA REQUISIÇÃO"></asp:Label>
                <br />
                <SharePoint:DateTimeControl runat="server" SelectedDate="" DateOnly="true" ID="dateTimeControlRequisicao" LocaleId="1046" CssClassTextBox="input1" />
            </td>
            <td nowrap="nowrap">
                <asp:Label runat="server" ID="labelDataInicio" Text="DATA INÍCIO"></asp:Label>
                <br />
                &nbsp;(<i>a ser preenchido pelo RH</i>)
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
        </tr>
    </table>
    <table id="table1" align="center" border="1" class="tableFormulario" style="border-bottom: 0px;">
        <tr>
            <td valign="top" style="width: 50%;">
                <asp:Label runat="server" ID="labelNomeCandidato" Text="NOME DO CANDIDATO SELECIONADO"></asp:Label>
            </td>
            <td style="width: 50%;">
                <asp:Label runat="server" ID="label2" Text="EM SUBSTITUÍÇÃO A"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxEmSubstituicao" Width="100%" CssClass="input1"></asp:TextBox>
            </td>
        </tr>
    </table>
    <table id="table2" align="center" border="1" class="tableFormulario">
        <tr>
            <td colspan="2">
                <asp:Label runat="server" ID="labelCentroCusto" Text="CENTRO DE CUSTO"></asp:Label>
                <br />
                <asp:DropDownList runat="server" ID="dropDownListCentroCusto" CssClass="dropdownlist" AutoPostBack="true" Width="99%" OnSelectedIndexChanged="dropDownListCentroCusto_SelectedIndexChanged" />
            </td>
            <td>
                <asp:Label runat="server" ID="labelDepartamentoArea" Text="DEPARTAMENTO / ÁREA"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxDepartamentoArea" Width="100%" CssClass="input1" ReadOnly="true"></asp:TextBox>
            </td>
            <td>
                <asp:Label runat="server" ID="labelDiretoria" Text="DIRETORIA"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxDiretoria" Width="100%" CssClass="input1" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="labelNivel" Text="NÍVEL"></asp:Label>
                <br />
                <asp:RadioButtonList runat="server" ID="radioButtonListNivel" RepeatColumns="2" AutoPostBack="true" OnSelectedIndexChanged="radioButtonListNivel_SelectedIndexChanged">
                    <asp:ListItem Value="MEDIO">MÉDIO</asp:ListItem>
                    <asp:ListItem Value="SUPERIOR">SUPERIOR</asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td>
                <asp:Label runat="server" ID="labelValorAuxilioBolsa" Text="VALOR AUXÍLIO-BOLSA"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxValorAuxilioBolsa" Width="100%" CssClass="input1" ReadOnly="true"></asp:TextBox>
            </td>
            <td nowrap="nowrap">
                <asp:Label runat="server" ID="labelStatusVaga" Text="STATUS DA VAGA"></asp:Label>
                <br />
                <asp:RadioButtonList runat="server" ID="radioButtonListStatusVaga" RepeatColumns="2">
                    <asp:ListItem Value="VAGA_NOVA">VAGA NOVA</asp:ListItem>
                    <asp:ListItem Value="SUBSTITUICAO">SUBSTITUÍÇÃO</asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td>
                <asp:Label runat="server" ID="labelOrcado" Text="ORÇADO"></asp:Label>
                <br />
                <asp:RadioButtonList runat="server" ID="radioButtonListOrcado" RepeatColumns="2">
                    <asp:ListItem Value="1">SIM</asp:ListItem>
                    <asp:ListItem Value="0">NÃO</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Label runat="server" ID="labelJustificativa" Text="JUSTIFICATIVA ( NO CASO DE NÃO ESTAR ORÇADO)"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxJustificativa" TextMode="MultiLine" CssClass="textarea"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label runat="server" ID="labelEstudanteCurso" Text="ESTUDANTE DO CURSO"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxEstudanteCurso" CssClass="input1"></asp:TextBox>
            </td>
            <td colspan="2">
                <asp:Label runat="server" ID="labelPeriodo" Text="PERÍODO"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxPeriodo" CssClass="input1"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label runat="server" ID="labelHorarioEstagio" Text="HORÁRIO DE ESTÁGIO"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxHorarioEstagio" CssClass="input1"></asp:TextBox>
            </td>
            <td align="center" colspan="2">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="labelJornadaSemanal" Text="JORNADA SEMANAL"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            30 HORAS
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Label runat="server" ID="labelObservacoes" Text="OBSERVAÇÕES"></asp:Label>
            </td>
        </tr>
        <tr valign="top">
            <td style="height: 80px;" colspan="4">
                <div style="height: 20px;">
                </div>
                <span>
                    <hr />
                </span>
                <div style="height: 20px;">
                </div>
                <span>
                    <hr />
                </span>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="labelSupervisorEstagio" Text="SUPERVISOR DO ESTÁGIO"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxSupervisorEstagio" CssClass="input1" Width="100%"></asp:TextBox>
            </td>
            <td colspan="2">
                <asp:Label runat="server" ID="labelCargo" Text="CARGO"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxCargo" CssClass="input1" Width="100%"></asp:TextBox>
            </td>
            <td nowrap="nowrap">
                <asp:Label runat="server" ID="labelFormacaoProfissional" Text="FORMAÇÃO PROFISSIONAL"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxFormacaoProfissional" Width="100%" CssClass="input1"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Label runat="server" ID="labelRequisitante" Text="REQUISITANTE"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxRequisitante" CssClass="input1" Width="100%"></asp:TextBox>
            </td>
            <td>
                <asp:Label runat="server" ID="labelDataRequisitante" Text="DATA"></asp:Label>
                <br />
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Label runat="server" ID="labelDiretoriaArea" Text="DIRETORIA DA ÁREA"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxDiretoriaArea" CssClass="input1" Width="100%"></asp:TextBox>
            </td>
            <td>
                <asp:Label runat="server" ID="labelDataDiretoriaArea" Text="DATA"></asp:Label>
                <br />
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Label runat="server" ID="labelRecursosHumanos" Text="RECURSOS HUMANOS"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxRecursosHumanos" CssClass="input1" Width="100%"></asp:TextBox>
            </td>
            <td>
                <asp:Label runat="server" ID="label3" Text="DATA"></asp:Label>
                <br />
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Label runat="server" ID="labelDiretoriaGestao" Text="DIRETORIA DE GESTÃO"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxDiretoriaGestao" CssClass="input1" Width="100%"></asp:TextBox>
            </td>
            <td>
                <asp:Label runat="server" ID="labelDataDiretoriaGestao" Text="DATA"></asp:Label>
                <br />
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;/
            </td>
        </tr>
    </table>
</div>
