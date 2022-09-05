﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagramPermissionService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
// 
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski,
//                Simon Wood
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

namespace CDP4WebServices.API.Services.Authorization
{
    using CDP4Common.CommonData;
    using CDP4Common.DiagramData;
    using CDP4Common.DTO;

    /// <summary>
    /// Permission services of diagrams. Extra layer for normal permissions.
    /// </summary>
    public class DiagramPermissionService : IDiagramPermissionService
    {
        /// <summary>
        /// Check whether a canvas is readable by certain OWNER based on publication state
        /// </summary>
        /// <param name="canvas">The canvas</param>
        /// <param name="publicationState">The current state</param>
        /// <param name="accessKind">The actual accesskind of this class</param>
        /// <returns>True if readable</returns>
        public bool CanReadOwnedDiagram(IOwnedThing canvas, PublicationState publicationState, ParticipantAccessRightKind accessKind)
        {
            return true;

            // TODO: GH #917 the folowing code causes severe desync with the several clients. Needs to be discussed. For now all publication state READ restrictions
            // based on the publication state moved to client
            // assumes you are NOT the owner

            // return !((publicationState == PublicationState.Hidden || publicationState == PublicationState.ReadyForPublish) && (accessKind == ParticipantAccessRightKind.READ || accessKind == ParticipantAccessRightKind.MODIFY_IF_OWNER));
        }
    }
}
