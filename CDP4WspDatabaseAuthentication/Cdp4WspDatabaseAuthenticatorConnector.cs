﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4WSPDatabaseAuthenticatorConnector.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WspDatabaseAuthentication
{
    using CDP4Authentication;
    using CDP4Authentication.Contracts;

    /// <summary>
    /// A connector for WSP basic authentication against a CDP4 database.
    /// </summary>
    public class Cdp4WspDatabaseAuthenticatorConnector : AuthenticatorConnector<AuthenticatorWspProperties>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cdp4WspDatabaseAuthenticatorConnector"/> class.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        public Cdp4WspDatabaseAuthenticatorConnector(AuthenticatorWspProperties properties)
            : base(properties)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the connector is up.
        /// </summary>
        public override bool IsUp
        {
            get
            {
                this.IsDown = false;
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the connector is down.
        /// </summary>
        public override bool IsDown { get; set; }

        /// <summary>
        /// Gets or sets the status message of the <see cref="IAuthenticatorConnector"/>.
        /// </summary>
        public override string StatusMessage { get; set; }

        /// <summary>
        /// Gets the <see cref="IAuthenticatorConnector"/> name for display purposes.
        /// </summary>
        public override string Name
        {
            get { return "CDP4 Wsp Database Authenticator"; }
        }

        /// <summary>
        /// Authenticate the <see cref="AuthenticationPerson"/> information against the supplied password.
        /// </summary>
        /// <param name="person">
        /// The <see cref="AuthenticationPerson"/> information to authenticate.
        /// </param>
        /// <param name="password">
        /// The password to authenticate against.
        /// </param>
        /// <returns>
        /// True if the <see cref="AuthenticationPerson"/> could be authenticated.
        /// </returns>
        public override bool Authenticate(AuthenticationPerson person, string password)
        {
            return this.ValidatePassword(password, person);
        }

        /// <summary>
        /// Verifies that the password that the login has provided is correct.
        /// </summary>
        /// <param name="password">
        /// The input password to test.
        /// </param>
        /// <param name="person">
        /// The <see cref="AuthenticationPerson"/> that this password should authenticate against.
        /// </param>
        /// <returns>
        /// True if the passwords match.
        /// </returns>
        private bool ValidatePassword(string password, AuthenticationPerson person)
        {
            var result = false;

            var serverSalts = (this.Properties as IAuthenticatorWspProperties)?.ServerSalts;

            if (serverSalts == null)
            {
                return false;
            }

            foreach (var serverSalt in serverSalts)
            {
                result = EncryptionUtils.CompareWspSaltedString(password, person.Password, person.Salt, serverSalt);

                if (result)
                {
                    break;
                }
            }

            return result;
        }
    }
}
