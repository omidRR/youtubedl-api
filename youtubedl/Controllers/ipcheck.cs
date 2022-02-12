using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using AngleSharp.Io;

namespace youtubedl.Controllers
{
    [Route("api")]
    [ApiController]
    public class ipcheck : Controller
    {
        public IActionResult About()
        {
            try
            {
                
                var addlist = Dns.GetHostEntry(Dns.GetHostName());
                string GetHostName = addlist.HostName.ToString();
                string GetIPV6 = addlist.AddressList[0].ToString();
                string GetIPV4 = addlist.AddressList[1].ToString();


                //Retrieve client IP address through HttpContext.Connection
                var ClientIPAddr = HttpContext.Connection.RemoteIpAddress?.ToString();


                return Ok( GetHostName + "\n" +
                          GetIPV4 + "\n" + GetIPV6 + "\n" + ClientIPAddr);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message + "\n" + e.InnerException);
            }

        }
    }
}