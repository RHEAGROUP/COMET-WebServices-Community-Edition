﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiBase.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Cryptography;

    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using CDP4WebServices.API.Services.Operations;
    using CDP4WebServices.API.Services.Supplemental;

    using Helpers;

    using Nancy;
    using Nancy.Responses;

    using NLog;

    using Npgsql;

    using Services;
    using Services.Authorization;
    using Services.Protocol;

    using IServiceProvider = Services.IServiceProvider;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// This is an API abstract base class which holds utility functionalities
    /// </summary>
    public abstract class ApiBase : NancyModule
    {
        /// <summary>
        /// The site directory data.
        /// </summary>
        protected const string SiteDirectoryData = "SiteDirectory";

        /// <summary>
        /// The url segment matcher.
        /// </summary>
        protected readonly string UrlSegmentMatcher = "{uri*}";

        /// <summary>
        /// The API url format template.
        /// </summary>
        protected readonly string ApiFormat = "/{0}/{1}";

        /// <summary>
        /// The mime type JSON.
        /// </summary>
        protected readonly string MimeTypeJson = "application/json";

        /// <summary>
        /// The mime type octet stream.
        /// </summary>
        protected readonly string MimeTypeOctetStream = "application/octet-stream";

        /// <summary>
        /// The content type header.
        /// </summary>
        protected readonly string ContentTypeHeader = "Content-Type";

        /// <summary>
        /// The content disposition header.
        /// </summary>
        private const string ContentDispositionHeader = "Content-Disposition";

        /// <summary>
        /// The content length header.
        /// </summary>
        private const string ContentLengthHeader = "Content-Length";

        /// <summary>
        /// The model reference data library type name.
        /// </summary>
        private const string ModelReferenceDataLibraryType = "ModelReferenceDataLibrary";

        /// <summary>
        /// The site reference data library type name.
        /// </summary>
        private const string SiteReferenceDataLibraryType = "SiteReferenceDataLibrary";

        /// <summary>
        /// The site reference data library type name.
        /// </summary>
        private const string EngineeringModelZipFileName = "filename=AnnexC3ModelExport.zip";

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the header info provider.
        /// </summary>
        public IHeaderInfoProvider HeaderInfoProvider { get; set; }

        /// <summary>
        /// Gets or sets the service registry.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets the permission service.
        /// </summary>
        public IPermissionService PermissionService { get; set; }

        /// <summary>
        /// Gets or sets the Utils instance for this request.
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Gets or sets the operation processor.
        /// </summary>
        public IOperationProcessor OperationProcessor { get; set; }

        /// <summary>
        /// Gets or sets the file binary service.
        /// </summary>
        public IFileBinaryService FileBinaryService { get; set; }

        /// <summary>
        /// Gets or sets the file archive service.
        /// </summary>
        public IFileArchiveService FileArchiveService { get; set; }

        /// <summary>
        /// Gets or sets the engineering model zip export service.
        /// </summary>
        public IEngineeringModelZipExportService EngineeringModelZipExportService { get; set; }

        /// <summary>
        /// Gets or sets the revision service.
        /// </summary>
        public IRevisionService RevisionService { get; set; }

        /// <summary>
        /// Gets or sets the transaction manager for this request.
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICdp4JsonSerializer"/>
        /// </summary>
        public ICdp4JsonSerializer JsonSerializer { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPermissionInstanceFilterService"/>
        /// </summary>
        public IPermissionInstanceFilterService PermissionInstanceFilterService { get; set; }

        /// <summary>
        /// Gets the request path.
        /// </summary>
        protected string RequestPath
        {
            get
            {
                return string.Format("{0}{1}", this.Context.Request.Url.Path, this.Context.Request.Url.Query);
            }
        }

        /// <summary>
        /// Process the get request and return the requested resources.
        /// </summary>
        /// <param name="processor">
        /// The resource accessor
        /// </param>
        /// <param name="topContainer">
        /// The top container.
        /// </param>
        /// <param name="partition">
        /// The data partition.
        /// </param>
        /// <param name="routeSegments">
        /// The route segments.
        /// </param>
        /// <param name="resourcePath">
        /// The resource Path.
        /// </param>
        /// <returns>
        /// The collection of retrieved <see cref="CDP4Common.DTO.Thing"/>.
        /// </returns>
        public IEnumerable<Thing> ProcessRequestPath(
            IProcessor processor,
            string topContainer,
            string partition,
            string[] routeSegments,
            out List<Thing> resourcePath)
        {
            var containmentColl = new List<Thing>();
            var responseColl = new List<Thing>();
            var authorizedContext = new RequestSecurityContext { ContainerReadAllowed = true };
            resourcePath = new List<Thing>();

            // select per route segment tuple (type and optional id)
            // processing containment from top to bottom, we populate first the containmentColl then the responseColl
            for (var i = 0; i < routeSegments.Length; i++)
            {
                if (i % 2 != 0)
                {
                    // iterate per tuple
                    continue;
                }

                // a resource containment path segment consists of a containment property name and identifier segment
                // it is preceded by the url host and is followed by the resource request
                var resourceContainmentSegment = routeSegments.Length > i + 2;

                // a general resource request path ends without any identifier segment or a wildcard character '*'
                var generalResourceRequest = (routeSegments.Length == i + 2 && routeSegments[i + 1] == "*")
                                             || routeSegments.Length == i + 1;

                // a specific resource request path ends with an identifier segment
                var specificResourceRequest = !generalResourceRequest && routeSegments.Length == i + 2;

                var containerProperty = routeSegments[i];
                var serviceType = i == 0
                                      ? topContainer
                                      : processor.GetContainmentType(containmentColl, containerProperty);

                if (serviceType == typeof(Iteration).Name && !generalResourceRequest)
                {
                    // switch to iteration context for further processing,
                    // in case of Iteration generalResource request, this is handled separately below
                    this.TransactionManager.SetIterationContext(processor.Transaction, partition);
                }

                if (resourceContainmentSegment)
                {
                    // this part is always used except for the last tuple
                    var identifier = Utils.ParseIdentifier(routeSegments[i + 1]);

                    processor.ValidateContainment(containmentColl, containerProperty, identifier);

                    var container =
                        processor.GetContainmentResource(serviceType, partition, identifier, authorizedContext);

                    if (serviceType == typeof(Iteration).Name)
                    {
                        // switch the partition (schema) for further processing, allowing retrieval of Iteration contained data
                        partition = partition.Replace("EngineeringModel", "Iteration");
                    }

                    // collect the specified containment resource
                    containmentColl.Add(container);
                    resourcePath.Add(container);

                    // authorized
                    authorizedContext.ContainerReadAllowed = true;
                }
                else if (generalResourceRequest)
                {
                    // get containment info
                    var containmentInfo = processor.GetContainment(containmentColl, containerProperty);

                    if (serviceType == typeof(Iteration).Name && containmentInfo != null)
                    {
                        // support temporal retrieval if iteration general resource is requested
                        // should only contain 1 element
                        foreach (var containedIterationId in containmentInfo)
                        {
                            // switch to iteration context for further processing
                            // use engineering-model id to set the iteration context
                            // partition here should be EngineeringModel_<uuid>
                            this.TransactionManager.SetIterationContext(
                                processor.Transaction,
                                partition,
                                containedIterationId);

                            // collect resources
                            responseColl.AddRange(
                                processor.GetResource(
                                    serviceType,
                                    partition,
                                    new[] { containedIterationId },
                                    authorizedContext));
                        }
                    }
                    else
                    {
                        // collect resources
                        responseColl.AddRange(
                            processor.GetResource(serviceType, partition, containmentInfo, authorizedContext));
                    }
                }
                else if (specificResourceRequest)
                {
                    var identifier = Utils.ParseIdentifier(routeSegments[i + 1]);

                    processor.ValidateContainment(containmentColl, containerProperty, identifier);

                    var resource = processor.GetResource(
                        serviceType,
                        partition,
                        new[] { identifier },
                        authorizedContext).ToList();
                    if (!resource.Any())
                    {
                        continue;
                    }

                    // collect resources
                    responseColl.AddRange(resource);

                    // set specific resource from uri request
                    resourcePath.Add(resource.First());
                }
            }

            if (this.RequestUtils.QueryParameters.IncludeAllContainers)
            {
                responseColl.InsertRange(0, containmentColl);
            }

            return responseColl;
        }

        /// <summary>
        /// Abstract method contract for get response data processing.
        /// </summary>
        /// <param name="routeParams">
        /// The route parameters from the request.
        /// </param>
        /// <returns>
        /// The <see cref="Response"/>.
        /// </returns>
        protected abstract Response GetResponseData(dynamic routeParams);

        /// <summary>
        /// Wrapper function to wire up authorization on the get response.
        /// </summary>
        /// <param name="routeParams">
        /// The route parameters from the request
        /// </param>
        /// <returns>
        /// The <see cref="Response"/>.
        /// </returns>
        protected virtual Response GetResponse(dynamic routeParams)
        {
            // wireup cdp authorization support
            this.CdpAuthorization();
            var response = this.GetResponseData(routeParams);

            // Register the required CDP4 headers to every response send
            this.HeaderInfoProvider.RegisterResponseHeaders(response);
            return response;
        }

        /// <summary>
        /// Abstract method contract for post response data processing.
        /// </summary>
        /// <param name="routeParams">
        /// The route parameters from the request.
        /// </param>
        /// <returns>
        /// The <see cref="Response"/>.
        /// </returns>
        protected abstract Response PostResponseData(dynamic routeParams);

        /// <summary>
        /// Wrapper function to wire up authorization on the post response.
        /// </summary>
        /// <param name="routeParams">
        /// The route parameters from the request
        /// </param>
        /// <returns>
        /// The <see cref="Response"/>.
        /// </returns>
        protected virtual Response PostResponse(dynamic routeParams)
        {
            // wireup cdp authorization support
            this.CdpAuthorization();
            var response = this.PostResponseData(routeParams);

            this.HeaderInfoProvider.RegisterResponseHeaders(response);
            return response;
        }

        /// <summary>
        /// Construct a request log message.
        /// </summary>
        /// <param name="message">
        /// The log message.
        /// </param>
        /// <param name="success">
        /// The success.
        /// </param>
        /// <returns>
        /// A formatted string ready for logging.
        /// </returns>
        protected string ConstructLog(string message = null, bool success = true)
        {
            var credentials = this.RequestUtils.Context.AuthenticatedCredentials;
            var request = this.Context.Request;
            var requestMessage = string.Format(
                "[{0}][{1}]{2}",
                request.Method,
                this.RequestPath,
                !string.IsNullOrWhiteSpace(message) ? string.Format(" : {0}", message) : string.Empty);
            return LoggerUtils.GetLogMessage(credentials.Person, request.UserHostAddress, success, requestMessage);
        }

        /// <summary>
        /// Construct a request log message.
        /// </summary>
        /// <param name="message">
        /// The log message.
        /// </param>
        /// <returns>
        /// A formatted string ready for logging.
        /// </returns>
        protected string ConstructFailureLog(string message = null)
        {
            return this.ConstructLog(message, false);
        }

        /// <summary>
        /// Get a JSON response instance from the passed in resource collection.
        /// </summary>
        /// <param name="resourceResponse">
        /// The resource collection to serialize.
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        /// <param name="statusCode">
        /// The optional HTML status Code.
        /// </param>
        /// <param name="requestToken">
        /// optional request token
        /// </param>
        /// <returns>
        /// The <see cref="Response"/>.
        /// </returns>
        protected Response GetJsonResponse(
            IReadOnlyList<Thing> resourceResponse,
            Version requestDataModelVersion,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            string requestToken = "")
        {
            // create a new response with contents assigned by stream
            return new Response
            {
                Contents = stream => this.CreateFilteredResponseStream(
                    resourceResponse,
                    stream,
                    requestDataModelVersion,
                    requestToken),
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Create a multipart response for the included file revisions.
        /// </summary>
        /// <param name="fileRevisions">
        /// The file revisions.
        /// </param>
        /// <param name="resourceResponse">
        /// The resource response.
        /// </param>
        /// <param name="statusCode">
        /// The optional HTML status Code.
        /// </param>
        /// <returns>
        /// The <see cref="Response"/>.
        /// </returns>
        protected Response GetMultipartResponse(
            List<FileRevision> fileRevisions,
            List<Thing> resourceResponse,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            // create a new multipart response with contents assigned by stream
            return new Response
            {
                Contents = stream => this.PrepareMultiPartResponse(
                    stream,
                    fileRevisions,
                    resourceResponse,
                    this.RequestUtils.GetRequestDataModelVersion),
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Create an archived response for a folder or a file store.
        /// </summary>
        /// <param name="resourceResponse">
        /// The resource response.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="routeSegments">
        /// The route segments of the HTTP request.
        /// </param>
        /// <param name="statusCode">
        /// The optional HTTP status Code.
        /// </param>
        /// <returns>
        /// The <see cref="Response"/>.
        /// </returns>
        protected Response GetArchivedResponse(
            List<Thing> resourceResponse,
            string partition,
            dynamic routeSegments,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            // create a new archived response with contents assigned by stream
            return new Response
            {
                Contents = stream => this.PrepareArchivedResponse(
                    stream,
                    resourceResponse,
                    this.RequestUtils.GetRequestDataModelVersion,
                    partition,
                    routeSegments),
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Read the current state of the top container.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="topContainer">
        /// The Top Container type name.
        /// </param>
        /// <returns>
        /// A top container instance.
        /// </returns>
        protected Thing GetTopContainer(NpgsqlTransaction transaction, string partition, string topContainer)
        {
            return this.ServiceProvider.MapToReadService(topContainer).GetShallow(
                transaction,
                partition,
                null,
                new RequestSecurityContext { ContainerReadAllowed = true }).FirstOrDefault();
        }

        /// <summary>
        /// Collect and return the reference data library chain.
        /// </summary>
        /// <param name="processor">
        /// The processor instance.
        /// </param>
        /// <param name="modelSetup">
        /// The model setup instance for which to retrieve the reference data library chain.
        /// </param>
        /// <returns>
        /// IEnumerable collection of data reference library items.
        /// </returns>
        protected IEnumerable<Thing> CollectReferenceDataLibraryChain(
            IProcessor processor,
            EngineeringModelSetup modelSetup)
        {
            var securityContext = this.SetupSecurityContextForLibraryRead();

            // retrieve the required model reference data library
            var modelReferenceDataLibraryData = processor.GetResource(
                ModelReferenceDataLibraryType,
                SiteDirectoryData,
                modelSetup.RequiredRdl,
                securityContext);

            return this.RetrieveChainedReferenceData(processor, securityContext, modelReferenceDataLibraryData);
        }

        /// <summary>
        /// Collect and return the reference data library chain.
        /// </summary>
        /// <param name="processor">
        /// The processor instance.
        /// </param>
        /// <param name="modelReferenceDataLibrary">
        /// The model Reference Data Library.
        /// </param>
        /// <returns>
        /// IEnumerable collection of data reference library items.
        /// </returns>
        protected IEnumerable<Thing> CollectReferenceDataLibraryChain(
            IProcessor processor,
            ModelReferenceDataLibrary modelReferenceDataLibrary)
        {
            var securityContext = this.SetupSecurityContextForLibraryRead();

            // retrieve the required model reference data library with extent = deep
            var modelReferenceDataLibraryData = processor.GetResource(
                ModelReferenceDataLibraryType,
                SiteDirectoryData,
                new[] { modelReferenceDataLibrary.Iid },
                securityContext);

            return this.RetrieveChainedReferenceData(processor, securityContext, modelReferenceDataLibraryData)
                .Where(x => x.Iid != modelReferenceDataLibrary.Iid);
        }

        /// <summary>
        /// Generates a random string that is used as a token in log statements to match log statements related to the 
        /// processing of one request
        /// </summary>
        /// <returns>
        /// random token</returns>
        protected string GenerateRandomToken()
        {
            using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[4];
                cryptoServiceProvider.GetBytes(data);
                var result = BitConverter.ToString(data).Replace("-", string.Empty);
                return result;
            }
        }

        /// <summary>
        /// Get a zipped models archive response instance from the passed list of <see cref="Guid"/>.
        /// </summary>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        /// <param name="modelSetupGuidList">
        /// The list of EngineeringModelSetup <see cref="Guid"/> to export
        /// </param>
        /// <param name="statusCode">
        /// The optional HTML status Code.
        /// </param>
        /// <param name="requestToken">
        /// optional request token
        /// </param>
        /// <returns>
        /// The <see cref="Response"/>.
        /// </returns>
        protected Response GetZippedModelsResponse(
            Version requestDataModelVersion,
            List<Guid> modelSetupGuidList,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            string requestToken = "")
        {
            var path = this.EngineeringModelZipExportService.CreateZipExportFile(modelSetupGuidList, null);

            try
            {
                if (path == null)
                {
                    throw new Exception("The server was unable to export EngineeringModel. You might not be eligible to export some models, please check your permissions or contact your administrator.");
                }

                // create a new response with contents assigned by stream
                var response = new Response
                {
                    Contents = stream => this.CreateFileResponseStream(stream, path),
                    StatusCode = statusCode
                };

                response.Headers.Add(this.ContentTypeHeader, this.MimeTypeOctetStream);
                response.Headers.Add(ContentDispositionHeader, EngineeringModelZipFileName);

                return response;
            }
            catch (Exception ex)
            {
                // error handling
                var errorResponse = new JsonResponse(
                    string.Format("exception:{0}", ex.Message),
                    new DefaultJsonSerializer());

                if (path != null)
                {
                    System.IO.File.Delete(path);
                }

                return errorResponse.WithStatusCode(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Filters supplied DTO's and creates a JSON response stream based on an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="dtos">
        /// The DTO's that needs to be serialized to a stream
        /// </param>
        /// <param name="stream">
        /// Stream to which to write the serializer output
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        /// <param name="requestToken">
        /// optional request token
        /// </param>
        private void CreateFilteredResponseStream(
            IReadOnlyList<Thing> dtos,
            Stream stream,
            Version requestDataModelVersion,
            string requestToken = "")
        {
            var filteredDtos = this.PermissionInstanceFilterService.FilterOutPermissions(dtos, requestDataModelVersion).ToArray();

            var sw = new Stopwatch();
            sw.Start();
            Logger.Debug("{0} start serialing dtos", requestToken);

            this.JsonSerializer.Initialize(
                this.RequestUtils.MetaInfoProvider,
                this.RequestUtils.GetRequestDataModelVersion);
            this.JsonSerializer.SerializeToStream(filteredDtos, stream);
            sw.Stop();

            Logger.Debug("serialing dtos {0} in {1} [ms]", requestToken, sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Prepare a multi part response based on all included fileRevisions in the response.
        /// </summary>
        /// <param name="targetStream">
        /// The target Stream.
        /// </param>
        /// <param name="fileRevisions">
        /// The file Revisions.
        /// </param>
        /// <param name="resourceResponse">
        /// The resource response.
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The request data model version.
        /// </param>
        private void PrepareMultiPartResponse(
            Stream targetStream,
            List<FileRevision> fileRevisions,
            List<Thing> resourceResponse,
            Version requestDataModelVersion)
        {
            if (!fileRevisions.Any())
            {
                // do nothing if no file revisions are present
                return;
            }

            var content = new MultipartContent("mixed", "----Boundary");

            using (var stream = new MemoryStream())
            {
                this.CreateFilteredResponseStream(resourceResponse, stream, requestDataModelVersion);

                // rewind stream prior to reading
                stream.Position = 0;

                // write out the json content to the first multipart content entry
                var jsonContent = new StringContent(new StreamReader(stream).ReadToEnd());
                jsonContent.Headers.Clear();
                jsonContent.Headers.Add(this.ContentTypeHeader, this.MimeTypeJson);
                content.Add(jsonContent);

                stream.Flush();
            }

            foreach (var hash in fileRevisions.Select(x => x.ContentHash).Distinct())
            {
                byte[] buffer;
                long fileSize;
                using (var fileStream = this.FileBinaryService.RetrieveBinaryData(hash))
                {
                    fileSize = fileStream.Length;
                    buffer = new byte[(int)fileSize];
                    fileStream.Read(buffer, 0, (int)fileSize);
                }

                // write out the binary content to the first multipart content entry
                var binaryContent = new ByteArrayContent(buffer);
                binaryContent.Headers.Add(this.ContentTypeHeader, this.MimeTypeOctetStream);

                // use the file hash value to easily identify the multipart content for each respective filerevision hash entry
                binaryContent.Headers.Add(ContentDispositionHeader, string.Format("attachment; filename={0}", hash));
                binaryContent.Headers.Add(ContentLengthHeader, fileSize.ToString());
                content.Add(binaryContent);
            }

            // stream the multipart content to the request contents target stream
            content.CopyToAsync(targetStream);
        }

        /// <summary>
        /// Prepare an archived response based on a folder or fileStore in the response.
        /// </summary>
        /// <param name="targetStream">
        /// The target Stream.
        /// </param>
        /// <param name="resourceResponse">
        /// The resource response.
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The request data model version.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        ///  <param name="routeSegments">
        /// The route segments.
        /// </param>
        private void PrepareArchivedResponse(
            Stream targetStream,
            List<Thing> resourceResponse,
            Version requestDataModelVersion,
            string partition,
            dynamic routeSegments)
        {
            string folderPath = this.FileArchiveService.CreateFileStructure(resourceResponse, partition, routeSegments);

            try
            {
                var content = new MultipartContent("mixed", "----Boundary");

                using (var stream = new MemoryStream())
                {
                    this.CreateFilteredResponseStream(resourceResponse, stream, requestDataModelVersion);

                    // rewind stream prior to reading
                    stream.Position = 0;

                    // write out the json content to the first multipart content entry
                    var jsonContent = new StringContent(new StreamReader(stream).ReadToEnd());
                    jsonContent.Headers.Clear();
                    jsonContent.Headers.Add(this.ContentTypeHeader, this.MimeTypeJson);
                    content.Add(jsonContent);

                    stream.Flush();
                }

                this.FileArchiveService.CreateZipArchive(folderPath);

                byte[] buffer;
                long fileSize;
                using (var fileStream = new FileStream(folderPath + ".zip", FileMode.Open))
                {
                    fileSize = fileStream.Length;
                    buffer = new byte[(int)fileSize];
                    fileStream.Read(buffer, 0, (int)fileSize);
                }

                var binaryContent = new ByteArrayContent(buffer);
                binaryContent.Headers.Add(this.ContentTypeHeader, this.MimeTypeOctetStream);

                // use the file hash value to easily identify the multipart content for each respective filerevision hash entry
                binaryContent.Headers.Add(
                    ContentDispositionHeader,
                    string.Format("attachment; filename={0}", folderPath + ".zip"));
                binaryContent.Headers.Add(ContentLengthHeader, fileSize.ToString());
                content.Add(binaryContent);

                // stream the multipart content to the request contents target stream
                content.CopyToAsync(targetStream);
            }
            finally
            {
                this.FileArchiveService.DeleteFileStructureWithArchive(folderPath);
            }
        }

        /// <summary>
        /// Creates file response stream.
        /// </summary>
        /// <param name="targetStream">
        /// The target stream where the file stream is copied.
        /// </param>
        /// <param name="path">
        /// The path of the file to put in a stream.
        /// </param>
        private void CreateFileResponseStream(Stream targetStream, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                var fileSize = fileStream.Length;
                var buffer = new byte[(int)fileSize];
                fileStream.Read(buffer, 0, (int)fileSize);

                var content = new ByteArrayContent(buffer);

                // stream the multipart content to the request contents target stream
                content.CopyToAsync(targetStream);
            }

            System.IO.File.Delete(path);
        }

        /// <summary>
        /// Setup the security context for the library data retrieval.
        /// </summary>
        /// <returns>
        /// The <see cref="RequestSecurityContext"/>.
        /// </returns>
        private RequestSecurityContext SetupSecurityContextForLibraryRead()
        {
            // override query parameters to return full set
            this.RequestUtils.OverrideQueryParameters =
                new QueryParameters { ExtentDeep = true, IncludeReferenceData = true };

            return new RequestSecurityContext { ContainerReadAllowed = true };
        }

        /// <summary>
        /// The retrieve chained reference data.
        /// </summary>
        /// <param name="processor">
        /// The processor.
        /// </param>
        /// <param name="securityContext">
        /// The security context.
        /// </param>
        /// <param name="modelReferenceDataLibraryData">
        /// The model reference data library data.
        /// </param>
        /// <returns>
        /// A collection of retrieved reference data.
        /// </returns>
        private IEnumerable<Thing> RetrieveChainedReferenceData(
            IProcessor processor,
            ISecurityContext securityContext,
            IEnumerable<Thing> modelReferenceDataLibraryData)
        {
            var chainedReferenceDataColl = new List<Thing>();

            // do not collect the model reference data library again, as it is already present
            chainedReferenceDataColl.AddRange(modelReferenceDataLibraryData);
            var referenceDataLibrary = (ReferenceDataLibrary)chainedReferenceDataColl.First();

            // while the requiredRdl is set retrieve the reference data library chain bottom up
            while (referenceDataLibrary.RequiredRdl != null)
            {
                var siteReferenceDataLibraryData = processor.GetResource(
                    SiteReferenceDataLibraryType,
                    SiteDirectoryData,
                    new List<Guid> { (Guid)referenceDataLibrary.RequiredRdl },
                    securityContext).ToList();
                chainedReferenceDataColl.AddRange(siteReferenceDataLibraryData);
                referenceDataLibrary = (ReferenceDataLibrary)siteReferenceDataLibraryData.First();
            }

            // reset query parameters
            this.RequestUtils.OverrideQueryParameters = null;
            return chainedReferenceDataColl;
        }
    }
}