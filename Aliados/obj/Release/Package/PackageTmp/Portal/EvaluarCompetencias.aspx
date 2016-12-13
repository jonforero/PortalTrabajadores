<%@ Page Title="" Language="C#" MasterPageFile="~/Portal/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="EvaluarCompetencias.aspx.cs" Inherits="PortalTrabajadores.Portal.EvaluarCompetencias" %>

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
            <div id="Container_UpdatePanel1" runat="server" visible="true">
                <asp:GridView ID="gvEmpleadosAsociados" runat="server" AutoGenerateColumns="false"
                    OnRowCommand="gvEmpleadosAsociados_RowCommand"
                    OnRowDataBound="gvEmpleadosAsociados_RowDataBound">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="Cedula_Empleado" HeaderText="Cedula Empleado" SortExpression="Cedula_Empleado" />
                        <asp:BoundField DataField="Nombres_Completos_Empleado" HeaderText="Nombre" />
                        <asp:BoundField DataField="IdCargos" HeaderText="Cargos" Visible="false" />
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnEvaluar" runat="server"
                                    ImageUrl="~/Img/search.gif"
                                    CommandArgument='<%#Eval("idJefeEmpleado") + ";" + Eval("Cedula_Empleado")%>'
                                    CommandName="Evaluar" ToolTip="Revisar" />
                                <asp:ImageButton ID="btnOk" runat="server"
                                    ImageUrl="~/Img/ok.gif" Visible="false"
                                    CommandArgument='<%#Eval("idJefeEmpleado") + ";" + Eval("Cedula_Empleado")%>'
                                    CommandName="Revisar" />
                                <asp:ImageButton ID="btnAlerta" runat="server"
                                    ImageUrl="~/Img/Alert.png" Visible="false"
                                    Enabled="false" ToolTip="No tiene cargo asociado" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
            </div>
            <div id="Container_UpdatePanel2" runat="server" visible="false">
                <asp:Label ID="lblCargo" runat="server" Text="" CssClass="TextCentrado"></asp:Label>
                <asp:GridView ID="gvCompetencias" runat="server" AutoGenerateColumns="false"
                    OnRowCommand="gvCompetencias_RowCommand"
                    OnRowDataBound="gvCompetencias_RowDataBound">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="Competencia" HeaderText="Competencia" />
                        <asp:BoundField DataField="Conductas" HeaderText="Conductas" />
                        <asp:BoundField DataField="NivelRequerido" HeaderText="Nivel Requerido" />
                        <asp:BoundField DataField="Calificacion" HeaderText="Calificacion" />
                        <asp:BoundField DataField="nvlCompetencia" HeaderText="Nivel Competencia" />
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnCalificar" runat="server"
                                    ImageUrl="~/Img/edit.gif" CommandArgument='<%#Eval("idCompetencia") + ";" + Eval("IdCargos") + ";" + Eval("idEva")%>'
                                    CommandName="Evaluar" ToolTip="Evaluar" />
                                <asp:ImageButton ID="btnFin" runat="server"
                                    ImageUrl="~/Img/ok.gif" Visible="false"
                                    CommandArgument='<%#Eval("idCompetencia") + ";" + Eval("IdCargos") + ";" + Eval("idEva")%>'
                                    CommandName="Editar" />
                                <asp:ImageButton ID="btnPlan" runat="server"
                                    ImageUrl="~/Img/add.gif" Visible="false"
                                    CommandArgument='<%#Eval("idCompetencia") + ";" + Eval("IdCargos") + ";" + Eval("idEva")%>'
                                    CommandName="Plan" ToolTip="No cumple el objetivo, puede crearle un plan de desarrollo" />
                                <asp:ImageButton ID="btnAlerta" runat="server"
                                    ImageUrl="~/Img/Alert.png" Visible="false"
                                    Enabled="false" ToolTip="La competencia no tiene conductas." />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:GridView ID="gvPlanGeneral" runat="server" AutoGenerateColumns="false"
                    OnRowCommand="gvPlanGeneral_RowCommand"
                    OnRowDataBound="gvPlanGeneral_RowDataBound">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="plan" HeaderText="Plan" />
                        <asp:BoundField DataField="fechaCumplimiento" HeaderText="Fecha Cumplimiento" DataFormatString="{0:yyyy/MM/dd}" />
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnSeguimientoG" runat="server"
                                    ImageUrl="~/Img/add.gif" CommandArgument='<%#Eval("idPlanEstrategico")%>'
                                    CommandName="Seguimiento" ToolTip="Seguimiento" />
                                <asp:ImageButton ID="btnEditPlanG" runat="server"
                                    ImageUrl="~/Img/edit.gif" CommandArgument='<%#Eval("idPlanEstrategico")%>'
                                    CommandName="Editar" ToolTip="Editar Plan" />
                                <asp:ImageButton ID="btnFinPlanG" runat="server"
                                    ImageUrl="~/Img/unchecked.png"
                                    CommandArgument='<%#Eval("idPlanEstrategico")%>'
                                    CommandName="Fin" ToolTip="Finalizar Plan" />
                                <asp:ImageButton ID="btnPlanOkG" runat="server"
                                    ImageUrl="~/Img/ok.gif" Visible="false" Enabled="false" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button ID="BtnCargarPlanGeneral" runat="server" Text="Crear Plan General" Visible="false" OnClick="BtnCargarPlanGeneral_Click" />
                <asp:Button ID="BtnAceptar" runat="server" Text="Finalizar" Visible="false" OnClick="BtnAceptar_Click" />
                <asp:Button ID="BtnRegresar" runat="server" Text="Regresar" OnClick="BtnRegresar_Click" />
                <table style="margin-top: 10px;">
                    <tr>
                        <td colspan="2">Para crear un plan de desarrollo general, califique al menos una competencia</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ImageButton ID="btnEval" runat="server" ImageUrl="~/Img/edit.gif" Enabled="false" /></td>
                        <td>Carga el menu para calificar la competencia</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ImageButton ID="btnO" runat="server" ImageUrl="~/Img/ok.gif" Enabled="false" /></td>
                        <td>Indica que la competencia calificada esta por encima del rango impuesto</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ImageButton ID="btnP" runat="server" ImageUrl="~/Img/add.gif" Enabled="false" /></td>
                        <td>Indica que la competencia calificada es menor al rango impuesto, lo que le permite crearle un plan de desarrollo</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ImageButton ID="btnA" runat="server" ImageUrl="~/Img/Alert.png" Enabled="false" /></td>
                        <td>Indica que la competencia no tiene conductas y se debe comunicar con el administrador</td>
                    </tr>
                    <tr>
                        <td>Niveles creados</td>
                        <td>
                            <asp:GridView ID="gvNivelesCreados" CssClass="tablaInfo" runat="server" AutoGenerateColumns="false">
                                <AlternatingRowStyle CssClass="ColorOscuro" />
                                <Columns>
                                    <asp:BoundField DataField="nombre" HeaderText="Nivel" />
                                    <asp:BoundField DataField="rangoMin" HeaderText="Minimo" />
                                    <asp:BoundField DataField="rangoMax" HeaderText="Máximo" />
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="Container_UpdatePanel3" runat="server" visible="false">
                <asp:Label ID="lblCompetenciaG" runat="server" Text="" Style="display: none"></asp:Label>
                <asp:Label ID="lblCalificacionG" runat="server" Text="" Style="display: none"></asp:Label>
                <table id="TablaDatos">
                    <tr>
                        <th colspan="2">Califique la competencia</th>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="lblCompetencia" runat="server" />
                        </td>
                        <td class="CeldaTablaDatos">
                            <asp:Label ID="txtCalificacion" runat="server" />
                        </td>
                    </tr>
                    <tr class="ColorOscuro">
                        <td class="BotonTablaDatos">
                            <asp:Button ID="BtnCalificar" runat="server" Text="Calificar"
                                ValidationGroup="objForm" OnClick="BtnCalificar_Click"
                                OnClientClick="return confirm('Esta seguro que quiere continuar?');return false;" />
                        </td>
                        <td class="BotonTablaDatos">
                            <asp:Button ID="BtnRegresarCal" runat="server" Text="Regresar" OnClick="BtnRegresarCal_Click" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:GridView ID="gvCalConductas" runat="server" AutoGenerateColumns="false"
                    OnRowCommand="gvCalConductas_RowCommand">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="conducta" HeaderText="Conducta" />
                        <asp:BoundField DataField="calificacion" HeaderText="Calificación" />
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnCalificar" runat="server"
                                    ImageUrl="~/Img/edit.gif"
                                    CommandArgument='<%#Eval("idCarConCom") + ";" + Eval("conducta") + ";" + Eval("calificacion") %>'
                                    CommandName="Calificar" ToolTip="Calificar Conducta" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <div id="Container_Conducta" runat="server" visible="false">
                    <br />
                    <table id="TablaDatos">
                        <tr>
                            <th colspan="2">Calificar la conducta</th>
                        </tr>
                        <tr>
                            <td class="CeldaTablaDatos">
                                <asp:Label ID="lblConducta" runat="server" />
                            </td>
                            <td class="CeldaTablaDatos">
                                <asp:TextBox ID="txtCien" runat="server" Text="100" Style="display: none" />
                                <asp:TextBox ID="txtCalConducta" runat="server" MaxLength="3" onkeypress="return ValidaSoloNumeros(event)" />
                                <asp:CompareValidator ID="cValidator"
                                    runat="server"
                                    ErrorMessage="CompareValidator"
                                    ControlToValidate="txtCalConducta"
                                    ControlToCompare="txtCien"
                                    CssClass="MensajeError"
                                    Display="Dynamic"
                                    Operator="LessThanEqual"
                                    Type="Integer"
                                    Text="Error: La conducta no puede ser mayor a 100"
                                    ValidationGroup="objConduc">
                                </asp:CompareValidator>
                                <asp:RequiredFieldValidator ID="rfvConducta"
                                    runat="server"
                                    ErrorMessage="Debe digitar valor"
                                    CssClass="MensajeError"
                                    Display="Dynamic"
                                    ControlToValidate="txtCalConducta"
                                    ValidationGroup="objConduc"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr class="ColorOscuro">
                            <td class="BotonTablaDatos">
                                <asp:Button ID="BtnCalificarConducta" runat="server" Text="Guardar" ValidationGroup="objConduc" OnClick="BtnCalificarConducta_Click" />
                            </td>
                            <td class="BotonTablaDatos">
                                <asp:Button ID="BtnCerrarConducta" runat="server" Text="Cerrar" OnClick="BtnCerrarConducta_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
                <br />
                <table>
                    <tr>
                        <td colspan="2">Para calificar una competencia, primero debe calificar las conductas.</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Img/edit.gif" Enabled="false" /></td>
                        <td>Carga la ventana de calificación de la competencia.</td>
                    </tr>
                </table>
            </div>
            <div id="Container_UpdatePanel4" runat="server" visible="false">
                <asp:Label ID="lblPlanDesarrollo" runat="server" Text="" CssClass="TextCentrado"></asp:Label>
                <asp:GridView ID="gvPlanes" runat="server" AutoGenerateColumns="false"
                    OnRowCommand="gvPlanes_RowCommand"
                    OnRowDataBound="gvPlanes_RowDataBound">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="plan" HeaderText="Plan" />
                        <asp:BoundField DataField="fechaCumplimiento" HeaderText="Fecha Cumplimiento" DataFormatString="{0:yyyy/MM/dd}" />
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="btnSeguimiento" runat="server"
                                    ImageUrl="~/Img/add.gif" CommandArgument='<%#Eval("idPlanEstrategico")%>'
                                    CommandName="Seguimiento" ToolTip="Seguimiento" />
                                <asp:ImageButton ID="btnEditPlan" runat="server"
                                    ImageUrl="~/Img/edit.gif" CommandArgument='<%#Eval("idPlanEstrategico")%>'
                                    CommandName="Editar" ToolTip="Editar Plan" />
                                <asp:ImageButton ID="btnFinPlan" runat="server"
                                    ImageUrl="~/Img/unchecked.png"
                                    CommandArgument='<%#Eval("idPlanEstrategico")%>'
                                    CommandName="Fin" ToolTip="Finalizar Plan" />
                                <asp:ImageButton ID="btnPlanOk" runat="server"
                                    ImageUrl="~/Img/ok.gif" Visible="false" Enabled="false" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button ID="BtnRegCalificar" runat="server" Text="Calificar" OnClick="BtnRegCalificar_Click" />
                <asp:Button ID="BtnCrearPlan" runat="server" Text="Crear nuevo plan" OnClick="BtnCrearPlan_Click" />
                <asp:Button ID="BtnRegCompetencia" runat="server" Text="Regresar" OnClick="BtnCerrarPlanComp_Click" />
            </div>
            <div id="Container_UpdatePanel5" runat="server" visible="false">
                Ingrese un plan de desarrollo
                <table id="TablaDatos">
                    <tr>
                        <th colspan="2">Plan de desarrollo para la competencia</th>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">Plan de Desarrollo
                        </td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtPlan" runat="server" TextMode="MultiLine" MaxLength="200" Height="60px" Width="180px" />
                            <asp:RequiredFieldValidator ID="rfvPlan"
                                runat="server"
                                ErrorMessage="Debe digitar información del Plan"
                                CssClass="MensajeError"
                                Display="Dynamic"
                                ControlToValidate="txtPlan"
                                ValidationGroup="objForm"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">Fecha de cumplimiento
                        </td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtFecha" Style="width: 140px; margin-right: 10px" runat="server" CssClass="jqCalendar" onkeypress="return ValidaSoloNumerosFecha(event)"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFecha"
                                runat="server"
                                ErrorMessage="Debe seleccionar una fecha"
                                CssClass="MensajeError"
                                Display="Dynamic"
                                ControlToValidate="txtFecha"
                                ValidationGroup="objForm"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="ColorOscuro">
                        <td class="BotonTablaDatos" colspan="2">
                            <asp:Button ID="BtnGuardarPlan" runat="server" Text="Crear Plan" ValidationGroup="objForm" OnClick="BtnGuardarPlan_Click" />
                            <asp:Button ID="BtnCerrarPlan" runat="server" Text="Regresar" OnClick="BtnCerrarPlan_Click" Visible="false" />
                            <asp:Button ID="BtnCerrarPlanComp" runat="server" Text="Regresar" OnClick="BtnCerrarPlanComp_Click" Visible="false" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="Container_UpdatePanel6" runat="server" visible="false">
                <asp:GridView ID="gvSeguimiento" runat="server" AutoGenerateColumns="false">
                    <AlternatingRowStyle CssClass="ColorOscuro" />
                    <Columns>
                        <asp:BoundField DataField="seguimiento" HeaderText="Seguimiento" />
                        <asp:BoundField DataField="fecha" HeaderText="Fecha Seguimiento" DataFormatString="{0:yyyy/MM/dd}" />
                    </Columns>
                </asp:GridView>
                <table id="TablaDatos">
                    <tr>
                        <th colspan="2">Ingrese un nuevo Seguimiento</th>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">Seguimiento
                        </td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtSeguimiento" runat="server" TextMode="MultiLine" MaxLength="200" Height="60px" Width="180px" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                                runat="server"
                                ErrorMessage="Debe digitar información del Plan"
                                CssClass="MensajeError"
                                Display="Dynamic"
                                ControlToValidate="txtSeguimiento"
                                ValidationGroup="objForm"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="ColorOscuro">
                        <td class="BotonTablaDatos" colspan="2">
                            <asp:Button ID="BtnGuardarSeg" runat="server" Text="Crear Seguimiento" ValidationGroup="objForm" OnClick="BtnGuardarSeg_Click" />
                            <asp:Button ID="BtnRegresarSeg" runat="server" Text="Regresar" OnClick="BtnRegresarSeg_Click" />
                            <asp:Button ID="BtnRegresarComp" runat="server" Visible="false" Text="Regresar" OnClick="btnRegresarGeneral_Click" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="Container_UpdatePanelPlan1" runat="server" visible="false">
                Ingrese el plan de desarrollo general para el empleado
                <table id="TablaDatos">
                    <tr>
                        <th colspan="2">Plan de desarrollo general</th>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">Plan de Desarrollo
                        </td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtPlanGeneral" runat="server" TextMode="MultiLine" MaxLength="200" Height="60px" Width="180px" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2"
                                runat="server"
                                ErrorMessage="Debe digitar información del Plan"
                                CssClass="MensajeError"
                                Display="Dynamic"
                                ControlToValidate="txtPlanGeneral"
                                ValidationGroup="objForm2"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="CeldaTablaDatos">Fecha de cumplimiento
                        </td>
                        <td class="CeldaTablaDatos">
                            <asp:TextBox ID="txtFechaGeneral" Style="width: 140px; margin-right: 10px"
                                runat="server" CssClass="jqCalendar"
                                onkeypress="return ValidaSoloNumerosFecha(event)"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3"
                                runat="server"
                                ErrorMessage="Debe seleccionar una fecha"
                                CssClass="MensajeError"
                                Display="Dynamic"
                                ControlToValidate="txtFechaGeneral"
                                ValidationGroup="objForm2"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="ColorOscuro">
                        <td class="BotonTablaDatos" colspan="2">
                            <asp:Button ID="btnCrearPlanGeneral" runat="server" Text="Crear Plan"
                                ValidationGroup="objForm2" OnClick="BtnCrearPlanGeneral_Click" />
                            <asp:Button ID="btnRegresarGeneral" runat="server" Text="Regresar"
                                OnClick="btnRegresarGeneral_Click" />
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="gvEmpleadosAsociados" />
            <asp:AsyncPostBackTrigger ControlID="gvCompetencias" />
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

