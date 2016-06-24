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
        string CnTrabajadores = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString"].ConnectionString.ToString();
        string CnObjetivos = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString2"].ConnectionString.ToString();
        string CnCompetencias = ConfigurationManager.ConnectionStrings["trabajadoresConnectionString3"].ConnectionString.ToString();
        string bdBasica = ConfigurationManager.AppSettings["BD1"].ToString();
        string bdTrabajadores = ConfigurationManager.AppSettings["BD2"].ToString();
        string bdModobjetivos = ConfigurationManager.AppSettings["BD3"].ToString();
        string bdModCompetencias = ConfigurationManager.AppSettings["BD4"].ToString();

        #region Generales

        /// <summary>
        /// Comprueba si la compania tiene el modulo de objetivos activos
        /// </summary>
        /// <returns>True si esta activo</returns>
        public bool ComprobarModuloObjetivos(string idCompania, string idEmpresa)
        {
            CnMysql Conexion = new CnMysql(CnTrabajadores);

            try
            {
                MySqlCommand rolCommand = new MySqlCommand("SELECT * FROM " +
                                                            bdBasica + ".matriz_modulostercero where idCompania = '" +
                                                            idCompania + "' and idEmpresa = '" +
                                                            idEmpresa + "'", Conexion.ObtenerCnMysql());

                MySqlDataAdapter rolDataAdapter = new MySqlDataAdapter(rolCommand);
                DataSet rolDataSet = new DataSet();
                DataTable rolDataTable = null;

                rolDataAdapter.Fill(rolDataSet);
                rolDataTable = rolDataSet.Tables[0];

                if (rolDataTable != null && rolDataTable.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (Conexion.EstadoConexion() == ConnectionState.Open)
                {
                    Conexion.CerrarCnMysql();
                }
            }
        }

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

        /// <summary>
        /// Comprueba que el peso de los objetivos sea igual a 100
        /// </summary>
        /// <param name="idEmpresa"></param>
        /// <param name="idCompania"></param>
        /// <returns>Periodo Activo</returns>
        public string ObtenerPeriodoActivo(string idEmpresa, string idCompania)
        {
            CnMysql Conexion = new CnMysql(CnTrabajadores);

            try
            {
                string consulta = "SELECT * FROM " + bdModobjetivos + ".parametrosgenerales " +
                                  "WHERE Empresas_idEmpresa = '" + idEmpresa +
                                  "' AND idCompania = '" + idCompania +
                                  "' AND Activo = 1;";

                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand(consulta, Conexion.ObtenerCnMysql());
                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    return rd["Ano"].ToString();
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

        #endregion

        #region Competencias

        /// <summary>
        /// Devuelve los años registrados en el sistema
        /// </summary>
        /// <returns></returns>
        public DataTable ConsultarTrabajadoresXJefe(string idCompania, string cedulaJefe, string anio)
        {
            CnMysql Conexion = new CnMysql(CnObjetivos);

            try
            {
                Conexion.AbrirCnMysql();
                string consulta;

                consulta = "SELECT jefeempleado.idJefeEmpleado, " +
                           "jefeempleado.idTercero, " +
                           "jefeempleado.idCompania, " +
                           "jefeempleado.Cedula_Empleado, " +
                           "jefeempleado.Cedula_Jefe, " +
                           "empleados.Nombres_Completos_Empleado, " +
                           "empleados.IdCargos " +
                           "FROM " + bdModobjetivos + ".jefeempleado " +
                           "INNER JOIN " + bdTrabajadores + ".empleados " +
                           "ON jefeempleado.Cedula_Empleado = empleados.Id_Empleado  " +
                           "WHERE idCompania = '" + idCompania + "' " +
                           "AND Cedula_Jefe = " + cedulaJefe + " AND Ano = '" + anio + "';";

                MySqlCommand cmd = new MySqlCommand(consulta, Conexion.ObtenerCnMysql());
                MySqlDataAdapter sdaSqlDataAdapter = new MySqlDataAdapter(cmd);
                DataSet dsDataSet = new DataSet();
                DataTable dtDataTable = null;

                sdaSqlDataAdapter.Fill(dsDataSet);
                dtDataTable = dsDataSet.Tables[0];

                if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                {
                    return dtDataTable;
                }
                else
                {
                    return null;
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

        /// <summary>
        /// Devuelve el cargo y competencias del usuario
        /// </summary>
        /// <param name="cedula_Empleado">Cedula Empleado</param>
        /// <returns>Tabla con los datos</returns>
        public DataTable ConsultarCargosTrabajador(int cedula_Empleado)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand("sp_ConsultarCargosTrabajador", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id_Empleado", cedula_Empleado);

                MySqlDataAdapter sdaSqlDataAdapter = new MySqlDataAdapter(cmd);
                DataSet dsDataSet = new DataSet();
                DataTable dtDataTable = null;
                sdaSqlDataAdapter.Fill(dsDataSet);
                dtDataTable = dsDataSet.Tables[0];

                if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                {
                    return dtDataTable;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                Conexion.CerrarCnMysql();
            }
        }

        /// <summary>
        /// Devuelve el cargo y competencias del usuario
        /// </summary>
        /// <param name="cedula_Empleado">Cedula Empleado</param>
        /// <returns>Tabla con los datos</returns>
        public DataTable ConsultarPlanes(int idCargo, int idCompetencia)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);

            try
            {
                Conexion.AbrirCnMysql();
                string consulta;

                consulta = "SELECT * FROM " + bdModCompetencias + ".cargoplan c " +
                           "INNER JOIN " + bdModCompetencias + ".planestrategico p ON " +
                           "c.idPlanEstrategico = p.idPlanEstrategico " +
                           "WHERE c.idCargo = " + idCargo + " " +
                           "AND c.idCompetencia = " + idCompetencia;

                MySqlCommand cmd = new MySqlCommand(consulta, Conexion.ObtenerCnMysql());
                MySqlDataAdapter sdaSqlDataAdapter = new MySqlDataAdapter(cmd);
                DataSet dsDataSet = new DataSet();
                DataTable dtDataTable = null;
                sdaSqlDataAdapter.Fill(dsDataSet);
                dtDataTable = dsDataSet.Tables[0];

                if (dtDataTable != null && dtDataTable.Rows.Count > 0)
                {
                    return dtDataTable;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                Conexion.CerrarCnMysql();
            }
        }

        /// <summary>
        /// Consulta si el usuario ya tiene creada una evaluacion
        /// </summary>
        /// <param name="idTercero">Nit de la empresa</param>
        /// <param name="idCompania">Id Compañia</param>
        /// <param name="idEmpresa">Id de la Empresa</param>
        /// <param name="cedulaJefe">Cedula Jefe</param>
        /// <param name="cedulaEmpleado">Cedula Empleado</param>
        /// <returns>Si tiene datos devuelve true</returns>
        public bool EvaluacionCompetencia(string idCompania, string idEmpresa, string cedulaJefe, string cedulaEmpleado) 
        {
            CnMysql Conexion = new CnMysql(CnObjetivos);

            try
            {
                Conexion.AbrirCnMysql();
                string consulta;

                consulta = "SELECT estadoEvaluacion FROM " + bdModCompetencias + ".evaluacioncompetencia" +
                           " WHERE idJefe = " + cedulaJefe +
                           " AND idEmpleado = " + cedulaEmpleado +
                           " AND idCompania = '" + idCompania + "'" +
                           " AND idEmpresa = '" + idEmpresa + "';";

                MySqlCommand cmd = new MySqlCommand(consulta, Conexion.ObtenerCnMysql());
                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    return string.Equals("1", rd["estadoEvaluacion"].ToString());
                }
                else
                {
                    return false;
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

       /// <summary>
       /// Consulta si la calificacion esta dentro del rango
       /// </summary>
       /// <param name="idCompania">id de la compania</param>
       /// <param name="idEmpresa">id de la empresa</param>
       /// <param name="idEvaluacionCompetencia">id de la evaluacion</param>
       /// <param name="idCargo">id del cargo</param>
       /// <param name="idCompetencia">id de la competencia</param>
       /// <returns>Estado de la calificacion</returns>
        public bool ConsultarCalificacionRango(string idCompania, string idEmpresa, string idEvaluacionCompetencia, string idCargo, string idCompetencia)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand("sp_ConsultarCalificacionRango", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idEvaluacionCompetencia", idEvaluacionCompetencia);
                cmd.Parameters.AddWithValue("@idCargo", idCargo);
                cmd.Parameters.AddWithValue("@idCompetencia", idCompetencia);

                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    int cal = Convert.ToInt32(rd["calificacion"].ToString());
                    int rMax = Convert.ToInt32(rd["rangoMax"].ToString());
                    int rMin = Convert.ToInt32(rd["rangoMin"].ToString());

                    if (cal >= rMax) 
                    {
                        return true;
                    }
                    else if (cal < rMin) 
                    {
                        return false;
                    }
                    else if (cal > rMin) 
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
                throw ex;
            }
            finally
            {
                Conexion.CerrarCnMysql();
            }
        }
        
        /// <summary>
        /// Crea una observacion
        /// </summary>
        /// <param name="cedula">Cedula de quien observa</param>
        /// <param name="observacion">texto observacion</param>
        /// <returns>true si el proceso es correcto</returns>
        public int CrearEvaluacionCompetencia(int idJefe, int idEmpleado, int idCargo, bool estadoEvaluacion, string anio, string idCompania, string idEmpresa)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                cmd = new MySqlCommand("sp_CrearEvaluacionCompentecia", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idJefe", idJefe);
                cmd.Parameters.AddWithValue("@idEmpleado", idEmpleado);
                cmd.Parameters.AddWithValue("@idCargo", idCargo);
                cmd.Parameters.AddWithValue("@estadoEvaluacion", estadoEvaluacion);
                cmd.Parameters.AddWithValue("@anio", anio);
                cmd.Parameters.AddWithValue("@idCompania", idCompania);
                cmd.Parameters.AddWithValue("@idEmpresa", idEmpresa);

                // Crea un parametro de salida para el SP
                MySqlParameter outputIdParam = new MySqlParameter("@respuesta", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(outputIdParam);
                cmd.ExecuteNonQuery();

                //Almacena la respuesta de la variable de retorno del SP
                res = int.Parse(outputIdParam.Value.ToString());

                if (res != 0)
                {
                    return res;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                Conexion.CerrarCnMysql();
            }
        }

        /// <summary>
        /// Crea una calificacion
        /// </summary>
        /// <param name="cedula">Cedula de quien observa</param>
        /// <param name="observacion">texto observacion</param>
        /// <returns>true si el proceso es correcto</returns>
        public int CrearEvalCargoCompetencias(int idEvaluacionCompetencia, int idCargo, int idCompetencia, int calificacion)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                cmd = new MySqlCommand("sp_CrearEvalCargoCompetencias", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idEvaluacionCompetencia", idEvaluacionCompetencia);
                cmd.Parameters.AddWithValue("@idCargo", idCargo);
                cmd.Parameters.AddWithValue("@idCompetencia", idCompetencia);
                cmd.Parameters.AddWithValue("@calificacion", calificacion);

                // Crea un parametro de salida para el SP
                MySqlParameter outputIdParam = new MySqlParameter("@respuesta", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(outputIdParam);
                cmd.ExecuteNonQuery();

                //Almacena la respuesta de la variable de retorno del SP
                res = int.Parse(outputIdParam.Value.ToString());

                if (res != 0)
                {
                    return res;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                Conexion.CerrarCnMysql();
            }
        }

        /// <summary>
        /// Actualizauna calificacion
        /// </summary>
        /// <param name="cedula">Cedula de quien observa</param>
        /// <param name="observacion">texto observacion</param>
        /// <returns>true si el proceso es correcto</returns>
        public int ActualizarCalificacion(int idEvaluacionCompetencia, int idCargo, int idCompetencia, int calificacion)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                cmd = new MySqlCommand("sp_ActualizarCalificacion", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idEvaluacionCompetencia", idEvaluacionCompetencia);
                cmd.Parameters.AddWithValue("@idCargo", idCargo);
                cmd.Parameters.AddWithValue("@idCompetencia", idCompetencia);
                cmd.Parameters.AddWithValue("@calificacion", calificacion);

                // Crea un parametro de salida para el SP
                MySqlParameter outputIdParam = new MySqlParameter("@respuesta", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(outputIdParam);
                cmd.ExecuteNonQuery();

                //Almacena la respuesta de la variable de retorno del SP
                res = int.Parse(outputIdParam.Value.ToString());

                if (res != 0)
                {
                    return res;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                Conexion.CerrarCnMysql();
            }
        }

        /// <summary>
        /// Crea una plan de desarrollo
        /// </summary>
        public int CrearPlanDesarrollo(string plan, DateTime fecha, int idCargo, int idCompetencia)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);
            int res = 0;
            int res2 = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                cmd = new MySqlCommand("sp_CrearPlan", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@plan", plan);
                cmd.Parameters.AddWithValue("@fechaCumplimiento", fecha);
                cmd.Parameters.AddWithValue("@estadoPlan", false);

                // Crea un parametro de salida para el SP
                MySqlParameter outputIdParam = new MySqlParameter("@respuesta", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(outputIdParam);
                cmd.ExecuteNonQuery();

                //Almacena la respuesta de la variable de retorno del SP
                res = int.Parse(outputIdParam.Value.ToString());

                //Actualizo la conexion entre eval y plan de desarrollo
                if (res != 0)
                {
                    MySqlCommand cmd2;

                    cmd2 = new MySqlCommand("sp_ActualizarIdPlan", Conexion.ObtenerCnMysql());
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@idCargo", idCargo);
                    cmd2.Parameters.AddWithValue("@idCompetencia", idCompetencia);
                    cmd2.Parameters.AddWithValue("@idPlanEstrategico", res);

                    // Crea un parametro de salida para el SP
                    MySqlParameter outputIdParam2 = new MySqlParameter("@respuesta", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd2.Parameters.Add(outputIdParam2);
                    cmd2.ExecuteNonQuery();

                    //Almacena la respuesta de la variable de retorno del SP
                    res2 = int.Parse(outputIdParam2.Value.ToString());

                    if (res2 != 0)
                    {
                        return res2;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else 
                {
                    return 0;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                Conexion.CerrarCnMysql();
            }
        }

        #endregion
    }
}