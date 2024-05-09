using Amazon.SecurityToken.Model;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Service
{
    public class Influxdbproxy
    {
        private InfluxDbClient _influxClient;

        public Influxdbproxy(string url, string user, string pwd)
		{
            _influxClient = new InfluxDbClient(url, user, pwd, InfluxDbVersion.Latest);
        }

        public async void AddData(string db, string table, Dictionary<string, object> tags, Dictionary<string, object> fields)
        {
            var point = new Point()
            {
                Name = table,
                Tags = tags,
                Fields = fields,
                Timestamp = DateTime.UtcNow
            };

            _ = await _influxClient.Client.WriteAsync(point, db);
        }
    }
}

