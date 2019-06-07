using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

/* 
 * @author: Ryan Obman
 * Program to return a list of companies whos RSS Feeds have been inactive for a given amount of days.
 * 
 * In the Main method, I have some test data for the function. There is also a Post class for each new RSS post. I am using LINQ to XML to read RSS Urls
 * Assumption 1: The dictionary is using <string, List<string>> datatypes to store multiple values (links) under one key 
 * Assumption 2: The datetime format for pubDate is "ddd, dd MMM yyyy HH:mm:ss zzz" This works for the links I have given, but not all RSS feeds.
 * The function InactiveCompanies returns a list of companies that have not been active for the given number of days (daysOfInactivity)
*/
namespace RSSActivity1
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // Implimenting dictionary to test function
            IDictionary<string, List<string>> rssDict = new Dictionary<string, List<string>>();

            rssDict["BillMaher"] = new List<string> { "http://billmaher.hbo.libsynpro.com/rss" };
            rssDict["DianeRehm"] = new List<string> { "https://dianerehm.org/rss/npr/dr_podcast.xml" };
            rssDict["HackerNews"] = new List<string> { "https://hnrss.org/newcomments", "https://hnrss.org/jobs" };

            // Given days to test function
            int daysOfInactivity = 7;


            Console.WriteLine("Companies inactive for {0} days: {1}", daysOfInactivity, string.Join(", ", InactiveCompanies(rssDict, daysOfInactivity)));
            Console.ReadLine();
        }


        //Assuming dictionary is using <string, List<string>> datatype to keep multiple URLs under a single Company
        //Assuming PublishDate format is "ddd, dd MMM yyyy HH:mm:ss zzz". I know different feeds use many different date formats.
        public static List<string> InactiveCompanies (IDictionary<string, List<string>> rssDict, int days)
        {
            List<string> companies = new List<string>();
            DateTime currentDay = DateTime.Now;

            foreach (KeyValuePair<string, List<string>> item in rssDict)
            {
                foreach (string link in item.Value)
                {
                    if (!companies.Contains(item.Key))
                    {
                        XDocument doc = XDocument.Load(link);
                        var newPost = (from element in doc.Descendants("item")
                                       select new Post
                                       {
                                           Company = item.Key,
                                           PubDate = DateTime.ParseExact(element.Element("pubDate").Value, "ddd, dd MMM yyyy HH:mm:ss zzz", 
                                           CultureInfo.InvariantCulture, DateTimeStyles.None)
                                       });
                        DateTime latestDate = newPost.Max(r => r.PubDate);
                        int difference = (currentDay - latestDate).Days;
                        if (days <= difference)
                        {
                            companies.Add(item.Key);
                        }
                        
                    }
                    
                }                
            }
            return companies;
        }
    }

    //Post class for each RSS post
    public class Post
    {
        public string Company;
        public DateTime PubDate;
    }
}
