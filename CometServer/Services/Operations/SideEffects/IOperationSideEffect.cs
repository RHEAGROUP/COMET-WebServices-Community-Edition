// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationSideEffect.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Geren�, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Th�ate
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
    /// <summary>
    /// The purpose of the OperationSideEffect interface is to provide a contract to register SideEffect classes.
    /// </summary>
    public interface IOperationSideEffect : IOperationSideEffectFunctions
    {
        /// <summary>
        /// Gets the type name of this generically typed <see cref="OperationSideEffect{T}"/>.
        /// </summary>
        /// <returns>
        /// The type name.
        /// </returns>
        /// <remarks>
        /// The result is used to register this instance in the <see cref="OperationSideEffectProcessor"/> map.
        /// </remarks>
        string RegistryKey { get; }
    }
}