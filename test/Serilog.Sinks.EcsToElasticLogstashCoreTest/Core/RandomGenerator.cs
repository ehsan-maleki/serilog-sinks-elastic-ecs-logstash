using System;
using System.ComponentModel.Design;
using System.Data;
using Emzam.Log.ElkLogProvider.Enum;

namespace Serilog.Sinks.EcsToElasticLogstashCoreTest.Core
{
    public static class RandomGenerator
    {
        private static readonly Random Randomizer = new Random();

        public static int RandomIndex(int max)
            => Randomizer.Next(0, max);

        public static string[] RandomApplication()
        {
            var index = RandomIndex(6);
            var ids = new[]
            {
                "JOBSRV", "RABAGT",
                "WEBAPI", "RESAPI", "PNWAPI",
                "WBFRNT", "PNFRND"
            };
            var names = new[]
            {
                "Job Windows Service", "RabbitMQ Agent Service",
                "Web API", "Resellers API", "Panel Web API",
                "Website Frontend (nux)", "Panel Fontend"
            };
            return new[]
            {
                ids[index],
                names[index],
                index <= 1 ? "Api" : index <= 4 ? "WindowsService" : "Website",
                "1.0.0"
            };
        }

        public static string RandomUsername()
        {
            var categories = new[] {"EhsanMaleki", "RezaAbbasi", "Soldier2019", "RedApple", "NewIceCream"};
            return categories[RandomIndex(4)];
        }

        public static Severities RandomSeverity()
        {
            var index = RandomIndex(500);
            if (index <= 100)
                return Severities.Low;

            if (index > 200 && index <= 300)
                return Severities.High;

            if (index > 300 && index <= 400)
                return Severities.Critical;

            if (index > 400 && index <= 500)
                return Severities.Fetal;

            return Severities.Normal;
        }

        public static string RandomOrderId()
        {
            return Randomizer.Next(120000, 1000000).ToString();
        }

        public static string RandomActionStatus()
        {
            var index = RandomIndex(100);
            var categories = new[] {"Success", "Failed"};
            return categories[index < 50 ? 0 : 1];
        }

        public static string RandomAuditAction()
        {
            var categories = new[] {"User Signup", "User Login", "User Forget Password", "User Change Password", "User New Password", "User LogOut"};
            return categories[RandomIndex(5)];
        }

        public static string RandomInfoAction()
        {
            var categories = new[]
            {
                "User payed order", "User got voucher", "User requested for cancellation", "User cancellation approved",
                "User refused payment", "User checked in", "User checked out"
            };
            return categories[RandomIndex(5)];
        }
        
        public static string[] RandomDebugAction()
        {
            var index = RandomIndex(6);
            var category = new[]
            {
                "Coin Management", 
                "Mobile verification"
            };
            var action = new[]
            {
                "New coins assigned to user", "User earned coins on voucher", "User lost coins on cancellation", 
                "User mobile changed", "Mobile confirm code sent", "User mobile confirmed", "User mobile confirm failed" 
            };
            return new []
            {
                category[index <= 2 ? 0 : 1],
                action[index]
            };
        }

        public static Exception RandomError()
        {
            var exceptions = new Exception[]
            {
                new ArgumentException("Invalid argument"), 
                new CheckoutException("There is a config exception!!"), 
                new ConstraintException("Invalid constraint!!"),  
                new NullReferenceException("Object reference is null!!"), 
                new AggregateException("Unable to aggregated!!"), 
                new ArithmeticException("Wow, this is an arithmetic exception!!"), 
                new FormatException("Invalid format!!"), 
                new Exception("This is a general error!!"), 
            };
            
            return exceptions[RandomIndex(exceptions.Length - 1)];
        }

        public static string[] RandomErrorAction()
        {
            var index = RandomIndex(6);
            var category = new[]
            {
                "Place Details", 
                "User Authentication", 
                "Home Page", 
                "Booking"
            };
            var action = new[]
            {
                "Place detail failed", "Place availability failed", 
                "User login failed", "User signup failed", "User forgot password failed", "User change password failed", 
                "Home suggestion failed", "Home search failed", "Home places section read failed", "Home accommodations section read failed", "Home experinces section read failed", 
                "Booking start failed", "New order by user failed", "Charge payment failed", "Online payment failed", "Order finalization failed" 
            };
            return new []
            {
                category[index <= 1 ? 0 : index <= 5 ? 1 : index <= 10 ? 2 : 3],
                action[index]
            };
        }

        public static string[] RandomWarningAction()
        {
            var index = RandomIndex(5);
            var category = new[]
            {
                "iCompany Account Balance", 
                "iCompany Transfer"
            };
            var action = new[]
            {
                "Unable to get balance", "Account balance is low", "Account balance is zero!", 
                "Unable to transfer", "Transfer rejected", "Transfer failed" 
            };
            return new []
            {
                category[index <= 2 ? 0 : 1],
                action[index]
            };
        }

        public static string[] RandomFetalAction()
        {
            var index = RandomIndex(5);
            var category = new[]
            {
                "Orders", 
                "Payments"
            };
            var action = new[]
            {
                "User cannot create order", "User cannot pay order", "User unable to get voucher", 
                "Unable to get contact with pay system", "Supplier transfer request failed", "Manual transfer has eceptions" 
            };
            return new []
            {
                category[index <= 2 ? 0 : 1],
                action[index]
            };
        }
    }
}