﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileInitiator.cs" company="Starion Group S.A.">
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

namespace CometServer.Services
{
    using System;

    /// <summary>
    /// The initiator (person) of an exchange file export.
    /// This is included in the <see cref="ExchangeFileHeader"/>
    /// </summary>
    public class ExchangeFileInitiator
    {
        /// <summary>
        /// Gets or sets an optional unique identifier of the person who initiated the export.
        /// </summary>
        public Guid? Iid { get; set; }

        /// <summary>
        /// Gets or sets the given name of the person who initiated the export.
        /// </summary>
        public string GivenName { get; set; }

        /// <summary>
        /// Gets or sets the surname of the person who initiated the export.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the optional email address of the person who initiated the export.
        /// </summary>
        public string Email { get; set; }
    }
}
