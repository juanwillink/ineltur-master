using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using ArgentinahtlMVC.Models.Services;
using System.Web.Caching;
using System.Collections;

namespace ArgentinahtlMVC.Models
{
    public static class CacheData
    {
        private static Cache GeneralCache
        {
            get { return HttpRuntime.Cache; }
        }

        //public static IEnumerable<TimeZoneModel> TimeZones
        //{
        //    get 
        //    {
        //        var timeZones = GeneralCache["TimeZoneList"] as List<TimeZoneModel>;

        //        if (timeZones == null)
        //        {
        //            GeneralCache["TimeZoneList"] = timeZones = DbAccess.GetTimeZones();
        //        }
        //        return timeZones;
        //    }
        //}

        public static void Clear()
        {
            foreach (DictionaryEntry entry in GeneralCache)
                GeneralCache.Remove(entry.Key as string);
        }
    }
}