<%@ Assembly Name="Globosat.Remuneracao.CustomFields, Version=1.0.0.0, Culture=neutral, PublicKeyToken=a0be42e87db1e8c3" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, 
PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" 
Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, 
PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" 
Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, 
PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" 
Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, 
PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" 
Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, 
Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" %>
<SharePoint:RenderingTemplate ID="CentroCustoFieldControl" runat="server">
    <Template>
    <table class="ms-form" width="100%">
        <tr>
            <td>
                <asp:DropDownList ID="CentroCustoDropDownList" runat="server" >
                    
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="CentroCustoLabel" runat="server" ForeColor="Red">
                    
                </asp:Label>
            </td>
        </tr>
    </table>
    </Template>
</SharePoint:RenderingTemplate>
