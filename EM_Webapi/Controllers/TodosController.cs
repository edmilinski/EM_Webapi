using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using EM_Webapi.Models;
using System.Collections.Generic;
using System.Web.Http.Description;
using EM_Webapi.Model;
using System.Web.Http.Cors;
using System.Net;
using System.Net.Http;

namespace EM_Webapi.Controllers
{
    public class TodosController : ApiController
   {
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        private PetaPoco.Database GetDb() {
         string connectionStringName = ConfigurationManager.AppSettings["connectionStringName"];
         return new PetaPoco.Database(connectionStringName);
      }

      [HttpGet]
      public IEnumerable<Todo> GetTodos()
      {
         using (var db = GetDb())
         {
            return db.Fetch<Todo>("SELECT * FROM Todos");
         }
      }

      [HttpGet]
      public List<Todo> GetTodos(string userid)
      {
         using (var db = GetDb())
         {
            return db.Fetch<Todo> ("SELECT * FROM Todos WHERE userid=@0", userid);
         }
      }

      [HttpGet]
      [ResponseType(typeof(Todo))]
      public Todo GetTodos(string userid, int id)
      {
         using (var db = GetDb())
         {
            return db.SingleOrDefault<Todo>("SELECT * FROM Todos WHERE userid=@0 AND id=@1", userid, id);
         }
      }

      public void Put(Todo todo)
      {
         using (var db = GetDb())
         {
            string sql = "SELECT * FROM Todos WHERE userid=@0 AND id=@1";
            var curTodo = db.SingleOrDefault<Todo>(sql, todo.userid, todo.id);

            if (curTodo == null)
               throw new ApplicationException ("No matching record found: " + sql);

            Utility.CopyPropertyValues(todo, curTodo);
            curTodo.modified = DateTime.Now;

            db.Update("Todos", "id", curTodo);
         }
      }

      [HttpPost]
      public int Post(Todo todo)
      {
         using (var db = GetDb())
         {
            todo.modified = DateTime.Now;
            if (todo.completed == null) todo.completed = false;
            if (todo.urgent == null) todo.urgent = false;

            db.Insert("Todos", "id", todo);
            return todo.id;
         }
      }

      [HttpPost]
      public List<Todo> GetFilteredTodosPage(string page, string count, [FromBody] Todo filter)
      {
         using (var db = GetDb())
         {
            int  pageIndex = Int32.Parse(page);
            int  itemCount = Int32.Parse(count);

            PetaPoco.Sql sql = new PetaPoco.Sql("SELECT * FROM Todos");

            if (filter.userid != null)
               sql.Append("WHERE userid=@0", filter.userid);

            if (filter.modified != null)
               sql.Append("WHERE modified >= @0", filter.modified);

            if (filter.completed != null)
               sql.Append("WHERE completed=@0", filter.completed);

            if (filter.urgent != null)
               sql.Append("WHERE urgent=@0", filter.urgent);

            if (filter.title != null)
               sql.Append("WHERE title like @0", "%" + filter.title + "%");

            if (filter.tags != null)
               sql.Append("WHERE tags like @0", "%" + filter.tags + "%");

            if (filter.content != null)
               sql.Append("WHERE content like @0", "%" + filter.content + "%");

            sql.OrderBy("modified DESC");

            return  db.Fetch<Todo>(pageIndex, itemCount, sql.SQL, sql.Arguments);
         }
      }

      public List<Todo> GetFilteredTodos(Todo filter)
      {
         using (var db = GetDb())
         {
            PetaPoco.Sql sql = new PetaPoco.Sql("SELECT * FROM Todos");

            if (filter.userid != null)
               sql.Append("WHERE userid=@0", filter.userid);

            if (filter.completed != null)
               sql.Append("WHERE completed=@0", filter.completed);

            if (filter.urgent != null)
               sql.Append("WHERE urgent=@0", filter.urgent);

            if (filter.title != null)
               sql.Append("WHERE title like @0", "%"+filter.title+"%");

            if (filter.tags != null)
               sql.Append("WHERE tags like @0", "%" + filter.tags + "%");

            if (filter.content != null)
               sql.Append("WHERE content like @0", "%" + filter.content + "%");

            return db.Fetch<Todo>(sql);
         }
      }

      public void Delete(int id)
      {
         using (var db = GetDb())
         {
            db.Delete("Todos", "id", null, id);
         }
      }
   }
}