<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AltFuncCargoVWPUserControl.ascx.cs" Inherits="Cit.Globosat.Remuneracao.Formularios.WebParts.AltFuncCargoVWP.AltFuncCargoVWPUserControl" %>
<%@ Register Assembly="Cit.Globosat.Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=dfe7308c061203c4" Namespace="Cit.Globosat.Controls" TagPrefix="cc1" %>
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
        $('#<%= buttonBuscar.ClientID %>').click(function () {
            var filial = $('input[id*=<%= radioButtonListFilial.ClientID %>]:checked').val();
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
        $('#<%= dropDownListSalNivelProposto.ClientID %>').change(function () {
            var value = $(this).val();
            if ((value === 'I*') || (value === '')) {
                var options = {
                    url: getRootSiteUrl() + '_layouts/Globosat.Remuneracao.CustomPages/AlteraSalarioProposto.aspx',
                    title: 'Alterar Salário Proposto',
                    showClose: true,
                    allowMaximize: false,
                    autoSize: true
                };
                options.dialogReturnValueCallback = Function.createDelegate(null, closeCallbackAlterarSalario);
                SP.UI.ModalDialog.showModalDialog(options);

                return false;
            } else if ((value === '...') || (value === '0')) {
                return false;
            } else {
                var nivel = value;
                var filial = $.trim($('input[id*=<%= radioButtonListFilial.ClientID %>]:checked').val());
                var jornada = $.trim($('#<%= hiddenField_strJornada.ClientID %>').val());
                var classe = $.trim($('#<%= textBoxClasseProposto.ClientID %>').val());
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

                    if ((classe !== '') && (coligadaGerente !== '')) {
                        alterarSalarioProposto(classe, nivel, jornada, filial, coligadaGerente);
                        alterarDiferencaSalario($.trim($('#<%= textBoxSalarioAtual.ClientID %>').val()), $.trim($('#<%= textBoxSalarioProposto.ClientID %>').val()));
                        alterarPercentualAumento($.trim($('#<%= textBoxSalarioAtual.ClientID %>').val()), $.trim($('#<%= textBoxSalarioProposto.ClientID %>').val()));
                        atualizarMotivo($.trim($('#<%= textBoxSalarioAtual.ClientID %>').val().replace('R$', '')), $.trim($('#<%= textBoxSalarioProposto.ClientID %>').val().replace('R$', '')));
                    }
                }
            }
        });

        // Efeito read-olny campo Motivo item Promoção.
        $('#<%= radioButtonListMotivoPromocao.ClientID %>').click(function () {
            return false;
        });

        // Efeito read-olny campo Motivo item Merito.
        $('#<%= radioButtonListMotivoMerito.ClientID %>').click(function () {
            return false;
        });

        // Click do campo Motivo item Reenquadramento.
        $('#<%= radioButtonListMotivoReenquadramento.ClientID %>').click(function () {
            $('#<%= radioButtonListMotivoPromocao.ClientID %>').attr('checked', false);
            $('#<%= radioButtonListMotivoMerito.ClientID %>').attr('checked', false);
        });

        // Efeito read-olny campo Filial.
        $('#<%= radioButtonListFilial.ClientID %>').click(function () {

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
        $('#<%= radioButtonListNovaJornada.ClientID %>').click(function () {

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

    // Função executada ao fechar o modal da página Buscar Cargo.
    var closeCallbackBuscarCargo = function (result, returnValue) {

        if (result == SP.UI.DialogResult.OK) {

            if (returnValue !== '') {

                var codigo = $.trim(returnValue.split(';')[0]);
                var cargo = $.trim(returnValue.split(';')[1]);
                var jornada = $.trim(returnValue.split(';')[2]);
                var nivel = $.trim(returnValue.split(';')[3]);
                var classe = $.trim(returnValue.split(';')[4]);
                var salario = $.trim(returnValue.split(';')[5]);

                // Limpar a jornada anteriormente já marcada. Pois existem item da tela de Buscar Cargo com jornada vazia.
                $('#<%= radioButtonListNovaJornada.ClientID %> input:radio:checked').removeAttr("checked");

                $('#<%= hiddenField_CodigoCargoProposto.ClientID %>').val(codigo);
                $('#<%= labelCodigoCargoProposto.ClientID %>').text($('#<%= hiddenField_CodigoCargoProposto.ClientID %>').val());
                $('#<%= textBoxCargoProposto.ClientID %>').val(cargo);

                // Tem cargo que possui a jornada vazia.
                if ((isNaN(jornada)) || (jornada === '') || (jornada === undefined)) {
                    window.alert('Este cargo não possui jornada definida!');
                } else {
                    $('#<%= radioButtonListNovaJornada.ClientID %>').find("input[value='" + jornada + "']").attr("checked", "checked");

                    if ($('#<%= hiddenField_JornadaAtual.ClientID %>').val() !== jornada) {
                        $('#<%= hiddenField_NovaJornadaDiferenteAtual.ClientID %>').val('SIM');
                    } else {
                        $('#<%= hiddenField_NovaJornadaDiferenteAtual.ClientID %>').val('NÃO');
                    }
                    $('#<%= labelNovaJornadaDiferenteAtual.ClientID %>').text($('#<%= hiddenField_NovaJornadaDiferenteAtual.ClientID %>').val());
                }

                $('#<%= hiddenField_strJornada.ClientID %>').val(jornada);
                $('#<%= dropDownListSalNivelProposto.ClientID %>').val(nivel);
                $('#<%= hiddenField_tb_Nivel.ClientID %>').val(nivel);
                $('#<%= textBoxClasseProposto.ClientID %>').val(classe);
                $('#<%= textBoxSalarioProposto.ClientID %>').val(salario);

                atualizarMotivo($.trim($('#<%= textBoxSalarioAtual.ClientID %>').val().replace('R$', '')), $.trim(salario.replace('R$', '')));
                alterarDiferencaSalario($.trim($('#<%= textBoxSalarioAtual.ClientID %>').val()), $.trim($('#<%= textBoxSalarioProposto.ClientID %>').val()));
                alterarPercentualAumento($.trim($('#<%= textBoxSalarioAtual.ClientID %>').val()), $.trim($('#<%= textBoxSalarioProposto.ClientID %>').val()));
            }
            else if (result == SP.UI.DialogResult.cancel) {
                // Não faz nada.
            }
        }
    }

    // Função executada ao fechar o modal de Alterar Salário.
    var closeCallbackAlterarSalario = function (result, returnValue) {
        if (result == SP.UI.DialogResult.OK) {
            if (returnValue !== '') {
                var salario = $.trim(returnValue.replace(';', ''));

                if ($.trim(salario.replace('R$', '')).length > 0) {
                    $('#<%= textBoxSalarioProposto.ClientID %>').val(salario);
                }

                atualizarMotivo($.trim($('#<%= textBoxSalarioAtual.ClientID %>').val().replace('R$', '')), $.trim(salario.replace('R$', '')));
                alterarDiferencaSalario($.trim($('#<%= textBoxSalarioAtual.ClientID %>').val()), $.trim($('#<%= textBoxSalarioProposto.ClientID %>').val()));
                alterarPercentualAumento($.trim($('#<%= textBoxSalarioAtual.ClientID %>').val()), $.trim($('#<%= textBoxSalarioProposto.ClientID %>').val()));
            }
        }
    }

    // Atualizar o campo Motivo.
    var atualizarMotivo = function (salarioAtual, salarioNovo) {
        // Somente é atualizado se reenquadramento não estiver marcado.
        if (!$('#<%= radioButtonListMotivoReenquadramento.ClientID %>').is(':checked')) {
            if (parseInt(salarioNovo.replace('.', '').replace(',', '')) > parseInt(salarioAtual.replace('.', '').replace(',', ''))) {
                var antigaClasse = $.trim($('#<%= textBoxClasseSalNivel.ClientID %>').val().split('-')[0]);
                var novaClasse = $.trim($('#<%= textBoxClasseProposto.ClientID %>').val());
               
                if (antigaClasse === novaClasse && salarioNovo > salarioAtual) {
                    // Campo Motivo igual a 'Merito'.
                    $('#<%= radioButtonListMotivoPromocao.ClientID %>').attr('checked', false);
                    $('#<%= radioButtonListMotivoMerito.ClientID %>').attr('checked', true);
                    $('#<%= radioButtonListMotivoReenquadramento.ClientID %>').attr('checked', false);
                } else if (novaClasse > antigaClasse) {
                    // Campo Motivo igual a 'Promoção'.
                    $('#<%= radioButtonListMotivoPromocao.ClientID %>').attr('checked', true);
                    $('#<%= radioButtonListMotivoMerito.ClientID %>').attr('checked', false);
                    $('#<%= radioButtonListMotivoReenquadramento.ClientID %>').attr('checked', false);
                } else {
                    // Nemhum valor no campo Motivo.
                    $('#<%= radioButtonListMotivoPromocao.ClientID %>').attr('checked', false);
                    $('#<%= radioButtonListMotivoMerito.ClientID %>').attr('checked', false);
                    $('#<%= radioButtonListMotivoReenquadramento.ClientID %>').attr('checked', false);
                }
            } else {
                // Nemhum valor no campo Motivo.
                $('#<%= radioButtonListMotivoPromocao.ClientID %>').attr('checked', false);
                $('#<%= radioButtonListMotivoMerito.ClientID %>').attr('checked', false);
                $('#<%= radioButtonListMotivoReenquadramento.ClientID %>').attr('checked', false);
            }
        }
    }

    // Atualizar o campo Diferença Salarial.
    var alterarDiferencaSalario = function (salarioAtual, salarioProposto) {
        $.ajax({
            async: false,
            url: getRootSiteUrl() + '_layouts/Cit.Globosat.Services/Api.ashx',
            data: { type: 'component', resource: 'Formulario', method: 'CalcularDiferencaSalarial', SalarioProposto: salarioProposto, SalarioAtual: salarioAtual },
            type: 'POST',
            dataType: 'json',
            success: function (data) {
                $('#<%= textBoxDiferenca.ClientID %>').val(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                if (xhr.status === 404) {
                    logError('Url inexistente!', 'Um erro ocorreu ao executar a function: alterarDiferencaSalario.', 'warn', thrownError);
                } else {
                    logError('Erro inesperado!', 'Um erro ocorreu ao executar a function: alterarDiferencaSalario.', 'error', thrownError);
                }
            }
        });
    }

    // Atualizar o campo Percentual de Aumento.
    function alterarPercentualAumento(salarioAtual, salarioProposto) {
        $.ajax({
            async: false,
            url: getRootSiteUrl() + '_layouts/Cit.Globosat.Services/Api.ashx',
            data: { type: 'component', resource: 'Formulario', method: 'CalcularPercentualAumento', SalarioProposto: salarioProposto, SalarioAtual: salarioAtual },
            type: 'POST',
            dataType: 'json',
            success: function (data) {
                $('#<%= textBoxPercentualAumentoProposto.ClientID %>').val(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                if (xhr.status === 404) {
                    logError('Url inexistente!', 'Um erro ocorreu ao executar a function: alterarPercentualAumento.', 'warn', thrownError);
                } else {
                    logError('Erro inesperado!', 'Um erro ocorreu ao executar a function: alterarPercentualAumento.', 'error', thrownError);
                }
            }
        });
    }

    // Atualizar o campo Salário Proposto.
    var alterarSalarioProposto = function (classe, nivel, jornada, filial, coligadaGerente) {
        $.ajax({
            async: false,
            url: getRootSiteUrl() + '_layouts/Cit.Globosat.Services/Api.ashx',
            data: { type: 'component', resource: 'Formulario', method: 'BuscaSalarioProposto', ParamClasse: classe, ParamNivel: nivel, ParamJornada: jornada, ParamFilial: filial, ParamColigadaGerente: coligadaGerente },
            type: 'POST',
            dataType: 'json',
            success: function (data) {
                $('#<%= textBoxSalarioProposto.ClientID %>').val(data);
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

    var printForm = function (elementId) {
        var printContent = document.getElementById(elementId);
        var windowUrl = '';
        var uniqueName = new Date();
        var windowName = 'Print';
        var printWindow = window.open(windowUrl, windowName, 'resizable=yes,location=0,top=0,scrollbars=auto,width=0,height=0');
        printWindow.document.write('<HTML><Head><Title></Title>');
        printWindow.document.write('<link rel="stylesheet" type="text/css" href="/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/AltFuncCargo/AltFuncCargo.css" />');
        printWindow.document.write('<link rel="stylesheet" type="text/css" href="/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/AltFuncCargo/PrintOnlyForm.css" media="print" />');
        printWindow.document.write('</Head><Body>');
        printWindow.document.write($('#divForm').html());
        printWindow.document.write('</Body></HTML>');
        printWindow.document.close();
        printWindow.focus();
        printWindow.print();
        printWindow.close();
    }

    var printPDF = function () {

        // Post Back does not work after writing files to response in ASP.NET.
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);

        var printPDF = '<HTML><Head><Title></Title>';
        printPDF += '<link rel="stylesheet" type="text/css" href="/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/AltFuncCargo/AltFuncCargo.css" />';
        printPDF += '<link rel="stylesheet" type="text/css" href="/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/AltFuncCargo/PrintPDF.css" />';
        printPDF += '</Head><Body>';
        printPDF += $('#divForm').html();
        printPDF += '</Body></HTML>';

        document.getElementById('<%= hiddenFieldPDF.ClientID %>').value = printPDF;

        return true;
    }
    
</script>
<link rel="stylesheet" type="text/css" href="/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/AltFuncCargo/AltFuncCargo.css" />
<asp:HiddenField runat="server" ID="hiddenFieldPDF" />
<div id="divForm">
    <div id="divBotoes">
        <table id="tableButtons" align="center" border="0" class="tableFormulario">
            <tr>
                <td style="text-align: right;">
                    <asp:ImageButton runat="server" ID="imageButtonVoltar" ImageUrl="~/_layouts/images/Cit.Globosat.Base/icon_back.jpg" ToolTip="Voltar" OnClick="imageButtonVoltar_Click" />
                    <asp:ImageButton runat="server" ID="imageButtonImprimir" ImageUrl="~/_layouts/images/Cit.Globosat.Base/print_icon_disable.jpg" OnClientClick="javascript:printForm();return false;" />
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
                <asp:Label runat="server" ID="labelTitulo" Text="SOLICITAÇÃO PARA ALTERAÇÕES FUNCIONAIS" CssClass="titulo"></asp:Label>
            </th>
        </tr>
    </table>
    <table id="tableBody1" align="center" border="1" class="tableFormulario">
        <tr>
            <td colspan="2">
                <asp:Label runat="server" ID="labelCentroCusto" Text="CENTRO DE CUSTO"></asp:Label>
                <br />
                <asp:DropDownList runat="server" ID="dropDownListCentroCusto" CssClass="dropdownlist" AutoPostBack="true" OnSelectedIndexChanged="dropDownListCentroCusto_SelectedIndexChanged" Width="100%" />
            </td>
            <td colspan="2">
                <asp:Label runat="server" ID="labelFuncionarios" Text="FUNCIONÁRIOS"></asp:Label>
                <br />
                <asp:DropDownList runat="server" ID="dropDownListFuncionarios" CssClass="dropdownlist" AutoPostBack="true" OnSelectedIndexChanged="dropDownListFuncionarios_SelectedIndexChanged" Width="100%" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="labelDataRequisicao" Text="DATA DA REQUISIÇÃO"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxDataRequisicao" CssClass="input1"></asp:TextBox>
            </td>
            <td>
                <asp:Label runat="server" ID="labelDiretoria" Text="DIRETORIA"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxDiretoria" CssClass="input1"></asp:TextBox>
            </td>
            <td>
                <asp:Label runat="server" ID="labelMatricula" Text="MATRÍCULA"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxMatricula" CssClass="input1"></asp:TextBox>
            </td>
            <td>
                <asp:Label runat="server" ID="labelDataAdmissao" Text="DATA ADMISSÃO"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxDataAdmissao" CssClass="input1"></asp:TextBox>
            </td>
        </tr>
    </table>
    <table id="tableBody2" align="center" border="1" class="tableFormulario">
        <tr valign="top">
            <td nowrap="nowrap">
                <asp:CheckBox runat="server" ID="checkBoxTranfCentroCusto" Text="TRANSFERÊNCIA DE CENTRO DE CUSTO" AutoPostBack="true" OnCheckedChanged="checkBoxTranfCentroCusto_CheckedChanged" />
                <br />
                <asp:Label runat="server" ID="labelPara" Text="PARA:"></asp:Label>&nbsp;
                <asp:DropDownList runat="server" ID="dropDownListTransferenciaPara" Width="173px" CssClass="dropdownlist" AutoPostBack="true" OnSelectedIndexChanged="dropDownListTransferenciaPara_SelectedIndexChanged" />
                <br />
                <span style="float: left; margin-top: 4px;">FILIAL</span>
                <asp:RadioButtonList runat="server" ID="radioButtonListFilial" TextAlign="Right" RepeatColumns="3">
                    <asp:ListItem Value="RJ" Text="RJ"></asp:ListItem>
                    <asp:ListItem Value="SP" Text="SP"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td colspan="2">
                <asp:Label runat="server" ID="labelMotivo" Text="MOTIVO"></asp:Label>
                <br />
                <asp:RadioButton runat="server" ID="radioButtonListMotivoPromocao" Text="PROMOÇÃO" />
                <br />
                <asp:RadioButton runat="server" ID="radioButtonListMotivoMerito" Text="MÉRITO" />
                <br />
                <asp:RadioButton runat="server" ID="radioButtonListMotivoReenquadramento" 
                    Text="REENQUADRAMENTO" CssClass="input_reenquadramento" Enabled="false" />
            </td>
            <td>
                <asp:Label runat="server" ID="labelAlteracaoValida" Text="ALTERAÇÃO VÁLIDA A PARTIR DE"></asp:Label>
                <br />
                <cc1:DateTimeCustom runat="server" ID="dateTimeControlAlteracaoValida" CssClassTextBox="input1" ondatechanged="dateTimeControlAlteracaoValida_DateChanged"/>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label runat="server" ID="labelCargoAtual" Text="CARGO ATUAL:"></asp:Label>&nbsp;<asp:Label runat="server" ID="labelCodigoCargoAtual"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxCargoAtual" CssClass="input1"></asp:TextBox>
            </td>
            <td>
                <asp:Label runat="server" ID="labelSalarioAtual" Text="SALÁRIO ATUAL"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxSalarioAtual" CssClass="input1"></asp:TextBox>
            </td>
            <td>
                <asp:Label runat="server" ID="labelClasseSalNivel" Text="CLASSE SAL. /NÍVEL"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxClasseSalNivel" CssClass="input1"></asp:TextBox>
            </td>
            <td>
                <asp:Label runat="server" ID="labelDiferenca" Text="DIFERENÇA (EM R$)"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxDiferenca" CssClass="input1"></asp:TextBox>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label runat="server" ID="labelCargoProposto" Text="CARGO PROPOSTO:"></asp:Label>&nbsp;<asp:Label runat="server" ID="labelCodigoCargoProposto"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxCargoProposto" CssClass="input1"></asp:TextBox>
                <br />
                <asp:Button runat="server" ID="buttonBuscar" Width="100px" Text="Novo Cargo" />
                <asp:Button runat="server" ID="buttonMesmoCargo" Width="100px" Text="Mesmo Cargo" OnClick="buttonMesmoCargo_Click" />
                <br />
                NOVA JORNADA:&nbsp;<asp:Label runat="server" ID="labelNovaJornadaDiferenteAtual" Text="NÃO"></asp:Label>
                <asp:RadioButtonList runat="server" ID="radioButtonListNovaJornada" TextAlign="Right" RepeatColumns="3" OnSelectedIndexChanged="radioButtonListNovaJornada_SelectedIndexChanged">
                    <asp:ListItem Value="220" Text="220H"></asp:ListItem>
                    <asp:ListItem Value="180" Text="180H"></asp:ListItem>
                    <asp:ListItem Value="150" Text="150H"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td nowrap="nowrap">
                <asp:Label runat="server" ID="labelSalarioProposto" Text="SALÁRIO PROPOSTO"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxSalarioProposto" CssClass="input1"></asp:TextBox>
            </td>
            <td nowrap="nowrap">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="labelClasseProposto" Text="CLASSE"></asp:Label>
                        </td>
                        <td style="text-align: right;">
                            &nbsp;<asp:Label runat="server" ID="labelSalNivelProposto" Text="SAL./NÍVEL"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center;">
                            <asp:TextBox runat="server" ID="textBoxClasseProposto" CssClass="input1" Width="20px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            <asp:DropDownList runat="server" ID="dropDownListSalNivelProposto" Width="50px" CssClass="dropdownlist" />
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <asp:Label runat="server" ID="labelPercentualAumento" Text="% DE AUMENTO"></asp:Label>
                <br />
                <asp:TextBox runat="server" ID="textBoxPercentualAumentoProposto" CssClass="input1"></asp:TextBox>
            </td>
        </tr>
    </table>
    <table id="tableHistorico" align="center" border="1" class="tableFormulario">
        <tr>
            <td style="text-align: center;">
                <asp:Label runat="server" ID="labelHistorico" Text="HISTÓRICO SALARIAL (ÚLTIMOS 8 ANOS SEM ACORDO COLETIVO)" CssClass="textoCaixaAlta"></asp:Label>
            </td>
        </tr>
        <tr valign="top">
            <td class="gridImpressao">
                <asp:GridView runat="server" ID="gridViewHistorico" AutoGenerateColumns="false" PageSize="9999" Width="100%" OnRowDataBound="gridViewHistorico_RowDataBound">
                    <HeaderStyle CssClass="gridHeaderColumn" HorizontalAlign="Center" />
                    <RowStyle CssClass="gridRow" />
                    <AlternatingRowStyle CssClass="gridRow" />
                    <Columns>
                        <asp:BoundField DataField="Data" HeaderText="DATA" HeaderStyle-Width="10%" />
                        <asp:BoundField DataField="Salario" HeaderText="SALÁRIO" HeaderStyle-Width="20%" />
                        <asp:BoundField DataField="Percentual" HeaderText="( % )" HeaderStyle-Width="10%" />
                        <asp:BoundField DataField="Motivo" HeaderText="MOTIVO" HeaderStyle-Width="20%" />
                        <asp:BoundField DataField="Funcao" HeaderText="CARGO" HeaderStyle-Width="40%" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <table id="tableJustificativa" align="center" border="1" class="tableFormulario">
        <tr>
            <td style="text-align: center;">
                <asp:Label runat="server" ID="labelJustificativa" Text="JUSTIFICATIVA ÁREA/CANAL SOLICITANTE" CssClass="textoCaixaAlta"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="height: 100px;">
                <asp:TextBox runat="server" ID="textBoxJustificativa" TextMode="MultiLine" 
                    CssClass="textarea" onKeyUp="javascript:Check(this, 408);" onChange="javascript:Check(this, 408);"
                    ontextchanged="textBoxJustificativa_TextChanged2" Width="690px" 
                    Height="123px" MaxLength="650" Rows="6"></asp:TextBox>
                    <script type="text/javascript">
                        function Check(textBox, maxLength) {
                            if (textBox.value.length > maxLength) {
                                alert("O maximo de caracteres permitidos são " + maxLength);
                                textBox.value = textBox.value.substr(0, maxLength);
                            }
                        }        
</script>
            </td>
        </tr>
    </table>
    <table id="tableRecursosHumanos" align="center" border="1" class="tableFormulario">
        <tr>
            <td style="text-align: center;">
                <asp:Label runat="server" ID="labelObservacoes" Text="OBSERVAÇÕES DO RECURSOS HUMANOS" CssClass="textoCaixaAlta"></asp:Label>
            </td>
        </tr>
        <tr valign="top">
            <td style="height: 80px;">
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
    </table>
    <table id="tableParecer" align="center" border="1" class="tableFormulario">
        <tr>
            <td style="text-align: center;">
                <asp:Label runat="server" ID="labelParecer" Text="PARECER DA ÁREA DE REMUNERAÇÃO (RH)" CssClass="textoCaixaAlta"></asp:Label>
            </td>
        </tr>
        <tr valign="top">
            <td style="height: 80px;">
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
    </table>
    <table id="tableFooter" align="center" border="1" class="tableFormulario">
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
        <tr style="height: 22px;">
            <td>
                <asp:Label runat="server" ID="labelNome" Text="Nome:"></asp:Label>&nbsp;
                <asp:Label runat="server" ID="labelNomeRequisitante"></asp:Label>
            </td>
            <td rowspan="2">
                <asp:TextBox runat="server" ID="textBoxDiretoriaArea" CssClass="input1"></asp:TextBox>
            </td>
            <td rowspan="2">
            </td>
            <td rowspan="2">
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="labelAssRequisitante" Text="Ass.:"></asp:Label>
            </td>
        </tr>
        <tr>
            <td nowrap="nowrap">
                <asp:Label runat="server" ID="labelDataRequisitante" Text="DATA ___/___/_____"></asp:Label>
            </td>
            <td nowrap="nowrap">
                <asp:Label runat="server" ID="labelDataDiretoriaArea" Text="DATA ___/___/_____"></asp:Label>
            </td>
            <td nowrap="nowrap">
                <asp:Label runat="server" ID="labelDataRecursosHumanos" Text="DATA ___/___/_____"></asp:Label>
            </td>
            <td nowrap="nowrap">
                <asp:Label runat="server" ID="labelDataDiretoriaGestao" Text="DATA ___/___/_____"></asp:Label>
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField runat="server" ID="hiddenField_tb_DepartamentoArea" />
<asp:HiddenField runat="server" ID="hiddenField_tb_Funcionario" />
<asp:HiddenField runat="server" ID="hiddenField_tb_Nivel" />
<asp:HiddenField runat="server" ID="hiddenField_tb_Classe" />
<asp:HiddenField runat="server" ID="hiddenField_tb_Coligada" />
<asp:HiddenField runat="server" ID="hiddenField_coligadaCentroCusto" />
<asp:HiddenField runat="server" ID="hiddenField_strJornada" />
<asp:HiddenField runat="server" ID="hiddenField_PrintPDf" />
<asp:HiddenField runat="server" ID="hiddenField_JornadaAtual" />
<asp:HiddenField runat="server" ID="hiddenField_CodigoCargoProposto" />
<asp:HiddenField runat="server" ID="hiddenField_NovaJornadaDiferenteAtual" />
