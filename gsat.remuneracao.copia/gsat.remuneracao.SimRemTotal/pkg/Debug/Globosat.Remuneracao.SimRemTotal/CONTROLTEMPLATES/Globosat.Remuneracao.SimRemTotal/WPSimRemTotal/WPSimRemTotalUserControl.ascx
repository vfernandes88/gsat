<%@ Assembly Name="Globosat.Remuneracao.SimRemTotal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=baf3531ff51e00d3" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPSimRemTotalUserControl.ascx.cs"
    Inherits="Globosat.Remuneracao.SimRemTotal.WPSimRemTotal.WPSimRemTotalUserControl" %>
    <%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<table width="100%"><tr><td style="width:650px">
<asp:Label runat="server" ID="lblTabelaSimulador"></asp:Label>
</td>
<td>
<table>
<tr><td>
<asp:RadioButtonList ID="rblEbitda" runat="server" Font-Bold="True" 
                Font-Names="Arial" Font-Size="Small" RepeatDirection="Horizontal" 
                Visible="false" AutoPostBack="True" RepeatLayout="Flow">
            <asp:ListItem Value="100" Selected="True">Ebitda 100%</asp:ListItem>
            <asp:ListItem Value="130">Ebitda 130%</asp:ListItem>
            </asp:RadioButtonList>
</td></tr>
    <tr>
        <td>
            <asp:Chart ID="ChartSimulador" runat="server" Width="500px" >
                <series>
                        <asp:Series Name="Rem.Fixa" XValueMember="X" YValueMembers="Y" Color="#66CCFF"
                            IsVisibleInLegend="false" ChartType="Pie">
                        </asp:Series>
                       
                    </series>
                <chartareas>
                        <asp:ChartArea Name="ChartArea1" Area3DStyle-Enable3D="true" >
                            <AxisX LineColor="DarkGray">
                                <MajorGrid LineColor="LightGray" />
                            </AxisX>
                            <AxisY LineColor="DarkGray">
                                <MajorGrid LineColor="LightGray" />
                            </AxisY>

<Area3DStyle Enable3D="True"></Area3DStyle>
                        </asp:ChartArea>
                    </chartareas>
                <legends>
                        <asp:Legend>
                        </asp:Legend>
                    </legends>
            </asp:Chart>
        </td>
    </tr>
</table>
</td></tr></table>
