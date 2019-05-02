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

    [TextField( "Set Page Title", "The text to display as the heading.", false, "EQ Inventory Assessment", order: 0 )]
    [TextField( "Set Page Icon", "The css class name to use for the heading icon.", false, "fa fa-gift", order: 1 )]
    [IntegerField( "Number of Questions", "The number of questions to show per page while taking the test", true, 7, order: 2 )]
    [BooleanField( "Allow Retakes", "If enabled, the person can retake the test after the minimum days passes.", true, order: 3 )]
    [IntegerField( "Min Days To Retake", "The number of days that must pass before the test can be taken again. Leave blank to use the Assessment Type's minimum.", false, order: 4 )]
    [CodeEditorField( "Instructions", "The text (HTML) to display at the top of the instructions section.  <span class='tip tip-lava'></span> <span class='tip tip-html'></span>", CodeEditorMode.Html, CodeEditorTheme.Rock, 400, true, @"
<h2>Welcome to the EQ Inventory Assessment</h2>
<p>
    {{ Person.NickName }}, this assessment was developed and researched by Dr. Gregory A. Wiens.
</p>
<p>
 Our TrueWiring Emotional Intelligence Inventory (EQ-W) assesses your developed skills in two domains:
   <ol>
      <li> understanding your own emotions </li>
      <li> understanding the emotions of others. This instrument identifies your ability to appropriately express your emotions while encouraging others to do the same. </li>
   </ol>
</p>
<p>
    Before you begin, please take a moment and pray that the Holy Spirit would guide your thoughts,
    calm your mind, and help you respond to each item as honestly as you can. Don't spend much time
    on each item. Your first instinct is probably your best response.
</p>" )]

    [CodeEditorField( "Results Message", "The text (HTML) to display at the top of the results section.<span class='tip tip-lava'></span><span class='tip tip-html'></span>", CodeEditorMode.Html, CodeEditorTheme.Rock, 400, true, @"

<h2>EQ Inventory Assessment</h2>

<h3>Self Awareness</h3>
<p>
    Self Awareness is being aware of what emotions you are experiencing and why you
    are experiencing these emotions. This skill is demonstrated in real time. In other
    words, when you are in the midst of a discussion or even a disagreement with someone
    else, ask yourself these questions:
    <ul>
        <li>Are you aware of what emotions you are experiencing?</li>
        <li>Are you aware of why you are experiencing these emotions?</li>
    </ul>

    More than just knowing you are angry, our goal is to understand what has caused the
    anger, such as frustration, hurt, pain, confusion, etc.
</p>

<!-- Graph -->
{[ chart type:'horizontalBar' legendshow:'false' tooltipshow:'false' chartheight:'50' xaxistype:'linear0to100' ]}
    [[ dataitem label:'Self Awareness' value:'{{SelfAwareness}}' fillcolor:'#5c8ae7' ]] [[ enddataitem ]]
{[ endchart ]}
<p class='text-center'><cite>Source: https://healthygrowingleaders.com</cite></p>

<blockquote>
    Your responses to the items on the Self Awareness scale indicate the score for the
    ability to be aware of your own emotions is equal to or better than {{ SelfAwareness }}%
    of those who completed this instrument.
</blockquote>

<h3>Self Regulating</h3>
<p>
    Self Regulating is appropriately expressing your emotions in the context of relationships
    around you. Don’t confuse this with learning to suppress your emotions; rather, think of Self
    Regulating as the ability to express your emotions appropriately. Healthy human beings
    experience a full range of emotions and these are important for family, friends, and
    co-workers to understand. Self Regulating is learning to tell others what you are
    feeling in the moment.
</p>

    {[ chart type:'horizontalBar' legendshow:'false' tooltipshow:'false' chartheight:'50' xaxistype:'linear0to100' ]}
        [[ dataitem label:'Self Regulating' value:'{{SelfRegulating}}' fillcolor:'#175c2d']] [[ enddataitem ]]
    {[ endchart ]}
<p class='text-center'><cite>Source: https://healthygrowingleaders.com</cite></p>

<blockquote>
    Your responses to the items on the Self Regulation scale indicate the score for the
    the ability to appropriately express your own emotions is equal to or better than {{ SelfRegulating }}%
    of those who completed this instrument.
</blockquote>


<h3>Others Awareness</h3>
<p>
    Others Awareness is being aware of what emotions others are experiencing around you and
    why they are experiencing these emotions. As with understanding your own emotions, this
    skill is knowing in real time what another is experiencing. This skill involves reading
    cues to their emotional state through their eyes, facial expressions, body posture, the
    tone of voice or many other ways. It is critical you learn to pay attention to these
    cues for you to enhance your awareness of others' emotions.
</p>

    {[ chart type:'horizontalBar' legendshow:'false' tooltipshow:'false' chartheight:'50' xaxistype:'linear0to100' ]}
        [[ dataitem label:'Others Awareness' value:'{{OthersAwareness}}' fillcolor:'#2e2e5e' ]] [[ enddataitem ]]
    {[ endchart ]}
<p class='text-center'><cite>Source: https://healthygrowingleaders.com</cite></p>

<blockquote>
    Your responses to the items on the Others Awareness scale indicate the score for the
    ability to be aware of others emotions is equal to or better than {{ OthersAwareness }}%
    of those who completed this instrument.
</blockquote>


<h3>Others Regulating</h3>
<p>
    Others Regulating is helping those around you express their emotions appropriately
    in the context of your relationship with them. This skill centers on helping others
    know what emotions they are experiencing and then asking questions or giving permission
    to them to freely and appropriately express their emotions in the context of your relationship.
</p>

    {[ chart type:'horizontalBar' legendshow:'false' tooltipshow:'false' chartheight:'50' xaxistype:'linear0to100' ]}
        [[ dataitem label:'Others Regulating' value:'{{OthersRegulating}}' fillcolor:'#5c5c2d' ]] [[ enddataitem ]]
    {[ endchart ]}
<p class='text-center'><cite>Source: https://healthygrowingleaders.com</cite></p>

<blockquote>
    Your responses to the items on the Others Regulation scale indicate the score for
    the ability to enable others to appropriately express their emotions in the context
    of your relationship is equal to or better than {{OthersRegulating}}% of those who
    completed this instrument.
</blockquote>


<h2>Additional Scales</h2>
<p>
    The EQ*i includes two additional scales which are particularly useful for those in
    leadership roles: 1) EQ in Problem Solving and 2) EQ under stress. Frequently we
    find it difficult to appreciate that conflicting emotions exacerbate most problems
    we experience in life. The solution, therefore, must account for these emotions, not
    just logic or doing the “right” thing.
</p>

<h3>EQ in Problem Solving</h3>
<p>
    The EQ in Problem Solving identifies how proficient you are in using emotions to
    solve problems. This skill requires first being aware of what emotions are involved
    in the problem and what is the source of those emotions. It also includes helping
    others (and yourself) express those emotions within the discussion.
</p>

    {[ chart type:'horizontalBar' legendshow:'false' tooltipshow:'false' chartheight:'50' xaxistype:'linear0to100' ]}
        [[ dataitem label:'EQ in Problem Solving' value:'{{EQinProblemSolving}}' fillcolor:'#5b2d09' ]] [[ enddataitem ]]
    {[ endchart ]}
<p class='text-center'><cite>Source: https://healthygrowingleaders.com</cite></p>

<blockquote>
    Your responses to the items on the EQ in Problem Solving scale indicate the score for
    the ability to use emotions in resolving problems is equal to or better than {{ EQinProblemSolving }}%
    of those who completed this instrument.
</blockquote>


<h3>EQ Under Stress</h3>
<p>
    It is more difficult to maintain high EQ under high stress than at any other time,
    so EQ Under Stress identifies how capable you are to keep high EQ under high-stress
    moments. This skill requires highly developed Self and Others awareness to understand
    how the stress is impacting yourself and others. It also involves being able to
    articulate the appropriate emotions under pressure which may be different from
    articulating them when not under stress.
</p>

    {[ chart type:'horizontalBar' legendshow:'false' tooltipshow:'false' chartheight:'50' xaxistype:'linear0to100' ]}
        [[ dataitem label:'EQ Under Stress' value:'{{EQUnderStress}}' fillcolor:'#8a5c2d' ]] [[ enddataitem ]]
    {[ endchart ]}
<p class='text-center'><cite>Source: https://healthygrowingleaders.com</cite></p>

<blockquote>
    Your responses to the items on the EQ in Under Stress scale indicate the score
    for the ability to maintain EQ under significant stress is equal to or better than {{ EQUnderStress }}%
    of those who completed this instrument.
</blockquote>
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
        private Dictionary<int, string> NEGATIVE_OPTION = new Dictionary<int, string>
        {
            { 5, "Never" },
            { 4, "Rarely" },
            { 3, "Sometimes" },
            { 2, "Usually" },
            { 1, "Always" }
        };

        private Dictionary<int, string> POSITIVE_OPTION = new Dictionary<int, string>
        {
            { 1, "Never" },
            { 2, "Rarely" },
            { 3, "Sometimes" },
            { 4, "Usually" },
            { 5, "Always" }
        };

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
                var assessmentType = new AssessmentTypeService( rockContext ).Get( Rock.SystemGuid.AssessmentType.EQ.AsGuid() );
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
                        EQInventoryService.AssessmentResults savedScores = EQInventoryService.LoadSavedAssessmentResults( _targetPerson );
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
                EQInventoryService.AssessmentResults result = EQInventoryService.GetResult( AssessmentResponses.ToDictionary( a => a.Code, b => b.Response.Value ) );
                EQInventoryService.SaveAssessmentResults( _targetPerson, result );
                var rockContext = new RockContext();

                var assessmentService = new AssessmentService( rockContext );
                Assessment assessment = null;

                if ( hfAssessmentId.ValueAsInt() != 0 )
                {
                    assessment = assessmentService.Get( int.Parse( hfAssessmentId.Value ) );
                }

                if ( assessment == null )
                {
                    var assessmentType = new AssessmentTypeService( rockContext ).Get( Rock.SystemGuid.AssessmentType.EQ.AsGuid() );
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

            if ( assessmentResponseRow.Code.EndsWith( "N" ) )
            {
                rblQuestion.DataSource = NEGATIVE_OPTION;
               
            }
            else
            {
                rblQuestion.DataSource = POSITIVE_OPTION;
            }
            rblQuestion.DataTextField = "Value";
            rblQuestion.DataValueField = "Key";
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
        private void ShowResult( EQInventoryService.AssessmentResults result, Assessment assessment )
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
                mergeFields.Add( "SelfAwareness", result.SelfAwareConstruct );
                mergeFields.Add( "SelfRegulating", result.SelfRegulatingConstruct );
                mergeFields.Add( "OthersAwareness", result.OtherAwarenessContruct );
                mergeFields.Add( "OthersRegulating", result.OthersRegulatingConstruct );
                mergeFields.Add( "EQinProblemSolving", result.EQ_ProblemSolvingScale );
                mergeFields.Add( "EQUnderStress", result.EQ_UnderStressScale );
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
            AssessmentResponses = EQInventoryService.GetQuestions()
                                    .Select( a => new AssessmentResponse()
                                    {
                                        Code = a.Key,
                                        Question = a.Value
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
            public int? Response { get; set; }
        }

        #endregion
    }
}