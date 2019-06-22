using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Serilog.Sinks.ElasticEcsLogstashTest.Controllers
{
    public class HomeController : Controller
    {
        public JsonResult Index()
        {
            return Json(new
            {
                Message = "Welcom to elasticsearch ecs to logstash test api",
                Urls = new
                {
                    TroughputWithoutErrorTest = Url.Action("Index", "Throughput"),
                    TroughputWithErrorTest = Url.Action("Index", "Throughput", new { re = "yes"}),
                    AuditLogTest = Url.Action("Index", "AuditLog"),
                    InformationLogTest = Url.Action("Index", "InformationLog"),
                    DebugLogTest = Url.Action("Index", "DebugLog"),
                    WarningLogTest = Url.Action("Index", "WarningLog"),
                    CriticalLogTest = Url.Action("Index", "CriticalLog"),
                    FetalLogTest = Url.Action("Index", "FetalLog"),
                    ErrorLogTest = Url.Action("Index", "ErrorLog")
                }
            }, JsonRequestBehavior.AllowGet);
        }
    }
}