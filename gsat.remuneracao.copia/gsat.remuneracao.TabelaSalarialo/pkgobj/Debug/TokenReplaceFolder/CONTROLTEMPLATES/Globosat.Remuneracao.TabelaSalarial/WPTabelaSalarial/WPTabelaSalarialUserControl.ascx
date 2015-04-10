<%@ Assembly Name="Globosat.Remuneracao.TabelaSalarial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4f9cbd9e1850446" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPTabelaSalarialUserControl.ascx.cs"
    Inherits="Globosat.Remuneracao.TabelaSalarial.WPTabelaSalarial.WPTabelaSalarialUserControl" %>
<style type="text/css">
    table.stats
    {
        width: 100%;
        font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;
        font-size: 18px;
        color: #fff;
        background-color: #fff;
        border: 1px;
        border-collapse: collapse;
        border-spacing: 0px;
    }
    table.stats td
    {
        color: #000;
        padding: 5px 2px 5px 2px;
        border: 1px #000 solid;
    }
    .style1
    {
        text-align: center;
        font-weight: bold;
    }
 
    .style3
    {
       
        text-align: center;
        font-style: italic;
    }
    
     #valores td, #valores tr
        {
            border: 2px solid #000000;
                padding:8px;
        }
</style>
<table style="width: 100%">
    <tr>
        <td colspan="2">
            <h3>
                Selecione abaixo as opções de tabelas salariais disponíveis.</h3>
        </td>
    </tr>
    <tr>
        <td style="width: 50%" align="left">
            <asp:DropDownList ID="ddlTabelas" runat="server" />
        </td>
        <td align="right" style="width: 50%">
            <!--<asp:ImageButton ID="btnImprimir" runat="server" OnClientClick="javascript:window.print()"
                ImageUrl="~/_layouts/images/EvolucaoSalarial/print_icon.jpg" />-->
        </td>
    </tr>
</table>
<!-- tabela salarial!-->
<table width="100%">
    <tr align="left" valign="top">
        <td>
            <asp:Label ID="table_salarial" runat="server" />
        </td>
    </tr>
</table>
