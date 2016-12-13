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
        /// Devuelve el inicio de sesion
        /// </summary>
        /// "(Activo_Empleado = 'A' OR Activo_Empleado = '1') and " +
        public DataTable InicioSesion(string usuario, string pass)
        {
            CnMysql Conexion = new CnMysql(CnObjetivos);

            try
            {
                Conexion.AbrirCnMysql();
                string consulta;

                consulta = "SELECT emp.Id_Empleado, emp.Id_Rol, emp.Nombres_Empleado, " +
                           "emp.Companias_idCompania, emp.Companias_idEmpresa, emp.Externo, " +
                           "com.Terceros_Nit_Tercero FROM " + bdTrabajadores + ".empleados emp INNER JOIN " +
                           bdTrabajadores + ".companias com ON emp.Companias_idCompania = com.idCompania and " +
                           "emp.Companias_idEmpresa = com.Empresas_idEmpresa where Id_Empleado = '" + usuario +
                           "' and Contrasena_Empleado = '" + pass + "' and (Companias_idEmpresa = 'ST' " +
                           "or Companias_idEmpresa = 'SB');";

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
        /// Comprueba si la compania esta activa
        /// </summary>
        /// <returns>True si esta activo</returns>
        public bool ComprobarCompaniaActiva(string idCompania, string idEmpresa)
        {
            CnMysql Conexion = new CnMysql(CnTrabajadores);
            
            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand("SELECT Activo_Compania FROM " +
                                                    bdTrabajadores + ".companias where idCompania = '" +
                                                    idCompania + "' and Empresas_idEmpresa = '" +
                                                    idEmpresa + "'", Conexion.ObtenerCnMysql());
                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    return rd["Activo_Compania"].ToString() == "1" ? true : false;
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
        /// Comprueba si la compania tiene el modulo de objetivos activos
        /// </summary>
        /// <returns>True si esta activo</returns>
        public bool ComprobarModuloObjetivos(string idTercero, string idEmpresa)
        {
            CnMysql Conexion = new CnMysql(CnTrabajadores);

            try
            {
                MySqlCommand rolCommand = new MySqlCommand("SELECT * FROM " +
                                                            bdBasica + ".matriz_modulostercero where idTercero = '" +
                                                            idTercero + "' and idEmpresa = '" +
                                                            idEmpresa + "' and idModulo = 1", Conexion.ObtenerCnMysql());

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
        /// Comprueba si la compania tiene el modulo de objetivos activos
        /// </summary>
        /// <returns>True si esta activo</returns>
        public bool ComprobarModuloCompetencias(string idTercero, string idEmpresa)
        {
            CnMysql Conexion = new CnMysql(CnTrabajadores);

            try
            {
                MySqlCommand rolCommand = new MySqlCommand("SELECT * FROM " +
                                                            bdBasica + ".matriz_modulostercero where idTercero = '" +
                                                            idTercero + "' and idEmpresa = '" +
                                                            idEmpresa + "' and idModulo = 2", Conexion.ObtenerCnMysql());

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

        #region Objetivos

        /// <summary>
        /// Devuelve los trabajadores que tiene un jefe
        /// </summary>
        public DataTable ConsultarObservaciones(string idJefeEmpleado, string etapa)
        {
            CnMysql Conexion = new CnMysql(CnObjetivos);

            try
            {
                Conexion.AbrirCnMysql();
                string consulta;

                consulta = "SELECT *, (SELECT em.Nombres_Completos_Empleado " +
                           "FROM pru_trabajadores.empleados as em " +
                           "WHERE em.Id_Empleado = ob.Cedula) as Nombre " +
                           "FROM " + bdModobjetivos + ".observaciones as ob" +
                           " WHERE JefeEmpleado_idJefeEmpleado = " + idJefeEmpleado +
                           " AND Etapas_idEtapas = " + etapa +
                           " Order by Orden;";

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

        #endregion

        #region Competencias

        /// <summary>
        /// Devuelve los trabajadores que tiene un jefe
        /// </summary>
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
        public DataTable ConsultarCargosTrabajador(int cedula_Empleado, string ano)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd = new MySqlCommand("sp_ConsultarCargosTrabajador", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id_Empleado", cedula_Empleado);
                cmd.Parameters.AddWithValue("@estado", true);
                cmd.Parameters.AddWithValue("@anioActual", ano);

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
        /// Devuelve los planes de una competencia
        /// </summary>
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
        /// Devuelve los planes de una competencia
        /// </summary>
        public DataTable ConsultarPlanesIdPlan(int idPlanEstrategico)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);

            try
            {
                Conexion.AbrirCnMysql();
                string consulta;

                consulta = "SELECT * FROM " + bdModCompetencias + ".planestrategico p " +
                           "WHERE p.idPlanEstrategico = " + idPlanEstrategico + ";";

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
        /// Devuelve evaluacion plan
        /// </summary>
        public DataTable ConsultarEvalPlan(int idEvaluacionCompetencia)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);

            try
            {
                Conexion.AbrirCnMysql();
                string consulta;

                consulta = "SELECT * FROM " + bdModCompetencias + ".evaluacionplan p " +
                           "WHERE p.idEvaluacionCompetencia = " + idEvaluacionCompetencia + ";";

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
        /// Devuelve los seguimientos de un plan
        /// </summary>
        public DataTable ConsultarSeguimiento(int idPlanEstrategico)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);

            try
            {
                Conexion.AbrirCnMysql();
                string consulta;

                consulta = "SELECT * FROM " + bdModCompetencias + ".seguimientoplan s " +
                           "WHERE s.idPlanEstrategico = " + idPlanEstrategico;

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
        /// Consulta el estado de una competencia
        /// </summary>
        public bool EvaluacionPlan(int idPlanEstrategico)
        {
            CnMysql Conexion = new CnMysql(CnObjetivos);

            try
            {
                Conexion.AbrirCnMysql();
                string consulta;

                consulta = "SELECT estadoPlan FROM " + bdModCompetencias + ".planestrategico" +
                           " WHERE idPlanEstrategico = " + idPlanEstrategico;

                MySqlCommand cmd = new MySqlCommand(consulta, Conexion.ObtenerCnMysql());
                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    return string.Equals("1", rd["estadoPlan"].ToString());
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
        /// Crea una evaluacion para un usuario
        /// </summary>
        public int CrearEvaluacionCompetencia(int idJefe, int idEmpleado, int idCargo, int idCompetencia, bool estadoEvaluacion, string anio, string idCompania, string idEmpresa)
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
                cmd.Parameters.AddWithValue("@idCompetencia", idCompetencia);
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
        /// Crea una calificacion de para una competencia
        /// </summary>
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
        /// Actualiza una calificacion
        /// </summary>
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

        /// <summary>
        /// Crea una plan de desarrollo
        /// </summary>
        public int CrearPlanGeneral(string plan, DateTime fecha, int idEval)
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

                    cmd2 = new MySqlCommand("sp_ActualizarEvalPlan", Conexion.ObtenerCnMysql());
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@idEvaluacionCompetencia", idEval);
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
                        return res;
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

        /// <summary>
        /// Actualiza la calificacion de un plan
        /// </summary>
        public int ActualizarCalificacion(int idPlanEstrategico, string plan, DateTime fecha)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                cmd = new MySqlCommand("sp_ActualizarPlanDesarrollo", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idPlanEstrategico", idPlanEstrategico);
                cmd.Parameters.AddWithValue("@plan", plan);
                cmd.Parameters.AddWithValue("@fechaCumplimiento", fecha);

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
        /// Actualiza el estado de un plan
        /// </summary>
        public int ActualizarEstadoPlan(int idPlanEstrategico, bool estado)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                cmd = new MySqlCommand("sp_ActualizarEstadoPlan", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idPlanEstrategico", idPlanEstrategico);
                cmd.Parameters.AddWithValue("@estadoPlan", estado);

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
        /// Crea un seguimiento
        /// </summary>
        public int CrearSeguimiento(string seguimiento, DateTime fecha, int idPlanEstrategico)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                cmd = new MySqlCommand("sp_CrearSeguimiento", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@seguimiento", seguimiento);
                cmd.Parameters.AddWithValue("@fecha", fecha);
                cmd.Parameters.AddWithValue("@idPlanEstrategico", idPlanEstrategico);

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
        /// Actualiza el estado evaluacion
        /// </summary>
        public int ActualizarEstadoEvaluacion(int idEvaluacionCompetencia, bool estado)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                cmd = new MySqlCommand("sp_ActualizarEstadoEvaluacion", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idEvaluacionCompetencia", idEvaluacionCompetencia);
                cmd.Parameters.AddWithValue("@estadoEvaluacion", estado);

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

        #endregion

        #region Conductas

        /// <summary>
        /// Devuelve los planes de una competencia
        /// </summary>
        public DataTable ConsultarConductasCompetencia(int idCargo, int idCompetencia, string idCompania, string idEmpresa, string ano)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);

            try
            {
                Conexion.AbrirCnMysql();
                string consulta;

                consulta = "SELECT ccc.idCarConCom, ccc.idConducta, cond.conducta, " +
                           "(SELECT eval.calificacion FROM " + bdModCompetencias + ".evalcompconducta eval " +
                           "WHERE eval.idCarConCom = ccc.idCarConCom) as calificacion " +
                           "FROM " + bdModCompetencias + ".cargocompconductas ccc INNER JOIN " +
                            bdModCompetencias + ".conductas cond ON ccc.idConducta = cond.idConducta " +
                           "WHERE ccc.idCargo = " + idCargo + " AND ccc.idCompetencia = " + idCompetencia +
                           " AND ccc.idCompania = '" + idCompania + "' AND ccc.idEmpresa = '" + idEmpresa +
                           "' AND ccc.ano = " + ano + " AND ccc.estado = 1;";

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
        /// Crea una calificacion de para una competencia
        /// </summary>
        public bool CrearEvalCompConductas(DataTable dtCarConCom, int calificacion)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                if (dtCarConCom != null)
                {
                    foreach (DataRow row in dtCarConCom.Rows)
                    {
                        string idCarConCom = row["idCarConCom"].ToString();

                        cmd = new MySqlCommand("sp_CrearEvalCompConducta", Conexion.ObtenerCnMysql());
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@idCarConCom", idCarConCom);
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
                    }
                }

                if (res != 0)
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
                throw E;
            }
            finally
            {
                Conexion.CerrarCnMysql();
            }
        }

        /// <summary>
        /// Actualiza una calificacion de conducta
        /// </summary>
        public int ActualizarCalConducta(int idCarConCom, int calificacion)
        {
            CnMysql Conexion = new CnMysql(CnCompetencias);
            int res = 0;

            try
            {
                Conexion.AbrirCnMysql();
                MySqlCommand cmd;

                cmd = new MySqlCommand("sp_ActualizarCalConducta", Conexion.ObtenerCnMysql());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCarConCom", idCarConCom);
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

        #endregion
    }
}