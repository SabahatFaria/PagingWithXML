using Microsoft.Azure.WebJobs.Host;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MOCD
{
    public class CRMDataManager
    {
        private static TraceWriter log = null;
        private static IOrganizationService _orgService;
        //private CRMResponse response = null;
        //public string connectionString { get; set; }

        public static IOrganizationService orgService
        {
            get
            {
                if (_orgService != null)
                {
                    return _orgService;
                }
                else
                {
                    return _orgService = GetCRMConnection();
                }
            }
        }

        public CRMDataManager(TraceWriter _log)
        {
            log = _log;
            GetCRMConnection();
        }

        public CRMDataManager(string connection)
        {
            
        }

        public static IOrganizationService GetCRMConnection()
        {
            string CrmConfiguration = string.Empty;
            try
            {
                if (_orgService != null)
                {
                    return _orgService;
                }
                else
                {
                    log.Info("Creating connection:..");
                    string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
                    //string connectionString = ConfigurationManager.AppSettings[ConnectionString];
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    CrmServiceClient conn = new CrmServiceClient(connectionString);
                    CrmServiceClient.MaxConnectionTimeout = new TimeSpan(0, 10, 0);
                    _orgService = conn.OrganizationWebProxyClient != null ? conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;


                    if (conn.OrganizationServiceProxy != null)
                        conn.OrganizationServiceProxy.Timeout = new TimeSpan(1, 0, 0, 0);
                }

            }
            catch (Exception ex)

            {
                log.Info("GetCRMConnection:........." + ex.Message);
                log.Info(ex.StackTrace);
            }

            return _orgService;
        }
        //public CRMResponse CrmGet(QueryExpression query, int? topN = null, PagingInfo pagingInfo = null)
        //{
        //    response = new CRMResponse();
        //    try
        //    {
        //        query.TopCount = topN;

        //        if (pagingInfo != null)
        //            query.PageInfo = pagingInfo;

        //        response.Collection = _orgService.RetrieveMultiple(query);
        //        response.Exception = null;
        //        response.Success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        SaveException(ex, "CrmGet");
        //        response.ErrorMessage = "Error.Error-Fetching";
        //        response.Exception = ex;
        //        response.Success = false;
        //    }

        //    return response;
        //}

        //public CRMResponse CrmUpdateEntity(Entity entity)
        //{
        //    response = new CRMResponse();
        //    try
        //    {
        //        if (entity != null)
        //        {
        //            _orgService.Update(entity);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SaveException(ex, "CrmGet");
        //        response.ErrorMessage = "CrmGet Error.Error-Fetching" + ex.Message;
        //        response.Exception = ex;
        //        response.Success = false;
        //    }
        //    return response;
        //}
        //public CRMResponse CrmGet(QueryExpression query, PagingInfo pagingInfo = null)
        //{
        //    {
        //        response = new CRMResponse();
        //        try
        //        {
        //            if (pagingInfo != null)
        //                query.PageInfo = pagingInfo;

        //            response.Collection = _orgService.RetrieveMultiple(query);
        //            response.Exception = null;
        //            response.Success = true;
        //            log.Info("CrmGet function finished............. ");
        //        }
        //        catch (Exception ex)
        //        {
        //            SaveException(ex, "CrmGet");
        //            response.ErrorMessage = "CrmGet Error.Error-Fetching" + ex.Message;
        //            response.Exception = ex;
        //            response.Success = false;
        //        }

        //        return response;
        //    }
        //}

        //public CRMResponse CrmGetSingle(string entityName, Guid companyID, ColumnSet columns, PagingInfo pagingInfo = null)
        //{
        //    {
        //        response = new CRMResponse();
        //        try
        //        {
        //            /*if (pagingInfo != null)
        //                query.PageInfo = pagingInfo;*/

        //            response.Entity = _orgService.Retrieve(entityName, companyID, columns);
        //            response.Exception = null;
        //            response.Success = true;
        //            log.Info("CrmGetSingle function finished............. ");
        //        }
        //        catch (Exception ex)
        //        {
        //            SaveException(ex, "CrmGetSingle");
        //            response.ErrorMessage = "CrmGetSingle Error.Error-Fetching" + ex.Message;
        //            response.Exception = ex;
        //            response.Success = false;
        //        }

        //        return response;
        //    }
        //}


        //public CRMResponse BulkUpdate(List<Entity> entities)
        //{
        //    response = new CRMResponse();

        //    try
        //    {
        //        var multipleRequest = new ExecuteMultipleRequest()
        //        {
        //            Settings = new ExecuteMultipleSettings()
        //            {
        //                ContinueOnError = false,
        //                ReturnResponses = true
        //            },
        //            Requests = new OrganizationRequestCollection()
        //        };

        //        foreach (Entity entity in entities)
        //        {
        //            UpdateRequest updateRequest = new UpdateRequest { Target = entity };
        //            multipleRequest.Requests.Add(updateRequest);
        //        }

        //        ExecuteMultipleResponse multipleResponse = (ExecuteMultipleResponse)_orgService.Execute(multipleRequest);
        //        response.Success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        SaveException(ex, "BulkCrmUpdate");
        //        response.Success = false;
        //        response.ErrorMessage = ex.Message;
        //        response.Exception = ex;
        //    }

        //    return response;
        //}
        //public CRMResponse BulkUpdate(IOrganizationService service, List<Entity> entities)
        //{
        //    response = new CRMResponse();

        //    try
        //    {
        //        // Create an ExecuteMultipleRequest object.
        //        var multipleRequest = new ExecuteMultipleRequest()
        //        {
        //            // Assign settings that define execution behavior: continue on error, return responses. 
        //            Settings = new ExecuteMultipleSettings()
        //            {
        //                ContinueOnError = false,
        //                ReturnResponses = true
        //            },
        //            // Create an empty organization request collection.
        //            Requests = new OrganizationRequestCollection()
        //        };

        //        // Add a UpdateRequest for each entity to the request collection.
        //        foreach (var entity in entities)
        //        {
        //            UpdateRequest updateRequest = new UpdateRequest { Target = entity };
        //            multipleRequest.Requests.Add(updateRequest);
        //        }

        //        // Execute all the requests in the request collection using a single web method call.
        //        ExecuteMultipleResponse multipleResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);
        //        foreach (var responseItem in multipleResponse.Responses)
        //        {
        //            // A valid response.
        //            if (responseItem.Response != null)
        //            {
        //                response.Success = true;

        //            }


        //            // An error has occurred.
        //            else if (responseItem.Fault != null)
        //            {
        //                response.Success = false;
        //                response.ErrorMessage = responseItem.Fault.ToString();
        //                log.Error("Company could not updated");
        //                log.Info("");
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        SaveException(ex, "BulkCrmUpdate");
        //        response.Success = false;
        //        response.ErrorMessage = ex.Message;
        //        response.Exception = ex;
        //    }

        //    return response;
        //}

        //private void SaveException(Exception ex, string v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
