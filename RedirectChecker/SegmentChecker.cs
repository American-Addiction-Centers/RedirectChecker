using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace RedirectChecker {
    public class SegmentChecker {
        public SegmentChecker() {
        }

        public void AssertSegments() {
            ChromeOptions options = new ChromeOptions();
            IWebDriver driver = new ChromeDriver();

            string resultPath = @"/Users/anthonybaker/Desktop/TextFiles/DataSegments";
            using (StreamWriter output = new StreamWriter(Path.Combine(resultPath, "incorrect_data-segment.txt"))) {
                using (StreamWriter output2 = new StreamWriter(Path.Combine(resultPath, "redirected_url.txt"))) {
                    using (StreamWriter output3 = new StreamWriter(Path.Combine(resultPath, "redirected_missing_data-segment.txt"))) {
                        try {
                            string url_path = @"/Users/anthonybaker/Desktop/TextFiles/DataSegments/urls";
                            string segment_path = @"/Users/anthonybaker/Desktop/TextFiles/DataSegments/segments";


                            IJavaScriptExecutor je = (IJavaScriptExecutor)driver;
                            var wait5 = new WebDriverWait(driver, TimeSpan.FromSeconds(180));
                            string[] urls = System.IO.File.ReadAllLines(url_path);
                            string[] segments = File.ReadAllLines(segment_path);

                            output.WriteLine("URL, ACTUAL SEGMENT, EXPECTED SEGMENT");
                            output2.WriteLine("ACTUAL URL, EXPECTED URL");
                            for (int i = 0; i < urls.Length; i++) {
                                driver.Navigate().GoToUrl(urls[i]);

                                if (urls[i].Equals(driver.Url)) {
                                    IWebElement element = driver.FindElement(By.XPath("//*[@data-segment]"));
                                    if (!element.GetAttribute("data-segment").ToString().Equals(segments[i])) {
                                        output.WriteLine(urls[i] + ", " + element.GetAttribute("data-segment").ToString() + ", " + segments[i]);
                                    }
                                }
                                else {
                                    output2.WriteLine(driver.Url + ", " + urls[i]);
                                    IWebElement element = driver.FindElement(By.XPath("//*[@data-segment]"));
                                    if (!element.GetAttribute("data-segment").ToString().Equals(segments[i])) {
                                        output3.WriteLine(driver.Url + ", " + element.GetAttribute("data-segment").ToString() + ", " + segments[i]);
                                    }
                                }
                            }
                            output.Close();
                            driver.Quit();
                        }
                        catch (Exception e) {
                            output.WriteLine(e); ;
                        }
                    }
                }
            }
        }
    }
}
