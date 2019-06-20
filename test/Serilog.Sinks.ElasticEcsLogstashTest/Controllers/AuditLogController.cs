using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Emzam.Log.ElkLogProvider;
using Emzam.Log.ElkLogProvider.Enum;
using Emzam.Log.ElkLogProvider.Models;
using Serilog.Sinks.ElasticEcsLogstashTest.Core;

namespace Serilog.Sinks.ElasticEcsLogstashTest.Controllers
{
    [RoutePrefix("audit-log")]
    public class AuditLogController : Controller
    {
        private readonly ILogProvider _logProvider;

        public AuditLogController()
        {
            _logProvider = new ElkLogProvider();
        }

        [HttpGet, Route("fake-it")]
        public JsonResult Index()
        {
            for (var i = 0; i < 300; i++)
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

                var action = RandomGenerator.RandomAuditAction();
                var username = RandomGenerator.RandomUsername();
                var status = RandomGenerator.RandomActionStatus();

                _logProvider.LogAudit(action, new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Username", username),
                        new KeyValuePair<string, string>("AuthStatus", status)
                    },
                    "User Authentication");

                Console.WriteLine($"User <{username}> {action} is <{status}>");
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}