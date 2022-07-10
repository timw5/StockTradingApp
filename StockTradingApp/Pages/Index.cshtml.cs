using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTradingApp.Helpers;
using System.Text.Json;
using Newtonsoft.Json;
using System.Linq;

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
                var result = JsonConvert.DeserializeObject<IDictionary<string, string>>(data.ToString());
                //var result = JsonConvert.DeserializeObject(data);
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

        //buy
        public IActionResult OnPostMinusFunds([FromBody]dynamic? data)
        {
            if(HttpContext.Session.Get("cents") is null || HttpContext.Session.Get("date") is null || data is null)
            {
                return new JsonResult("");
            }
            try
            {

                var x = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.ToString());

                DataAccess stock = new(x["ticker"]);
                Dictionary<string, string> response = new();

                var date = DataAccess.Str_To_date(HttpContext.Session.GetString("date"));
                decimal prevAssets = 0;
                if (HttpContext.Session.Get("assets") is not null)
                {
                    prevAssets = (decimal)HttpContext.Session.GetInt32("assetsCents") / 100;
                }
                else
                {
                    prevAssets = 10000;
                }

                var new_date = date.AddDays(7);
                var balance = HttpContext.Session.GetInt32("cents");
                int newbalance = (int)(balance - int.Parse(x["amnt"]));
                var prevclosePrice = stock.GetClose_Price(date.ToString("yyyy-MM-dd"));
                var newclosePrice = stock.GetClose_Price(new_date.ToString("yyyy-MM-dd"));
                var amnt = int.Parse(x["amnt"]);

                //getting the updated asset value is tricky
                decimal quantity = ((Decimal.Parse(x["amnt"])/100) / prevclosePrice);
                decimal CurrentStockValue = quantity * newclosePrice;
                decimal PreviousStockValue = quantity * prevclosePrice;
                decimal Net = CurrentStockValue - PreviousStockValue;
                amnt = newclosePrice * quantity;
                
                decimal assets = (decimal)CurrentStockValue + (decimal)(newbalance)/(decimal)100.0;
                
                int assetsCents = (int)(assets * 100);

                HttpContext.Session.SetString("date", date.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetInt32("cents", newbalance);
                HttpContext.Session.SetInt32("Assets", assetsCents);
                

                response.Add("amnt", newbalance.ToString());
                response.Add("quantity", quantity.ToString());
                response.Add("assets", assets.ToString());
                response.Add("date", date.ToString("yyyy-MM-dd"));

                return new JsonResult(response);
            }
            catch
            {
                return new JsonResult("");
            }
        }
        
        public IActionResult OnGetTimewarp(dynamic? data)
        {
            //var mydata = JsonConvert.DeserializeObject<List<Root>>(data.ToString());
            //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(data);

            if (HttpContext.Session.Get("Assets") is null)
            {
                TimeWarp();
            }
            else if(HttpContext.Session.Get("date") is null || HttpContext.Session.Get("cents") is null)
            {
                return new JsonResult("");
            }
            else
            {
                TimeWarp();
                var balance = HttpContext.Session.GetInt32("cents");
                var PrevAssets = HttpContext.Session.GetInt32("Assets")/100;
                var date = HttpContext.Session.GetString("date");

                //very messy...idk how else to get the data from the post lol
                var x = this.HttpContext.Request.Query.ToList();

                //use ORM Deserialization for json Array
                var myObj = JsonConvert.DeserializeObject<Root>(x[1].Key);

                decimal CurrentValueOfAllStocks = 0;
                foreach (Data d in myObj.data)
                {
                    var temp = new DataAccess(d.ticker);
                    var quantity = decimal.Parse(d.quantity);
                    var ClosePrice = temp.GetClose_Price(date);
                    CurrentValueOfAllStocks += quantity * ClosePrice;
                }
                decimal assets = (decimal)CurrentValueOfAllStocks + (decimal)(balance)/(decimal)100;
                int assetsCents = (int)(assets * 100);
                HttpContext.Session.SetInt32("Assets", assetsCents);
                return new JsonResult(assets);
            }

            return new JsonResult("none");

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

        [Serializable]
        public class Data
        {
            public string id { get; set; }
            public string date { get; set; }
            public string ticker { get; set; }
            public string amnt { get; set; }
            public string quantity { get; set; }
        }

        public class Root
        {
            public List<Data> data { get; set; }
        }
    }

}