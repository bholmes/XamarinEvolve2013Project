using System;
using System.Collections.Generic;
using ServiceStack.ServiceInterface;
using XamarinEvolveSSLibrary;

namespace XamarinEvolveSS
{
    public class PlacesService : RestServiceBase<PlacesRequest>
    {
        public override object OnGet(PlacesRequest request)
        {
            try
            {
                var limit = request.Limit > 0 ? request.Limit : SystemConstants.MaxPlacesPerRequest;

                if (string.IsNullOrEmpty(request.Method))
                    throw new ArgumentException("Method not specified");
                MySqlCheckInAccess sql = new MySqlCheckInAccess();
                List<Place> placeList;

                switch (request.Method)
                {
                    case "Recent" :
                        placeList = sql.GetRecentPlaceList(limit);
                        break;
                    case "Popular":
                        placeList = sql.GetPopularPlaceList(limit);
                        break;
                    case "DistanceFrom":
                        placeList = sql.GetPlaceListNearLocation(request.Latitude, request.Longitude, limit);
                        break;

                    default:
                        throw new ArgumentException(string.Format("Method '{0}' not recognized", request.Method));
                }

                return new PlacesRequestResponse { Places = placeList };
            }
            catch (Exception ex)
            {
                return new PlacesRequestResponse() { Exception = ex };
            }

            
        }
    }
}