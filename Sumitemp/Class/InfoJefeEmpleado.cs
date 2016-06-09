using MySql.Data.MySqlClient;
using PortalTrabajadores.Portal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace PortalTrabajadores.Class
{
    public class InfoJefeEmpleado
    {
        string Cn1 = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString2"].ConnectionString.ToString();
        string Cn2 = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString"].ConnectionString.ToString();
        string bd2 = ConfigurationManager.AppSettings["BD2"].ToString();
        string bd3 = ConfigurationManager.AppSettings["BD3"].ToString();

        public bool ConsultarEstadoJefe(string cedulaEmp) 
        {
            CnMysql Conexion = new CnMysql(Cn1);

            try
            {                
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM " + bd3 + ".jefeempleado where Cedula_Empleado = " +
                                                    cedulaEmp, Conexion.ObtenerCnMysql());
                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    string cedulaJefe = rd["Cedula_Jefe"].ToString();
                    Conexion.CerrarCnMysql();

                    Conexion.AbrirCnMysql();
                    MySqlCommand cmd2 = new MySqlCommand("SELECT Id_Rol FROM " + bd2 + ".empleados where Id_Empleado = " +
                                                         cedulaJefe, Conexion.ObtenerCnMysql());
                    MySqlDataReader rd2 = cmd2.ExecuteReader();

                    if (rd2.Read())
                    {
                        if (rd2["Id_Rol"].ToString() == "6")
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
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally 
            {
                Conexion.CerrarCnMysql();
            }
        }
    }
}