using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EM_Webapi
{
   public class Utility
   {
      public static void CopyPropertyValues(object source, object destination, bool copyNull = false)
      {
         var destProperties = destination.GetType().GetProperties();

         foreach (var sourceProperty in source.GetType().GetProperties())
         {
            object sourceValue = sourceProperty.GetValue(source, new object[] { });
            if (!copyNull && sourceValue == null)
               continue;

            foreach (var destProperty in destProperties)
            {
               if (destProperty.Name == sourceProperty.Name && destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
               {
                  destProperty.SetValue(destination, sourceValue, new object[] { });

                  break;
               }
            }
         }
      }
   }
}