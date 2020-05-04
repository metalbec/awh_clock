using System;
using Microsoft.AspNetCore.Mvc;
using Clockwork.API.Models;
using System.Collections.Generic;

namespace Clockwork.API.Controllers
{
    [Route("api/[controller]")]
   
    public class CurrentTimeController : Controller
    {
        // GET api/currenttime
        [HttpGet]
        public IActionResult Get([FromQuery] string ti)
        {
            var utcTime = DateTime.UtcNow;
            var serverTime = DateTime.Now;
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();
            TimeZoneInfo timeZone = TimeZoneInfo.Utc;
            if(ti != "-1")
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(ti);
                utcTime = TimeZoneInfo.ConvertTime(utcTime, timeZone);
            }
            var returnVal = new CurrentTimeQuery
            {
                UTCTime = utcTime.ToString(),
                ClientIp = ip,
                Time = serverTime.ToString(),
                TimeZone = timeZone.DisplayName
            };

            using (var db = new ClockworkContext())
            {
                db.CurrentTimeQueries.Add(returnVal);
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine();
                foreach (var CurrentTimeQuery in db.CurrentTimeQueries)
                {
                    Console.WriteLine(" - {0}", CurrentTimeQuery.UTCTime);
                }
            }

            return Ok(returnVal);
        }

        [HttpGet("GetRequests")]
        public IActionResult GetRequests()
        {
    
            LinkedList<CurrentTimeQuery> requestList = new LinkedList<CurrentTimeQuery>();
            using (var db = new ClockworkContext())
            {

                foreach (var CurrentTimeQuery in db.CurrentTimeQueries)
                {
                    
                    requestList.AddFirst(CurrentTimeQuery);
                }
            }
            return Ok(requestList);
        }

        [HttpGet("GetTimeZones")]
        public IActionResult GetTimeZones()
        {
            List<TimeZoneInfo> zoneList = new List<TimeZoneInfo>();

                foreach (var tzi in TimeZoneInfo.GetSystemTimeZones())
                {
                    zoneList.Add(tzi);
                }
           
            return Ok(zoneList);
        }

    }
}
