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
    public partial class FijarSeguimiento1 : System.Web.UI.Page
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
                        MySqlCommand scSqlCommand = new MySqlCommand("SELECT descripcion FROM " + bd1 + ".Options_Menu WHERE url = 'FijarSeguimiento1.aspx'", Conexion.ObtenerCnMysql());
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

                        //// Revisa si tiene un jefe asignado
                        if (this.CargarParametros())
                        {
                            //// Comprueba que este en las fechas de la etapa
                            if (this.ComprobarFechaEtapas("2", DateTime.Now))
                            {
                                //// Comprueba si esta en una estado de la etapa valido, sino restringe la vista
                                if (this.ComprobarEstadoEtapa(Session["idJefeEmpleado"].ToString(), "2"))
                                {
                                    //// Carga los objetivos Modificables
                                    this.CargarObjetivos(Session["idJefeEmpleado"].ToString());
                                }
                                else
                                {
                                    if (Session["idEstadoEtapa"] != null)
                                    {
                                        this.CargarObjetivoBloqueados(Session["idJefeEmpleado"].ToString());

                                        if (Session["idEstadoEtapa"].ToString() == "4")
                                        {
                                            MensajeError("El seguimiento fue aceptado por su jefe.");
                                        }
                                        else
                                        {
                                            MensajeError("Alerta, su seguimiento fue enviado, se carga una vista de solo lectura");
                                        }
                                    }
                                    else
                                    {
                                        MensajeError("No puede acceder a esta sección en el momento.");
                                    }
                                }
                            }
                            else
                            {
                                this.CargarObjetivoBloqueados(Session["idJefeEmpleado"].ToString());
                                MensajeError("Usted no puede continuar ya que esta por fuera de las fechas. Por favor comuniquese con su Jefe");
                            }

                            if (Session["idEstadoEtapa"] != null)
                            {
                                this.CargarObservaciones(Session["idJefeEmpleado"].ToString(), "2");
                            }
                        }
                        else
                        {
                            MensajeError("Usted no puede continuar hasta asignar un jefe, por favor revise la pagina Selección de Jefe");
                        }

                        this.ComprobarSeguimientoTotal(Session["idJefeEmpleado"].ToString());
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
        /// Permite cargar los parametros esenciales
        /// Revisa si tiene un jefe asignado
        /// </summary>
        /// <returns>true si tiene jefe</returns>
        public bool CargarParametros()
        {
            try
            {
                MySqlCn = new MySqlConnection(Cn);
                MySqlCommand scSqlCommand;
                string consulta = "SELECT * FROM " + bd3 + ".jefeempleado where idCompania = '" + Session["compania"]
                                + "' AND Cedula_Empleado = " + Session["usuario"].ToString()
                                + " AND Ano = '" + Session["anoActivo"] + "';";

                scSqlCommand = new MySqlCommand(consulta, MySqlCn);

                MySqlCn.Open();
                MySqlDataReader rd = scSqlCommand.ExecuteReader();

                if (rd.HasRows)
                {
                    if (rd.Read())
                    {
                        Session.Add("cedulaJeje", rd["Cedula_Jefe"]);
                        Session.Add("idJefeEmpleado", rd["idJefeEmpleado"]);
                    }

                    return true;
                }
                else
                {
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
        /// Revisa que el usuario este dentro de las fechas validas para trabajar en la pagina
        /// </summary>
        /// <param name="etapa">Identificador de la etapa</param>
        /// <param name="fecha">fecha actual</param>
        /// <returns>true si puede modificar</returns>
        public bool ComprobarFechaEtapas(string etapa, DateTime fecha)
        {
            try
            {
                MySqlCn = new MySqlConnection(Cn);
                MySqlCommand scSqlCommand;
                string consulta = "SELECT * FROM  " + bd3 + ".fechasetapas where Etapas_idEtapas = " + etapa +
                                  " AND (Corte_Inicio <= '" + fecha.ToString("yyyy/MM/dd") + "' AND Corte_Fin >= '" + fecha.ToString("yyyy/MM/dd") + "');";

                scSqlCommand = new MySqlCommand(consulta, MySqlCn);
                MySqlCn.Open();
                MySqlDataReader rd = scSqlCommand.ExecuteReader();

                if (rd.HasRows)
                {
                    return true;
                }
                else
                {
                    MySqlCn.Close();

                    consulta = "SELECT * FROM  " + bd3 + ".periodoextra where Etapas_idEtapas = " + etapa +
                                  " AND (Fecha_Inicio <= '" + fecha.ToString("yyyy/MM/dd") + "' AND Fecha_Fin >= '" + fecha.ToString("yyyy/MM/dd") + "');";

                    scSqlCommand = new MySqlCommand(consulta, MySqlCn);
                    MySqlCn.Open();
                    rd = scSqlCommand.ExecuteReader();

                    if (rd.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
        /// Verifica el estado de la etapa
        /// </summary>
        /// <returns>true si esta en etapa valida sino no lo deja continuar</returns>
        public bool ComprobarEstadoEtapa(string idJefeEmpleado, string etapa)
        {
            try
            {
                Session.Remove("idEstadoEtapa");

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
                    Session.Add("idEstadoEtapa", dtDataTable.Rows[0].ItemArray[3].ToString());

                    if (dtDataTable.Rows[0].ItemArray[3].ToString() == "1" || dtDataTable.Rows[0].ItemArray[3].ToString() == "3")
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
                    if (this.ComprobarEtapaAnterior(idJefeEmpleado, "1"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
        /// Comprueba el estado de la etapa
        /// </summary>
        /// <param name="idJefeEmpleado"></param>
        /// <param name="etapa"></param>
        /// <returns>true si la etapa esta finalizada</returns>
        public bool ComprobarEtapaAnterior(string idJefeEmpleado, string etapa)
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
                    if (dtDataTable.Rows[0].ItemArray[3].ToString() == "4")
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
        /// Observaciones del jefe
        /// </summary>
        /// <param name="idJefeEmpleado">Id del JefeEmpleado</param>
        public void CargarObservaciones(string idJefeEmpleado, string etapa)
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
            finally
            {
                MySqlCn.Close();
            }
        }

        /// <summary>
        /// Carga los objetivos modificables
        /// </summary>
        /// <param name="idJefeEmpleado">id JefeEmpleado</param>
        public void CargarObjetivos(string idJefeEmpleado)
        {
            try
            {
                DataSet dsDataSet = new DataSet();
                DataTable dtDataTable = null;

                MySqlCn = new MySqlConnection(Cn2);
                MySqlCommand scSqlCommand;
                scSqlCommand = new MySqlCommand("sp_ConsultarObjetivosSeguimientos", MySqlCn);
                scSqlCommand.CommandType = CommandType.StoredProcedure;
                scSqlCommand.Parameters.AddWithValue("@idJefeEmpleado", Session["idJefeEmpleado"]);

                MySqlDataAdapter sdaSqlDataAdapter = new MySqlDataAdapter(scSqlCommand);
                sdaSqlDataAdapter.Fill(dsDataSet);
                dtDataTable = dsDataSet.Tables[0];

                if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                {
                    gvObjetivosCreados.DataSource = dtDataTable;
                }
                else
                {
                    gvObjetivosCreados.DataSource = null;
                }

                gvObjetivosCreados.DataBind();
                Container_UpdatePanel2.Visible = false;
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
        /// Carga los objetivos bloqueados
        /// </summary>
        /// <param name="idJefeEmpleado">id JefeEmpleado</param>
        public void CargarObjetivoBloqueados(string idJefeEmpleado)
        {
            try
            {
                DataSet dsDataSet = new DataSet();
                DataTable dtDataTable = null;

                MySqlCn = new MySqlConnection(Cn2);
                MySqlCommand scSqlCommand;
                scSqlCommand = new MySqlCommand("sp_ConsultarObjetivosSeguimientos", MySqlCn);
                scSqlCommand.CommandType = CommandType.StoredProcedure;
                scSqlCommand.Parameters.AddWithValue("@idJefeEmpleado", Session["idJefeEmpleado"]);

                MySqlDataAdapter sdaSqlDataAdapter = new MySqlDataAdapter(scSqlCommand);
                sdaSqlDataAdapter.Fill(dsDataSet);
                dtDataTable = dsDataSet.Tables[0];

                if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                {
                    gvObjetivosBloqueados.DataSource = dtDataTable;
                }
                else
                {
                    gvObjetivosBloqueados.DataSource = null;
                }

                gvObjetivosBloqueados.DataBind();
                Container_UpdatePanel1.Visible = false;
                Container_UpdatePanelBloqueado.Visible = true;
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
        /// Crea una observacion
        /// </summary>
        /// <param name="cedula">Cedula de quien observa</param>
        /// <param name="observacion">texto observacion</param>
        /// <returns>true si el proceso es correcto</returns>
        public bool CrearObservacion(string cedula, string observacion, int etapa)
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
                cmd.Parameters.AddWithValue("@Etapas_idEtapas", etapa);
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
        /// Comprueba el que se hayan registrado todos los seguimientos
        /// </summary>
        public void ComprobarSeguimientoTotal(string idJefeEmpleado)
        {
            try
            {
                MySqlCn = new MySqlConnection(Cn2);
                MySqlCommand scSqlCommand;
                string consulta = "SELECT IF((SELECT COUNT(*) FROM objetivos WHERE objetivos.JefeEmpleado_idJefeEmpleado = " + idJefeEmpleado + ") = " +
                                  "(SELECT COUNT(*) FROM seguimiento WHERE seguimiento.Objetivos_JefeEmpleado_idJefeEmpleado = " + idJefeEmpleado +
                                  " AND seguimiento.Periodo = '1'), 1, 0) " + 
                                  "AS Seguimiento;";

                scSqlCommand = new MySqlCommand(consulta, MySqlCn);

                MySqlCn.Open();
                MySqlDataReader rd = scSqlCommand.ExecuteReader();

                if (rd.HasRows)
                {
                    if (rd.Read())
                    {
                        if (rd["Seguimiento"].ToString() == "1")
                        {
                            BtnEnviar.Visible = true;
                        }
                    }
                }

                Container_UpdatePanelObservaciones.Visible = true;
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

        #endregion

        #region EventosControles

        /// <summary>
        /// Envia los objetivos al jefe
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnEnviar_Click(object sender, EventArgs e)
        {
            LblMsj.Visible = false;
            UpdatePanel3.Update();

            Container_UpdatePanel2.Visible = false;
            Container_UpdatePanel3.Visible = true;
            UpdatePanel1.Update();
        }

        /// <summary>
        /// Guardar o editar los objetivos
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnGuardar_Click(object sender, EventArgs e)
        {
            CnMysql Conexion = new CnMysql(Cn2);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                if (BtnGuardar.Text == "Guardar")
                {
                    cmd = new MySqlCommand("sp_CrearSeguimiento", Conexion.ObtenerCnMysql());
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Objetivos_idObjetivos", Session["idObjetivos"]);
                    cmd.Parameters.AddWithValue("@Objetivos_JefeEmpleado_idJefeEmpleado", Session["idJefeEmpleado"]);
                    cmd.Parameters.AddWithValue("@Cedula", Session["usuario"]);
                    cmd.Parameters.AddWithValue("@Descripcion", txtSeguimiento.Text);
                    cmd.Parameters.AddWithValue("@Meta", Convert.ToDouble(txtMeta.Text)/100);
                    cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Ano", Session["anoActivo"]);
                    cmd.Parameters.AddWithValue("@Periodo", "1");
                }
                else
                {
                    cmd = new MySqlCommand("sp_ActualizarSeguimiento", Conexion.ObtenerCnMysql());
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Objetivos_idObjetivos", Session["idObjetivos"]);
                    cmd.Parameters.AddWithValue("@Objetivos_JefeEmpleado_idJefeEmpleado", Session["idJefeEmpleado"]);
                    cmd.Parameters.AddWithValue("@Descripcion", txtSeguimiento.Text);
                    cmd.Parameters.AddWithValue("@Meta", Convert.ToDouble(txtMeta.Text) / 100);
                }

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
                    if (BtnGuardar.Text == "Guardar")
                    {
                        MensajeError("Seguimiento creado correctamente");
                    }
                    else
                    {
                        MensajeError("Seguimiento actualizado correctamente");
                    }
                }
                else
                {
                    MensajeError("Hubo un error al crear, por favor revise con su administrador");
                }

                this.ComprobarSeguimientoTotal(Session["idJefeEmpleado"].ToString());
                txtSeguimiento.Text = string.Empty;
                txtMeta.Text = string.Empty;
                this.CargarObjetivos(Session["idJefeEmpleado"].ToString());
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                Container_UpdatePanel2.Visible = false;
                UpdatePanel1.Update();
                Conexion.CerrarCnMysql();
            }
        }

        /// <summary>
        /// Cancela el formulario actual
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            txtSeguimiento.Text = string.Empty;
            txtMeta.Text = string.Empty;

            LblMsj.Visible = false;
            UpdatePanel3.Update();

            Container_UpdatePanel2.Visible = false;
            UpdatePanel1.Update();
        }

        /// <summary>
        /// Maneja los eventos de la grilla objetivos
        /// </summary>
        /// <param name="sender">Objeto sender</param>
        /// <param name="e">evento e de la grilla</param>
        protected void gvObjetivosCreados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LblMsj.Visible = false;
            UpdatePanel3.Update();

            Container_UpdatePanel3.Visible = false;
            UpdatePanel1.Update();

            txtSeguimiento.Text = string.Empty;
            txtMeta.Text = string.Empty;

            CnMysql Conexion = new CnMysql(Cn2);
            Conexion.AbrirCnMysql();

            try
            {
                string[] arg = new string[4];
                arg = e.CommandArgument.ToString().Split(';');
                int idObjetivos = Convert.ToInt32(arg[0]);
                int estadoSeguimiento = Convert.ToInt32(arg[1]);
                string segDescripcion = arg[2];
                string meta = string.Empty;

                if (arg[3] != "") 
                {
                    meta = (Convert.ToDouble(arg[3]) * 100).ToString();
                }                

                Session.Add("idObjetivos", idObjetivos);

                if (e.CommandName == "Seguimiento")
                {
                    if (estadoSeguimiento == 1)
                    {
                        BtnGuardar.Text = "Editar";
                        txtSeguimiento.Text = segDescripcion;
                        txtMeta.Text = meta;
                    }
                    else
                    {
                        BtnGuardar.Text = "Guardar";
                    }

                    Container_UpdatePanel2.Visible = true;
                    UpdatePanel1.Update();
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

        /// <summary>
        /// Al cargar la grilla se realizan modificaciones sobre las acciones
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e de la grilla</param>
        protected void gvObjetivosCreados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton btnOk = (ImageButton)e.Row.FindControl("btnOk");

                string idJefeEmpleado = DataBinder.Eval(e.Row.DataItem, "Seguimiento").ToString();

                if (idJefeEmpleado == "1")
                {
                    btnOk.Visible = true;
                }
                else
                {
                    btnOk.Visible = false;
                }
            }
        }

        /// <summary>
        /// Guardar Observacion y enviar evento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnGuardarObs_Click(object sender, EventArgs e)
        {
            if (this.CrearObservacion(Session["usuario"].ToString(), txtObservacion.Text, 2))
            {
                CnMysql Conexion = new CnMysql(Cn2);
                Conexion.AbrirCnMysql();

                int res = 0;

                try
                {
                    MySqlCommand cmd = new MySqlCommand("sp_CrearEtapaJefeEmpleado", Conexion.ObtenerCnMysql());
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Etapas_idEtapas", 2);
                    cmd.Parameters.AddWithValue("@JefeEmpleado_idJefeEmpleado", Session["idJefeEmpleado"]);
                    cmd.Parameters.AddWithValue("@EstadoEtapa_idEstadoEtapa", 5);
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
                        this.CargarObjetivoBloqueados(Session["idJefeEmpleado"].ToString());
                    }

                    Container_UpdatePanelObservaciones.Visible = false;
                    UpdatePanel1.Update();
                    MensajeError("Se envio correctamente la información");

                    Container_UpdatePanel1.Visible = false;
                    Container_UpdatePanel2.Visible = false;
                    Container_UpdatePanel3.Visible = false;
                    UpdatePanel1.Update();
                }
                catch (Exception ex)
                {
                    MensajeError("Ha ocurrido el siguiente error: " + ex.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
                finally
                {
                    Conexion.CerrarCnMysql();
                }
            }
            else
            {
                MensajeError("Hubo un error, por favor revise con su administrador");
            }
        }

        #endregion
    }
}