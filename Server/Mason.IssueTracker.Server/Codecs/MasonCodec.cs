﻿using MasonBuilder.Net;
using Newtonsoft.Json;
using OpenRasta.Codecs;
using OpenRasta.Web;
using System.IO;


namespace Mason.IssueTracker.Server.Codecs
{
  [MediaType("application/vnd.mason+json;q=0.9", "ms")]
  [MediaType("application/json;q=1", "json")]
  public abstract class MasonCodec<T> : IMediaTypeWriter
  {
    #region Dependencies

    public ICommunicationContext CommunicationContext { get; set; }

    public IMasonBuilderContext MasonBuilderContext { get; set; }

    #endregion


    public object Configuration { get; set; }


    public void WriteTo(object resource, IHttpEntity response, string[] parameters)
    {
      if (resource == null)
        return;

      Resource representation = ConvertToMason((T)resource);

      JsonSerializer serializer = new JsonSerializer();
      serializer.NullValueHandling = NullValueHandling.Ignore;
      using (StreamWriter sw = new StreamWriter(response.Stream))
      using (JsonWriter jw = new JsonTextWriter(sw))
      {
        serializer.Serialize(jw, representation);
      }
    }


    protected abstract MasonBuilder.Net.Resource ConvertToMason(T resource);
  }
}
