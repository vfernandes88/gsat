<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPExibeFuncionariosUserControl.ascx.cs"
    Inherits="Globosat.Remuneracao.EvolucaoSalarial.WPExibeFuncionarios.WPExibeFuncionariosUserControl" %>
<style type="text/css">
    .funcionarios
    {
        font-size: 14px;
    }
    .NaoGestores
    {
        font-size: 14px;
        color: Orange;
    }
    .EvSalarial
    {
        font-size: 14px;
        border: 1;
        border-style: solid;
        background-color: Window;
        margin-left: 50px;
        padding: 8px;
    }
</style>
<script type="text/javascript">   

    function open(dadosColaborador) {
        var options =
      { url: dadosColaborador,
          title: "Mais Detalhes",
          allowMaximize: false,
          showClose: true,
          width: 1100,
          height: 800,
          dialogReturnValueCallback: silentCallback
      };
        SP.UI.ModalDialog.showModalDialog(options);
    }

    function silentCallback(dialogResult, returnValue) {
    }
    function refreshCallback(dialogResult, returnValue) {
        SP.UI.ModalDialog.RefreshPage(SP.UI.DialogResult.OK);
    }</script>

    <script type="text/javascript">
        function OnCheckBoxCheckChanged(evt) {
            var src = window.event != window.undefined ? window.event.srcElement : evt.target;
            var isChkBoxClick = (src.tagName.toLowerCase() == "input" && src.type == "checkbox");
            if (isChkBoxClick) {
                var parentTable = GetParentByTagName("table", src);
                var nxtSibling = parentTable.nextSibling;
                if (nxtSibling && nxtSibling.nodeType == 1)//check if nxt sibling is not null & is an element node 
                {
                    if (nxtSibling.tagName.toLowerCase() == "div") //if node has children 
                    {
                        //check or uncheck children at all levels 
                        CheckUncheckChildren(parentTable.nextSibling, src.checked);
                    }
                }
                //check or uncheck parents at all levels 
                CheckUncheckParents(src, src.checked);
            }
        }
        function CheckUncheckChildren(childContainer, check) {
            var childChkBoxes = childContainer.getElementsByTagName("input");
            var childChkBoxCount = childChkBoxes.length;
            for (var i = 0; i < childChkBoxCount; i++) {
                childChkBoxes[i].checked = check;
            }
        }
        function CheckUncheckParents(srcChild, check) {
            var parentDiv = GetParentByTagName("div", srcChild);
            var parentNodeTable = parentDiv.previousSibling;

            if (parentNodeTable) {
                var checkUncheckSwitch;

                if (check) //checkbox checked 
                {
                    var isAllSiblingsChecked = AreAllSiblingsChecked(srcChild);
                    if (isAllSiblingsChecked)
                        checkUncheckSwitch = true;
                    else
                        return; //do not need to check parent if any(one or more) child not checked 
                }
                else //checkbox unchecked 
                {
                    checkUncheckSwitch = false;
                }

                var inpElemsInParentTable = parentNodeTable.getElementsByTagName("input");
                if (inpElemsInParentTable.length > 0) {
                    var parentNodeChkBox = inpElemsInParentTable[0];
                    parentNodeChkBox.checked = checkUncheckSwitch;
                    //do the same recursively 
                    CheckUncheckParents(parentNodeChkBox, checkUncheckSwitch);
                }
            }
        }
        function AreAllSiblingsChecked(chkBox) {
            var parentDiv = GetParentByTagName("div", chkBox);
            var childCount = parentDiv.childNodes.length;
            for (var i = 0; i < childCount; i++) {
                if (parentDiv.childNodes[i].nodeType == 1) //check if the child node is an element node 
                {
                    if (parentDiv.childNodes[i].tagName.toLowerCase() == "table") {
                        var prevChkBox = parentDiv.childNodes[i].getElementsByTagName("input")[0];
                        //if any of sibling nodes are not checked, return false 
                        if (!prevChkBox.checked) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        //utility function to get the container of an element by tagname 
        function GetParentByTagName(parentTagName, childElementObj) {
            var parent = childElementObj.parentNode;
            while (parent.tagName.toLowerCase() != parentTagName.toLowerCase()) {
                parent = parent.parentNode;
            }
            return parent;
        } 
</script>

<table width="100%">
    <tr>
        <td valign="top" align="center">
            <table>
                <tr>
                    <td>
                        <div id="divTitulo" runat="server">
                            <h2>
                                Selecione o centro de custo e/ou funcionário para visualizar sua evolução salarial.</h2>
                        </div>
                        <div style="float:right">
                        <asp:ImageButton ID="btnImprimir" runat="server" OnClientClick="javascript:window.print()" ImageUrl="~/_layouts/images/EvolucaoSalarial/print_icon.jpg" />
                        <asp:ImageButton ID="emailButton" ImageUrl="~/_layouts/images/EvolucaoSalarial/mail_icon.jpg"  ToolTip="Enviar por email" runat="server" OnClick="emailButton_Click" />
                        </div>
                    </td>        
                </tr>
                <tr>
                    <td align="center">
                        <table width="400px">
                            <tr>
                                <td align="left">
                                    <asp:Label ID="lblValidacao" runat="server" CssClass="NaoGestores" />
                                    <asp:TreeView ID="treeview" runat="server" OnTreeNodePopulate="treeview_TreeNodePopulate"
                                        SelectedNodeStyle-BackColor="LightGray"
                                        SelectedNodeStyle-ForeColor="Black" HoverNodeStyle-BackColor="LightGray" RootNodeStyle-ForeColor="Black" RootNodeStyle-NodeSpacing="2px" NodeStyle-ForeColor="GrayText" ShowCheckBoxes="All" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td> 
    </tr>
</table>
