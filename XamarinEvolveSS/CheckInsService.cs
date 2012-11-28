using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using XamarinEvolveSSLibrary;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.Auth;

namespace XamarinEvolveSS
{
    public class CheckInsService : RestServiceBase<CheckInRequest>
    {
        public override object OnGet(CheckInRequest request)
        {
            try
            {
                var limit = request.RecentLimit > 0 ? request.RecentLimit : SystemConstants.MaxPlacesPerRequest;
                
                MySqlCheckInAccess sql = new MySqlCheckInAccess();
                List<CheckInUserPair> activePairList;
                List<CheckInUserPair> recentPairList;

                sql.GetCheckinInfo(request.PlaceId, out activePairList, out recentPairList, limit);

                return new CheckInRequestResponse { ActivePairList = activePairList,RecentPairList = recentPairList };
            }
            catch (Exception ex)
            {
                return new CheckInRequestResponse() { Exception = ex };
            }
        }

        public override object OnPut(CheckInRequest request)
        {
            try
            {
                string userName = CheckIsAuthorizedAndGetName();
                if (string.IsNullOrEmpty(userName))
                {
                    return null;
                }

                MySqlCheckInAccess sql = new MySqlCheckInAccess();
                sql.CheckInUserAtPlace(userName, request.Place);
            }
            catch (Exception ex)
            {
                return new CheckInRequestResponse() { Exception = ex };
            }

            return new CheckInRequestResponse();
        }

        private string CheckIsAuthorizedAndGetName()
        {
            if (SystemConstants.UseAuthentication)
            {
                IAuthSession auth = this.GetSession();
                if (auth == null || string.IsNullOrWhiteSpace (auth.UserName))
                {
                    this.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    return string.Empty;
                }

                return auth.UserName;
            }

            throw new NotImplementedException("CheckInsService can not run without authentication");
        }
    }
}