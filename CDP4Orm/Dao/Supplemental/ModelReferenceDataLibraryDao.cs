﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelReferenceDataLibraryDao.cs" company="RHEA System S.A.">
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
    /// The ModelReferenceDataLibrary Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class ModelReferenceDataLibraryDao
    {
        /// <summary>
        /// GEts the identifiers of the chain of <see cref="SiteReferenceDataLibrary"/> dependencies for <see cref="EngineeringModelSetup"/>s
        /// </summary>
        /// <param name="modelSetups">The <see cref="EngineeringModelSetup"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <returns>The identifiers of the <see cref="SiteReferenceDataLibrary"/> dependency</returns>
        public IEnumerable<Guid> GetSiteReferenceDataLibraryDependency(IEnumerable<EngineeringModelSetup> modelSetups, NpgsqlTransaction transaction)
        {
            using (var command = new NpgsqlCommand())
            {
                var sql = $@"
WITH RECURSIVE get_chain AS (
    SELECT * FROM ""SiteDirectory"".""SiteReferenceDataLibrary_View"" WHERE ""Iid"" IN (SELECT ""RequiredRdl"" FROM ""SiteDirectory"".""ModelReferenceDataLibrary_View"" WHERE ""Iid"" = ANY(:modelRdls))
    UNION
    SELECT ""SiteDirectory"".""SiteReferenceDataLibrary_View"".*
        FROM ""SiteDirectory"".""SiteReferenceDataLibrary_View""
        JOIN get_chain on get_chain.""RequiredRdl"" = ""SiteDirectory"".""SiteReferenceDataLibrary_View"".""Iid""
)
SELECT * FROM get_chain";

                command.Parameters.Add("modelRdls", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = modelSetups.Select(x => x.RequiredRdl.First()).ToArray();

                command.CommandText = sql;
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                // log the sql command 
                this.LogCommand(command);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return Guid.Parse(reader.GetValue(0).ToString());
                    }
                }
            }
        }
    }
}
