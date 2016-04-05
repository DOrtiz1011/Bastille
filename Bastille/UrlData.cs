using System;
using System.Collections.Generic;
using System.Linq;

namespace Bastille
{
    public class UrlData
    {
        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool SaveUrl(string userToken, string url)
        {
            //if (string.IsNullOrEmpty(userToken))
            //{
            //    throw new ArgumentNullException("userToken");
            //}
            //if (string.IsNullOrEmpty(url))
            //{
            //    throw new ArgumentNullException("url");
            //}
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
        /// 
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="url"></param>
        /// <returns></returns>
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

        public IEnumerable<string> GetUsersByDomain(string domain)
        {
            IsStringParameterValid("domain", domain);

            var usersByDomain = default(IEnumerable<string>);

            return usersByDomain;
        }

        #endregion Public Methods

        #region Private Fields

        private Dictionary<string, List<string>> _userHash;
        private Dictionary<string, List<string>> _domainHash;

        #endregion Private Fields

        #region Properties

        private Dictionary<string, List<string>> UserHash
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

        private Dictionary<string, List<string>> DomainHash
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

        #endregion Properties

        #region Private Methods

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

        private void SaveToDomainHash(string userToken, string url)
        {
            var domain = GetDomain(url);

            if (DomainHash.ContainsKey(domain))
            {
                // user already exists in the hash, add url to the user's list
                var userList = DomainHash[domain];

                if (!userList.Contains(userToken))
                {
                    userList.Add(userToken);

                    userList = userList.OrderBy(x => x).ToList();
                }
            }
            else
            {
                // user does not exist in the hash, add the user, init a new list, then add the url to the list
                var userList = new List<string>();

                userList.Add(userToken);
                DomainHash.Add(domain, userList);
            }
        }

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

        private void IsStringParameterValid(string parameterName, string parameterValue)
        {
            if (string.IsNullOrEmpty(parameterValue))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        private string GetDomain(string url)
        {
            var uri = default(Uri);

            try
            {
                uri =  new Uri(url);
            }
            catch (Exception exception)
            {
                throw new ArgumentException(string.Format("'{0}' is not a valid URL format.", exception));
            }

            return uri.Host;
        }

        #endregion Private Methods
    }
}
