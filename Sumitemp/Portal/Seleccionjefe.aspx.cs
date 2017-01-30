using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;


namespace PortalTrabajadores.Portal
{
    public partial class Seleccionjefe : System.Web.UI.Page
    {
        string Cn = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString"].ConnectionString.ToString();
        string Cn2 = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString2"].ConnectionString.ToString();
        string bd1 = ConfigurationManager.AppSettings["BD1"].ToString();
        string bd2 = ConfigurationManager.AppSettings["BD2"].ToString();
        string bd3 = ConfigurationManager.AppSettings["BD3"].ToString();
        MySqlConnection MySqlCn;

        /* ****************************************************************************/
        /* Metodo que se ejecuta al momento de la carga de la Pagina
        /* ****************************************************************************/
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
                    if (Session["Seleccionjefe"] != null)
                    {
                        if (Session["Seleccionjefe"].ToString() == "1")
                        {
                            lblTexto.Visible = true;
                            Session.Remove("Seleccionjefe");
                        }
                    }

                    CnMysql Conexion = new CnMysql(Cn);
                    try
                    {
                        txtuser.Focus();

                        MySqlCommand scSqlCommand = new MySqlCommand("SELECT descripcion FROM " + bd1 + ".Options_Menu WHERE url = 'Seleccionjefe.aspx' and Tipoportal = 'T'", Conexion.ObtenerCnMysql());
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

        /* ****************************************************************************/
        /* Metodo que habilita el label de mensaje de error
        /* ****************************************************************************/
        public void MensajeError(string Msj)
        {
            LblMsj.Text = Msj;
            LblMsj.Visible = true;
            UpdatePanel3.Update();
        }

        /* ****************************************************************************/
        /* Metodo que consulta la información del tercero a actualizar
        /* ****************************************************************************/
        protected void BtnBuscar_Click(object sender, EventArgs e)
        {
            //Se valida que se haya digitado un usuario
            if (txtuser.Text.Trim() == "")
            {
                MensajeError("Digite un Número de Identificación.");
            }
            else
            {
                if (BtnBuscar.Text == "Nueva Búsqueda")
                {
                    txtuser.Text = "";
                    txtuser.Enabled = true;
                    BtnBuscar.Text = "Buscar Jefe";
                    MensajeError("");

                    Container_UpdatePanel2.Visible = false;

                    TxtDoc.Text = "";
                    TxtNombres.Text = "";
                    txtuser.Focus();

                    BtnEditar.Enabled = true;
                    BtnEditar.BackColor = default(System.Drawing.Color);
                }
                else
                {
                    BtnBuscar.Text = "Nueva Búsqueda";

                    TxtDoc.Text = "";
                    TxtNombres.Text = "";

                    txtuser.Enabled = false;
                    MensajeError("");
                    CargarInfoCliente(txtuser.Text);
                }
            }
        }

        /* ****************************************************************************/
        /* Metodo que carga la informacion del tercero en el formulario
        /* ****************************************************************************/
        public void CargarInfoCliente(string IdJefe)
        {
            CnMysql Conexion = new CnMysql(Cn);

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand(bd3 + ".ConsultarJefe", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idjefe", IdJefe);
                cmd.Parameters.AddWithValue("@idEmpleado", Session["usuario"].ToString());
                cmd.Parameters.AddWithValue("@empresa", "ST");
                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    Container_UpdatePanel2.Visible = true;
                    TxtDoc.Text = rd["Id_Empleado"].ToString();
                    TxtNombres.Text = rd["Nombres_Completos_Empleado"].ToString();
                    BtnEditar.Text = "Guardar Información";
                }
                else
                {
                    MensajeError("No se encontro el Número de Identificación. ");
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

        /* ********************************************************************************************************/
        /* Evento que se produce al dar clic sobre el boton BtnEditar para almacenar la informacion del tercero
        /* ********************************************************************************************************/
        protected void BtnEditar_Click(object sender, EventArgs e)
        {
            CnMysql Conexion = new CnMysql(Cn2);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand(bd3 + ".AsignarJefe", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idjefe", TxtDoc.Text);
                cmd.Parameters.AddWithValue("@idEmpleado", Session["usuario"].ToString());
                cmd.Parameters.AddWithValue("@empresa", "ST");
                cmd.Parameters.AddWithValue("@anio", Session["anoActivo"].ToString());

                // Crea un parametro de salida para el SP
                MySqlParameter outputIdParam = new MySqlParameter("@respuesta", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(outputIdParam);
                cmd.ExecuteNonQuery();

                //Almacena la respuesta de la variable de retorno del SP
                res = int.Parse(outputIdParam.Value.ToString());
                if (res == 0)
                {
                    MensajeError("Usted ya cuenta con un Jefe Asignado. ");
                    BtnEditar.Enabled = false;
                }
                else if (res == 3)
                {
                    MensajeError("Usted No puede ser su propio Jefe. ");
                    BtnEditar.Enabled = false;
                }
                else if (res == 2)
                {
                    MensajeError("El Jefe fue Actualizado. ");
                    BtnEditar.Enabled = false;
                }
                else
                {
                    MensajeError("El Jefe fue Asignado. ");
                    BtnEditar.Enabled = false;
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

        /* ****************************************************************************/
        /* Metodo que actualiza la informacion del tercero en la Base de datos
        /* ****************************************************************************/
        public int ActualizaInfoTercero(string Nit_tercero)
        {
            CnMysql Conexion = new CnMysql(Cn);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand(bd2 + ".sp_ActualizaTercero", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NitTercero", TxtDoc.Text.ToUpper());
                cmd.Parameters.AddWithValue("@nombreTercero", TxtNombres.Text.ToUpper());

                // Crea un parametro de salida para el SP
                MySqlParameter outputIdParam = new MySqlParameter("@respuesta", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(outputIdParam);
                cmd.ExecuteNonQuery();

                //Almacena la respuesta de la variable de retorno del SP
                res = int.Parse(outputIdParam.Value.ToString());
                return res;
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                return res;
            }
            finally
            {
                Conexion.CerrarCnMysql();
                Session.Remove("Contrasena");
                Session.Remove("cambiaContrasena");
            }
        }

        /* ****************************************************************************/
        /* Metodo que inserta la informacion del tercero en la Base de datos
        /* ****************************************************************************/
        public int CrearTercero()
        {
            CnMysql Conexion = new CnMysql(Cn);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand(bd2 + ".sp_CrearTercero", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NitTercero", TxtDoc.Text);
                cmd.Parameters.AddWithValue("@nombreTercero", TxtNombres.Text);

                // Crea un parametro de salida para el SP
                MySqlParameter outputIdParam = new MySqlParameter("@respuesta", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(outputIdParam);
                cmd.ExecuteNonQuery();

                //Almacena la respuesta de la variable de retorno del SP
                res = int.Parse(outputIdParam.Value.ToString());
                return res;
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                return res;
            }
            finally
            {
                Conexion.CerrarCnMysql();
                Session.Remove("Contrasena");
                Session.Remove("cambiaContrasena");
            }
        }

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
    }
}