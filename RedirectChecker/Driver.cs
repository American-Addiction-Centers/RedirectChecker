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
            //rc.FindAmountOfRedirect();
            rc.AssertRedirects();
            //rc.AssertNo404();
            //sc.AssertSegments();
            //dl.FindDuplicates();
        }
    }
}
