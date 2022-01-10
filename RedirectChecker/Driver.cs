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
            RedirectChecker rc = new RedirectChecker();
            //rc.FindAmountOfRedirect();
            //rc.AssertRedirects();
            rc.AssertNo404();
        }
    }
}
