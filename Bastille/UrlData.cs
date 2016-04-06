using System;
using System.Collections.Generic;
using System.Linq;

namespace Bastille
{
    public class UrlData
    {
        #region Public Methods

        /// <summary>
        /// Saves a user token and url
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="url"></param>
        /// <returns>True if a save was made and false otherwise</returns>
        public bool SaveUrl(string userToken, string url)
        {
            IsStringParameterValid("userToken", userToken);
            IsStringParameterValid("url", url);

            var saveSucessful = SaveToUserHash(userToken, url);

            if (saveSucessful)
            {
                SaveToDomainHash(userToken, url);
            }

            return saveSucessful;
        }

        /// <summary>
        /// Gets all the urls saved for a specified user
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns>IEnumerable<string> with all the urls for the specified user. Null if none exist.</returns>
        public IEnumerable<string> GetUrlsForUser(string userToken)
        {
            IsStringParameterValid("userToken", userToken);

            var urlsForUser = default(IEnumerable<string>);

            if (UserHash.ContainsKey(userToken))
            {
                urlsForUser = UserHash[userToken];
            }

            return urlsForUser;
        }

        /// <summary>
        /// Removes a url from a specific user's list
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="url"></param>
        /// <returns>True if a remove was performed, false otherwise</returns>
        public bool RemoveUrl(string userToken, string url)
        {
            IsStringParameterValid("userToken", userToken);
            IsStringParameterValid("url", url);

            var deleteSucessful = RemoveUrlFromUserHash(userToken, url);

            if (deleteSucessful)
            {
                RemoveUrlFromDomainHash(userToken, url);
            }

            return deleteSucessful;
        }

        /// <summary>
        /// Returns all the users who have saved a urls within a specified domain
        /// </summary>
        /// <param name="domain"></param>
        /// <returns>IEnumerable<string> with all the users who save a url in the specified domain</returns>
        public IEnumerable<string> GetUsersByDomain(string domain)
        {
            IsStringParameterValid("domain", domain);

            var usersByDomain = default(IEnumerable<string>);

            if (DomainHash.ContainsKey(domain))
            {
                usersByDomain = DomainHash[domain];
            }

            return usersByDomain;
        }

        /// <summary>
        /// Clears all the user tokens and urls from memory.
        /// </summary>
        public void ClearData()
        {
            if (_userHash != null)
            {
                _userHash.Clear();
                _userHash = null;
            }

            if (_domainHash != null)
            {
                _domainHash.Clear();
                _domainHash = null;
            }
        }

        #endregion Public Methods

        #region Private Fields

        /// <summary>
        /// Backing field for lazy loaded UserHash property
        /// </summary>
        private Dictionary<string, List<string>> _userHash;

        /// <summary>
        /// Backing field for lazy loaded DomainHash property
        /// </summary>
        private Dictionary<string, List<string>> _domainHash;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Stores all the urls saved for each user
        /// </summary>
        public Dictionary<string, List<string>> UserHash
        {
            get
            {
                if (_userHash == null)
                {
                    _userHash = new Dictionary<string, List<string>>();
                }

                return _userHash;
            }
        }

        /// <summary>
        /// Stores users who have saved a url within a domain
        /// </summary>
        public Dictionary<string, List<string>> DomainHash
        {
            get
            {
                if (_domainHash == null)
                {
                    _domainHash = new Dictionary<string, List<string>>();
                }

                return _domainHash;
            }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Adds a record to the UserHash if it does not already exist
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="url"></param>
        /// <returns>True if a record was added, false otherwise</returns>
        private bool SaveToUserHash(string userToken, string url)
        {
            var saveSucessful = false;

            if (UserHash.ContainsKey(userToken))
            {
                // user already exists in the hash, add url to the user's list
                var urlList = UserHash[userToken];

                if (!urlList.Contains(url))
                {
                    urlList.Add(url);

                    urlList = urlList.OrderBy(x => x).ToList();
                    saveSucessful = true;
                }
            }
            else
            {
                // user does not exist in the hash, add the user, init a new list, then add the url to the list
                var urlList = new List<string>();

                urlList.Add(url);
                UserHash.Add(userToken, urlList);
                saveSucessful = true;
            }

            return saveSucessful;
        }

        /// <summary>
        /// Adds a record to the DomainHash if it does not already exist
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="url"></param>
        private void SaveToDomainHash(string userToken, string url)
        {
            var domain = GetDomain(url);

            if (DomainHash.ContainsKey(domain))
            {
                var userList = DomainHash[domain];

                if (!userList.Contains(userToken))
                {
                    userList.Add(userToken);
                    userList = userList.OrderBy(x => x).ToList();
                }
            }
            else
            {
                var userList = new List<string>();

                userList.Add(userToken);
                DomainHash.Add(domain, userList);
            }
        }

        /// <summary>
        /// Removes a record from the UserHash if it exists
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="url"></param>
        /// <returns>True if a record was removed, false otherwise</returns>
        private bool RemoveUrlFromUserHash(string userToken, string url)
        {
            var deleteSucessful = false;

            if (UserHash.ContainsKey(userToken))
            {
                var urlList = UserHash[userToken];

                if (urlList.Contains(url))
                {
                    urlList.Remove(url);
                    deleteSucessful = true;

                    if (urlList.Count == 0)
                    {
                        UserHash.Remove(userToken);
                    }
                }
            }

            return deleteSucessful;
        }

        /// <summary>
        /// Removes a user from a domains list if that user does not have any saved urls in that domain
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="url"></param>
        private void RemoveUrlFromDomainHash(string userToken, string url)
        {
            var domain = GetDomain(url);
            var domainUserList = DomainHash[domain];
            var userUrlList = UserHash.ContainsKey(userToken) ? UserHash[userToken] : null;

            if (userUrlList == null || userUrlList.Count == 0 || !userUrlList.Any(x => x.Contains(domain)))
            {
                domainUserList.Remove(userToken);
            }
        }

        /// <summary>
        /// Utility method for validating string parameters
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        private void IsStringParameterValid(string parameterName, string parameterValue)
        {
            if (string.IsNullOrEmpty(parameterValue))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Returns the domain given a valid URL. Must be a full valid URL or it will fail.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetDomain(string url)
        {
            var uri = default(Uri);

            try
            {
                uri =  new Uri(url);
            }
            catch (Exception exception)
            {
                throw new ArgumentException(string.Format("'{0}' is not a valid URL format.", url), exception);
            }

            return uri.Host;
        }

        #endregion Private Methods
    }
}
