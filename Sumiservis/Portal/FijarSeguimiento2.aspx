﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Portal/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="FijarSeguimiento2.aspx.cs" Inherits="PortalTrabajadores.Portal.FijarSeguimiento2" %>
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
            <div id="Container_UpdatePanel1" runat="server" visible="true" style="margin-top: 15px">
                <asp:GridView ID="gvObjetivosCreados" runat="server" AutoGenerateColumns="false" OnRowCommand="gvObjetivosCreados_RowCommand" OnRowDataBound="gvObjetivosCreados_RowDataBound">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="Descripcion" HeaderText="Objetivos" SortExpression="Descripcion" />
                        <asp:BoundField DataField="Meta" HeaderText="Meta" SortExpression="Meta" DataFormatString="{0:P0}"/>
                        <asp:BoundField DataField="Ano" HeaderText="Año" />
                        <asp:BoundField DataField="SegDescripcion1" HeaderText="Descripción Seg 1" />
                        <asp:BoundField DataField="SegMeta1" HeaderText="Meta Seg 1" DataFormatString="{0:P0}"/>
                        <asp:BoundField DataField="SegDescripcion2" HeaderText="Descripción Seg 2" />
                        <asp:BoundField DataField="SegMeta2" HeaderText="Avance Seg" DataFormatString="{0:P0}"/>
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnSeguimiento" runat="server" ImageUrl="~/Img/edit.gif" CommandArgument='<%#Eval("id_obj") + ";" + Eval("Seguimiento2") + ";" + Eval("SegDescripcion2") + ";" + Eval("SegMeta2")%>' CommandName="Seguimiento" />
                                <asp:ImageButton ID="btnOk" runat="server" ImageUrl="~/Img/ok.gif" Enabled="false" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button ID="BtnEnviar" runat="server" Text="Enviar Objetivos a jefe" Visible="false" OnClick="BtnEnviar_Click" />
            </div>
            <div id="Container_UpdatePanelBloqueado" runat="server" visible="false">
                <asp:GridView ID="gvObjetivosBloqueados" runat="server" AutoGenerateColumns="false">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="Descripcion" HeaderText="Objetivos" SortExpression="Descripcion" />                        
                        <asp:BoundField DataField="Meta" HeaderText="Meta" SortExpression="Meta" DataFormatString="{0:P0}"/>
                        <asp:BoundField DataField="Ano" HeaderText="Año" />
                        <asp:BoundField DataField="SegDescripcion1" HeaderText="Descripción Seg 1" />
                        <asp:BoundField DataField="SegMeta1" HeaderText="Meta Seg 1" DataFormatString="{0:P0}"/>
                        <asp:BoundField DataField="SegDescripcion2" HeaderText="Descripción Seg 2" />
                        <asp:BoundField DataField="SegMeta2" HeaderText="Avance Seg" DataFormatString="{0:P0}"/>
                    </Columns>
                </asp:GridView>
            </div>
            <div id="Container_UpdatePanelObservaciones" runat="server" visible="false">
                <asp:GridView ID="gvObservaciones" runat="server" AutoGenerateColumns="false">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="Descripcion" HeaderText="Observaciones" SortExpression="Descripcion" />
                        <asp:BoundField DataField="Nombre" HeaderText="Usuario" SortExpression="Nombre" />
                    </Columns>
                </asp:GridView>
            </div>
            <div id="Container_UpdatePanel2" runat="server" visible="false">
                <br />
                <table id="TablaDatos">
                    <tr>
                        <th colspan="2">Digite su seguimiento</th>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="lblSeguimiento" runat="server" Text="Seguimiento:" /></td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtSeguimiento" runat="server" TextMode="MultiLine" MaxLength="200" Height="60px" Width="180px" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:RequiredFieldValidator ID="rfvSeguimiento" 
                                runat="server"
                                ErrorMessage="Debe digitar valor"
                                CssClass="MensajeError"
                                Display="Dynamic"
                                ControlToValidate="txtSeguimiento"
                                ValidationGroup="objForm"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="lblMeta" runat="server" Text="Avance de la Meta:" /></td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtMeta" runat="server" onkeypress="return ValidaSoloNumeros(event)"/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:RequiredFieldValidator ID="rfvMeta" 
                                runat="server"
                                ErrorMessage="Debe digitar valor"
                                CssClass="MensajeError"
                                Display="Dynamic"
                                ControlToValidate="txtMeta"
                                ValidationGroup="objForm"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="ColorOscuro">
                        <td class="BotonTablaDatos">
                            <asp:Button ID="BtnGuardar" runat="server" ValidationGroup="objForm" Text="Guardar" OnClick="BtnGuardar_Click" /></td>
                        <td class="BotonTablaDatos">
                            <asp:Button ID="BtnCancel" runat="server" Text="Cancelar" OnClick="BtnCancel_Click" /></td>
                    </tr>
                </table>
            </div>
            <div id="Container_UpdatePanel3" runat="server" visible="false">
                <br />
                <table id="TablaDatos2">
                    <tr>
                        <th>Observaciones</th>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtObservacion" runat="server" TextMode="MultiLine" MaxLength="200" Height="60px" Width="180px" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:RequiredFieldValidator id="rfvObservacion" ValidationGroup="userForm"
                                ControlToValidate="txtObservacion" ErrorMessage="Debe Ingresar una observación" 
                                Display="Dynamic" runat="server" CssClass="MensajeError"/>
                        </td>
                    </tr>
                    <tr class="ColorOscuro">
                        <td class="BotonTablaDatos">
                            <asp:Button ID="BtnGuardarObs" runat="server" Text="Enviar" ValidationGroup="userForm" OnClick="BtnGuardarObs_Click" /></td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="gvObjetivosCreados" />
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