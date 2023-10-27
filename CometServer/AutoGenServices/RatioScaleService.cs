// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RatioScaleService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2022 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using CometServer.Services.Authorization;
    using Microsoft.Extensions.Logging;
    using Npgsql;

    /// <summary>
    /// The <see cref="RatioScale"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class RatioScaleService : ServiceBase, IRatioScaleService
    {
        /// <summary>
        /// Gets or sets the <see cref="ICyclicRatioScaleService"/>.
        /// </summary>
        public ICyclicRatioScaleService CyclicRatioScaleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAliasService"/>.
        /// </summary>
        public IAliasService AliasService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefinitionService"/>.
        /// </summary>
        public IDefinitionService DefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IHyperLinkService"/>.
        /// </summary>
        public IHyperLinkService HyperLinkService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMappingToReferenceScaleService"/>.
        /// </summary>
        public IMappingToReferenceScaleService MappingToReferenceScaleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IScaleValueDefinitionService"/>.
        /// </summary>
        public IScaleValueDefinitionService ValueDefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRatioScaleDao"/>.
        /// </summary>
        public IRatioScaleDao RatioScaleDao { get; set; }

        /// <summary>
        /// Get the requested <see cref="RatioScale"/>s from the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The security context of the container instance.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RatioScale"/>, optionally with contained <see cref="Thing"/>s.
        /// </returns>
        public IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            return this.RequestUtils.QueryParameters.ExtentDeep
                        ? this.GetDeep(transaction, partition, ids, containerSecurityContext)
                        : this.GetShallow(transaction, partition, ids, containerSecurityContext);
        }

        /// <summary>
        /// Add the supplied value to the association link table indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be persisted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="RatioScale"/> id that will be the source for each link table record.
        /// </param>
        /// <param name="value">
        /// A value for which a link table record will be created.
        /// </param>
        /// <returns>
        /// True if the link was created.
        /// </returns>
        public bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return this.RatioScaleDao.AddToCollectionProperty(transaction, partition, propertyName, iid, value);
        }

        /// <summary>
        /// Remove the supplied value from the association property as indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="propertyName">
        /// The association property from where the supplied value will be removed.
        /// </param>
        /// <param name="iid">
        /// The <see cref="RatioScale"/> id that is the source of the link table records.
        /// </param>
        /// <param name="value">
        /// A value for which the link table record will be removed.
        /// </param>
        /// <returns>
        /// True if the link was removed.
        /// </returns>
        public bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return this.RatioScaleDao.DeleteFromCollectionProperty(transaction, partition, propertyName, iid, value);
        }

        /// <summary>
        /// Reorder the supplied value collection of the association link table indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource order will be updated.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be reordered.
        /// </param>
        /// <param name="iid">
        /// The <see cref="RatioScale"/> id that is the source for the reordered link table record.
        /// </param>
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// True if the link was created.
        /// </returns>
        public bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            return this.RatioScaleDao.ReorderCollectionProperty(transaction, partition, propertyName, iid, orderUpdate);
        }

        /// <summary>
        /// Update the containment order as indicated by the supplied orderedItem.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource order will be updated.
        /// </param>
        /// <param name="orderedItem">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// True if the contained item was successfully reordered.
        /// </returns>
        public bool ReorderContainment(NpgsqlTransaction transaction, string partition, CDP4Common.Types.OrderedItem orderedItem)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Delete the supplied <see cref="RatioScale"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="RatioScale"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="RatioScale"/> to be removed.
        /// </param>
        /// <returns>
        /// True if the removal was successful.
        /// </returns>
        public bool DeleteConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, DeleteOperation))
            {
                throw new SecurityException("The person " + this.CredentialsService.Credentials.Person.UserName + " does not have an appropriate delete permission for " + thing.GetType().Name + ".");
            }

            return this.RatioScaleDao.Delete(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Delete the supplied <see cref="RatioScale"/> instance.
        /// A "Raw" Delete means that the delete is performed without calling before-, or after actions, or other side effects.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="RatioScale"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="RatioScale"/> to be removed.
        /// </param>
        /// <returns>
        /// True if the removal was successful.
        /// </returns>
        public bool RawDeleteConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {

            return this.RatioScaleDao.RawDelete(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Update the supplied <see cref="RatioScale"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="RatioScale"/> <see cref="Thing"/> to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="RatioScale"/> to be updated.
        /// </param>
        /// <returns>
        /// True if the update was successful.
        /// </returns>
        public bool UpdateConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, UpdateOperation))
            {
                throw new SecurityException("The person " + this.CredentialsService.Credentials.Person.UserName + " does not have an appropriate update permission for " + thing.GetType().Name + ".");
            }

            var ratioScale = thing as RatioScale;
            if (ratioScale.IsSameOrDerivedClass<CyclicRatioScale>())
            {
                return this.CyclicRatioScaleService.UpdateConcept(transaction, partition, ratioScale, container);
            }
            return this.RatioScaleDao.Update(transaction, partition, ratioScale, container);
        }

        /// <summary>
        /// Persist the supplied <see cref="RatioScale"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="RatioScale"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="RatioScale"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        public bool CreateConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, CreateOperation))
            {
                throw new SecurityException("The person " + this.CredentialsService.Credentials.Person.UserName + " does not have an appropriate create permission for " + thing.GetType().Name + ".");
            }

            var ratioScale = thing as RatioScale;
            if (ratioScale.IsSameOrDerivedClass<CyclicRatioScale>())
            {
                return this.CyclicRatioScaleService.CreateConcept(transaction, partition, ratioScale, container);
            }

            var createSuccesful = this.RatioScaleDao.Write(transaction, partition, ratioScale, container);
            return createSuccesful && this.CreateContainment(transaction, partition, ratioScale);
        }

        /// <summary>
        /// Persist the supplied <see cref="RatioScale"/> instance. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="RatioScale"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="RatioScale"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        public bool UpsertConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            var ratioScale = thing as RatioScale;
            if (ratioScale.IsSameOrDerivedClass<CyclicRatioScale>())
            {
                return this.CyclicRatioScaleService.UpsertConcept(transaction, partition, ratioScale, container);
            }

            var createSuccesful = this.RatioScaleDao.Upsert(transaction, partition, ratioScale, container);
            return createSuccesful && this.UpsertContainment(transaction, partition, ratioScale);
        }

        /// <summary>
        /// Get the requested data from the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The security context of the container instance.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RatioScale"/>.
        /// </returns>
        public IEnumerable<Thing> GetShallow(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("RatioScale", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && this.BeforeGet(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var ratioScaleColl = new List<Thing>(this.RatioScaleDao.Read(transaction, partition, idFilter, this.TransactionManager.IsCachedDtoReadEnabled(transaction), (DateTime)this.TransactionManager.GetRawSessionInstant(transaction)));

            ratioScaleColl.AddRange(
                this.RequestUtils.QueryParameters.ExtentDeep
                    ? this.CyclicRatioScaleService.GetDeep(transaction, partition, idFilter, authorizedContext)
                    : this.CyclicRatioScaleService.GetShallow(transaction, partition, idFilter, authorizedContext));

            return this.AfterGet(ratioScaleColl, transaction, partition, idFilter);
        }

        /// <summary>
        /// Get the requested data from the ORM layer by chaining the containment properties.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The security context of the container instance.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RatioScale"/> and contained <see cref="Thing"/>s.
        /// </returns>
        public IEnumerable<Thing> GetDeep(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>(this.GetShallow(transaction, partition, idFilter, containerSecurityContext));
            var ratioScaleColl = results.Where(i => i.GetType() == typeof(RatioScale)).Cast<RatioScale>().ToList();

            results.AddRange(this.AliasService.GetDeep(transaction, partition, ratioScaleColl.SelectMany(x => x.Alias), containerSecurityContext));
            results.AddRange(this.DefinitionService.GetDeep(transaction, partition, ratioScaleColl.SelectMany(x => x.Definition), containerSecurityContext));
            results.AddRange(this.HyperLinkService.GetDeep(transaction, partition, ratioScaleColl.SelectMany(x => x.HyperLink), containerSecurityContext));
            results.AddRange(this.MappingToReferenceScaleService.GetDeep(transaction, partition, ratioScaleColl.SelectMany(x => x.MappingToReferenceScale), containerSecurityContext));
            results.AddRange(this.ValueDefinitionService.GetDeep(transaction, partition, ratioScaleColl.SelectMany(x => x.ValueDefinition), containerSecurityContext));

            return results;
        }

        /// <summary>
        /// Execute additional logic after each GET function call.
        /// </summary>
        /// <param name="resultCollection">
        /// An instance collection that was retrieved from the persistence layer.
        /// </param>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from which the requested resource is to be retrieved.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="includeReferenceData">
        /// Control flag to indicate if reference library data should be retrieved extent=deep or extent=shallow.
        /// </param>
        /// <returns>
        /// A post filtered instance of the passed in resultCollection.
        /// </returns>
        public override IEnumerable<Thing> AfterGet(IEnumerable<Thing> resultCollection, NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, bool includeReferenceData = false)
        {
            var filteredCollection = new List<Thing>();
            foreach (var thing in resultCollection)
            {
                if (this.IsInstanceReadAllowed(transaction, thing, partition))
                {
                    filteredCollection.Add(thing);
                }
                else
                {
                    Logger.Trace("The person {0} does not have a read permission for {1}.", this.CredentialsService.Credentials.Person.UserName, thing.GetType().Name);
                }
            }

            return filteredCollection;
        }

        /// <summary>
        /// Persist the <see cref="RatioScale"/> containment tree to the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="ratioScale">
        /// The <see cref="RatioScale"/> instance to persist.
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        private bool CreateContainment(NpgsqlTransaction transaction, string partition, RatioScale ratioScale)
        {
            var results = new List<bool>();

            foreach (var alias in this.ResolveFromRequestCache(ratioScale.Alias))
            {
                results.Add(this.AliasService.CreateConcept(transaction, partition, alias, ratioScale));
            }

            foreach (var definition in this.ResolveFromRequestCache(ratioScale.Definition))
            {
                results.Add(this.DefinitionService.CreateConcept(transaction, partition, definition, ratioScale));
            }

            foreach (var hyperLink in this.ResolveFromRequestCache(ratioScale.HyperLink))
            {
                results.Add(this.HyperLinkService.CreateConcept(transaction, partition, hyperLink, ratioScale));
            }

            foreach (var mappingToReferenceScale in this.ResolveFromRequestCache(ratioScale.MappingToReferenceScale))
            {
                results.Add(this.MappingToReferenceScaleService.CreateConcept(transaction, partition, mappingToReferenceScale, ratioScale));
            }

            foreach (var valueDefinition in this.ResolveFromRequestCache(ratioScale.ValueDefinition))
            {
                results.Add(this.ValueDefinitionService.CreateConcept(transaction, partition, valueDefinition, ratioScale));
            }

            return results.All(x => x);
        }
                
        /// <summary>
        /// Persist the <see cref="RatioScale"/> containment tree to the ORM layer. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="ratioScale">
        /// The <see cref="RatioScale"/> instance to persist.
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        private bool UpsertContainment(NpgsqlTransaction transaction, string partition, RatioScale ratioScale)
        {
            var results = new List<bool>();

            foreach (var alias in this.ResolveFromRequestCache(ratioScale.Alias))
            {
                results.Add(this.AliasService.UpsertConcept(transaction, partition, alias, ratioScale));
            }

            foreach (var definition in this.ResolveFromRequestCache(ratioScale.Definition))
            {
                results.Add(this.DefinitionService.UpsertConcept(transaction, partition, definition, ratioScale));
            }

            foreach (var hyperLink in this.ResolveFromRequestCache(ratioScale.HyperLink))
            {
                results.Add(this.HyperLinkService.UpsertConcept(transaction, partition, hyperLink, ratioScale));
            }

            foreach (var mappingToReferenceScale in this.ResolveFromRequestCache(ratioScale.MappingToReferenceScale))
            {
                results.Add(this.MappingToReferenceScaleService.UpsertConcept(transaction, partition, mappingToReferenceScale, ratioScale));
            }

            foreach (var valueDefinition in this.ResolveFromRequestCache(ratioScale.ValueDefinition))
            {
                results.Add(this.ValueDefinitionService.UpsertConcept(transaction, partition, valueDefinition, ratioScale));
            }

            return results.All(x => x);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
