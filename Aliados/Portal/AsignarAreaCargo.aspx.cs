using MySql.Data.MySqlClient;
using PortalTrabajadores.Class;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PortalTrabajadores.Portal
{
    public partial class AsignarAreaCargo : System.Web.UI.Page
    {
        string Cn1 = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString2"].ConnectionString.ToString();
        string Cn2 = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString"].ConnectionString.ToString();
        string bd1 = ConfigurationManager.AppSettings["BD1"].ToString();
        string bd2 = ConfigurationManager.AppSettings["BD2"].ToString();
        MySqlConnection MySqlCn;

        #region Metodo Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuario"] == null)
            {
                //Redirecciona a la pagina de login en caso de que el usuario no se halla autenticado
                Response.Redirect("~/Login.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    this.LimpiarMensajes();

                    if (Session["AsignarAreaCargo"] != null)
                    {
                        if (Session["AsignarAreaCargo"].ToString() == "1")
                        {
                            lblTexto.Visible = true;
                            Session.Remove("AsignarAreaCargo");
                        }
                    }

                    try
                    {
                        MySqlCn = new MySqlConnection(Cn2);
                        MySqlCommand scSqlCommand;
                        string consulta = "SELECT Id_Empleado FROM " + bd2 + ".empleados where Id_Empleado = " + Session["usuario"] + ";";

                        scSqlCommand = new MySqlCommand(consulta, MySqlCn);

                        MySqlDataAdapter sdaSqlDataAdapter = new MySqlDataAdapter(scSqlCommand);
                        DataSet dsDataSet = new DataSet();
                        DataTable dtDataTable = null;
                        MySqlCn.Open();
                        sdaSqlDataAdapter.Fill(dsDataSet);
                        dtDataTable = dsDataSet.Tables[0];

                        if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                        {
                            this.CargarInfoCliente(Session["usuario"].ToString(), false);
                        }

                        scSqlCommand = new MySqlCommand("SELECT descripcion FROM " + bd1 + ".Options_Menu WHERE url = 'AsignarAreaCargo.aspx'", MySqlCn);
                        sdaSqlDataAdapter = new MySqlDataAdapter(scSqlCommand);
                        dsDataSet = new DataSet();
                        dtDataTable = null;

                        sdaSqlDataAdapter.Fill(dsDataSet);
                        dtDataTable = dsDataSet.Tables[0];
                        if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                        {
                            this.lblTitulo.Text = dtDataTable.Rows[0].ItemArray[0].ToString();
                        }

                        UpdatePanel1.Update();
                    }
                    catch (Exception ex)
                    {
                        MensajeError("Ha ocurrido el siguiente error: " + ex.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
                    finally
                    {
                        MySqlCn.Close();
                    }

                    this.CargarAreasCargos();
                }
            }
        }

        #endregion

        #region Metodo MensajeError

        /// <summary>
        /// Mensaje de error
        /// </summary>
        /// <param name="Msj">Mensaje de error</param>
        public void MensajeError(string Msj)
        {
            LblMsj.Text = Msj;
            LblMsj.Visible = true;
            UpdatePanel3.Update();
        }

        /// <summary>
        /// Limpia los mensajes
        /// </summary>
        private void LimpiarMensajes()
        {
            LblMsj.Visible = false;
            UpdatePanel3.Update();
        }

        #endregion

        /// <summary>
        /// Edita los datos del usuario
        /// </summary>
        /// <param name="sender">objeto sender</param>
        /// <param name="e">evento e</param>
        protected void BtnEditar_Click(object sender, EventArgs e)
        {
            this.LimpiarMensajes();
            CnMysql Conexion = new CnMysql(Cn2);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                cmd = new MySqlCommand("sp_ActualizarAreaCargoEmp", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoId_empleado", "2");
                cmd.Parameters.AddWithValue("@Id_Empleado", txtUser2.Text);
                cmd.Parameters.AddWithValue("@Companias_idEmpresa", Session["idEmpresa"]);
                cmd.Parameters.AddWithValue("@Companias_idCompania", Session["compania"]);
                cmd.Parameters.AddWithValue("@IdAreas", ddlArea.SelectedValue);
                cmd.Parameters.AddWithValue("@IdCargos", ddlCargo.SelectedValue);               

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
                    Response.Redirect("index.aspx", false);
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
        /// Cargar informacion del usuario
        /// </summary>
        /// <param name="idUsuario">Id del usuario</param>
        /// <param name="externo">Indica si es un usuario externo</param>
        public void CargarInfoCliente(string idUsuario, bool externo)
        {
            this.LimpiarMensajes();
            CnMysql Conexion = new CnMysql(Cn1);

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand(bd2 + ".sp_ConsultaEmpleadosExt", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id_Empleado", idUsuario);
                cmd.Parameters.AddWithValue("@Companias_idCompania", Session["compania"]);
                cmd.Parameters.AddWithValue("@empresa", Session["idEmpresa"]);
                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    txtUser2.Text = Session["usuario"].ToString();
                    txtNombres.Text = rd["Nombres_Empleado"].ToString();
                    txtPrimerApellido.Text = rd["Primer_Apellido_empleado"].ToString();
                    txtSegundoApellido.Text = rd["Segundo_Apellido_Empleado"].ToString();

                    if (rd["IdAreas"].ToString() != "")
                    {
                        ddlArea.SelectedValue = rd["IdAreas"].ToString();
                    }

                    if (rd["IdCargos"].ToString() != "")
                    {
                        ddlCargo.SelectedValue = rd["IdCargos"].ToString();
                    }
                }
                else
                {
                    MensajeError("Error al cargar la información");
                }

                rd.Close();         
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
        /// Carga las areas y los cargos al ddl
        /// </summary>
        public void CargarAreasCargos()
        {
            this.LimpiarMensajes();
            CnMysql Conexion = new CnMysql(Cn2);
            string msgError = string.Empty;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand(bd2 + ".sp_ConsultaAreasEmp", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idEmpresa", Session["idEmpresa"]);
                cmd.Parameters.AddWithValue("@id_Compania", Session["compania"]);
                cmd.Parameters.AddWithValue("@estado", true);

                MySqlDataAdapter sdaSqlDataAdapter = new MySqlDataAdapter(cmd);
                DataSet dsDataSet = new DataSet();
                DataTable dtDataTable = null;
                MySqlCn.Open();
                sdaSqlDataAdapter.Fill(dsDataSet);
                dtDataTable = dsDataSet.Tables[0];

                if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                {
                    ddlArea.DataSource = dtDataTable;
                    ddlArea.DataTextField = "Area";
                    ddlArea.DataValueField = "IdAreas";
                    ddlArea.DataBind();

                    ddlArea.Items.Insert(0, new ListItem("---Seleccione---", "0", true));
                }
                else
                {
                    msgError = "No se han creado areas.";
                }

                cmd = new MySqlCommand(bd2 + ".sp_ConsultaCargosEmp", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idEmpresa", Session["idEmpresa"]);
                cmd.Parameters.AddWithValue("@id_Compania", Session["compania"]);
                cmd.Parameters.AddWithValue("@estado", true);

                sdaSqlDataAdapter = new MySqlDataAdapter(cmd);
                DataSet dsDataCargos = new DataSet();
                DataTable dtDataTableCargos = null;
                sdaSqlDataAdapter.Fill(dsDataCargos);
                dtDataTableCargos = dsDataCargos.Tables[0];

                if (dtDataTableCargos != null && dtDataTableCargos.Rows.Count > 0)
                {
                    ddlCargo.DataSource = dtDataTableCargos;
                    ddlCargo.DataTextField = "Cargo";
                    ddlCargo.DataValueField = "IdCargos";
                    ddlCargo.DataBind();

                    ddlCargo.Items.Insert(0, new ListItem("---Seleccione---", "0", true));
                }
                else
                {
                    msgError += "No se han creado cargos.";
                }

                if (msgError != string.Empty)
                {
                    MensajeError(msgError + " Comuniquese con su admnistrador");
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