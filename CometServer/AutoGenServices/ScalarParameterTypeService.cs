// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScalarParameterTypeService.cs" company="RHEA System S.A.">
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
    /// The <see cref="ScalarParameterType"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class ScalarParameterTypeService : ServiceBase, IScalarParameterTypeService
    {
        /// <summary>
        /// Gets or sets the <see cref="IBooleanParameterTypeService"/>.
        /// </summary>
        public IBooleanParameterTypeService BooleanParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDateParameterTypeService"/>.
        /// </summary>
        public IDateParameterTypeService DateParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDateTimeParameterTypeService"/>.
        /// </summary>
        public IDateTimeParameterTypeService DateTimeParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEnumerationParameterTypeService"/>.
        /// </summary>
        public IEnumerationParameterTypeService EnumerationParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IQuantityKindService"/>.
        /// </summary>
        public IQuantityKindService QuantityKindService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ITextParameterTypeService"/>.
        /// </summary>
        public ITextParameterTypeService TextParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ITimeOfDayParameterTypeService"/>.
        /// </summary>
        public ITimeOfDayParameterTypeService TimeOfDayParameterTypeService { get; set; }

        /// <summary>
        /// Get the requested <see cref="ScalarParameterType"/>s from the ORM layer.
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
        /// List of instances of <see cref="ScalarParameterType"/>, optionally with contained <see cref="Thing"/>s.
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
        /// The <see cref="ScalarParameterType"/> id that will be the source for each link table record.
        /// </param>
        /// <param name="value">
        /// A value for which a link table record will be created.
        /// </param>
        /// <returns>
        /// True if the link was created.
        /// </returns>
        public bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotSupportedException();
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
        /// The <see cref="ScalarParameterType"/> id that is the source of the link table records.
        /// </param>
        /// <param name="value">
        /// A value for which the link table record will be removed.
        /// </param>
        /// <returns>
        /// True if the link was removed.
        /// </returns>
        public bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            throw new NotSupportedException();
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
        /// The <see cref="ScalarParameterType"/> id that is the source for the reordered link table record.
        /// </param>
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// True if the link was created.
        /// </returns>
        public bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            throw new NotSupportedException();
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
        /// Delete the supplied <see cref="ScalarParameterType"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ScalarParameterType"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ScalarParameterType"/> to be removed.
        /// </param>
        /// <returns>
        /// True if the removal was successful.
        /// </returns>
        public bool DeleteConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Delete the supplied <see cref="ScalarParameterType"/> instance.
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
        /// The <see cref="ScalarParameterType"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ScalarParameterType"/> to be removed.
        /// </param>
        /// <returns>
        /// True if the removal was successful.
        /// </returns>
        public bool RawDeleteConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Update the supplied <see cref="ScalarParameterType"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ScalarParameterType"/> <see cref="Thing"/> to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ScalarParameterType"/> to be updated.
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

            var scalarParameterType = thing as ScalarParameterType;
            if (scalarParameterType.IsSameOrDerivedClass<BooleanParameterType>())
            {
                return this.BooleanParameterTypeService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateParameterType>())
            {
                return this.DateParameterTypeService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateTimeParameterType>())
            {
                return this.DateTimeParameterTypeService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<EnumerationParameterType>())
            {
                return this.EnumerationParameterTypeService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<QuantityKind>())
            {
                return this.QuantityKindService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TextParameterType>())
            {
                return this.TextParameterTypeService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TimeOfDayParameterType>())
            {
                return this.TimeOfDayParameterTypeService.UpdateConcept(transaction, partition, scalarParameterType, container);
            }
            throw new NotSupportedException(string.Format("The supplied DTO type: {0} is not a supported type", scalarParameterType.GetType().Name));
        }

        /// <summary>
        /// Persist the supplied <see cref="ScalarParameterType"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ScalarParameterType"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ScalarParameterType"/> to be persisted.
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

            var scalarParameterType = thing as ScalarParameterType;
            if (scalarParameterType.IsSameOrDerivedClass<BooleanParameterType>())
            {
                return this.BooleanParameterTypeService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateParameterType>())
            {
                return this.DateParameterTypeService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateTimeParameterType>())
            {
                return this.DateTimeParameterTypeService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<EnumerationParameterType>())
            {
                return this.EnumerationParameterTypeService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<QuantityKind>())
            {
                return this.QuantityKindService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TextParameterType>())
            {
                return this.TextParameterTypeService.CreateConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TimeOfDayParameterType>())
            {
                return this.TimeOfDayParameterTypeService.CreateConcept(transaction, partition, scalarParameterType, container);
            }
            throw new NotSupportedException(string.Format("The supplied DTO type: {0} is not a supported type", scalarParameterType.GetType().Name));
        }

        /// <summary>
        /// Persist the supplied <see cref="ScalarParameterType"/> instance. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="ScalarParameterType"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="ScalarParameterType"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        public bool UpsertConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            var scalarParameterType = thing as ScalarParameterType;
            if (scalarParameterType.IsSameOrDerivedClass<BooleanParameterType>())
            {
                return this.BooleanParameterTypeService.UpsertConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateParameterType>())
            {
                return this.DateParameterTypeService.UpsertConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<DateTimeParameterType>())
            {
                return this.DateTimeParameterTypeService.UpsertConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<EnumerationParameterType>())
            {
                return this.EnumerationParameterTypeService.UpsertConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<QuantityKind>())
            {
                return this.QuantityKindService.UpsertConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TextParameterType>())
            {
                return this.TextParameterTypeService.UpsertConcept(transaction, partition, scalarParameterType, container);
            }

            if (scalarParameterType.IsSameOrDerivedClass<TimeOfDayParameterType>())
            {
                return this.TimeOfDayParameterTypeService.UpsertConcept(transaction, partition, scalarParameterType, container);
            }
            throw new NotSupportedException(string.Format("The supplied DTO type: {0} is not a supported type", scalarParameterType.GetType().Name));
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
        /// List of instances of <see cref="ScalarParameterType"/>.
        /// </returns>
        public IEnumerable<Thing> GetShallow(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("ScalarParameterType", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && this.BeforeGet(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var scalarParameterTypeColl = new List<Thing>();
            scalarParameterTypeColl.AddRange(this.BooleanParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(this.DateParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(this.DateTimeParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(this.EnumerationParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(this.QuantityKindService.GetShallow(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(this.TextParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));
            scalarParameterTypeColl.AddRange(this.TimeOfDayParameterTypeService.GetShallow(transaction, partition, idFilter, authorizedContext));

            return this.AfterGet(scalarParameterTypeColl, transaction, partition, idFilter);
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
        /// List of instances of <see cref="ScalarParameterType"/> and contained <see cref="Thing"/>s.
        /// </returns>
        public IEnumerable<Thing> GetDeep(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>();
            results.AddRange(this.BooleanParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DateParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.DateTimeParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.EnumerationParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.QuantityKindService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.TextParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
            results.AddRange(this.TimeOfDayParameterTypeService.GetDeep(transaction, partition, idFilter, containerSecurityContext));
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
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
