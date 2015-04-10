<%@ Assembly Name="Globosat.Remuneracao.TabelaSalarialLista, Version=1.0.0.0, Culture=neutral, PublicKeyToken=e423f659c3f14551" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPTSalarialListaUC.ascx.cs" Inherits="Globosat.Remuneracao.TabelaSalarialLista.WPTabelaSalarialLista.WPTabelaSalarialListaUC" %>
<SharePoint:CssRegistration ID="cssReg" runat="server" Name="/_layouts/Globosat.Remuneracao.TabelaSalarialLista/TabelaSalarial.css">
</SharePoint:CssRegistration>
<asp:Label ID="lblMain" runat="server"></asp:Label>

