using Default;
using System;
using System.Linq;
using System.Threading.Tasks;
using InLoox.ODataClient;
using InLoox.ODataClient.Data.BusinessObjects;
using IQmedialab.InLoox.Data.Api.Model.OData;

namespace InLooxInvoiceWebservice.Services
{
    public class Service : IService
    {
        private const string SERVICE_URL = "https://app.inlooxnow.com/"; // <- insert your IWA service url here in case you run InLoox on premise
        private const string ODATA_ROUTE = "odata/";
        protected internal Container _context;

        public bool IsConnected { get; private set; }

        public Contact CurrentUser { get; private set; }

        public Service()
        {
            IsConnected = false;
            CurrentUser = null;
        }

        #region Connection
        public async Task<bool> Connect(string username, string password)
        {
            var tokenResponse = ODataBasics.GetToken(new Uri(SERVICE_URL), username, password)
                .Result;

            var token = tokenResponse?.AccessToken;

            if (token == null)
            {
                // credentials invalid
                IsConnected = false;
                return IsConnected;
            }

            return await Connect(token);
        }

        public async Task<bool> Connect(string token)
        {
            var endPointOdata = new Uri(new Uri(SERVICE_URL), ODATA_ROUTE);

            try
            {
                // set context
                _context = ODataBasics.GetInLooxContext(endPointOdata, token);

                // get current user, also test for connection
                CurrentUser = await GetCurrentUser();
            }
            catch
            {
                // token invalid
                IsConnected = false;
                return IsConnected;
            }

            IsConnected = true;
            return IsConnected;
        }
        #endregion

        #region Common
        private async Task<Contact> GetCurrentUser()
        {
            var userRequest = _context.contact.getauthenticated();
            var users = await userRequest.ExecuteAsync();
            return users.First();
        }

        public async Task<CustomExpandDefaultExtend> GetCustomField(string entity, int type, string name)
        {
            var ceDefaults = (await _context.customexpanddefaultextend.ExecuteAsync()).ToList();
            var ceField = ceDefaults.FirstOrDefault(k => k.DisplayName == name && k.TableName == entity && k.ColumnType == type);
            return ceField;
        }
        #endregion

        }
    }
