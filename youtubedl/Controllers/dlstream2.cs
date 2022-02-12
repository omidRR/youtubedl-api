using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace dlhome
{
    [ApiController]
    [Route("dl2")]
    public class HomeController : Controller
    {
        public async Task Index(string dl)
        {
            //Create a stream for the file
            Stream stream = null;

            //This controls how many bytes to read at a time and send to the client
            int bytesToRead = 100;

            // Buffer to read bytes in chunk size specified above
            byte[] buffer = new Byte[bytesToRead];

            // The number of bytes read
            try
            {
                //Create a WebRequest to get the file
                HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create(dl);
                try
                {
                    var headerRange = HttpContext.Request.Headers.FirstOrDefault(r => r.Key == "Range");
                    if (headerRange.Key == "Range")
                    {
                        fileReq.Headers.Add(headerRange.Key, headerRange.Value);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }


                //Create a response for this request
                HttpWebResponse fileResp = (HttpWebResponse)await fileReq.GetResponseAsync();

                if (fileReq.ContentLength > 0)
                    fileResp.ContentLength = fileReq.ContentLength;

                //Get the Stream returned from the response
                stream = fileResp.GetResponseStream();

                // prepare the response to the client. resp is the client Response
                //  var resp = HttpContext.Response;

                //Indicate the type of data being sent
                HttpContext.Response.ContentType = "application/octet-stream";

                //Name the file 
                HttpContext.Response.Headers.Add("Content-Disposition", "attachment; filename=\"" + "fileName" + "\"");
                HttpContext.Response.Headers.Add("Content-Length", fileResp.ContentLength.ToString());
                if (fileResp.Headers["Content-Range"] != "" && fileResp.Headers["Content-Range"] != null)
                {
                    HttpContext.Response.Headers.Add("Content-Range", fileResp.Headers["Content-Range"]);
                }

                int length;
                do
                {
                    // Verify that the client is connected.
                    if (!HttpContext.RequestAborted.IsCancellationRequested)
                    {
                        // Read data into the buffer.
                        length = await stream.ReadAsync(buffer, 0, bytesToRead);

                        // and write it out to the response's output stream
                        await HttpContext.Response.Body.WriteAsync(buffer, 0, length);


                        //Clear the buffer
                        buffer = new Byte[bytesToRead];
                    }
                    else
                    {
                        // cancel the download if client has disconnected
                        length = -1;
                    }
                } while (length > 0); //Repeat until no data is read
            }
            finally
            {
                if (stream != null)
                {
                    //Close the input stream

                    stream.Close();
                }
            }


        }

    }
}
