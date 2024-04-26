﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArchitectureDiagramSideEffect.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Services.Operations.SideEffects
{
    using System.Security;

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Services.Authorization;
    using CometServer.Services.Supplemental;

    using Npgsql;

    public class ArchitectureDiagramSideEffect : OperationSideEffect<ArchitectureDiagram>
    {
        /// <summary>
        /// Gets or sets the <see cref="IDiagramCanvasBusinessRuleService"/>.
        /// </summary>
        public IDiagramCanvasBusinessRuleService DiagramCanvasBusinessRuleService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="ArchitectureDiagram"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The raw update info that was serialized from the user posted request.
        /// The <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be changed likely as it can/will change the operation processor outcome.
        /// </param>
        public override void BeforeUpdate(ArchitectureDiagram thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            base.BeforeUpdate(thing, container, transaction, partition, securityContext, rawUpdateInfo);
            this.HasWriteAccess(thing, transaction, partition);
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="ArchitectureDiagram"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override void BeforeDelete(ArchitectureDiagram thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            base.BeforeDelete(thing, container, transaction, partition, securityContext);
            this.HasWriteAccess(thing, transaction, partition);
        }

        /// <summary>
        /// Checks the <see cref="ArchitectureDiagram"/> security
        /// </summary>
        /// <param name="thing">
        /// The instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        public void HasWriteAccess(ArchitectureDiagram thing, NpgsqlTransaction transaction, string partition)
        {
            if (!this.DiagramCanvasBusinessRuleService.IsWriteAllowed(
                    transaction,
                    thing,
                    partition))
            {
                throw new SecurityException($"User is not allowed to write to {nameof(ArchitectureDiagram)} '{thing.Name}'");
            }
        }
    }
}
