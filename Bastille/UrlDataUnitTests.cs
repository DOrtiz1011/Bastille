using System;
using System.Collections.Generic;
using System.Linq;

namespace Bastille
{
    class UrlDataUnitTests
    {
        private UrlData testUrlData = new UrlData();
        private List<Tuple<string, string>> testData = new List<Tuple<string, string>>();

        public void RunAllUnitTests()
        {
            SetupTestData();

            SaveSingleRecordEmptyHashes();
            SaveSingleRecordTwiceEmptyHashes();
            TestRemoveUrl();
            TestGetUrlsForUser();
            TestGetUsersByDomain();
        }

        private void SetupTestData()
        {
            testData.Add(new Tuple<string, string>("Dan", @"https://www.google.com"));
            testData.Add(new Tuple<string, string>("Dan", @"https://www.nba.com"));
            testData.Add(new Tuple<string, string>("Dan", @"https://www.linkedin.com"));
            testData.Add(new Tuple<string, string>("Joe", @"https://www.google.com"));
            testData.Add(new Tuple<string, string>("Joe", @"https://www.nfl.com"));
            testData.Add(new Tuple<string, string>("Joe", @"https://www.espn.com"));
            testData.Add(new Tuple<string, string>("Ed",  @"https://www.google.com"));
            testData.Add(new Tuple<string, string>("Ed",  @"https://www.nfl.com"));
            testData.Add(new Tuple<string, string>("Ed",  @"https://www.glassdoor.com"));
        }

        private void AddAllTestData()
        {
            foreach (var tuple in testData)
            {
                testUrlData.SaveUrl(tuple.Item1, tuple.Item2);
            }
        }

        private bool SaveSingleRecordEmptyHashes()
        {
            var success = false;
            var userToken = testData.First().Item1;
            var url = testData.First().Item2;
            var domain = testUrlData.GetDomain(url);

            testUrlData.ClearData();

            var saveSucessful = testUrlData.SaveUrl(userToken, url);

            if (testUrlData.UserHash.ContainsKey(userToken) &&
                testUrlData.UserHash.Count == 1 &&
                testUrlData.UserHash[userToken].Count == 1 &&
                testUrlData.UserHash[userToken].Contains(url) &&
                testUrlData.DomainHash.ContainsKey(domain) &&
                testUrlData.DomainHash.Count == 1 &&
                testUrlData.DomainHash[domain].Count == 1 &&
                testUrlData.DomainHash[domain].Contains(userToken) &&
                saveSucessful)
            {
                success = true;
            }
            else
            {
                throw new Exception("SaveSingleRecordEmptyHashes test has failed.");
            }

            return success;
        }

        private bool SaveSingleRecordTwiceEmptyHashes()
        {
            var success = false;
            var userToken = testData.First().Item1;
            var url = testData.First().Item2;
            var domain = testUrlData.GetDomain(url);

            testUrlData.ClearData();

            testUrlData.SaveUrl(userToken, url);
            var saveSucessful = testUrlData.SaveUrl(userToken, url);

            if (testUrlData.UserHash.ContainsKey(userToken) &&
                testUrlData.UserHash.Count == 1 &&
                testUrlData.UserHash[userToken].Count == 1 &&
                testUrlData.UserHash[userToken].Contains(url) &&
                testUrlData.DomainHash.ContainsKey(domain) &&
                testUrlData.DomainHash.Count == 1 &&
                testUrlData.DomainHash[domain].Count == 1 &&
                testUrlData.DomainHash[domain].Contains(userToken) &&
                !saveSucessful)
            {
                success = true;
            }
            else
            {
                throw new Exception("SaveSingleRecordTwiceEmptyHashes test has failed.");
            }

            testUrlData.ClearData();

            return success;
        }

        private bool TestRemoveUrl()
        {
            var success = false;
            var userToken = testData.First().Item1;
            var url = testData.First().Item2;
            var domain = testUrlData.GetDomain(url);

            testUrlData.ClearData();
            AddAllTestData();

            var deleteSucessful = testUrlData.RemoveUrl(userToken, url);

            if (!testUrlData.UserHash[userToken].Contains(url) && 
                !testUrlData.DomainHash[domain].Contains(userToken) &&
                deleteSucessful)
            {
                success = true;
            }
            else
            {
                throw new Exception("SaveSingleRecordTwiceEmptyHashes test has failed.");
            }

            return success;
        }

        private bool TestGetUrlsForUser()
        {
            var success = false;
            var userToken = testData.First().Item1;

            testUrlData.ClearData();
            AddAllTestData();

            var list = testUrlData.GetUrlsForUser(userToken).ToList();

            foreach (var url in testData.Where(x => x.Item1 == userToken).Select(x => x.Item2).ToList())
            {
                if (!list.Contains(url))
                {
                    throw new Exception("TestGetUrlsForUser test has failed.");
                }
            }

            return success;
        }

        private bool TestGetUsersByDomain()
        {
            var success = false;
            var url = testData.First().Item2;
            var domain = testUrlData.GetDomain(url);

            testUrlData.ClearData();
            AddAllTestData();

            var list = testUrlData.GetUsersByDomain(domain).ToList();

            foreach (var userToken in testData.Where(x => x.Item2 == url).Select(x => x.Item1).ToList())
            {
                if (!list.Contains(userToken))
                {
                    throw new Exception("TestGetUsersByDomain test has failed.");
                }
            }

            return success;
        }
    }
}
