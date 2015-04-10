<%@ Assembly Name="WPMenuRemuneracao, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f5dc0e1983ac8b0c" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPMenuRemuneracaoUserControl.ascx.cs" Inherits="WPMenuRemuneracao.WPMenuRemuneracao.WPMenuRemuneracaoUserControl" %>
<script src="/remuneracao/jquery-1.5.1.min.js" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function () {
        $("#nav-one li").hover(
				function () { $("ul", this).fadeIn("fast"); },
				function () { }
			);
        if (document.all) {
            $("#nav-one li").hoverClass("sfHover");
        }
    });

    $.fn.hoverClass = function (c) {
        return this.each(function () {
            $(this).hover(
					function () { $(this).addClass(c); },
					function () { $(this).removeClass(c); }
				);
        });
    };	  
</script>
<style type="text/css">
    .nav img
    {
        border: none;
    }
    .nav
    {
        list-style-type: none;
        margin: 0px;
        padding: 0px;
        font-family: "Lucida Sans";
        width: 100%;
        display: table;
        border-bottom: 3px solid #506a91;
        background: rgb(255,255,255); /* Old browsers */ /* IE9 SVG, needs conditional override of 'filter' to 'none' */
        background: url(data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiA/Pgo8c3ZnIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgd2lkdGg9IjEwMCUiIGhlaWdodD0iMTAwJSIgdmlld0JveD0iMCAwIDEgMSIgcHJlc2VydmVBc3BlY3RSYXRpbz0ibm9uZSI+CiAgPGxpbmVhckdyYWRpZW50IGlkPSJncmFkLXVjZ2ctZ2VuZXJhdGVkIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSIgeDE9IjAlIiB5MT0iMCUiIHgyPSIwJSIgeTI9IjEwMCUiPgogICAgPHN0b3Agb2Zmc2V0PSIwJSIgc3RvcC1jb2xvcj0iI2ZmZmZmZiIgc3RvcC1vcGFjaXR5PSIxIi8+CiAgICA8c3RvcCBvZmZzZXQ9IjUwJSIgc3RvcC1jb2xvcj0iI2YxZjFmMSIgc3RvcC1vcGFjaXR5PSIxIi8+CiAgICA8c3RvcCBvZmZzZXQ9IjUxJSIgc3RvcC1jb2xvcj0iI2UxZTFlMSIgc3RvcC1vcGFjaXR5PSIxIi8+CiAgICA8c3RvcCBvZmZzZXQ9IjEwMCUiIHN0b3AtY29sb3I9IiNmNmY2ZjYiIHN0b3Atb3BhY2l0eT0iMSIvPgogIDwvbGluZWFyR3JhZGllbnQ+CiAgPHJlY3QgeD0iMCIgeT0iMCIgd2lkdGg9IjEiIGhlaWdodD0iMSIgZmlsbD0idXJsKCNncmFkLXVjZ2ctZ2VuZXJhdGVkKSIgLz4KPC9zdmc+);
        background: -moz-linear-gradient(top,  rgba(255,255,255,1) 0%, rgba(241,241,241,1) 50%, rgba(225,225,225,1) 51%, rgba(246,246,246,1) 100%); /* FF3.6+ */
        background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,rgba(255,255,255,1)), color-stop(50%,rgba(241,241,241,1)), color-stop(51%,rgba(225,225,225,1)), color-stop(100%,rgba(246,246,246,1))); /* Chrome,Safari4+ */
        background: -webkit-linear-gradient(top,  rgba(255,255,255,1) 0%,rgba(241,241,241,1) 50%,rgba(225,225,225,1) 51%,rgba(246,246,246,1) 100%); /* Chrome10+,Safari5.1+ */
        background: -o-linear-gradient(top,  rgba(255,255,255,1) 0%,rgba(241,241,241,1) 50%,rgba(225,225,225,1) 51%,rgba(246,246,246,1) 100%); /* Opera 11.10+ */
        background: -ms-linear-gradient(top,  rgba(255,255,255,1) 0%,rgba(241,241,241,1) 50%,rgba(225,225,225,1) 51%,rgba(246,246,246,1) 100%); /* IE10+ */
        background: linear-gradient(to bottom,  rgba(255,255,255,1) 0%,rgba(241,241,241,1) 50%,rgba(225,225,225,1) 51%,rgba(246,246,246,1) 100%); /* W3C */
        filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#ffffff', endColorstr='#f6f6f6',GradientType=0 ); /* IE6-8 */
    }
    .nav LI
    {
        margin: 0px;
        padding: 5px 0px;
        display: table-cell;
    }
    .nav LI A
    {
        padding: 5px 4px;
        display: block;
        font-family: "Lucida Sans";
        white-space: nowrap;
        color: #000 !important;
        font-size: 10px;
        text-align: center;
        border-right: dotted 1px #ccc;
    }
    .nav LI A:hover
    {
        text-decoration: none;
    }
    #nav-one LI.sfHover A
    {
        background: #fafaff;
        color: #000;
    }
    #nav-one LI:hover UL A
    {
        background: #ffffff;
        color: #000000;
    }
    #nav-one LI.sfHover UL A
    {
        background: #ffffff;
        color: #000000;
    }
    #nav-one LI:hover UL A:hover
    {
        background: #fafaff;
        color: #000;
    }
    #nav-one LI.sfHover UL A:hover
    {
        background: #fafaff;
        color: #000;
    }
    .nav LI:hover UL
    {
        margin-top: 0px;
        display: block;
    }
    
    .nav LI.sfHover ul
    {
        top: 34px;
    }
    .nav ul
    {
        border: #CCC 1px solid;
        position: absolute;
        list-style-type: none;
        margin: 0px;
        padding: 0;
        background: #FFF;
        display: none;
        min-width: 200px;
        text-align: left;
    }
    
    .nav ul LI
    {
        border: 0px;
        padding: 0;
        float: none;
        display: block;
    }
    .nav ul li A
    {
        white-space: nowrap;
        padding: 5px 10px;
        border: none;
        text-align: left;
        font-family: Verdana;
    }
    .nav ul li a:hover
    {
        display: block;
        background: #e1e1e1 !important;
        color: #000;
    }
