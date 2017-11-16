using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using EM_Webapi.Models;
using System.Collections.Generic;
using System.Web.Http.Description;
using System.Net.Http;
using System.Net;
using System.Text;

namespace EM_Webapi.Controllers
{
    public class LogsController : ApiController
    {
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }
        private PetaPoco.Database GetDb()
        {
            string connectionStringName = ConfigurationManager.AppSettings["connectionStringName"];
            return new PetaPoco.Database(connectionStringName);
        }

        public void Delete(int id)
        {
            using (var db = GetDb())
            {
                db.Delete("Logs", "Id", id);
            }
        }


        [HttpGet]
        public int Add(string category, string param)
        {
            using (var db = GetDb())
            {
                return (int)db.Insert(new Logs { Category=category, Content=param });
            }
        }

        public void Put(int id, [FromBody] Logs log)
        {
            using (var db = GetDb())
            {
                log.Modified = DateTime.Now;
                db.Update(log);
            }
        }

        public int Post(Logs log)
        {
            using (var db = GetDb())
            {
                log.Modified = DateTime.Now;
                return (int)db.Insert(log);
            }
        }

        [HttpGet]
        [ResponseType(typeof(Logs))]
        public Logs GetLogs(string category, int id)
        {
            using (var db = GetDb())
            {
                return db.Single<Logs>("SELECT * FROM Logs WHERE Category=@0 AND Id=@1", category,id);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetLogsTextByCategory(string category)
        {
            StringBuilder sb = new StringBuilder();
            List<Logs> lsData = GetLogsByCategory(category);
            lsData.ForEach(x => sb.Append($"{x.Id} - {x.Modified.ToShortDateString()} - {x.Content}\r\n"));

            var resp = new HttpResponseMessage(HttpStatusCode.OK);

            resp.Content = new StringContent(sb.ToString(), System.Text.Encoding.UTF8, "text/plain");
            return resp;
        }

        [HttpGet]
        public List<Logs> GetLogsByCategory(string category)
        {
            using (var db = GetDb())
            {                    
                return db.Fetch<Logs>("SELECT * FROM Logs WHERE Category = @0 ORDER BY Id DESC", category);
            }
        }
    }
}
