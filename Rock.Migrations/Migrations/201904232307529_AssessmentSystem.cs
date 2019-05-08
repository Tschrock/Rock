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
namespace Rock.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    /// <summary>
    ///
    /// </summary>
    public partial class AssessmentSystem : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            AlterColumn("dbo.AssessmentType", "Description", c => c.String(nullable: false, maxLength: 100));
            MigrateSystemEmailsUp();
            AssessmentRemindersServiceJobUp();
            CreateRequestAssessmentWorkflow();
            PagesBlocksAndAttributesUp();
            UpdateSpirtualGiftsResultsMessageBlockAttribute();
        }
        
        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            PagesBlocksAndAttributesDown();
            AssessmentRemindersServiceJobDown();
            RockMigrationHelper.DeleteSystemEmail("41FF4269-7B48-40CD-81D4-C11370A13DED"); // Assessment Request System Eamil
            AlterColumn("dbo.AssessmentType", "Description", c => c.String(nullable: false));
        }
        
        /// <summary>
        /// Migrates new system emails up.
        /// Code Generated from Dev Tools\Sql\CodeGen_SystemEmail.sql
        /// </summary>
        private void MigrateSystemEmailsUp()
        {
		            // Assessment Request
            RockMigrationHelper.UpdateSystemEmail( "System", "Assessment Request", "", "", "", "", "", "Assessments Are Ready To Take", @"{% capture assessmentsLink %}{{ 'Global' | Attribute:'PublicApplicationRoot' }}Assessments{% endcapture %}

{{ 'Global' | Attribute:'EmailHeader' }}
{{ Person.NickName }},

<p>
    We're each a unique creation. We'd love to learn more about you through a simple and quick online personality profile. The results of the assessment will help us tailor our ministry to you and can also be used for building healthier teams and groups.
</p>
<p>
    The assessment takes less than ten minutes and will go a long way toward helping us get to know you better. Thanks in advance!
</p>
<p>
    These assessments are ready for you to take:
</p>
<p>
	<div><!--[if mso]>
	  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{ assessmentsLink }}"" style=""height:38px;v-text-anchor:middle;width:175px;"" arcsize=""11%"" strokecolor=""#e76812"" fillcolor=""#ee7624"">
		<w:anchorlock/>
		<center style=""color:#ffffff;font-family:sans-serif;font-size:13px;font-weight:normal;"">Take Assessment</center>
	  </v:roundrect>
	<![endif]--><a href=""{{ assessmentsLink }}""
	style=""background-color:#ee7624;border:1px solid #e76812;border-radius:4px;color:#ffffff;display:inline-block;font-family:sans-serif;font-size:13px;font-weight:normal;line-height:38px;text-align:center;text-decoration:none;width:175px;-webkit-text-size-adjust:none;mso-hide:all;"">Take Assessment</a></div>
</p>
<br />
<br />
{{ 'Global' | Attribute:'EmailFooter' }}", "41FF4269-7B48-40CD-81D4-C11370A13DED");

}

        /// <summary>
        /// Creates the Service Job Send Assessment Reminders.
        /// </summary>
        private void AssessmentRemindersServiceJobUp()
        {
            // add ServiceJob: Send Assessment Reminders
            // Code Generated using Rock\Dev Tools\Sql\CodeGen_ServiceJobWithAttributes_ForAJob.sql
            Sql(@"IF NOT EXISTS( SELECT [Id] FROM [ServiceJob] WHERE [Class] = 'Rock.Jobs.SendAssessmentReminders' AND [Guid] = 'E3F48F24-E9FC-4A93-95B5-DE7BEDB95B99' )
            BEGIN
               INSERT INTO [ServiceJob] (
                  [IsSystem]
                  ,[IsActive]
                  ,[Name]
                  ,[Description]
                  ,[Class]
                  ,[CronExpression]
                  ,[NotificationStatus]
                  ,[Guid] )
               VALUES ( 
                  0
                  ,1
                  ,'Send Assessment Reminders'
                  ,'Sends reminders to persons with pending assessments if the created date/time is less than the calculated cut off date and the last reminder date is greater than the calculated reminder date.'
                  ,'Rock.Jobs.SendAssessmentReminders'
                  ,'0 0 8 1/1 * ? *'
                  ,1
                  ,'E3F48F24-E9FC-4A93-95B5-DE7BEDB95B99'
                  );
            END" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.ServiceJob", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Class", "Rock.Jobs.SendAssessmentReminders", "Reminder Every", "The number of days between reminder emails.", 0, @"7", "635122EA-3694-44A2-B7C3-4C4D19F9873C", "ReminderEveryDays" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.ServiceJob", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Class", "Rock.Jobs.SendAssessmentReminders", "Cut off Days", "The number of days past the initial requested date to stop sending reminders. After this cut-off, reminders will need to be sent manually by a person.", 1, @"60", "F4312D3D-26B6-41C7-9842-0CDD319C747C", "CutoffDays" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.ServiceJob", "08F3003B-F3E2-41EC-BDF1-A2B7AC2908CF", "Class", "Rock.Jobs.SendAssessmentReminders", "Assessment Reminder System Email", "", 2, @"41FF4269-7B48-40CD-81D4-C11370A13DED", "4BF49C30-ED3B-41AF-AF05-BDE2BA2C0056", "AssessmentSystemEmail" );
            RockMigrationHelper.AddAttributeValue( "635122EA-3694-44A2-B7C3-4C4D19F9873C", 48, @"7", "635122EA-3694-44A2-B7C3-4C4D19F9873C" ); // Send Assessment Reminders: Reminder Every
            RockMigrationHelper.AddAttributeValue( "F4312D3D-26B6-41C7-9842-0CDD319C747C", 48, @"60", "F4312D3D-26B6-41C7-9842-0CDD319C747C" ); // Send Assessment Reminders: Cut off Days
            RockMigrationHelper.AddAttributeValue( "4BF49C30-ED3B-41AF-AF05-BDE2BA2C0056", 48, @"41ff4269-7b48-40cd-81d4-c11370a13ded", "4BF49C30-ED3B-41AF-AF05-BDE2BA2C0056" ); // Send Assessment Reminders: Assessment Reminder System Email

        }

        /// <summary>
        /// Removes the Service Job Send Assessment Reminders
        /// </summary>
        private void AssessmentRemindersServiceJobDown()
        {
            // Code Generated using Rock\Dev Tools\Sql\CodeGen_ServiceJobWithAttributes_ForAJob.sql
            RockMigrationHelper.DeleteAttribute("635122EA-3694-44A2-B7C3-4C4D19F9873C"); // Rock.Jobs.SendAssessmentReminders: Reminder Every
            RockMigrationHelper.DeleteAttribute("F4312D3D-26B6-41C7-9842-0CDD319C747C"); // Rock.Jobs.SendAssessmentReminders: Cut off Days
            RockMigrationHelper.DeleteAttribute("4BF49C30-ED3B-41AF-AF05-BDE2BA2C0056"); // Rock.Jobs.SendAssessmentReminders: Assessment Reminder System Email

            // remove ServiceJob: Send Assessment Reminders
            Sql(@"IF EXISTS( SELECT [Id] FROM [ServiceJob] WHERE [Class] = 'Rock.Jobs.SendAssessmentReminders' AND [Guid] = 'E3F48F24-E9FC-4A93-95B5-DE7BEDB95B99' )
            BEGIN
               DELETE [ServiceJob]  WHERE [Guid] = 'E3F48F24-E9FC-4A93-95B5-DE7BEDB95B99';
            END" );

        }

        /// <summary>
        /// Creates the request assessment workflow.
        /// </summary>
        private void CreateRequestAssessmentWorkflow()
        {
            #region FieldTypes

            RockMigrationHelper.UpdateFieldType("Assessment Types","","Rock","Rock.Field.Types.AssessmentTypesFieldType","C263513A-30BE-4823-ABF1-AC12A56F9644");

            #endregion

            #region EntityTypes

            RockMigrationHelper.UpdateEntityType("Rock.Model.Workflow", "3540E9A7-FE30-43A9-8B0A-A372B63DFC93", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActivity", "2CB52ED0-CB06-4D62-9E2C-73B60AFA4C9F", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActionType", "23E3273A-B137-48A3-9AFF-C8DC832DDCA6", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.CompleteWorkflow","EEDA4318-F014-4A46-9C76-4C052EF81AA1",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.CreateAssessmentRequest","7EDCCA06-C539-4B5B-B6E4-400A19655898",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.RunSQL","A41216D6-6FB0-4019-B222-2C29B4519CF4",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SendEmail","66197B01-D1F0-4924-A315-47AD54E030DE",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SendSystemEmail","4487702A-BEAF-4E5A-92AD-71A1AD48DFCE",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetAttributeFromEntity","972F19B9-598B-474B-97A4-50E56E7B59D2",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetAttributeToCurrentPerson","24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetAttributeValue","C789E457-0783-44B3-9D8F-2EBAB5F11110",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.UserEntryForm","486DC4FA-FCBC-425F-90B0-E606DA8A9F68",false,true);
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","DE9CB292-4785-4EA3-976D-3826F91E9E98"); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A","33E6DF69-BDFA-407A-9744-C175B60643AE","Person Attribute","PersonAttribute","The attribute to set to the currently logged in person.",0,@"","BBED8A83-8BB2-4D35-BAFB-05F67DCAD112"); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Person Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","89E9BCED-91AB-47B0-AD52-D78B0B7CB9E8"); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("4487702A-BEAF-4E5A-92AD-71A1AD48DFCE","08F3003B-F3E2-41EC-BDF1-A2B7AC2908CF","System Email","SystemEmail","A system email to send.",0,@"","C879B8B4-574C-4BCE-BC4D-0C7245AF19D4"); // Rock.Workflow.Action.SendSystemEmail:System Email
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("4487702A-BEAF-4E5A-92AD-71A1AD48DFCE","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","BD6978CE-EDBF-45A9-A548-96630DFF52C1"); // Rock.Workflow.Action.SendSystemEmail:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("4487702A-BEAF-4E5A-92AD-71A1AD48DFCE","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Save Communication History","SaveCommunicationHistory","Should a record of this communication be saved to the recipient's profile",2,@"False","9C5436E6-7EF2-4BD4-B87A-D3E980E55DE3"); // Rock.Workflow.Action.SendSystemEmail:Save Communication History
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("4487702A-BEAF-4E5A-92AD-71A1AD48DFCE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","Send To Email Addresses|Attribute Value","Recipient","The email addresses or an attribute that contains the person or email address that email should be sent to. <span class='tip tip-lava'></span>",1,@"","E58E9280-77CF-4DBB-BF66-87F29D0BF707"); // Rock.Workflow.Action.SendSystemEmail:Send To Email Addresses|Attribute Value
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("4487702A-BEAF-4E5A-92AD-71A1AD48DFCE","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","A52C2EBD-D1CC-469F-803C-FF4C5326D456"); // Rock.Workflow.Action.SendSystemEmail:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("486DC4FA-FCBC-425F-90B0-E606DA8A9F68","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","234910F2-A0DB-4D7D-BAF7-83C880EF30AE"); // Rock.Workflow.Action.UserEntryForm:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("486DC4FA-FCBC-425F-90B0-E606DA8A9F68","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","C178113D-7C86-4229-8424-C6D0CF4A7E23"); // Rock.Workflow.Action.UserEntryForm:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","1D0D3794-C210-48A8-8C68-3FBEC08A6BA5","Body","Body","The body of the email that should be sent. <span class='tip tip-lava'></span> <span class='tip tip-html'></span>",4,@"","4D245B9E-6B03-46E7-8482-A51FBA190E4D"); // Rock.Workflow.Action.SendEmail:Body
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","36197160-7D3D-490D-AB42-7E29105AFE91"); // Rock.Workflow.Action.SendEmail:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Save Communication History","SaveCommunicationHistory","Should a record of this communication be saved to the recipient's profile",8,@"False","65E69B78-37D8-4A88-B8AC-71893D2F75EF"); // Rock.Workflow.Action.SendEmail:Save Communication History
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","33E6DF69-BDFA-407A-9744-C175B60643AE","Attachment One","AttachmentOne","Workflow attribute that contains the email attachment. Note file size that can be sent is limited by both the sending and receiving email services typically 10 - 25 MB.",5,@"","C2C7DA55-3018-4645-B9EE-4BCD11855F2C"); // Rock.Workflow.Action.SendEmail:Attachment One
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","33E6DF69-BDFA-407A-9744-C175B60643AE","Attachment Three","AttachmentThree","Workflow attribute that contains the email attachment. Note file size that can be sent is limited by both the sending and receiving email services typically 10 - 25 MB.",7,@"","A059767A-5592-4926-948A-1065AF4E9748"); // Rock.Workflow.Action.SendEmail:Attachment Three
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","33E6DF69-BDFA-407A-9744-C175B60643AE","Attachment Two","AttachmentTwo","Workflow attribute that contains the email attachment. Note file size that can be sent is limited by both the sending and receiving email services typically 10 - 25 MB.",6,@"","FFD9193A-451F-40E6-9776-74D5DCAC1450"); // Rock.Workflow.Action.SendEmail:Attachment Two
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","33E6DF69-BDFA-407A-9744-C175B60643AE","Send to Group Role","GroupRole","An optional Group Role attribute to limit recipients to if the 'Send to Email Address' is a group or security role.",2,@"","D43C2686-7E02-4A70-8D99-3BCD8ECAFB2F"); // Rock.Workflow.Action.SendEmail:Send to Group Role
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","From Email Address|Attribute Value","From","The email address or an attribute that contains the person or email address that email should be sent from (will default to organization email). <span class='tip tip-lava'></span>",0,@"","9F5F7CEC-F369-4FDF-802A-99074CE7A7FC"); // Rock.Workflow.Action.SendEmail:From Email Address|Attribute Value
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","Send To Email Addresses|Attribute Value","To","The email addresses or an attribute that contains the person or email address that email should be sent to. <span class='tip tip-lava'></span>",1,@"","0C4C13B8-7076-4872-925A-F950886B5E16"); // Rock.Workflow.Action.SendEmail:Send To Email Addresses|Attribute Value
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","9C204CD0-1233-41C5-818A-C5DA439445AA","Subject","Subject","The subject that should be used when sending email. <span class='tip tip-lava'></span>",3,@"","5D9B13B6-CD96-4C7C-86FA-4512B9D28386"); // Rock.Workflow.Action.SendEmail:Subject
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","D1269254-C15A-40BD-B784-ADCC231D3950"); // Rock.Workflow.Action.SendEmail:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("7EDCCA06-C539-4B5B-B6E4-400A19655898","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","D686BDFF-03C8-4F7C-A3FC-89C42DF74714"); // Rock.Workflow.Action.CreateAssessmentRequest:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("7EDCCA06-C539-4B5B-B6E4-400A19655898","33E6DF69-BDFA-407A-9744-C175B60643AE","Assessment Types","AssessmentTypesKey","The attribute that contains the selected list of assessments being requested.",0,@"","B672E4D0-14DE-424A-BC38-7A91F5385A18"); // Rock.Workflow.Action.CreateAssessmentRequest:Assessment Types
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("7EDCCA06-C539-4B5B-B6E4-400A19655898","33E6DF69-BDFA-407A-9744-C175B60643AE","Due Date","DueDate","The attribute that contains the Due Date (if any) for the requests.",2,@"","1010F5DA-565B-4A86-B5C6-E5CE4C26F330"); // Rock.Workflow.Action.CreateAssessmentRequest:Due Date
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("7EDCCA06-C539-4B5B-B6E4-400A19655898","33E6DF69-BDFA-407A-9744-C175B60643AE","Person","Person","The attribute containing the person being requested to take the assessment(s).",1,@"","9E2360BE-4C22-4817-8D2B-5796426D6192"); // Rock.Workflow.Action.CreateAssessmentRequest:Person
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("7EDCCA06-C539-4B5B-B6E4-400A19655898","33E6DF69-BDFA-407A-9744-C175B60643AE","Requested By","RequestedBy","The attribute containing the person requesting the test be taken.",2,@"","5494809A-B0CC-44D9-BFD9-B60D514D020F"); // Rock.Workflow.Action.CreateAssessmentRequest:Requested By
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("7EDCCA06-C539-4B5B-B6E4-400A19655898","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","46FB6CFC-28A3-4822-94BB-B01B8F2D5ED3"); // Rock.Workflow.Action.CreateAssessmentRequest:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("972F19B9-598B-474B-97A4-50E56E7B59D2","1D0D3794-C210-48A8-8C68-3FBEC08A6BA5","Lava Template","LavaTemplate","By default this action will set the attribute value equal to the guid (or id) of the entity that was passed in for processing. If you include a lava template here, the action will instead set the attribute value to the output of this template. The mergefield to use for the entity is 'Entity.' For example, use {{ Entity.Name }} if the entity has a Name property. <span class='tip tip-lava'></span>",4,@"","00D8331D-3055-4531-B374-6B98A9A71D70"); // Rock.Workflow.Action.SetAttributeFromEntity:Lava Template
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("972F19B9-598B-474B-97A4-50E56E7B59D2","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","9392E3D7-A28B-4CD8-8B03-5E147B102EF1"); // Rock.Workflow.Action.SetAttributeFromEntity:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("972F19B9-598B-474B-97A4-50E56E7B59D2","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Entity Is Required","EntityIsRequired","Should an error be returned if the entity is missing or not a valid entity type?",2,@"True","DDEFB300-0A4F-4086-99BE-A32761928F5E"); // Rock.Workflow.Action.SetAttributeFromEntity:Entity Is Required
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("972F19B9-598B-474B-97A4-50E56E7B59D2","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Use Id instead of Guid","UseId","Most entity attribute field types expect the Guid of the entity (which is used by default). Select this option if the entity's Id should be used instead (should be rare).",3,@"False","1246C53A-FD92-4E08-ABDE-9A6C37E70C7B"); // Rock.Workflow.Action.SetAttributeFromEntity:Use Id instead of Guid
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("972F19B9-598B-474B-97A4-50E56E7B59D2","33E6DF69-BDFA-407A-9744-C175B60643AE","Attribute","Attribute","The attribute to set the value of.",1,@"","61E6E1BC-E657-4F00-B2E9-769AAA25B9F7"); // Rock.Workflow.Action.SetAttributeFromEntity:Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("972F19B9-598B-474B-97A4-50E56E7B59D2","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","AD4EFAC4-E687-43DF-832F-0DC3856ABABB"); // Rock.Workflow.Action.SetAttributeFromEntity:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4","1D0D3794-C210-48A8-8C68-3FBEC08A6BA5","SQLQuery","SQLQuery","The SQL query to run. <span class='tip tip-lava'></span>",0,@"","F3B9908B-096F-460B-8320-122CF046D1F9"); // Rock.Workflow.Action.RunSQL:SQLQuery
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","A18C3143-0586-4565-9F36-E603BC674B4E"); // Rock.Workflow.Action.RunSQL:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Continue On Error","ContinueOnError","Should processing continue even if SQL Error occurs?",3,@"False","9A567F6A-2A77-4ECD-80F7-BBD7D54E843C"); // Rock.Workflow.Action.RunSQL:Continue On Error
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4","33E6DF69-BDFA-407A-9744-C175B60643AE","Result Attribute","ResultAttribute","An optional attribute to set to the scaler result of SQL query.",2,@"","56997192-2545-4EA1-B5B2-313B04588984"); // Rock.Workflow.Action.RunSQL:Result Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4","73B02051-0D38-4AD9-BF81-A2D477DE4F70","Parameters","Parameters","The parameters to supply to the SQL query. <span class='tip tip-lava'></span>",1,@"","EA9A026A-934F-4920-97B1-9734795127ED"); // Rock.Workflow.Action.RunSQL:Parameters
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","FA7C685D-8636-41EF-9998-90FFF3998F76"); // Rock.Workflow.Action.RunSQL:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("C789E457-0783-44B3-9D8F-2EBAB5F11110","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","D7EAA859-F500-4521-9523-488B12EAA7D2"); // Rock.Workflow.Action.SetAttributeValue:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("C789E457-0783-44B3-9D8F-2EBAB5F11110","33E6DF69-BDFA-407A-9744-C175B60643AE","Attribute","Attribute","The attribute to set the value of.",0,@"","44A0B977-4730-4519-8FF6-B0A01A95B212"); // Rock.Workflow.Action.SetAttributeValue:Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("C789E457-0783-44B3-9D8F-2EBAB5F11110","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","Text Value|Attribute Value","Value","The text or attribute to set the value from. <span class='tip tip-lava'></span>",1,@"","E5272B11-A2B8-49DC-860D-8D574E2BC15C"); // Rock.Workflow.Action.SetAttributeValue:Text Value|Attribute Value
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("C789E457-0783-44B3-9D8F-2EBAB5F11110","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","57093B41-50ED-48E5-B72B-8829E62704C8"); // Rock.Workflow.Action.SetAttributeValue:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","0CA0DDEF-48EF-4ABC-9822-A05E225DE26C"); // Rock.Workflow.Action.CompleteWorkflow:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","Status|Status Attribute","Status","The status to set the workflow to when marking the workflow complete. <span class='tip tip-lava'></span>",0,@"Completed","07CB7DBC-236D-4D38-92A4-47EE448BA89A"); // Rock.Workflow.Action.CompleteWorkflow:Status|Status Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","25CAD4BE-5A00-409D-9BAB-E32518D89956"); // Rock.Workflow.Action.CompleteWorkflow:Order

            #endregion

            #region Categories

            RockMigrationHelper.UpdateCategory("C9F3C4A5-1526-474D-803F-D6C7A45CBBAE","Data Integrity","fa fa-magic","","BBAE05FD-8192-4616-A71E-903A927E0D90",0); // Data Integrity

            #endregion

            #region Request Assessment

            RockMigrationHelper.UpdateWorkflowType(false,true,"Request Assessment","","BBAE05FD-8192-4616-A71E-903A927E0D90","Work","fa fa-list-ol",0,true,0,"31DDC001-C91A-4418-B375-CAB1475F7A62",0); // Request Assessment
            RockMigrationHelper.UpdateWorkflowTypeAttribute("31DDC001-C91A-4418-B375-CAB1475F7A62","C263513A-30BE-4823-ABF1-AC12A56F9644","Assessments To Take","AssessmentsToTake","The assessments to take.",0,@"","69E8513A-D9E4-4C98-B938-48B1B24F9C08", false); // Request Assessment:Assessments To Take
            RockMigrationHelper.UpdateWorkflowTypeAttribute("31DDC001-C91A-4418-B375-CAB1475F7A62","E4EAB7B2-0B76-429B-AFE4-AD86D7428C70","Person","Person","The person who will take the assessments",1,@"","A201EB57-0AD0-4B98-AD44-9D3A7C0F16BA", false); // Request Assessment:Person
            RockMigrationHelper.UpdateWorkflowTypeAttribute("31DDC001-C91A-4418-B375-CAB1475F7A62","E4EAB7B2-0B76-429B-AFE4-AD86D7428C70","Requested By","RequestedBy","The person who requested the assessments.",2,@"","66B8DEC5-1B55-4AD1-8E4B-C719279A1947", false); // Request Assessment:Requested By
            RockMigrationHelper.UpdateWorkflowTypeAttribute("31DDC001-C91A-4418-B375-CAB1475F7A62","6B6AA175-4758-453F-8D83-FCD8044B5F36","Due Date","DueDate","When all the selected assessments should be completed.",3,@"","7FB54D8C-B6FC-4864-9F14-EEC155CF6D4C", false); // Request Assessment:Due Date
            RockMigrationHelper.UpdateWorkflowTypeAttribute("31DDC001-C91A-4418-B375-CAB1475F7A62","9C204CD0-1233-41C5-818A-C5DA439445AA","No Email Warning","NoEmailWarning","Warning message when the person does not have an email address.",4,@"","B13D6F19-1436-4689-B644-FB70805C255B", false); // Request Assessment:No Email Warning
            RockMigrationHelper.UpdateWorkflowTypeAttribute("31DDC001-C91A-4418-B375-CAB1475F7A62","C28C7BF3-A552-4D77-9408-DEDCF760CED0","Custom Message","CustomMessage","A custom message you would like to include in the request.  Otherwise the default will be used.",5,@"We're each a unique creation. We'd love to learn more about you through a simple and quick online personality profile. The results of the assessment will help us tailor our ministry to you and can also be used for building healthier teams and groups.

The assessment takes less than ten minutes and will go a long way toward helping us get to know you better. Thanks in advance!","DBFB3F53-7AE1-4923-A286-3D69B60BA639", false); // Request Assessment:Custom Message
            RockMigrationHelper.AddAttributeQualifier("69E8513A-D9E4-4C98-B938-48B1B24F9C08","includeInactive",@"False","8FD003E0-9FBD-407F-86A8-FC72E5BAE552"); // Request Assessment:Assessments To Take:includeInactive
            RockMigrationHelper.AddAttributeQualifier("69E8513A-D9E4-4C98-B938-48B1B24F9C08","repeatColumns",@"","B0FDE78C-7E2D-42B2-857E-FF96D3AE1C3C"); // Request Assessment:Assessments To Take:repeatColumns
            RockMigrationHelper.AddAttributeQualifier("A201EB57-0AD0-4B98-AD44-9D3A7C0F16BA","EnableSelfSelection",@"False","80BC8D0A-A585-457E-89C9-FB171C464060"); // Request Assessment:Person:EnableSelfSelection
            RockMigrationHelper.AddAttributeQualifier("66B8DEC5-1B55-4AD1-8E4B-C719279A1947","EnableSelfSelection",@"False","0E1769ED-811C-49A2-BE9C-E7A1D6772F3B"); // Request Assessment:Requested By:EnableSelfSelection
            RockMigrationHelper.AddAttributeQualifier("7FB54D8C-B6FC-4864-9F14-EEC155CF6D4C","datePickerControlType",@"Date Picker","D47E8D70-47E1-46B1-B319-0C0BFBF5CD62"); // Request Assessment:Due Date:datePickerControlType
            RockMigrationHelper.AddAttributeQualifier("7FB54D8C-B6FC-4864-9F14-EEC155CF6D4C","displayCurrentOption",@"False","7F4653B4-1155-4BF4-A1B6-1F3B0B6F49A5"); // Request Assessment:Due Date:displayCurrentOption
            RockMigrationHelper.AddAttributeQualifier("7FB54D8C-B6FC-4864-9F14-EEC155CF6D4C","displayDiff",@"False","228A9BD3-1BE5-4873-92D3-AB44591F2414"); // Request Assessment:Due Date:displayDiff
            RockMigrationHelper.AddAttributeQualifier("7FB54D8C-B6FC-4864-9F14-EEC155CF6D4C","format",@"","2ECC60A7-6D8C-4268-BC37-3C6A85E9F182"); // Request Assessment:Due Date:format
            RockMigrationHelper.AddAttributeQualifier("7FB54D8C-B6FC-4864-9F14-EEC155CF6D4C","futureYearCount",@"","0EFFF1E6-D874-4F00-BB90-1E49EA5B0411"); // Request Assessment:Due Date:futureYearCount
            RockMigrationHelper.AddAttributeQualifier("B13D6F19-1436-4689-B644-FB70805C255B","ispassword",@"False","5D5AA32F-C81E-4A26-9C25-ADA8653A9A47"); // Request Assessment:No Email Warning:ispassword
            RockMigrationHelper.AddAttributeQualifier("B13D6F19-1436-4689-B644-FB70805C255B","maxcharacters",@"","836050D6-3053-47EA-9E53-59AE03D967C9"); // Request Assessment:No Email Warning:maxcharacters
            RockMigrationHelper.AddAttributeQualifier("B13D6F19-1436-4689-B644-FB70805C255B","showcountdown",@"False","D6933BFF-E9A9-45EF-B59D-0CFCAEC331E3"); // Request Assessment:No Email Warning:showcountdown
            RockMigrationHelper.AddAttributeQualifier("DBFB3F53-7AE1-4923-A286-3D69B60BA639","allowhtml",@"False","6E365CCA-06EF-4165-925B-3EFB94F5472B"); // Request Assessment:Custom Message:allowhtml
            RockMigrationHelper.AddAttributeQualifier("DBFB3F53-7AE1-4923-A286-3D69B60BA639","maxcharacters",@"","11E73FDC-AD71-4C62-8CB2-F380D3E153BC"); // Request Assessment:Custom Message:maxcharacters
            RockMigrationHelper.AddAttributeQualifier("DBFB3F53-7AE1-4923-A286-3D69B60BA639","numberofrows",@"6","24411F53-15A6-4E70-8BD6-13CC6FB2A7C8"); // Request Assessment:Custom Message:numberofrows
            RockMigrationHelper.AddAttributeQualifier("DBFB3F53-7AE1-4923-A286-3D69B60BA639","showcountdown",@"False","C04C3BCB-B37C-4120-A5F1-188BEBE81C5D"); // Request Assessment:Custom Message:showcountdown
            RockMigrationHelper.UpdateWorkflowActivityType("31DDC001-C91A-4418-B375-CAB1475F7A62",true,"Launch From Person Profile","When this workflow is initiated from the Person Profile page, the \"Entity\" will have a value so the first action will run successfully, and the workflow will then be persisted.",true,0,"41C1D8A6-570C-49D2-A818-08F631FCDBAD"); // Request Assessment:Launch From Person Profile
            RockMigrationHelper.UpdateWorkflowActivityType("31DDC001-C91A-4418-B375-CAB1475F7A62",true,"Save And Send","",false,1,"88373EA3-CF09-4B5C-8582-73F331CD1FB4"); // Request Assessment:Save And Send
            RockMigrationHelper.UpdateWorkflowActionForm(@"{{ Workflow | Attribute:'NoEmailWarning' }}

Assign assessments to {{ Workflow | Attribute: 'Person' }}.",@"","Send^fdc397cd-8b4a-436e-bea1-bce2e6717c03^88373EA3-CF09-4B5C-8582-73F331CD1FB4^Assessment(s) have been sent successfully.|","88C7D1CC-3478-4562-A301-AE7D4D7FFF6D",true,"","A56DA6B0-60A1-4998-B3F0-6BFA6F342167"); // Request Assessment:Launch From Person Profile:User Entry
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("A56DA6B0-60A1-4998-B3F0-6BFA6F342167","69E8513A-D9E4-4C98-B938-48B1B24F9C08",0,true,false,true,false,@"",@"","86E38BD3-21C8-493F-8A5C-FBAEEBB9AEE8"); // Request Assessment:Launch From Person Profile:User Entry:Assessments To Take
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("A56DA6B0-60A1-4998-B3F0-6BFA6F342167","A201EB57-0AD0-4B98-AD44-9D3A7C0F16BA",3,false,true,false,false,@"",@"","8608396B-E634-4E5C-91C7-DECABC22CD56"); // Request Assessment:Launch From Person Profile:User Entry:Person
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("A56DA6B0-60A1-4998-B3F0-6BFA6F342167","66B8DEC5-1B55-4AD1-8E4B-C719279A1947",4,false,true,false,false,@"",@"","961BF92D-6FCB-461F-BA9A-41E7D0CE2205"); // Request Assessment:Launch From Person Profile:User Entry:Requested By
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("A56DA6B0-60A1-4998-B3F0-6BFA6F342167","7FB54D8C-B6FC-4864-9F14-EEC155CF6D4C",2,true,false,false,false,@"",@"","FF867549-A68F-45A7-8495-4AA102FA586B"); // Request Assessment:Launch From Person Profile:User Entry:Due Date
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("A56DA6B0-60A1-4998-B3F0-6BFA6F342167","B13D6F19-1436-4689-B644-FB70805C255B",5,false,true,false,false,@"",@"","DA087B2B-986B-4721-A712-A013D403F357"); // Request Assessment:Launch From Person Profile:User Entry:No Email Warning
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("A56DA6B0-60A1-4998-B3F0-6BFA6F342167","DBFB3F53-7AE1-4923-A286-3D69B60BA639",1,true,false,false,false,@"",@"","18048430-8A97-40CC-8186-CA0C16A912EC"); // Request Assessment:Launch From Person Profile:User Entry:Custom Message
            RockMigrationHelper.UpdateWorkflowActionType("41C1D8A6-570C-49D2-A818-08F631FCDBAD","Set Person",0,"972F19B9-598B-474B-97A4-50E56E7B59D2",true,false,"","",1,"","B5ED3032-8B0B-4ADC-A2A1-522F8C0086CF"); // Request Assessment:Launch From Person Profile:Set Person
            RockMigrationHelper.UpdateWorkflowActionType("41C1D8A6-570C-49D2-A818-08F631FCDBAD","Set Requested By",1,"24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A",true,false,"","",1,"","6A48CC08-D9AE-4508-A270-FDC7343F461B"); // Request Assessment:Launch From Person Profile:Set Requested By
            RockMigrationHelper.UpdateWorkflowActionType("41C1D8A6-570C-49D2-A818-08F631FCDBAD","Set No Email Warning",2,"A41216D6-6FB0-4019-B222-2C29B4519CF4",true,false,"","",1,"","D947CBE0-437A-4FCE-8898-0141CC03ACEC"); // Request Assessment:Launch From Person Profile:Set No Email Warning
            RockMigrationHelper.UpdateWorkflowActionType("41C1D8A6-570C-49D2-A818-08F631FCDBAD","Set No Email Warning Message",3,"C789E457-0783-44B3-9D8F-2EBAB5F11110",true,false,"","B13D6F19-1436-4689-B644-FB70805C255B",1,"False","215FC2CD-AC85-46C7-9B03-826DDF72923D"); // Request Assessment:Launch From Person Profile:Set No Email Warning Message
            RockMigrationHelper.UpdateWorkflowActionType("41C1D8A6-570C-49D2-A818-08F631FCDBAD","User Entry",4,"486DC4FA-FCBC-425F-90B0-E606DA8A9F68",true,true,"A56DA6B0-60A1-4998-B3F0-6BFA6F342167","",1,"","E9C27804-A31D-4121-A963-9F52BEAE7404"); // Request Assessment:Launch From Person Profile:User Entry
            RockMigrationHelper.UpdateWorkflowActionType("88373EA3-CF09-4B5C-8582-73F331CD1FB4","Save Assessment Requests",0,"7EDCCA06-C539-4B5B-B6E4-400A19655898",true,false,"","",1,"","E871D1D7-04DB-472D-9DF4-6C0389FE2FFC"); // Request Assessment:Save And Send:Save Assessment Requests
            RockMigrationHelper.UpdateWorkflowActionType("88373EA3-CF09-4B5C-8582-73F331CD1FB4","Send Assessment System Email",1,"4487702A-BEAF-4E5A-92AD-71A1AD48DFCE",true,false,"","DBFB3F53-7AE1-4923-A286-3D69B60BA639",32,"","41E75458-C86C-430C-B7C8-4189419D69C6"); // Request Assessment:Save And Send:Send Assessment System Email
            RockMigrationHelper.UpdateWorkflowActionType("88373EA3-CF09-4B5C-8582-73F331CD1FB4","Send Assessment Custom Message Email",2,"66197B01-D1F0-4924-A315-47AD54E030DE",true,false,"","DBFB3F53-7AE1-4923-A286-3D69B60BA639",64,"","C8DE19DB-106A-4AFA-9069-C420B45B976C"); // Request Assessment:Save And Send:Send Assessment Custom Message Email
            RockMigrationHelper.UpdateWorkflowActionType("88373EA3-CF09-4B5C-8582-73F331CD1FB4","Workflow Complete",3,"EEDA4318-F014-4A46-9C76-4C052EF81AA1",true,true,"","",1,"","8F2AC810-83E1-48D2-B306-8F396539343D"); // Request Assessment:Save And Send:Workflow Complete
            RockMigrationHelper.AddActionTypeAttributeValue("B5ED3032-8B0B-4ADC-A2A1-522F8C0086CF","9392E3D7-A28B-4CD8-8B03-5E147B102EF1",@"False"); // Request Assessment:Launch From Person Profile:Set Person:Active
            RockMigrationHelper.AddActionTypeAttributeValue("B5ED3032-8B0B-4ADC-A2A1-522F8C0086CF","61E6E1BC-E657-4F00-B2E9-769AAA25B9F7",@"a201eb57-0ad0-4b98-ad44-9d3a7c0f16ba"); // Request Assessment:Launch From Person Profile:Set Person:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("B5ED3032-8B0B-4ADC-A2A1-522F8C0086CF","DDEFB300-0A4F-4086-99BE-A32761928F5E",@"True"); // Request Assessment:Launch From Person Profile:Set Person:Entity Is Required
            RockMigrationHelper.AddActionTypeAttributeValue("B5ED3032-8B0B-4ADC-A2A1-522F8C0086CF","1246C53A-FD92-4E08-ABDE-9A6C37E70C7B",@"False"); // Request Assessment:Launch From Person Profile:Set Person:Use Id instead of Guid
            RockMigrationHelper.AddActionTypeAttributeValue("6A48CC08-D9AE-4508-A270-FDC7343F461B","DE9CB292-4785-4EA3-976D-3826F91E9E98",@"False"); // Request Assessment:Launch From Person Profile:Set Requested By:Active
            RockMigrationHelper.AddActionTypeAttributeValue("6A48CC08-D9AE-4508-A270-FDC7343F461B","BBED8A83-8BB2-4D35-BAFB-05F67DCAD112",@"66b8dec5-1b55-4ad1-8e4b-c719279a1947"); // Request Assessment:Launch From Person Profile:Set Requested By:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("D947CBE0-437A-4FCE-8898-0141CC03ACEC","F3B9908B-096F-460B-8320-122CF046D1F9",@"DECLARE @PersonAliasGuid uniqueidentifier = '{{ Workflow | Attribute:'Person','RawValue' }}'

SELECT  CASE
    WHEN EXISTS ( SELECT 1
        FROM [Person] P
        INNER JOIN [PersonAlias] PA ON PA.[Guid] = @PersonAliasGuid AND P.[Id] = PA.[PersonId]
        WHERE P.[IsEmailActive] <> 0 AND P.[Email] IS NOT NULL AND P.[Email] != '' )
    THEN ''
    ELSE 'False'
    END"); // Request Assessment:Launch From Person Profile:Set No Email Warning:SQLQuery
            RockMigrationHelper.AddActionTypeAttributeValue("D947CBE0-437A-4FCE-8898-0141CC03ACEC","A18C3143-0586-4565-9F36-E603BC674B4E",@"False"); // Request Assessment:Launch From Person Profile:Set No Email Warning:Active
            RockMigrationHelper.AddActionTypeAttributeValue("D947CBE0-437A-4FCE-8898-0141CC03ACEC","56997192-2545-4EA1-B5B2-313B04588984",@"b13d6f19-1436-4689-b644-fb70805c255b"); // Request Assessment:Launch From Person Profile:Set No Email Warning:Result Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("D947CBE0-437A-4FCE-8898-0141CC03ACEC","9A567F6A-2A77-4ECD-80F7-BBD7D54E843C",@"False"); // Request Assessment:Launch From Person Profile:Set No Email Warning:Continue On Error
            RockMigrationHelper.AddActionTypeAttributeValue("215FC2CD-AC85-46C7-9B03-826DDF72923D","D7EAA859-F500-4521-9523-488B12EAA7D2",@"False"); // Request Assessment:Launch From Person Profile:Set No Email Warning Message:Active
            RockMigrationHelper.AddActionTypeAttributeValue("215FC2CD-AC85-46C7-9B03-826DDF72923D","44A0B977-4730-4519-8FF6-B0A01A95B212",@"b13d6f19-1436-4689-b644-fb70805c255b"); // Request Assessment:Launch From Person Profile:Set No Email Warning Message:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("215FC2CD-AC85-46C7-9B03-826DDF72923D","E5272B11-A2B8-49DC-860D-8D574E2BC15C",@"<div class=""alert alert-warning margin-t-sm"">{{ Workflow | Attribute:'Person' }} does not have an active email address. Please add an address to their record.</div>"); // Request Assessment:Launch From Person Profile:Set No Email Warning Message:Text Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue("E9C27804-A31D-4121-A963-9F52BEAE7404","234910F2-A0DB-4D7D-BAF7-83C880EF30AE",@"False"); // Request Assessment:Launch From Person Profile:User Entry:Active
            RockMigrationHelper.AddActionTypeAttributeValue("E871D1D7-04DB-472D-9DF4-6C0389FE2FFC","D686BDFF-03C8-4F7C-A3FC-89C42DF74714",@"False"); // Request Assessment:Save And Send:Save Assessment Requests:Active
            RockMigrationHelper.AddActionTypeAttributeValue("E871D1D7-04DB-472D-9DF4-6C0389FE2FFC","B672E4D0-14DE-424A-BC38-7A91F5385A18",@"69e8513a-d9e4-4c98-b938-48b1b24f9c08"); // Request Assessment:Save And Send:Save Assessment Requests:Assessment Types
            RockMigrationHelper.AddActionTypeAttributeValue("E871D1D7-04DB-472D-9DF4-6C0389FE2FFC","9E2360BE-4C22-4817-8D2B-5796426D6192",@"a201eb57-0ad0-4b98-ad44-9d3a7c0f16ba"); // Request Assessment:Save And Send:Save Assessment Requests:Person
            RockMigrationHelper.AddActionTypeAttributeValue("E871D1D7-04DB-472D-9DF4-6C0389FE2FFC","5494809A-B0CC-44D9-BFD9-B60D514D020F",@"66b8dec5-1b55-4ad1-8e4b-c719279a1947"); // Request Assessment:Save And Send:Save Assessment Requests:Requested By
            RockMigrationHelper.AddActionTypeAttributeValue("E871D1D7-04DB-472D-9DF4-6C0389FE2FFC","1010F5DA-565B-4A86-B5C6-E5CE4C26F330",@"7fb54d8c-b6fc-4864-9f14-eec155cf6d4c"); // Request Assessment:Save And Send:Save Assessment Requests:Due Date
            RockMigrationHelper.AddActionTypeAttributeValue("41E75458-C86C-430C-B7C8-4189419D69C6","C879B8B4-574C-4BCE-BC4D-0C7245AF19D4",@"41ff4269-7b48-40cd-81d4-c11370a13ded"); // Request Assessment:Save And Send:Send Assessment System Email:System Email
            RockMigrationHelper.AddActionTypeAttributeValue("41E75458-C86C-430C-B7C8-4189419D69C6","BD6978CE-EDBF-45A9-A548-96630DFF52C1",@"False"); // Request Assessment:Save And Send:Send Assessment System Email:Active
            RockMigrationHelper.AddActionTypeAttributeValue("41E75458-C86C-430C-B7C8-4189419D69C6","E58E9280-77CF-4DBB-BF66-87F29D0BF707",@"a201eb57-0ad0-4b98-ad44-9d3a7c0f16ba"); // Request Assessment:Save And Send:Send Assessment System Email:Send To Email Addresses|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue("41E75458-C86C-430C-B7C8-4189419D69C6","9C5436E6-7EF2-4BD4-B87A-D3E980E55DE3",@"False"); // Request Assessment:Save And Send:Send Assessment System Email:Save Communication History
            RockMigrationHelper.AddActionTypeAttributeValue("C8DE19DB-106A-4AFA-9069-C420B45B976C","36197160-7D3D-490D-AB42-7E29105AFE91",@"False"); // Request Assessment:Save And Send:Send Assessment Custom Message Email:Active
            RockMigrationHelper.AddActionTypeAttributeValue("C8DE19DB-106A-4AFA-9069-C420B45B976C","0C4C13B8-7076-4872-925A-F950886B5E16",@"a201eb57-0ad0-4b98-ad44-9d3a7c0f16ba"); // Request Assessment:Save And Send:Send Assessment Custom Message Email:Send To Email Addresses|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue("C8DE19DB-106A-4AFA-9069-C420B45B976C","5D9B13B6-CD96-4C7C-86FA-4512B9D28386",@"Assessments Are Ready To Take"); // Request Assessment:Save And Send:Send Assessment Custom Message Email:Subject
            RockMigrationHelper.AddActionTypeAttributeValue("C8DE19DB-106A-4AFA-9069-C420B45B976C","4D245B9E-6B03-46E7-8482-A51FBA190E4D",@"{% capture assessmentsLink %}{{ 'Global' | Attribute:'PublicApplicationRoot' }}assessments{% endcapture %}

{{ 'Global' | Attribute:'EmailHeader' }}
{{ Person.NickName }},

<p>{{ Workflow | Attribute:'CustomMessage' | NewlineToBr }}</p>
<p>
    These assessments are ready for you to take:
</p>
<p>
	<div><!--[if mso]>
	  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{ assessmentsLink }}"" style=""height:38px;v-text-anchor:middle;width:175px;"" arcsize=""11%"" strokecolor=""#e76812"" fillcolor=""#ee7624"">
		<w:anchorlock/>
		<center style=""color:#ffffff;font-family:sans-serif;font-size:13px;font-weight:normal;"">Take Assessment</center>
	  </v:roundrect>
	<![endif]--><a href=""{{ assessmentsLink }}""
	style=""background-color:#ee7624;border:1px solid #e76812;border-radius:4px;color:#ffffff;display:inline-block;font-family:sans-serif;font-size:13px;font-weight:normal;line-height:38px;text-align:center;text-decoration:none;width:175px;-webkit-text-size-adjust:none;mso-hide:all;"">Take Assessment</a></div>
</p>
<br />
<br />
{{ 'Global' | Attribute:'EmailFooter' }}"); // Request Assessment:Save And Send:Send Assessment Custom Message Email:Body
            RockMigrationHelper.AddActionTypeAttributeValue("C8DE19DB-106A-4AFA-9069-C420B45B976C","65E69B78-37D8-4A88-B8AC-71893D2F75EF",@"False"); // Request Assessment:Save And Send:Send Assessment Custom Message Email:Save Communication History
            RockMigrationHelper.AddActionTypeAttributeValue("8F2AC810-83E1-48D2-B306-8F396539343D","0CA0DDEF-48EF-4ABC-9822-A05E225DE26C",@"False"); // Request Assessment:Save And Send:Workflow Complete:Active
            RockMigrationHelper.AddActionTypeAttributeValue("8F2AC810-83E1-48D2-B306-8F396539343D","07CB7DBC-236D-4D38-92A4-47EE448BA89A",@"Completed"); // Request Assessment:Save And Send:Workflow Complete:Status|Status Attribute

            #endregion

            #region Edit bio block list of workflow actions

            // Add Request Assessment
            Sql( @"
                DECLARE @bioWFActionsAttributeValueId INT = 
                (SELECT v.[Id]
                FROM [dbo].[attribute] a
                JOIN [AttributeValue] v ON a.id = v.AttributeId
                WHERE a.[EntityTypeId] = 9
                    AND a.[EntityTypeQualifierColumn] = 'BlockTypeId'
                    AND a.[Key] = 'WorkflowActions'
                    AND a.[EntityTypeQualifierValue] = (SELECT [Id] FROM [dbo].[BlockType] WHERE [Name] = 'person bio')
                    AND v.[Value] NOT LIKE '%31DDC001-C91A-4418-B375-CAB1475F7A62%')

                IF (@bioWFActionsAttributeValueId IS NOT NULL)
                BEGIN
                    UPDATE [dbo].[AttributeValue]
                    SET [Value] = [Value] + ',31DDC001-C91A-4418-B375-CAB1475F7A62'
                    WHERE [Id] = @bioWFActionsAttributeValueId
                END" );

            // Remove legacy DISC Request
            Sql( @"
                DECLARE @bioWFActionsAttributeValueId INT = 
                (SELECT v.[Id]
                FROM [dbo].[attribute] a
                JOIN [AttributeValue] v ON a.id = v.AttributeId
                WHERE a.[EntityTypeId] = 9
                    AND a.[EntityTypeQualifierColumn] = 'BlockTypeId'
                    AND a.[Key] = 'WorkflowActions'
                    AND a.[EntityTypeQualifierValue] = (SELECT [Id] FROM [dbo].[BlockType] WHERE [Name] = 'person bio')
                    AND v.[Value] LIKE '%885CBA61-44EA-4B4A-B6E1-289041B6A195%')

                IF (@bioWFActionsAttributeValueId IS NOT NULL)
                BEGIN
                    UPDATE [dbo].[AttributeValue]
                    SET [Value] = REPLACE([Value], ',885CBA61-44EA-4B4A-B6E1-289041B6A195', '')
                    WHERE [Id] = @bioWFActionsAttributeValueId
                END" );

            #endregion Edit bio block list of workflow actions

            // Set old DISC workflow to inactive.
            Sql( @"
                UPDATE [dbo].[WorkflowType]
                SET [IsActive] = 0
                WHERE [Guid] = '885CBA61-44EA-4B4A-B6E1-289041B6A195'" );
        }

        /// <summary>
        /// Updates the spirtual gifts results message block attribute.
        /// </summary>
        private void UpdateSpirtualGiftsResultsMessageBlockAttribute()
        {
            RockMigrationHelper.UpdateBlockTypeAttribute( "A7E86792-F0ED-46F2-988D-25EBFCD1DC96", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Results Message", "ResultsMessage", "", @"The text (HTML) to display at the top of the results section.<span class='tip tip-lava'></span><span class='tip tip-html'></span>", 0, @"
<div class='row'>
    <div class='col-md-12'>
    <h2 class='h2'> Dominant Gifts</h2>
    </div>
    <div class='col-md-9'>
    <table class='table table-bordered table-responsive'>
    <thead>
        <tr>
            <th>
                Spiritual Gift
            </th>
            <th>
                You are uniquely wired to:
            </th>
        </tr>
    </thead>
    <tbody>
        {% if DominantGifts != empty %}
            {% for dominantGift in DominantGifts %}
                <tr>
                    <td>
                        {{ dominantGift.Value }}
                    </td>
                    <td>
                        {{ dominantGift.Description }}    
                    </td>
                </tr>
            {% endfor %}
        {% else %}
            <tr>
                <td colspan='2'>
                    You did not have any Dominant Gifts
                </td>
            </tr>
        {% endif %}
    </tbody>
    </table>
    </div>
</div>
    
<div class='row'>
    <div class='col-md-12'>
        <h2 class='h2'> Supportive Gifts</h2>
    </div>
    <div class='col-md-9'>
        <table class='table table-bordered table-responsive'>
            <thead>
                <tr>
                   <th>
                    Spiritual Gift
                    </th>
                    <th>
                    You are uniquely wired to:
                    </th>
                </tr>
            </thead>
            <tbody>
                {% if SupportiveGifts != empty %}
                    {% for supportiveGift in SupportiveGifts %}
                        <tr>
                            <td>
                                {{ supportiveGift.Value }}
                            </td>
                            <td>
                                {{ supportiveGift.Description }}
                            </td>
                        </tr>
                    {% endfor %}
                {% else %}
                    <tr>
                        <td colspan='2'>
                            You did not have any Supportive Gifts
                        </td>
                    </tr>
                {% endif %}
            </tbody>
        </table>
    </div>
</div?
<div class='row'>
    <div class='col-md-12'>
        <h2 class='h2'> Other Gifts</h2>
    </div>
    <div class='col-md-9'>
        <table class='table table-bordered table-responsive'>
            <thead>
                <tr>
                   <th>
                    Spiritual Gift
                    </th>
                    <th>
                    You are uniquely wired to:
                    </th>
                </tr>
            </thead>
            <tbody>
                {% if OtherGifts != empty %}
                    {% for otherGift in OtherGifts %}
                        <tr>
                            <td>
                                {{ otherGift.Value }}
                            </td>
                            <td>
                                {{ otherGift.Description }}
                            </td>
                        </tr>
                    {% endfor %}
                {% else %}
                    <tr>
                        <td colspan='2'>
                            You did not have any Other Gifts
                        </td>
                    </tr>
                {% endif %}
           </tbody>
        </table>
    </div>
</div>", "85256610-56EB-4E6F-B62B-A5517B54B39E" );
        }

        /// <summary>
        /// Assessments the list page block attributes.
        /// </summary>
        private void AssessmentListPageBlockAttributes()
        {
            // Assessments list page, this is where the link in the emails will go.
            RockMigrationHelper.AddPage( true, "EBAA5140-4B8F-44B8-B1E8-C73B654E4B22","5FEAF34C-7FB6-4A11-8A1E-C452EC7849BD","Assessments","","FCF44690-D74C-4FB7-A01B-0EFCA6EA9E1E","fa fa-bar-chart"); // Site:External Website
            RockMigrationHelper.AddPageRoute("FCF44690-D74C-4FB7-A01B-0EFCA6EA9E1E","Assessments","F2873F65-617C-4BD3-94E0-48E2408EBDBD");// for Page:Assessments
            RockMigrationHelper.UpdateBlockType("Assessment List","Allows you to view and take any available assessments.","~/Blocks/Crm/AssessmentList.ascx","CRM","0AD1D108-4ABF-4AED-B3B7-4AAEA16D10E4");
            RockMigrationHelper.AddBlock( true, "FCF44690-D74C-4FB7-A01B-0EFCA6EA9E1E".AsGuid(),null,"F3F82256-2D66-432B-9D67-3552CD2F4C2B".AsGuid(),"0AD1D108-4ABF-4AED-B3B7-4AAEA16D10E4".AsGuid(), "Assessment List","Main",@"",@"",0,"0E22E6CB-1634-41CA-83EF-4BC7CE52F314"); // Add Block to Page: Assessments Site: External Website
            RockMigrationHelper.AddBlock( true, "C0854F84-2E8B-479C-A3FB-6B47BE89B795".AsGuid(),null,"F3F82256-2D66-432B-9D67-3552CD2F4C2B".AsGuid(),"0AD1D108-4ABF-4AED-B3B7-4AAEA16D10E4".AsGuid(), "Assessment List","Sidebar1",@"",@"",2,"37D4A991-9F9A-47CE-9084-04466F166B6A"); 
            
            // Attrib for BlockType: Assessment List:Only Show Requested
            RockMigrationHelper.UpdateBlockTypeAttribute("0AD1D108-4ABF-4AED-B3B7-4AAEA16D10E4","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Only Show Requested","OnlyShowRequested","",@"If enabled, limits the list to show only assessments that have been requested or completed.",0,@"True","7A10C446-B0F3-43F0-9FEB-78B689593736");
            // Attrib for BlockType: Assessment List:Hide If No Requests
            RockMigrationHelper.UpdateBlockTypeAttribute("0AD1D108-4ABF-4AED-B3B7-4AAEA16D10E4","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Hide If No Requests","HideIfNoRequests","",@"If enabled, nothing will be shown where there are no requests (pending or completed).",2,@"False","1E5EE52F-DFD5-4406-A517-4B76E2800D2A");
            // Attrib for BlockType: Assessment List:Hide If No Active Requests
            RockMigrationHelper.UpdateBlockTypeAttribute("0AD1D108-4ABF-4AED-B3B7-4AAEA16D10E4","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Hide If No Active Requests","HideIfNoActiveRequests","",@"If enabled, nothing will be shown if there are not pending (waiting to be taken) assessment requests.",1,@"False","305AD0A5-6E35-402A-A6A2-50474733368A");

            // Attrib for BlockType: Assessment List:Lava Template
            RockMigrationHelper.UpdateBlockTypeAttribute("0AD1D108-4ABF-4AED-B3B7-4AAEA16D10E4","1D0D3794-C210-48A8-8C68-3FBEC08A6BA5","Lava Template","LavaTemplate","",@"The lava template to use to format the entire block.  <span class='tip tip-lava'></span> <span class='tip tip-html'></span>",0,@"<div class='panel panel-default'>
    <div class='panel-heading'>Assessments</div>
    <div class='panel-body'>
            {% for assessmenttype in AssessmentTypes %}
                {% if assessmenttype.LastRequestObject %}
                    {% if assessmenttype.LastRequestObject.Status == 'Complete' %}
                        <div class='panel panel-success'>
                            <div class='panel-heading'>{{ assessmenttype.Title }}<br />
                                Completed: {{ assessmenttype.LastRequestObject.CompletedDate | Date:'M/d/yyyy'}} <br />
                                <a href='{{ assessmenttype.AssessmentResultsPath}}'>View Results</a>
                            </div>
                        </div>
                    {% elseif assessmenttype.LastRequestObject.Status == 'Pending' %}
                        <div class='panel panel-warning'>
                            <div class='panel-heading'> {{ assessmenttype.Title }}<br />
                                Requested: {{assessmenttype.LastRequestObject.Requester}} ({{ assessmenttype.LastRequestObject.RequestedDate | Date:'M/d/yyyy'}})<br />
                                <a href='{{ assessmenttype.AssessmentPath}}'>Start Assessment</a>
                            </div>
                        </div>
                    {% endif %}
                    {% else %}
                        <div class='panel panel-default'>
                            <div class='panel-heading'> {{ assessmenttype.Title }}<br />
                                Available<br />
                                <a href='{{ assessmenttype.AssessmentPath}}'>Start Assessment</a>
                            </div>
                        </div>
                {% endif %}
            {% endfor %}
    </div>
</div>","044D444A-ECDC-4B7A-8987-91577AAB227C");

            // Attrib Value for Block:Assessment List, Attribute:Only Show Requested Page: Assessments, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue("0E22E6CB-1634-41CA-83EF-4BC7CE52F314","7A10C446-B0F3-43F0-9FEB-78B689593736",@"False");
            // Attrib Value for Block:Assessment List, Attribute:Hide If No Requests Page: Assessments, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue("0E22E6CB-1634-41CA-83EF-4BC7CE52F314","1E5EE52F-DFD5-4406-A517-4B76E2800D2A",@"False");
            // Attrib Value for Block:Assessment List, Attribute:Lava Template Page: Assessments, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue("0E22E6CB-1634-41CA-83EF-4BC7CE52F314","044D444A-ECDC-4B7A-8987-91577AAB227C",@"<div class='panel panel-default'>
    <div class='panel-heading'>Assessments</div>
    <div class='panel-body'>
            {% for assessmenttype in AssessmentTypes %}
                {% if assessmenttype.LastRequestObject %}
                    {% if assessmenttype.LastRequestObject.Status == 'Complete' %}
                        <div class='panel panel-success'>
                            <div class='panel-heading'>{{ assessmenttype.Title }}<br />
                                Completed: {{ assessmenttype.LastRequestObject.CompletedDate | Date:'M/d/yyyy'}} <br />
                                <a href='{{ assessmenttype.AssessmentResultsPath}}'>View Results</a>
                            </div>
                        </div>
                    {% elseif assessmenttype.LastRequestObject.Status == 'Pending' %}
                        <div class='panel panel-warning'>
                            <div class='panel-heading'> {{ assessmenttype.Title }}<br />
                                Requested: {{assessmenttype.LastRequestObject.Requester}} ({{ assessmenttype.LastRequestObject.RequestedDate | Date:'M/d/yyyy'}})<br />
                                <a href='{{ assessmenttype.AssessmentPath}}'>Start Assessment</a>
                            </div>
                        </div>
                    {% endif %}
                    {% else %}
                        <div class='panel panel-default'>
                            <div class='panel-heading'> {{ assessmenttype.Title }}<br />
                                Available<br />
                                <a href='{{ assessmenttype.AssessmentPath}}'>Start Assessment</a>
                            </div>
                        </div>
                {% endif %}
            {% endfor %}
    </div>
</div>");
            // Attrib Value for Block:Assessment List, Attribute:Hide If No Active Requests Page: Assessments, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue("0E22E6CB-1634-41CA-83EF-4BC7CE52F314","305AD0A5-6E35-402A-A6A2-50474733368A",@"False");
            // Attrib Value for Block:Assessment List, Attribute:Hide If No Active Requests Page: My Account, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue("37D4A991-9F9A-47CE-9084-04466F166B6A","305AD0A5-6E35-402A-A6A2-50474733368A",@"False");
            // Attrib Value for Block:Assessment List, Attribute:Hide If No Requests Page: My Account, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue("37D4A991-9F9A-47CE-9084-04466F166B6A","1E5EE52F-DFD5-4406-A517-4B76E2800D2A",@"False");
            // Attrib Value for Block:Assessment List, Attribute:Lava Template Page: My Account, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue("37D4A991-9F9A-47CE-9084-04466F166B6A","044D444A-ECDC-4B7A-8987-91577AAB227C",@"<div class='panel panel-default'>
    <div class='panel-heading'>Assessments</div>
    <div class='panel-body'>
            {% for assessmenttype in AssessmentTypes %}
                {% if assessmenttype.LastRequestObject %}
                    {% if assessmenttype.LastRequestObject.Status == 'Complete' %}
                        <div class='panel panel-success'>
                            <div class='panel-heading'>{{ assessmenttype.Title }}<br />
                                Completed: {{ assessmenttype.LastRequestObject.CompletedDate | Date:'M/d/yyyy'}} <br />
                                <a href='{{ assessmenttype.AssessmentResultsPath}}'>View Results</a>
                            </div>
                        </div>
                    {% elseif assessmenttype.LastRequestObject.Status == 'Pending' %}
                        <div class='panel panel-warning'>
                            <div class='panel-heading'> {{ assessmenttype.Title }}<br />
                                Requested: {{assessmenttype.LastRequestObject.Requester}} ({{ assessmenttype.LastRequestObject.RequestedDate | Date:'M/d/yyyy'}})<br />
                                <a href='{{ assessmenttype.AssessmentPath}}'>Start Assessment</a>
                            </div>
                        </div>
                    {% endif %}
                    {% else %}
                        <div class='panel panel-default'>
                            <div class='panel-heading'> {{ assessmenttype.Title }}<br />
                                Available<br />
                                <a href='{{ assessmenttype.AssessmentPath}}'>Start Assessment</a>
                            </div>
                        </div>
                {% endif %}
            {% endfor %}
    </div>
</div>");
            // Attrib Value for Block:Assessment List, Attribute:Only Show Requested Page: My Account, Site: External Website
            RockMigrationHelper.AddBlockAttributeValue("37D4A991-9F9A-47CE-9084-04466F166B6A","7A10C446-B0F3-43F0-9FEB-78B689593736",@"False");

            // update block order for pages with new blocks if the page,zone has multiple blocks
            Sql(@"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = '87068AAB-16A7-42CC-8A31-5A957D6C4DD5'");  // Page: My Account,  Zone: Sidebar1,  Block: Actions
            Sql(@"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '8C513CAC-FB3F-40A2-A0F6-D4C50FF72EC8'");  // Page: My Account,  Zone: Sidebar1,  Block: Group List Personalized Lava
            Sql(@"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = '37D4A991-9F9A-47CE-9084-04466F166B6A'");  // Page: My Account,  Zone: Sidebar1,  Block: Assessment List
            Sql(@"UPDATE [Block] SET [Order] = 3 WHERE [Guid] = 'E5596525-B176-4753-A337-25F1F9B83FCE'");  // Page: My Account,  Zone: Sidebar1,  Block: Recent Registrations
        }

        /// <summary>
        /// Conflicts the profile assessment page block attributes.
        /// </summary>
        private void ConflictProfileAssessmentPageBlockAttributes()
        {
            // Conflict Profile Assessment
            RockMigrationHelper.AddPage( true, "FCF44690-D74C-4FB7-A01B-0EFCA6EA9E1E","BE15B7BC-6D64-4880-991D-FDE962F91196","Conflict Profile Assessment","","37F17AD8-8103-4F85-865C-94E76B4470BB",""); // Site:External Website
            RockMigrationHelper.AddPageRoute("37F17AD8-8103-4F85-865C-94E76B4470BB","ConflictProfile","B843AFE4-9198-49DE-904B-8D6440158DAC");// for Page:Conflict Profile Assessment
            RockMigrationHelper.AddPageRoute("37F17AD8-8103-4F85-865C-94E76B4470BB","ConflictProfile/{rckipid}","AFD90575-B363-4862-B4A6-1283D5C00AD9");// for Page:Conflict Profile Assessment
            RockMigrationHelper.UpdateBlockType("Conflict Profile","Allows you to take a conflict profile test and saves your conflict profile score.","~/Blocks/Crm/ConflictProfile.ascx","CRM","91473D2F-607D-4260-9C6A-DD3762FE472D");
            RockMigrationHelper.AddBlock( true, "37F17AD8-8103-4F85-865C-94E76B4470BB".AsGuid(),null,"F3F82256-2D66-432B-9D67-3552CD2F4C2B".AsGuid(),"91473D2F-607D-4260-9C6A-DD3762FE472D".AsGuid(), "Conflict Profile","Main",@"",@"",0,"D005E292-25F8-45D4-A713-2A5C811F0219"); // Add Block to Page: Conflict Profile Assessment Site: External Website
            RockMigrationHelper.UpdateBlockTypeAttribute("91473D2F-607D-4260-9C6A-DD3762FE472D","9C204CD0-1233-41C5-818A-C5DA439445AA","Set Page Title","SetPageTitle","",@"The text to display as the heading.",0,@"Conflict Profile","C5698564-7178-43BA-B4A3-58B13DDC3AF0"); // Attrib for BlockType: Conflict Profile:Set Page Title
            RockMigrationHelper.UpdateBlockTypeAttribute("91473D2F-607D-4260-9C6A-DD3762FE472D","9C204CD0-1233-41C5-818A-C5DA439445AA","Set Page Icon","SetPageIcon","",@"The css class name to use for the heading icon.",1,@"fa fa-gift","D5ABBD1A-61F1-4C48-8AD9-C26AC7F5CAEF"); // Attrib for BlockType: Conflict Profile:Set Page Icon
            RockMigrationHelper.UpdateBlockTypeAttribute("91473D2F-607D-4260-9C6A-DD3762FE472D","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Allow Retakes","AllowRetakes","",@"If enabled, the person can retake the test after the minimum days passes.",3,@"True","E3965E46-603C-40E5-AB28-1B53E44561DE"); // Attrib for BlockType: Conflict Profile:Allow Retakes
            RockMigrationHelper.UpdateBlockTypeAttribute("91473D2F-607D-4260-9C6A-DD3762FE472D","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Min Days To Retake","MinDaysToRetake","",@"The number of days that must pass before the test can be taken again. Leave blank to use the Assessment Type's minimum.",4,@"","E8147587-812D-4118-995D-E5B7A3189979"); // Attrib for BlockType: Conflict Profile:Min Days To Retake
            RockMigrationHelper.UpdateBlockTypeAttribute("91473D2F-607D-4260-9C6A-DD3762FE472D","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Number of Questions","NumberofQuestions","",@"The number of questions to show per page while taking the test",2,@"7","6CBCA505-E5BA-4FE9-9DD8-7F3C507B12B8"); // Attrib for BlockType: Conflict Profile:Number of Questions
            
            // Attrib for BlockType: Conflict Profile:Instructions
            RockMigrationHelper.UpdateBlockTypeAttribute("91473D2F-607D-4260-9C6A-DD3762FE472D","1D0D3794-C210-48A8-8C68-3FBEC08A6BA5","Instructions","Instructions","",@"The text (HTML) to display at the top of the instructions section.  <span class='tip tip-lava'></span> <span class='tip tip-html'></span>",0,@"
<h2>Welcome to the Conflict Profile Assessment</h2>
<p>
    {{ Person.NickName }}, this assessment was developed and researched by Dr. Gregory A. Wiens and Al Ells and is based on the work and writings of Kenneth Thomas and Ralph Kilmann. When dealing with conflict, one way isn’t always the right way to solve a problem. All five of the modes evaluated in this assessment are appropriate at different times. The challenge is to know which approach is appropriate at what times. It is also important to understand how to use each approach.
</p>
<p>
   Most people feel comfortable using a few approaches and these approaches are often what we saw demonstrated in our culture of origin. This may not have been a healthy method of dealing with conflict.
</p>
<p>
    Before you begin, please take a moment and pray that the Holy Spirit would guide your thoughts,
    calm your mind, and help you respond to each item as honestly as you can. Don't spend much time
    on each item. Your first instinct is probably your best response.
</p>","2E455190-2BAE-4E9F-8505-F393BCE52342");

            // Attrib for BlockType: Conflict Profile:Results Message
            RockMigrationHelper.UpdateBlockTypeAttribute("91473D2F-607D-4260-9C6A-DD3762FE472D","1D0D3794-C210-48A8-8C68-3FBEC08A6BA5","Results Message","ResultsMessage","",@"The text (HTML) to display at the top of the results section.<span class='tip tip-lava'></span><span class='tip tip-html'></span>",0,@"
<p>
   Your scores on this report are how YOU see yourself currently dealing with conflict in the environment chosen. This may or may not be accurate depending on how you are aware of yourself in the midst of conflict. It is most helpful to discuss your scores with someone who understands both you and this assessment.  Remember, in the future, the way you approach conflict should be dictated by the situation, not just how you are used to dealing with conflict. In doing so, everyone benefits, including you.
</p>

<h2>Conflict Engagement Profile</h2>
{[ chart type:'pie' ]}
    [[ dataitem label:'Solving' value:'{{EngagementProfileSolving}}' fillcolor:'#FFCD56' ]] [[ enddataitem ]]
    [[ dataitem label:'Accommodating' value:'{{EngagementProfileAccommodating}}' fillcolor:'#4BC0C0' ]] [[ enddataitem ]]
    [[ dataitem label:'Winning' value:'{{EngagementProfileWinning}}' fillcolor:'#FF3D67' ]] [[ enddataitem ]]
{[ endchart ]}

<h4>Brief Definitions for Conflict Engagement Modes</h4>

<p>
    <b>SOLVING</b> describes those who seek to use both RESOLVING and COMPROMISING modes for solving conflict. By combining these two modes, those who seek to solve problems as a team. Their leadership styles are highly cooperative and empowering for the benefit of the entire group.<br>
    <b>ACCOMMODATING</b> combines AVOIDING and YIELDING modes for solving conflict. Those who are ACCOMMODATING are most effective in roles where allowing others to have their way is better for the team. They are often most effective in support roles or roles where an emphasis on the contribution of others is significant.<br>
    A <b>WINNING</b> engagement profile relates to the WINNING mode for solving conflict. Depending on your role, WINNING engagement is important for times when quick decisions need to be made and essential for sole-proprietors.
</p>

<h2>Your Results Across All Five Modes</h2>
{[ chart type:'bar' ]}
    [[ dataitem label:'Winning' value:'{{Winning}}' fillcolor:'#FF3D67' ]] [[ enddataitem ]]
    [[ dataitem label:'Resolving' value:'{{Resolving}}' fillcolor:'#059BFF' ]] [[ enddataitem ]]
    [[ dataitem label:'Compromising' value:'{{Compromising}}' fillcolor:'#4BC0C0' ]] [[ enddataitem ]]
    [[ dataitem label:'Avoiding' value:'{{Avoiding}}' fillcolor:'#FFCD56' ]] [[ enddataitem ]]
    [[ dataitem label:'Yielding' value:'{{Yielding}}' fillcolor:'#880D37' ]] [[ enddataitem ]]
{[ endchart ]}

<h4>Brief Definitions for Conflict Profile Modes</h4>
<p>
    <b>WINNING</b> is competing and uncooperative. You believe you have the right answer and you must prove you are right whatever it takes. This may be standing up for your own rights, beliefs or position.<br>
    <b>RESOLVING</b> is attempting to work with the other person in depth to find the best solution regardless of where it may lie on the continuum. This involves digging beneath the presenting issue to find a way out that benefits both parties.<br>
    <b>COMPROMISING</b> is finding a middle ground in the conflict. This often involves meeting in the middle or finding some mutually agreeable point between both positions and is useful for quick solutions.<br>
    <b>AVOIDING</b> is not pursuing your own rights or those of the other person. You do not address the conflict. This may be diplomatically sidestepping an issue or avoiding a threatening situation.<br>
    <b>YIELDING</b> is neglecting your own interests and giving in to those of the other person. This is self-sacrifice and may be charity, serving or choosing to obey another when you prefer not to.
</p>
","1A855117-6489-4A15-846A-5A99F54E9747");
        }

        /// <summary>
        /// Emotionals the intelligence assessment page block attributes.
        /// </summary>
        private void EmotionalIntelligenceAssessmentPageBlockAttributes()
        {
             
            // Emotional Intelligence Assessment
            RockMigrationHelper.AddPage( true, "FCF44690-D74C-4FB7-A01B-0EFCA6EA9E1E","BE15B7BC-6D64-4880-991D-FDE962F91196","Emotional Intelligence Assessment","","BE5F3984-C25E-47CA-A602-EE1CED99E9AC",""); // Site:External Website
            RockMigrationHelper.AddPageRoute("BE5F3984-C25E-47CA-A602-EE1CED99E9AC","EQAssessment","8C5F1CF8-8AC1-4123-B7FD-E57EA36CFBBF");// for Page:Emotional Intelligence Assessment
            RockMigrationHelper.AddPageRoute("BE5F3984-C25E-47CA-A602-EE1CED99E9AC","EQAssessment/{rckipid}","C97D4D5A-F082-4F2B-A873-71F734B539CC");// for Page:Emotional Intelligence Assessment
            RockMigrationHelper.UpdateBlockType("EQ Assessment","Allows you to take a EQ Inventory test and saves your EQ Inventory score.","~/Blocks/Crm/EQInventory.ascx","CRM","040CFD6D-5155-4BC9-BAEE-A53219A7BECE");
            RockMigrationHelper.AddBlock( true, "BE5F3984-C25E-47CA-A602-EE1CED99E9AC".AsGuid(),null,"F3F82256-2D66-432B-9D67-3552CD2F4C2B".AsGuid(),"040CFD6D-5155-4BC9-BAEE-A53219A7BECE".AsGuid(), "EQ Assessment","Main",@"",@"",0,"71BE6A7A-7D51-4149-AFB1-3307DF04B2DF"); // Add Block to Page: Emotional Intelligence Assessment Site: External Website
            RockMigrationHelper.UpdateBlockTypeAttribute("040CFD6D-5155-4BC9-BAEE-A53219A7BECE","9C204CD0-1233-41C5-818A-C5DA439445AA","Set Page Title","SetPageTitle","",@"The text to display as the heading.",0,@"EQ Inventory Assessment","E99F01A7-AF8F-4010-A456-3A9048347859"); // Attrib for BlockType: EQ Assessment:Set Page Title
            RockMigrationHelper.UpdateBlockTypeAttribute("040CFD6D-5155-4BC9-BAEE-A53219A7BECE","9C204CD0-1233-41C5-818A-C5DA439445AA","Set Page Icon","SetPageIcon","",@"The css class name to use for the heading icon.",1,@"fa fa-gift","D5CF91C1-2CC8-46BF-8CC6-DD6AD8B07518"); // Attrib for BlockType: EQ Assessment:Set Page Icon
            RockMigrationHelper.UpdateBlockTypeAttribute("040CFD6D-5155-4BC9-BAEE-A53219A7BECE","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Allow Retakes","AllowRetakes","",@"If enabled, the person can retake the test after the minimum days passes.",3,@"True","A0905767-79C9-4567-BA76-A3FFEE71E0B3"); // Attrib for BlockType: EQ Assessment:Allow Retakes
            RockMigrationHelper.UpdateBlockTypeAttribute("040CFD6D-5155-4BC9-BAEE-A53219A7BECE","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Number of Questions","NumberofQuestions","",@"The number of questions to show per page while taking the test",2,@"7","8D2C5502-0AAB-4FE6-ABE9-05900439827D"); // Attrib for BlockType: EQ Assessment:Number of Questions
            RockMigrationHelper.UpdateBlockTypeAttribute("040CFD6D-5155-4BC9-BAEE-A53219A7BECE","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Min Days To Retake","MinDaysToRetake","",@"The number of days that must pass before the test can be taken again. Leave blank to use the Assessment Type's minimum.",4,@"","B47E54AC-A66B-4EC6-B4FC-F9A7B1175E27"); // Attrib for BlockType: EQ Assessment:Min Days To Retake
            #region Attrib for BlockType: EQ Assessment:Results Message
            RockMigrationHelper.UpdateBlockTypeAttribute("040CFD6D-5155-4BC9-BAEE-A53219A7BECE","1D0D3794-C210-48A8-8C68-3FBEC08A6BA5","Results Message","ResultsMessage","",@"The text (HTML) to display at the top of the results section.<span class='tip tip-lava'></span><span class='tip tip-html'></span>",0,@"

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
","5B6219CE-84B5-4F68-BE5B-C3187EDFF2A6");
            #endregion Attrib for BlockType: EQ Assessment:Results Message

            #region Attrib for BlockType: EQ Assessment:Instructions
            RockMigrationHelper.UpdateBlockTypeAttribute("040CFD6D-5155-4BC9-BAEE-A53219A7BECE","1D0D3794-C210-48A8-8C68-3FBEC08A6BA5","Instructions","Instructions","",@"The text (HTML) to display at the top of the instructions section.  <span class='tip tip-lava'></span> <span class='tip tip-html'></span>",0,@"
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
</p>","6C00C171-E6DC-4027-B587-0AB63AC939E3");
            #endregion Attrib for BlockType: EQ Assessment:Instructions

        }

        /// <summary>
        /// Creates the pages, blocks, and attributes for assessments.
        /// </summary>
        private void PagesBlocksAndAttributesUp()
        {
            RockMigrationHelper.AddLayout( "F3F82256-2D66-432B-9D67-3552CD2F4C2B", "FullWidthNarrow", "Full Width Narrow", "", "BE15B7BC-6D64-4880-991D-FDE962F91196" ); // Site:External Website

            AssessmentListPageBlockAttributes();
            ConflictProfileAssessmentPageBlockAttributes();
            EmotionalIntelligenceAssessmentPageBlockAttributes();

            // Use Assessment List page as the parent page for existing assessments
            RockMigrationHelper.MovePage( "C8CEF4B0-4A09-46D2-9B6B-CD2B6D3078B1", "FCF44690-D74C-4FB7-A01B-0EFCA6EA9E1E" );// Move DISC page to new parent Assessment Tests
            RockMigrationHelper.MovePage( "06410598-3DA4-4710-A047-A518157753AB", "FCF44690-D74C-4FB7-A01B-0EFCA6EA9E1E" );// Move gifts page to new parent Assessment Tests

            RockMigrationHelper.UpdateFieldType("Assessment Types","","Rock","Rock.Field.Types.AssessmentTypesFieldType","C263513A-30BE-4823-ABF1-AC12A56F9644");
        }

        /// <summary>
        /// Deletes the pages, blocks and attributes for assessments.
        /// </summary>
        private void PagesBlocksAndAttributesDown()
        {
            // Attrib for BlockType: EQ Assessment:Instructions
            RockMigrationHelper.DeleteAttribute("6C00C171-E6DC-4027-B587-0AB63AC939E3");
            // Attrib for BlockType: EQ Assessment:Min Days To Retake
            RockMigrationHelper.DeleteAttribute("B47E54AC-A66B-4EC6-B4FC-F9A7B1175E27");
            // Attrib for BlockType: EQ Assessment:Allow Retakes
            RockMigrationHelper.DeleteAttribute("A0905767-79C9-4567-BA76-A3FFEE71E0B3");
            // Attrib for BlockType: EQ Assessment:Set Page Icon
            RockMigrationHelper.DeleteAttribute("D5CF91C1-2CC8-46BF-8CC6-DD6AD8B07518");
            // Attrib for BlockType: EQ Assessment:Set Page Title
            RockMigrationHelper.DeleteAttribute("E99F01A7-AF8F-4010-A456-3A9048347859");
            // Attrib for BlockType: EQ Assessment:Results Message
            RockMigrationHelper.DeleteAttribute("5B6219CE-84B5-4F68-BE5B-C3187EDFF2A6");
            // Attrib for BlockType: EQ Assessment:Number of Questions
            RockMigrationHelper.DeleteAttribute("8D2C5502-0AAB-4FE6-ABE9-05900439827D");
            // Attrib for BlockType: Conflict Profile:Allow Retakes
            RockMigrationHelper.DeleteAttribute("E3965E46-603C-40E5-AB28-1B53E44561DE");
            // Attrib for BlockType: Conflict Profile:Number of Questions
            RockMigrationHelper.DeleteAttribute("6CBCA505-E5BA-4FE9-9DD8-7F3C507B12B8");
            // Attrib for BlockType: Conflict Profile:Results Message
            RockMigrationHelper.DeleteAttribute("1A855117-6489-4A15-846A-5A99F54E9747");
            // Attrib for BlockType: Conflict Profile:Min Days To Retake
            RockMigrationHelper.DeleteAttribute("E8147587-812D-4118-995D-E5B7A3189979");
            // Attrib for BlockType: Conflict Profile:Set Page Icon
            RockMigrationHelper.DeleteAttribute("D5ABBD1A-61F1-4C48-8AD9-C26AC7F5CAEF");
            // Attrib for BlockType: Conflict Profile:Instructions
            RockMigrationHelper.DeleteAttribute("2E455190-2BAE-4E9F-8505-F393BCE52342");
            // Attrib for BlockType: Conflict Profile:Set Page Title
            RockMigrationHelper.DeleteAttribute("C5698564-7178-43BA-B4A3-58B13DDC3AF0");
            // Attrib for BlockType: Assessment List:Hide If No Active Requests
            RockMigrationHelper.DeleteAttribute("305AD0A5-6E35-402A-A6A2-50474733368A");
            // Attrib for BlockType: Assessment List:Lava Template
            RockMigrationHelper.DeleteAttribute("044D444A-ECDC-4B7A-8987-91577AAB227C");
            // Attrib for BlockType: Assessment List:Hide If No Requests
            RockMigrationHelper.DeleteAttribute("1E5EE52F-DFD5-4406-A517-4B76E2800D2A");
            // Attrib for BlockType: Assessment List:Only Show Requested
            RockMigrationHelper.DeleteAttribute("7A10C446-B0F3-43F0-9FEB-78B689593736");
            // Remove Block: Conflict Profile, from Page: Conflict Profile Assessment, Site: External Website
            RockMigrationHelper.DeleteBlock("D005E292-25F8-45D4-A713-2A5C811F0219");
            // Remove Block: EQ Assessment, from Page: Emotional Intelligence Assessment, Site: External Website
            RockMigrationHelper.DeleteBlock("71BE6A7A-7D51-4149-AFB1-3307DF04B2DF");
            // Remove Block: Assessment List, from Page: My Account, Site: External Website
            RockMigrationHelper.DeleteBlock("37D4A991-9F9A-47CE-9084-04466F166B6A");
            // Remove Block: Assessment List, from Page: Assessments, Site: External Website
            RockMigrationHelper.DeleteBlock("0E22E6CB-1634-41CA-83EF-4BC7CE52F314");
            RockMigrationHelper.DeleteBlockType("040CFD6D-5155-4BC9-BAEE-A53219A7BECE"); // EQ Assessment
            RockMigrationHelper.DeleteBlockType("91473D2F-607D-4260-9C6A-DD3762FE472D"); // Conflict Profile
            RockMigrationHelper.DeleteBlockType("0AD1D108-4ABF-4AED-B3B7-4AAEA16D10E4"); // Assessment List
            RockMigrationHelper.DeletePage("F44A6424-8B9C-4B44-91A8-4BB6F683D4B6"); //  Page: Motivators Assessment, Layout: FullWidth, Site: External Website
            RockMigrationHelper.DeletePage("BE5F3984-C25E-47CA-A602-EE1CED99E9AC"); //  Page: Emotional Intelligence Assessment, Layout: FullWidth, Site: External Website
            RockMigrationHelper.DeletePage("37F17AD8-8103-4F85-865C-94E76B4470BB"); //  Page: Conflict Profile Assessment, Layout: FullWidth, Site: External Website
            RockMigrationHelper.DeletePage("FCF44690-D74C-4FB7-A01B-0EFCA6EA9E1E"); //  Page: Assessments, Layout: FullWidth, Site: External Website
        }
    }
}
