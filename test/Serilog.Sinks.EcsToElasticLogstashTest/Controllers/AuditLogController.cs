using System;
using System.Collections.Generic;
using Emzam.Log.ElkLogProvider;
using Emzam.Log.ElkLogProvider.Enum;
using Emzam.Log.ElkLogProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            logProvider = new ElkLogProvider(httpContextAccessor, "https://queue.netbar.org");
        }

        [HttpGet, Route("fake-it")]
        public JsonResult Index()
        {
            for (var i = 0; i < 300; i++)
            {
                LogApplicationModel application = new LogApplicationModel();
                if (i % 100 == 0)
                {
                    var app = RandomGenerator.RandomApplication();
                    application = new LogApplicationModel
                    {
                        Id = app[0],
                        Name = app[1],
                        Type = (ApplicationTypes) Enum.Parse(typeof(ApplicationTypes), app[2], true),
                        Version = app[3],
                        Server = "e-maleki.netbar.local"
                    };
                    logProvider.SetApplication(application);
                }

                var action = RandomGenerator.RandomAuditAction();
                var username = RandomGenerator.RandomUsername();
                var status = RandomGenerator.RandomActionStatus();

                logProvider.LogAudit(action, new Dictionary<string, string>
                    {
                        {"Username", username},
                        {"AuthStatus", status},
                        {"App", JsonConvert.SerializeObject(application)}
                    },
                    "User Authentication");

                Console.WriteLine($"User <{username}> {action} is <{status}>");
            }

            return Json(true);
        }
    }
}