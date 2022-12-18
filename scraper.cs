using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Internal;

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
            Console.WriteLine("3) NBA Data");
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
                    Console.WriteLine("Enter a player's first name: ");
                    var name = Console.ReadLine();
                    Console.WriteLine("Enter a player's surname: ");
                    var surname = Console.ReadLine();
                    Basketball(name, surname);
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
            int count = 0;

            driver.Navigate().GoToUrl("https://www.youtube.com/results?search_query=" + searchTerm + "&sp=CAI%253D");

            var cookies = driver.FindElement(By.XPath("//*[@id=\"content\"]/div[2]/div[6]/div[1]/ytd-button-renderer[1]/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]"));
            cookies.Click();

            var titles = driver.FindElements(By.XPath("//*[@id=\"video-title\"]/yt-formatted-string"));
            var views = driver.FindElements(By.XPath("//*[@id=\"metadata-line\"]/span[1]"));
            var releases = driver.FindElements(By.XPath("//*[@id=\"metadata-line\"]/span[2]"));
            var uploaders = driver.FindElements(By.XPath("//*[@id=\"channel-info\"]"));
            var links = driver.FindElements(By.XPath("//*[@id=\"video-title\"]"));

            while (count < 5)
            {
                Console.WriteLine("");
                string title = titles[count].Text;
                string view = views[count].Text;
                string release = releases[count].Text;
                string uploader = uploaders[count].Text;
                string link = links[count].GetAttribute("href");
                Console.WriteLine("Title: " + title);
                Console.WriteLine("Views: " + view);
                Console.WriteLine("Released: " + release);
                Console.WriteLine("Uploaded by: " + uploader);
                Console.WriteLine("Link: " + link);
                count++;
            }


            Console.ReadLine();
            driver.Quit();
        }

        public static void addRecord(string value1, string value2, string value3, string value4, string value5, string filepath)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepath, true))
                {
                    file.WriteLine(value1 + ";" + value2 + ";" + value3 + ";" + value4 + ";" + value5);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("It didn't write: ", ex);
            }

        }

        public static void JobSite(string jobname)
        {
            // Open Site
            IWebDriver driver = new FirefoxDriver();
            int count = 1;
            List<Dictionary<string, string>> dataList = new List<Dictionary<string, string>>();

            driver.Navigate().GoToUrl("https://www.ictjob.be/");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var elementsWithSearchID = wait.Until((driver) => driver.FindElements(By.Id("keywords-input")));
            var search = elementsWithSearchID.Where(e => e.TagName == "input").FirstOrDefault();

            search.SendKeys(jobname);

            var searchBut = driver.FindElement(By.XPath("//*[@id=\"main-search-button\"]"));
            searchBut.Submit();

            Thread.Sleep(40000);

            var cookies = driver.FindElement(By.XPath("//*[@id=\"body-ictjob\"]/div[2]/a"));
            cookies.Click();

            var date = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[1]/div[2]/div/div[2]/span[2]/a"));
            date.Click();


            Thread.Sleep(20000);

            while (count < 7)
            {
                if (count == 4)
                {
                    count += 1;
                }
                Console.WriteLine("");
                string title = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + count + "]/span[2]/a/h2")).Text;
                string company = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + count + "]/span[2]/span[1]")).Text;
                string location = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + count + "]/span[2]/span[2]/span[2]/span/span")).Text;
                string keyword = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + count + "]/span[2]/span[3]")).Text;
                string link = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + count + "]/span[2]/a")).GetAttribute("href");

                Console.WriteLine("Title: " + title);
                Console.WriteLine("Company: " + company);
                Console.WriteLine("Located in: " + location);
                Console.WriteLine("Keywords: " + keyword);
                Console.WriteLine("Link: " + link);

                addRecord(title, company, keyword, location, link, "jobs.csv");
                count++;
            }

            Console.ReadLine();
            driver.Quit();
        }

        public static void addRecords(string value1, string value2, string value3, string value4, string value5, string value6, string value7, string filepath)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepath, true))
                {
                    file.WriteLine("Name" + ";"  + value1);
                    file.WriteLine("Season"+ ";" + "PPG" + ";" + "RPG" + ";" + "APG");
                    file.WriteLine("Current Season" + ";" + value2 + ";" + value3 + ";" + value4);
                    file.WriteLine("Carreer" + ";" + value5 + ";" + value6 + ";" + value7);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("It didn't write: ", ex);
            }

        }

        public static void addHtml(string value1, string value2, string value3, string value4, string value5, string value6, string value7, string filepath, string value8, string value9)
        {
            try
            {
                using (System.IO.StreamWriter ofile = new System.IO.StreamWriter(@filepath, true))
                {
                    ofile.WriteLine("<html>");
                    ofile.WriteLine("<body style='background-color: #D3D3D3'>");

                    ofile.WriteLine("<h2>" + "Player Information" + "</h2>");
                    ofile.WriteLine("<p>" + "Name: " + value1 + "</p>");
                    ofile.WriteLine("<p>" + "Born on: " + value8 + "</p>");
                    ofile.WriteLine("<p>" + "Team: " + value9 + "</p>");

                    ofile.WriteLine("<h2>" + "Player Statistics" + "</h2>");
                    ofile.WriteLine("<table style='width:75%; border-collapse: collapse; text-align: center;'>");
                    ofile.WriteLine("<tr>");
                    ofile.WriteLine("<th>" + "Season" + "</th>");
                    ofile.WriteLine("<th>" + "Points" + "</th>");
                    ofile.WriteLine("<th>" + "Rebounds" + "</th>");
                    ofile.WriteLine("<th>" + "Assists" + "</th>");
                    ofile.WriteLine("</tr>");

                    ofile.WriteLine("<tr>");
                    ofile.WriteLine("<td>" + "Current Season" + "</td>");
                    ofile.WriteLine("<td>" + value2 + "</td>");
                    ofile.WriteLine("<td>" + value3 + "</td>");
                    ofile.WriteLine("<td>" + value4 + "</td>");
                    ofile.WriteLine("</tr>");

                    ofile.WriteLine("<tr>");
                    ofile.WriteLine("<td>" + "Carreer" + "</td>");
                    ofile.WriteLine("<td>" + value5 + "</td>");
                    ofile.WriteLine("<td>" + value6 + "</td>");
                    ofile.WriteLine("<td>" + value7 + "</td>");
                    ofile.WriteLine("</tr>");

                    ofile.WriteLine("</table>");
                    ofile.WriteLine("</body>");
                    ofile.WriteLine("</html>");
                    ofile.Close();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("It didn't write: ", ex);
            }        
        }


        static void Basketball(string name, string surname)
        {
            IWebDriver driver = new FirefoxDriver();
            driver.Manage().Window.Maximize();

            string firstLet = surname.Substring(0, 1);
            string lastFirst = "";
            if (surname.Length >= 5)
            {
                 lastFirst = surname.Substring(0, 5) + name.Substring(0, 2);
            }
            else
            {
                lastFirst = surname.Substring(0, 4) + name.Substring(0, 2);
            }
           

            driver.Navigate().GoToUrl("https://www.basketball-reference.com/players/"+ firstLet.ToLower() +"/" + lastFirst.ToLower() + "01.html");

            Thread.Sleep(3000);

            var cookies = driver.FindElement(By.XPath("//*[@id=\"qc-cmp2-ui\"]/div[2]/div/button[3]"));
            cookies.Click();

            var details = driver.FindElement(By.XPath("//*[@id=\"meta_more_button\"]"));
            details.Click();

            var playerName = driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div[1]/div[2]/h1/span")).Text;
            var dateOfBirth = driver.FindElement(By.XPath("//*[@id=\"necro-birth\"]")).Text;
            var teams = driver.FindElement(By.XPath("//*[@id=\"per_game.2023\"]")).Text;
            var currentStat = teams.Substring(0, 7) + " " + teams.Substring(11, 11);
            string team = currentStat.Substring(8, 7);

            Console.WriteLine("");
            Console.WriteLine("Player Details");
            Console.WriteLine("Name: " + playerName);
            Console.WriteLine("Born on: " + dateOfBirth);
            Console.WriteLine("Team: " + currentStat);

            var currentSeason = driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div[4]/div[1]/div/p[1]/strong")).Text;
            var games = driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div[4]/div[2]/div[1]/p[1]")).Text;
            var ppg = driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div[4]/div[2]/div[2]/p[1]")).Text;
            var trb = driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div[4]/div[2]/div[3]/p[1]")).Text;
            var apg = driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div[4]/div[2]/div[4]/p[1]")).Text;

            var cgames = driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div[4]/div[2]/div[1]/p[2]")).Text; 
            var cppg = driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div[4]/div[2]/div[2]/p[2]")).Text; 
            var ctrb = driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div[4]/div[2]/div[3]/p[2]")).Text; 
            var capg = driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div[4]/div[2]/div[4]/p[2]")).Text;

            Console.WriteLine("\nPlayer Statistics");
            Console.WriteLine("\nCurrent Season: \t" + currentSeason + "\nGames Played: \t\t" + games + "\nPoints Per Game: \t" + ppg + "\nRebounds Per Game: \t" + trb + "\nAssists Per Game: \t" + apg);
            Console.WriteLine("\nCarreer: \t" + "\nGames Played: \t\t" + cgames + "\nPoints Per Game: \t" + cppg + "\nRebounds Per Game: \t" + ctrb + "\nAssists Per Game: \t" + capg);

            addRecords(playerName, ppg, trb, apg, cppg, ctrb, capg, surname + name + "_Stats.csv");
            addHtml(playerName, ppg, trb, apg, cppg, ctrb, capg, surname + name + "_stats.html", dateOfBirth, team);

            var jsonWriter = new StringBuilder();
            jsonWriter.AppendLine("\"Name\":\"" + playerName + "\",");
            jsonWriter.AppendLine("\"Team\":\"" + team + "\",");
            jsonWriter.AppendLine("\"Season\":\"" + currentSeason + "\",");
            jsonWriter.AppendLine("\"Points\":\"" + ppg + "\",");
            jsonWriter.AppendLine("\"Rebounds\":\"" + trb + "\",");
            jsonWriter.AppendLine("\"Assists\":\"" + apg + "\",");
            jsonWriter.AppendLine("\"Carreer\":\"" + "\",");
            jsonWriter.AppendLine("\"Points\":\"" + cppg + "\",");
            jsonWriter.AppendLine("\"Rebounds\":\"" + ctrb + "\",");
            jsonWriter.AppendLine("\"Assists\":\"" + capg + "\",");
            File.WriteAllText("C:\\Users\\Tymo\\source\\repos\\Webscraper\\Webscraper\\bin\\Debug\\net6.0\\" + surname + name + "_stats.json", json.ToString());

            Console.ReadLine();
            driver.Quit();
        }

    }
}
