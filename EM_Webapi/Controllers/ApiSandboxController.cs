﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Configuration;
using EM_Webapi.Model;

namespace EM_Webapi.Controllers
{
  public class ApiSandboxController : ApiController
  {
    [HttpGet]
    [Route("ApiSandbox/col")]
    public List<string> GetCollectionNames()
    {
      return _dicAllCollections.Select(x => x.Key).ToList();
    }

    [Route("ApiSandbox/{collection}")]
    public List<JObject> Get(string collection)
    {
      Dictionary<string, JObject> dicCollection = GetCollection(collection);
      return dicCollection.Values.ToList();
    }

    [Route("ApiSandbox/{collection}/{id}")]
    public JObject Get(string collection, string id)
    {
      return getItem(GetCollection(collection), id);
    }

    [Route("ApiSandbox/{collection}")]
    public HttpResponseMessage Post(string collection, [FromBody]JObject jsonbody)
    {
      if (jsonbody["id"] == null)
      {
        long newId = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
        jsonbody["id"] = newId.ToString();
      }
      string id = jsonbody["id"].ToString();
      setItem(GetCollection(collection), id, jsonbody);
      return new HttpResponseMessage(HttpStatusCode.Created);
    }

    [Route("ApiSandbox/{collection}/{id}")]
    public void Put(string collection, string id, [FromBody]JObject jsonbody)
    {
      jsonbody["id"] = id;
      setItem(GetCollection(collection), id, jsonbody);
    }

    [Route("ApiSandbox/{collection}/{id}")]
    public void Delete(string collection, string id)
    {
      deleteItem(GetCollection(collection), id);
    }

    [HttpGet]
    [Route("ApiSandbox/dbc")]
    public List<JsonCollection> GetDbCollectionInfo()
    {
      using (var db = GetDb())
      {
        return db.Fetch<JsonCollection>("SELECT name,groupname,modified FROM JsonCollections");
      }
    }

    [HttpGet]
    [Route("ApiSandbox/dbc/gsave/{group_name}")]
    public void dbcollection_group_save(string group_name)
    {
      dbcollection_group_delete(group_name);

      foreach ( var dicColl in _dicAllCollections)
      {
        dbcollection_save(dicColl.Key, dicColl.Key, group_name);
      }
    }

    [HttpGet]
    [Route("ApiSandbox/dbc/gload/{group_name}")]
    public int dbcollection_group_load(string group_name)
    {
      using (var db = GetDb())
      {
        List<JsonCollection> dbCollections = db.Fetch<JsonCollection>("SELECT * FROM JsonCollections WHERE GroupName=@0", group_name);
        foreach (JsonCollection item in dbCollections)
        {
          CollectionLoad(item.Name, item.Value);
        }

        return dbCollections.Count;
      }
    }

    [HttpGet]
    [Route("ApiSandbox/dbc/gdelete/{group_name}")]
    public void dbcollection_group_delete(string group_name)
    {
      using (var db = GetDb())
      {
        db.Execute("DELETE FROM JsonCollections WHERE GroupName = @0", group_name);
      }
    }

    [HttpGet]
    [Route("ApiSandbox/dbc/delete/{name}")]
    public int dbcollection_delete(string name)
    {
      using (var db = GetDb())
      {
        return db.Execute("DELETE FROM JsonCollections WHERE Name = @0 AND GroupName IS NULL", name);
      }
    }

    [HttpGet]
    [Route("ApiSandbox/dbc/save/{coll_name}/{target_name}")]
    public void dbcollection_save(string coll_name, string target_name, string group_name = null)
    {
      StringBuilder sb = new StringBuilder("[");
      Dictionary<string, JObject> coll = GetCollection(coll_name);
      foreach (JObject item in coll.Values)
      {
        sb.Append(item.ToString() + ",");
      }
      sb.Append("]");
      using (var db = GetDb())
      {
        var existingRecord = db.SingleOrDefault<JsonCollection>("WHERE Name = @0 AND GroupName = @1", target_name, group_name);
        if (existingRecord != null)
          db.Update(new JsonCollection { Id = existingRecord.Id, GroupName = group_name, Name = target_name, Value = sb.ToString(), Modified = DateTime.Now });
        else
          db.Insert(new JsonCollection { Name = target_name, GroupName = group_name, Value = sb.ToString(), Modified=DateTime.Now });
      }
    }

    [HttpGet]
    [Route("ApiSandbox/dbc/load/{coll_name}/{src_name}")]
    public int dbcollection_load(string coll_name, string src_name)
    {
      Dictionary<string, JObject> dicColl = GetCollection(coll_name);
      dicColl.Clear();

      using (var db = GetDb())
      {
        var dbColl = db.SingleOrDefault<JsonCollection>("SELECT * FROM JsonCollections WHERE Name=@0", src_name);

        if (dbColl != null)
        {
          int itemCount = CollectionLoad(coll_name, dbColl.Value);
          return itemCount;
        }
      }

      return 0;
    }

    [HttpPost]
    [Route("ApiSandbox/{collection}/bulkload")]
    public void BulkLoad(string collection, [FromBody]JObject[] jsonbody)
    {
      Dictionary<string, JObject> dicCollection = GetCollection(collection);
      dicCollection.Clear();

      foreach (JObject item in jsonbody)
        setItem(dicCollection, item);
    }

    private PetaPoco.Database GetDb()
    {
      string connectionStringName = ConfigurationManager.AppSettings["connectionStringName"];
      return new PetaPoco.Database(connectionStringName);
    }

    private static Dictionary<string, Dictionary<string, JObject>> _dicAllCollections = new Dictionary<string, Dictionary<string, JObject>>();

    private Dictionary<string, JObject> GetCollection(string name)
    {
      name = name.ToLower();
      if (!_dicAllCollections.ContainsKey(name))
        _dicAllCollections.Add(name, new Dictionary<string, JObject>());
      return _dicAllCollections[name];
    }

    private void setItem(Dictionary<string, JObject> dicCollection, string id, JObject value)
    {
      if (dicCollection.ContainsKey(id))
        dicCollection[id] = value;
      else
        dicCollection.Add(id, value);
    }

    private void setItem(Dictionary<string, JObject> dicCollection, JObject value)
    {
      if (value["id"] == null)
      {
        long newId = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
        value["id"] = newId.ToString();
      }
      string id = value["id"].ToString();
      setItem(dicCollection, id, value);
    }

    private JObject getItem(Dictionary<string, JObject> dicCollection, string id)
    {
      if (dicCollection.ContainsKey(id))
        return dicCollection[id];
      else
        return null;
    }

    private bool deleteItem(Dictionary<string, JObject> dicCollection, string id)
    {
      if (dicCollection.ContainsKey(id))
      {
        dicCollection.Remove(id);
        return true;
      }
      else
        return false;
    }

    private int CollectionLoad(string coll_name, string value)
    {
      Dictionary<string, JObject> dicColl = GetCollection(coll_name);
      dicColl.Clear();

      int itemCount = 0;

      JArray arr = JArray.Parse(value);
      foreach (JObject item in arr)
      {
        itemCount++;
        var id = item["id"];
        if (id != null)
        {
          string sId = id.ToString();
          if (!dicColl.ContainsKey(sId))
            dicColl.Add(sId, item);
        }
      }
      return itemCount;
    }
  }
}
