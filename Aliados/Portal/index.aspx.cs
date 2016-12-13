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
    public partial class index1 : System.Web.UI.Page
    {
        string Cn = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString"].ConnectionString.ToString();
        string bd2 = ConfigurationManager.AppSettings["BD2"].ToString();

        #region Definicion de los Metodos de la Clase

        #region Metodo Page Load
        /* ****************************************************************************/
        /* Metodo que se ejecuta al momento de la carga de la Pagina
        /* ****************************************************************************/
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuario"] == null)
            {
                //Redirecciona a la pagina de login en caso de que el usuario no se haya autenticado
                Response.Redirect("~/Login.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    ConsultasGenerales consultaGeneral = new ConsultasGenerales();
                    bool objetivos = consultaGeneral.ComprobarModuloObjetivos(Session["nit"].ToString(), Session["idEmpresa"].ToString());
                    bool comp = consultaGeneral.ComprobarModuloCompetencias(Session["nit"].ToString(), Session["idEmpresa"].ToString());

                    CnMysql Conexion = new CnMysql(Cn);
                    MySqlCommand scSqlCommand = new MySqlCommand("SELECT Contrasena_Activo, IdAreas, IdCargos, Id_Rol FROM " + bd2 + ".empleados where Id_Empleado = '" + this.Session["usuario"].ToString() + "'", Conexion.ObtenerCnMysql());
                    MySqlDataAdapter sdaSqlDataAdapter = new MySqlDataAdapter(scSqlCommand);
                    DataSet dsDataSet = new DataSet();
                    DataTable dtDataTable = null;

                    try
                    {
                        Conexion.AbrirCnMysql();
                        sdaSqlDataAdapter.Fill(dsDataSet);
                        dtDataTable = dsDataSet.Tables[0];

                        if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                        {
                            InfoJefeEmpleado info = new InfoJefeEmpleado();

                            if (dtDataTable.Rows[0].ItemArray[0].ToString() == "1")
                            {
                                Response.Redirect("PrimeraContrasena.aspx", false);
                            }
                            else if (dtDataTable.Rows[0].ItemArray[1].ToString() == "" ||
                                     dtDataTable.Rows[0].ItemArray[2].ToString() == "")
                            {
                                if (objetivos || comp)
                                {
                                    Session.Add("AsignarAreaCargo", "1");
                                    Response.Redirect("AsignarAreaCargo.aspx", false);
                                }
                            }
                            else if (!info.ConsultarEstadoJefe(this.Session["usuario"].ToString()) &&
                                     dtDataTable.Rows[0].ItemArray[3].ToString() == "2")
                            {
                                Session.Add("Seleccionjefe", "1");
                                Response.Redirect("Seleccionjefe.aspx", false);
                            }
                        }
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
            }
        }
        #endregion

        #region Metodo MensajeError
        /* ****************************************************************************/
        /* Metodo que habilita el label de mensaje de error
        /* ****************************************************************************/
        //IniciaMetodo
        public void MensajeError(string Msj)
        {
            ContentPlaceHolder cPlaceHolder;
            cPlaceHolder = (ContentPlaceHolder)Master.FindControl("Errores");
            if (cPlaceHolder != null)
            {
                Label lblMessage = (Label)cPlaceHolder.FindControl("LblMsj") as Label;
                if (lblMessage != null)
                {
                    lblMessage.Text = Msj;
                    lblMessage.Visible = true;
                }
            }
        }
        #endregion

        #endregion
    }
}