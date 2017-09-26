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
    public class NotesController : ApiController
    {
        public void Delete(int id)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Notes_Delete", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par1 = cmd.Parameters.Add("@Id", SqlDbType.Int);
                par1.Direction = ParameterDirection.Input;
                par1.Value = id;

                cmd.ExecuteNonQuery();
            }
        }

        public void Put(int id, [FromBody] Notes inf)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Notes_Update", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par1 = cmd.Parameters.Add("@Content", SqlDbType.VarChar, 3000);
                par1.Direction = ParameterDirection.Input;
                par1.Value = inf.Content;

                SqlParameter par2 = cmd.Parameters.Add("@Tags", SqlDbType.VarChar, 20);
                par2.Direction = ParameterDirection.Input;
                par2.Value = inf.Tags;

                SqlParameter par3 = cmd.Parameters.Add("@Id", SqlDbType.Int);
                par3.Direction = ParameterDirection.Input;
                par3.Value = id;

                cmd.ExecuteNonQuery();
            }
        }

        public void Post(Notes note)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Notes_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par1 = cmd.Parameters.Add("@Content", SqlDbType.VarChar, 3000);
                par1.Direction = ParameterDirection.Input;
                par1.Value = note.Content;

                SqlParameter par2 = cmd.Parameters.Add("@Tags", SqlDbType.VarChar, 100);
                par2.Direction = ParameterDirection.Input;
                par2.Value = note.Tags;

                SqlParameter par3 = cmd.Parameters.Add("@Title", SqlDbType.VarChar, 100);
                par3.Direction = ParameterDirection.Input;
                par3.Value = note.Title;

                cmd.ExecuteNonQuery();
            }
        }
        /*
                [HttpGet]
                [ActionName("FilterContent")]
                public List<Notes> FilterContent(string filter = null)
                {
                    List<Notes> result = new List<Notes>();

                    using (SqlConnection conn = GetSqlConnection())
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("Notes_FilterContent", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (filter != null)
                        {
                            SqlParameter par1 = cmd.Parameters.Add("@contentFilter", SqlDbType.VarChar, 3000);
                            par1.Direction = ParameterDirection.Input;
                            par1.Value = filter;
                        }

                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            result.Add(GetNotes(rdr));
                        }
                    }

                    return result;
                }

                [HttpGet]
                [ActionName("FilterTags")]
                public List<Notes> FilterTags(string filter = null)
                {
                    List<Notes> result = new List<Notes>();

                    using (SqlConnection conn = GetSqlConnection())
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("Notes_FilterTags", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (filter != null)
                        {
                            SqlParameter par1 = cmd.Parameters.Add("@TagsFilter", SqlDbType.VarChar, 20);
                            par1.Direction = ParameterDirection.Input;
                            par1.Value = filter;
                        }

                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            result.Add(GetNotes(rdr));
                        }
                    }

                    return result;
                }
        */
        public List<Notes> GetNotes()
        {
            List<Notes> result = new List<Notes>();

            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Notes_GetAll", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                    result.Add(GetNotes(rdr));
            }

            return result;
        }

        // datetime param, (removed bit after dot, replaced : with _) returns records where modified > param
        public List<Notes> GetNewer(string param)
        {
            // make it again valid datetime string
            param = param.Replace('_', ':'); 

            List<Notes> result = new List<Notes>();

            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Notes_GetNewer", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par1 = cmd.Parameters.Add("@Modified", SqlDbType.DateTime);
                par1.Direction = ParameterDirection.Input;
                par1.Value = param;

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                    result.Add(GetNotes(rdr));
            }

            return result;
        }

        [HttpGet]
        [ResponseType(typeof(Notes))]
        public Notes GetNotes(int id)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Notes_GetById", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par1 = cmd.Parameters.Add("@ID", SqlDbType.Int);
                par1.Direction = ParameterDirection.Input;
                par1.Value = id;

                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                    return GetNotes(rdr);
                else
                    return null;
            }
        }

        private Notes GetNotes(SqlDataReader rdr)
        {
            Notes note = new Notes();
            note.Id = Int32.Parse(rdr["Id"].ToString());
            note.Title = rdr["Title"].ToString();
            note.Tags = rdr["Tags"].ToString();
            note.Content = rdr["Content"].ToString();
            note.Modified = (DateTime)rdr["Modified"];

            return note;
        }
        private SqlConnection GetSqlConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["Smarterasp"].ToString();
            return new SqlConnection(connStr);
        }

    }
}
