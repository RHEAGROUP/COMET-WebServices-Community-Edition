﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IICredentialsResolver.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
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

namespace CometServer.Authorization
{
    using System.Threading.Tasks;

    using Npgsql;

    /// <summary>
    /// The ICredentialsService interface.
    /// </summary>
    public interface ICredentialsService
    {
        /// <summary>
        /// Gets the resolved <see cref="ICredentials"/>
        /// </summary>
        public Credentials Credentials { get; }

        /// <summary>
        /// Resolves the username to <see cref="ICredentials"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="username">
        /// The supplied username
        /// </param>
        /// <returns>
        /// A <see cref="ICredentials"/> representing the resolved user, null if the user was not found.
        /// </returns>
        Task ResolveCredentials(NpgsqlTransaction transaction, string username);

        /// <summary>
        /// Resolve and set participant information for the current <see cref="Credentials"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="credentials">
        /// The supplied credential class which can hold participant information
        /// </param>
        void ResolveParticipantCredentials(NpgsqlTransaction transaction);
    }
}