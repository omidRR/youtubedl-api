﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
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

        public async Task<IActionResult> InfoAsync(int dl, string url, string langbot)
        {
            try
            {
                if (url == null)
                {
                    return BadRequest("Your link Wrong!🤔");
                }


                if (Directory.Exists("videodl") == false)
                {
                    Directory.CreateDirectory("videodl");
                }
                if (Directory.Exists("mysubdl") == false)
                {
                    Directory.CreateDirectory("mysubdl");
                }


                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync(url);
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                Regex reg = new Regex("[*'\",_&#^@:|łŁ$ß€&@#<>÷×¤*.:,?;!}{đĐ~–ˇ^˘°˛`˙■´˝¨■¸/éáűúőöüóí()-]");
                var titleauthor = reg.Replace(video.Author.Title, string.Empty).Replace(@"\", "");

                var videotitle = reg.Replace(video.Title, string.Empty).Replace(@"\", "");

                if (dl == 3 && url.StartsWith("https://"))
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

                if (dl == 1 && url.StartsWith("https://"))
                {
                    foreach (var youtubee in streamManifest.GetMuxedStreams())
                    {
                        koshare += youtubee.Url.ToString() + " \n\n" + youtubee.VideoQuality + " " +
                                   youtubee.Container.Name + " " +
                                   youtubee.Size.MegaBytes + "mb" + "\n___________________\n\n";
                    }
                    return Ok("Channel Name: " + video.Author.Title + "\nTitle: " + video.Title + "\n\n" + koshare);
                }

                if (dl == 2 && url.StartsWith("https://"))
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