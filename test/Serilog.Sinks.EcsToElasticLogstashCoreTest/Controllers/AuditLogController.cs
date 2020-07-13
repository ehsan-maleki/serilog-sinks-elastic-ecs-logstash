using System;
using System.Collections.Generic;
using Emzam.Log.ElkEcsLogBySerilogProvider;
using Emzam.Log.ElkEcsLogBySerilogProvider.Enum;
using Emzam.Log.ElkEcsLogBySerilogProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog.Sinks.EcsToElasticLogstashCoreTest.Core;

namespace Serilog.Sinks.EcsToElasticLogstashCoreTest.Controllers
{
    [ApiController]
    [Route("audit-log")]
    public class AuditLogController : Controller
    {
        private readonly ILogProvider logProvider;

        public AuditLogController(IHttpContextAccessor httpContextAccessor)
        {
            logProvider = new ElkLogProvider(httpContextAccessor, "http://localhost:1010");
        }

        [HttpGet, Route("fake-it")]
        public JsonResult Index()
        {
            for (var i = 0; i < 300; i++)
            {
                if (i % 100 == 0)
                {
                    var app = RandomGenerator.RandomApplication();
                    logProvider.SetApplication(new LogApplicationModel
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

                logProvider.LogAudit(action, new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Username", username),
                        new KeyValuePair<string, string>("AuthStatus", status)
                    },
                    "User Authentication");

                Console.WriteLine($"User <{username}> {action} is <{status}>");
            }

            return Json(true);
        }
    }
}