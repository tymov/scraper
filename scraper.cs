using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using CsvHelper;
using System.Linq;
using System.Text;


namespace WebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }
        }

        private static bool MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1) Youtube Search");
            Console.WriteLine("2) Jobsite");
            Console.WriteLine("3) Price Finder");
            Console.WriteLine("4) Exit");
            Console.Write("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Enter a searchterm: ");
                    var searchterm = Console.ReadLine();
                    Youtube(searchterm);
                    return true;
                case "2":
                    Console.WriteLine("Enter a searchterm: ");
                    var jobsearch = Console.ReadLine();
                    JobSite(jobsearch);
                    return true;
                case "3":
                    Console.WriteLine("Enter a productname (EAN's will be most accurate): ");
                    var EAN = Console.ReadLine();
                    PriceBol(EAN);
                    PriceCB(EAN);
                    return true;
                case "4":
                    return false;
                default:
                    return true;
            }
        }



        static void Youtube(string searchTerm)
        {
            IWebDriver driver = new ChromeDriver();

            List<Dictionary<string, string>> myList = new List<Dictionary<string, string>>();

            driver.Navigate().GoToUrl("https://www.youtube.com/results?search_query=" + searchTerm);

            var cookies = driver.FindElement(By.XPath("//*[@id=\"content\"]/div[2]/div[6]/div[1]/ytd-button-renderer[1]/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]"));
            cookies.Click();

            IWebElement title = driver.FindElement(By.XPath("//*[@id=\"video-title\"]/yt-formatted-string"));
            IWebElement uploader = driver.FindElement(By.Id("//*[@id=\"metadata-line\"]/span[1]"));
            IWebElement metadata = driver.FindElement(By.Id("//*[@id=\"text\"]/a"));

            for (int i = 0; i < 5; i++)
            {
                myList[i].Add("title", title.Text);
                myList[i].Add("author", uploader.Text);
                myList[i].Add("metadata", metadata.Text);


                Console.WriteLine("title: " + myList[i]["title"]);
                Console.WriteLine("title: " + myList[i]["author"]);
                Console.WriteLine("title: " + myList[i]["metadata"]);
                i += 1;
            }
            driver.Quit();
        }

        static void JobSite(string jobname)
        {
            // Open Site
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl("https://www.ictjob.be/");

            // Enter search
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var elementsWithSearchID = wait.Until((driver) => driver.FindElements(By.Id("keywords-input")));
            var search = elementsWithSearchID.Where(e => e.TagName == "input").FirstOrDefault();

            search.SendKeys(jobname);

            Thread.Sleep(3);
            var searchBut = driver.FindElement(By.XPath("//*[@id=\"main-search-button\"]"));
            searchBut.Submit();

            Thread.Sleep(10); 

            var title = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[1]/span[2]/a/h2"));
            var company = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[1]/span[2]/span[1]"));
            var location = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[1]/span[2]/span[2]/span[2]"));
            var keywords = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[1]/span[2]/span[3]"));
            var link = driver.FindElements(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[1]/span[2]/a"));

            var result = "Title: " + title.ToString() + "\nUploaded By: " + company.ToString() + "\nLocation: " + location.ToString() + "\nKeywords: " + keywords.ToString() + "\nLink: " + link.ToString();

            var data = new[]
            {
                title.ToString()
                , company.ToString()
                , location.ToString()
                , keywords.ToString()
                , link.ToString()
            };

            using (var writer = new StreamWriter("fileJobs.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(data);
            }

            driver.Quit();    
        }


        static void PriceBol(string EAN)
        {
            IWebDriver driver = new ChromeDriver();

            List<string> priceList = new List<string>();

            driver.Navigate().GoToUrl("https://www.bol.com/nl/nl/");

            var cookies = driver.FindElement(By.XPath("//*[@id=\"js-reject-all-button\"]"));
            cookies.Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var elementsWithSearchID = wait.Until((driver) => driver.FindElements(By.XPath("//*[@id=\"searchfor\"]")));
            var search = elementsWithSearchID.Where(e => e.TagName == "input").FirstOrDefault();

            search.SendKeys(EAN);


            var searchBut = driver.FindElement(By.XPath("//*[@id=\"siteSearch\"]/wsp-search-box-register/wsp-search-input/button"));
            searchBut.Submit();

            var price = driver.FindElement(By.XPath("//*[@id=\"js_items_content\"]/li/div[2]/wsp-buy-block/div[1]/section/div/div/meta"));
            priceList.Add(price.Text);
            
            Thread.Sleep(500);
            driver.Quit();
        }

        static void PriceCB(string EAN)
        {
            IWebDriver driver = new ChromeDriver();

            List<string> priceList = new List<string>();

            driver.Navigate().GoToUrl("https://www.coolblue.be/nl");

            var cookies = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[2]/div/div[1]/div/div[1]/form/div[2]/button"));
            cookies.Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var elementsWithSearchID = wait.Until((driver) => driver.FindElements(By.XPath("//*[@id=\"search_query\"]")));
            var search = elementsWithSearchID.Where(e => e.TagName == "input").FirstOrDefault();

            search.SendKeys(EAN);


            var searchBut = driver.FindElement(By.XPath("//*[@id=\"js-search-container\"]/div[1]/form/div/div[1]/button[2]"));
            searchBut.Submit();

            var price = driver.FindElement(By.CssSelector("#product-results > div.grid.gap--6\\@md.product-grid__products > div:nth-child(1) > div > div > div.product-card__details.product-card__custom-breakpoint.js-product-details.col--8.col--12\\@md > div.section--3.flex.justify-content--between > div.mr--2.grow--1 > span"));
            priceList.Add(price.Text);


            Console.WriteLine(priceList);

            Thread.Sleep(500);
        }

    }
}
