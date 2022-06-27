using System;

namespace RedirectChecker
{
    public class Driver
    {
        public Driver()
        {
            ;
        }

        static void Main(string[] args)
        {
            SegmentChecker sc = new SegmentChecker();
            RedirectChecker rc = new RedirectChecker();
            DuplicateListings dl = new DuplicateListings();
            //rc.Test();
            //rc.FindAmountOfRedirect();
            //rc.AssertNo404();
            //sc.AssertSegments();
            //dl.FindDuplicates();
            //Console.WriteLine(rc.GetFinalRedirect("https://staging.withdrawal.net/alcohol/outpatient-drug-and-detox-centers/"));
            rc.AssertRedirects();
      
        }
    }
}
