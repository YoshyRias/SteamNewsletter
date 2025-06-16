using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamNewsletterLib
{
    public class RawgFilters
    {

        public string curDateStartString {  get; set; }
        public string curDateEndString { get; set;}
        public string order { get; set; } 
        public string page_size { get; set; } 
        public string platforms { get; set; } 
        public string apiKey { get; set; } 

        public RawgFilters() 
        {
            DateTime currentDate = DateTime.Now; // Get the current date
            // ChatGPT Prompt: Can you format my time to the start and end of the current month
            DateTime startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime endDate = startDate.AddMonths(1);

            curDateStartString = startDate.ToString("yyyy-MM-dd");
            curDateEndString = endDate.ToString("yyyy-MM-dd");

            order = "-rating";
            page_size = "40";
            platforms = "4";
            try
            {
                using (StreamReader sr = new StreamReader("rawg-api-key.txt"))
                {
                    apiKey = sr.ReadLine();
                }
            }
            catch
            {

            }
        }


        public override string ToString()
        {
            return $"Filter Debug: {page_size} {platforms} {curDateStartString} {curDateEndString}";
        }
    }
}
