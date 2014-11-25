﻿// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock;
using Rock.Communication;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace RockWeb
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Init( object sender, EventArgs e )
        {
            // If this is an API call, and it somehow got this far, set status code and exit
            if ( Request.Url.Query.Contains( Request.Url.Authority + ResolveUrl( "~/api/" ) ) )
            {
                Response.StatusCode = 500;
                Response.Flush();
                Response.End();
                return;
            }
        }
        
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load( object sender, EventArgs e )
        {
            try
            {
                lLogoSvg.Text = System.IO.File.ReadAllText( HttpContext.Current.Request.MapPath( "~/Assets/Images/rock-logo-sm.svg" ) );

                string errorType = Request["type"];
                if ( string.IsNullOrWhiteSpace( errorType ) )
                {
                    errorType = "exception";
                }

                if ( errorType.Equals( "security", StringComparison.OrdinalIgnoreCase ) )
                {
                    ShowSecurityError();
                }
                else
                {
                    ShowException();
                }
            }
            catch { }
        }

        /// <summary>
        /// Shows the security error.
        /// </summary>
        private void ShowSecurityError()
        {
            pnlSecurity.Visible = true;
            pnlException.Visible = false;
        }

        /// <summary>
        /// Shows the exception.
        /// </summary>
        private void ShowException()
        {
            Exception ex = GetSavedValue("RockLastException") as Exception;
            if ( ex != null )
            {
                int? siteId = ( GetSavedValue( "Rock:SiteId" ) ?? "" ).ToString().AsIntegerOrNull();
                if (siteId.HasValue)
                {
                    var site = SiteCache.Read(siteId.Value);
                    if (site != null && !string.IsNullOrWhiteSpace( site.ErrorPage))
                    {
                        Context.Response.Redirect( site.ErrorPage, false );
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                }

                pnlSecurity.Visible = false;
                pnlException.Visible = true;

                int? errorLevel = ( GetSavedValue("RockExceptionOrder") ?? "" ).ToString().AsIntegerOrNull();

                ClearSavedValue( "RockExceptionOrder" );
                ClearSavedValue( "RockLastException" );

                bool showDetails = errorLevel.HasValue && errorLevel.Value == 66;
                if (!showDetails)
                {
                    try 
                    {
                        // check to see if the user is an admin, if so allow them to view the error details
                        var userLogin = Rock.Model.UserLoginService.GetCurrentUser();
                        GroupService service = new GroupService( new RockContext() );
                        Group adminGroup = service.GetByGuid( new Guid( Rock.SystemGuid.Group.GROUP_ADMINISTRATORS ) );
                        showDetails = userLogin != null && adminGroup.Members.Where( m => m.PersonId == userLogin.PersonId ).Count() > 0;
                    }
                    catch {}
                }

                if (showDetails)
                {
                    lErrorInfo.Text = "<h3>Exception Log:</h3>";
                    ProcessException( ex, " " );
                }
            }
        }

        /// <summary>
        /// Processes the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="exLevel">The ex level.</param>
        private void ProcessException( Exception ex, string exLevel )
        {
            lErrorInfo.Text += "<div class=\"alert alert-danger\">";
            lErrorInfo.Text += "<h4>" + exLevel + ex.GetType().Name + " in " + ex.Source + "</h4>";
            lErrorInfo.Text += "<p><strong>Message</strong><br>" + ex.Message + "</p>";
            lErrorInfo.Text += "<p><strong>Stack Trace</strong><br><pre>" + ex.StackTrace + "</pre></p>";
            lErrorInfo.Text += "</div>";

            // check for inner exception
            if ( ex.InnerException != null )
            {
                //lErrorInfo.Text += "<p /><p />";
                ProcessException( ex.InnerException, "-" + exLevel );
            }
        }

        /// <summary>
        /// Gets the saved value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private object GetSavedValue(string key)
        {
            object item = null;
            if ( Context.Session != null)
            {
                item = Context.Session[key];
            }
            if (item == null)
            {
                item = Context.Cache[key];
            }
            return item;
        }

        /// <summary>
        /// Clears the saved value.
        /// </summary>
        /// <param name="key">The key.</param>
        private void ClearSavedValue(string key)
        {
            if ( Context.Session != null)
            {
                Context.Session.Remove(key);
            }
            Context.Cache.Remove(key);
        }

    }
}