﻿using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace MAYoutubeDownload.Controllers
{
    [Route("")]
    [ApiController]
    public class Youtubedl : Controller
    {
        public string koshare;
        public string kosharemusic;
        YoutubeClient youtube;

        public async Task<IActionResult> InfoAsync(int dl, string url, string langbot, bool? jsontokhmi)
        {
            try
            {

                if (url == null)
                {
                    return BadRequest("Your link Wrong!🤔");
                }

                if (Directory.Exists("mysubdl"))
                {
                    string[] files = Directory.GetFiles("mysubdl");
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
                if (Directory.Exists("videodl") == false)
                {
                    Directory.CreateDirectory("videodl");
                }
                if (Directory.Exists("mysubdl") == false)
                {
                    Directory.CreateDirectory("mysubdl");
                }









                RestClient client = new RestClient("https://www.youtube.com/watch?v=CODPdy98e4c&bpctr=9999999999&hl=en");
                //     client.Proxy = proxyr.proxyxx();
                client.UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.101 Safari/537.36";
                RestRequest request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var result = new HttpClientHandler();
                    var proxy = new WebProxy("u1.p.webshare.io:80")
                    {
                        Credentials = new NetworkCredential("bpddjimc-1", "7e9d2isi0qjs"),

                    };
                    result = new HttpClientHandler
                    {
                        Proxy = proxy,
                        UseProxy = true
                    };
                    var httpClient = new HttpClient(result);
                    youtube = new YoutubeClient(httpClient);
                }
                else
                {
                    var cook = new CookieContainer();
                    Cookie cook1 = new Cookie("APISID", "S_8PVzLOnHi538hy/AaP8wxMz33kmasd9f", "/", ".youtube.com");
                    Cookie cook2 = new Cookie("SAPISID", "xg4QQddE82k_DDn8/AAWxTP8h4nK1WrRN1", "/", ".youtube.com");
                    Cookie cook3 = new Cookie("LOGIN_INFO", "AFmmF2swRQIhALpdcFkq91MJiZCETslO6AlFH7meJH5UJlsb3Eg6pjGHAiBL0petmyQpuH9-iJ0fPnmq1Ey6zPBlqycSyDhbCAvP4w:QUQ3MjNmeE84b3RBdF9zQ2N0QW5OMkk2cE04OTNQX19maGVmZ2s4N2V1MW5KbE1rbkNnR0ZxeGh1SElnRTB6SnJZaUw5WWc4RzNXd3NtTjVzWkQ0bUlaOVp1Z0NJaGY0cU1rQ3psWjJzenVwQ1hGODFUMDVpVjc5WUQwMzhMdHhqNmFmZ3E0Z2xYUlhfQnNUNUphU1o3bTMySGdBc01LWExR", "/", ".youtube.com");
                    Cookie cook4 = new Cookie("GOOGLE_ABUSE_EXEMPTION", "ID=b6424e8ce0a77d85:TM=1644755186:C=r:IP=135.125.150.55-:S=I5Cbo3LlyGFvuTGMSkEtTi4", "/", ".youtube.com");



                    cook.Add(cook1);
                    cook.Add(cook2);
                    cook.Add(cook3);
                    cook.Add(cook4);


                    HttpClientHandler handler = new HttpClientHandler();
                    handler.CookieContainer = cook;
                    HttpClient httpClient = new HttpClient(handler);
                    youtube = new YoutubeClient(httpClient);
                }















                var video = await youtube.Videos.GetAsync(url);
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                Regex reg = new Regex("[*'\",_&#^@:|łŁ$ß€&@#<>÷×¤*.:,?;!}{đĐ~–ˇ^˘°˛`˙■´˝¨■¸/éáűúőöüóí()-]");
                var titleauthor = reg.Replace(video.Author.Title, string.Empty).Replace(@"\", "");

                var videotitle = reg.Replace(video.Title, string.Empty).Replace(@"\", "");

                if (dl == 3 && url.StartsWith("http"))
                {
                    if (langbot == null)
                    {
                        return NotFound("use '&langbot=en' last your link 😐");
                    }

                    var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(video.Id);
                    //foreach (var sub in trackManifest.Tracks)
                    //{

                    //    //subbstring += "Lang: "+sub.Language + "\n\nAutoGenrated: " + sub.IsAutoGenerated + "\n\nurl: " + sub.Url+"\n\n------------------------------\n\n";
                    ////}
                    //if (subbstring==null)
                    //{
                    //    return NotFound("zirnevis khali");
                    //}
                    //return Ok(subbstring);

                    var trackInfo = trackManifest.TryGetByLanguage(langbot);
                    if (trackInfo == null)
                    {
                        return NotFound("Sub.Empty");
                    }

                    //randomname
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var stringChars = new char[8];
                    var random = new Random();

                    for (int i = 0; i < stringChars.Length; i++)
                    {
                        stringChars[i] = chars[random.Next(chars.Length)];
                    }

                    var finalString = new String(stringChars);
                    //zirnevis


                    string masirsub =
                        $"mysubdl/omidkhan.{titleauthor + "_" + videotitle + finalString + "_" + langbot}.srt";
                    await youtube.Videos.ClosedCaptions.DownloadAsync(trackInfo, masirsub);
                    var memoryStream = new MemoryStream();
                    var contentType = "application/omid-stream";
                    Byte[]
                        b = System.IO.File.ReadAllBytes(masirsub);
                    return File(b, contentType, fileDownloadName: masirsub.Replace("mysubdl/", ""));

                }










                if (dl == 4 && url.StartsWith("http") && jsontokhmi == null)
                {
                    return NotFound("use '&jsontokhmi=true' or false, last your link 😐");
                }
                if (dl == 4 && url.StartsWith("http") && jsontokhmi == true)
                {
                    JsonArray allArray = new JsonArray();
                    allArray.Add("Channel Name:" + video.Author.Title);
                    allArray.Add("Title:" + video.Title);
                    allArray.AddRange(streamManifest.GetAudioOnlyStreams());
                    return new JsonResult(allArray);
                }
                else if (dl == 4 && url.StartsWith("http") && jsontokhmi == false)
                {
                    foreach (var youtubee in streamManifest.GetAudioOnlyStreams())
                    {
                        kosharemusic += youtubee.Url.ToString() + " \n\n" + youtubee.AudioCodec + "       " + youtubee.Size.MegaBytes + "mb" + "\n___________________\n\n";
                    }
                    return Ok("Channel Name: " + video.Author.Title + "\nTitle: " + video.Title + "\n\n" + kosharemusic);
                }



                if (dl == 1 && url.StartsWith("http"))
                {
                    foreach (var youtubee in streamManifest.GetMuxedStreams())
                    {
                        koshare += youtubee.Url.ToString() + " \n\n" + youtubee.VideoQuality + " " +
                                   youtubee.Container.Name + " " +
                                   youtubee.Size.MegaBytes + "mb" + "\n___________________\n\n";
                    }
                    return Ok("Channel Name: " + video.Author.Title + "\nTitle: " + video.Title + "\n\n" + koshare);
                }

                if (dl == 2 && url.StartsWith("http"))
                {

                    if (System.IO.File.Exists("ffmpeg/ffmpeg.exe") == false)
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        WebClient dd = new WebClient();
                        dd.DownloadFile("https://github.com/ffbinaries/ffbinaries-prebuilt/releases/download/v4.4.1/ffmpeg-4.4.1-win-64.zip", "ffmpeg.zip");
                        var filePath = "ffmpeg.zip";
                        ZipFile.ExtractToDirectory(filePath, "ffmpeg");
                    }
                    var audioStreamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();
                    var videoStreamInfo = streamManifest.GetVideoStreams().TryGetWithHighestVideoQuality();
                    var streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
                    //randomname
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var stringChars = new char[8];
                    var random = new Random();
                    for (int i = 0; i < stringChars.Length; i++)
                    {
                        stringChars[i] = chars[random.Next(chars.Length)];
                    }

                    var finalString = new String(stringChars);


                    string path =
                        $"videodl/{videotitle}.{titleauthor}.{videoStreamInfo.VideoQuality.Label}.{videoStreamInfo.VideoQuality.Framerate}FPS.{finalString}.{videoStreamInfo.Container.Name}";

                    await youtube.Videos.DownloadAsync(streamInfos,
                        new ConversionRequestBuilder(path).SetFFmpegPath("ffmpeg/ffmpeg.exe").SetPreset(ConversionPreset.UltraFast)
                            .Build());
                    //zirnevis
                    var contentType = "application/omid-stream";
                    Byte[]
                        b = System.IO.File.ReadAllBytes(path);

                    return File(b, contentType, fileDownloadName: path.Replace("videodl/", ""));
                }

                return null;
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message + "\n" + ex.InnerException);
            }
        }
    }
}