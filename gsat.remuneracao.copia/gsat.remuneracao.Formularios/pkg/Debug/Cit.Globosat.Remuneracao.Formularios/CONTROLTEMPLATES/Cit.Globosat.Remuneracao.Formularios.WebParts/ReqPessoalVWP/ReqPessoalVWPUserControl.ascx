<%@ Assembly Name="Cit.Globosat.Remuneracao.Formularios, Version=1.0.0.0, Culture=neutral, PublicKeyToken=dfe7308c061203c4" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReqPessoalVWPUserControl.ascx.cs" Inherits="Cit.Globosat.Remuneracao.Formularios.WebParts.ReqPessoalVWP.ReqPessoalVWPUserControl" %>
<script type="text/javascript" src="/_layouts/Cit.Globosat.Base/Scripts/jquery-1.10.1.min.js"></script>
<script type="text/javascript" src="/_layouts/Cit.Globosat.Base/Scripts/log4javascript_production.js"></script>
<script type="text/javascript" src="/_layouts/Cit.Globosat.Base/Scripts/Cit.Globosat.Base.js"></script>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {

        // Remove o cursor do campo para que o usuário não confunda com um campo digitável.
        $('input[readonly="readonly"]').focus(function () {
            $(this).blur();
            return false;
        });

        // Botão Buscar. Abre modal para pesquisar um cargo.
        $('#<%= btnBuscar.ClientID %>').click(function () {
            var filial = $('input[id*=<%= rbFilial.ClientID %>]:checked').val();
            var codColigada = $('#<%= hiddenField_coligadaCentroCusto.ClientID %>').val();
            var options = {
                url: getRootSiteUrl() + '_layouts/Globosat.Remuneracao.CustomPages/BuscaCargo.aspx?filial=' + filial + '&codColigada=' + codColigada,
                title: 'Buscar Novo Cargo',
                showClose: true,
                allowMaximize: false,
                autoSize: true
            };
            options.dialogReturnValueCallback = Function.createDelegate(null, closeCallbackBuscarCargo);
            SP.UI.ModalDialog.showModalDialog(options);

            return false;
        });

        // Itens específicos do campo Nível. Abre modal para definir salário.
        $('#<%= ddlSalNivel.ClientID %>').change(function () {
            var value = $(this).val();
            if ((value === 'I*') || (value === '')) {
                var options = {
                    url: getRootSiteUrl() + '_layouts/Globosat.Remuneracao.CustomPages/AlteraSalarioProposto.aspx',
                    title: 'Aletrar Salário Proposto',
                    width: 300,
                    height: 110,
                    showClose: true,
                    allowMaximize: false,
                    autoSize: false
                };
                options.dialogReturnValueCallback = Function.createDelegate(null, closeCallbackAlterarSalario);
                SP.UI.ModalDialog.showModalDialog(options);

                return false;
            } else if ((value === '...') || (value === '0')) {
                return false;
            } else {

                var nivel = value;
                var filial = $.trim($('input[id*=<%= rbFilial.ClientID %>]:checked').val());
                var jornada = $.trim($('input[id*=<%= rbJornada.ClientID %>]:checked').val());
                var classe = $.trim($('#<%= lblClasse.ClientID %>').text());
                var coligadaGerente = $.trim($('#<%= hiddenField_coligadaCentroCusto.ClientID %>').val());

                // Tratamento para solucionar o problema oriundo da tela de Buscar Cargo. 
                // Tem cargo que possui a jornada vazia.
                if ((isNaN(jornada)) || (jornada === '') || (jornada === undefined)) {
                    window.alert('Este cargo não possui jornada definida!');
                    // Retorna com o nivel anterior.
                    $(this).val($('#<%= hiddenField_tb_Nivel.ClientID %>').val());
                    return false;
                } else {
                    if ((classe !== '') && (coligadaGerente !== '')) {
                        alterarSalarioProposto(classe, nivel, jornada, filial, coligadaGerente);
                    }
                }
            }
        });

        // Efeito read-olny campo Filial.
        $('#<%= rbFilial.ClientID %>').click(function () {

            // Código addicionado para adequar ao IE.
            var objRadioRJ = $('#' + $(this).attr('id') + '_0');
            var objRadioSP = $('#' + $(this).attr('id') + '_1');

            if (objRadioRJ.attr('checked') === 'checked') {
                objRadioRJ.attr('checked', true);
                objRadioSP.attr('checked', false);
            } else if (objRadioSP.attr('checked') === 'checked') {
                objRadioRJ.attr('checked', false);
                objRadioSP.attr('checked', true);
            }

            return false;
        });

        // Efeito read-olny.
        $('#<%= rbJornada.ClientID %>').click(function () {

            // Código addicionado para adequar ao IE.
            var objRadio220 = $('#' + $(this).attr('id') + '_0');
            var objRadio180 = $('#' + $(this).attr('id') + '_1');
            var objRadio150 = $('#' + $(this).attr('id') + '_2');

            if (objRadio220.attr('checked') === 'checked') {
                objRadio220.attr('checked', true);
            } else if (objRadio180.attr('checked') === 'checked') {
                objRadio180.attr('checked', true);
            } else if (objRadio150.attr('checked') === 'checked') {
                objRadio150.attr('checked', true);
            }

            return false;
        });
    });

    var closeCallbackBuscarCargo = function (result, returnValue) {

        if (result == SP.UI.DialogResult.OK) {

            if (returnValue !== '') {
            
                var codigo = $.trim(returnValue.split(';')[0]);
                var cargo = $.trim(returnValue.split(';')[1]);
                var jornada = $.trim(returnValue.split(';')[2]);
                var nivel = $.trim(returnValue.split(';')[3]);
                var classe = $.trim(returnValue.split(';')[4]);
                var salario = $.trim(returnValue.split(';')[5]);

                // Limpar a jornada anteiormente já marcada. Pois existem item da tela de Buscar Cargo com jornada vazia.
                $('#<%= rbJornada.ClientID %> input:radio:checked').removeAttr("checked");

                $('#<%= tbCargoPreencher.ClientID %>').val(codigo + ' - ' + cargo);

                // Tem cargo que possui a jornada vazia.
                if ((isNaN(jornada)) || (jornada === '') || (jornada === undefined)) {
                    window.alert('Este cargo não possui jornada definida!');
                } else {
                    $('#<%= rbJornada.ClientID %>').find("input[value='" + jornada + "']").attr("checked", "checked");
                }

                $('#<%= ddlSalNivel.ClientID %>').val(nivel);
                $('#<%= hiddenField_tb_Nivel.ClientID %>').val(nivel);
                $('#<%= lblClasse.ClientID %>').text(classe);
                $('#<%= hiddenField_tb_Classe.ClientID %>').val(classe);
                $('#<%= tbSalario.ClientID %>').val(salario);
                
                return false;
            }
            else if (result == SP.UI.DialogResult.cancel) {
                // Não faz nada!
            }
        }
    }

    var closeCallbackAlterarSalario = function (result, returnValue) {
        if (result == SP.UI.DialogResult.OK) {
            if (returnValue !== '') {
                var salario = $.trim(returnValue.replace(';', ''));

                if ($.trim(salario.replace('R$', '')).length > 0) {
                    $('#<%= tbSalario.ClientID %>').val(salario);
                }
            }
        }
    }

    var alterarSalarioProposto = function (classe, nivel, jornada, filial, coligadaGerente) {
    $.ajax({
        async: false,
        url: getRootSiteUrl() + '_layouts/Cit.Globosat.Services/Api.ashx',
        data: { type: 'component', resource: 'Formulario', method: 'BuscaSalarioProposto', ParamClasse: classe, ParamNivel: nivel, ParamJornada: jornada, ParamFilial: filial, ParamColigadaGerente: coligadaGerente },
        type: 'POST',
        dataType: 'json',
        success: function (data) {
                $('#<%= tbSalario.ClientID %>').val(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                if (xhr.status === 404) {
                    logError('Url inexistente!', 'Um erro ocorreu ao executar a function: alterarSalarioProposto.', 'warn', thrownError);
                } else {
                    logError('Erro inesperado!', 'Um erro ocorreu ao executar a function: alterarSalarioProposto.', 'error', thrownError);
                }
            }
        });
    }

    var printPDF = function () {

        // Para os raddionbuttons que não possuem postback é preciso adicionar 
        // o atributo de checked nos campos que o usuário selecionou.
        
        // Motivo
        if ($('input[id*="<%= rbMotivo.ClientID %>"]:checked').val() !== undefined) {
            $('input:radio[value=' + $('input[id*="<%= rbMotivo.ClientID %>"]:checked').val() + ']').attr('checked', 'checked');
        }

        // Tipo Vaga
        if ($('input[id*="<%= rbTipoVaga.ClientID %>"]:checked').val() !== undefined) {
            $('input:radio[value=' + $('input[id*="<%= rbTipoVaga.ClientID %>"]:checked').val() + ']').attr('checked', 'checked');
        }

        // Orçado
        if ($('input[id*="<%= rbOrcado.ClientID %>"]:checked').val() !== undefined) {
            $('input:radio[value=' + $('input[id*="<%= rbOrcado.ClientID %>"]:checked').val() + ']').attr('checked', 'checked');
        }

        // Post Back does not work after writing files to response in ASP.NET.
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);

        var printPDF = '<HTML><Head><Title></Title>';
        printPDF += '<link rel="stylesheet" type="text/css" href="/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/ReqPessoal/ReqPessoal.css" />';
        printPDF += '<link rel="stylesheet" type="text/css" href="/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/ReqPessoal/PrintToPDF.css" />';
        printPDF += '</Head><Body>';
        printPDF += $('#divForm').html();
        printPDF += '</Body></HTML>';

        document.getElementById('<%= hiddenFieldPDF.ClientID %>').value = printPDF;

        return true;
    }
