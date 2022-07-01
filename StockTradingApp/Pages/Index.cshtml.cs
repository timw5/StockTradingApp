using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTradingApp.Helpers;

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
        public List<DateTime> Dates { get; set; }

        public void OnGet()
        {
            DataAccess da = new DataAccess("AAPL");
            var x = da.GetAvg_High();
            var y = da.GetOpen_Price("2022-01-10");
            SupportedTickers = new();
            SupportedTickers = DataAccess.GetSupportedTickers();
            Dates = new();
            Dates = DataAccess.GetDates();

        }
    }
}