</style>
<ul class="nav" id="nav-one">
    <li runat="server" id="divHome"><a href="/remuneracoes">PÁGINA INICIAL</a> </li>
    <li runat="server" id="divRegras"><a href="/remuneracoes/paginas/regrasjogoremuneracao.aspx">REGULAMENTO</a> </li>
    <li runat="server" id="divOrganograma"><a href="/remuneracoes/paginas/organogramas.aspx">ORGANOGRAMA</a> </li>
    <li runat="server" id="divDescCargos"><a href="/remuneracoes/paginas/descricaocargossalarios.aspx">DESCRIÇÃO DE CARGOS</a> </li>
    <li runat="server" id="divFormularios"><a href="#">FORMULÁRIOS
        <img src="/_layouts/images/Cit.Globosat.Base/menu-down.gif" /></a>
        <ul>
            <li><a href="/remuneracoes/Paginas/formreqestag.aspx">Requisição de Estagiário</a></li>
            <li><a href="/remuneracoes/Paginas/formreqpessoal.aspx">Requisição de Pessoal</a></li>
            <li><a href="/remuneracoes/paginas/formaltfunccargo.aspx">Alteração Funcional</a></li>
            <li><a onclick="createNewDocumentWithProgID('http://rj2k8shp01/Remuneracoes/Biblioteca Cargos/Forms/Modelo_descricao_cargo.doc', 'http://rj2k8shp01/Remuneracoes/Biblioteca Cargos', 'SharePoint.OpenDocuments', false);return false;"
                href="#">Nova Descrição de Cargo</a></li>
            <li><a href="/Remuneracoes/Biblioteca Cargos/Forms/Instruções Preenchimento.pdf" target="_blank">Instruções de Preenchimento de Descrição de Cargo</a></li>
        </ul>
    </li>
    <li runat="server" id="divEvolucao"><a href="/remuneracoes/paginas/evolucaosalarial.aspx">EVOLUÇÃO SALARIAL</a> </li>
    <li runat="server" id="divTabSalarial"><a href="/remuneracoes/paginas/tabelasalarial.aspx">TABELA SALARIAL</a> </li>
    <li runat="server" id="divFolhaPagto"><a href="/remuneracoes/paginas/folhapagamento.aspx">FOLHA DE PAGAMENTO</a> </li>
    <li runat="server" id="divMetas"><a href="/remuneracoes/paginas/metasFuncionarios.aspx">METAS</a> </li>
    <li runat="server" id="divRemuneracaoVariavel"><a href="#">REMUNERAÇÃO VARIÁVEL
        <img src="/_layouts/images/Cit.Globosat.Base/menu-down.gif" /></a>
        <ul>
            <li><a href="/remuneracoes/paginas/remuneracaovariavel.aspx">Target RV</a></li>
            <li><a href="/remuneracoes/Paginas/remuneracaoVariavelAno.aspx">Valores RV</a></li>
        </ul>
    </li>
    <li runat="server" id="divRelatorios"><a href="#">PRÊMIOS<img src="/_layouts/images/Cit.Globosat.Base/menu-down.gif" alt="down" /></a>
        <ul>
            <li><a href="/remuneracoes/paginas/relatoriopremios.aspx">Prêmios por Executivo</a></li>
			<li id="Li1" runat="server" visible="true"><a href="/remuneracoes/paginas/graficofuncpremios.aspx">Prêmios Consolidado</a></li>
        </ul>
    </li>
    <li runat="server" id="divFaleConosco"><a href="/remuneracoes/paginas/faleconoscoremuneracao.aspx">FALE CONOSCO</a></li>
</ul>
