using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RedirectChecker
{
    public class RedirectChecker
    {

        public RedirectChecker()
        {
            ;
        }

        bool AssertEqualSize(double a_tot, double b_tot)
        {
            return a_tot == b_tot;
        }

        public void FindAmountOfRedirect()
        {
            var path = @"/Users/anthonybaker/Desktop/TextFiles/Redirects";
            double lineTot = File.ReadAllLines(Path.Combine(path, "initial_url")).Length;
            string[] fileA = File.ReadAllLines(Path.Combine(path, "initial_url"));
            List<int> status_codes = new List<int>();
            int _301 = 0, _200 = 0;
            int statusCode = 0;

            StringBuilder sb = new StringBuilder();

            using (StreamWriter output = new StreamWriter(Path.Combine(path, "too_many_redirects")))
            {
                output.WriteLine("# of hops, URLs start to finish");
                for (int i = 0; i < lineTot; i++)
                {
                    Console.WriteLine("Line number: " + (i + 1));
                    string location = fileA[i];
                    try
                    {
                        while (!string.IsNullOrWhiteSpace(location))
                        {
                            sb.Append(location + ", ");

                            HttpWebRequest request = HttpWebRequest.CreateHttp(location);
                            request.AllowAutoRedirect = false;
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                location = response.GetResponseHeader("Location");
                                statusCode = (int)response.StatusCode;
                                status_codes.Add(statusCode);
                            }
                            for (int j = 0; j < status_codes.Count; j++)
                            {
                                Console.WriteLine("code: " + status_codes[j]);
                            }
                        }
                    }

                    catch (System.Net.WebException e1)
                    {
                        Console.WriteLine(e1.Message);
                    }

                    foreach (var status in status_codes)
                    {
                        if (status == 301)
                        {
                            _301++;
                        }
                        else if (status == 200)
                        {
                            _200++;
                        }
                    }

                    Console.WriteLine("URLs start to finish:\n" + sb);
                    Console.WriteLine("Number of hops: " + _301);

                    if (_301 == 0)
                    {
                        output.WriteLine(_301 + ", " + sb);

                        Console.WriteLine("Failed: Zero redirects.");
                    }
                    else if (_301 > 1)
                    {
                        output.WriteLine(_301 + ", " + sb);
                        Console.WriteLine("Failed: More than 1 redirect (" + _301 + " total redirects).");
                    }
                    else if (_200 == 0)
                    {
                        Console.WriteLine("Failed: 200 status code not found.");
                    }
                    else if (_301 == 1)
                    {
                        Console.WriteLine("Success: Only 1 redirect.");
                    }

                    _301 = 0;
                    _200 = 0;
                    sb = new StringBuilder();
                    status_codes = new List<int>();
                    Console.WriteLine("==================================================");
                }
            }
        }

        public void AssertRedirects()
        {
            //Retrieves the original text file that contains unfiltered zeroed/nonzeroed map links
            var path = @"/Users/anthonybaker/Desktop/TextFiles/Redirects";

            //Contains the filtered text file with ONLY nonzeroed map links
            string location = "", initialURL = "", prevLocation = "";

            //Finds the amount of lines in the given text file.
            double lineTot_A = File.ReadAllLines(Path.Combine(path, "initial_url")).Length;
            double lineTot_B = File.ReadAllLines(Path.Combine(path, "final_url")).Length;

            //reads all lines of the original text file (var path) and stores them in an array of strings
            string[] init_url = File.ReadAllLines(Path.Combine(path, "initial_url"));
            string[] final_url = File.ReadAllLines(Path.Combine(path, "final_url"));
            int statusCode = 0;

            if (AssertEqualSize(lineTot_A, lineTot_B))
            {
                using (StreamWriter output = new StreamWriter(Path.Combine(path, "bad_redirect")))
                {
                    output.WriteLine("Initial URL, Redirected URL, Expected URL");
                    for (int i = 0; i < lineTot_A; i++)
                    {
                        location = init_url[i];
                        Console.WriteLine("Navigating to: " + location);
                        initialURL = location;
                        try
                        {

                            while (!string.IsNullOrWhiteSpace(location))
                            {
                                HttpWebRequest request = HttpWebRequest.CreateHttp(location);
                                request.AllowAutoRedirect = false;

                                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                                {
                                    prevLocation = location;
                                    location = response.GetResponseHeader("Location");

                                    statusCode = (int)response.StatusCode;
                                    if (statusCode == 200)
                                    {
                                        Console.WriteLine("Redirected to: " + prevLocation);
                                        Console.WriteLine("Expected URL:  " + final_url[i]);

                                        if (!prevLocation.Equals(final_url[i]))
                                        {
                                            output.WriteLine(initialURL + ", " + prevLocation + ", " + final_url[i]);

                                            Console.WriteLine("Bad redirect");
                                            Console.WriteLine("=================================================");
                                            Console.WriteLine("=================================================");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Successful redirect");
                                            Console.WriteLine("=================================================");
                                            Console.WriteLine("=================================================");
                                        }
                                    }
                                }
                            }
                        }
                        catch (System.Net.WebException e1)
                        {
                            Console.WriteLine(e1.Message);
                            output.WriteLine(initialURL + ", " + prevLocation + ", " + final_url[i]);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("The sizes are different.");
            }
        }

        public void AssertNo404()
        {
            var path = @"/Users/anthonybaker/Desktop/TextFiles/Redirects";
            //Contains the filtered text file with ONLY nonzeroed map links
            var resultPath = @"/Users/anthonybaker/Desktop/TextFiles/Redirects";
            string location = "", initialURL = "", prevLocation = "";
            //Finds the amount of lines in the given text file.
            double lineTot_A = File.ReadAllLines(Path.Combine(path, "urls")).Length;
            Console.WriteLine("LINE TOTAL: " + lineTot_A);
            //reads all lines of the original text file (var path) and stores them in an array of strings
            string[] fileA = File.ReadAllLines(Path.Combine(path, "urls"));

            int statusCode = 0;
            using (StreamWriter output = new StreamWriter(Path.Combine(resultPath, "404_urls")))
            {
                for (int i = 0; i < lineTot_A; i++)
                {
                    location = fileA[i];
                    Console.WriteLine("Navigating to: " + location);
                    initialURL = location;
                    try
                    {
                        while (!string.IsNullOrWhiteSpace(location))
                        {
                            HttpWebRequest request = HttpWebRequest.CreateHttp(location);
                            request.AllowAutoRedirect = false;

                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                prevLocation = location;
                                location = response.GetResponseHeader("Location");

                                statusCode = (int)response.StatusCode;

                                Console.WriteLine("Page loaded successfully.");
                                Console.WriteLine("=================================================");
                                Console.WriteLine("=================================================");
                            }
                        }
                    }
                    catch (System.Net.WebException e1)
                    {
                        Console.WriteLine(e1.Message);
                        Console.WriteLine(location);
                        Console.WriteLine("=================================================");
                        Console.WriteLine("=================================================");

                        output.WriteLine(e1.Message);
                        output.WriteLine(location);
                        output.WriteLine("=================================================");
                        output.WriteLine("=================================================");
                    }
                }
            }
        }
    }
}
