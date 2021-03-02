namespace InLooxInvoiceWebservice.Helpers
{
    public static class ConstantsHelper
    {
        // Security
        public const string ACCESS_TOKEN_PARAMETER_NAME = "access_token";

        // Budgets
        public const int BUDGET_TYPE_PLANNED_EXPENSE = 5;
        public const int BUDGET_TYPE_ACTUAL_EXPENSE = 2;

        // Custom Fields
        public const int CUSTOM_FIELD_TYPE_TEXT = 0;
        public const int CUSTOM_FIELD_TYPE_DATETIME = 1;
        public const int CUSTOM_FIELD_TYPE_INTEGER = 2;
        public const int CUSTOM_FIELD_TYPE_DECIMAL = 3;
        public const int CUSTOM_FIELD_TYPE_BOOLEAN = 4;
        public const int CUSTOM_FIELD_TYPE_LIST = 5;
        public const int CUSTOM_FIELD_TYPE_CURRENCY = 6;

        public const string CUSTOM_FIELD_TYPE_STRING_VALUE = "StringValue";
    }
}
