using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Emzam.Log.ElkLogProvider;
using Emzam.Log.ElkLogProvider.Enum;
using Emzam.Log.ElkLogProvider.Models;
using Serilog.Sinks.ElasticEcsLogstashTest.Core;

namespace Serilog.Sinks.ElasticEcsLogstashTest.Controllers
{
    [RoutePrefix("critical-log")]
    public class CriticalLogController : Controller
    {
        private readonly ILogProvider _logProvider;

        public CriticalLogController()
        {
            _logProvider = new ElkLogProvider();
        }

        [HttpGet, Route("fake-it")]
        public JsonResult Index()
        {
            for (var i = 0; i < 700; i++)
            {
                if (i % 100 == 0)
                {
                    var app = RandomGenerator.RandomApplication();
                    _logProvider.ChangeApplication(new LogApplicationModel
                    {
                        Id = app[0],
                        Name = app[1],
                        Type = (ApplicationTypes) Enum.Parse(typeof(ApplicationTypes), app[2], true),
                        Version = app[3]
                    });
                }

                var action = RandomGenerator.RandomWarningAction();
                var username = RandomGenerator.RandomUsername();
                var status = RandomGenerator.RandomActionStatus();
                
                _logProvider.LogCritical(action[1], new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Username", username),
                        new KeyValuePair<string, string>("OrderId", RandomGenerator.RandomOrderId())
                    }, action[0]);
                
                Console.WriteLine($"User <{username}> {action[0]},{action[1]} is <{status}>");
            }
            
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        
        
    }
}