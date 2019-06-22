using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Web.Mvc;
using Emzam.Log.ElkLogProvider;
using Emzam.Log.ElkLogProvider.Enum;
using Emzam.Log.ElkLogProvider.Models;
using Serilog.Sinks.ElasticEcsLogstashTest.Core;

namespace Serilog.Sinks.ElasticEcsLogstashTest.Controllers
{
    [RoutePrefix("throughput-log")]
    public class ThroughputController : Controller
    {
        private readonly ILogProvider _logProvider;

        public ThroughputController()
        {
            _logProvider = new ElkLogProvider();
        }

        [HttpGet, Route("start")]
        public JsonResult Index()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (RandomGenerator.RandomIndex(1000) <= 800) 
                return Json(true, JsonRequestBehavior.AllowGet);

            try
            {
                throw RandomGenerator.RandomError();
            }
            catch (Exception exception)
            {
                if (!string.IsNullOrEmpty(Request.Params["le"]) && Request.Params["le"] == "yes")
                    _logProvider.LogError("Throughput test exception", exception, new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Username", "ehsan.maleki@gmail.com"),
                        new KeyValuePair<string, string>("OrderId", RandomGenerator.RandomOrderId()),
                        new KeyValuePair<string, string>("UserRole", "Admin")
                    }, "Throughput Test", Severities.Fetal);
                throw;
            }
        }
    }
}