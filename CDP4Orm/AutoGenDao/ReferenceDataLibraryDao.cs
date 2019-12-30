// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceDataLibraryDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2019 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft.
//
//    This file is part of CDP4 Web Services Community Edition. 
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;

    using Npgsql;
    using NpgsqlTypes;

    /// <summary>
    /// The abstract ReferenceDataLibrary Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public abstract partial class ReferenceDataLibraryDao : DefinedThingDao
    {

        /// <summary>
        /// Insert a new database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="referenceDataLibrary">
        /// The referenceDataLibrary DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ReferenceDataLibrary referenceDataLibrary, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, referenceDataLibrary, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, referenceDataLibrary, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ReferenceDataLibrary\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"RequiredRdl\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :requiredRdl);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = referenceDataLibrary.Iid;
                    command.Parameters.Add("requiredRdl", NpgsqlDbType.Uuid).Value = !this.IsDerived(referenceDataLibrary, "RequiredRdl") ? Utils.NullableValue(referenceDataLibrary.RequiredRdl) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
                referenceDataLibrary.BaseQuantityKind.ForEach(x => this.AddBaseQuantityKind(transaction, partition, referenceDataLibrary.Iid, x));
                referenceDataLibrary.BaseUnit.ForEach(x => this.AddBaseUnit(transaction, partition, referenceDataLibrary.Iid, x));
            }

            return this.AfterWrite(beforeWrite, transaction, partition, referenceDataLibrary, container);
        }

        /// <summary>
        /// Add the supplied value collection to the association link table indicated by the supplied property name
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
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="value">
        /// A value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public override bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            var isCreated = base.AddToCollectionProperty(transaction, partition, propertyName, iid, value);

            switch (propertyName)
            {
                case "BaseQuantityKind":
                    {
                        isCreated = this.AddBaseQuantityKind(transaction, partition, iid, (CDP4Common.Types.OrderedItem)value);
                        break;
                    }

                case "BaseUnit":
                    {
                        isCreated = this.AddBaseUnit(transaction, partition, iid, (Guid)value);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            return isCreated;
        }

        /// <summary>
        /// Insert a new association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="baseQuantityKind">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddBaseQuantityKind(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem baseQuantityKind)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ReferenceDataLibrary_BaseQuantityKind\"", partition);
                sqlBuilder.AppendFormat(" (\"ReferenceDataLibrary\", \"BaseQuantityKind\", \"Sequence\")");
                sqlBuilder.Append(" VALUES (:referenceDataLibrary, :baseQuantityKind, :sequence);");

                command.Parameters.Add("referenceDataLibrary", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("baseQuantityKind", NpgsqlDbType.Uuid).Value = baseQuantityKind.V;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = baseQuantityKind.K;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }
        /// <summary>
        /// Insert a new association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that will be the source for each link table record.
        /// </param> 
        /// <param name="baseUnit">
        /// The value for which a link table record wil be created.
        /// </param>
        /// <returns>
        /// True if the value link was successfully created.
        /// </returns>
        public bool AddBaseUnit(NpgsqlTransaction transaction, string partition, Guid iid, Guid baseUnit)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ReferenceDataLibrary_BaseUnit\"", partition);
                sqlBuilder.AppendFormat(" (\"ReferenceDataLibrary\", \"BaseUnit\")");
                sqlBuilder.Append(" VALUES (:referenceDataLibrary, :baseUnit);");

                command.Parameters.Add("referenceDataLibrary", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("baseUnit", NpgsqlDbType.Uuid).Value = baseUnit;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Update a database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="referenceDataLibrary">
        /// The ReferenceDataLibrary DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ReferenceDataLibrary referenceDataLibrary, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, referenceDataLibrary, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, referenceDataLibrary, container);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ReferenceDataLibrary\"", partition);
                    sqlBuilder.AppendFormat(" SET \"RequiredRdl\"");
                    sqlBuilder.AppendFormat(" = :requiredRdl");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = referenceDataLibrary.Iid;
                    command.Parameters.Add("requiredRdl", NpgsqlDbType.Uuid).Value = !this.IsDerived(referenceDataLibrary, "RequiredRdl") ? Utils.NullableValue(referenceDataLibrary.RequiredRdl) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, referenceDataLibrary, container);
        }

        /// <summary>
        /// Reorder the supplied value collection of the association link table indicated by the supplied property name
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
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that is the source for the reordered link table record.
        /// </param> 
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// True if the value link was successfully reordered.
        /// </returns>
        public override bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            var isReordered = base.ReorderCollectionProperty(transaction, partition, propertyName, iid, orderUpdate);
 
            switch (propertyName)
            {
                case "BaseQuantityKind":
                    {
                        isReordered = this.ReorderBaseQuantityKind(transaction, partition, iid, orderUpdate);
                        break;
                    }

                default:
                {
                    break;
                }
            }

            return isReordered;
      }

        /// <summary>
        /// Reorder an item in an association link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be reordered.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that will be the source for reordered link table record.
        /// </param> 
        /// <param name="baseQuantityKind">
        /// The value for which a link table record wil be reordered.
        /// </param>
        /// <returns>
        /// True if the value link was successfully reordered.
        /// </returns>
        public bool ReorderBaseQuantityKind(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem baseQuantityKind)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ReferenceDataLibrary_BaseQuantityKind\"", partition);
                sqlBuilder.AppendFormat(" SET \"Sequence\"");
                sqlBuilder.Append(" = :reorderSequence");
                sqlBuilder.Append(" WHERE \"ReferenceDataLibrary\" = :referenceDataLibrary");
                sqlBuilder.Append(" AND \"BaseQuantityKind\" = :baseQuantityKind");
                sqlBuilder.Append(" AND \"Sequence\" = :sequence;");

                command.Parameters.Add("referenceDataLibrary", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("baseQuantityKind", NpgsqlDbType.Uuid).Value = baseQuantityKind.V;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = baseQuantityKind.K;
                command.Parameters.Add("reorderSequence", NpgsqlDbType.Bigint).Value = baseQuantityKind.M;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Delete a database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that is to be deleted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully deleted.
        /// </returns>
        public override bool Delete(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            bool isHandled;
            var beforeDelete = this.BeforeDelete(transaction, partition, iid, out isHandled);
            if (!isHandled)
            {
                beforeDelete = beforeDelete && base.Delete(transaction, partition, iid);
            }

            return this.AfterDelete(beforeDelete, transaction, partition, iid);
        }

        /// <summary>
        /// Delete the supplied value from the association link table indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be removed.
        /// </param>
        /// <param name="propertyName">
        /// The association property name from where the value is to be removed.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that is the source of each link table record.
        /// </param> 
        /// <param name="value">
        /// A value for which a link table record wil be removed.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public override bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            var isDeleted = base.DeleteFromCollectionProperty(transaction, partition, propertyName, iid, value);

            switch (propertyName)
            {
                case "BaseQuantityKind":
                    {
                        isDeleted = this.DeleteBaseQuantityKind(transaction, partition, iid, (CDP4Common.Types.OrderedItem)value);
                        break;
                    }

                case "BaseUnit":
                    {
                        isDeleted = this.DeleteBaseUnit(transaction, partition, iid, (Guid)value);
                        break;
                    }

                default:
                {
                    break;
                }
            }

            return isDeleted;
        }

        /// <summary>
        /// Delete an association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="baseQuantityKind">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteBaseQuantityKind(NpgsqlTransaction transaction, string partition, Guid iid, CDP4Common.Types.OrderedItem baseQuantityKind)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"ReferenceDataLibrary_BaseQuantityKind\"", partition);
                sqlBuilder.Append(" WHERE \"ReferenceDataLibrary\" = :referenceDataLibrary");
                sqlBuilder.Append(" AND \"BaseQuantityKind\" = :baseQuantityKind");
                sqlBuilder.Append(" AND \"Sequence\" = :sequence;");

                command.Parameters.Add("referenceDataLibrary", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("baseQuantityKind", NpgsqlDbType.Uuid).Value = baseQuantityKind.V;
                command.Parameters.Add("sequence", NpgsqlDbType.Bigint).Value = baseQuantityKind.K;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }

        /// <summary>
        /// Delete an association record in the link table.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ReferenceDataLibrary"/> id that is the source for each link table record.
        /// </param> 
        /// <param name="baseUnit">
        /// A value for which a link table record wil be deleted.
        /// </param>
        /// <returns>
        /// True if the value link was successfully removed.
        /// </returns>
        public bool DeleteBaseUnit(NpgsqlTransaction transaction, string partition, Guid iid, Guid baseUnit)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"ReferenceDataLibrary_BaseUnit\"", partition);
                sqlBuilder.Append(" WHERE \"ReferenceDataLibrary\" = :referenceDataLibrary");
                sqlBuilder.Append(" AND \"BaseUnit\" = :baseUnit;");

                command.Parameters.Add("referenceDataLibrary", NpgsqlDbType.Uuid).Value = iid;
                command.Parameters.Add("baseUnit", NpgsqlDbType.Uuid).Value = baseUnit;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                return this.ExecuteAndLogCommand(command) > 0;
            }
        }
    }
}
