namespace ClaimManagement.Api.Security
{
    public static class Roles
    {
        public const string Customer = "Customer";
        public const string Agent = "Agent";
        public const string Admin = "Admin";          // change to "Administrator" if role_master says so

        public const string All = Customer + "," + Agent + "," + Admin;
    }
}
