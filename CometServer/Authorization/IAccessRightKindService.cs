﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccessRightKindService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
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
    using CDP4Common.CommonData;

    /// <summary>
    /// The purpose of the <see cref="IAccessRightKindService"/> is to provide a service that returns the <see cref="PersonAccessRightKind"/>
    /// and <see cref="ParticipantAccessRightKind"/> for a specific type
    /// </summary>
    /// <remarks>
    /// the implementation should be a caching service and is therefore only valid per request
    /// </remarks>
    public interface IAccessRightKindService
    {
        /// <summary>
        /// Queries <see cref="PersonAccessRightKind"/> for the supplied object type.
        /// </summary>
        /// <param name="credentials">
        /// The <see cref="Credentials"/> that are valid for the HTTP request the <see cref="IAccessRightKindService"/> is valid for
        /// </param>
        /// <param name="typeName">
        /// The string representation of the typeName to compute permissions for.
        /// </param>
        /// <returns>
        /// <see cref="PersonAccessRightKind"/> for the supplied object type
        /// </returns>
        PersonAccessRightKind QueryPersonAccessRightKind(Credentials credentials, string typeName);

        /// <summary>
        /// Queries <see cref="ParticipantAccessRightKind"/> for the supplied object type.
        /// </summary>
        /// <param name="credentials">
        /// The <see cref="Credentials"/> that are valid for the HTTP request the <see cref="IAccessRightKindService"/> is valid for
        /// </param>
        /// <param name="typeName">
        /// The string representation of the typeName to compute permissions for.
        /// </param>
        /// <returns><see cref="ParticipantAccessRightKind"/> for the supplied object type.</returns>
        ParticipantAccessRightKind QueryParticipantAccessRightKind(Credentials credentials, string typeName);
    }
}