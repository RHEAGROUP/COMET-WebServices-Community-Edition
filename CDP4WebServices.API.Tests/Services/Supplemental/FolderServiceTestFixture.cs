﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderServiceTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
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

namespace CDP4WebServices.API.Tests.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using API.Services;
    using API.Services.Authorization;

    using CDP4Authentication;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services.Authentication;

    using Moq;
    
    using Npgsql;
    
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="FolderService"/>
    /// </summary>
    [TestFixture]
    public class FolderServiceTestFixture
    {
        private Folder folder;
        private IFolderService folderService;
        private Mock<IDomainFileStoreService> domainFileStoreService;
        private Mock<IPermissionService> permissionService;
        private Mock<IFolderDao> folderDao;
        private NpgsqlTransaction transaction;
        private Mock<ICdp4TransactionManager> transactionManager;
        private string iterationPartitionName;
        private Person person;

        [SetUp]
        public void Setup()
        {
            this.folder = new Folder(Guid.NewGuid(), 0);
            this.folderDao = new Mock<IFolderDao>();
            this.permissionService = new Mock<IPermissionService>();
            this.domainFileStoreService = new Mock<IDomainFileStoreService>();
            this.transaction = null;
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.person = new Person(Guid.NewGuid(), 0);

            this.folderService = new FolderService
            {
                PermissionService = this.permissionService.Object,
                FolderDao = this.folderDao.Object,
                TransactionManager = this.transactionManager.Object,
                DomainFileStoreService = this.domainFileStoreService.Object
            };

            this.iterationPartitionName = "Iteration_" + Guid.NewGuid();

            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), this.folder)).Returns(true);

            this.permissionService.Setup(x => x.Credentials)
                .Returns(
                    new Credentials
                    {
                        Person = new AuthenticationPerson(this.person.Iid, 0)
                        {
                            UserName = "TestRunner"
                        }
                    });

            this.folderDao
                .Setup(
                    x => x.Read(It.IsAny<NpgsqlTransaction>(), this.iterationPartitionName, It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>()))
                .Returns(new[] { this.folder });

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(true);
        }

        [Test]
        public void VerifyContainerIsInstanceReadAllowed()
        {
            this.domainFileStoreService.Setup(x => x.HasReadAccess(It.IsAny<Thing>(),It.IsAny<IDbTransaction>(), this.iterationPartitionName)).Returns(true);

            Assert.That(this.folderService.IsAllowedAccordingToIsHidden(this.transaction, this.folder, this.iterationPartitionName), Is.True);

            this.domainFileStoreService.Setup(x => x.HasReadAccess(It.IsAny<Thing>(),It.IsAny<IDbTransaction>(), this.iterationPartitionName)).Returns(false);

            Assert.That(this.folderService.IsAllowedAccordingToIsHidden(this.transaction, this.folder, this.iterationPartitionName), Is.False);
        }
    }
}
