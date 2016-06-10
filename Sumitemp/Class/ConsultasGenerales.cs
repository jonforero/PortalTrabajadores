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
    public class ConsultasGenerales
    {
        string CnObjetivos = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString2"].ConnectionString.ToString();
        string CnTrabajadores = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString"].ConnectionString.ToString();
        string bdTrabajadores = ConfigurationManager.AppSettings["BD2"].ToString();
        string bdModobjetivos = ConfigurationManager.AppSettings["BD3"].ToString();

        /// <summary>
        /// Consulta el periodo seleccionado (semestre trimestre)
        /// </summary>
        /// <param name="idCompania">Id Compañia</param>
        /// <param name="idEmpresa">Id Empresa</param>
        /// <returns>Valor del periodo</returns>
        public string ConsultarPeriodoSeguimiento(string idCompania, string idEmpresa)
        {
            CnMysql Conexion = new CnMysql(CnTrabajadores);

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand("SELECT Periodo_Seguimiento FROM " + bdModobjetivos +
                                                    ".parametrosgenerales where idCompania = '" + idCompania +
                                                    "' AND Empresas_idEmpresa = '" + idEmpresa + "'", Conexion.ObtenerCnMysql());
                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    return rd["Periodo_Seguimiento"].ToString();
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                Conexion.CerrarCnMysql();
            }
        }

        /// <summary>
        /// Comprueba que el peso de los objetivos sea igual a 100
        /// </summary>
        /// <param name="idJefeEmpleado">Id jefe empleado</param>
        /// <returns>Devuelve true si cumple los 100</returns>
        public string ComprobarPesoObjetivos(string idJefeEmpleado) 
        {
            CnMysql Conexion = new CnMysql(CnTrabajadores);

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand("SELECT sum(Peso) as Peso FROM " + bdModobjetivos +
                                                    ".objetivos where JefeEmpleado_idJefeEmpleado = " + idJefeEmpleado, Conexion.ObtenerCnMysql());
                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    return rd["Peso"].ToString();
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                Conexion.CerrarCnMysql();
            }
        }
    }
}