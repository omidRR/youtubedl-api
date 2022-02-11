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
    public class ValuesController : Controller
    {
        public IActionResult About()
        {
            try
            {
                IPAddress address = Dns.GetHostAddresses("localhost")
                    .Where(x => x.AddressFamily == AddressFamily.InterNetwork).First();



                IPAddress ip;
                var headers = Request.Headers.ToList();
                if (headers.Exists((kvp) => kvp.Key == "X-Forwarded-For"))
                {
                    // when running behind a load balancer you can expect this header
                    var header = headers.First((kvp) => kvp.Key == "X-Forwarded-For").Value.ToString();
                    // in case the IP contains a port, remove ':' and everything after
                    ip = IPAddress.Parse(header.Remove(header.IndexOf(':')));
                }
                else
                {
                    // this will always have a value (running locally in development won't have the header)
                    ip = Request.HttpContext.Connection.RemoteIpAddress;
                }

                var addlist = Dns.GetHostEntry(Dns.GetHostName());
                string GetHostName = addlist.HostName.ToString();
                string GetIPV6 = addlist.AddressList[0].ToString();
                string GetIPV4 = addlist.AddressList[1].ToString();


                //Retrieve client IP address through HttpContext.Connection
                var ClientIPAddr = HttpContext.Connection.RemoteIpAddress?.ToString();


                return Ok("host ip: " + address.ToString() + "\n" + ip.ToString() + "\n" + GetHostName + "\n" +
                          GetIPV4 + "\n" + GetIPV6 + "\n" + ClientIPAddr);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message + "\n" + e.InnerException);
            }

        }
    }
}