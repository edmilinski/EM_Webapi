using System;

namespace EM_Webapi.Models
{
   public class Todos
   {
      public int Id { get; set; }
      public DateTime Modified { get; set; }
      public string User { get; set; }
      public string Tags { get; set; }
      public string Title { get; set; }
      public bool Urgent { get; set; }
      public string Content { get; set; }
   }
}

