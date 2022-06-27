using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace RedirectChecker {
    public class RedirectChecker {
        public RedirectChecker() {
            ;
        }

        public void Test() {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://staging.withdrawal.net/aac/accreditations");
            
            webRequest.AllowAutoRedirect = false;
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            Console.WriteLine("Initial URL: " + webResponse.Headers["Location"]);
            int redirCount = 0;
            while (webResponse.StatusCode == HttpStatusCode.TemporaryRedirect ||
                   webResponse.StatusCode == HttpStatusCode.MovedPermanently ||
                   webResponse.StatusCode == HttpStatusCode.MultipleChoices ||
                   webResponse.StatusCode == HttpStatusCode.Found ||
                   webResponse.StatusCode == HttpStatusCode.SeeOther) {
                string location = webResponse.Headers["Location"];

                redirCount++;
                Console.Out.WriteLine("Redirection location: {0}", location);
                Console.WriteLine("Redirect count: " + redirCount);

                webRequest = (HttpWebRequest)WebRequest.Create(location);
                webRequest.AllowAutoRedirect = false;

                webResponse = (HttpWebResponse)webRequest.GetResponse();
            }
        }

        public void FindAmountOfRedirect() {
            var path = @"/Users/anthonybaker/Desktop/TextFiles/Redirects";
            //double lineTot = File.ReadAllLines(Path.Combine(path, "initial_url")).Length;
            //string[] fileA = File.ReadAllLines(Path.Combine(path, "initial_url"));
            List<int> status_codes = new List<int>();
            int _301 = 0, _200 = 0;
            int statusCode = 0;

            StringBuilder sb = new StringBuilder();

            using (StreamWriter output = new StreamWriter(Path.Combine(path, "too_many_redirects"))) {
                output.WriteLine("# of hops, URLs start to finish");
                for (int i = 0; i < 1; i++) {
                    Console.WriteLine("Line number: " + (i + 1));
                    string location = "https://staging.withdrawal.net/aac/accreditations/";
                    try {
                        // while (!string.IsNullOrWhiteSpace("https://staging.withdrawal.net/aac/accreditations/"))
                        //{
                        //sb.Append(location + ", ");

                        HttpWebRequest request = HttpWebRequest.CreateHttp(location);
                        request.AllowAutoRedirect = false;
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                            location = response.GetResponseHeader("Location");
                            Console.WriteLine(location);
                            statusCode = (int)response.StatusCode;
                            status_codes.Add(statusCode);
                        }
                        for (int j = 0; j < status_codes.Count; j++) {
                            Console.WriteLine("code: " + status_codes[j]);
                        }
                        //}
                    }

                    catch (System.Net.WebException e1) {
                        Console.WriteLine(e1.Message);
                    }

                    foreach (var status in status_codes) {
                        if (status == 301) {
                            _301++;
                        }
                        else if (status == 200) {
                            _200++;
                        }
                    }

                    Console.WriteLine("URLs start to finish:\n" + sb);
                    Console.WriteLine("Number of hops: " + _301);

                    if (_301 == 0) {
                        output.WriteLine(_301 + ", " + sb);

                        Console.WriteLine("Failed: Zero redirects.");
                    }
                    else if (_301 > 1) {
                        output.WriteLine(_301 + ", " + sb);
                        Console.WriteLine("Failed: More than 1 redirect (" + _301 + " total redirects).");
                    }
                    else if (_200 == 0) {
                        Console.WriteLine("Failed: 200 status code not found.");
                    }
                    else if (_301 == 1) {
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

        public void AssertRedirects() {
            /* get length of each file to make sure they're the same size */
            int init_count = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "initial_url")).Length;
            int final_count = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "final_url")).Length;

            string[] initial_url = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "initial_url"));
            string[] expected_url = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "final_url"));

            using (StreamWriter output = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "bad_redirect"))) {
                output.WriteLine("Initial URL, Redirected URL, Expected URL");
                if (init_count == final_count) {
                    for (int i = 0; i < init_count; i++) {
                        Console.WriteLine("Line #:" + (i + 1));
                        if (!GetFinalRedirect(initial_url[i]).Equals(expected_url[i])) {
                            output.WriteLine(initial_url[i] + ", " + GetFinalRedirect(initial_url[i]) + ", " + expected_url[i]);
                        }
                        else {
                            Console.WriteLine("Successful redirect");
                            Console.WriteLine(initial_url[i] + ", " + GetFinalRedirect(initial_url[i]) + ", " + expected_url[i]);
                        }
                    }
                }
                else {
                    throw new Exception("Lists of URLs are different length");
                }
            }
        }

        public string GetFinalRedirect(string url) {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            int maxRedirCount = 8;  // prevent infinite loops
            string newUrl = url;
            do {
                HttpWebRequest req = null;
                HttpWebResponse resp = null;
                try {
                    req = (HttpWebRequest)HttpWebRequest.Create(url);
                    req.Method = "HEAD";
                    req.AllowAutoRedirect = false;
                    resp = (HttpWebResponse)req.GetResponse();
                    switch (resp.StatusCode) {
                        case HttpStatusCode.OK:
                            return newUrl;
                        case HttpStatusCode.Redirect:
                        case HttpStatusCode.MovedPermanently:
                        case HttpStatusCode.RedirectKeepVerb:
                        case HttpStatusCode.RedirectMethod:
                            newUrl = resp.Headers["Location"];
                            if (newUrl == null)
                                return url;

                            if (newUrl.IndexOf("://", System.StringComparison.Ordinal) == -1) {
                                // Doesn't have a URL Schema, meaning it's a relative or absolute URL
                                Uri u = new Uri(new Uri(url), newUrl);
                                newUrl = u.ToString();
                            }
                            break;
                        default:
                            return newUrl;
                    }
                    url = newUrl;
                }
                catch (WebException) {
                    // Return the last known good URL
                    return newUrl;
                }
                catch (Exception ex) {
                    return null;
                }
                finally {
                    if (resp != null)
                        resp.Close();
                }
            } while (maxRedirCount-- > 0);

            return newUrl;
        }
    }
}
