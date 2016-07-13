using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;


namespace Rock.Crawler
{
    public static class Crawler
    {
        #region Private Fields

        private static List<Page> _pages = new List<Page>();
        private static List<string> _externalUrls = new List<string>();
        private static List<string> _otherUrls = new List<string>();
        private static List<string> _failedUrls = new List<string>();
        private static List<string> _exceptions = new List<string>();
        private static StringBuilder _logBuffer = new StringBuilder();

        private static string _startUrl = string.Empty;

        #endregion

        /// <summary>
        /// Crawls a site.
        /// </summary>
        public static int CrawlSite(string startUrl)
        {
            _startUrl = startUrl;
            CrawlPage( startUrl );

            return _pages.Count;
        }

        /// <summary>
        /// Crawls a page.
        /// </summary>
        /// <param name="url">The url to crawl.</param>
        private static void CrawlPage( string url )
        {
            if ( !PageHasBeenCrawled( url ) )
            {
                string htmlText = GetWebText( url );

                Page page = new Page();
                page.Text = htmlText;
                page.Url = url;
                page.CalculateViewstateSize();

                _pages.Add( page );

                LinkParser linkParser = new LinkParser();
                linkParser.ParseLinks( page, url, _startUrl );


                //Add data to main data lists
                AddRangeButNoDuplicates( _externalUrls, linkParser.ExternalUrls );
                AddRangeButNoDuplicates( _otherUrls, linkParser.OtherUrls );
                AddRangeButNoDuplicates( _failedUrls, linkParser.BadUrls );

                foreach ( string exception in linkParser.Exceptions )
                    _exceptions.Add( exception );


                //Crawl all the links found on the page.
                foreach ( string link in linkParser.GoodUrls )
                {
                    string formattedLink = link;
                    try
                    {

                        formattedLink = FixPath( url, formattedLink );

                        if ( formattedLink != String.Empty )
                        {
                            CrawlPage( formattedLink );
                        }
                    }
                    catch ( Exception exc )
                    {
                        _failedUrls.Add( formattedLink + " (on page at url " + url + ") - " + exc.Message );
                    }
                }
            }
        }

        /// <summary>
        /// Fixes a path. Makes sure it is a fully functional absolute url.
        /// </summary>
        /// <param name="originatingUrl">The url that the link was found in.</param>
        /// <param name="link">The link to be fixed up.</param>
        /// <returns>A fixed url that is fit to be fetched.</returns>
        public static string FixPath( string originatingUrl, string link )
        {
            string formattedLink = String.Empty;

            if ( link.IndexOf( "../" ) > -1 )
            {
                formattedLink = ResolveRelativePaths( link, originatingUrl );
            }
            else if ( originatingUrl.IndexOf( _startUrl ) > -1
                && link.IndexOf( _startUrl ) == -1 )
            {
                formattedLink = originatingUrl.Substring( 0, originatingUrl.LastIndexOf( "/" ) + 1 ) + link;
            }
            else if ( link.IndexOf( _startUrl ) == -1 )
            {
                formattedLink = _startUrl + link;
            }

            return formattedLink;
        }

        /// <summary>
        /// Needed a method to turn a relative path into an absolute path. And this seems to work.
        /// </summary>
        /// <param name="relativeUrl">The relative url.</param>
        /// <param name="originatingUrl">The url that contained the relative url.</param>
        /// <returns>A url that was relative but is now absolute.</returns>
        private static string ResolveRelativePaths( string relativeUrl, string originatingUrl )
        {
            string resolvedUrl = String.Empty;

            string[] relativeUrlArray = relativeUrl.Split( new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries );
            string[] originatingUrlElements = originatingUrl.Split( new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries );
            int indexOfFirstNonRelativePathElement = 0;
            for ( int i = 0; i <= relativeUrlArray.Length - 1; i++ )
            {
                if ( relativeUrlArray[i] != ".." )
                {
                    indexOfFirstNonRelativePathElement = i;
                    break;
                }
            }

            int countOfOriginatingUrlElementsToUse = originatingUrlElements.Length - indexOfFirstNonRelativePathElement - 1;
            for ( int i = 0; i <= countOfOriginatingUrlElementsToUse - 1; i++ )
            {
                if ( originatingUrlElements[i] == "http:" || originatingUrlElements[i] == "https:" )
                    resolvedUrl += originatingUrlElements[i] + "//";
                else
                    resolvedUrl += originatingUrlElements[i] + "/";
            }

            for ( int i = 0; i <= relativeUrlArray.Length - 1; i++ )
            {
                if ( i >= indexOfFirstNonRelativePathElement )
                {
                    resolvedUrl += relativeUrlArray[i];

                    if ( i < relativeUrlArray.Length - 1 )
                        resolvedUrl += "/";
                }
            }

            return resolvedUrl;
        }

        /// <summary>
        /// Checks to see if the page has been crawled.
        /// </summary>
        /// <param name="url">The url that has potentially been crawled.</param>
        /// <returns>Boolean indicating whether or not the page has been crawled.</returns>
        private static bool PageHasBeenCrawled( string url )
        {
            foreach ( Page page in _pages )
            {
                if ( page.Url == url )
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Merges a two lists of strings.
        /// </summary>
        /// <param name="targetList">The list into which to merge.</param>
        /// <param name="sourceList">The list whose values need to be merged.</param>
        private static void AddRangeButNoDuplicates( List<string> targetList, List<string> sourceList )
        {
            foreach ( string str in sourceList )
            {
                if ( !targetList.Contains( str ) )
                    targetList.Add( str );
            }
        }

        /// <summary>
        /// Gets the response text for a given url.
        /// </summary>
        /// <param name="url">The url whose text needs to be fetched.</param>
        /// <returns>The text of the response.</returns>
        private static string GetWebText( string url )
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create( url );
            request.UserAgent = "Rock Web Crawler";

            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();

            StreamReader reader = new StreamReader( stream );
            string htmlText = reader.ReadToEnd();
            return htmlText;
        }
    }
}
