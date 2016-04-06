using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Suyati.FeedAggreagator;
using System.Configuration;
using System.IO;

namespace BuildRSSGetLinks
{
    class Program
    {
        static void Main(string[] args)
        {
            int totalCount = 0;
            int totalMedia = 0;
            int totalUrls = 0;
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            var buildURL = ConfigurationManager.AppSettings["BuildEventRSSUrl"];

            //var isValidFeed = SyndicationFeed.IsValidFeed(buildURL);
            //if (isValidFeed)

            try
            {
                // love <3 the FeedAggreagator!!
                SyndicationFeed syndicationFeed = SyndicationFeed.Load(buildURL);
                if (syndicationFeed.FeedType == FeedType.MediaRSS)
                {
                    MediaRSSFeed rss = (MediaRSSFeed)syndicationFeed.Feed;

                    foreach (MediaRSSFeedItem item in rss.Items)
                    {
                        if (item.MediaGroup != null && item.MediaGroup.MediaContentList.Count > 0)
                        {
                            //foreach (var media in item.MediaGroup.MediaContentList)
                            //{
                            //    sb2.Append(media.Url);
                            //    sb2.Append(Environment.NewLine);
                            //}

                            totalCount++;

                            if (item.MediaGroup.MediaContentList.Any(m => m.Url.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)))
                                totalMedia++;

                            if (item.MediaGroup.MediaContentList.Any(m => m.Url.EndsWith("_high.mp4", StringComparison.OrdinalIgnoreCase)))
                            {
                                totalUrls = AppendMediaUrl(totalUrls, sb, item, "_high.mp4");
                            }
                            else if (item.MediaGroup.MediaContentList.Any(m => m.Url.EndsWith("_lg.mp4", StringComparison.OrdinalIgnoreCase)))
                            {
                                totalUrls = AppendMediaUrl(totalUrls, sb, item, "_lg.mp4");
                            }
                            else if (item.MediaGroup.MediaContentList.Any(m => m.Url.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)))
                            {
                                totalUrls = AppendMediaUrl(totalUrls, sb, item, ".mp4");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return;
            }

            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("Total Media Found : " + totalUrls);
            sb.Append(Environment.NewLine);
            sb.Append("Total Media(NotParsed) Found : " + totalMedia);
            sb.Append(Environment.NewLine);
            sb.Append("Total Count : " + totalCount);

            Console.WriteLine(sb.ToString());


            File.WriteAllText("Build_Videos_" + DateTime.Now.Ticks + ".txt", sb.ToString());
            //File.WriteAllText("Build_Videos_sb2_" + DateTime.Now.Ticks + ".txt", sb2.ToString());

            Console.ReadLine();
        }

        private static int AppendMediaUrl(int totalUrls, StringBuilder sb, MediaRSSFeedItem item, string endsWith)
        {
            totalUrls++;
            sb.Append(item.MediaGroup.MediaContentList.FirstOrDefault(m => (m.Url.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase) && !m.Url.EndsWith("_mid.mp4", StringComparison.OrdinalIgnoreCase))).Url);
            sb.Append(Environment.NewLine);
            return totalUrls;
        }
    }
}
