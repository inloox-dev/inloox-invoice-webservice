namespace InLooxInvoiceWebservice.Test.Helper.General
{
    public static class ParameterHelper
    {
        public static string GetTokenParameter(string token) {
            return "access_token=" + token;
        }
    }
}
