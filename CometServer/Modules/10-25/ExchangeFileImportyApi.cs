﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileImportyApi.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Helpers;

    using CDP4Orm.Dao;
    using CDP4Orm.MigrationEngine;

    using CometServer.Configuration;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.DataStore;
    using CometServer.Services.Operations.SideEffects;
    using CometServer.Services.Protocol;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using NLog;
    using Npgsql;

    using IServiceProvider = CometServer.Services.IServiceProvider;
    using Thing = CDP4Common.DTO.Thing;
    using TopContainer = CDP4Common.DTO.TopContainer;

    /// <summary>
    /// This is an API endpoint class to support the ECSS-E-TM-10-25-AnnexC exchange file format import
    /// </summary>
    public class ExchangeFileImportyApi : CarterModule
    {
        /// <summary>
        /// The top container.
        /// </summary>
        private const string TopContainer = "SiteDirectory";

        /// <summary>
        /// Due to the behaviour of a sql sequence the first time the sequence nextval us queried the seemignly currentval
        /// is returned.
        /// </summary>
        private const int IterationNumberSequenceInitialization = 1;

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeFileImportyApi"/>
        /// </summary>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService"/>
        /// </param>
        public ExchangeFileImportyApi(IAppConfigService appConfigService)
        {
            this.AppConfigService = appConfigService;
        }

        /// <summary>
        /// Gets or sets the <see cref="IAppConfigService"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Add the routes to the <see cref="IEndpointRouteBuilder"/>
        /// </summary>
        /// <param name="app">
        /// The <see cref="IEndpointRouteBuilder"/> to which the routes are added
        /// </param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/Data/Exchange", async (HttpRequest req, HttpResponse res,
                IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IJsonExchangeFileReader jsonExchangeFileReader, IMigrationService migrationService, IRevisionService revisionService, IEngineeringModelDao engineeringModelDao, IServiceProvider serviceProvider, IPersonService personService, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, IDataStoreController dataStoreController, ISiteDirectoryService siteDirectoryService, IEngineeringModelSetupService engineeringModelSetupService) => {
                await this.SeedDataStore(req, res, requestUtils, transactionManager, jsonExchangeFileReader, migrationService, revisionService, engineeringModelDao, serviceProvider, personService, personRoleService, personPermissionService, defaultPermissionProvider, participantRoleService, participantPermissionService, dataStoreController, siteDirectoryService, engineeringModelSetupService);
            });

            app.MapPost("/Data/Import", async (HttpRequest req, HttpResponse res,
                IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IJsonExchangeFileReader jsonExchangeFileReader, IMigrationService migrationService, IRevisionService revisionService, IEngineeringModelDao engineeringModelDao, IServiceProvider serviceProvider, IPersonService personService, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, IDataStoreController dataStoreController) => {
                await this.ImportDataStore(req, res, requestUtils, transactionManager, jsonExchangeFileReader, migrationService, revisionService, engineeringModelDao, serviceProvider, personService, personRoleService, personPermissionService, defaultPermissionProvider, participantRoleService, participantPermissionService, dataStoreController);
            });

            app.MapPost("/Data/Restore", async (HttpResponse res, IDataStoreController dataStoreController) => {
                await this.RestoreDatastore(res, dataStoreController);
            });
        }

        /// <summary>
        /// Restore the data store.
        /// </summary>
        /// <param name="request">
        /// The <see cref="HttpRequest"/> that is being handled
        /// </param>
        /// <param name="response">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <param name="dataStoreController">
        /// The (injected) <see cref="IDataStoreController"/>
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        internal async Task RestoreDatastore(HttpResponse response, IDataStoreController dataStoreController)
        {
            if (!this.AppConfigService.AppConfig.Backtier.IsDbRestoreEnabled)
            {
                Logger.Info("Data restore API invoked but it was disabled from configuration, cancel further processing...");

                response.StatusCode = (int)HttpStatusCode.Forbidden;
                await response.AsJson("restore is not allowed");
            }

            try
            {
                Logger.Info("Starting data store rollback");
                dataStoreController.RestoreDataStore();

                // TODO: check what to do with this in origianl CDP4 code
                // reset the credential cache as the underlying datastore was reset
                //this.WebServiceAuthentication.ResetCredentialCache();

                Logger.Info("Finished data store rollback");

                response.StatusCode = (int)HttpStatusCode.OK;
                await response.AsJson("DataStore restored");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occured during data store rollback");

                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.AsJson("DataStore restored failed");
            }
        }

        /// <summary>
        /// Saves the seed file to disk and returns the file path where it has been saved
        /// </summary>
        /// <param name="request">
        /// The <see cref="HttpRequest"/> that contains fhe file
        /// </param>
        /// <returns>
        /// the file path where the seed file has been stored
        /// </returns>
        private async Task<string> SaveTemporarySeedFile(HttpRequest request)
        {
            var uploadDirectory = this.AppConfigService.AppConfig.Midtier.UploadDirectory;

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var tempFileName = Guid.NewGuid().ToString();

            var templFilePath = Path.Combine(uploadDirectory, tempFileName);

            await request.BindAndSaveFile(uploadDirectory, tempFileName);

            return templFilePath;
        }

        /// <summary>
        /// Asynchronously import the data store.
        /// </summary>
        /// <returns>
        /// The <see cref="Task{Response}"/>.
        /// </returns>
        internal async Task ImportDataStore(HttpRequest request, HttpResponse response, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IJsonExchangeFileReader jsonExchangeFileReader, IMigrationService migrationService, IRevisionService revisionService, IEngineeringModelDao engineeringModelDao, IServiceProvider serviceProvider, IPersonService personService, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, IDataStoreController dataStoreController)
        {
            if (!this.AppConfigService.AppConfig.Backtier.IsDbImportEnabled)
            {
                Logger.Info("Data store import API invoked but it was disabled from configuration, cancel further processing...");

                response.StatusCode = (int)HttpStatusCode.Forbidden;
                await response.AsJson("Data store import is not allowed");
            }

            Logger.Info("Starting data store importing");

            var temporarysSeedFilePath = await this.SaveTemporarySeedFile(request);

            // drop existing data stores
            this.DropDataStoreAndPrepareNew(dataStoreController);

            var version = requestUtils.GetRequestDataModelVersion(request);

            // handle exchange processing
            if (!this.UpsertModelData(requestUtils, transactionManager, jsonExchangeFileReader, migrationService, revisionService, engineeringModelDao, serviceProvider, personService, personRoleService, personPermissionService, defaultPermissionProvider, participantRoleService, participantPermissionService, version, temporarysSeedFilePath, null))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await response.AsJson("invalid seed file");
            }

            // Remove the exchange file after processing (saving space)
            try
            {
                System.IO.File.Delete(temporarysSeedFilePath);
            }
            catch (Exception ex)
            {
                // swallow exception but log it
                Logger.Error(ex, "Unable to remove file {0}", temporarysSeedFilePath);
            }

            try
            {
                // TODO: check what to do with this in origianl CDP4 code
                // reset the credential cache as the underlying datastore was reset
                //this.WebServiceAuthentication.ResetCredentialCache();

                Logger.Info("Finished the data store import");

                response.StatusCode = (int)HttpStatusCode.OK;
                await response.AsJson("Datastore imported");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to import the datastore");

                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.AsJson("Data store import failed");
            }
        }

        /// <summary>
        /// Asynchronously seed the data store.
        /// </summary>
        /// <param name="request">
        /// The <see cref="HttpRequest"/> that is being handled
        /// </param>
        /// <param name="response">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        internal async Task SeedDataStore(HttpRequest request, HttpResponse response, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IJsonExchangeFileReader jsonExchangeFileReader, IMigrationService migrationService, IRevisionService revisionService, IEngineeringModelDao engineeringModelDao, IServiceProvider serviceProvider, IPersonService personService, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, IDataStoreController dataStoreController, ISiteDirectoryService siteDirectoryService, IEngineeringModelSetupService engineeringModelSetupService)
        {
            if (!this.AppConfigService.AppConfig.Backtier.IsDbSeedEnabled)
            {
                Logger.Info("Data store seed API invoked but it was disabled from configuration, cancel further processing...");
                
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                await response.AsJson("seed is not allowed");
            }

            Logger.Info("Starting data store seeding");

            var temporarysSeedFilePath = await this.SaveTemporarySeedFile(request);

            // drop existing data stores
            this.DropDataStoreAndPrepareNew(dataStoreController);

            var version = requestUtils.GetRequestDataModelVersion(request);

            // handle exchange processing
            if (!this.InsertModelData(requestUtils, transactionManager, jsonExchangeFileReader, migrationService, revisionService, engineeringModelDao, serviceProvider, personService, personRoleService, personPermissionService, defaultPermissionProvider, participantRoleService, participantPermissionService, version, temporarysSeedFilePath, null, this.AppConfigService.AppConfig.Backtier.IsDbSeedEnabled))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await response.AsJson("invalid seed file");
            }

            // Remove the exchange file after processing (saving space)
            try
            {
                System.IO.File.Delete(temporarysSeedFilePath);
            }
            catch (Exception ex)
            {
                // swallow exception but log it
                Logger.Error(ex, "Unable to remove file");
            }

            try
            {
                // Create a jsonb for each entry in the database
                this.CreateRevisionHistoryForEachEntry(requestUtils, transactionManager, revisionService, personService, siteDirectoryService, engineeringModelSetupService);

                // database was succesfully seeded
                // create a clone of the data store for future restore support
                dataStoreController.CloneDataStore();

                // TODO: check what to do with this in origianl CDP4 code
                // reset the credential cache as the underlying datastore was reset
                //this.WebServiceAuthentication.ResetCredentialCache();

                Logger.Info("Finished the data store seed");

                response.StatusCode = (int) HttpStatusCode.OK;
                await response.AsJson("Datastore seeded");
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.AsJson("DataStore restored failed");
            }
        }

        /// <summary>
        /// Parse the url segments and return the data as serialized JSON
        /// </summary>
        /// <param name="fileName">
        /// The exchange file name.
        /// </param>
        /// <param name="password">
        /// The optional archive password as supplied by the request
        /// </param>
        /// <param name="seed">
        /// Optional seed indicator which removes any existing data first if true
        /// </param>
        /// <returns>
        /// True if successful, false if not
        /// </returns>
        private bool InsertModelData(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IJsonExchangeFileReader jsonExchangeFileReader, IMigrationService migrationService, IRevisionService revisionService, IEngineeringModelDao engineeringModelDao, IServiceProvider serviceProvider,  IPersonService personService, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, Version version, string fileName, string password = null, bool seed = true)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            try
            {
                var sw = new Stopwatch();
                if (seed)
                {
                    // clear database schemas if seeding
                    Logger.Info("Start clearing the current data store");
                    transaction = transactionManager.SetupTransaction(ref connection, null);
                    transactionManager.SetFullAccessState(true);
                    this.ClearDatabaseSchemas(transaction);
                    transaction.Commit();

                    // Flushes the type cache and reload the types for this connection
                    connection.ReloadTypes();
                }

                sw.Start();
                Logger.Info("Start seeding the data");

                // use new transaction to for inserting the data
                transaction = transactionManager.SetupTransaction(ref connection, null);
                transactionManager.SetFullAccessState(true);

                // important, make sure to defer the constraints
                var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED;", transaction.Connection, transaction);
                command.ExecuteAndLogNonQuery(transactionManager.CommandLogger);

                // make sure to only log insert changes, no subsequent trigger updates for exchange import
                transactionManager.SetAuditLoggingState(transaction, false);

                // get sitedirectory data
                var items = jsonExchangeFileReader.ReadSiteDirectoryFromfile(version, fileName, password).ToList();

                // assign default password to all imported persons.
                foreach (var person in items.OfType<Person>())
                {
                    person.Password = this.AppConfigService.AppConfig.Defaults.PersonPassword;
                }

                var topContainer = items.SingleOrDefault(x => x.IsSameOrDerivedClass<TopContainer>()) as TopContainer;
                if (topContainer == null)
                {
                    Logger.Error("No Topcontainer item encountered");
                    throw new NoNullAllowedException("Topcontainer item needs to be present in the dataset");
                }

                requestUtils.Cache = new List<Thing>(items);

                // setup Site Directory schema
                using (var siteDirCommand = new NpgsqlCommand())
                {
                    Logger.Info("Start Site Directory structure");
                    siteDirCommand.ReadSqlFromResource("CDP4Orm.AutoGenStructure.SiteDirectoryDefinition.sql");

                    siteDirCommand.Connection = transaction.Connection;
                    siteDirCommand.Transaction = transaction;
                    siteDirCommand.ExecuteAndLogNonQuery(transactionManager.CommandLogger);
                }

                // apply migration on new SiteDirectory partition
                migrationService.ApplyMigrations(transaction, typeof(SiteDirectory).Name, false);

                var result = false;
                if (topContainer.GetType().Name == "SiteDirectory")
                {
                    // make sure single iterationsetups are set to unfrozen before persitence
                    this.FixSingleIterationSetups(items);
                    var siteDirectoryService =
                        serviceProvider.MapToPersitableService<SiteDirectoryService>("SiteDirectory");

                    result = siteDirectoryService.Insert(transaction, "SiteDirectory", topContainer);
                }

                if (result)
                {
                    requestUtils.QueryParameters = new QueryParameters();

                    // Get users credentials from migration.json file
                    var migrationCredentials = jsonExchangeFileReader.ReadMigrationJsonFromFile(fileName, password).ToList();

                    foreach (var person in items.OfType<Person>())
                    {
                        var credential = migrationCredentials.FirstOrDefault(mc => mc.Iid == person.Iid);
                        if (credential != null)
                        {
                            personService.UpdateCredentials(transaction, "SiteDirectory", person, credential);
                        }
                    }

                    // Add missing Person permissions
                    this.CreateMissingPersonPermissions(transaction, personRoleService, personPermissionService, defaultPermissionProvider);

                    var engineeringModelSetups =
                        items.OfType<EngineeringModelSetup>()
                            .ToList();
                    var engineeringModelService =
                        serviceProvider.MapToPersitableService<EngineeringModelService>("EngineeringModel");
                    var iterationService = serviceProvider.MapToPersitableService<IterationService>("Iteration");

                    foreach (var engineeringModelSetup in engineeringModelSetups)
                    {
                        // cleanup before handling TopContainer
                        requestUtils.Cache.Clear();

                        // get referenced engineeringmodel data
                        var engineeringModelItems = jsonExchangeFileReader
                            .ReadEngineeringModelFromfile(version, fileName, password, engineeringModelSetup).ToList();

                        // should return one engineeringmodel topcontainer
                        var engineeringModel = engineeringModelItems.OfType<EngineeringModel>().Single();
                        if (engineeringModel == null)
                        {
                            result = false;
                            break;
                        }

                        var dataPartition = CDP4Orm.Dao.Utils.GetEngineeringModelSchemaName(engineeringModel.Iid);
                        requestUtils.Cache = new List<Thing>(engineeringModelItems);

                        if (!engineeringModelService.Insert(transaction, dataPartition, engineeringModel))
                        {
                            result = false;
                            break;
                        }

                        // Add missing Participant permissions
                        this.CreateMissingParticipantPermissions(transaction, participantRoleService, participantPermissionService, defaultPermissionProvider);

                        // extract any referenced file data to disk if not already present
                        this.PersistFileBinaryData(requestUtils, jsonExchangeFileReader, fileName, password);

                        var iterationSetups = items.OfType<IterationSetup>()
                            .Where(
                                x => engineeringModelSetup.IterationSetup.Contains(x.Iid)).ToList();

                        // get current maximum iterationNumber and increase by one for the next value
                        int maxIterationNumber = iterationSetups.Select(x => x.IterationNumber).Max() + IterationNumberSequenceInitialization;

                        // reset the start number of iterationNumber sequence
                        engineeringModelDao.ResetIterationNumberSequenceStartNumber(
                            transaction,
                            dataPartition,
                            maxIterationNumber);

                        var iterationInsertResult = true;
                        foreach (var iterationSetup in iterationSetups)
                        {
                            requestUtils.Cache.Clear();
                            var iterationItems = jsonExchangeFileReader
                                .ReadModelIterationFromFile(version, fileName, password, iterationSetup).ToList();

                            // FixRevisionNumber(iterationItems);
                            requestUtils.Cache = new List<Thing>(iterationItems);

                            // should return one iteration
                            var iteration =
                                iterationItems.SingleOrDefault(x => x.ClassKind == ClassKind.Iteration) as Iteration;
                            if (iteration == null || !iterationService.CreateConcept(
                                    transaction,
                                    dataPartition,
                                    iteration,
                                    engineeringModel))
                            {
                                iterationInsertResult = false;
                                break;
                            }
                        }

                        if (!iterationInsertResult)
                        {
                            result = false;
                            break;
                        }

                        // extract any referenced file data to disk if not already present
                        this.PersistFileBinaryData(requestUtils, jsonExchangeFileReader, fileName, password);
                    }
                }

                transaction.Commit();
                sw.Stop();
                Logger.Info("Finished seeding the data store in {0} [ms]", sw.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                Logger.Error(ex, "Error occured during data store seeding");

                return false;
            }
            finally
            {
                // clean log (will happen at end of request as well due to IOC lifetime
                transactionManager.CommandLogger.ClearLog();

                transaction?.Dispose();

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Import data and use Upsert flow to add/update data. Return the data as serialized JSON
        /// </summary>
        /// <param name="fileName">
        /// The exchange file name.
        /// </param>
        /// <param name="password">
        /// The optional archive password as supplied by the request
        /// </param>
        /// <returns>
        /// True if successful
        /// </returns>
        private bool UpsertModelData(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IJsonExchangeFileReader jsonExchangeFileReader, IMigrationService migrationService, IRevisionService revisionService, IEngineeringModelDao engineeringModelDao, IServiceProvider serviceProvider, IPersonService personService, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, Version version, string fileName, string password = null)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            try
            {
                var sw = new Stopwatch();

                // clear database schemas if seeding
                Logger.Info("Start clearing the current data store");
                transaction = transactionManager.SetupTransaction(ref connection, null);
                transactionManager.SetFullAccessState(true);
                this.ClearDatabaseSchemas(transaction);
                transaction.Commit();

                // Flushes the type cache and reload the types for this connection
                connection.ReloadTypes();

                sw.Start();
                Logger.Info("Start importing the data");

                // use new transaction to for inserting the data
                transaction = transactionManager.SetupTransaction(ref connection, null);
                transactionManager.SetFullAccessState(true);

                // important, make sure to defer the constraints
                var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED;", transaction.Connection, transaction);
                command.ExecuteAndLogNonQuery(transactionManager.CommandLogger);

                // make sure to only log insert changes, no subsequent trigger updates for exchange import
                transactionManager.SetAuditLoggingState(transaction, true);

                // get sitedirectory data
                var items = jsonExchangeFileReader.ReadSiteDirectoryFromfile(version, fileName, password).ToList();

                // assign default password to all imported persons.
                foreach (var person in items.OfType<Person>())
                {
                    person.Password = this.AppConfigService.AppConfig.Defaults.PersonPassword;
                }

                var topContainer = items.SingleOrDefault(x => x.IsSameOrDerivedClass<TopContainer>()) as TopContainer;

                if (topContainer == null)
                {
                    Logger.Error("No Topcontainer item encountered");
                    throw new NoNullAllowedException("Topcontainer item needs to be present in the dataset");
                }

                requestUtils.Cache = new List<Thing>(items);

                // setup Site Directory schema
                using (var siteDirCommand = new NpgsqlCommand())
                {
                    Logger.Info("Start Site Directory structure");
                    siteDirCommand.ReadSqlFromResource("CDP4Orm.AutoGenStructure.SiteDirectoryDefinition.sql");

                    siteDirCommand.Connection = transaction.Connection;
                    siteDirCommand.Transaction = transaction;
                    siteDirCommand.ExecuteAndLogNonQuery(transactionManager.CommandLogger);
                }

                // apply migration on new SiteDirectory partition
                migrationService.ApplyMigrations(transaction, nameof(SiteDirectory), false);

                var result = false;

                if (topContainer.GetType().Name == "SiteDirectory")
                {
                    // make sure single iterationsetups are set to unfrozen before persitence
                    this.FixSingleIterationSetups(items);

                    var siteDirectoryService = serviceProvider.MapToPersitableService<SiteDirectoryService>("SiteDirectory");

                    result = siteDirectoryService.Insert(transaction, "SiteDirectory", topContainer);
                }

                if (result)
                {
                    requestUtils.QueryParameters = new QueryParameters();

                    // Get users credentials from migration.json file
                    var migrationCredentials = jsonExchangeFileReader.ReadMigrationJsonFromFile(fileName, password).ToList();

                    foreach (var person in items.OfType<Person>())
                    {
                        var credential = migrationCredentials.FirstOrDefault(mc => mc.Iid == person.Iid);

                        if (credential != null)
                        {
                            personService.UpdateCredentials(transaction, "SiteDirectory", person, credential);
                        }
                    }

                    // Add missing Person permissions
                    this.CreateMissingPersonPermissions(transaction, personRoleService, personPermissionService, defaultPermissionProvider);

                    var engineeringModelSetups =
                        items.OfType<EngineeringModelSetup>()
                            .ToList();

                    var engineeringModelService =
                        serviceProvider.MapToPersitableService<EngineeringModelService>("EngineeringModel");

                    var iterationService = serviceProvider.MapToPersitableService<IterationService>("Iteration");
                    var createRevisionForSiteDirectory = true;
                    var actorId = Guid.Empty;

                    foreach (var engineeringModelSetup in engineeringModelSetups)
                    {
                        var revisionNumber = EngineeringModelSetupSideEffect.FirstRevision;

                        // cleanup before handling TopContainer
                        requestUtils.Cache.Clear();

                        // get referenced engineeringmodel data
                        var engineeringModelItems = jsonExchangeFileReader
                            .ReadEngineeringModelFromfile(version, fileName, password, engineeringModelSetup).ToList();

                        // should return one engineeringmodel topcontainer
                        var engineeringModel = engineeringModelItems.OfType<EngineeringModel>().Single();

                        if (engineeringModel == null)
                        {
                            result = false;
                            break;
                        }

                        var dataPartition = CDP4Orm.Dao.Utils.GetEngineeringModelSchemaName(engineeringModel.Iid);

                        var iterationPartition = CDP4Orm.Dao.Utils.GetEngineeringModelIterationSchemaName(engineeringModel.Iid);

                        requestUtils.Cache = new List<Thing>(engineeringModelItems);

                        if (!engineeringModelService.Insert(transaction, dataPartition, engineeringModel))
                        {
                            result = false;
                            break;
                        }

                        // Add missing Participant permissions
                        this.CreateMissingParticipantPermissions(transaction, participantRoleService, participantPermissionService, defaultPermissionProvider);

                        // extract any referenced file data to disk if not already present
                        this.PersistFileBinaryData(requestUtils, jsonExchangeFileReader, fileName, password);

                        var iterationSetups = items.OfType<IterationSetup>()
                            .Where(
                                x => engineeringModelSetup.IterationSetup.Contains(x.Iid)).ToList();

                        // get current maximum iterationNumber and increase by one for the next value
                        var maxIterationNumber = iterationSetups.Select(x => x.IterationNumber).Max() + IterationNumberSequenceInitialization;

                        // reset the start number of iterationNumber sequence
                        engineeringModelDao.ResetIterationNumberSequenceStartNumber(
                            transaction,
                            dataPartition,
                            maxIterationNumber);

                        var iterationInsertResult = true;
                        List<Thing> previousIterationItems = null;

                        foreach (var iterationSetup in iterationSetups.OrderBy(x => x.IterationNumber))
                        {
                            requestUtils.Cache.Clear();

                            var iterationItems = jsonExchangeFileReader
                                .ReadModelIterationFromFile(version, fileName, password, iterationSetup).ToList();

                            requestUtils.Cache = new List<Thing>(iterationItems);

                            // should return one iteration
                            // for the every model EngineeringModel schema ends with the same ID as Iteration schema
                            var iteration =
                                iterationItems.SingleOrDefault(x => x.ClassKind == ClassKind.Iteration) as Iteration;

                            iterationInsertResult = false;

                            if (iteration == null)
                            {
                                break;
                            }

                            if (iterationService.UpsertConcept(
                                transaction,
                                dataPartition,
                                iteration,
                                engineeringModel))
                            {
                                iterationInsertResult = true;

                                if (previousIterationItems != null)
                                {
                                    // Compute differences between iterations
                                    var thingsToBeDeleted = previousIterationItems
                                        .Where(thing => thing.ClassKind != ClassKind.Iteration &&
                                                        !iterationItems.Select(id => id.Iid).Contains(thing.Iid)).ToList();

                                    // Remove differences between iterations
                                    foreach (var thing in thingsToBeDeleted)
                                    {
                                        var service = serviceProvider.MapToPersitableService<IPersistService>(thing.ClassKind.ToString());
                                        service.RawDeleteConcept(transaction, iterationPartition, thing);
                                    }
                                }

                                transaction.Commit();

                                if (createRevisionForSiteDirectory)
                                {
                                    // Create revision history only once for SiteDirectory
                                    this.CreateRevisionHistoryForSiteDirectory(requestUtils, transactionManager, revisionService, personService, ref actorId);
                                    createRevisionForSiteDirectory = false;
                                }

                                // Create revision history for each EngineeringModel
                                this.CreateRevisionHistoryForEngineeringModel(requestUtils, transactionManager, revisionService, actorId, revisionNumber, engineeringModelSetup.EngineeringModelIid);

                                // use new transaction to for inserting the data
                                transaction = transactionManager.SetupTransaction(ref connection, null);
                                transactionManager.SetFullAccessState(true);

                                // important, make sure to defer the constraints
                                var constraintcommand = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED;", transaction.Connection, transaction);
                                constraintcommand.ExecuteAndLogNonQuery(transactionManager.CommandLogger);

                                // make sure to only log insert changes, no subsequent trigger updates for exchange import
                                transactionManager.SetAuditLoggingState(transaction, true);

                                // revision number goes up for the next Iteration
                                revisionNumber += 1;
                            }

                            previousIterationItems = iterationItems;
                        }

                        if (!iterationInsertResult)
                        {
                            result = false;
                            break;
                        }

                        // extract any referenced file data to disk if not already present
                        this.PersistFileBinaryData(requestUtils, jsonExchangeFileReader, fileName, password);
                    }
                }

                transaction.Commit();

                sw.Stop();
                Logger.Info("Finished importing the data store in {0} [ms]", sw.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                Logger.Error(ex, "Error occured during data store import");

                return false;
            }
            finally
            {
                // clean log (will happen at end of request as well due to IOC lifetime
                transactionManager.CommandLogger.ClearLog();

                transaction?.Dispose();

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Persist the file binary data in the reference zip archive.
        /// </summary>
        /// <param name="fileName">
        /// The file path of the zip archive being processed.
        /// </param>
        /// <param name="password">
        /// The archive password.
        /// </param>
        private void PersistFileBinaryData(IRequestUtils requestUtils, IJsonExchangeFileReader jsonExchangeFileReader,  string fileName, string password = null)
        {
            var fileRevisions = requestUtils.Cache.OfType<FileRevision>().ToList();
            if (!fileRevisions.Any())
            {
                // nothing to do
                return;
            }

            foreach (var hash in fileRevisions.Select(x => x.ContentHash).Distinct())
            {
                jsonExchangeFileReader.StoreFileBinary(fileName, password, hash);
            }
        }

        /// <summary>
        /// Fix single iteration setups in the export.
        /// An exchange file can contain one or all iteration setups of a site directory.
        /// Make sure that incase one iteration setup is supplied it is unfrozen and has no source iteration set
        /// </summary>
        /// <param name="items">
        /// The read in items.
        /// </param>
        private void FixSingleIterationSetups(List<Thing> items)
        {
            var engineeringModelSetups = items.OfType<EngineeringModelSetup>().ToList();

            // unset the FrozenOn and SourceIterationSetup properties for single iteration setup in the siteDirectory.
            foreach (var iterationSetups in engineeringModelSetups.Select(
                engineeringModelSetup =>
                    items.OfType<IterationSetup>().Where(
                            x => engineeringModelSetup.IterationSetup.Contains(x.Iid))
                         .ToList()).Where(iterationSetups => iterationSetups.Count == 1))
            {
                iterationSetups[0].FrozenOn = null;
                iterationSetups[0].SourceIterationSetup = null;
            }
        }

        /// <summary>
        /// The clear database schemas.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        private void ClearDatabaseSchemas(NpgsqlTransaction transaction)
        {
            // clear the current database data (except public and pg_catalog schemas)
            using (var cleanSchemaCommand = new NpgsqlCommand())
            {
                var sqlBuilder = new StringBuilder();

                // setup clear schema function if not existent
                // source: https://chawlasumit.wordpress.com/2014/07/29/drop-all-schemas-in-postgres/
                sqlBuilder.AppendLine("CREATE OR REPLACE FUNCTION public.clear_schemas()")
                    .AppendLine("    RETURNS VOID AS").AppendLine("    $$").AppendLine("    DECLARE rec RECORD;")
                    .AppendLine("    BEGIN").AppendLine("        -- Get all schemas").AppendLine("        FOR rec IN")
                    .AppendLine("        SELECT DISTINCT schema_name")
                    .AppendLine("        FROM information_schema.schemata").AppendLine("        -- exclude schemas")
                    .AppendLine(
                        "        WHERE schema_name NOT LIKE 'pg_%' AND schema_name NOT LIKE 'information_schema' AND schema_name <> 'public'")
                    .AppendLine("            LOOP")
                    .AppendLine("                EXECUTE 'DROP SCHEMA \"' || rec.schema_name || '\" CASCADE';")
                    .AppendLine("            END LOOP;").AppendLine("        RETURN;").AppendLine("    END;")
                    .AppendLine("    $$ LANGUAGE plpgsql;");

                sqlBuilder.AppendLine("SELECT clear_schemas();");

                cleanSchemaCommand.CommandText = sqlBuilder.ToString();
                cleanSchemaCommand.Connection = transaction.Connection;
                cleanSchemaCommand.Transaction = transaction;
                cleanSchemaCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Drops existing working and restore data stores if they exist
        /// and creates a new working data store.
        /// </summary>
        private void DropDataStoreAndPrepareNew(IDataStoreController dataStoreController)
        {
            Logger.Info("start dropping existing data stores");

            var backtierConfig = this.AppConfigService.AppConfig.Backtier;

            // Drop the existing databases
            using (var connection = new NpgsqlConnection(
                Services.Utils.GetConnectionString(backtierConfig, backtierConfig.DatabaseManage)))
            {
                connection.Open();

                // Drop the existing database
                using (var cmd = new NpgsqlCommand())
                {
                    Logger.Debug("Drop the data store");

                    dataStoreController.DropDataStoreConnections(backtierConfig.Database, connection);

                    cmd.Connection = connection;
                    
                    cmd.CommandText = $"DROP DATABASE IF EXISTS {backtierConfig.Database};";

                    cmd.ExecuteNonQuery();
                }

                // Drop the existing restore database
                using (var cmd = new NpgsqlCommand())
                {
                    Logger.Debug("Drop the restore data store");

                    cmd.Connection = connection;
                    
                    cmd.CommandText = $"DROP DATABASE IF EXISTS {backtierConfig.DatabaseRestore};";

                    cmd.ExecuteNonQuery();
                }

                // Create a new database
                using (var cmd = new NpgsqlCommand())
                {
                    Logger.Debug("Create the data store");
                    cmd.Connection = connection;

                    cmd.CommandText = $"CREATE DATABASE {backtierConfig.Database} WITH OWNER = {backtierConfig.UserName} TEMPLATE = {backtierConfig.DatabaseManage} ENCODING = 'UTF8';";

                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        /// <summary>
        /// Create revision history for SiteDirectory and retrieve first person Id
        /// </summary>
        /// <param name="personId">
        /// First person Id <see cref="Person" />
        /// </param>
        private void CreateRevisionHistoryForSiteDirectory(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IRevisionService revisionService, IPersonService personService,  ref Guid personId)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            requestUtils.QueryParameters = new QueryParameters();

            try
            {
                // Create a revision history for SiteDirectory's entries
                transaction = transactionManager.SetupTransaction(ref connection, null);
                transactionManager.SetFullAccessState(true);

                // Get first person Id (so that actor isnt guid.empty), it is hard to determine who it should be.
                var person = personService.GetShallow(transaction, TopContainer, null, new RequestSecurityContext { ContainerReadAllowed = true }).OfType<Person>().FirstOrDefault();
                personId = person != null ? person.Iid : Guid.Empty;

                // Save revision history for SiteDirectory's entries
                revisionService.SaveRevisions(transaction, TopContainer, personId, EngineeringModelSetupSideEffect.FirstRevision);

                transaction.Commit();
            }
            catch (NpgsqlException ex)
            {
                transaction?.Rollback();
                Logger.Error(ex, "Error occured during revision history creation");
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                Logger.Error(ex, "Error occured during revision history creation");
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Create revision history for each EngineeringModel in the database.
        /// </summary>
        /// <param name="revisionNumber">
        /// The revision number we want to create revision records for
        /// </param>
        /// <param name="personId">
        /// First person Id <see cref="Guid" />
        /// </param>
        /// <param name="engineeringModelIid">
        /// Engineering model Id <see cref="Guid" />
        /// </param>
        private void CreateRevisionHistoryForEngineeringModel(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IRevisionService revisionService, Guid personId, int revisionNumber, Guid engineeringModelIid)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            requestUtils.QueryParameters = new QueryParameters();

            try
            {
                transaction = transactionManager.SetupTransaction(ref connection, null);
                transactionManager.SetFullAccessState(true);

                var partition = requestUtils.GetEngineeringModelPartitionString(engineeringModelIid);

                // Save revision history for EngineeringModel's entries
                revisionService.SaveRevisions(transaction, partition, personId, revisionNumber);

                // commit revision history
                transaction.Commit();
            }
            catch (NpgsqlException ex)
            {
                transaction?.Rollback();
                Logger.Error(ex, "Error occured during revision history creation");
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                Logger.Error(ex, "Error occured during revision history creation");
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Create revision history for each entry in the database.
        /// </summary>
        private void CreateRevisionHistoryForEachEntry(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IRevisionService revisionService, IPersonService personService, ISiteDirectoryService siteDirectoryService, IEngineeringModelSetupService engineeringModelSetupService)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            requestUtils.QueryParameters = new QueryParameters();

            try
            {
                // Create a revision history for SiteDirectory's entries
                transaction = transactionManager.SetupTransaction(ref connection, null);
                transactionManager.SetFullAccessState(true);

                // Get first person Id (so that actor isnt guid.empty), it is hard to determine who it should be.
                var actor = personService.GetShallow(transaction, TopContainer, null, new RequestSecurityContext {ContainerReadAllowed = true}).OfType<Person>().FirstOrDefault();
                var actorId = actor != null ? actor.Iid : Guid.Empty;

                // Save revision history for SiteDirectory's entries
                revisionService.SaveRevisions(transaction, TopContainer, actorId, EngineeringModelSetupSideEffect.FirstRevision);

                var siteDirectory = siteDirectoryService.Get(
                    transaction,
                    TopContainer,
                    null,
                    new RequestSecurityContext { ContainerReadAllowed = true }).OfType<SiteDirectory>().ToList();

                var engineeringModelSetups = engineeringModelSetupService.GetShallow(
                        transaction,
                        TopContainer,
                        siteDirectory[0].Model,
                        new RequestSecurityContext { ContainerReadAllowed = true }).OfType<EngineeringModelSetup>()
                    .ToList();

                // commit revision history
                transaction.Commit();

                // Get all EngineeringModelSetups and create a revision history for each EngineeringModel
                foreach (var engineeringModelSetup in engineeringModelSetups)
                {
                    transaction = transactionManager.SetupTransaction(ref connection, null);
                    transactionManager.SetFullAccessState(true);

                    var partition =
                        requestUtils.GetEngineeringModelPartitionString(engineeringModelSetup.EngineeringModelIid);

                    // Save revision history for EngineeringModel's entries
                    revisionService.SaveRevisions(transaction, partition, actorId, EngineeringModelSetupSideEffect.FirstRevision);

                    // commit revision history
                    transaction.Commit();
                }
            }
            catch (NpgsqlException ex)
            {
                transaction?.Rollback();
                Logger.Error(ex, "Error occured during revision history creation");
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                Logger.Error(ex, "Error occured during revision history creation");
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Create missing participant permissions for all <see cref="ParticipantRole"/>s
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void CreateMissingParticipantPermissions(NpgsqlTransaction transaction, IParticipantRoleService participantRoleService, IParticipantPermissionService participantPermissionService, IDefaultPermissionProvider defaultPermissionProvider)
        {
            var participantRoles = participantRoleService.GetShallow(
                transaction,
                TopContainer,
                null,
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<ParticipantRole>();

            // Find and create all missing permissions for each Participant
            foreach (var participantRole in participantRoles)
            {
                this.FindAndCreateMissingParticipantPermissions(participantRole, transaction, participantPermissionService, defaultPermissionProvider);
            }
        }

        /// <summary>
        /// Find and create missing <see cref="ParticipantPermission"/>
        /// </summary>
        /// <param name="participantRole">
        /// The <see cref="ParticipantRole"/> to find and create <see cref="ParticipantPermission"/>s for.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void FindAndCreateMissingParticipantPermissions(ParticipantRole participantRole, NpgsqlTransaction transaction, IParticipantPermissionService participantPermissionService,  IDefaultPermissionProvider defaultPermissionProvider)
        {
            var participantPermissions = participantPermissionService.GetShallow(
                transaction,
                TopContainer,
                participantRole.ParticipantPermission,
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<ParticipantPermission>().ToList();

            foreach (var classKind in Enum.GetValues(typeof(ClassKind)).Cast<ClassKind>())
            {
                var defaultPermission = defaultPermissionProvider.GetDefaultParticipantPermission(classKind);

                if (defaultPermission == ParticipantAccessRightKind.NONE)
                {
                    var participantPermission = participantPermissions.Find(x => x.ObjectClass == classKind);

                    if (participantPermission == null)
                    {
                        Logger.Debug("Create ParticipantPermission for class {0} for ParticipantRole {1}", classKind, participantRole.Iid);

                        var permission = new ParticipantPermission(Guid.NewGuid(), 0)
                        {
                            ObjectClass = classKind,
                            AccessRight = defaultPermission
                        };

                        participantRole.ParticipantPermission.Add(permission.Iid);
                        participantPermissionService.CreateConcept(transaction, TopContainer, permission, participantRole);
                    }
                }
            }
        }

        /// <summary>
        /// Create missing person permissions.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void CreateMissingPersonPermissions(NpgsqlTransaction transaction, IPersonRoleService personRoleService, IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider)
        {
            var personRoles = personRoleService.GetShallow(
                transaction,
                TopContainer,
                null,
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<PersonRole>();

            // Find and create all missing permissions for each Person
            foreach (var personRole in personRoles)
            {
                this.FindAndCreateMissingPersonPermissions(personPermissionService, defaultPermissionProvider, personRole, transaction);
            }
        }

        /// <summary>
        /// Find and create missing <see cref="PersonPermission"/>s
        /// </summary>
        /// <param name="personRole">
        /// The <see cref="PersonRole"/> to find and create <see cref="PersonPermission"/> for.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void FindAndCreateMissingPersonPermissions(IPersonPermissionService personPermissionService, IDefaultPermissionProvider defaultPermissionProvider, PersonRole personRole, NpgsqlTransaction transaction)
        {
            var personPermissions = personPermissionService.GetShallow(
                transaction,
                TopContainer,
                personRole.PersonPermission,
                new RequestSecurityContext { ContainerReadAllowed = true }).OfType<PersonPermission>().ToList();

            foreach (var classKind in Enum.GetValues(typeof(ClassKind)).Cast<ClassKind>())
            {
                var defaultPermission = defaultPermissionProvider.GetDefaultPersonPermission(classKind);

                if (defaultPermission == PersonAccessRightKind.NONE)
                {
                    var personPermission = personPermissions.Find(x => x.ObjectClass == classKind);

                    if (personPermission == null)
                    {
                        Logger.Debug("Create PersonPermission for class {0} for PersonRole {1}", classKind, personRole.Iid);

                        var permission = new PersonPermission(Guid.NewGuid(), 0)
                        {
                            ObjectClass = classKind,
                            AccessRight = defaultPermission
                        };

                        personRole.PersonPermission.Add(permission.Iid);
                        personPermissionService.CreateConcept(transaction, TopContainer, permission, personRole);
                    }
                }
            }
        }
    }
}