using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

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


                return Ok(GetHostName + "\n" +
                          GetIPV4 + "\n" + GetIPV6 + "\n" + ClientIPAddr);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message + "\n" + e.InnerException);
            }

        }
    }

    public class cc : Controller
    {


        [Route("dl")]
        public async Task<FileStreamResult> dd(string dl)
        {
            try
            {
                if (dl.StartsWith("http"))
                {
                    Uri uri = new Uri(dl);
                    string fname = System.IO.Path.GetFileName(uri.LocalPath);

                    HttpClient client = new HttpClient();
                    var fileName = Path.GetFileName(fname);

                    var stream = await client.GetStreamAsync(dl).ConfigureAwait(false);
                    // note that at this point stream only contains the header the body content is not read

                    return new FileStreamResult(stream, "application/omid-stream")
                    {
                        FileDownloadName = fileName
                    };
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}