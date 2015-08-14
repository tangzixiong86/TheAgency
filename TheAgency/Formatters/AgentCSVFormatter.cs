﻿using System.Web.Http.Routing;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Http;
using TheAgency.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace TheAgency.Formatters
{
    public class AgentCSVFormatter : MediaTypeFormatter
    {
        HttpRequestMessage _request;
        public AgentCSVFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
        }
        public AgentCSVFormatter(HttpRequestMessage request)
        {
            _request = request;
        }
        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            return new AgentCSVFormatter(request);
        }
       

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            Type enumerableType = typeof(IEnumerable<Agent>);
            return enumerableType.IsAssignableFrom(type);
        }
        public override async  System.Threading.Tasks.Task WriteToStreamAsync(Type type, object value, Stream writeStream,
            HttpContent content,TransportContext transportContext)
        {
            var agents = value as IEnumerable<Agent>;
            using (var writer = new StreamWriter(writeStream))
            {
                // Write the CSV header
                writer.WriteLine("First name,Last name,Link");
                if (agents != null)
                {
                    UrlHelper url = _request.GetUrlHelper();
                    // Write the CSV content
                    foreach (var agent in agents)
                    {
                        string agentUrl = url.Link("DefaultApi", new { id = agent.AgentID });
                        await writer.WriteLineAsync(string.Format("{0},{1},{2}", agent.FirstName, agent.LastName, agentUrl));
                    }
                }
            }
        }
    }
}
