using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace PortalTrabajadores.Portal
{
    public partial class Estadocesantias : System.Web.UI.Page
    {
        string Cn = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString"].ConnectionString.ToString();
        string bd1 = ConfigurationManager.AppSettings["BD1"].ToString();
        string bd2 = ConfigurationManager.AppSettings["BD2"].ToString();
		string ruta = ConfigurationManager.AppSettings["RepositorioPDF"].ToString();
		
        #region Definicion de los Metodos de la Clase

        #region Metodo Page Load
        /* ****************************************************************************/
        /* Metodo que se ejecuta al momento de la carga de la Pagina Maestra
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
                SqlDataSource1.SelectCommand = "SELECT idCesantia, anio_cesantia, Mes_Cesantia, ruta_archivo_cesantia as ruta,nombre_archivo_cesantia as nombre FROM " + bd2 + ".cesantias where Empleados_Id_Empleado =" + Session["usuario"].ToString() + " and Empleados_idEmpresa = 'SS';";
                if (!IsPostBack)
                {
                    CnMysql Conexion = new CnMysql(Cn);
                    try
                    {
                        MySqlCommand scSqlCommand = new MySqlCommand("SELECT descripcion FROM " + bd1 + ".Options_Menu WHERE url = 'Cesantias.aspx'", Conexion.ObtenerCnMysql());
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

        #region Metodo GridView1_RowCommand
        /* ***********************************************************************************/
        /* Evento que se ejecuta al momento de dar clic sobre los elementos IMG del GridView
        /* ***********************************************************************************/
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "PDF")
                {
                    int index = Convert.ToInt32(e.CommandArgument);
                    string filepath = "";
                    string filename2 = "";
                    DataView dvSql = (DataView)SqlDataSource1.Select(DataSourceSelectArguments.Empty);

                    //Recorre el SqlDataSource
                    foreach (DataRowView drvSql in dvSql)
                    {
                        if (drvSql["idCesantia"].ToString() == index.ToString())
                        {
                            filepath = drvSql["ruta"].ToString();
                            filename2 = drvSql["nombre"].ToString();
                            break;
                        }
                    }
                    //Valida la Existencia de la ruta Origen para poder descargar el PDF los Archivos
                    string filename = System.IO.Path.Combine(ruta,filepath, filename2);
                    if (System.IO.File.Exists(filename))
                    {
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.Clear();
                        //Response.ContentType = "application/octet-stream";
                        //Response.AddHeader("Content-Disposition", "attachment; filename=" + filename2);
                        //Response.WriteFile(filename);
                        Response.Buffer = true;
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + filename2);
                        Response.WriteFile(filename);
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        MensajeError("Aun no se encuentra disponible el certificado.");
                    }
                }
            }
            catch (Exception E)
            {
                MensajeError("Ha ocurrido el siguiente error: " + E.Message + " _Metodo: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }
        #endregion

        #endregion
    }
}