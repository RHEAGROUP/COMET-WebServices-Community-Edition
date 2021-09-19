﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileInitiator.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CDP4WebServices.API.Services
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
