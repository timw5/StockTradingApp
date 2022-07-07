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
        [BindProperty]
        public int balance { get; set; }
        public void OnGet()
        {
            SupportedTickers = new();
            SupportedTickers = DataAccess.GetSupportedTickers();
            Dates = DataAccess.GetRandomDate();
            HttpContext.Session.SetInt32("cents", 1000000);
            balance = 10000;
            

        }

        public IActionResult OnPostGetTickerData([FromBody]string ticker)
        {
            DataAccess da = new DataAccess(ticker);
            var data = da.BulkData.ToList();
            return new JsonResult(data);
        }

        public IActionResult OnPostMinusFunds([FromBody]int amnt)
        {
            var balance = HttpContext.Session.GetInt32("cents");

            int newamnt = (int)(balance - amnt);
            
            HttpContext.Session.SetInt32("cents", newamnt);
            return new JsonResult(newamnt);

        }


    }

}