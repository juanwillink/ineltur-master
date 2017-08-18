using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Collections;
using NemoTypes;

namespace CheckArgentina.Models.Service
{
    public static class CacheData
    {
        private static Cache GeneralCache
        {
            get { return HttpRuntime.Cache; }
        }

        public static IEnumerable<DestinationModel> Countries
        {
            get
            {
                var countries = GeneralCache["CountryList"] as List<DestinationModel>;

                return countries;
            }

            set
            {
                GeneralCache["CountryList"] = value;
            }
        }

        public static DateTime CountryListLastModification { get; set; }

        public static IEnumerable<NemoTypes.RoomType> NemoRoomTypes
        {
            get 
            {
                var roomTypes = GeneralCache["RoomTypeList"] as List<NemoTypes.RoomType>;

                if (roomTypes == null)
                {
                    roomTypes = RoomTypes.GetRoomTypes();
                    GeneralCache["RoomTypeList"] = roomTypes;
                }

                return roomTypes;
            }
        }

        public static void Clear()
        {
            foreach (DictionaryEntry entry in GeneralCache)
                GeneralCache.Remove(entry.Key as string);
        }
    }
}