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
    public partial class ValidarObjetivos : System.Web.UI.Page
    {
        string Cn = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString"].ConnectionString.ToString();
        string Cn2 = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString2"].ConnectionString.ToString();
        string bd1 = ConfigurationManager.AppSettings["BD1"].ToString();
        string bd2 = ConfigurationManager.AppSettings["BD2"].ToString();
        string bd3 = ConfigurationManager.AppSettings["BD3"].ToString();
        MySqlConnection MySqlCn;
        ConsultasGenerales claseConsultas;

        #region Metodo Page Load

        /// <summary>
        /// Carga de la pagina
        /// </summary>
        /// <param name="sender">Objeto sender</param>
        /// <param name="e">Evento e</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            claseConsultas = new ConsultasGenerales();

            if (Session["usuario"] == null)
            {
                //Redirecciona a la pagina de login en caso de que el usuario no se halla autenticado
                Response.Redirect("~/Login.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    CnMysql Conexion = new CnMysql(Cn);
                    try
                    {
                        MySqlCommand scSqlCommand = new MySqlCommand("SELECT descripcion FROM " + bd1 + ".Options_Menu WHERE url = 'ValidarObjetivos.aspx'", Conexion.ObtenerCnMysql());
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

                        this.ObtenerPeriodoActivo();
                        this.CargarEmpleados(Session["usuario"].ToString(), Session["compania"].ToString());
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

        #endregion

        #region Parametros

        /// <summary>
        /// Obtiene el año activo en el sistema
        /// </summary>
        public void ObtenerPeriodoActivo()
        {
            try
            {
                MySqlCn = new MySqlConnection(Cn);
                MySqlCommand scSqlCommand;
                string consulta = "SELECT * FROM " + bd3 + ".parametrosgenerales " +
                                  "WHERE Empresas_idEmpresa = '" + Session["idEmpresa"] +
                                  "' AND idCompania = '" + Session["compania"] + "'" +
                                  " AND Activo = 1;";

                scSqlCommand = new MySqlCommand(consulta, MySqlCn);

                MySqlCn.Open();
                MySqlDataReader rd = scSqlCommand.ExecuteReader();

                if (rd.HasRows)
                {
                    if (rd.Read())
                    {
                        Session.Add("anoActivo", rd["Ano"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MensajeError("El sistema no se encuentra disponible en este momento. " + ex.Message);
            }
            finally
            {
                MySqlCn.Close();
            }
        }

        /// <summary>
        /// Carga los empleados asociados al jefe
        /// </summary>
        /// <param name="cedulaJefe">Cedula Jefe</param>
        /// <param name="idCompania">Id Compañia</param>
        public void CargarEmpleados(string cedulaJefe, string idCompania)
        {
            try
            {
                DataSet dsDataSet = new DataSet();
                DataTable dtDataTable = null;

                MySqlCn = new MySqlConnection(Cn);
                MySqlCommand scSqlCommand;
                string consulta = "SELECT jefeempleado.idJefeEmpleado," +
                                  "jefeempleado.idTercero," +
                                  "jefeempleado.idCompania," +
                                  "jefeempleado.Cedula_Empleado," +
                                  "empleados.Nombres_Completos_Empleado " +
                                  "FROM " + bd3 + ".jefeempleado " +
                                  "INNER JOIN " + bd2 + ".empleados ON jefeempleado.Cedula_Empleado = empleados.Id_Empleado " +
                                  "WHERE idCompania = '" + idCompania + "' AND Cedula_Jefe = " + cedulaJefe + " AND Ano = '" + Session["anoActivo"] + "';";

                scSqlCommand = new MySqlCommand(consulta, MySqlCn);
                MySqlDataAdapter sdaSqlDataAdapter = new MySqlDataAdapter(scSqlCommand);
                sdaSqlDataAdapter.Fill(dsDataSet);
                dtDataTable = dsDataSet.Tables[0];

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
                Container_UpdatePanel1.Visible = true;
                Container_UpdatePanel2.Visible = false;
                Container_UpdatePanel3.Visible = false;
                Container_UpdatePanelObservaciones.Visible = false;
                UpdatePanel1.Update();
            }
            catch (Exception ex)
            {
                MensajeError("El sistema no se encuentra disponible en este momento. " + ex.Message);
            }
            finally
            {
                MySqlCn.Close();
            }
        }

        /// <summary>
        /// Actualiza la tabla etapaJefe segun la decisión del jefe
        /// </summary>
        /// <param name="etapa">Etapa actual</param>
        /// <param name="estadoEtapa">Estado de la etapa</param>
        /// <returns>true si se realizo correctamente el procedimiento</returns>
        public bool ActualizarEtapa(int etapa, int estadoEtapa)
        {
            CnMysql Conexion = new CnMysql(Cn2);
            Conexion.AbrirCnMysql();

            int res = 0;

            try
            {
                MySqlCommand cmd = new MySqlCommand("sp_CrearEtapaJefeEmpleado", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Etapas_idEtapas", etapa);
                cmd.Parameters.AddWithValue("@JefeEmpleado_idJefeEmpleado", Session["idJefeEmpleado"]);
                cmd.Parameters.AddWithValue("@EstadoEtapa_idEstadoEtapa", estadoEtapa);
                cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);

                // Crea un parametro de salida para el SP
                MySqlParameter outputIdParam = new MySqlParameter("@respuesta", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(outputIdParam);
                cmd.ExecuteNonQuery();

                //Almacena la respuesta de la variable de retorno del SP
                res = int.Parse(outputIdParam.Value.ToString());

                if (res == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MensajeError("Ha ocurrido el siguiente error: " + ex.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
            finally
            {
                Conexion.CerrarCnMysql();
            }
        }

        /// <summary>
        /// Verifica el estado de la etapa
        /// </summary>
        /// <returns>true si esta en etapa valida sino no lo deja continuar</returns>
        public bool ComprobarEstadoEtapa(string idJefeEmpleado, string etapa)
        {
            try
            {
                DataSet dsDataSet = new DataSet();
                DataTable dtDataTable = null;

                MySqlCn = new MySqlConnection(Cn);
                MySqlCommand scSqlCommand;
                string consulta = "SELECT * FROM " + bd3 + ".etapa_jefeempleado where " +
                                  "JefeEmpleado_idJefeEmpleado = " + idJefeEmpleado +
                                  " AND Etapas_idEtapas = " + etapa +
                                  " ORDER BY fecha desc;";

                scSqlCommand = new MySqlCommand(consulta, MySqlCn);
                MySqlDataAdapter sdaSqlDataAdapter = new MySqlDataAdapter(scSqlCommand);
                sdaSqlDataAdapter.Fill(dsDataSet);
                dtDataTable = dsDataSet.Tables[0];

                if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                {
                    Session.Add("etapaJefeEmpleado", dtDataTable.Rows[0].ItemArray[3].ToString());

                    if (dtDataTable.Rows[0].ItemArray[3].ToString() == "2")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Session.Add("etapaJefeEmpleado", "0");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MensajeError("El sistema no se encuentra disponible en este momento. " + ex.Message);
                return false;
            }
            finally
            {
                MySqlCn.Close();
            }
        }

        /// <summary>
        /// Crea una observacion
        /// </summary>
        /// <param name="cedula">Cedula de quien observa</param>
        /// <param name="observacion">texto observacion</param>
        /// <returns>true si el proceso es correcto</returns>
        public bool CrearObservacion(string cedula, string observacion)
        {
            CnMysql Conexion = new CnMysql(Cn2);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                cmd = new MySqlCommand("sp_CrearObservacion", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@JefeEmpleado_idJefeEmpleado", Session["idJefeEmpleado"]);
                cmd.Parameters.AddWithValue("@Etapas_idEtapas", 1);
                cmd.Parameters.AddWithValue("@Cedula", cedula);
                cmd.Parameters.AddWithValue("@Descripcion", observacion);
                cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                cmd.Parameters.AddWithValue("@Ano", Session["anoActivo"]);

                // Crea un parametro de salida para el SP
                MySqlParameter outputIdParam = new MySqlParameter("@respuesta", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(outputIdParam);
                cmd.ExecuteNonQuery();

                //Almacena la respuesta de la variable de retorno del SP
                res = int.Parse(outputIdParam.Value.ToString());

                if (res == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        /// <summary>
        /// Observaciones del jefe
        /// </summary>
        /// <param name="idJefeEmpleado">Id del JefeEmpleado</param>
        public void CargarObservacionesEmpleado(string idJefeEmpleado, string etapa)
        {
            try
            {
                DataTable dtDataTable = claseConsultas.ConsultarObservaciones(idJefeEmpleado, etapa);

                if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                {
                    gvObservaciones.DataSource = dtDataTable;
                }
                else
                {
                    gvObservaciones.DataSource = null;
                }

                gvObservaciones.DataBind();
                Container_UpdatePanelObservaciones.Visible = true;
                UpdatePanel1.Update();
            }
            catch (Exception ex)
            {
                MensajeError("El sistema no se encuentra disponible en este momento. " + ex.Message);
            }
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
            Container_UpdatePanelObservaciones.Visible = false;
            LblMsj.Visible = false;
            UpdatePanel3.Update();

            CnMysql Conexion = new CnMysql(Cn2);
            Conexion.AbrirCnMysql();

            try
            {
                string[] arg = new string[2];
                arg = e.CommandArgument.ToString().Split(';');
                int idJefeEmpleado = Convert.ToInt32(arg[0]);
                int cedulaEmpleado = Convert.ToInt32(arg[1]);

                Session.Add("idJefeEmpleado", idJefeEmpleado);

                DataSet dsDataSet = new DataSet();
                DataTable dtDataTable = null;

                MySqlCommand scSqlCommand;
                string consulta = "SELECT objetivos.Descripcion, objetivos.Peso, objetivos.Meta " +
                                    "FROM " + bd3 + ".objetivos " +
                                    "INNER JOIN " + bd3 + ".jefeempleado " +
                                    "ON objetivos.JefeEmpleado_idJefeEmpleado = jefeempleado.idJefeEmpleado " +
                                    "where jefeempleado.Cedula_Jefe = " + Session["usuario"].ToString() +
                                    " AND jefeempleado.idCompania = '" + Session["compania"].ToString() +
                                    "' AND jefeempleado.idJefeEmpleado = " + idJefeEmpleado + ";";

                scSqlCommand = new MySqlCommand(consulta, Conexion.ObtenerCnMysql());
                MySqlDataAdapter sdaSqlDataAdapter = new MySqlDataAdapter(scSqlCommand);
                sdaSqlDataAdapter.Fill(dsDataSet);
                dtDataTable = dsDataSet.Tables[0];

                if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                {
                    gvObjetivosCreados.DataSource = dtDataTable;
                    this.CargarObservacionesEmpleado(idJefeEmpleado.ToString(), "1");

                    if (e.CommandName == "Evaluar")
                    {
                        this.BtnAceptar.Visible = true;
                        this.BtnRechazar.Visible = true;
                    }
                    else if (e.CommandName == "Revisar")
                    {
                        this.BtnAceptar.Visible = false;
                        this.BtnRechazar.Visible = false;
                    }
                }
                else
                {
                    gvObjetivosCreados.DataSource = null;
                    MensajeError("El empleado no ha creado objetivos");
                    this.BtnAceptar.Visible = false;
                    this.BtnRechazar.Visible = false;
                }

                gvObjetivosCreados.DataBind();
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
                Conexion.CerrarCnMysql();
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
                ImageButton btnRevisar = (ImageButton)e.Row.FindControl("btnRevisar");
                ImageButton btnOk = (ImageButton)e.Row.FindControl("btnOk");

                string idJefeEmpleado = DataBinder.Eval(e.Row.DataItem, "idJefeEmpleado").ToString();

                if (this.ComprobarEstadoEtapa(idJefeEmpleado, "1"))
                {
                    btnEvaluar.Visible = true;
                    btnRevisar.Visible = false;
                    btnOk.Visible = false;
                }
                else
                {
                    if (Session["etapaJefeEmpleado"].ToString() == "0")
                    {
                        btnEvaluar.Visible = false;
                        btnRevisar.Visible = false;
                        btnOk.Visible = false;
                    }
                    else
                    {
                        btnEvaluar.Visible = false;
                        btnRevisar.Visible = true;
                        btnOk.Visible = true;
                    }
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
            this.BtnAceptar.Visible = false;
            this.BtnRechazar.Visible = false;
            Container_UpdatePanel3.Visible = true;
            UpdatePanel1.Update();
            Session.Add("botonOpc", 4);
        }

        /// <summary>
        /// Se rechazan los objetivos y se envia una observación
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnRechazar_Click(object sender, EventArgs e)
        {
            this.BtnAceptar.Visible = false;
            this.BtnRechazar.Visible = false;
            Container_UpdatePanelObservaciones.Visible = false;
            Container_UpdatePanel3.Visible = true;
            UpdatePanel1.Update();
            Session.Add("botonOpc", 3);
        }

        /// <summary>
        /// Guarda la observacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (this.ActualizarEtapa(1, Convert.ToInt32(Session["botonOpc"].ToString())))
            {
                if (this.CrearObservacion(Session["usuario"].ToString(), txtObservacion.Text))
                {
                    MensajeError("Se envio correctamente la información");
                }
                else
                {
                    MensajeError("Hubo un error, por favor revise con su administrador");
                }
            }
            else
            {
                MensajeError("Hubo un error al guardar la información");
            }

            this.CargarEmpleados(Session["usuario"].ToString(), Session["compania"].ToString());
        }

        /// <summary>
        /// Regresa a la pantalla de empleados
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnRegresar_Click(object sender, EventArgs e)
        {
            LblMsj.Visible = false;
            UpdatePanel3.Update();
            this.CargarEmpleados(Session["usuario"].ToString(), Session["compania"].ToString());
        }

        #endregion
    }
}