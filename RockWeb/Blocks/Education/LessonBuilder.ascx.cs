// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
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
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Rock;
using Rock.Attribute;
using Rock.Constants;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Attribute = Rock.Model.Attribute;

namespace RockWeb.Blocks.Education
{
    /// <summary>
    /// Displays the details of the given calendar event item.
    /// </summary>
    [DisplayName( "Lesson Builder" )]
    [Category( "Education" )]
    [Description( "Allows teachers to build a lesson plan and indicate the materials required for their lesson." )]

    #region "Block Attributes"

    [GroupTypesField(
        name: "Available Group Types",
        description: "desc",
        required: true,
        defaultGroupTypeGuids: "",
        category: "",
        order: 1,
        key: AttributeKey.AvailableGroupTypes )]

    [ContentChannelField(
        name: "Lesson Plan Content Channel",
        description: "Select the content channel type that contains available lesson content.",
        required: true,
        defaultValue: "",
        category: "",
        order: 2,
        key: AttributeKey.LessonPlanContentChannel )]

    [CodeEditorField(
        name: "Lava Template",
        description: "The Lava Template used to render the lesson builder.",
        mode: CodeEditorMode.Lava,
        theme: CodeEditorTheme.Rock,
        height: 400,
        required: true,
        defaultValue: @"{% include '~/Assets/Lava/LessonBuilder/LessonBuilder.lava' %}",
        category: "",
        order: 3,
        key: AttributeKey.LavaTemplate )]

    #endregion

    public partial class LessonBuilder : RockBlock
    {
        protected static class AttributeKey
        {
            public const string AvailableGroupTypes = "AvailableGroupTypes";
            public const string LessonPlanContentChannel = "LessonPlanContentChannel";
            public const string LavaTemplate = "LavaTemplate";
        }

        private void OutputDebugInfo()
        {
            var lessonChoice = new LessonChoiceDTO()
            {
                //Id = 123,
                //PersonAliasId = 10,
                ScheduleId = 321,
                RoleId = 444,
                LessonOptionIds = new List<int>() { 1, 2, 3, 4 }
            };
            lDebug.Text = "<pre>" + Environment.NewLine +
                "Sample Json Object:" + Environment.NewLine +
                JsonConvert.SerializeObject( lessonChoice ) + Environment.NewLine +
                "</pre>";
        }
        private class LessonChoiceDTO
        {
            //public int Id, PersonAliasId, ScheduleId, RoleId;
            public int ScheduleId, RoleId;
            public List<int> LessonOptionIds;
            public LessonChoiceDTO()
            {
                LessonOptionIds = new List<int>();
            }

        }

        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );

            OutputDebugInfo();

        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            if (!Page.IsPostBack)
            {
                DisplayDetails();
            }
        }

        #endregion Base Control Methods

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            DisplayDetails();
        }

        #endregion Events

        #region Internal Methods

        private void DisplayDetails()
        {
            var contentChannelGuid = GetAttributeValue( AttributeKey.LessonPlanContentChannel ).AsGuid();
            var groupTypeGuids = GetAttributeValues( AttributeKey.AvailableGroupTypes ).AsGuidList();

            ContentChannel contentChannel = null;
            var selectedGroups = new List<Group>();
            var userAgeGroups = new List<Group>();

            using (var rockContext = new RockContext())
            {
                contentChannel = new ContentChannelService( rockContext ).Get( contentChannelGuid );

                var groupTypeService = new GroupTypeService( rockContext );
                var groupIds = groupTypeService.Queryable().AsNoTracking()
                    .Where( gt => groupTypeGuids.Contains( gt.Guid ) )
                    .Select( gt => gt.Id )
                    .ToList();

                var groupService = new GroupService( rockContext );
                selectedGroups = groupService.Queryable().AsNoTracking()
                    .Where( g => groupIds.Contains( g.GroupTypeId) )
                    .ToList();

                var groupMemberService = new GroupMemberService( rockContext );
                foreach (var group in selectedGroups)
                {
                    userAgeGroups.AddRange(
                        groupMemberService.Queryable().AsNoTracking()
                        .Where( gm => gm.PersonId == CurrentPerson.Id && gm.GroupId == group.Id )
                        .Select( gm => gm.Group )
                        .ToList()
                    );
                }

                var mergeFields = new Dictionary<string, object>()
                {
                    { "SelectedGroups", selectedGroups },
                    { "UserAgeGroups", userAgeGroups }
                };
                if (contentChannel != null)
                {
                    mergeFields.Add( "LessonPlanContentChannelId", contentChannel.Id );
                }
                lOutput.Text = GetAttributeValue( AttributeKey.LavaTemplate ).ResolveMergeFields( mergeFields );
            }

            //RockPage.PageTitle = pageTitle;
            //RockPage.BrowserTitle = String.Format( "{0} | {1}", pageTitle, RockPage.Site.Name );
            //RockPage.Header.Title = String.Format( "{0} | {1}", pageTitle, RockPage.Site.Name );
        }
        #endregion


        protected void btnSave_Click( object sender, EventArgs e )
        {
            lDebug.Text = string.Format( "PostBack Successful at {0}.  The value was: {1}", DateTime.Now.ToShortTimeString(),  hfWriteBackValue.Value );

            LessonChoiceDTO lessonChoice;
            try
            {
                lessonChoice = JsonConvert.DeserializeObject<LessonChoiceDTO>( hfWriteBackValue.Value );
                lDebug.Text += "<br />Json Deserialization succeeded.";
                lDebug.Text += "<br />Current Person Alias ID: " + CurrentPersonAliasId;
                lDebug.Text += "<br />Role ID: " + lessonChoice.RoleId;
                lDebug.Text += "<br />Schedule ID: " + lessonChoice.ScheduleId;
                lDebug.Text += "<br />Lesson Option IDs: " + lessonChoice.LessonOptionIds.AsDelimited( ", " );

            }
            catch (Exception ex)
            {
                lDebug.Text += "<br />Json Deserialization failed:" + ex.Message;
            }
            
        }
    }
}