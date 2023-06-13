﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryParameters.cs" company="RHEA System S.A.">
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

namespace CDP4WebServices.API.Services.Protocol
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    /// <summary>
    /// The Query Parameters interface.
    /// </summary>
    public interface IQueryParameters
    {
        /// <summary>
        /// Gets or sets a value indicating whether to extent deep.
        /// </summary>
        bool ExtentDeep { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include reference data.
        /// </summary>
        bool IncludeReferenceData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include all containers.
        /// </summary>
        bool IncludeAllContainers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include file data.
        /// </summary>
        bool IncludeFileData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to export data.
        /// </summary>
        bool Export { get; set; }

        /// <summary>
        /// Gets or sets the revision number.
        /// </summary>
        int RevisionNumber { get; set; }

        /// <summary>
        /// Gets or sets the revision number, or DateTime from which the request is done
        /// </summary>
        object RevisionFrom { get; set; }

        /// <summary>
        /// Gets or sets the revision number, or DateTime to which the request is done
        /// </summary>
        object RevisionTo { get; set; }

        /// <summary>
        /// Gets or sets a collection of <see cref="ClassKind"/> to used during the cherry picking request
        /// </summary>
        IEnumerable<ClassKind> ClassKinds { get; set; }

        /// <summary>
        /// Gets or sets a collection of <see cref="Category"/>s id to used during the cherry picking request
        /// </summary>
        IEnumerable<Guid> CategoriesId { get; set; }

        /// <summary>
        /// The validate query parameter.
        /// </summary>
        /// <param name="queryParameter">
        /// The query parameter.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="Exception">
        /// If unknown query parameter or value is passed
        /// </exception>
        void ValidateQueryParameter(string queryParameter, string value);
    }
}
