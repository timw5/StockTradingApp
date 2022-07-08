using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTradingApp.Helpers;
using System.Text.Json;

namespace StockTradingApp.Pages
{

    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;

        }


        [BindProperty]
        public List<string>? SupportedTickers { get; set; }
        [BindProperty]
        public string? StartingDate { get; set; }
        [BindProperty]
        public int? balance { get; set; }
        public void OnGet()
        {
            SupportedTickers = new();
            SupportedTickers = DataAccess.GetSupportedTickers();
            
            StartingDate = DataAccess.GetRandomDate();
            
            HttpContext.Session.Remove("date");
            HttpContext.Session.Remove("cents");
            HttpContext.Session.SetString("date", StartingDate);
            HttpContext.Session.SetInt32("cents", 1000000);
            
            balance = 10000;
            
            
        }
        
        public IActionResult OnPostGetTickerData([FromBody]dynamic? data)
        {
            if (HttpContext.Session.Get("date") is null || data is null)
            {
                return new JsonResult("");
            }
            try
            {
                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
                string ticker = result["ticker"];
                string date_ = HttpContext.Session.GetString("date");

                DataAccess da = new DataAccess(ticker);
                var idx = da.BulkData.Where(x => x.date == date_ && x.ticker == ticker).Select(x => x.index).First();
                var dat = da.BulkData.Where(x => x.index < idx && x.ticker == ticker);

                return new JsonResult(dat);
            }
            catch
            {
                return new JsonResult("");
            }

            

        }
        
        public IActionResult OnPostMinusFunds([FromBody]dynamic? data)
        {
            if(HttpContext.Session.Get("cents") is null || HttpContext.Session.Get("date") is null || data is null)
            {
                return new JsonResult("");
            }
            try
            {
                var x = JsonSerializer.Deserialize<Dictionary<string, string>>(data);

                var date = HttpContext.Session.GetString("date");
                var balance = HttpContext.Session.GetInt32("cents");

                int newamnt = (int)(balance - int.Parse(x["amnt"]));
                var dateFormat = DataAccess.Str_To_date(date);
                var new_date = dateFormat.AddDays(7);

                HttpContext.Session.SetString("date", new_date.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetInt32("cents", newamnt);

                return new JsonResult(newamnt);
            }
            catch
            {
                return new JsonResult("");
            }
        }

        public IActionResult OnPostTimewarp()
        {
            TimeWarp();
            return new JsonResult("");

        }
        
        private void TimeWarp()
        {
            if (HttpContext.Session.Get("date") is null)
            {
                return;
            }
            var date = HttpContext.Session.GetString("date");
            var dateFormat = DataAccess.Str_To_date(date);
            var new_date = dateFormat.AddDays(7);
            HttpContext.Session.SetString("date", new_date.ToString("yyyy-MM-dd"));
            
        }


    }

}