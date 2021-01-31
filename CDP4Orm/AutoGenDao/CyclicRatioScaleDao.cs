// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CyclicRatioScaleDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2020 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Kamil Wojnowski, 
//            Nathanael Smiechowski
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
// <summary>
//   This is an auto-generated Dao class. Any manual changes on this file will be overwritten.
// </summary>
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
    /// The CyclicRatioScale Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class CyclicRatioScaleDao : RatioScaleDao, ICyclicRatioScaleDao
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
        /// List of instances of <see cref="CDP4Common.DTO.CyclicRatioScale"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.CyclicRatioScale> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"CyclicRatioScale_Cache\"", partition);

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
                                yield return thing as CyclicRatioScale;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"CyclicRatioScale_View\"", partition);

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
        /// A deserialized instance of <see cref="CDP4Common.DTO.CyclicRatioScale"/>.
        /// </returns>
        public virtual CDP4Common.DTO.CyclicRatioScale MapToDto(NpgsqlDataReader reader)
        {
            string tempIsDeprecated;
            string tempIsMaximumInclusive;
            string tempIsMinimumInclusive;
            string tempMaximumPermissibleValue;
            string tempMinimumPermissibleValue;
            string tempModifiedOn;
            string tempModulus;
            string tempName;
            string tempNegativeValueConnotation;
            string tempNumberSet;
            string tempPositiveValueConnotation;
            string tempShortName;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.CyclicRatioScale(iid, revisionNumber);
            dto.Alias.AddRange(Array.ConvertAll((string[])reader["Alias"], Guid.Parse));
            dto.Definition.AddRange(Array.ConvertAll((string[])reader["Definition"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.HyperLink.AddRange(Array.ConvertAll((string[])reader["HyperLink"], Guid.Parse));
            dto.MappingToReferenceScale.AddRange(Array.ConvertAll((string[])reader["MappingToReferenceScale"], Guid.Parse));
            dto.Unit = Guid.Parse(reader["Unit"].ToString());
            dto.ValueDefinition.AddRange(Array.ConvertAll((string[])reader["ValueDefinition"], Guid.Parse));

            if (valueDict.TryGetValue("IsDeprecated", out tempIsDeprecated))
            {
                dto.IsDeprecated = bool.Parse(tempIsDeprecated);
            }

            if (valueDict.TryGetValue("IsMaximumInclusive", out tempIsMaximumInclusive))
            {
                dto.IsMaximumInclusive = bool.Parse(tempIsMaximumInclusive);
            }

            if (valueDict.TryGetValue("IsMinimumInclusive", out tempIsMinimumInclusive))
            {
                dto.IsMinimumInclusive = bool.Parse(tempIsMinimumInclusive);
            }

            if (valueDict.TryGetValue("MaximumPermissibleValue", out tempMaximumPermissibleValue) && tempMaximumPermissibleValue != null)
            {
                dto.MaximumPermissibleValue = tempMaximumPermissibleValue.UnEscape();
            }

            if (valueDict.TryGetValue("MinimumPermissibleValue", out tempMinimumPermissibleValue) && tempMinimumPermissibleValue != null)
            {
                dto.MinimumPermissibleValue = tempMinimumPermissibleValue.UnEscape();
            }

            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
            }

            if (valueDict.TryGetValue("Modulus", out tempModulus))
            {
                dto.Modulus = tempModulus.UnEscape();
            }

            if (valueDict.TryGetValue("Name", out tempName))
            {
                dto.Name = tempName.UnEscape();
            }

            if (valueDict.TryGetValue("NegativeValueConnotation", out tempNegativeValueConnotation) && tempNegativeValueConnotation != null)
            {
                dto.NegativeValueConnotation = tempNegativeValueConnotation.UnEscape();
            }

            if (valueDict.TryGetValue("NumberSet", out tempNumberSet))
            {
                dto.NumberSet = Utils.ParseEnum<CDP4Common.SiteDirectoryData.NumberSetKind>(tempNumberSet);
            }

            if (valueDict.TryGetValue("PositiveValueConnotation", out tempPositiveValueConnotation) && tempPositiveValueConnotation != null)
            {
                dto.PositiveValueConnotation = tempPositiveValueConnotation.UnEscape();
            }

            if (valueDict.TryGetValue("ShortName", out tempShortName))
            {
                dto.ShortName = tempShortName.UnEscape();
            }

            if (valueDict.TryGetValue("ThingPreference", out tempThingPreference) && tempThingPreference != null)
            {
                dto.ThingPreference = tempThingPreference.UnEscape();
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
        /// <param name="cyclicRatioScale">
        /// The cyclicRatioScale DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.CyclicRatioScale cyclicRatioScale, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, cyclicRatioScale, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, cyclicRatioScale, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "Modulus", !this.IsDerived(cyclicRatioScale, "Modulus") ? cyclicRatioScale.Modulus.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"CyclicRatioScale\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = cyclicRatioScale.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, cyclicRatioScale, container);
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
        /// <param name="cyclicRatioScale">
        /// The CyclicRatioScale DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.CyclicRatioScale cyclicRatioScale, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, cyclicRatioScale, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, cyclicRatioScale, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "Modulus", !this.IsDerived(cyclicRatioScale, "Modulus") ? cyclicRatioScale.Modulus.Escape() : string.Empty },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"CyclicRatioScale\"", partition);
                    sqlBuilder.AppendFormat(" SET \"ValueTypeDictionary\"");
                    sqlBuilder.AppendFormat(" = :valueTypeDictionary");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = cyclicRatioScale.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, cyclicRatioScale, container);
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
        /// The <see cref="CDP4Common.DTO.CyclicRatioScale"/> id that is to be deleted.
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
    }
}
