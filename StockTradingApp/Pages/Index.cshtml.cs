﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTradingApp.Helpers;
using System.Text.Json;
using System.Web.Helpers;

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
        public List<string> SupportedTickers { get; set; }
        [BindProperty]
        public string StartingDate { get; set; }
        [BindProperty]
        public int balance { get; set; }
        public void OnGet()
        {
            SupportedTickers = new();
            SupportedTickers = DataAccess.GetSupportedTickers();
            StartingDate = DataAccess.GetRandomDate();
            HttpContext.Session.SetString("date", StartingDate);
            HttpContext.Session.SetInt32("cents", 1000000);
            balance = 10000;
            
            
        }
        
        public IActionResult OnPostGetTickerData([FromBody]dynamic? data)
        {
            var result= JsonSerializer.Deserialize<Dictionary<string, string>>(data);
            string ticker = result["ticker"];
            string date = result["date"];
            DataAccess da = new DataAccess(ticker);
            var idx = da.BulkData.Where(x => x.date == date).Select(x => x.index).First();
            var dat = da.BulkData.Where(x => x.index < idx && x.ticker == ticker);
            return new JsonResult(dat);
        }
        
        public IActionResult OnPostMinusFunds([FromBody]dynamic? data)
        {
            var x = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
            var balance = HttpContext.Session.GetInt32("cents");

            int newamnt = (int)(balance - int.Parse(x["amnt"]));

            HttpContext.Session.SetInt32("cents", newamnt);
            return new JsonResult(newamnt);

        }


    }

}