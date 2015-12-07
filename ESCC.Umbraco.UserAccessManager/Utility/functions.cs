using System;

namespace ESCC.Umbraco.UserAccessManager.Utility
{
    public static class Functions
    {
        //public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        //{
        //    foreach (var cur in enumerable)
        //    {
        //        collection.Add(cur);
        //    }
        //}

        public static string GetAppPath(string appPath)
        {
            if (String.IsNullOrEmpty(appPath))
            {
                appPath = "/.";
            }
            else if (appPath.Length == 1)
            {
                appPath += ".";
            }
            else if (!appPath.EndsWith("/"))
            {
                appPath += "/";
            }

            return appPath;
        }
    }
}