﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileRevisionSideEffectTestFixture.cs" company="RHEA System S.A.">
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

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Security;

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="FileSideEffect"/>
    /// </summary>
    [TestFixture]
    public class FileRevisionSideEffectTestFixture
    {
        private Mock<IDomainFileStoreService> domainFileStoreService;
        private File file;
        private FileRevision fileRevision;
        private DomainFileStore fileStore;
        private FileRevisionSideEffect sideEffect;
        private Mock<IFileService> fileService;
        private NpgsqlTransaction npgsqlTransaction;

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.fileService = new Mock<IFileService>();
            this.fileRevision = new FileRevision(Guid.NewGuid(), 0);
            this.file = new File(Guid.NewGuid(), 0);
            
            this.fileStore = new DomainFileStore
                                 {
                                     Iid = Guid.NewGuid(),
                                     File =
                                     {
                                        this.file.Iid
                                     }
                                 };

            this.fileService = new Mock<IFileService>();

            this.domainFileStoreService = new Mock<IDomainFileStoreService>();

            this.sideEffect = new FileRevisionSideEffect
            {
                DomainFileStoreService = this.domainFileStoreService.Object,
                FileService = this.fileService.Object,
            };
        }
 
        [Test]
        public void VerifyThatBeforeDeleteCheckSecurityWorks()
        {
           this.sideEffect.BeforeDelete(this.fileRevision, this.file, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext());

            this.domainFileStoreService.Verify(x => x.HasWriteAccess(It.IsAny<File>(), null, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyThatBeforeUpdateCheckSecurityWorks()
        {
            this.sideEffect.BeforeUpdate(this.fileRevision, this.file, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext(), null);

            this.domainFileStoreService.Verify(x => x.HasWriteAccess(It.IsAny<File>(), null, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyThatBeforeCreateCheckSecurityWorks()
        {
            this.sideEffect.BeforeCreate(this.fileRevision, this.file, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext());

            this.domainFileStoreService.Verify(x => x.HasWriteAccess(It.IsAny<File>(), null, It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void VerifyThatAdditionalCheckSecurityChecksWork()
        {
            // Wrong container, should be a File
            Assert.Throws<IncompleteModelException>(() => this.sideEffect.BeforeUpdate(this.fileRevision, this.fileStore, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext(), null));

            //Locked by me
            this.sideEffect.BeforeUpdate(this.fileRevision, this.file, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext(), null);
            this.domainFileStoreService.Verify(x => x.HasWriteAccess(It.IsAny<File>(), null, It.IsAny<string>()), Times.Once);

            //Locked by someone else
            this.fileService.Setup(x => x.CheckFileLock(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), this.file)).Throws<SecurityException>();
            Assert.Throws<SecurityException>(() => this.sideEffect.BeforeUpdate(this.fileRevision, this.file, this.npgsqlTransaction, "Iteration_something", new RequestSecurityContext(), null));
        }
    }
}