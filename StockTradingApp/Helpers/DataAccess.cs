using Microsoft.EntityFrameworkCore;
using StockTradingApp.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StockTradingApp.Helpers
{
    public class DataAccess
    {
        public DataAccess(string ticker)
        {
            Conn = new sliceofbreadContext();
            DB = Conn.StockData;
            Ticker = ticker;
            BulkData = DB.Where(x => x.ticker == ticker);
        }

        public string Ticker { get; set; }
        public sliceofbreadContext Conn { get; set; }
        public DbSet<StockData> DB { get; set; }
        public IQueryable<StockData> BulkData { get; set; }

        public decimal GetOpen_Price(string date)
        {
            return BulkData.Where(x => x.date == date).Select(x => x.open).First();
        }

        public decimal GetClose_Price(string date)
        {
            string corrected = date;
            if (BulkData.Where(x => x.date == date).Select(x => x.close).Count() == 0)
            {
                var d = Str_To_date(date);
                d.AddDays(2);
                corrected = d.ToString("yyyy-MM-dd");
            }
            return BulkData.Where(x => x.date == corrected).Select(x => x.close).First();

        }

        public decimal GetAdj_Close_Price(string date)
        {
            return BulkData.Where(x => x.date == date).Select(x => x.adjClose).First();
        }
        public decimal GetHigh(string date)//lol..
        {
            return BulkData.Where(x => x.date == date).Select(x => x.high).First();
        } 

        public decimal GetLow(string date)
        {
            return BulkData.Where(x => x.date == date).Select(x => x.low).First();

        }
        
        public double GetVolume(string date)
        {
            return BulkData.Where(x => x.date == date).Select(x => x.volume).First();
        }

        public decimal GetAvg_Open()
        {
            return BulkData.Average(x => x.open);
        }

        public decimal GetAvg_Close()
        {
            return BulkData.Average(x => x.close);
        }

        public decimal GetAvg_High()
        {
            return BulkData.Average(x => x.high);
        }

        public decimal GetAvg_Low()
        {
            return BulkData.Average(x => x.low);
        }



        public static List<string> GetSupportedTickers()
        {
            List<string> tickers = new();
            sliceofbreadContext Conn = new();
            DbSet<StockData> db = Conn.StockData;

            return db.Select(x => x.ticker).Distinct().ToList();
        }

        public static List<DateTime> GetDates()
        {
            List<string> tickers = new();
            sliceofbreadContext Conn = new();
            DbSet<StockData> db = Conn.StockData;

            var dates =  db.Select(x=>x.date).Distinct().ToList();
            List<DateTime> result = new();
            foreach(var d in dates)
            {
                result.Add(Str_To_date(d));
            }
            result = result.OrderBy(x => x.Date).ToList();
            return result;
        }

        public static DateTime Str_To_date(string d)
        {
            DateTime result = DateTime.ParseExact(d, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return result;
        }

        public static string GetRandomDate()
        {
            sliceofbreadContext Conn = new();
            DbSet<StockData> db = Conn.StockData;
            Random rand = new Random();
            DateTime six_months_ago = DateTime.Now.AddMonths(-7);
            var dates = db.Select(x => x.date).Distinct().ToList();
            var datesDateformat = new List<DateTime>();
            foreach (var date in dates)
            {
                datesDateformat.Add(Str_To_date(date));
            }
            
            var final = datesDateformat.Where(x => x <= six_months_ago).ToList();
            int count = final.Count();
            int random = rand.Next(0, count);
            return final[random].ToString("yyyy-MM-dd");
            



        }



    }
}
