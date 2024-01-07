using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Microsoft.ServiceBus.Messaging;
using System.Configuration;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Library.Values;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using MOCD;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Text;
using System.Xml;

namespace FetchData
{
    public static class MainFunc //removed 'static'
    {
        static TraceWriter log = null;
        static CRMDataManager CRMData = null;
        static string ConnectionString = ConfigurationManager.AppSettings["ConnectionStringProd"];
        static CrmServiceClient service = new CrmServiceClient(ConnectionString);
        static List<Response> responses = new List<Response>();
        static Dictionary<string, string> nationalityLookup = new Dictionary<string, string>
        {
            {"<<GUID>>","<<Name_Of_Country>"}   //lookups from dataverse for countries
        };
        static string monthBatch;

        [FunctionName("CDARFA")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "api/MainFunc")] HttpRequestMessage req, TraceWriter _log)
        {
            log = _log;
            log.Info("C# HTTP trigger function processed a request.");
            try
            {
                CRMData = new CRMDataManager(log);
                //Response response;
                var requestBody = await req.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<RequestPayload>(requestBody);
                List<Response> responses = new List<Response>();
                responses = Queries(payload);
                var responseList = responses.ToList();
                responses.Clear();
                log.Info("End of Function");

                return new OkObjectResult(responseList);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);

            }
        }
        private static List<Response> Queries(RequestPayload payload)
        {
            if (service.IsReady)
            {
                Response response = new Response();
                if (!string.IsNullOrEmpty(payload.))
                {
                    log.Info("Query received");

                    string query = @"put your fetch XML here";
                    //log.Info("Query completed");
                    //response = FetchRecords(query);
                    response = FetchRecords(query);
                    if (response != null)
                        responses.Add(response);
                    return responses;

                }
                else
                    return null;
            }
            else
            {
                log.Error("CRM service connection failed.");
                return null;
            }
        }
        
        private static Response FetchRecords(string query)
        {
            //List<Response> responses = new List<Response>();
            //int fetchCount = 5000;
            //// Initialize the page number.
            //int pageNumber = 1;
            //// Initialize the number of records.
            //int recordCount = 0;
            //// Specify the current paging cookie. For retrieving the first page, 
            //// pagingCookie should be null.
            //string pagingCookie = null;
            //string xml = CreateXml(query, pagingCookie, pageNumber, fetchCount);
            //// EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(query));
            //RetrieveMultipleRequest fetchRequest1 = new RetrieveMultipleRequest
            //{
            //    Query = new FetchExpression(xml)
            //};
            //EntityCollection entityCollection = ((RetrieveMultipleResponse)service.Execute(fetchRequest1)).EntityCollection;

            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(query));
            //log.Info("Fetching records");
            Response response = new Response();
            
            //while (true)
            //{
                if (entityCollection != null)
                {
                    if (entityCollection.Entities != null)
                    {
                        if (entityCollection.Entities.Count > 0)
                        {
                            foreach (var entity in entityCollection.Entities)
                            {
                                HouseHoldMember houseHold = new HouseHoldMember();
                                //Assigning values to Alias variables
                                if (entity.Attributes.Contains("Beneficiary.tvs_fullnamearabic"))
                                {
                                    AliasedValue fullNameArAlias = (AliasedValue)entity["Beneficiary.tvs_fullnamearabic"];
                                    response.FullNameAr = fullNameArAlias.Value.ToString();
                                }
                                if (entity.Attributes.Contains("Beneficiary.tvs_fullname"))
                                {
                                    AliasedValue FullNameEnAlias = (AliasedValue)entity["Beneficiary.tvs_fullname"];
                                    response.FullNameEn = FullNameEnAlias.Value.ToString();
                                }
                                if (entity.Attributes.Contains("Beneficiary.tvs_emiratesid"))
                                {
                                    AliasedValue emiratesIdAlias = (AliasedValue)entity["Beneficiary.tvs_emiratesid"];
                                    response.EID = emiratesIdAlias.Value.ToString();
                                }
                                if (entity.Attributes.Contains("Beneficiary.tvs_lkpnationality"))
                                {
                                    var nationalityAlias = entity.GetAttributeValue<AliasedValue>("Beneficiary.tvs_lkpnationality").Value;
                                    //nationalityAlias.ToString();
                                    EntityReference _natRef = null;
                                    if (nationalityAlias != null)
                                    {
                                        _natRef = ((EntityReference)entity.GetAttributeValue<AliasedValue>("Beneficiary.tvs_lkpnationality").Value);
                                    }
                                    var nationalityGuid = _natRef.Id.ToString();
                                    //string nationality = string.Empty;
                                    string nationality = nationalityLookup[nationalityGuid];

                                    response.Nationality = nationality;
                                }
                                if (entity.Attributes.Contains("Beneficiary.tvs_dewaaccountnumber"))
                                {
                                    AliasedValue dewaAlias = (AliasedValue)entity["Beneficiary.tvs_dewaaccountnumber"];
                                    response.DewaAcc = dewaAlias.Value.ToString();
                                }
                                if (entity.Attributes.Contains("Beneficiary.gendercode"))
                                {
                                    QueryExpression genderExp = new QueryExpression();
                                    Int32 genderValue;
                                    AliasedValue genderAlias = (AliasedValue)entity["Beneficiary.gendercode"];
                                    genderValue = ((genderAlias.Value) as OptionSetValue).Value;
                                    string gender = entity.FormattedValues["Beneficiary.gendercode"];
                                    response.Gender = gender;
                                }
                                if (entity.Attributes.Contains("Beneficiary.tvs_dateofbirth"))
                                {
                                    AliasedValue dobAlias = (AliasedValue)entity["Beneficiary.tvs_dateofbirth"];
                                    response.DateOfBirth = dobAlias.Value.ToString();
                                }

                                //assigning household members
                                if (entity.Attributes.Contains("FamilyBook.tvs_gender"))
                                {
                                    QueryExpression genderExp = new QueryExpression();
                                    Int32 genderValue;
                                    AliasedValue genderAlias = (AliasedValue)entity["FamilyBook.tvs_gender"];
                                    genderValue = ((genderAlias.Value) as OptionSetValue).Value;
                                    string gender = entity.FormattedValues["FamilyBook.tvs_gender"];
                                    houseHold.Gender = gender;
                                }
                                if (entity.Attributes.Contains("FamilyBook.tvs_idncase"))
                                {
                                    AliasedValue idnAlias = (AliasedValue)entity["FamilyBook.tvs_idncase"];
                                    houseHold.IDNcase = idnAlias.Value.ToString();
                                }
                                if (entity.Attributes.Contains("FamilyBook.tvs_name"))
                                {
                                    AliasedValue fullNameAlias = (AliasedValue)entity["FamilyBook.tvs_name"];
                                    houseHold.FullName = fullNameAlias.Value.ToString();
                                }
                                if (entity.Attributes.Contains("FamilyBook.tvs_nationality"))
                                {
                                    var nationalityAlias = entity.GetAttributeValue<AliasedValue>("FamilyBook.tvs_nationality").Value;
                                    EntityReference _natRef = null;
                                    if (nationalityAlias != null)
                                    {
                                        _natRef = ((EntityReference)entity.GetAttributeValue<AliasedValue>("FamilyBook.tvs_nationality").Value);
                                    }

                                    var nationalityGuid = _natRef.Id.ToString();
                                    string nationality = nationalityLookup[nationalityGuid];
                                    houseHold.Nationality = nationality;
                                }
                                if (entity.Attributes.Contains("FamilyBook.tvs_dob_case"))
                                {
                                    AliasedValue dob_Alias = (AliasedValue)entity["FamilyBook.tvs_dob_case"];
                                    houseHold.DOBcase = dob_Alias.Value.ToString();
                                }

                                response.houseHoldMembers.Add(houseHold);
                            
                            }
                            return response;
                            //responses.Add(response);
                        }
                        else
                            response = null;
                    }
                    else
                        response = null;
                }
                //if (entityCollection.MoreRecords)
                //{
                //    // Increment the page number to retrieve the next page.
                //    pageNumber++;
                //    xml = CreateXml(query, pagingCookie, pageNumber, fetchCount);

                //    // Set the paging cookie to the paging cookie returned from current results.                            
                //    pagingCookie = entityCollection.PagingCookie;
                //}
                //else
                //{
                //    // If no more records in the result nodes, exit the loop.
                //    break;
                //}
                ////return response;
            //}
            return response;

        }
        private static string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            var reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }

    }
}
