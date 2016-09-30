<%@ Page Title="" Language="C#" MasterPageFile="~/Portal/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="FijarSeguimientoFinal.aspx.cs" Inherits="PortalTrabajadores.Portal.FijarSeguimientoFinal" %>

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
                        <asp:BoundField DataField="SegMeta1" HeaderText="Primer Seg" DataFormatString="{0:P0}" HtmlEncode="false"/>
                        <asp:BoundField DataField="SegMeta2" HeaderText="Segundo Seg" DataFormatString="{0:P0}" HtmlEncode="false"/>
                        <asp:BoundField DataField="MetaEval" HeaderText="Meta Evaluacion" DataFormatString="{0:P0}" HtmlEncode="false"/>
                        <asp:BoundField DataField="ResultadoEval" HeaderText="Resultado" DataFormatString="{0:P0}" HtmlEncode="false"/>                     
                        <asp:BoundField DataField="EvalDescripcion" HeaderText="Evaluación" />
                        <asp:BoundField DataField="Ano" HeaderText="Año" />
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnEvaluacion" runat="server" ImageUrl="~/Img/edit.gif" CommandArgument='<%#Eval("id_obj") + ";" + Eval("CodEvaluacion") + ";" + 
                                Eval("Evaluacion") + ";" + Eval("MetaEval") + ";" + Eval("SegMeta1") + ";" + Eval("SegMeta2") %>' CommandName="Evaluacion" />
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
                <asp:GridView ID="gvObjetivosBloqueados" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvObjetivosBloqueados_RowDataBound">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="Descripcion" HeaderText="Objetivos" SortExpression="Descripcion" />
                        <asp:BoundField DataField="SegMeta1" HeaderText="Primer Seg" DataFormatString="{0:P0}" HtmlEncode="false"/>
                        <asp:BoundField DataField="SegMeta2" HeaderText="Segundo Seg" DataFormatString="{0:P0}"/>
                        <asp:BoundField DataField="MetaEval" HeaderText="Meta Evaluacion" DataFormatString="{0:P0}"/>
                        <asp:BoundField DataField="ResultadoEval" HeaderText="Resultado" DataFormatString="{0:P0}"/>                        
                        <asp:BoundField DataField="EvalDescripcion" HeaderText="Evaluación" />
                        <asp:BoundField DataField="EvalJefe" HeaderText="Evaluación Jefe" />
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
                    <tr>
                        <th colspan="2">Seleccione su calificación</th>
                    </tr>
                    <tr class="ColorOscuro">
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="lblEvaluacion" runat="server" Text="Calificación:" /></td>
                        <td class="CeldaTablaDatos">
                            <asp:DropDownList ID="ddlEvaluacion" runat="server" DataSourceID="sqlEvaluacion" DataTextField="Valor" DataValueField="idCalificacion"></asp:DropDownList>
                            <asp:SqlDataSource ID="sqlEvaluacion" runat="server" ConnectionString='<%$ ConnectionStrings:trabajadoresConnectionString2 %>' ProviderName='<%$ ConnectionStrings:trabajadoresConnectionString2.ProviderName %>' SelectCommand="SELECT idCalificacion, Valor FROM calificacion"></asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="lblMeta" runat="server" Text="Meta Final" /></td>
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
                            <asp:Button ID="BtnGuardar" runat="server" Text="Guardar" ValidationGroup="objForm" OnClick="BtnGuardar_Click" /></td>
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
                    <tr  class="ColorOscuro">
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtObservacion" runat="server" TextMode="MultiLine" MaxLength="200" Height="60px" Width="180px" />
                        </td>
                    </tr>
                    <tr  class="ColorOscuro">
                        <td colspan="2">
                            <asp:RequiredFieldValidator id="rfvObservacion" ValidationGroup="userForm"
                                ControlToValidate="txtObservacion" ErrorMessage="Debe Ingresar una observación" 
                                Display="Dynamic" runat="server" CssClass="MensajeError"/>
                        </td>
                    </tr>
                    <tr>
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
