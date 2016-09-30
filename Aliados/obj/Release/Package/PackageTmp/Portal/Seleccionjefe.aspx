<%@ Page Language="C#"  MasterPageFile="~/Portal/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="Seleccionjefe.aspx.cs" Inherits="PortalTrabajadores.Portal.Seleccionjefe" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <p class="TextoFiltros">
                <asp:Label ID="lblTexto" runat="server" 
                    Text="Su jefe actual ha sido desactivado, por favor seleccione uno nuevo"
                    Visible="false"></asp:Label>                
            </p>
            <div id="Container_UpdatePanel1">
                <table id="TablaDatos">
                    <tr>
                        <th colspan="2">Módulo de Selección de Jefe</th>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="LblCambio" runat="server" Text="Digite Número de Identificación:" />
                        </td>
                        <td class="BotonTablaDatos">
                            <asp:TextBox ID="txtuser" runat="server" MaxLength="15" CssClass="MarcaAgua"
                                ToolTip="Usuario" placeholder="Identificación" onkeypress="return ValidaSoloNumeros(event)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos"></td>
                        <td class="BotonTablaDatos">
                            <asp:Button ID="BtnBuscar" runat="server" Text="Buscar Jefe" OnClick="BtnBuscar_Click" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="Container_UpdatePanel2" runat="server" visible="false">
                <table id="TablaDatos2">
                    <tr>
                        <th colspan="2">Datos del Jefe</th>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="LblDoc" runat="server" Text="Cédula Jefe:" /></td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="TxtDoc" runat="server" Enabled="false" MaxLength="15" /></td>
                    </tr>
                    <tr class="ColorOscuro">
                        <td>
                            <asp:Label ID="LblNombres" runat="server" Text="Nombre Completo Jefe:" /></td>
                        <td>
                            <asp:TextBox ID="TxtNombres" runat="server" MaxLength="100" Style="text-transform: uppercase" Enabled="False" /></td>
                    </tr>                                    
                    <tr class="ColorOscuro">
                        <td class="BotonTablaDatos">
                            
                        </td>
                        <td class="BotonTablaDatos">
                            <asp:Button ID="BtnEditar" runat="server" Text="Guardar Datos" ValidationGroup="form" OnClick="BtnEditar_Click" />
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BtnBuscar" />
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

