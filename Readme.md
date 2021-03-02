# InLoox Invoice Webservice

a webservice that writes data to InLoox.ODataClient nuget package https://www.nuget.org/packages/InLoox.ODataClient/

## Setup

1. Create an InLoox now! account here: https://app.inlooxnow.com/Account/CreateAndLogin
2. Create at least one expense-type budget with a budget position item in it (copy the BudgetPositionId value)
3. Obtain a token
- go to https://app.inlooxnow.com/tests/oauth2
- click the "Authorize" button
- copy the value of the "Access Token" field
4. Run the code in debug mode in a local environment

**Important note:** Sharing an access token is equivalent to sharing username and password for an account.

## Use

To trigger the workflow, POST a request to https://localhost:5001/invoice

Headers:

`Content-Type: application/json`

`access_token: {INSERT_ACCESS_TOKEN_HERE}`

Body:
```yaml
{
  "InvoiceNumber" : "12345/2021",
  "AmountEur" : 17.55,
  "BudgetPositionId" : {INSERT_BUDGET_POSITION_ID_HERE},
  "AttachmentLink" : "https://www.inloox456.com"
}
```

You should see a 200 response.

## Deployment

### Server Deploy

You can either deploy the application internally to an Internet Information Server or to an Azure App Service:
- Locally: https://docs.microsoft.com/de-de/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.1
- Cloud: https://docs.microsoft.com/de-de/aspnet/core/tutorials/publish-to-azure-webapp-using-vs?view=aspnetcore-3.1#deploy-the-app-to-azure

**Important note:** SSL/TLS is highly recommended

## Local Server (InLoox PM Enterprise Server)

To connect to a local InLoox service, update the endpoint to InLoox [endpoint](https://github.com/inloox-dev/inLoox-calendar-webservice/blob/05b33462cd4d77e85d8398dfaebc3bdb2bb77ae5/InLooxCalendarWebservice/Startup.cs#L12) to reflect your InLoox PM Enterprise Server url
