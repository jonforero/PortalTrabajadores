<%@ Page Title="" Language="C#" MasterPageFile="~/Portal/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="AsignarAreaCargo.aspx.cs" Inherits="PortalTrabajadores.Portal.AsignarAreaCargo" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../Js/jquery-ui.css">
    <script src="../Js/jquery-ui.js"></script>
    <!-- Css para la fecha -->
    <link href="../CSS/CSSCallapsePanel.css" rel="stylesheet" type="text/css" />
    <!-- Js De Los campos de Textos -->
    <script src="../Js/funciones.js" type="text/javascript"></script>    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContainerTitulo" runat="server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <asp:Label ID="lblTitulo" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Container" runat="server">
    <asp:UpdateProgress ID="upProgress" DynamicLayout="true" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="loader">
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <p class="TextoFiltros">
                <asp:Label ID="lblTexto" runat="server" 
                    Text="Antes de continuar debe seleccionar el area y el cargo que desempeña actualmente."
                    Visible="false"></asp:Label>                
            </p>
            <div id="Container_UpdatePanel1">
                <table id="TablaDatos2">
                    <tr>
                        <th colspan="2">Datos del Empleado</th>
                    </tr>
                    <tr class="ColorOscuro">
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="lblId" runat="server" Text="Cedula:" />
                            <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtUser2" runat="server" MaxLength="100" 
                                placeholder="Número" Enabled="false"/>
                        </td>
                    </tr>
                    <tr class="ColorOscuro">
                        <td class="CeldaTablaDatos"><asp:Label ID="lblNombres" runat="server" Text="Nombres:" /></td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtNombres" runat="server" Enabled="false" 
                                Style="text-transform: uppercase" />
                        </td>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos"><asp:Label ID="lblApellidos" runat="server" Text="Apellidos:" /></td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtPrimerApellido" runat="server" Enabled="false"
                                Style="text-transform: uppercase" />
                        </td>
                    </tr> 
                    <tr>
                        <td></td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtSegundoApellido" runat="server" Enabled="false"
                                Style="text-transform: uppercase" />
                        </td>
                    </tr> 
                    <tr class="ColorOscuro">
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="lblArea" runat="server" Text="Área" />
                        </td>
                        <td class="CeldaTablaDatos">
                            <asp:DropDownList ID="ddlArea" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="lblCargo" runat="server" Text="Cargo" />
                        </td>
                        <td class="CeldaTablaDatos">
                            <asp:DropDownList ID="ddlCargo" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="BotonTablaDatos">
                            <asp:Button ID="BtnEditar" runat="server" Text="Guardar Información" ValidationGroup="userForm" OnClick="BtnEditar_Click"/></td>                        
                    </tr>
                </table>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BtnEditar"/>
        </Triggers>         
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Errores" runat="server">
    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
             <asp:Label ID="LblMsj" runat="server" Text="LabelMsjError" Visible="False"></asp:Label>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
