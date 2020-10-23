﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersonDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;

    using Npgsql;

    /// <summary>
    /// The Person dao Interface.
    /// </summary>
    public partial interface IPersonDao
    {
        /// <summary>
        /// Gets the password change token that is only valid in the context of this class instance lifetime.
        /// </summary>
        string PasswordChangeToken { get; }

        /// <summary>
        /// Gets the given name of a person.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="personIid">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GivenName(NpgsqlTransaction transaction, Guid personIid);

        /// <summary>
        /// Update user credentials after migration
        /// </summary>
        /// <param name="transaction">The database transaction.</param>
        /// <param name="partition">The database schema</param>
        /// <param name="person">The person <see cref="CDP4Common.DTO.Person" /></param>
        /// <param name="credentials">The new credentials from migration.json <see cref="MigrationPasswordCredentials" /></param>
        /// <returns></returns>
        bool UpdateCredentials(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Person person, MigrationPasswordCredentials credentials);
    }
}
