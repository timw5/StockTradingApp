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
        public string Dates { get; set; }

        public void OnGet()
        {
            DataAccess da = new DataAccess("AAPL");
            SupportedTickers = new();
            SupportedTickers = DataAccess.GetSupportedTickers();
            Dates = DataAccess.GetRandomDate();


        }

        public IActionResult OnPostGetTickerData([FromBody]string ticker)
        {
            DataAccess da = new DataAccess(ticker);
            var data = da.BulkData.ToList();
            return new JsonResult(data);
        }


    }

}