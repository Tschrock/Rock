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
namespace Rock.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    /// <summary>
    ///
    /// </summary>
    public partial class AddMotivators : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Believing", "core_MotivatorBelieving", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_BELIVING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_BELIVING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Caring", "core_MotivatorCaring", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_CARING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_CARING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Communicating", "core_MotivatorCommunicating", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_COMMUNICATING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_COMMUNICATING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Empowering", "core_MotivatorEmpowering", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_EMPOWERING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_EMPOWERING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Flexing", "core_MotivatorFlexing", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_FLEXING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_FLEXING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Gathering", "core_MotivatorGathering", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_GATHERING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_GATHERING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Innovating", "core_MotivatorInnovating", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_INNOVATING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_INNOVATING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Leading", "core_MotivatorLeading", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_LEADING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_LEADING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Learning", "core_MotivatorLearning", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_LEARNING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_LEARNING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Maximizing", "core_MotivatorMaximizing", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_MAXIMIZING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_MAXIMIZING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Organizing", "core_MotivatorOrganizing", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_ORGANIZING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_ORGANIZING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Pacing", "core_MotivatorPacing", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_PACING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_PACING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Perceiving", "core_MotivatorPerceiving", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_PERCEIVING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_PERCEIVING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Relating", "core_MotivatorRelating", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_RELATING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_RELATING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Serving", "core_MotivatorServing", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_SERVING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_SERVING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Thinking", "core_MotivatorThinking", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_THINKING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_THINKING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Transforming", "core_MotivatorTransforming", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_TRANSFORMING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_TRANSFORMING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Uniting", "core_MotivatorUniting", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_UNITING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_UNITING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Unwavering", "core_MotivatorUnwavering", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_UNWAVERING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_UNWAVERING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Venturing", "core_MotivatorVenturing", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_VENTURING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_VENTURING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Visioning", "core_MotivatorVisioning", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_VISIONING );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_VISIONING );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator: Growth Propensity", "core_MotivatorGrowthPropensity", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_GROWTHPROPENSITY );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_GROWTHPROPENSITY );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator Cluster: Influential", "core_MotivatorClusterInfluential", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_CLUSTER_INFLUENTIAL );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_CLUSTER_INFLUENTIAL );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator Cluster: Organizational", "core_MotivatorClusterOrganizational", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_CLUSTER_ORGANIZATIONAL );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_CLUSTER_ORGANIZATIONAL );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator Cluster: Intellectual", "core_MotivatorClusterIntellectual", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_CLUSTER_INTELLECTUAL );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_CLUSTER_INTELLECTUAL );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.DECIMAL, "B08A3096-FCFA-4DA0-B95D-1F3F11CC9969", "Motivator Cluster: Operational", "core_MotivatorClusterOperational", "", "", 0, "", SystemGuid.Attribute.PERSON_MOTIVATOR_CLUSTER_OPERATIONAL );
            AddSecurityToAttributes( Rock.SystemGuid.Attribute.PERSON_MOTIVATOR_CLUSTER_OPERATIONAL );

            //Add Motivator Cluster Defined Type
            AddMotivatorClusterDefinedType();

            //Add Motivator Defined Type
            AddMotivatorDefinedType();

            Sql( string.Format( @" DECLARE @DefinedTypeEntityTypeId int = (
                    SELECT TOP 1 [Id]
                    FROM [EntityType]
                    WHERE [Name] = 'Rock.Model.DefinedType' )
                    
                    DECLARE @CategoryId int = (
                    SELECT TOP 1 [Id] FROM [Category]
                    WHERE [EntityTypeId] = @DefinedTypeEntityTypeId
                    AND [Name] = 'TrueWiring' )

                    UPDATE
                        [DefinedType]
                    SET
                        [CategoryId] = @CategoryId
                    WHERE
                        [Guid] IN ( '{0}','{1}','{2}' )",
                        SystemGuid.DefinedType.ASSESSMENT_CONFLICT_PROFILE,
                        SystemGuid.DefinedType.DISC_RESULTS_TYPE,
                        SystemGuid.DefinedType.SPIRITUAL_GIFTS ) );

            AddMotivatorsAssessmenPage();

        }

        private void AddMotivatorsAssessmenPage()
        {
            RockMigrationHelper.AddPage( true, "FCF44690-D74C-4FB7-A01B-0EFCA6EA9E1E", "5FEAF34C-7FB6-4A11-8A1E-C452EC7849BD", "Motivators Assessment", "", "0E6AECD6-675F-4908-9FA3-C7E46040527C", "" ); // Site:External Website
            RockMigrationHelper.UpdateBlockType( "Motivators Assessment", "Allows you to take a Motivators Assessment test and saves your results.", "~/Blocks/Crm/Motivators.ascx", "CRM", "18CF8DA8-5DE0-49EC-A279-D5507CFA5713" );
            // Add Block to Page: Motivator Assessment Site: External Website
            RockMigrationHelper.AddBlock( true, "0E6AECD6-675F-4908-9FA3-C7E46040527C".AsGuid(), null, "F3F82256-2D66-432B-9D67-3552CD2F4C2B".AsGuid(), "18CF8DA8-5DE0-49EC-A279-D5507CFA5713".AsGuid(), "Motivators Assessment", "Main", @"", @"", 0, "92C58130-9CE7-44E0-8F22-DF358A0F69C2" );
            // Attrib for BlockType: Motivators Assessment:Instructions
            RockMigrationHelper.UpdateBlockTypeAttribute( "18CF8DA8-5DE0-49EC-A279-D5507CFA5713", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Instructions", "Instructions", "", @"The text (HTML) to display at the top of the instructions section.  <span class='tip tip-lava'></span> <span class='tip tip-html'></span>", 0, @"<h2>Welcome to the Motivators Assessment</h2>
<p>
    {{ Person.NickName }}, this assessment was developed and researched by Dr. Gregory A. Wiens and is intended to help identify the things that you value. These motivators influence your personal, professional, social and every other part of your life because they influence what you view as important and what should or should not be paid attention to. They impact the way you lead or even if you lead. They directly sway how you view your current situation.
</p>
<p>
   We all have internal mechanisms that cause us to view life very differently from others. Some of this could be attributed to our personality. However, a great deal of research has been done to identify different values, motivators or internal drivers which cause each of us to have a different perspective on people, places, and events. These values cause you to construe one situation very differently from another who value things differently.
</p>
<p>
    Before you begin, please take a moment and pray that the Holy Spirit would guide your thoughts, calm your mind, and help you respond to each item as honestly as you can. Don't spend much time on each item. Your first instinct is probably your best response.
</p>" , "973511D4-7C77-42E0-8FDC-23AE5DF61177" );
            // Attrib for BlockType: Motivators Assessment:Min Days To Retake
            RockMigrationHelper.UpdateBlockTypeAttribute( "18CF8DA8-5DE0-49EC-A279-D5507CFA5713", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Min Days To Retake", "MinDaysToRetake", "", @"The number of days that must pass before the test can be taken again. Leave blank to use the Assessment Type's minimum.", 4, @"", "6DE443ED-658F-422A-8B83-6B8FA4511DED" );
            // Attrib for BlockType: Motivators Assessment:Results Message
            RockMigrationHelper.UpdateBlockTypeAttribute( "18CF8DA8-5DE0-49EC-A279-D5507CFA5713", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Results Message", "ResultsMessage", "", @"The text (HTML) to display at the top of the results section.<span class='tip tip-lava'></span><span class='tip tip-html'></span>", 0, @"<p>
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
</p>", "BA51DFCD-B174-463F-AE3F-6EEE73DD9338" );
            // Attrib for BlockType: Motivators Assessment:Set Page Icon
            RockMigrationHelper.UpdateBlockTypeAttribute( "18CF8DA8-5DE0-49EC-A279-D5507CFA5713", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Set Page Icon", "SetPageIcon", "", @"The css class name to use for the heading icon.", 1, @"", "7471495D-4C68-45EA-874D-6778608E81B2" );
            // Attrib for BlockType: Motivators Assessment:Allow Retakes
            RockMigrationHelper.UpdateBlockTypeAttribute( "18CF8DA8-5DE0-49EC-A279-D5507CFA5713", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Allow Retakes", "AllowRetakes", "", @"If enabled, the person can retake the test after the minimum days passes.", 3, @"True", "3A07B385-A3C1-4C0B-80F9-F50432503C0A" );
            // Attrib for BlockType: Motivators Assessment:Set Page Title
            RockMigrationHelper.UpdateBlockTypeAttribute( "18CF8DA8-5DE0-49EC-A279-D5507CFA5713", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Set Page Title", "SetPageTitle", "", @"The text to display as the heading.", 0, @"EQ Inventory Assessment", "4CE9D93E-2002-425A-A8FD-679CCEE991D7" );
            // Attrib for BlockType: Motivators Assessment:Number of Questions
            RockMigrationHelper.UpdateBlockTypeAttribute( "18CF8DA8-5DE0-49EC-A279-D5507CFA5713", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Number of Questions", "NumberofQuestions", "", @"The number of questions to show per page while taking the test", 2, @"7", "02489F19-384F-45BE-BBC4-D2ECC63D0992" );
            
            // Attrib Value for Block:Motivators Assessment, Attribute:Instructions Page: Motivator Assessment, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue( "92C58130-9CE7-44E0-8F22-DF358A0F69C2", "973511D4-7C77-42E0-8FDC-23AE5DF61177", @"<h2>Welcome to the Motivators Assessment</h2>
<p>
    {{ Person.NickName }}, this assessment was developed and researched by Dr. Gregory A. Wiens and is intended to help identify the things that you value. These motivators influence your personal, professional, social and every other part of your life because they influence what you view as important and what should or should not be paid attention to. They impact the way you lead or even if you lead. They directly sway how you view your current situation.
</p>
<p>
   We all have internal mechanisms that cause us to view life very differently from others. Some of this could be attributed to our personality. However, a great deal of research has been done to identify different values, motivators or internal drivers which cause each of us to have a different perspective on people, places, and events. These values cause you to construe one situation very differently from another who value things differently.
</p>
<p>
    Before you begin, please take a moment and pray that the Holy Spirit would guide your thoughts, calm your mind, and help you respond to each item as honestly as you can. Don't spend much time on each item. Your first instinct is probably your best response.
</p" );
            
            // Attrib Value for Block:Motivators Assessment, Attribute:Results Message Page: Motivator Assessment, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue( "92C58130-9CE7-44E0-8F22-DF358A0F69C2", "BA51DFCD-B174-463F-AE3F-6EEE73DD9338", @"<p>
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
</p>" );
            
            // Attrib Value for Block:Motivators Assessment, Attribute:Set Page Icon Page: Motivator Assessment, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue( "92C58130-9CE7-44E0-8F22-DF358A0F69C2", "7471495D-4C68-45EA-874D-6778608E81B2", @"" );
            // Attrib Value for Block:Motivators Assessment, Attribute:Allow Retakes Page: Motivator Assessment, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue( "92C58130-9CE7-44E0-8F22-DF358A0F69C2", "3A07B385-A3C1-4C0B-80F9-F50432503C0A", @"True" );
            // Attrib Value for Block:Motivators Assessment, Attribute:Set Page Title Page: Motivator Assessment, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue( "92C58130-9CE7-44E0-8F22-DF358A0F69C2", "4CE9D93E-2002-425A-A8FD-679CCEE991D7", @"Motivators Assessment" );
            // Attrib Value for Block:Motivators Assessment, Attribute:Number of Questions Page: Motivator Assessment, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue( "92C58130-9CE7-44E0-8F22-DF358A0F69C2", "02489F19-384F-45BE-BBC4-D2ECC63D0992", @"17" );

            RockMigrationHelper.AddPageRoute("0E6AECD6-675F-4908-9FA3-C7E46040527C","Motivators","7D00FD4E-9E6C-42B1-BB25-7F417DF25CA4");// for Page:Motivators Assessment
            RockMigrationHelper.AddPageRoute("0E6AECD6-675F-4908-9FA3-C7E46040527C","Motivators/{rckipid}","9299B437-38C6-421F-B705-B0F2BCEC2CD0");// for Page:Motivators Assessment
        }

        private void AddMotivatorClusterDefinedType()
        {
            RockMigrationHelper.AddDefinedType( "TrueWiring", "Motivator Cluster", "Used by Rock's TrueWiring Motivator assessment to hold the four cluster groupings.", "354715FA-564A-420A-8324-0411988AE7AB", @"" );
            RockMigrationHelper.AddDefinedTypeAttribute( "354715FA-564A-420A-8324-0411988AE7AB", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Attribute Score Key", "AttributeScoreKey", "", 1026, "", "CE3F126E-B56A-438A-BA45-8EC8437BB961" );
            RockMigrationHelper.AddDefinedTypeAttribute( "354715FA-564A-420A-8324-0411988AE7AB", "D747E6AE-C383-4E22-8846-71518E3DD06F", "Color", "Color", "", 1028, "", "8B5F72E4-5A49-4224-9437-82B1F23D8896" );
            RockMigrationHelper.AddDefinedTypeAttribute( "354715FA-564A-420A-8324-0411988AE7AB", "DD7ED4C0-A9E0-434F-ACFE-BD4F56B043DF", "Summary", "Summary", "", 1027, "", "07E85FA1-8F86-4414-8DC3-43D303C55457" );
            RockMigrationHelper.AddAttributeQualifier( "07E85FA1-8F86-4414-8DC3-43D303C55457", "documentfolderroot", "", "A6D6A112-01E9-4675-8D4E-2214219B1B59" );
            RockMigrationHelper.AddAttributeQualifier( "07E85FA1-8F86-4414-8DC3-43D303C55457", "imagefolderroot", "", "94BA3FFE-3DD9-4827-9779-54DE393467BE" );
            RockMigrationHelper.AddAttributeQualifier( "07E85FA1-8F86-4414-8DC3-43D303C55457", "toolbar", "Light", "AB148217-518F-419C-93BE-7CB0C49B9511" );
            RockMigrationHelper.AddAttributeQualifier( "07E85FA1-8F86-4414-8DC3-43D303C55457", "userspecificroot", "False", "B97AC79C-E703-45E7-B802-13BE25413576" );
            RockMigrationHelper.AddAttributeQualifier( "8B5F72E4-5A49-4224-9437-82B1F23D8896", "selectiontype", "Color Picker", "96D1A8DA-7E63-41FE-8B0A-0E6239828DBA" );
            RockMigrationHelper.AddAttributeQualifier( "CE3F126E-B56A-438A-BA45-8EC8437BB961", "ispassword", "False", "3F4357E7-4644-4CBD-93A8-A98DD1879814" );
            RockMigrationHelper.AddAttributeQualifier( "CE3F126E-B56A-438A-BA45-8EC8437BB961", "maxcharacters", "", "A7F4A5FA-43CF-4B86-B5B0-BE33E98B1C20" );
            RockMigrationHelper.AddAttributeQualifier( "CE3F126E-B56A-438A-BA45-8EC8437BB961", "showcountdown", "False", "20381B01-3B39-4008-83AC-048FF796DF75" );
            RockMigrationHelper.UpdateDefinedValue( "354715FA-564A-420A-8324-0411988AE7AB", "Influential", "How you relate to people.", "840C414E-A261-4243-8302-6117E8949FE4", false );
            RockMigrationHelper.UpdateDefinedValue( "354715FA-564A-420A-8324-0411988AE7AB", "Intellectual", "How your mind operates.", "58FEF15F-561D-420E-8937-6CF51D296F0E", false );
            RockMigrationHelper.UpdateDefinedValue( "354715FA-564A-420A-8324-0411988AE7AB", "Operational", "How you relate to structure.", "84322020-4E27-44EF-88F2-EAFDB7286A01", false );
            RockMigrationHelper.UpdateDefinedValue( "354715FA-564A-420A-8324-0411988AE7AB", "Organizational", "How you lead a team or organization.", "112A35BE-3108-48D9-B057-125A788AB531", false );
            RockMigrationHelper.AddDefinedValueAttributeValue( "112A35BE-3108-48D9-B057-125A788AB531", "07E85FA1-8F86-4414-8DC3-43D303C55457", @"<p>This cluster of motivators concentrates on how you connect to or lead an organization or team with which you are associated. The Organizational Cluster focuses on how you connect within the organization compared to the Operational Cluster which focuses on how you connect based on your role. The motivators in this cluster can be seen in the type of behavior you demonstrate as it relates to the long-term health of the organization or team. What you value in regard to your organization will be directly related to your top motivators from within this cluster. The greater the number of the motivators from this cluster you possess at the top third of your profile, the more dedicated you will be to making an impact on the organization or team.</p>" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "112A35BE-3108-48D9-B057-125A788AB531", "8B5F72E4-5A49-4224-9437-82B1F23D8896", @"#0067cb" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "112A35BE-3108-48D9-B057-125A788AB531", "CE3F126E-B56A-438A-BA45-8EC8437BB961", @"core_MotivatorClusterOrganizational" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "58FEF15F-561D-420E-8937-6CF51D296F0E", "07E85FA1-8F86-4414-8DC3-43D303C55457", @"<p>This cluster of motivators concentrates on how you use your cognitive faculties throughout your life. These motivators can be seen in the way you think or the kind of mental activities you naturally pursue. The way you view your mental activity and the way you think about your thinking will be directly influenced by the motivators in this cluster. Your conversations with yourself and others will be greatly influenced by the motivators from this cluster which are in the top third of your profile.  Others will generally see these motivators through how you talk, what you read or how you use your discretionary time.</p>" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "58FEF15F-561D-420E-8937-6CF51D296F0E", "8B5F72E4-5A49-4224-9437-82B1F23D8896", @"#cb0002" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "58FEF15F-561D-420E-8937-6CF51D296F0E", "CE3F126E-B56A-438A-BA45-8EC8437BB961", @"core_MotivatorClusterIntellectual" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "840C414E-A261-4243-8302-6117E8949FE4", "07E85FA1-8F86-4414-8DC3-43D303C55457", @"<p>This cluster of motivators describes how you relate to others. These motivators can best be seen as the reason you build relationships with people around you. Most of us are motivated to develop connections with others for more than one reason, but the motivators in this cluster will no doubt influence what you value in relationships. The greater the number of the motivators from this cluster you possess at the top third of your profile, the more strongly you will be focused on building healthy relationships for a variety of reasons.</p>" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "840C414E-A261-4243-8302-6117E8949FE4", "8B5F72E4-5A49-4224-9437-82B1F23D8896", @"#00cb64" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "840C414E-A261-4243-8302-6117E8949FE4", "CE3F126E-B56A-438A-BA45-8EC8437BB961", @"core_MotivatorClusterInfluential" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "84322020-4E27-44EF-88F2-EAFDB7286A01", "07E85FA1-8F86-4414-8DC3-43D303C55457", @"<p>This cluster of motivators concentrates on how you execute the role you prefer. While the Organizational Cluster focuses on your connection with the organization the Operational Cluster focuses on your connection to your role. The motivators in this cluster can be seen in the way you carry out the activities of what you do, moment by moment. They dramatically influence what you value and how you spend your time or effort at work or in the tasks you perform throughout the day. When others look at the way you work, your behavior will be greatly influenced by the motivators from this cluster which are in the top third of your profile.</p>" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "84322020-4E27-44EF-88F2-EAFDB7286A01", "8B5F72E4-5A49-4224-9437-82B1F23D8896", @"#cb6400" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "84322020-4E27-44EF-88F2-EAFDB7286A01", "CE3F126E-B56A-438A-BA45-8EC8437BB961", @"core_MotivatorClusterOperational" );
        }

        private void AddMotivatorDefinedType()
        {
            RockMigrationHelper.AddDefinedType( "TrueWiring", "Motivator", "Used by Rock's TrueWiring Motivator assessment to hold all the motivator values.", "1DFF1804-0055-491E-9559-54EA3F8F89D1", @"" );
            RockMigrationHelper.AddDefinedTypeAttribute( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Cluster", "Cluster", "", 1025, "", "A20E6DB1-B830-4D41-9003-43A184E4C910" );
            RockMigrationHelper.AddDefinedTypeAttribute( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Attribute Score Key", "AttributeScoreKey", "", 1022, "", "55FDABC3-22AE-4EE4-9883-8234E3298B99" );
            RockMigrationHelper.AddDefinedTypeAttribute( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "9C204CD0-1233-41C5-818A-C5DA439445AA", "MotivatorId", "MotivatorId", "", 1023, "", "8158A336-8129-4E82-8B61-8C0E883CB91A" );
            RockMigrationHelper.AddDefinedTypeAttribute( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "D747E6AE-C383-4E22-8846-71518E3DD06F", "Color", "Color", "", 1024, "", "9227E7D4-5725-49BD-A0B1-43B769E0A529" );
            RockMigrationHelper.AddAttributeQualifier( "55FDABC3-22AE-4EE4-9883-8234E3298B99", "ispassword", "False", "DFF0DDA7-8467-491A-9D50-849DB09787D4" );
            RockMigrationHelper.AddAttributeQualifier( "55FDABC3-22AE-4EE4-9883-8234E3298B99", "maxcharacters", "", "0215F71D-00E2-41BE-9D69-04375C11CF64" );
            RockMigrationHelper.AddAttributeQualifier( "55FDABC3-22AE-4EE4-9883-8234E3298B99", "showcountdown", "False", "26967EB4-A1A4-4C46-9106-EE985E5BEDFF" );
            RockMigrationHelper.AddAttributeQualifier( "8158A336-8129-4E82-8B61-8C0E883CB91A", "ispassword", "False", "7BE1FF02-DCDB-42DC-891F-60F632915F0B" );
            RockMigrationHelper.AddAttributeQualifier( "8158A336-8129-4E82-8B61-8C0E883CB91A", "maxcharacters", "", "BEAA8022-951F-4A7D-B9E8-0EADF134106B" );
            RockMigrationHelper.AddAttributeQualifier( "8158A336-8129-4E82-8B61-8C0E883CB91A", "showcountdown", "False", "ECD77226-33DC-4184-9DD4-FB9110C38566" );
            RockMigrationHelper.AddAttributeQualifier( "9227E7D4-5725-49BD-A0B1-43B769E0A529", "selectiontype", "Color Picker", "95CCC80C-DCA9-467B-9E42-E3734B6129BC" );
            RockMigrationHelper.AddAttributeQualifier( "A20E6DB1-B830-4D41-9003-43A184E4C910", "allowmultiple", "False", "929F009E-F42F-4893-BD46-34E5945D46BC" );
            RockMigrationHelper.AddAttributeQualifier( "A20E6DB1-B830-4D41-9003-43A184E4C910", "definedtype", "78", "6C084DB5-5EC0-4E73-BAE7-775AE429C852" );
            RockMigrationHelper.AddAttributeQualifier( "A20E6DB1-B830-4D41-9003-43A184E4C910", "displaydescription", "False", "5CA3CB93-B7F0-4C31-8101-0F0AC78AED16" );
            RockMigrationHelper.AddAttributeQualifier( "A20E6DB1-B830-4D41-9003-43A184E4C910", "enhancedselection", "False", "FD29FB8E-E349-4A0C-BF62-856B1AC851E1" );
            RockMigrationHelper.AddAttributeQualifier( "A20E6DB1-B830-4D41-9003-43A184E4C910", "includeInactive", "False", "0008F665-2B5A-49CB-9699-58F72FAC12EF" );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Believing", "This motivator influences you to pursue the principles which you believe in with dogged determination. You have a tremendous capacity to be tenacious in pursuing principles. You clearly know what you believe and are able to articulate these beliefs with others. You have expectations for yourself and others regarding those beliefs. You know that you have formed your beliefs through wise experience, counsel, and judgment. You influence others through your convictions. The challenge of this motivator is that some may see you as a black and white person.", "99F598E0-E0AC-4B4B-BEAF-589D41764EE1", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Caring", "This motivator influences you to pursue meeting the needs of others. You genuinely care for others especially when they are hurting. You have a large capacity for supporting others. Others would see you as empathetic; especially for individuals in difficult situations. You may find it easy to identify with the pain that others experience. You influence others through your care and compassion. The challenge to this motivator is that you may become so consumed in the needs that you miss long term solutions.", "FFD7EF9C-5D68-40D2-A362-416B2D660D51", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Communicating", "This motivator influences you to seek opportunities to speak in a variety of environments. You have the capacity to speak effectively. You enjoy talking and engaging others with what you know. You find it easy to articulate your thoughts and to share them in a credible manner. You are not intimidated by speaking in front of others, and you find pleasure in persuading others to your perspective. You influence others through speaking and convincing others of your position. The challenge to this motivator is you may feel if you talk enough people will understand.", "FA70E27D-6642-4162-AF17-530F66B507E7", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Empowering", "This motivator influences you to equip others to do what they are gifted to do. You have the capacity to equip and release individuals. You enjoy developing relationships with individuals who you can mentor and model leadership. You find it easy to see the strengths of others and also know what they need to develop. You will influence others through your investing in others to do so much more than they could have done without your intentional effort. The challenge to this motivator is you may not always address negative issues in the lives of those you are developing.", "C171D01E-C607-488B-A550-1E341081210B", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Engaging", "This motivator influences you to become deeply involved in the needs of the community. You understand the community, and you have a keen sense of how to meet those needs. You are frustrated when you see community needs go unmet. You have a desire to be involved in various community organizations which are making an impact in your community. You will influence others through your engaging the community needs in real and tangible ways to make a difference. The challenge with this motivator is you may not always see issues which need addressing in your life because you are so focused on the community.", "5635E95B-3A07-43B7-837A-0F131EF1DA97", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Flexing", "This motivator influences you to change quickly when circumstances require it. You have the capacity to be adaptable in diverse situations. You understand when the need for transformation is necessary before others. You enjoy change. You have a desire to be on the front edge of any movement. You will influence others by modifying what is currently not working so that you are better prepared to handle challenges. The challenge with this motivator is that you are so quick to adjust and adapt that you may miss the positive results of perseverance.", "BD5D99E7-E0FF-4535-8B26-BF73EF9B9F89", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Gathering", "This motivator influences you to bring people together.  You have the capacity to engage people, so they want to be around you. You understand what people want in a relationship and are able to meet those needs. You enjoy gathering people around you. You have a desire to influence those who are drawn to you. You will influence others by bringing them onboard with you wherever you go. The challenge with this motivator is that you may enjoy bringing people together more than actually accomplishing something together.", "73087DD2-B892-4367-894F-8922477B2F10", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Growth Propensity", "The Growth Propensity Scale (GPS) measures your perceived mindset on a continuum between a Growth Mindset (higher the number) or Fixed Mindset. Carol Dweck’s book, Mindset*, provides a framework to describe your state of mind in this context. She points to these mindsets as being two ends of a spectrum in how we view our own capacity and potential to grow or change. Most of us have general tendencies toward one type or another and may even display different mindsets in different areas of our lives.  You may display a fixed mindset in one area of your life, or in your life in general, but you are not destined to stay there. We are all products of our past, but we are not prisoners! No matter where your score is on this spectrum, it will influence what you do with the results of the previous twenty-two scales. Let me encourage you to start by reading Carol’s book and invest time in the exercises she outlines.  Remember, you may be a product of the past, but not a prisoner of your past...start changing today!", "605F3702-6AE7-4545-BEBE-23693E60031C", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Innovating", "This motivator influences you to look for new ways to do everything continuously. You have the capacity to see what could be done better. You understand why something isn’t working, and you can figure out how to do it differently. You enjoy finding new ways of doing something and have a desire to create something from nothing. You may be frustrated if something isn’t difficult for you. You will influence others through starting something with a high level of energy which may never have been done before. The challenge with this motivator is that you may enjoy creating so much that there is no execution of a plan to bring innovation to reality.", "D84E58E4-87FC-4CEB-B83E-A2C6D186366C", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Leading", "This motivator influences you to bring others together to accomplish a task. You have the capacity to take responsibility for others to achieve something together. You understand what needs to happen in most situations and can mobilize others to work together to undertake it. You enjoy others following your lead, and you have a desire to make an impact through others working with you. You will influence others by your ability to inspire and engage them to accomplish more together than they could have accomplished individually. The challenge with this motivator is that you may not feel comfortable as “just being one of a team” where you are not the sole leader.", "6A2354C6-3FA4-4BAD-89A8-7359FEC48FE3", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Learning", "This motivator influences you to seek out opportunities to understand new things continually. You have the capacity to continue learning through various media. You understand there is so much more to know about our world, and you feel stagnant if you are not growing in some manner. You have a desire to learn all that you can about everything. Every opportunity is an opportunity to learn and grow. You will influence others through what you are learning in one area and helping them apply it in different areas. The challenge with this motivator is that you may enjoy learning so much that there is little effort to actually do something with what you learn.", "7EA44A56-58CB-4E40-9779-CC0A79772926", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Maximizing", "This motivator influences you to only invest your time, money or effort in areas that can give you a significant rate of return for your investment. You have the capacity to see opportunities where a substantial benefit will result. You understand what is necessary to make an impact using your resources. You enjoy seeing a high return from your investment, and you have a desire to make it even higher. You will influence others through your strategic sense of when and where to invest resources for maximum impact. The challenge with this motivator is that at times you may need to serve others on the team rather than maximizing your own impact.", "3F678404-5844-494F-BDB0-DD9FEEBC98C9", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Organizing", "This motivator influences you to seek opportunities where you can bring disarray under control. You have the capacity to bring order out of disorder. You understand that the “devil is in the details,” so you work on creating systems to maintain control of the details. You enjoy when everything flows as planned. You have a desire to organize various pieces into one coherent whole. You will influence others by bringing disparate fragments together in alignment with your objectives. The challenge with this motivator is that you may be resistance to change because it will bring unwanted chaos.", "85459C0F-65A5-48F9-86F3-40B03F9C53E9", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Pacing", "This motivator influences you to keep a consistent and stable structure in your life and work. You have the capacity to know when your life is getting out of balanced. You understand how much you can handle and what has to change for your work/life balance to achieve healthy stability. You enjoy living in a structured and consistent manner. You desire to create beneficial boundaries in all areas of your life. You will influence others by your modeling and espousing the long-term sustainable margins within life and work. The challenge with this motivator is that you may resist times when an imbalance is required to complete a task with excellence.", "9F771853-2EBA-47A2-9AC5-26EBEA0A3B25", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Perceiving", "This motivator influences to discern in others what is not readily apparent. You have the capacity to observe behaviors in other which is not consistent with what they are saying. You understand when things don’t sometimes turn out to be as they were initially described. You enjoy trying to understand others. You have a desire to find out if your intuitions are correct. You will influence others through your ability to know things that will help the team be more effective in working together. The challenge with this motivator is you may not know what to do with what you are perceiving, so you gossip to others about your insights.", "4C898A5C-B48E-4BAE-AB89-835F25A451BF", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Relating", "This motivator influences you to seek others with whom you can build relationships. You have the capacity to draw people into your sphere of trust and rapport. You understand what others want or need relationally and are able to provide that in a healthy manner. You enjoy the close relationships that develop others. You have a desire to ensure everyone is tied into others through an interpersonal network. You will influence others by building strong ties with others who are socially connected to you. The challenge with this motivator is you may form relationships with so many people that you simply cannot maintain integrity and depth in them all.", "D7F9BDE2-8BEB-469E-BAD9-AA4DEBD3D995", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Serving", "This motivator influences to attend to others or the team for their best, not your own. You have the capacity to work behind the scenes when others are not even aware of what you are doing. You understand what needs to be done long before others. You enjoy doing the little things which may have gone unattended. You have a desire to serve others so they can flourish. You will influence others by helping them so they can function within their primary motivators. The challenge with this motivator is that you and/or others may undervalue your contribution to the group or team.", "D8430EAD-7A38-4AD1-B21A-B2119EE0F1CD", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Thinking", "This motivator influences you to be intentionally aware of your thinking at any given moment. You have the capacity to consciously think about your thinking. You understand what is going on in your mind and why you are thinking the way you are thinking. You enjoy reflecting on why you think as you do. You have a desire to understand the patterns in why others respond as they do based upon what you understand about your thinking. You will influence others by your insight into what others may be thinking in various situations. The challenge with this motivator is you can get lost in your thinking rather than making a decision and moving forward.", "0D82DC77-334C-44B0-84A6-989910907DD4", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Transforming", "This motivator influences you to try to improve organizations which you are a part of. You have the capacity to know what are the crucial changes which need to be made to bring about progress. You understand how to bring people along with the necessary changes, and you enjoy walking with people through those essential changes. You have a desire to help people embrace differences which are best for the team. You will influence others by enabling them to feel comfortable and commit to organizational transformation. The challenge with this motivator is you may experience the discomfort of leaving people behind who do not want to change while feeling the need to keep moving ahead with the change.", "2393C3CE-8E49-46FE-A75B-D5D624A37B49", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Uniting", "This motivator influences you to continually connect people around a common cause. You have the capacity to get people to gain ownership of a vision. You understand how to deal with the individual needs in order for the team to win. You enjoy helping others feel a part of the group. You have a desire to have everyone on the team to feel responsible for the success of the team. You will influence others by creating a sense of belonging for every member of the team. The challenge with this motivator is you may spend so long unifying the team, that it hurts the critical progress or momentum.", "D7601B56-7495-4D7B-A916-8C48F78675E3", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Unwavering", "This motivator influences you to maintain trying to accomplish the goal long after others have given up. You have the capacity to ride the ups and downs of circumstances without being defeated. You understand that all forward progress is accomplished through grit and hard work. You see demanding times as a test of your abilities. You have a desire to succeed in spite of resistance. You will influence others through your resilience and perseverance in difficult times. The challenge with this motivator is you may not walk away from some situations which simply are not worth the effort.", "A027F6B2-56DD-4724-962D-F865606AEAB8", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Venturing", "This motivator influences you to seek out opportunities involving risk and challenge. You have the capacity to thrive in environments which entail some level of risk. You understand how to handle uncertainty in the assignments you pursue. You enjoy the thrill of being challenged to attempt something you have not previously tried. You have a desire to do things that test you. You will influence others through your continued stretching the team to try something new. The challenge with this motivator is you will seldom be satisfied with the status quo and therefore easily bored.", "4D0A1A6D-3F5A-476E-A633-04EAEF457645", false );
            RockMigrationHelper.UpdateDefinedValue( "1DFF1804-0055-491E-9559-54EA3F8F89D1", "Visioning", "This motivator influences you to dream of things which don’t exist yet. You have the capacity to picture what things could be in the future for your team. You understand that you can’t accomplish this by yourself; therefore, you enjoy attracting others to your preferred picture of the future. You have a desire for your organization to be much more than it currently is and want to bring that into reality. You will influence others through your inspiring and encouraging them to see much more than their current reality. The challenge with this motivator is you can tend to live in the future and get frustrated with the realities of the current situation.", "EE1603BA-41AE-4CFA-B220-065768996501", false );
            RockMigrationHelper.AddDefinedValueAttributeValue( "0D82DC77-334C-44B0-84A6-989910907DD4", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorThinking" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "0D82DC77-334C-44B0-84A6-989910907DD4", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F17" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "0D82DC77-334C-44B0-84A6-989910907DD4", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#e50002" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "0D82DC77-334C-44B0-84A6-989910907DD4", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"58fef15f-561d-420e-8937-6cf51d296f0e" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "2393C3CE-8E49-46FE-A75B-D5D624A37B49", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorTransforming" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "2393C3CE-8E49-46FE-A75B-D5D624A37B49", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F18" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "2393C3CE-8E49-46FE-A75B-D5D624A37B49", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#005ab2" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "2393C3CE-8E49-46FE-A75B-D5D624A37B49", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"112a35be-3108-48d9-b057-125a788ab531" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "3F678404-5844-494F-BDB0-DD9FEEBC98C9", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorMaximizing" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "3F678404-5844-494F-BDB0-DD9FEEBC98C9", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F11" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "3F678404-5844-494F-BDB0-DD9FEEBC98C9", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#b25700" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "3F678404-5844-494F-BDB0-DD9FEEBC98C9", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"84322020-4e27-44ef-88f2-eafdb7286a01" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "4C898A5C-B48E-4BAE-AB89-835F25A451BF", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorPerceiving" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "4C898A5C-B48E-4BAE-AB89-835F25A451BF", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F14" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "4C898A5C-B48E-4BAE-AB89-835F25A451BF", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#cb0002" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "4C898A5C-B48E-4BAE-AB89-835F25A451BF", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"58fef15f-561d-420e-8937-6cf51d296f0e" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "4D0A1A6D-3F5A-476E-A633-04EAEF457645", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorVenturing" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "4D0A1A6D-3F5A-476E-A633-04EAEF457645", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F21" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "4D0A1A6D-3F5A-476E-A633-04EAEF457645", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#0081fe" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "4D0A1A6D-3F5A-476E-A633-04EAEF457645", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"112a35be-3108-48d9-b057-125a788ab531" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "5635E95B-3A07-43B7-837A-0F131EF1DA97", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorEngaging" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "5635E95B-3A07-43B7-837A-0F131EF1DA97", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F05" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "5635E95B-3A07-43B7-837A-0F131EF1DA97", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#00984b" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "5635E95B-3A07-43B7-837A-0F131EF1DA97", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"840c414e-a261-4243-8302-6117e8949fe4" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "605F3702-6AE7-4545-BEBE-23693E60031C", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorGrowthPropensity" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "605F3702-6AE7-4545-BEBE-23693E60031C", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"PS01" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "605F3702-6AE7-4545-BEBE-23693E60031C", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#6400cb" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "6A2354C6-3FA4-4BAD-89A8-7359FEC48FE3", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorLeading" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "6A2354C6-3FA4-4BAD-89A8-7359FEC48FE3", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F09" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "6A2354C6-3FA4-4BAD-89A8-7359FEC48FE3", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#004d98" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "6A2354C6-3FA4-4BAD-89A8-7359FEC48FE3", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"112a35be-3108-48d9-b057-125a788ab531" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "73087DD2-B892-4367-894F-8922477B2F10", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorGathering" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "73087DD2-B892-4367-894F-8922477B2F10", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F07" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "73087DD2-B892-4367-894F-8922477B2F10", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#00b257" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "73087DD2-B892-4367-894F-8922477B2F10", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"840c414e-a261-4243-8302-6117e8949fe4" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "7EA44A56-58CB-4E40-9779-CC0A79772926", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorLearning" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "7EA44A56-58CB-4E40-9779-CC0A79772926", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F10" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "7EA44A56-58CB-4E40-9779-CC0A79772926", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#b20002" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "7EA44A56-58CB-4E40-9779-CC0A79772926", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"58fef15f-561d-420e-8937-6cf51d296f0e" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "85459C0F-65A5-48F9-86F3-40B03F9C53E9", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorOrganizing" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "85459C0F-65A5-48F9-86F3-40B03F9C53E9", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F12" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "85459C0F-65A5-48F9-86F3-40B03F9C53E9", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#cb6400" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "85459C0F-65A5-48F9-86F3-40B03F9C53E9", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"84322020-4e27-44ef-88f2-eafdb7286a01" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "99F598E0-E0AC-4B4B-BEAF-589D41764EE1", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorBelieving" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "99F598E0-E0AC-4B4B-BEAF-589D41764EE1", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F01" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "99F598E0-E0AC-4B4B-BEAF-589D41764EE1", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#7f0001" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "99F598E0-E0AC-4B4B-BEAF-589D41764EE1", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"58fef15f-561d-420e-8937-6cf51d296f0e" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "9F771853-2EBA-47A2-9AC5-26EBEA0A3B25", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorPacing" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "9F771853-2EBA-47A2-9AC5-26EBEA0A3B25", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F13" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "9F771853-2EBA-47A2-9AC5-26EBEA0A3B25", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#e57100" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "9F771853-2EBA-47A2-9AC5-26EBEA0A3B25", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"84322020-4e27-44ef-88f2-eafdb7286a01" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "A027F6B2-56DD-4724-962D-F865606AEAB8", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorUnwavering" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "A027F6B2-56DD-4724-962D-F865606AEAB8", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F20" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "A027F6B2-56DD-4724-962D-F865606AEAB8", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#fe7d00" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "A027F6B2-56DD-4724-962D-F865606AEAB8", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"84322020-4e27-44ef-88f2-eafdb7286a01" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "BD5D99E7-E0FF-4535-8B26-BF73EF9B9F89", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorFlexing" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "BD5D99E7-E0FF-4535-8B26-BF73EF9B9F89", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F06" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "BD5D99E7-E0FF-4535-8B26-BF73EF9B9F89", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#7f3e00" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "BD5D99E7-E0FF-4535-8B26-BF73EF9B9F89", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"84322020-4e27-44ef-88f2-eafdb7286a01" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "C171D01E-C607-488B-A550-1E341081210B", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorEmpowering" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "C171D01E-C607-488B-A550-1E341081210B", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F04" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "C171D01E-C607-488B-A550-1E341081210B", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#00407f" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "C171D01E-C607-488B-A550-1E341081210B", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"112a35be-3108-48d9-b057-125a788ab531" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D7601B56-7495-4D7B-A916-8C48F78675E3", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorUniting" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D7601B56-7495-4D7B-A916-8C48F78675E3", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F19" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D7601B56-7495-4D7B-A916-8C48F78675E3", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#00e571" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D7601B56-7495-4D7B-A916-8C48F78675E3", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"840c414e-a261-4243-8302-6117e8949fe4" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D7F9BDE2-8BEB-469E-BAD9-AA4DEBD3D995", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorRelating" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D7F9BDE2-8BEB-469E-BAD9-AA4DEBD3D995", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F15" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D7F9BDE2-8BEB-469E-BAD9-AA4DEBD3D995", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#00cb64" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D7F9BDE2-8BEB-469E-BAD9-AA4DEBD3D995", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"840c414e-a261-4243-8302-6117e8949fe4" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D8430EAD-7A38-4AD1-B21A-B2119EE0F1CD", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorServing" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D8430EAD-7A38-4AD1-B21A-B2119EE0F1CD", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F16" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D8430EAD-7A38-4AD1-B21A-B2119EE0F1CD", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#0074e5" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D8430EAD-7A38-4AD1-B21A-B2119EE0F1CD", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"112a35be-3108-48d9-b057-125a788ab531" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D84E58E4-87FC-4CEB-B83E-A2C6D186366C", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorInnovating" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D84E58E4-87FC-4CEB-B83E-A2C6D186366C", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F08" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D84E58E4-87FC-4CEB-B83E-A2C6D186366C", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#984b00" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "D84E58E4-87FC-4CEB-B83E-A2C6D186366C", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"84322020-4e27-44ef-88f2-eafdb7286a01" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "EE1603BA-41AE-4CFA-B220-065768996501", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorVisioning" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "EE1603BA-41AE-4CFA-B220-065768996501", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F22" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "EE1603BA-41AE-4CFA-B220-065768996501", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#0067cb" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "EE1603BA-41AE-4CFA-B220-065768996501", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"112a35be-3108-48d9-b057-125a788ab531" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "FA70E27D-6642-4162-AF17-530F66B507E7", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorCommunicating" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "FA70E27D-6642-4162-AF17-530F66B507E7", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F03" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "FA70E27D-6642-4162-AF17-530F66B507E7", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#980001" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "FA70E27D-6642-4162-AF17-530F66B507E7", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"58fef15f-561d-420e-8937-6cf51d296f0e" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "FFD7EF9C-5D68-40D2-A362-416B2D660D51", "55FDABC3-22AE-4EE4-9883-8234E3298B99", @"core_MotivatorCaring" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "FFD7EF9C-5D68-40D2-A362-416B2D660D51", "8158A336-8129-4E82-8B61-8C0E883CB91A", @"F02" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "FFD7EF9C-5D68-40D2-A362-416B2D660D51", "9227E7D4-5725-49BD-A0B1-43B769E0A529", @"#007f3e" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "FFD7EF9C-5D68-40D2-A362-416B2D660D51", "A20E6DB1-B830-4D41-9003-43A184E4C910", @"840c414e-a261-4243-8302-6117e8949fe4" );
        }

        /// <summary>
        /// Add security to attributes
        /// </summary>
        private void AddSecurityToAttributes( string attributeGuid )
        {
            RockMigrationHelper.AddSecurityAuthForAttribute(
               attributeGuid,
               0,
               Rock.Security.Authorization.VIEW,
               true,
               Rock.SystemGuid.Group.GROUP_STAFF_MEMBERS,
               ( int ) Rock.Model.SpecialRole.None,
               Guid.NewGuid().ToString()
               );
            RockMigrationHelper.AddSecurityAuthForAttribute(
               attributeGuid,
               1,
               Rock.Security.Authorization.VIEW,
               true,
               Rock.SystemGuid.Group.GROUP_STAFF_LIKE_MEMBERS,
               ( int ) Rock.Model.SpecialRole.None,
               Guid.NewGuid().ToString()
               );
            RockMigrationHelper.AddSecurityAuthForAttribute(
                attributeGuid,
                2,
                Rock.Security.Authorization.VIEW,
                false,
                null,
                ( int ) Rock.Model.SpecialRole.AllUsers,
                Guid.NewGuid().ToString()
             );

            RockMigrationHelper.AddSecurityAuthForAttribute(
               attributeGuid,
               0,
               Rock.Security.Authorization.EDIT,
               true,
               Rock.SystemGuid.Group.GROUP_STAFF_MEMBERS,
               ( int ) Rock.Model.SpecialRole.None,
               Guid.NewGuid().ToString()
               );
            RockMigrationHelper.AddSecurityAuthForAttribute(
               attributeGuid,
               1,
               Rock.Security.Authorization.EDIT,
               true,
               Rock.SystemGuid.Group.GROUP_STAFF_LIKE_MEMBERS,
               ( int ) Rock.Model.SpecialRole.None,
               Guid.NewGuid().ToString()
               );
            RockMigrationHelper.AddSecurityAuthForAttribute(
                attributeGuid,
                2,
                Rock.Security.Authorization.EDIT,
                false,
                null,
                ( int ) Rock.Model.SpecialRole.AllUsers,
                Guid.NewGuid().ToString()
             );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
        }
    }
}
