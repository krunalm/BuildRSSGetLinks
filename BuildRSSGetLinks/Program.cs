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
            int totalUrls = 0;
            StringBuilder sb = new StringBuilder();
            var buildURL = ConfigurationManager.AppSettings["BuildEventRSSUrl"];

            //var isValidFeed = SyndicationFeed.IsValidFeed(buildURL);
            //if (isValidFeed)

            try
            {
                SyndicationFeed syndicationFeed = SyndicationFeed.Load(buildURL);
                if (syndicationFeed.FeedType == FeedType.MediaRSS)
                {
                    MediaRSSFeed rss = (MediaRSSFeed)syndicationFeed.Feed;

                    foreach (MediaRSSFeedItem item in rss.Items)
                    {
                        if (item.MediaGroup != null && item.MediaGroup.MediaContentList.Count > 0)
                        {
                            var media = item.MediaGroup.MediaContentList.Where(m =>
                                                    m.Url.EndsWith("_high.mp4", StringComparison.OrdinalIgnoreCase) ||
                                                    m.Url.EndsWith("_lg.mp4", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if (media != null)
                            {
                                totalUrls++;
                                sb.Append(media.Url);
                                sb.Append(Environment.NewLine);
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

            Console.WriteLine(sb.ToString());


            File.WriteAllText("Build_Videos_" + DateTime.Now.Ticks + ".txt", sb.ToString());

            Console.ReadLine();
        }
    }
}
