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
            return BulkData.Where(x => x.date == date).Select(x => x.close).First();
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

        public decimal GetAverage_Open()
        {
            return BulkData.Average(x => x.open);
        }

        public decimal GetAverage_Close()
        {
            return BulkData.Average(x => x.close);
        }

        public decimal GetAverage_High()
        {
            return BulkData.Average(x => x.high);
        }

        public decimal GetAverage_Low()
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

            var dates =  db.Select(x=>x.date).ToList();
            List<DateTime> result = new();
            foreach(var d in dates)
            {
                result.Add(Str_To_date(d));
            }
            result = result.OrderBy(x => x.Date).Distinct().ToList();
            return result;
        }

        private static DateTime Str_To_date(string d)
        {
            DateTime result = DateTime.ParseExact(d, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return result;
        }



    }
}
