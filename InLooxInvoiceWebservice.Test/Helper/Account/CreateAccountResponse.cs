namespace InLooxInvoiceWebservice.Test.Helper.Account
{
    public class CreateAccountResponse
    {
        public class ReturnResponse
#pragma warning disable IDE1006 // naming convention
        {
            public string url { get; set; }
        }
        public string err { get; set; }
        public ReturnResponse ret { get; set; }
#pragma warning restore IDE1006 // naming convention
    }
}
