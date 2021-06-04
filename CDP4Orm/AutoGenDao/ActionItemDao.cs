// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionItemDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
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

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;

    using Npgsql;
    using NpgsqlTypes;

    /// <summary>
    /// The ActionItem Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class ActionItemDao : ModellingAnnotationItemDao, IActionItemDao
    {
        /// <summary>
        /// Read the data from the database.
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
        /// <param name="isCachedDtoReadEnabledAndInstant">
        /// The value indicating whether to get cached last state of Dto from revision history.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="CDP4Common.DTO.ActionItem"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.ActionItem> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"ActionItem_Cache\"", partition);

                    if (ids != null && ids.Any())
                    {
                        sqlBuilder.Append(" WHERE \"Iid\" = ANY(:ids)");
                        command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids;
                    }

                    sqlBuilder.Append(";");

                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    command.CommandText = sqlBuilder.ToString();

                    // log the sql command 
                    this.LogCommand(command);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var thing = this.MapJsonbToDto(reader);
                            if (thing != null)
                            {
                                yield return thing as ActionItem;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"ActionItem_View\"", partition);

                    if (ids != null && ids.Any()) 
                    {
                        sqlBuilder.Append(" WHERE \"Iid\" = ANY(:ids)");
                        command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids;
                    }

                    sqlBuilder.Append(";");

                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    command.CommandText = sqlBuilder.ToString();

                    // log the sql command 
                    this.LogCommand(command);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return this.MapToDto(reader);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The mapping from a database record to data transfer object.
        /// </summary>
        /// <param name="reader">
        /// An instance of the SQL reader.
        /// </param>
        /// <returns>
        /// A deserialized instance of <see cref="CDP4Common.DTO.ActionItem"/>.
        /// </returns>
        public virtual CDP4Common.DTO.ActionItem MapToDto(NpgsqlDataReader reader)
        {
            string tempClassification;
            string tempCloseOutDate;
            string tempCloseOutStatement;
            string tempContent;
            string tempCreatedOn;
            string tempDueDate;
            string tempLanguageCode;
            string tempModifiedOn;
            string tempShortName;
            string tempStatus;
            string tempThingPreference;
            string tempTitle;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.ActionItem(iid, revisionNumber);
            dto.Actionee = Guid.Parse(reader["Actionee"].ToString());
            dto.ApprovedBy.AddRange(Array.ConvertAll((string[])reader["ApprovedBy"], Guid.Parse));
            dto.Author = Guid.Parse(reader["Author"].ToString());
            dto.Category.AddRange(Array.ConvertAll((string[])reader["Category"], Guid.Parse));
            dto.Discussion.AddRange(Array.ConvertAll((string[])reader["Discussion"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.Owner = Guid.Parse(reader["Owner"].ToString());
            dto.PrimaryAnnotatedThing = reader["PrimaryAnnotatedThing"] is DBNull ? (Guid?)null : Guid.Parse(reader["PrimaryAnnotatedThing"].ToString());
            dto.RelatedThing.AddRange(Array.ConvertAll((string[])reader["RelatedThing"], Guid.Parse));
            dto.SourceAnnotation.AddRange(Array.ConvertAll((string[])reader["SourceAnnotation"], Guid.Parse));

            if (valueDict.TryGetValue("Classification", out tempClassification))
            {
                dto.Classification = Utils.ParseEnum<CDP4Common.ReportingData.AnnotationClassificationKind>(tempClassification);
            }

            if (valueDict.TryGetValue("CloseOutDate", out tempCloseOutDate) && tempCloseOutDate != null)
            {
                dto.CloseOutDate = Utils.ParseUtcDate(tempCloseOutDate);
            }

            if (valueDict.TryGetValue("CloseOutStatement", out tempCloseOutStatement) && tempCloseOutStatement != null)
            {
                dto.CloseOutStatement = tempCloseOutStatement.UnEscape();
            }

            if (valueDict.TryGetValue("Content", out tempContent))
            {
                dto.Content = tempContent.UnEscape();
            }

            if (valueDict.TryGetValue("CreatedOn", out tempCreatedOn))
            {
                dto.CreatedOn = Utils.ParseUtcDate(tempCreatedOn);
            }

            if (valueDict.TryGetValue("DueDate", out tempDueDate))
            {
                dto.DueDate = Utils.ParseUtcDate(tempDueDate);
            }

            if (valueDict.TryGetValue("LanguageCode", out tempLanguageCode))
            {
                dto.LanguageCode = tempLanguageCode.UnEscape();
            }

            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
            }

            if (valueDict.TryGetValue("ShortName", out tempShortName))
            {
                dto.ShortName = tempShortName.UnEscape();
            }

            if (valueDict.TryGetValue("Status", out tempStatus))
            {
                dto.Status = Utils.ParseEnum<CDP4Common.ReportingData.AnnotationStatusKind>(tempStatus);
            }

            if (valueDict.TryGetValue("ThingPreference", out tempThingPreference) && tempThingPreference != null)
            {
                dto.ThingPreference = tempThingPreference.UnEscape();
            }

            if (valueDict.TryGetValue("Title", out tempTitle))
            {
                dto.Title = tempTitle.UnEscape();
            }

            return dto;
        }

        /// <summary>
        /// Insert a new database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="actionItem">
        /// The actionItem DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ActionItem actionItem, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, actionItem, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, actionItem, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "CloseOutDate", !this.IsDerived(actionItem, "CloseOutDate") && actionItem.CloseOutDate.HasValue ? actionItem.CloseOutDate.Value.ToString(Utils.DateTimeUtcSerializationFormat) : null },
                    { "CloseOutStatement", !this.IsDerived(actionItem, "CloseOutStatement") ? actionItem.CloseOutStatement.Escape() : null },
                    { "DueDate", !this.IsDerived(actionItem, "DueDate") ? actionItem.DueDate.ToString(Utils.DateTimeUtcSerializationFormat) : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActionItem\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Actionee\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :actionee);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = actionItem.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("actionee", NpgsqlDbType.Uuid).Value = !this.IsDerived(actionItem, "Actionee") ? actionItem.Actionee : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, actionItem, container);
        }

        /// <summary>
        /// Insert a new database record, or updates one if it already exists from the supplied data transfer object.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="actionItem">
        /// The actionItem DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ActionItem actionItem, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, actionItem, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "CloseOutDate", !this.IsDerived(actionItem, "CloseOutDate") && actionItem.CloseOutDate.HasValue ? actionItem.CloseOutDate.Value.ToString(Utils.DateTimeUtcSerializationFormat) : null },
                { "CloseOutStatement", !this.IsDerived(actionItem, "CloseOutStatement") ? actionItem.CloseOutStatement.Escape() : null },
                { "DueDate", !this.IsDerived(actionItem, "DueDate") ? actionItem.DueDate.ToString(Utils.DateTimeUtcSerializationFormat) : string.Empty },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                    
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActionItem\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Actionee\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :actionee)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = actionItem.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("actionee", NpgsqlDbType.Uuid).Value = !this.IsDerived(actionItem, "Actionee") ? actionItem.Actionee : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"Actionee\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :actionee);");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }

            return true;
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
        /// <param name="actionItem">
        /// The ActionItem DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.ActionItem actionItem, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, actionItem, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, actionItem, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "CloseOutDate", !this.IsDerived(actionItem, "CloseOutDate") && actionItem.CloseOutDate.HasValue ? actionItem.CloseOutDate.Value.ToString(Utils.DateTimeUtcSerializationFormat) : null },
                    { "CloseOutStatement", !this.IsDerived(actionItem, "CloseOutStatement") ? actionItem.CloseOutStatement.Escape() : null },
                    { "DueDate", !this.IsDerived(actionItem, "DueDate") ? actionItem.DueDate.ToString(Utils.DateTimeUtcSerializationFormat) : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActionItem\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Actionee\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :actionee)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = actionItem.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("actionee", NpgsqlDbType.Uuid).Value = !this.IsDerived(actionItem, "Actionee") ? actionItem.Actionee : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, actionItem, container);
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
        /// The <see cref="CDP4Common.DTO.ActionItem"/> id that is to be deleted.
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
        /// Delete a database record from the supplied data transfer object.
        /// A "Raw" Delete means that the delete is performed without calling BeforeDelete or AfterDelete.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.ActionItem"/> id that is to be deleted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully deleted.
        /// </returns>
        public override bool RawDelete(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            var result = false;

            result = base.Delete(transaction, partition, iid);
            return result;
        }
    }
}
