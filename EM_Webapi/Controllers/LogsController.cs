using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using EM_Webapi.Models;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace EM_Webapi.Controllers
{
    public class LogsController : ApiController
    {
        public void Delete(int id)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Logs_Delete", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par1 = cmd.Parameters.Add("@Id", SqlDbType.Int);
                par1.Direction = ParameterDirection.Input;
                par1.Value = id;

                cmd.ExecuteNonQuery();
            }
        }

        [HttpGet]
        public List<string> Page(string param)
        {
            int page = Int32.Parse(param);

            List<string> result = new List<string>();
            List<Logs> lsLogs = GetCountAndSkip(20, 20 * page);

            var enumLogs = lsLogs.GetEnumerator();
            while (enumLogs.MoveNext())
            {
                result.Add(string.Format("{0} - {1}", enumLogs.Current.Modified.ToShortDateString(), enumLogs.Current.Content));
            }

            return result;
        }

        [HttpGet]
        public void Delete(string category, string param)
        {
            int id = Int32.Parse(param);
            Delete(id);
        }

        [HttpGet]
        public int Add(string category, string param)
        {
            return InsertLog(category, param);
        }

        public void Put(int id, [FromBody] Logs inf)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Logs_Update", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par1 = cmd.Parameters.Add("@Content", SqlDbType.VarChar, 1000);
                par1.Direction = ParameterDirection.Input;
                par1.Value = inf.Content;

                SqlParameter par2 = cmd.Parameters.Add("@Category", SqlDbType.VarChar, 50);
                par2.Direction = ParameterDirection.Input;
                par2.Value = inf.Category;

                SqlParameter par3 = cmd.Parameters.Add("@Id", SqlDbType.Int);
                par3.Direction = ParameterDirection.Input;
                par3.Value = id;

                cmd.ExecuteNonQuery();
            }
        }

        public int Post(Logs log)
        {
            return InsertLog(log.Category, log.Content);
        }

        [HttpGet]
        public List<string> Last(string category, string param)
        {
            int askedCount = Int32.Parse(param);

            List<string> result = new List<string>();
            List<Logs> ls = GetAllLogs(category);  // all logs
            int count = Math.Min(ls.Count, askedCount);

            var lsEnum = ls.GetEnumerator();

            while(count-- > 0 && lsEnum.MoveNext())
                result.Add(string.Format("{0} - {1}", lsEnum.Current.Modified.ToShortDateString() , lsEnum.Current.Content));

            return result;
        }

        [HttpGet]
        public List<string> GetLogs(string category)
        {
            List<string> result = new List<string>();
            List<Logs> ls = GetAllLogs(category);

            var lsEnum = ls.GetEnumerator();
            while (lsEnum.MoveNext())
                result.Add(string.Format("{0} - {1} - {2}", lsEnum.Current.Id.ToString(), lsEnum.Current.Modified.ToShortDateString(), lsEnum.Current.Content));

            return result;
        }

        [HttpGet]
        [ResponseType(typeof(Logs))]
        public Logs GetLogs(string category, int id)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Logs_GetById", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par1 = cmd.Parameters.Add("@ID", SqlDbType.Int);
                par1.Direction = ParameterDirection.Input;
                par1.Value = id;

                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                    return GetSqlData(rdr);
                else
                    return null;
            }
        }

        private string GetLogsString(SqlDataReader rdr)
        {
            DateTime dt = (DateTime)rdr["Modified"];
            return string.Format("{0} - {1} - {2}", rdr["Id"].ToString(), dt.ToShortDateString(), rdr["Content"].ToString());
        }
        private Logs GetSqlData(SqlDataReader rdr)
        {
            Logs log = new Logs();
            log.Id = Int32.Parse(rdr["Id"].ToString());
            log.Content = rdr["Content"].ToString();
            log.Modified = (DateTime)rdr["Modified"];

            return log;
        }
        private SqlConnection GetSqlConnection()
        {
            string connStr = ConfigurationManager.AppSettings["dbSmarterasp"];
            return new SqlConnection(connStr);
        }

        private List<Logs> GetCountAndSkip(int count, int skip)
        {
            List<Logs> result = new List<Logs>();

            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Logs_GetTopX", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par1 = cmd.Parameters.Add("@x", SqlDbType.Int);
                par1.Direction = ParameterDirection.Input;
                par1.Value = count;

                SqlParameter par2 = cmd.Parameters.Add("@skip", SqlDbType.Int);
                par2.Direction = ParameterDirection.Input;
                par2.Value = skip;

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                    result.Add(GetSqlData(rdr));
            }

            return result;
        }

        private List<Logs> GetAllLogs(string category)
        {
            List<Logs> result = new List<Logs>();

            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Logs_GetAll", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par2 = cmd.Parameters.Add("@Category", SqlDbType.VarChar, 50);
                par2.Direction = ParameterDirection.Input;
                par2.Value = category;

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                    result.Add(GetSqlData(rdr));
            }

            return result;
        }

        private int InsertLog(string category, string content)
        {
            int result = 0;

            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Logs_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par1 = cmd.Parameters.Add("@Content", SqlDbType.VarChar, 1000);
                par1.Direction = ParameterDirection.Input;
                par1.Value = content;

                SqlParameter par2 = cmd.Parameters.Add("@Category", SqlDbType.VarChar, 50);
                par2.Direction = ParameterDirection.Input;
                par2.Value = category;

                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                result = Int32.Parse(returnParameter.Value.ToString());
            }

            return result;
        }

    }
}
