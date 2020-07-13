using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Emzam.Log.ElkLogProvider;
using Emzam.Log.ElkLogProvider.Enum;
using Emzam.Log.ElkLogProvider.Models;
using Serilog.Sinks.ElasticEcsLogstashTest.Core;

namespace Serilog.Sinks.EcsToElasticLogstashClassicTest.Controllers
{
    [RoutePrefix("error-log")]
    public class ErrorLogController  : Controller
    {
        private readonly ILogProvider _logProvider;

        public ErrorLogController ()
        {
            _logProvider = new ElkLogProvider();
        }

        [HttpGet, Route("fake-it")]
        public JsonResult Index()
        {
            for (var i = 0; i < 2000; i++)
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

                var action = RandomGenerator.RandomErrorAction();
                var username = RandomGenerator.RandomUsername();
                
                _logProvider.LogError(action[1], new Exception(action[1]), new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Username", username),
                        new KeyValuePair<string, string>("OrderId", RandomGenerator.RandomOrderId())
                    }, action[0], RandomGenerator.RandomSeverity());
                
                Console.WriteLine($"User <{username}> {action[0]}, {action[1]}");
            }
            
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        
        
    }
}