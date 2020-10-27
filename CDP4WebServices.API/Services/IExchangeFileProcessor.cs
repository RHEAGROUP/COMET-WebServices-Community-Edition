// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExchangeFileProcessor.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using CDP4Common.DTO;
    using CDP4JsonSerializer;
    using CDP4Orm.Dao;
    using System.Collections.Generic;

    /// <summary>
    /// The ExchangeFileProcessor interface.
    /// </summary>
    public interface IExchangeFileProcessor
    {
        /// <summary>
        /// Get the site directory from file.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The site directory contained <see cref="Thing"/> collection.
        /// </returns>
        IEnumerable<Thing> ReadSiteDirectoryFromfile(string filePath, string password);

        /// <summary>
        /// Get the engineering model from file.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="engineeringModelSetup">
        /// The engineering model setup.
        /// </param>
        /// <returns>
        /// The deserialized engineering model contained <see cref="Thing"/> collection.
        /// </returns>
        IEnumerable<Thing> ReadEngineeringModelFromfile(string filePath, string password, EngineeringModelSetup engineeringModelSetup);

        /// <summary>
        /// Get the model iteration from file.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="iterationSetup">
        /// The iteration setup.
        /// </param>
        /// <returns>
        /// The deserialized iteration contained <see cref="Thing"/> collection.
        /// </returns>
        IEnumerable<Thing> ReadModelIterationFromFile(string filePath, string password, IterationSetup iterationSetup);

        /// <summary>
        /// Get user credentials from migration.json if exists
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="password">The password</param>
        /// <returns>The deserialized collection that contains hashed password and salt</returns>
        IList<MigrationPasswordCredentials> ReadMigrationJsonFromFile(string filePath, string password);

        /// <summary>
        /// Gets or sets the <see cref="ICdp4JsonSerializer"/>
        /// </summary>
        ICdp4JsonSerializer JsonSerializer { get; set; }

        /// <summary>
        /// Stores the referenced (by hash) file file binary contained in the archive.
        /// </summary>
        /// <param name="filePath">
        /// The file path of the zip archive being processed.
        /// </param>
        /// <param name="password">
        /// The archive password.
        /// </param>
        /// <param name="fileHash">
        /// The file hash of the file binary that will be stored on disk.
        /// </param>
        void StoreFileBinary(string filePath, string password, string fileHash);
    }
}
