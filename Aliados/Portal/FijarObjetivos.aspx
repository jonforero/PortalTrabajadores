<%@ Page Title="" Language="C#" MasterPageFile="~/Portal/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="FijarObjetivos.aspx.cs" Inherits="PortalTrabajadores.Portal.Objetivos" %>

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
    <div>
        <p>Recuerde que todos los objetivos deben distribuir el peso y al final este debe sumar 100. 
            Tambien tenga presente que la meta debe ser menor al peso.</p>
        <asp:Label ID="lblInformacion" runat="server"></asp:Label></div>
    <br />
    <asp:UpdateProgress ID="upProgress" DynamicLayout="true" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="loader">
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="Container_UpdatePanel1" runat="server" visible="true" style="margin-top: 15px">
                <asp:GridView ID="gvObjetivosCreados" runat="server" AutoGenerateColumns="false" OnRowCommand="gvObjetivosCreados_RowCommand">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="Descripcion" HeaderText="Objetivos" />
                        <asp:BoundField DataField="Peso" HeaderText="Peso" />
                        <asp:BoundField DataField="Meta" HeaderText="Meta" DataFormatString="{0:P0}" HtmlEncode="false"/>
                        <asp:BoundField DataField="Ano" HeaderText="Año" />
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnUpdate" runat="server" ImageUrl="~/Img/edit.gif" CommandArgument='<%#Eval("idObjetivos")%>' CommandName="Editar" />
                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/Img/delete.gif" CommandArgument='<%#Eval("idObjetivos")%>' CommandName="Eliminar" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button ID="BtnCrear" runat="server" Text="Crear Objetivo" Visible="false" OnClick="BtnCrear_Click" />
                <asp:Button ID="BtnEnviar" runat="server" Text="Enviar Objetivos a jefe" Visible="false" OnClick="BtnEnviar_Click" />
            </div>
            <div id="Container_UpdatePanelBloqueado" runat="server" visible="false">
                <asp:GridView ID="gvObjetivosBloqueados" runat="server" AutoGenerateColumns="false">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="Descripcion" HeaderText="Objetivos" />
                        <asp:BoundField DataField="Peso" HeaderText="Peso" />
                        <asp:BoundField DataField="Meta" HeaderText="Meta" DataFormatString="{0:P0}" HtmlEncode="false"/>
                        <asp:BoundField DataField="Ano" HeaderText="Año" />
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
                    <tr class="ColorOscuro">
                        <th colspan="2">Objetivos</th>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="lblObjetivo" runat="server" Text="Objetivo:" /></td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtObjetivo" runat="server" TextMode="MultiLine" MaxLength="200" Height="60px" Width="180px" />                            
                        </td>
                    </tr>          
                    <tr>
                        <td colspan="2">
                            <asp:RequiredFieldValidator ID="rfvObjetivo" 
                                runat="server"
                                ErrorMessage="Debe digitar valor"
                                CssClass="MensajeError"
                                Display="Dynamic"
                                ControlToValidate="txtObjetivo"
                                ValidationGroup="objForm"></asp:RequiredFieldValidator>
                        </td>
                    </tr>          
                    <tr class="ColorOscuro">
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="lblPeso" runat="server" Text="Peso:" /></td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtPeso" runat="server" MaxLength="3" onkeypress="return ValidaSoloNumeros(event)" />
                            <asp:TextBox ID="txtCien" runat="server" Text="100" style="display:none"/>                            
                        </td>
                    </tr>
                    <tr class="ColorOscuro">
                        <td colspan="2">
                            <asp:CompareValidator ID="CompareValidator1" 
                                runat="server" 
                                ErrorMessage="CompareValidator"
                                ControlToValidate="txtPeso"
                                ControlToCompare="txtCien"
                                CssClass="MensajeError" 
                                Display="Dynamic"
                                Operator="LessThanEqual"
                                Type="Integer"
                                Text="Error: El Peso no puede ser mayor a 100"
                                ValidationGroup="objForm">
                            </asp:CompareValidator>
                            <asp:RequiredFieldValidator ID="rfvPeso" 
                                runat="server"
                                ErrorMessage="Debe digitar valor"
                                CssClass="MensajeError"
                                Display="Dynamic"
                                ControlToValidate="txtPeso"
                                ValidationGroup="objForm"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="lblMeta" runat="server" Text="Meta:" /></td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtMeta" runat="server" onkeypress="return ValidaSoloNumeros(event)" />
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
                            <asp:Button ID="BtnGuardar" runat="server" Text="Guardar" ValidationGroup="objForm" OnClick="BtnGuardar_Click" /></td>
                        <td class="BotonTablaDatos">
                            <asp:Button ID="BtnCancelar" runat="server" Text="Cancelar" OnClick="BtnCancel_Click" /></td>
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
            <asp:AsyncPostBackTrigger ControlID="BtnCrear" />
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
