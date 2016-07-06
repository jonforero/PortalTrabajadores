using MySql.Data.MySqlClient;
using PortalTrabajadores.Class;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PortalTrabajadores.Portal
{
    public partial class EvaluarCompetencias : System.Web.UI.Page
    {
        string CnTrabajadores = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString"].ConnectionString.ToString();
        string CnCompetencias = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString3"].ConnectionString.ToString();
        string bdBasica = ConfigurationManager.AppSettings["BD1"].ToString();
        string bdTrabajadores = ConfigurationManager.AppSettings["BD2"].ToString();
        string bdObjetivos = ConfigurationManager.AppSettings["BD3"].ToString();
        string bdCompetencias = ConfigurationManager.AppSettings["BD4"].ToString();

        MySqlConnection MySqlCn;
        ConsultasGenerales consultas;

        #region Metodo Page Load

        /// <summary>
        /// Carga de la pagina
        /// </summary>
        /// <param name="sender">Objeto sender</param>
        /// <param name="e">Evento e</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            consultas = new ConsultasGenerales();

            if (Session["usuario"] == null)
            {
                //Redirecciona a la pagina de login en caso de que el usuario no se halla autenticado
                Response.Redirect("~/Login.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    CnMysql Conexion = new CnMysql(CnTrabajadores);
                    try
                    {
                        MySqlCommand scSqlCommand = new MySqlCommand("SELECT descripcion FROM " + bdBasica + ".Options_Menu WHERE url = 'EvaluarCompetencias.aspx' AND idEmpresa = 'ST'", Conexion.ObtenerCnMysql());
                        MySqlDataAdapter sdaSqlDataAdapter = new MySqlDataAdapter(scSqlCommand);
                        DataSet dsDataSet = new DataSet();
                        DataTable dtDataTable = null;

                        Conexion.AbrirCnMysql();
                        sdaSqlDataAdapter.Fill(dsDataSet);
                        dtDataTable = dsDataSet.Tables[0];
                        if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                        {
                            this.lblTitulo.Text = dtDataTable.Rows[0].ItemArray[0].ToString();
                        }

                        string anio = consultas.ObtenerPeriodoActivo(Session["idEmpresa"].ToString(),
                                                                     Session["compania"].ToString());

                        if (anio != "0")
                        {
                            Session.Add("anoActivo", anio);
                            this.CargarEmpleados(Session["usuario"].ToString(), Session["compania"].ToString());
                        }
                        else
                        {
                            MensajeError("No hay un periodo activo en el momento");
                        }
                    }
                    catch (Exception E)
                    {
                        MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
                    finally
                    {
                        Conexion.CerrarCnMysql();
                    }
                }
            }
        }

        #endregion

        #region Metodo MensajeError

        /// <summary>
        /// Carga el mensaje de error
        /// </summary>
        /// <param name="Msj">Mensaje que se muestra</param>
        public void MensajeError(string Msj)
        {
            LblMsj.Text = Msj;
            LblMsj.Visible = true;
            UpdatePanel3.Update();
        }

        /// <summary>
        /// Limpia los mensajes
        /// </summary>
        public void LimpiarMensaje()
        {
            LblMsj.Visible = false;
            UpdatePanel3.Update();
        }

        #endregion

        #region Parametros

        /// <summary>
        /// Carga los empleados asociados al jefe
        /// </summary>
        /// <param name="cedulaJefe">Cedula Jefe</param>
        /// <param name="idCompania">Id Compañia</param>
        public void CargarEmpleados(string cedulaJefe, string idCompania)
        {
            this.LimpiarMensaje();

            try
            {
                DataTable dtDataTable = consultas.ConsultarTrabajadoresXJefe(idCompania,
                                                                             cedulaJefe,
                                                                             Session["anoActivo"].ToString());

                if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                {
                    gvEmpleadosAsociados.DataSource = dtDataTable;
                }
                else
                {
                    gvEmpleadosAsociados.DataSource = null;
                    MensajeError("No tiene empleados asociados");
                }

                gvEmpleadosAsociados.DataBind();
                txtCalificacion.Text = string.Empty;
                Container_UpdatePanel1.Visible = true;
                Container_UpdatePanel2.Visible = false;
                Container_UpdatePanel3.Visible = false;
                UpdatePanel1.Update();
            }
            catch (Exception ex)
            {
                MensajeError("El sistema no se encuentra disponible en este momento. " + ex.Message);
            }
        }

        /// <summary>
        /// Carga la niveles
        /// </summary>
        private void CargarNiveles()
        {
            this.LimpiarMensaje();
            CnMysql MysqlCn = new CnMysql(CnCompetencias);

            try
            {
                DataTable dtDataTable = null;
                MysqlCn.AbrirCnMysql();
                dtDataTable = MysqlCn.ConsultarRegistros("SELECT * FROM " + bdCompetencias + ".nivelcompetencias"
                                                         + " where idCompania = '" + Session["compania"]
                                                         + "' and ano = '" + Session["anoActivo"]
                                                         + "' and idEmpresa = '" + Session["idEmpresa"] + "';");

                Session.Add("DataNivel", dtDataTable);

                if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                {
                    gvNivelesCreados.DataSource = dtDataTable;
                }
                else
                {
                    gvNivelesCreados.DataSource = null;
                }

                gvNivelesCreados.DataBind();
                Container_UpdatePanel2.Visible = false;
                UpdatePanel1.Update();
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name + " Sin RED");
            }
            finally
            {
                MysqlCn.CerrarCnMysql();
            }
        }

        /// <summary>
        /// Carga el plan general de un empleado
        /// </summary>
        /// <param name="idPlan">Id Plan</param>
        private void CargarPlanGeneral(int idPlan)
        {
            DataTable planes = consultas.ConsultarPlanesIdPlan(idPlan);

            if (planes != null)
            {
                gvPlanGeneral.DataSource = planes;
                BtnCargarPlanGeneral.Visible = false;
            }
            else 
            {
                gvPlanGeneral.DataSource = null;
                BtnCargarPlanGeneral.Visible = true;
            }
            
            gvPlanGeneral.DataBind();
            UpdatePanel1.Update();
        }

        #endregion

        #region Eventos Controles

        /// <summary>
        /// Maneja los eventos de la grilla empleados
        /// </summary>
        /// <param name="sender">Objeto sender</param>
        /// <param name="e">evento e de la grilla</param>
        protected void gvEmpleadosAsociados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            this.LimpiarMensaje();

            try
            {
                string[] arg = new string[2];
                arg = e.CommandArgument.ToString().Split(';');
                int idJefeEmpleado = Convert.ToInt32(arg[0]);
                int cedulaEmpleado = Convert.ToInt32(arg[1]);

                Session.Add("idJefeEmpleado", idJefeEmpleado);
                Session.Add("cedulaEmpleado", cedulaEmpleado);

                DataTable dtDataTable = consultas.ConsultarCargosTrabajador(cedulaEmpleado);

                if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                {
                    gvCompetencias.DataSource = dtDataTable;

                    lblCargo.Text = "El cargo del usuario " + dtDataTable.Rows[0][1].ToString() +
                                    " es " + dtDataTable.Rows[0][3].ToString();

                    if (e.CommandName == "Evaluar")
                    {
                        this.BtnAceptar.Visible = true;
                    }
                    if (e.CommandName == "Revisar")
                    {
                        this.BtnAceptar.Visible = false;
                    }

                    //// Consultamos si la evaluación creo plan
                    Session.Add("btnPlanGeneral", true);
                    int idEval = (!string.IsNullOrEmpty(dtDataTable.Rows[0][7].ToString()) ?
                                  Convert.ToInt32(dtDataTable.Rows[0][7].ToString()) : 0);

                    if (idEval != 0)
                    {
                        BtnCargarPlanGeneral.Visible = true;
                        DataTable datos = consultas.ConsultarEvalPlan(idEval);

                        if (datos != null)
                        {
                            this.CargarPlanGeneral(Convert.ToInt32(datos.Rows[0][1].ToString()));
                            Session["btnPlanGeneral"] = false;
                            BtnCargarPlanGeneral.Visible = false;
                        }
                        else 
                        {
                            gvPlanGeneral.DataSource = null;
                            gvPlanGeneral.DataBind();
                        }
                    }                    

                    //// Consultamos si la evaluación creo plan
                    
                    this.CargarNiveles();
                }
                else
                {
                    gvCompetencias.DataSource = null;
                    MensajeError("Error al cargar la información del usuario, por favor comuniquese con su administrador");
                    this.BtnAceptar.Visible = false;
                }

                gvCompetencias.DataBind();
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                Container_UpdatePanel1.Visible = false;
                Container_UpdatePanel2.Visible = true;
                UpdatePanel1.Update();
            }
        }

        /// <summary>
        /// Al cargar la grilla se realizan modificaciones sobre las acciones
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e de la grilla</param>
        protected void gvEmpleadosAsociados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton btnEvaluar = (ImageButton)e.Row.FindControl("btnEvaluar");
                ImageButton btnAlerta = (ImageButton)e.Row.FindControl("btnAlerta");
                ImageButton btnOk = (ImageButton)e.Row.FindControl("btnOk");

                string idCargos = DataBinder.Eval(e.Row.DataItem, "IdCargos").ToString();
                string cedulaJefe = DataBinder.Eval(e.Row.DataItem, "Cedula_Jefe").ToString();
                string cedulaEmpleado = DataBinder.Eval(e.Row.DataItem, "Cedula_Empleado").ToString();

                if (idCargos != string.Empty)
                {
                    bool estado = consultas.EvaluacionCompetencia(Session["compania"].ToString(),
                                                                  "ST",
                                                                  cedulaJefe,
                                                                  cedulaEmpleado);
                    if (estado)
                    {
                        btnEvaluar.Visible = false;
                        btnAlerta.Visible = false;
                        btnOk.Visible = true;
                    }
                    else
                    {
                        btnEvaluar.Visible = true;
                        btnAlerta.Visible = false;
                        btnOk.Visible = false;
                    }
                }
                else
                {
                    btnEvaluar.Visible = false;
                    btnAlerta.Visible = true;
                    btnOk.Visible = false;
                }
            }
        }

        /// <summary>
        /// Maneja los eventos de la grilla competencias
        /// </summary>
        /// <param name="sender">Objeto sender</param>
        /// <param name="e">evento e de la grilla</param>
        protected void gvCompetencias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            this.LimpiarMensaje();

            try
            {
                string[] arg = new string[3];
                arg = e.CommandArgument.ToString().Split(';');
                int idCompetencia = Convert.ToInt32(arg[0]);
                int idCargos = Convert.ToInt32(arg[1]);
                int idEvaluacionCompetencia = 0;

                if (arg[2] != "")
                {
                    idEvaluacionCompetencia = Convert.ToInt32(arg[2]);
                    Session.Add("idEvaluacionCompetencia", idEvaluacionCompetencia);
                }

                Session.Add("idCompetencia", idCompetencia);
                Session.Add("idCargos", idCargos);

                GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                int RowIndex = gvr.RowIndex;

                if (e.CommandName == "Evaluar")
                {
                    if (idEvaluacionCompetencia == 0)
                    {
                        idEvaluacionCompetencia = consultas.CrearEvaluacionCompetencia(
                                                                 Convert.ToInt32(Session["usuario"].ToString()),
                                                                 Convert.ToInt32(Session["cedulaEmpleado"].ToString()),
                                                                 idCargos,
                                                                 false,
                                                                 Session["anoActivo"].ToString(),
                                                                 Session["compania"].ToString(),
                                                                 Session["idEmpresa"].ToString());
                    }

                    Session.Add("idEvaluacionCompetencia", idEvaluacionCompetencia);

                    lblCompetencia.Text = gvCompetencias.Rows[RowIndex].Cells[0].Text;
                    BtnCalificar.Text = "Calificar";
                    Container_UpdatePanel2.Visible = false;
                    Container_UpdatePanel3.Visible = true;
                    UpdatePanel1.Update();
                }
                else if (e.CommandName == "Plan")
                {
                    lblCompetenciaG.Text = gvCompetencias.Rows[RowIndex].Cells[0].Text;
                    lblCalificacionG.Text = gvCompetencias.Rows[RowIndex].Cells[1].Text;

                    DataTable planes = consultas.ConsultarPlanes(idCargos, idCompetencia);

                    if (planes == null)
                    {
                        Container_UpdatePanel2.Visible = false;
                        Container_UpdatePanel5.Visible = true;
                        BtnCerrarPlanComp.Visible = true;
                        UpdatePanel1.Update();
                    }
                    else
                    {
                        gvPlanes.DataSource = planes;
                        gvPlanes.DataBind();
                        Container_UpdatePanel2.Visible = false;
                        Container_UpdatePanel4.Visible = true;
                        BtnCerrarPlan.Visible = true;
                        UpdatePanel1.Update();
                        lblPlanDesarrollo.Text = "Competencia: " + lblCompetenciaG.Text;
                    }

                    ScriptManager.RegisterStartupScript(Page, GetType(), "Javascript", "javascript:CargarCalendario(); ", true);
                }
                else if (e.CommandName == "Editar")
                {
                    lblCompetencia.Text = gvCompetencias.Rows[RowIndex].Cells[0].Text;
                    txtCalificacion.Text = gvCompetencias.Rows[RowIndex].Cells[2].Text;
                    BtnCalificar.Text = "Editar";
                    Container_UpdatePanel2.Visible = false;
                    Container_UpdatePanel3.Visible = true;
                    UpdatePanel1.Update();
                }
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Al cargar la grilla se realizan modificaciones sobre las acciones
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e de la grilla</param>
        protected void gvCompetencias_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton btnCalificar = (ImageButton)e.Row.FindControl("btnCalificar");
                ImageButton btnFin = (ImageButton)e.Row.FindControl("btnFin");
                ImageButton btnPlan = (ImageButton)e.Row.FindControl("btnPlan");

                string idEvaluacionCompetencia = DataBinder.Eval(e.Row.DataItem, "idEva").ToString();
                string idCompetencia = DataBinder.Eval(e.Row.DataItem, "idCompetencia").ToString();
                string idCargo = DataBinder.Eval(e.Row.DataItem, "idCargos").ToString();
                string calificacion = DataBinder.Eval(e.Row.DataItem, "Calificacion").ToString();

                if (idEvaluacionCompetencia != "")
                {
                    Session.Add("idEvaluacionCompetencia", idEvaluacionCompetencia);
                }

                bool estado = consultas.EvaluacionCompetencia(Session["compania"].ToString(),
                                                              "ST",
                                                              Session["usuario"].ToString(),
                                                              Session["cedulaEmpleado"].ToString());
                if (!estado)
                {
                    if (calificacion != string.Empty)
                    {
                        bool eval = consultas.ConsultarCalificacionRango(Session["compania"].ToString(),
                                                                         Session["idEmpresa"].ToString(),
                                                                         idEvaluacionCompetencia,
                                                                         idCargo,
                                                                         idCompetencia);

                        if (eval)
                        {
                            btnCalificar.Visible = false;
                            btnFin.Visible = true;
                            btnPlan.Visible = false;
                        }
                        else
                        {
                            btnCalificar.Visible = false;
                            btnFin.Visible = false;
                            btnPlan.Visible = true;
                        }
                    }
                    else
                    {
                        btnCalificar.Visible = true;
                        btnFin.Visible = false;
                        btnPlan.Visible = false;
                    }
                }
                else
                {
                    btnCalificar.Visible = false;
                    btnFin.Visible = false;
                    btnPlan.Visible = false;
                }
            }
        }

        /// <summary>
        /// Acepta los objetivos y los envia al empleado
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnAceptar_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();

            try
            {
                int res = consultas.ActualizarEstadoEvaluacion(Convert.ToInt32(Session["idEvaluacionCompetencia"]),
                                                               true);

                if (res == 0)
                {
                    MensajeError("Hubo un error al crear el seguimiento");
                }
                else
                {
                    this.CargarEmpleados(Session["usuario"].ToString(), Session["compania"].ToString());
                    MensajeError("Evaluación Finalizada");
                }

                UpdatePanel1.Update();
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Regresa a la pantalla de empleados
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnRegresar_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();
            this.CargarEmpleados(Session["usuario"].ToString(), Session["compania"].ToString());
        }

        /// <summary>
        /// Guarda la observacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnCalificar_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();

            try
            {
                int res;

                if (BtnCalificar.Text != "Editar")
                {
                    res = consultas.CrearEvalCargoCompetencias(Convert.ToInt32(Session["idEvaluacionCompetencia"]),
                                                               Convert.ToInt32(Session["idCargos"]),
                                                               Convert.ToInt32(Session["idCompetencia"]),
                                                               Convert.ToInt32(txtCalificacion.Text));
                }
                else
                {
                    res = consultas.ActualizarCalificacion(Convert.ToInt32(Session["idEvaluacionCompetencia"]),
                                                           Convert.ToInt32(Session["idCargos"]),
                                                           Convert.ToInt32(Session["idCompetencia"]),
                                                           Convert.ToInt32(txtCalificacion.Text));
                }

                if (res == 0)
                {
                    MensajeError("Hubo un error al ingresar la información.");
                }
                else
                {
                    BtnCargarPlanGeneral.Visible = Convert.ToBoolean(Session["btnPlanGeneral"]);
                    MensajeError("Calificación Recibida.");
                }

                DataTable dtDataTable = consultas.ConsultarCargosTrabajador(Convert.ToInt32(Session["cedulaEmpleado"]));
                gvCompetencias.DataSource = dtDataTable;
                gvCompetencias.DataBind();

                lblCargo.Text = "El cargo del usuario " + dtDataTable.Rows[0][1].ToString() +
                                " es " + dtDataTable.Rows[0][3].ToString();

                Container_UpdatePanel3.Visible = false;
                Container_UpdatePanel2.Visible = true;
                UpdatePanel1.Update();

                txtCalificacion.Text = string.Empty;
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Regresa a la pantalla de competencias
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnRegresarCal_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();
            txtCalificacion.Text = string.Empty;
            Container_UpdatePanel3.Visible = false;
            Container_UpdatePanel2.Visible = true;
            UpdatePanel1.Update();
        }

        /// <summary>
        /// Maneja los eventos de la grilla planes
        /// </summary>
        /// <param name="sender">Objeto sender</param>
        /// <param name="e">evento e de la grilla</param>
        protected void gvPlanes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            this.LimpiarMensaje();

            try
            {
                int idPlanDesarrollo = 0;

                if (e.CommandArgument.ToString() != "")
                {
                    idPlanDesarrollo = Convert.ToInt32(e.CommandArgument);
                    Session.Add("idPlanEstrategico", idPlanDesarrollo);
                }

                GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                int RowIndex = gvr.RowIndex;

                if (e.CommandName == "Seguimiento")
                {
                    DataTable seguimiento = consultas.ConsultarSeguimiento(idPlanDesarrollo);

                    gvSeguimiento.DataSource = seguimiento;
                    gvSeguimiento.DataBind();
                    Container_UpdatePanel4.Visible = false;
                    Container_UpdatePanel6.Visible = true;
                    UpdatePanel1.Update();
                }
                else if (e.CommandName == "Editar")
                {
                    txtPlan.Text = HttpUtility.HtmlDecode(gvPlanes.Rows[RowIndex].Cells[0].Text);
                    txtFecha.Text = DateTime.Parse(gvPlanes.Rows[RowIndex].Cells[1].Text).ToString("yyyy/MM/dd");

                    Container_UpdatePanel4.Visible = false;
                    Container_UpdatePanel5.Visible = true;
                    BtnGuardarPlan.Text = "Editar Plan";
                    BtnCerrarPlan.Visible = true;
                    UpdatePanel1.Update();

                    ScriptManager.RegisterStartupScript(Page, GetType(), "Javascript", "javascript:CargarCalendario(); ", true);
                }
                else if (e.CommandName == "Fin")
                {
                    int estado = consultas.ActualizarEstadoPlan(idPlanDesarrollo, true);

                    if (estado != 0)
                    {
                        DataTable planes = consultas.ConsultarPlanes(Convert.ToInt32(Session["idCargos"]),
                                                                     Convert.ToInt32(Session["idCompetencia"]));
                        gvPlanes.DataSource = planes;
                        gvPlanes.DataBind();
                        Container_UpdatePanel4.Visible = true;
                        Container_UpdatePanel5.Visible = false;
                        this.MensajeError("Estado del plan actualizado");
                    }
                    else
                    {
                        Container_UpdatePanel4.Visible = true;
                        Container_UpdatePanel5.Visible = false;
                        this.MensajeError("Error al actualizar el estado del plan");
                    }

                    UpdatePanel1.Update();
                }
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Al cargar la grilla se realizan modificaciones sobre las acciones
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e de la grilla</param>
        protected void gvPlanes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton btnSeguimiento = (ImageButton)e.Row.FindControl("btnSeguimiento");
                ImageButton btnEditPlan = (ImageButton)e.Row.FindControl("btnEditPlan");
                ImageButton btnFinPlan = (ImageButton)e.Row.FindControl("btnFinPlan");
                ImageButton btnPlanOk = (ImageButton)e.Row.FindControl("btnPlanOk");

                string idPlanEstrategico = DataBinder.Eval(e.Row.DataItem, "idPlanEstrategico").ToString();

                bool estadoPlan = consultas.EvaluacionPlan(Convert.ToInt32(idPlanEstrategico));

                if (estadoPlan)
                {
                    btnSeguimiento.Visible = false;
                    btnEditPlan.Visible = false;
                    btnFinPlan.Visible = false;
                    btnPlanOk.Visible = true;
                }
                else
                {
                    btnSeguimiento.Visible = true;
                    btnEditPlan.Visible = true;
                    btnFinPlan.Visible = true;
                    btnPlanOk.Visible = false;
                }
            }
        }

        /// <summary>
        /// Si quiere calificar a un empleado y finalizar del plan de desarrollo 
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnRegCalificar_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();

            lblCompetencia.Text = lblCompetenciaG.Text;
            txtCalificacion.Text = lblCalificacionG.Text;
            BtnCalificar.Text = "Editar";
            Container_UpdatePanel4.Visible = false;
            Container_UpdatePanel5.Visible = false;
            Container_UpdatePanel3.Visible = true;
            UpdatePanel1.Update();
        }

        /// <summary>
        /// Crea un nuevo plan
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnCrearPlan_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();
            txtPlan.Text = string.Empty;
            txtFecha.Text = string.Empty;

            Container_UpdatePanel4.Visible = false;
            Container_UpdatePanel5.Visible = true;
            BtnGuardarPlan.Text = "Crear Plan";
            BtnCerrarPlan.Visible = true;
            UpdatePanel1.Update();

            ScriptManager.RegisterStartupScript(Page, GetType(), "Javascript", "javascript:CargarCalendario(); ", true);
        }

        /// <summary>
        /// Registra un plan nuevo
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnGuardarPlan_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();

            try
            {
                int res;

                if (BtnGuardarPlan.Text != "Editar Plan")
                {
                    res = consultas.CrearPlanDesarrollo(txtPlan.Text,
                                                        DateTime.Parse(txtFecha.Text),
                                                        Convert.ToInt32(Session["idCargos"]),
                                                        Convert.ToInt32(Session["idCompetencia"]));
                }
                else
                {
                    res = consultas.ActualizarCalificacion(Convert.ToInt32(Session["idPlanEstrategico"]),
                                                           txtPlan.Text,
                                                           DateTime.Parse(txtFecha.Text));
                }

                if (res == 0)
                {
                    if (BtnGuardarPlan.Text != "Editar Plan")
                    {
                        MensajeError("Hubo un error al crear el Plan");
                    }
                    else
                    {
                        MensajeError("Hubo un error al editar el Plan");
                    }

                    Container_UpdatePanel5.Visible = false;
                    Container_UpdatePanel2.Visible = true;
                }
                else
                {
                    DataTable planes = consultas.ConsultarPlanes(Convert.ToInt32(Session["idCargos"]),
                                                                 Convert.ToInt32(Session["idCompetencia"]));
                    gvPlanes.DataSource = planes;
                    gvPlanes.DataBind();
                    Container_UpdatePanel4.Visible = true;
                    Container_UpdatePanel5.Visible = false;

                    if (BtnGuardarPlan.Text != "Editar Plan")
                    {
                        MensajeError("Plan Creado Correctamente.");
                    }
                    else
                    {
                        MensajeError("Plan Actualizado.");
                    }

                    lblPlanDesarrollo.Text = "Competencia: " + lblCompetenciaG.Text;
                }

                BtnCerrarPlan.Visible = false;
                BtnCerrarPlanComp.Visible = false;
                UpdatePanel1.Update();
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Regresa a la pantalla de plan
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnCerrarPlan_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();
            BtnCerrarPlan.Visible = false;
            Container_UpdatePanel5.Visible = false;
            Container_UpdatePanel6.Visible = false;
            Container_UpdatePanel4.Visible = true;
            UpdatePanel1.Update();
        }

        /// <summary>
        /// Regresa a la pantalla de competencia
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnCerrarPlanComp_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();
            BtnCerrarPlanComp.Visible = false;
            Container_UpdatePanel4.Visible = false;
            Container_UpdatePanel5.Visible = false;
            Container_UpdatePanel6.Visible = false;
            Container_UpdatePanel2.Visible = true;
            UpdatePanel1.Update();
        }

        /// <summary>
        /// Registra un seguimiento
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnGuardarSeg_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();

            try
            {
                int res = consultas.CrearSeguimiento(txtSeguimiento.Text,
                                                     DateTime.Now,
                                                     Convert.ToInt32(Session["idPlanEstrategico"]));

                if (res == 0)
                {
                    MensajeError("Hubo un error al crear el seguimiento");
                }
                else
                {
                    DataTable seguimiento = consultas.ConsultarSeguimiento(Convert.ToInt32(Session["idPlanEstrategico"]));

                    gvSeguimiento.DataSource = seguimiento;
                    gvSeguimiento.DataBind();
                    Container_UpdatePanel4.Visible = false;
                    Container_UpdatePanel6.Visible = true;
                    txtSeguimiento.Text = string.Empty;

                    MensajeError("Seguimiento Creado.");
                }

                UpdatePanel1.Update();
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Regresa a la pantalla de planes
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnRegresarSeg_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();
            BtnCerrarPlanComp.Visible = false;
            Container_UpdatePanel6.Visible = false;
            Container_UpdatePanel4.Visible = true;
            UpdatePanel1.Update();
        }

        #endregion

        /// <summary>
        /// Carga el formulario de plan general
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnCargarPlanGeneral_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();
            txtPlanGeneral.Text = string.Empty;
            txtFechaGeneral.Text = string.Empty;

            Container_UpdatePanel2.Visible = false;
            Container_UpdatePanelPlan1.Visible = true;
            UpdatePanel1.Update();

            ScriptManager.RegisterStartupScript(Page, GetType(), "Javascript", "javascript:CargarCalendario(); ", true);
        }

        /// <summary>
        /// Comandos de la fila plan general
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void gvPlanGeneral_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            this.LimpiarMensaje();

            try
            {
                int idPlanDesarrollo = 0;

                if (e.CommandArgument.ToString() != "")
                {
                    idPlanDesarrollo = Convert.ToInt32(e.CommandArgument);
                    Session.Add("idPlanEstrategico", idPlanDesarrollo);
                }

                GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                int RowIndex = gvr.RowIndex;

                if (e.CommandName == "Seguimiento")
                {
                    DataTable seguimiento = consultas.ConsultarSeguimiento(idPlanDesarrollo);

                    gvSeguimiento.DataSource = seguimiento;
                    gvSeguimiento.DataBind();
                    Container_UpdatePanel2.Visible = false;
                    Container_UpdatePanel6.Visible = true;
                    BtnRegresarComp.Visible = true;
                    BtnRegresarSeg.Visible = false;
                    UpdatePanel1.Update();
                }
                else if (e.CommandName == "Editar")
                {
                    txtPlanGeneral.Text = HttpUtility.HtmlDecode(gvPlanGeneral.Rows[RowIndex].Cells[0].Text);
                    txtFechaGeneral.Text = DateTime.Parse(gvPlanGeneral.Rows[RowIndex].Cells[1].Text).ToString("yyyy/MM/dd");

                    Container_UpdatePanel2.Visible = false;
                    Container_UpdatePanelPlan1.Visible = true;
                    btnCrearPlanGeneral.Text = "Editar Plan";
                    UpdatePanel1.Update();

                    ScriptManager.RegisterStartupScript(Page, GetType(), "Javascript", "javascript:CargarCalendario(); ", true);
                }
                else if (e.CommandName == "Fin")
                {
                    int estado = consultas.ActualizarEstadoPlan(idPlanDesarrollo, true);

                    if (estado != 0)
                    {
                        this.CargarPlanGeneral(idPlanDesarrollo);
                        Container_UpdatePanel2.Visible = true;
                        Container_UpdatePanelPlan1.Visible = false;
                        this.MensajeError("Estado del plan actualizado");
                    }
                    else
                    {
                        Container_UpdatePanel2.Visible = true;
                        Container_UpdatePanelPlan1.Visible = false;
                        this.MensajeError("Error al actualizar el estado del plan");
                    }

                    UpdatePanel1.Update();
                }
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Databound de la fila plan general
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPlanGeneral_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton btnSeguimiento = (ImageButton)e.Row.FindControl("btnSeguimientoG");
                ImageButton btnEditPlan = (ImageButton)e.Row.FindControl("btnEditPlanG");
                ImageButton btnFinPlan = (ImageButton)e.Row.FindControl("btnFinPlanG");
                ImageButton btnPlanOk = (ImageButton)e.Row.FindControl("btnPlanOkG");

                string idPlanEstrategico = DataBinder.Eval(e.Row.DataItem, "idPlanEstrategico").ToString();

                bool estadoPlan = consultas.EvaluacionPlan(Convert.ToInt32(idPlanEstrategico));

                if (estadoPlan)
                {
                    btnSeguimiento.Visible = false;
                    btnEditPlan.Visible = false;
                    btnFinPlan.Visible = false;
                    btnPlanOk.Visible = true;
                }
                else
                {
                    btnSeguimiento.Visible = true;
                    btnEditPlan.Visible = true;
                    btnFinPlan.Visible = true;
                    btnPlanOk.Visible = false;
                }
            }
        }

        /// <summary>
        /// Crea un plan general
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnCrearPlanGeneral_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();

            try
            {
                int res;

                if (btnCrearPlanGeneral.Text != "Editar Plan")
                {
                    res = consultas.CrearPlanGeneral(txtPlanGeneral.Text,
                                                    DateTime.Parse(txtFechaGeneral.Text),
                                                    Convert.ToInt32(Session["idEvaluacionCompetencia"]));
                }
                else
                {
                    res = consultas.ActualizarCalificacion(Convert.ToInt32(Session["idPlanEstrategico"]),
                                                           txtPlanGeneral.Text,
                                                           DateTime.Parse(txtFechaGeneral.Text));

                    if (res != 0) 
                    { 
                        res = Convert.ToInt32(Session["idPlanEstrategico"]);
                    }
                }

                if (res == 0)
                {
                    if (btnCrearPlanGeneral.Text != "Editar Plan")
                    {
                        MensajeError("Hubo un error al crear el Plan General");
                    }
                    else
                    {
                        MensajeError("Hubo un error al editar el Plan General");
                    }

                    Container_UpdatePanelPlan1.Visible = false;
                    Container_UpdatePanel2.Visible = true;
                }
                else
                {
                    this.CargarPlanGeneral(res);

                    Container_UpdatePanel2.Visible = true;
                    Container_UpdatePanelPlan1.Visible = false;

                    if (btnCrearPlanGeneral.Text != "Editar Plan")
                    {
                        MensajeError("Plan Creado Correctamente.");
                    }
                    else
                    {
                        MensajeError("Plan Actualizado.");
                    }
                }

                UpdatePanel1.Update();
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Regresa a las competencias
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void btnRegresarGeneral_Click(object sender, EventArgs e)
        {
            this.LimpiarMensaje();
            txtPlanGeneral.Text = string.Empty;
            txtFechaGeneral.Text = string.Empty;

            Container_UpdatePanel2.Visible = true;
            Container_UpdatePanel6.Visible = false;
            Container_UpdatePanelPlan1.Visible = false;
            BtnRegresarComp.Visible = false;
            BtnRegresarSeg.Visible = true;
            UpdatePanel1.Update();
        }
    }
}