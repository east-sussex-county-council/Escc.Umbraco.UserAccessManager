using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;

namespace ESCC.Umbraco.UserAccessManager.Services
{
    public class RedirectsService : IRedirectsService
    {
        private readonly string _dbConnString;

        public RedirectsService()
        {
            _dbConnString = ConfigurationManager.ConnectionStrings["redirectsDbDSN"].ConnectionString;
        }

        public IList<RedirectModel> GetRedirectsByDestination(string destinationUrl)
        {
            IList<RedirectModel> redirectsList = new List<RedirectModel>();

            // define connection and command, in using blocks to ensure disposal
            using (var conn = new SqlConnection(_dbConnString))
            using (var cmd = new SqlCommand("[dbo].[usp_Redirect_SelectByDestination]", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramUrl = new SqlParameter
                {
                    ParameterName = "@destination",
                    SqlDbType = SqlDbType.VarChar,
                    Value = destinationUrl
                };

                cmd.Parameters.Add(paramUrl);
                conn.Open();

                using (var rtn = cmd.ExecuteReader())
                {
                    while (rtn.Read())
                    {
                        var redirectItem = new RedirectModel
                        {
                            RedirectId = rtn.GetFieldValue<int>(0),
                            Pattern = rtn.GetFieldValue<string>(1),
                            Destination = rtn.GetFieldValue<string>(2),
                            Type = rtn.GetFieldValue<int>(3),
                            Comment = rtn.GetFieldValue<string>(4),
                            DateCreated = rtn.GetFieldValue<DateTime>(5)
                        };

                        redirectsList.Add(redirectItem);
                    }
                }

                return redirectsList;
            }
        }
    }
}