<%@ Page Title="" Language="C#" MasterPageFile="~/Portal/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="ValidarSeguimiento1.aspx.cs" Inherits="PortalTrabajadores.Portal.PeriodoExtra" %>

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
            <div id="Container_UpdatePanel1" runat="server" visible="true">
                <asp:GridView ID="gvEmpleadosAsociados" runat="server" AutoGenerateColumns="false" OnRowCommand="gvEmpleadosAsociados_RowCommand" OnRowDataBound="gvEmpleadosAsociados_RowDataBound">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="Cedula_Empleado" HeaderText="Cedula Empleado" SortExpression="Cedula_Empleado" />
                        <asp:BoundField DataField="Nombres_Completos_Empleado" HeaderText="Nombre" />
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnEvaluar" runat="server" ImageUrl="~/Img/edit.gif" CommandArgument='<%#Eval("idJefeEmpleado") + ";" + Eval("Cedula_Empleado")%>' CommandName="Evaluar" />
                                <asp:ImageButton ID="btnOk" runat="server" ImageUrl="~/Img/ok.gif" Visible="false" Enabled="false" />
                                <asp:ImageButton ID="btnRevisar" runat="server" ImageUrl="~/Img/search.gif" CommandArgument='<%#Eval("idJefeEmpleado") + ";" + Eval("Cedula_Empleado")%>' CommandName="Revisar" Visible="false" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
            </div>
            <div id="Container_UpdatePanelObservaciones" runat="server" visible="false">
                <asp:GridView ID="gvObservaciones" runat="server" AutoGenerateColumns="false">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="Descripcion" HeaderText="Observaciones" SortExpression="Descripcion" />
                        <asp:BoundField DataField="Cedula" HeaderText="Cedula" SortExpression="Cedula" />
                    </Columns>
                </asp:GridView>
            </div>
            <div id="Container_UpdatePanel2" runat="server" visible="false">
                <asp:GridView ID="gvObjetivosCreados" runat="server" AutoGenerateColumns="false">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="Descripcion" HeaderText="Objetivos" SortExpression="Objetivos" />
                        <asp:BoundField DataField="SegDescripcion" HeaderText="Seguimiento" SortExpression="SegDescripcion" />
                        <asp:BoundField DataField="SegMeta" HeaderText="Avance Meta" SortExpression="SegMeta" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button ID="BtnAceptar" runat="server" Text="Aceptar Seguimiento" Visible="false" OnClick="BtnAceptar_Click" />
                <asp:Button ID="BtnRechazar" runat="server" Text="Rechazar y realizar observación" Visible="false" OnClick="BtnRechazar_Click" />
                <asp:Button ID="BtnRegresar" runat="server" Text="Regresar" OnClick="BtnRegresar_Click" />
            </div>
            <div id="Container_UpdatePanel3" runat="server" visible="false">
                <br />
                <table id="TablaDatos">
                    <tr>
                        <th>Observaciones</th>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtObservacion" runat="server" TextMode="MultiLine" MaxLength="200" Height="60px" Width="180px" />
                        </td>
                    </tr>
                    <tr class="ColorOscuro">
                        <td class="BotonTablaDatos">
                            <asp:Button ID="BtnGuardar" runat="server" Text="Enviar" OnClick="BtnGuardar_Click" /></td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="gvEmpleadosAsociados" />
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
