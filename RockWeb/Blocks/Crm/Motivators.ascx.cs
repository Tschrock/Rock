﻿// <copyright>
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
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;

namespace Rockweb.Blocks.Crm
{
    /// <summary>
    /// Calculates a person's EQ Inventory assessment score based on a series of question answers.
    /// </summary>
    [DisplayName( "Motivators Assessment" )]
    [Category( "CRM" )]
    [Description( "Allows you to take a EQ Inventory test and saves your EQ Inventory score." )]

    [TextField( "Set Page Title", "The text to display as the heading.", false, "Motivators Assessment", order: 0 )]
    [TextField( "Set Page Icon", "The css class name to use for the heading icon.", false, "fa fa-gift", order: 1 )]
    [IntegerField( "Number of Questions", "The number of questions to show per page while taking the test", true, 20, order: 2 )]
    [BooleanField( "Allow Retakes", "If enabled, the person can retake the test after the minimum days passes.", true, order: 3 )]
    [IntegerField( "Min Days To Retake", "The number of days that must pass before the test can be taken again. Leave blank to use the Assessment Type's minimum.", false, order: 4 )]
    [CodeEditorField( "Instructions", "The text (HTML) to display at the top of the instructions section.  <span class='tip tip-lava'></span> <span class='tip tip-html'></span>", CodeEditorMode.Html, CodeEditorTheme.Rock, 400, true, @"
<h2>Welcome to the Motivators Assessment</h2>
<p>
    {{ Person.NickName }}, this assessment was developed and researched by Dr. Gregory A. Wiens and is intended to help identify the things that you value. These motivators influence your personal, professional, social and every other part of your life because they influence what you view as important and what should or should not be paid attention to. They impact the way you lead or even if you lead. They directly sway how you view your current situation.
</p>
<p>
   We all have internal mechanisms that cause us to view life very differently from others. Some of this could be attributed to our personality. However, a great deal of research has been done to identify different values, motivators or internal drivers which cause each of us to have a different perspective on people, places, and events. These values cause you to construe one situation very differently from another who value things differently.
</p>
<p>
    Before you begin, please take a moment and pray that the Holy Spirit would guide your thoughts,
    calm your mind, and help you respond to each item as honestly as you can. Don't spend much time
    on each item. Your first instinct is probably your best response.
</p>" )]

    [CodeEditorField( "Results Message", "The text (HTML) to display at the top of the results section.<span class='tip tip-lava'></span><span class='tip tip-html'></span>", CodeEditorMode.Html, CodeEditorTheme.Rock, 400, true, @"

<h2>Welcome to the Motivators Assessment</h2>
<p>
    {{ Person.NickName }}, this assessment was developed and researched by Dr. Gregory A. Wiens and is intended to help identify the things that you value. These motivators influence your personal, professional, social and every other part of your life because they influence what you view as important and what should or should not be paid attention to. They impact the way you lead or even if you lead. They directly sway how you view your current situation.
</p>
<p>
   We all have internal mechanisms that cause us to view life very differently from others. Some of this could be attributed to our personality. However, a great deal of research has been done to identify different values, motivators or internal drivers which cause each of us to have a different perspective on people, places, and events. These values cause you to construe one situation very differently from another who value things differently.
</p>
<p>
    Before you begin, please take a moment and pray that the Holy Spirit would guide your thoughts,
    calm your mind, and help you respond to each item as honestly as you can. Don't spend much time
    on each item. Your first instinct is probably your best response.
</p>

Results Message (block setting default value):
<p>
   This assessment identifies 22 different motivators (scales) which illustrate different things to which we all assign importance. These motivators listed in descending order on the report from the highest to the lowest. No one motivator is better than another. They are all critical and essential for the health of an organization. There are over 1,124,000,727,777,607,680,000 different combinations of these 22 motivators so we would hope you realize that your exceptional combination is clearly unique. We believe it is as important for you to know the motivators which are at the top as well as the ones at the bottom of your list. This is because you would best be advised to seek roles and responsibilities where your top motivators are needed. On the other hand, it would be advisable to <i>avoid roles or responsibilities where your bottom motivators would be required</i>. 
</p>

<h2>Influential, Organizational, Intellectual, and Operational</h2>
<p>
Each of the 22 motivators are grouped into one of four clusters: Influential, Organizational, Intellectual, and Operational. The clusters, graphed below, include the motivators that fall within each grouping.
</p>
<!--  Cluster Chart -->
    <div class=""panel panel-default"">
      <div class=""panel-heading"">
        <h2 class=""panel-title""><b>Composite Score</b></h2>
      </div>
      <div class=""panel-body"">
    {[chart type:'horizontalBar' chartheight:'1200' chartwidth:'75' ]}
    {% for motivatorClusterScore in MotivatorClusterScores %}
        [[dataitem label:'{{ motivatorClusterScore.DefinedValue.Value }}' value:'{{ motivatorClusterScore.Value }}' fillcolor:'{{ motivatorClusterScore.DefinedValue | Attribute:'Color' }}' ]] 
        [[enddataitem]]
    {% endfor %}
    {[endchart]}
    
        Source: https://healthygrowingleaders.com
      </div>
    </div>
<p>
This graph is based on the average composite score for each cluster of Motivators.
</p>
{% for motivatorClusterScore in MotivatorClusterScores %}
<p>
<b>{{ motivatorClusterScore.DefinedValue.Value }}</b>
</br>
{{ motivatorClusterScore.DefinedValue.Description }}
</br>
{{ motivatorClusterScore.DefinedValue | Attribute:'Summary' }}
</p>

 {% endfor %}
<p>
   The following graph shows your motivators ranked from top to bottom.
</p>

  {[chart type:'horizontalBar' chartheight:'500' chartwidth:'75' ]}
    {% for motivatorScore in MotivatorScores %}
    {% assign cluster = motivatorScore.DefinedValue | Attribute:'Cluster' %}
        {% if cluster and cluster != empty %}
            [[dataitem label:'{{ motivatorScore.DefinedValue.Value }}' value:'{{ motivatorScore.Value }}' fillcolor:'{{ motivatorScore.DefinedValue | Attribute:'Color' }}' ]] 
            [[enddataitem]]
        {% endif %}
    {% endfor %}
    {[endchart]}
<p>
    Your motivators will no doubt shift and morph throughout your life.For instance, #4 may drop to #7 and vice versa.  However, it is very doubtful that #22 would ever become #1. For that reason, read through all of the motivators and appreciate the ones that you have. Seek input from those who know you to see if they agree or disagree with these results.
</p>
" )]
    public partial class Motivators : Rock.Web.UI.RockBlock
    {
        #region Fields

        //block attribute keys
        private const string NUMBER_OF_QUESTIONS = "NumberofQuestions";
        private const string INSTRUCTIONS = "Instructions";
        private const string SET_PAGE_TITLE = "SetPageTitle";
        private const string SET_PAGE_ICON = "SetPageIcon";
        private const string RESULTS_MESSAGE = "ResultsMessage";
        private const string ALLOW_RETAKES = "AllowRetakes";
        private const string MIN_DAYS_TO_RETAKE = "MinDaysToRetake";

        // View State Keys
        private const string ASSESSMENT_STATE = "AssessmentState";

        // View State Variables
        private List<AssessmentResponse> AssessmentResponses;

        // used for private variables
        Person _targetPerson = null;
        int? _assessmentId = null;
        bool _isQuerystringPersonKey = false;

        // protected variables
        private decimal _percentComplete = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the percent complete.
        /// </summary>
        /// <value>
        /// The percent complete.
        /// </value>
        public decimal PercentComplete
        {
            get
            {
                return _percentComplete;
            }

            set
            {
                _percentComplete = value;
            }
        }

        /// <summary>
        /// Gets or sets the total number of questions
        /// </summary>
        public int QuestionCount
        {
            get { return ViewState[NUMBER_OF_QUESTIONS] as int? ?? 0; }
            set { ViewState[NUMBER_OF_QUESTIONS] = value; }
        }

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            AssessmentResponses = ViewState[ASSESSMENT_STATE] as List<AssessmentResponse> ?? new List<AssessmentResponse>();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            SetPanelTitleAndIcon();

            // otherwise use the currently logged in person
            string personKey = PageParameter( "Person" );
            if ( !string.IsNullOrEmpty( personKey ) )
            {
                try
                {
                    _targetPerson = new PersonService( new RockContext() ).GetByUrlEncodedKey( personKey );
                    _isQuerystringPersonKey = true;
                }
                catch ( Exception )
                {
                    nbError.Visible = true;
                }
            }
            else if ( CurrentPerson != null )
            {
                _targetPerson = CurrentPerson;
            }

            _assessmentId = PageParameter( "AssessmentId" ).AsIntegerOrNull();
            if ( _targetPerson == null )
            {
                pnlInstructions.Visible = false;
                pnlQuestion.Visible = false;
                pnlResult.Visible = false;
                nbError.Visible = true;
                if ( _isQuerystringPersonKey )
                {
                    nbError.Text = "There is an issue locating the person associated with the request.";
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            if ( !Page.IsPostBack )
            {
                var rockContext = new RockContext();
                var assessmentType = new AssessmentTypeService( rockContext ).Get( Rock.SystemGuid.AssessmentType.MOTIVATORS.AsGuid() );
                Assessment assessment = null;

                if ( _targetPerson != null )
                {
                    var primaryAliasId = _targetPerson.PrimaryAliasId;
                    assessment = new AssessmentService( rockContext )
                                            .Queryable()
                                            .Where( a => ( _assessmentId.HasValue && a.Id == _assessmentId ) ||
                                                         ( a.PersonAliasId == primaryAliasId && a.AssessmentTypeId == assessmentType.Id ) )
                                            .OrderByDescending( a => a.CreatedDateTime )
                                            .FirstOrDefault();


                    if ( assessment != null )
                    {
                        hfAssessmentId.SetValue( assessment.Id );
                    }
                    else
                    {
                        hfAssessmentId.SetValue( 0 );
                    }

                    if ( assessment != null && assessment.Status == AssessmentRequestStatus.Complete )
                    {
                        MotivatorService.AssessmentResults savedScores = MotivatorService.LoadSavedAssessmentResults( _targetPerson );
                        ShowResult( savedScores, assessment );

                    }
                    else if ( ( assessment == null && !assessmentType.RequiresRequest ) || ( assessment != null && assessment.Status == AssessmentRequestStatus.Pending ) )
                    {
                        ShowInstructions();
                    }
                    else
                    {
                        pnlInstructions.Visible = false;
                        pnlQuestion.Visible = false;
                        pnlResult.Visible = false;
                        nbError.Visible = true;
                        nbError.Text = "Sorry, this test requires a request from someone before it can be taken.";
                    }
                }
            }
            else
            {
                // Hide notification panels on every postback
                nbError.Visible = false;
            }
        }

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            ViewState[ASSESSMENT_STATE] = AssessmentResponses;

            return base.SaveViewState();
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the Click event of the btnStart button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnStart_Click( object sender, EventArgs e )
        {
            ShowQuestions();
        }

        /// <summary>
        /// Handles the Click event of the btnRetakeTest button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnRetakeTest_Click( object sender, EventArgs e )
        {
            hfAssessmentId.SetValue( 0 );
            ShowInstructions();
        }

        /// <summary>
        /// Handles the Click event of the btnNext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnNext_Click( object sender, EventArgs e )
        {
            int pageNumber = hfPageNo.ValueAsInt() + 1;
            GetResponse();

            LinkButton btn = ( LinkButton ) sender;
            string commandArgument = btn.CommandArgument;

            var totalQuestion = pageNumber * QuestionCount;
            if ( AssessmentResponses.Count > totalQuestion && !AssessmentResponses.All( a => a.Response.HasValue ) || "Next".Equals( commandArgument ) )
            {
                BindRepeater( pageNumber );
            }
            else
            {
                MotivatorService.AssessmentResults result = MotivatorService.GetResult( AssessmentResponses.ToDictionary( a => a.Code, b => b.Response.Value ) );
                MotivatorService.SaveAssessmentResults( _targetPerson, result );
                var rockContext = new RockContext();

                var assessmentService = new AssessmentService( rockContext );
                Assessment assessment = null;

                if ( hfAssessmentId.ValueAsInt() != 0 )
                {
                    assessment = assessmentService.Get( int.Parse( hfAssessmentId.Value ) );
                }

                if ( assessment == null )
                {
                    var assessmentType = new AssessmentTypeService( rockContext ).Get( Rock.SystemGuid.AssessmentType.MOTIVATORS.AsGuid() );
                    assessment = new Assessment()
                    {
                        AssessmentTypeId = assessmentType.Id,
                        PersonAliasId = _targetPerson.PrimaryAliasId.Value
                    };
                    assessmentService.Add( assessment );
                }

                assessment.Status = AssessmentRequestStatus.Complete;
                assessment.CompletedDateTime = RockDateTime.Now;
                assessment.AssessmentResultData = result.AssessmentData.ToJson();
                rockContext.SaveChanges();

                ShowResult( result, assessment );
            }
        }

        /// <summary>
        /// Handles the Click event of the btnPrevious control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnPrevious_Click( object sender, EventArgs e )
        {
            int pageNumber = hfPageNo.ValueAsInt() - 1;
            GetResponse();
            BindRepeater( pageNumber );
        }

        /// <summary>
        /// Handles the ItemDataBound event of the rQuestions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void rQuestions_ItemDataBound( object sender, RepeaterItemEventArgs e )
        {
            var assessmentResponseRow = e.Item.DataItem as AssessmentResponse;
            RockRadioButtonList rblQuestion = e.Item.FindControl( "rblQuestion" ) as RockRadioButtonList;

            if ( assessmentResponseRow.OptionType == MotivatorService.OptionType.Frequency )
            {
                rblQuestion.DataSource = MotivatorService.Frequency_Option;
            }
            else
            {
                rblQuestion.DataSource = MotivatorService.Agreement_Option;
            }

            rblQuestion.DataTextField = "Name";

            if ( assessmentResponseRow.Code.EndsWith( "N" ) )
            {
                rblQuestion.DataValueField = "Negative";

            }
            else
            {
                rblQuestion.DataValueField = "Positive";
            }
            
            rblQuestion.DataBind();

            rblQuestion.Label = assessmentResponseRow.Question;

            if ( assessmentResponseRow != null && assessmentResponseRow.Response.HasValue )
            {
                rblQuestion.SetValue( assessmentResponseRow.Response );
            }
            else
            {
                rblQuestion.SetValue( string.Empty );
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the page title and icon.
        /// </summary>
        private void SetPanelTitleAndIcon()
        {
            string panelTitle = this.GetAttributeValue( SET_PAGE_TITLE );
            if ( !string.IsNullOrEmpty( panelTitle ) )
            {
                lTitle.Text = panelTitle;
            }

            string panelIcon = this.GetAttributeValue( SET_PAGE_ICON );
            if ( !string.IsNullOrEmpty( panelIcon ) )
            {
                iIcon.Attributes["class"] = panelIcon;
            }
        }

        /// <summary>
        /// Shows the instructions.
        /// </summary>
        private void ShowInstructions()
        {
            pnlInstructions.Visible = true;
            pnlQuestion.Visible = false;
            pnlResult.Visible = false;
            // Resolve the text field merge fields
            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, _targetPerson );
            if ( _targetPerson != null )
            {
                mergeFields.Add( "Person", _targetPerson );
            }
            lInstructions.Text = GetAttributeValue( INSTRUCTIONS ).ResolveMergeFields( mergeFields );
        }

        /// <summary>
        /// Shows the result.
        /// </summary>
        private void ShowResult( MotivatorService.AssessmentResults result, Assessment assessment )
        {
            pnlInstructions.Visible = false;
            pnlQuestion.Visible = false;
            pnlResult.Visible = true;

            var allowRetakes = GetAttributeValue( ALLOW_RETAKES ).AsBoolean();
            var minDays = GetAttributeValue( MIN_DAYS_TO_RETAKE ).AsInteger();
            if ( minDays == 0 )
            {
                minDays = assessment.AssessmentType.MinimumDaysToRetake;
            }

            if ( !_isQuerystringPersonKey && allowRetakes && assessment.CompletedDateTime.HasValue && assessment.CompletedDateTime.Value.AddDays( minDays ) <= RockDateTime.Now )
            {
                btnRetakeTest.Visible = true;
            }
            else
            {
                btnRetakeTest.Visible = false;
            }
            // Resolve the text field merge fields
            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, _targetPerson );
            if ( _targetPerson != null )
            {
                _targetPerson.LoadAttributes();
                mergeFields.Add( "Person", _targetPerson );

                // The five Mode scores
                mergeFields.Add( "MotivatorClusterScores", result.MotivatorClusterScores );
                mergeFields.Add( "MotivatorScores", result.MotivatorScores );
            }
            lResult.Text = GetAttributeValue( RESULTS_MESSAGE ).ResolveMergeFields( mergeFields );
        }

        /// <summary>
        /// Shows the questions.
        /// </summary>
        private void ShowQuestions()
        {
            pnlInstructions.Visible = false;
            pnlQuestion.Visible = true;
            pnlResult.Visible = false;
            AssessmentResponses = MotivatorService.GetQuestions()
                                    .Select( a => new AssessmentResponse()
                                    {
                                        Code = a.Id,
                                        Question = a.Question,
                                        OptionType = a.OptionType
                                    } ).ToList();

            // If _maxQuestions has not been set yet...
            if ( QuestionCount == 0 && AssessmentResponses != null )
            {
                // Set the max number of questions to be no greater than the actual number of questions.
                int numQuestions = this.GetAttributeValue( NUMBER_OF_QUESTIONS ).AsInteger();
                QuestionCount = ( numQuestions > AssessmentResponses.Count ) ? AssessmentResponses.Count : numQuestions;
            }

            BindRepeater( 0 );
        }

        /// <summary>
        /// Binds the question data to the rQuestions repeater control.
        /// </summary>
        private void BindRepeater( int pageNumber )
        {
            hfPageNo.SetValue( pageNumber );

            var answeredQuestionCount = AssessmentResponses.Where( a => a.Response.HasValue ).Count();
            PercentComplete = Math.Round( ( Convert.ToDecimal( answeredQuestionCount ) / Convert.ToDecimal( AssessmentResponses.Count ) ) * 100.0m, 2 );

            var skipCount = pageNumber * QuestionCount;

            var questions = AssessmentResponses
                .Skip( skipCount )
                .Take( QuestionCount + 1 )
                .ToList();

            rQuestions.DataSource = questions.Take( QuestionCount );
            rQuestions.DataBind();

            // set next button
            if ( questions.Count() > QuestionCount )
            {
                btnNext.Text = "Next";
                btnNext.CommandArgument = "Next";
            }
            else
            {
                btnNext.Text = "Finish";
                btnNext.CommandArgument = "Finish";
            }

            // build prev button
            if ( pageNumber == 0 )
            {
                btnPrevious.Visible = btnPrevious.Enabled = false;
            }
            else
            {
                btnPrevious.Visible = btnPrevious.Enabled = true;
            }
        }

        /// <summary>
        /// Gets the response to the rQuestions repeater control.
        /// </summary>
        private void GetResponse()
        {
            foreach ( var item in rQuestions.Items.OfType<RepeaterItem>() )
            {
                HiddenField hfQuestionCode = item.FindControl( "hfQuestionCode" ) as HiddenField;
                RockRadioButtonList rblQuestion = item.FindControl( "rblQuestion" ) as RockRadioButtonList;
                var assessment = AssessmentResponses.SingleOrDefault( a => a.Code == hfQuestionCode.Value );
                if ( assessment != null )
                {
                    assessment.Response = rblQuestion.SelectedValueAsInt( false );
                }
            }
        }

        #endregion

        #region nested classes

        [Serializable]
        public class AssessmentResponse
        {
            public string Code { get; set; }
            public string Question { get; set; }
            public MotivatorService.OptionType OptionType { get; set; }
            public int? Response { get; set; }
        }

        #endregion
    }
}