</script>
<link rel="stylesheet" type="text/css" href="/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/ReqPessoal/ReqPessoal.css" />
<asp:HiddenField runat="server" ID="hiddenFieldPDF" />
<div id="divForm">
    <div id="divBotoes">
        <table id="tableButtons" align="center" border="0" class="tableFormulario">
            <tr>
                <td style="text-align: right;">
                    <asp:ImageButton runat="server" ID="imageButtonVoltar" ImageUrl="~/_layouts/images/Cit.Globosat.Base/icon_back.jpg" ToolTip="Voltar" onclick="imageButtonVoltar_Click" />
                    <asp:ImageButton runat="server" ID="imageButtonImprimir" ImageUrl="~/_layouts/images/Cit.Globosat.Base/print_icon_disable.jpg" onclick="imageButtonImprimir_Click" />
                    <asp:ImageButton runat="server" ID="imageButtonGerarPDF" ImageUrl="~/_layouts/images/Cit.Globosat.Base/pdf_icon_disable.jpg" OnClick="imageButtonGerarPDF_Click" OnClientClick="javascript:printPDF();" />
                </td>
            </tr>
        </table>
    </div>
    <table align="center" class="tableFormulario">
        <tr>
            <td colspan="1" class="tdImage">
                <asp:Image runat="server" ID="imageLogo" style="border-width:0px;width: 113px;" />
            </td>
            <th align="center" colspan="3" class="gradeTd">
                <asp:Label runat="server" CssClass="titulo" Text="REQUISIÇÃO DE PESSOAL"></asp:Label>
            </th>
        </tr>
    </table>
    <table align="center" class="tableFormulario" width="100%">
        <tr valign="top">
            <td colspan="2" width="50%" class="gradeTd">
                <asp:Label ID="lblddlCentroCusto" Text="CENTRO DE CUSTO:" runat="server"></asp:Label>
                <br />
                <asp:DropDownList Width="98%" Height="99%" ID="ddlCentroCusto" runat="server" OnSelectedIndexChanged="ddlCentroCusto_SelectedIndexChanged" AutoPostBack="true" CssClass="dropdownlist"
                    Font-Size="7.5pt" />
            </td>
            <td colspan="1" width="25%" class="gradeTd">
                <asp:Label ID="lblDtRequisicao" Text="DATA DA REQUISIÇÃO:" runat="server"></asp:Label><br />
                <asp:Label runat="server" Text="" ID="lblDataRequisicao"></asp:Label>
            </td>
            <td colspan="1" width="25%" class="gradeTd">
                <asp:Label ID="lblTbDiretoria" Text="DIRETORIA:" runat="server"></asp:Label>
                <br />
                <asp:Label runat="server" ID="lblDiretoria" Text=""></asp:Label>
            </td>
        </tr>
        <tr valign="top">
            <td colspan="3" width="75%" class="gradeTd">
                <asp:Label ID="lblTbCargoPreencher" runat="server" Text="CARGO A SER PREENCHIDO:"></asp:Label>
                <br />
                <asp:TextBox ID="tbCargoPreencher" Width="79%" runat="server" CssClass="input1" Font-Names="Tahoma" Font-Size="7.5pt" Height="19px"></asp:TextBox>
                &nbsp;<asp:Button ID="btnBuscar" runat="server" Text="Novo Cargo" Width="100px" Height="21px" />
            </td>
            <td colspan="1" width="25%" class="gradeTd">
                <asp:Label ID="lblDtDataInicio" runat="server" Text="DATA DE INÍCIO:"></asp:Label>
                <SharePoint:DateTimeControl runat="server" DateOnly="true" ID="dtDataInicio" LocaleId="1046" CssClassTextBox="input1" />
            </td>
        </tr>
        <tr valign="top">
            <td colspan="3" width="75%" class="gradeTd">
                <asp:Label ID="lblTbFuncionarioSubstituido" runat="server" Text="EM SUBSTITUIÇÃO À:"></asp:Label>
                <br />
                <asp:TextBox ID="textBoxFuncSubstituido" Width="99%" runat="server" CssClass="input1" Font-Names="Tahoma" Font-Size="7.5pt" Height="19px"></asp:TextBox>
                <br />
                <br />
                <br />
                <br />
            </td>
            <td colspan="1" width="25%" class="gradeTd">
                <asp:Label ID="lblRbMotivo" runat="server" Text="MOTIVO:"></asp:Label>
                <asp:RadioButtonList ID="rbMotivo" runat="server" Width="99%" RepeatColumns="2" CssClass="label">
                    <asp:ListItem Value="desligado">DESLIGADO</asp:ListItem>
                    <asp:ListItem Value="transferido">TRANSFERIDO</asp:ListItem>
                    <asp:ListItem Value="promovido">PROMOVIDO</asp:ListItem>
                    <asp:ListItem Value="ferias">FÉRIAS</asp:ListItem>
                    <asp:ListItem Value="licenca">LICENÇA</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr valign="top">
            <td colspan="3" width="75%" class="gradeTd">
                <asp:Label ID="lblRbTipoContrato" runat="server" Text="TIPO DE CONTRATO:"></asp:Label>
                <table>
                    <tr>
                        <td width="45%">
                            <br />
                            <asp:RadioButtonList ID="rbTipoContrato" runat="server" Width="99%" OnSelectedIndexChanged="rbTipoContrato_SelectedIndexChanged" AutoPostBack="true" CssClass="label">
                                <asp:ListItem Value="indeterminado">PRAZO INDETERMINADO</asp:ListItem>
                                <asp:ListItem Value="determinado">PRAZO DETERMINADO ATÉ:</asp:ListItem>
                                <asp:ListItem Value="temporario">TEMPORÁRIO ATÉ:</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td width="45%">
                            <table width="100%" style="height: 76px">
                                <tr>
                                    <td class="style3">
                                        <br />
                                        <br />
                                        <SharePoint:DateTimeControl runat="server" SelectedDate="" DateOnly="true" ID="dtPrazoDeterminado" Enabled="false" LocaleId="1046" CssClassTextBox="input1" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style3">
                                        <SharePoint:DateTimeControl runat="server" SelectedDate="" DateOnly="true" ID="dtTemporario" Enabled="false" LocaleId="1046" CssClassTextBox="input1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
            <td colspan="1" width="25%" class="gradeTd">
                <asp:Label ID="lblRbTipoVaga" runat="server" Text="TIPO DE VAGA:"></asp:Label>
                <asp:RadioButtonList ID="rbTipoVaga" Width="99%" runat="server" CssClass="label">
                    <asp:ListItem Value="nova">NOVA</asp:ListItem>
                    <asp:ListItem Value="substituicao">SUBSTITUIÇÃO</asp:ListItem>
                    <asp:ListItem Value="renovacao">RENOVAÇÃO TEMP/PD</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr valign="top">
            <td colspan="3" width="75%" class="gradeTd">
                <asp:Label ID="lblTbCandidatoSelecionado" runat="server" Text="NOME DO CANDIDATO SELECIONADO:"></asp:Label>
                <br />
                <asp:TextBox ID="tbCandidatoSelecionado" Width="99%" runat="server" CssClass="input1" Font-Names="Tahoma" Font-Size="7.5pt" Height="20px"></asp:TextBox>
            </td>
            <td colspan="1" width="25%" class="gradeTd">
                <asp:Label ID="lblRbOrcado" runat="server" Text="ORÇADO?:"></asp:Label>
                <asp:RadioButtonList ID="rbOrcado" runat="server" Width="99%" RepeatColumns="2" CssClass="label">
                    <asp:ListItem>SIM</asp:ListItem>
                    <asp:ListItem>NÃO</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr valign="top">
            <td colspan="1" width="25%" class="gradeTd">
                <asp:Label ID="lblTbSalario" runat="server" Text="SALÁRIO:"></asp:Label>
                <br />
                <asp:TextBox ID="tbSalario" Width="95%" runat="server" CssClass="input1" Font-Names="Tahoma" Font-Size="7.5pt" Height="19px"></asp:TextBox>
            </td>
            <td width="25%" class="gradeTd">
                <table width="100%">
                    <tr valign="top">
                        <td align="left">
                            <asp:Label ID="lbltbClasse" runat="server" Text="CLASSE SALARIAL"></asp:Label>:
                        </td>
                        <td align="right">
                            <asp:Label ID="Label1" runat="server" Text="NIVEL"></asp:Label>:
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center;">
                            <asp:Label runat="server" ID="lblClasse" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:DropDownList Width="50px" ID="ddlSalNivel" runat="server" CssClass="dropdownlist" />
                        </td>
                    </tr>
                </table>
            </td>
            <td colspan="1" width="25%" class="gradeTd">
                <asp:Label ID="lblRbJornada" runat="server" Text="JORNADA:"></asp:Label>
                <asp:RadioButtonList ID="rbJornada" runat="server" Width="99%" RepeatColumns="3" CssClass="label">
                    <asp:ListItem Value="220">220H</asp:ListItem>
                    <asp:ListItem Value="180">180H</asp:ListItem>
                    <asp:ListItem Value="150">150H</asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td colspan="1" width="25%" class="gradeTd">
                <asp:Label ID="lblRbFilial" runat="server" Text="LOCAL DE TRABALHO:"></asp:Label>
                <asp:RadioButtonList ID="rbFilial" runat="server" Width="99%" RepeatColumns="2" CssClass="label">
                    <asp:ListItem Value="SP" Text="SP"></asp:ListItem>
                    <asp:ListItem Value="RJ" Text="RJ"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="border: thin solid #000000; text-align: center">
                <asp:Label ID="lblTbObservacao" runat="server" CssClass="textoCaixaAlta" Text="OBSERVAÇÃO"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="border-style: solid solid none solid; border-width: thin; border-color: #000000;">
                <asp:TextBox ID="tbObservacao" Width="99%" Height="75px" runat="server" TextMode="MultiLine" MaxLength="300" Rows="5" CssClass="textarea" Font-Size="7.5pt"></asp:TextBox>
            </td>
        </tr>
    </table>
    <table class="tableFormulario" align="center">
        <tr>
            <td colspan="4" style="border: thin solid #000000; text-align: center">
                <asp:Label ID="lblTbResumoResponsabilidades" runat="server" CssClass="textoCaixaAlta" Text="RESUMO DAS RESPONSABILIDADES E ATRIBUIÇÕES DO CARGO:"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="border: thin solid #000000;">
                <asp:TextBox ID="tbResumoResponsabilidades" Width="99%" Height="75px" TextMode="MultiLine" MaxLength="300" Rows="5" runat="server" CssClass="textarea" Font-Size="7.5pt"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="border: thin solid #000000; text-align: center">
                <asp:Label ID="lblTbJustificativa" runat="server" CssClass="textoCaixaAlta" Text="JUSTIFICATIVA (NO CASO DE AUMENTO DE QUADRO):"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="border: thin solid #000000;">
                <asp:TextBox ID="tbJustificativa" Width="99%" Height="75px" TextMode="MultiLine" MaxLength="300" Rows="5" runat="server" CssClass="textarea" Font-Size="7.5pt"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="border: thin solid #000000; text-align: center">
                <asp:Label ID="lblTbParecerRH" runat="server" CssClass="textoCaixaAlta" Text="PARECER DO RECURSOS HUMANOS:"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="border: thin solid #000000;">
                <asp:TextBox runat="server" ID="tbParecerRH" Width="99%" Height="75px" TextMode="MultiLine" MaxLength="300" Rows="5" CssClass="textarea" Font-Size="7.5pt" />
            </td>
        </tr>
        <tr>
            <td colspan="4" style="border: thin solid #000000; text-align: center">
                <asp:Label ID="lblTbParecerRemuneracao" runat="server" CssClass="textoCaixaAlta" Text="PARECER ÁREA DE REMUNERAÇÃO:"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="border: thin solid #000000; text-align: center">
                <asp:TextBox ID="tbParecerRemuneracao" Width="99%" Height="75px" TextMode="MultiLine" MaxLength="300" Rows="5" runat="server" CssClass="textarea" Font-Size="7.5pt"></asp:TextBox>
            </td>
        </tr>
    </table>
    <table class="tableFormulario" align="center" border="1">
        <tr valign="top">
            <td style="text-align: center;">
                <asp:Label runat="server" ID="labelRequisitante" Text="REQUISITANTE"></asp:Label>
            </td>
            <td style="text-align: center;">
                <asp:Label runat="server" ID="labelDiretoriaArea" Text="DIRETORIA DA ÁREA"></asp:Label>
            </td>
            <td style="text-align: center;">
                <asp:Label runat="server" ID="labelRecursosHumanos" Text="RECURSOS HUMANOS"></asp:Label>
            </td>
            <td style="text-align: center;">
                <asp:Label runat="server" ID="labelDiretoriaGestao" Text="DIRETORIA GESTÃO"></asp:Label>
            </td>
        </tr>
        <tr align="center" class="gradeTd">
            <td align="center" colspan="1">
                <asp:Label Width="80%" runat="server" ID="lblRequisitante" Font-Size="5"></asp:Label>
                <br />
                <br />
                Ass:
            </td>
            <td colspan="1">
                <asp:TextBox runat="server" ID="textBoxDiretoriaArea" CssClass="input1"></asp:TextBox>
                <br />
                <br />
                Ass:
            </td>
            <td colspan="1">
                &nbsp;
                <br />
                <br />
                Ass:
            </td>
            <td colspan="1">
                &nbsp;
                <br />
                <br />
                Ass:
            </td>
        </tr>
        <tr>
            <td colspan="1" class="gradeTd" nowrap="nowrap">
                <SharePoint:DateTimeControl ID="dtAssRequisitante" runat="server" DateOnly="true" LocaleId="1046" CssClassTextBox="input1" />
            </td>
            <td colspan="1" class="gradeTd" width="25%" nowrap="nowrap">
                <SharePoint:DateTimeControl ID="dtAssDiretoriaArea" runat="server" DateOnly="true" LocaleId="1046" CssClassTextBox="input1" />
            </td>
            <td class="gradeTd" width="25%" nowrap="nowrap">
                <SharePoint:DateTimeControl ID="dtAssRH" runat="server" DateOnly="true" LocaleId="1046" CssClassTextBox="input1" />
            </td>
            <td class="gradeTd" width="25%" nowrap="nowrap">
                <SharePoint:DateTimeControl ID="dtAssDiretoriaGestao" runat="server" DateOnly="true" LocaleId="1046" CssClassTextBox="input1" />
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField runat="server" ID="hiddenField_tb_DepartamentoArea" />
<asp:HiddenField runat="server" ID="hiddenField_tb_CentroCusto" />
<asp:HiddenField runat="server" ID="hiddenField_tb_Funcionario" />
<asp:HiddenField runat="server" ID="hiddenField_tb_Nivel" />
<asp:HiddenField runat="server" ID="hiddenField_tb_Classe" />
<asp:HiddenField runat="server" ID="hiddenField_tb_Coligada" />
<asp:HiddenField runat="server" ID="hiddenField_coligadaCentroCusto" />
<asp:HiddenField runat="server" ID="hiddenField_strJornada" />
<asp:HiddenField runat="server" ID="hiddenField_PrintPDf" />
