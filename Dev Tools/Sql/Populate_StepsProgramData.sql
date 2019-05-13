use rock_spark_steps
go

--
-- Script to populate a Rock RMS database with sample data for the Steps module.
--
-- This script assumes the following:
-- * The Person table is populated.
-- * Step Types are arranged so that prerequisite steps occur first in the sort order.
--

-- Parameters
declare
	-- Set this flag to indicate if existing sample data should be deleted.
	@deleteExistingData bit = 1
    -- Set this value to the maximum number of steps that can be created by this script.
	,@maxStepCount int = 1000
	-- Set this value to the maximum number of days between consecutive step achievements.
	,@maxDaysBetweenSteps int = 365
	-- Set this value to the number of years before today that the earliest steps will be created.
	,@yearsBack int = 5
	-- Set this value to the maximum number of people for whom steps will be created.
	,@maxPersonCount int = 100
	-- Set this Step Foreign Key value to enable sample data records to be easily identified.
	,@stepSampleDataForeignKey nvarchar(100) = 'Steps Sample Data'

set nocount on

print N'Create Steps Sample Data: started.';

print N'Creating Programs...';

--
-- Create Step Programs with associated Step Types.
--
DECLARE @categoryGuid AS UNIQUEIDENTIFIER = '43DC43A8-420B-4012-BAA0-0A0DDF2D4A9A';
DECLARE @programGuid AS UNIQUEIDENTIFIER = '2CAFBB12-901F-4880-A3E4-848F25CAF1A6';
DECLARE @baptismStepTypeGuid AS UNIQUEIDENTIFIER = '23E73F78-587A-483F-99EF-855FD6AD1B11';
DECLARE @marriageStepTypeGuid AS UNIQUEIDENTIFIER = 'D03B3C65-C128-4918-A300-509B94B90175';
DECLARE @holyOrdersStepTypeGuid AS UNIQUEIDENTIFIER = '0099C701-6C1E-418E-A94F-C247A2FE4BA5';
DECLARE @confirmationStepTypeGuid AS UNIQUEIDENTIFIER = 'F109169F-C1F6-46ED-9091-274540E3F3E2';
DECLARE @baptismTedGuid AS UNIQUEIDENTIFIER = '02BB71C9-5FE9-45B8-B230-51C7A8475B6B';
DECLARE @marriage1TedGuid AS UNIQUEIDENTIFIER = '414EBE88-2CD2-40E1-893B-216DEA2CB25E';
DECLARE @marriage2TedGuid AS UNIQUEIDENTIFIER = '314F5303-C803-442B-AE39-DAA7BC30CCEE';
DECLARE @tedPersonAliasId AS INT = 16;
DECLARE @successStatusGuid AS UNIQUEIDENTIFIER = 'A5C2A14F-9ED9-4DF4-A1C8-8ADF75E18833';
DECLARE @dangerStatusGuid AS UNIQUEIDENTIFIER = 'B591240C-4D4D-49DA-82E3-F8C1738B8EC6';
DECLARE @prereqGuid AS UNIQUEIDENTIFIER = 'D96A2C67-2D76-4697-838F-3514CA11485E';

--DELETE FROM StepType WHERE Guid IN (@marriageStepTypeGuid, @baptismStepTypeGuid);

IF NOT EXISTS (SELECT * FROM Category WHERE Guid = @categoryGuid)
BEGIN
	INSERT INTO Category (
		IsSystem,
		EntityTypeId,
		Name,
		Guid,
		[Order]
	) VALUES (
		1,
		( SELECT Id FROM EntityType WHERE Name = 'Rock.Model.StepProgram' ),
		'Sacraments Category',
		@categoryGuid,
		0
	);
END

IF NOT EXISTS (SELECT * FROM StepProgram WHERE Guid = @programGuid)
BEGIN
	INSERT INTO StepProgram (
		Guid,
		Name,
		Description,
		IconCssClass,
		IsActive,
		[Order],
		CategoryId,
		DefaultListView
	) VALUES (
		@programGuid,
		'Sacraments Program',
		'The description of the program',
		'fa fa-bible',
		1,
		0,
		( SELECT Id FROM Category WHERE Guid = @categoryGuid ),
		0
	);
END

IF NOT EXISTS (SELECT * FROM StepStatus WHERE Guid = @successStatusGuid)
BEGIN
	INSERT INTO StepStatus (
		StepProgramId,
		Name,
		IsCompleteStatus,
		StatusColor,
		IsActive,
		[Order],
		Guid
	) VALUES (
		( SELECT Id FROM StepProgram WHERE Guid = @programGuid ),
		'DONE!',
		1,
		'success',
		1,
		1,
		@successStatusGuid
	);
END

IF NOT EXISTS (SELECT * FROM StepStatus WHERE Guid = @dangerStatusGuid)
BEGIN
	INSERT INTO StepStatus (
		StepProgramId,
		Name,
		IsCompleteStatus,
		StatusColor,
		IsActive,
		[Order],
		Guid
	) VALUES (
		( SELECT Id FROM StepProgram WHERE Guid = @programGuid ),
		'ahh...',
		0,
		'danger',
		1,
		2,
		@dangerStatusGuid
	);
END

IF NOT EXISTS (SELECT * FROM StepType WHERE Guid = @baptismStepTypeGuid)
BEGIN
	INSERT INTO StepType (
		StepProgramId,
		Name,
		AllowManualEditing,
		AllowMultiple,
		HasEndDate,
		ShowCountOnBadge,
		IsActive,
		[Order],
		Guid,
		IconCssClass
	) VALUES (
		( SELECT Id FROM StepProgram WHERE Guid = @programGuid ),
		'Baptism',
		0,
		0,
		0,
		1,
		1,
		1,
		@baptismStepTypeGuid,
		'fa fa-tint'
	);
END

IF NOT EXISTS (SELECT * FROM StepType WHERE Guid = @marriageStepTypeGuid)
BEGIN
	INSERT INTO StepType (
		StepProgramId,
		Name,
		AllowManualEditing,
		AllowMultiple,
		HasEndDate,
		ShowCountOnBadge,
		IsActive,
		[Order],
		Guid,
		IconCssClass
	) VALUES (
		( SELECT Id FROM StepProgram WHERE Guid = @programGuid ),
		'Marriage',
		0,
		1,
		1,
		1,
		1,
		2,
		@marriageStepTypeGuid,
		'fa fa-ring'
	);
END

IF NOT EXISTS (SELECT * FROM StepType WHERE Guid = @confirmationStepTypeGuid)
BEGIN
	INSERT INTO StepType (
		StepProgramId,
		Name,
		AllowManualEditing,
		AllowMultiple,
		HasEndDate,
		ShowCountOnBadge,
		IsActive,
		[Order],
		Guid,
		IconCssClass
	) VALUES (
		( SELECT Id FROM StepProgram WHERE Guid = @programGuid ),
		'Confirmation',
		0,
		0,
		0,
		1,
		1,
		4,
		@confirmationStepTypeGuid,
		'fa fa-bible'
	);
END

IF NOT EXISTS (SELECT * FROM StepType WHERE Guid = @holyOrdersStepTypeGuid)
BEGIN
	INSERT INTO StepType (
		StepProgramId,
		Name,
		AllowManualEditing,
		AllowMultiple,
		HasEndDate,
		ShowCountOnBadge,
		IsActive,
		[Order],
		Guid,
		IconCssClass
	) VALUES (
		( SELECT Id FROM StepProgram WHERE Guid = @programGuid ),
		'Holy Orders',
		0,
		0,
		0,
		1,
		1,
		4,
		@holyOrdersStepTypeGuid,
		'fa fa-cross'
	);
END

-- Insert attributes for step types
DECLARE @textFieldTypeId AS INT = (SELECT Id FROM FieldType WHERE Class = 'Rock.Field.Types.TextFieldType');
DECLARE @intFieldTypeId AS INT = (SELECT Id FROM FieldType WHERE Class = 'Rock.Field.Types.IntegerFieldType');
DECLARE @stepEntityTypeId AS INT = (SELECT Id FROM EntityType WHERE Name = 'Rock.Model.Step');
DECLARE @baptismBaptismalGuid AS UNIQUEIDENTIFIER = '1CBE6BE4-9699-4660-B859-D240ABDB0FA8';
DECLARE @baptismPrepYearGuid AS UNIQUEIDENTIFIER = 'E36B38CD-D958-4552-A0D3-DF529C3283FD';

IF NOT EXISTS (SELECT * FROM Attribute WHERE Guid = @baptismBaptismalGuid)
BEGIN
	INSERT INTO Attribute (
		IsSystem,
		FieldTypeId,
		EntityTypeId,
		EntityTypeQualifierColumn,
		EntityTypeQualifierValue,
		[Key],
		Name,
		[Order],
		IsGridColumn,
		IsMultiValue,
		IsRequired,
		Guid
	) VALUES (
		1,
		@textFieldTypeId,
		@stepEntityTypeId,
		'StepTypeId',
		(SELECT Id FROM StepType WHERE Guid = @baptismStepTypeGuid),
		'BaptismalName',
		'Baptismal Name',
		1,
		0,
		0,
		0,
		@baptismBaptismalGuid
	);
END

IF NOT EXISTS (SELECT * FROM Attribute WHERE Guid = @baptismPrepYearGuid)
BEGIN
	INSERT INTO Attribute (
		IsSystem,
		FieldTypeId,
		EntityTypeId,
		EntityTypeQualifierColumn,
		EntityTypeQualifierValue,
		[Key],
		Name,
		[Order],
		IsGridColumn,
		IsMultiValue,
		IsRequired,
		Guid
	) VALUES (
		1,
		@intFieldTypeId,
		@stepEntityTypeId,
		'StepTypeId',
		(SELECT Id FROM StepType WHERE Guid = @baptismStepTypeGuid),
		'PrepYear',
		'Prep Year',
		1,
		0,
		0,
		0,
		@baptismPrepYearGuid
	);
END

-- Add step data for Ted Decker
IF NOT EXISTS (SELECT * FROM Step WHERE Guid = @baptismTedGuid)
BEGIN
	INSERT INTO Step (
		StepTypeId,
		PersonAliasId,
		CompletedDateTime,
		Guid,
		[Order],
		StepStatusId
	) VALUES (
		( SELECT Id FROM StepType WHERE Guid = @baptismStepTypeGuid ),
		@tedPersonAliasId,
		'12/4/1996',
		@baptismTedGuid,
		1,
		NULL
	);
END

IF NOT EXISTS (SELECT * FROM Step WHERE Guid = @marriage1TedGuid)
BEGIN
	INSERT INTO Step (
		StepTypeId,
		PersonAliasId,
		CompletedDateTime,
		Guid,
		[Order],
		StepStatusId
	) VALUES (
		( SELECT Id FROM StepType WHERE Guid = @marriageStepTypeGuid ),
		@tedPersonAliasId,
		'12/4/1980',
		@marriage1TedGuid,
		1,
		( SELECT Id FROM StepStatus WHERE Guid = @dangerStatusGuid )
	);
END

IF NOT EXISTS (SELECT * FROM Step WHERE Guid = @marriage2TedGuid)
BEGIN
	INSERT INTO Step (
		StepTypeId,
		PersonAliasId,
		CompletedDateTime,
		Guid,
		[Order],
		StepStatusId
	) VALUES (
		( SELECT Id FROM StepType WHERE Guid = @marriageStepTypeGuid ),
		@tedPersonAliasId,
		'12/4/2005',
		@marriage2TedGuid,
		2,
		( SELECT Id FROM StepStatus WHERE Guid = @successStatusGuid )
	);
END

IF NOT EXISTS (SELECT * FROM StepTypePrerequisite WHERE Guid = @prereqGuid)
BEGIN
	INSERT INTO StepTypePrerequisite (
		StepTypeId,
		PrerequisiteStepTypeId,
		Guid,
		[Order]
	) VALUES (
		( SELECT Id FROM StepType WHERE Guid = @holyOrdersStepTypeGuid ),
		( SELECT Id FROM StepType WHERE Guid = @confirmationStepTypeGuid ),
		@prereqGuid,
		1
	)
END

print N'Creating Steps...';

-- Local variables.
declare
    @stepCounter int = 0
    ,@personAliasId int
    ,@stepTypeId int
	,@stepProgramId int
    ,@startDateTime datetime
	,@nextStepDateTime datetime
    ,@campusId int
	,@maxStepTypeCount int
	,@stepsToAddCount int

declare
	@createdDateTime datetime = (select convert(date, SYSDATETIME()))
	,@createdByPersonAliasId int = (select pa.id from PersonAlias pa inner join Person p on pa.PersonId = p.Id where p.FirstName = 'Admin' and p.LastName = 'Admin');

declare
    @daysBack int = @yearsBack * 366

declare
    @stepTypeIds table ( id Int, rowNo Int );

declare
	@stepStatusIds table ( id int
						   ,stepProgramId int
						   ,isCompleteStatus bit );
declare
    @stepProgramIds table ( id Int );

declare
    @personAliasIds table ( id Int );

declare
     @stepTable table (
   	    [StepTypeId] [int] NOT NULL,
	    [StepStatusId] [int] NULL,	   
	    [PersonAliasId] [int] NULL,
		[CampusId] [int] NULL,
		[CompletedDateTime] [datetime] NULL,
	    [StartDateTime] [datetime] NOT NULL,
	    [EndDateTime] [datetime] NULL,
	    [Note] [nvarchar](max) NULL,
	    [Order] [int] NOT NULL,
		[Guid] [uniqueidentifier] NOT NULL,
	    [CreatedDateTime] [datetime] NULL,
	    [ModifiedDateTime] [datetime] NULL,
	    [CreatedByPersonAliasId] [int] NULL,
	    [ModifiedByPersonAliasId] [int] NULL,
	    [ForeignKey] [nvarchar](100) NULL		
    );

begin

	-- Remove existing data.
	if ( @deleteExistingData = 1 )
	begin
		delete from Step where [ForeignKey] = @stepSampleDataForeignKey;
	end

	-- Get a complete set of active Step Types ordered by Program and structure order.
	insert into @stepProgramIds
		select Id
		from [StepProgram] sp
		where exists (select top 1 Id from StepType where StepProgramId = sp.Id )
	  order by [Order];

	insert into @stepStatusIds
		select Id
				,StepProgramId
				,IsCompleteStatus
		from [StepStatus]

	declare @itemCount int;

	set @itemCount = ( select count(*) from @stepProgramIds )

	if ( @itemCount = 0 )
		throw 50000, 'Populate Steps data failed. There are no configured Step Programs in this database.', 1;
    
	print N'--> Adding steps for ' + CAST(@itemCount AS nvarchar(10)) + ' program(s).';

	-- Get a random selection of people.
	insert into @personAliasIds
		select top (@maxPersonCount)
			   Id
		from PersonAlias
		order by newid();
	
	set @startDateTime = DATEADD(DAY, -@daysBack, SYSDATETIME())	
	set @stepCounter = 1;
	declare @nextStepTypeId int = 0;
	declare @statusId int = 0;
	declare @addDays int;

    WHILE @stepCounter <= @maxStepCount
    BEGIN

        -- Randomly select a Person, Program and number of Steps to achieve.
		-- Steps are added in Program sequence order to ensure that prerequisite steps are completed first.
		set @stepProgramId = (select top 1 Id from @stepProgramIds order by newid())
		set @personAliasId =  (select top 1 Id from @personAliasIds order by newid())
		set @nextStepDateTime = @startDateTime
		set @campusId = (select top 1 Id from Campus order by newid()) 

		set @maxStepTypeCount = (SELECT COUNT(Id) from StepType where StepProgramId = @stepProgramId);

		-- Randomly select the number of Steps that this person will achieve in the Program, from 1 to @maxStepTypeCount.
		set @stepsToAddCount = (SELECT FLOOR(RAND()*(@maxStepTypeCount)+1))

		insert into @stepTypeIds
			 select top (@stepsToAddCount)
				Id
				,ROW_NUMBER() OVER(order by [Order]) rowNo
			 from StepType
			 where StepProgramId = @stepProgramId order by [Order]

		set @stepTypeId = (select top 1 Id from @stepTypeIds)

	    while ( @stepTypeId is not null AND @stepCounter <= @maxStepCount)
		begin
			
		if (@stepCounter % 100 = 0)
			print N'--> (' + CAST(@stepCounter AS nvarchar(10)) + ' added)...';

			-- Get the next Step Type to process.
			delete from @stepTypeIds
				where Id = @stepTypeId
			
			set @nextStepTypeId = (select top 1 Id from @stepTypeIds order by [rowNo])

			-- Set the step status. If not the last step, make sure the status represents a completion.
			if ( @nextStepTypeId is null )
				set @statusId = ( select top 1 Id from @stepStatusIds where stepProgramId = @stepProgramId order by newid() )
			else
				set @statusId = ( select top 1 Id from @stepStatusIds where stepProgramId = @stepProgramId and isCompleteStatus = 1 order by newid() )

		    -- Add a random number of days to the date of the previous step taken by this person.
			set @addDays = (SELECT FLOOR(RAND()*(@maxDaysBetweenSteps)+1))
			set @nextStepDateTime = DATEADD(DAY, @addDays, @nextStepDateTime);

			insert into @stepTable
   						([StepTypeId]
						,[StepStatusId] 
						,[PersonAliasId]
						,[CampusId]
						,[CompletedDateTime]
						,[StartDateTime]
						,[Order]
						,[Guid]
						,[CreatedDateTime]
						,[CreatedByPersonAliasId]
						,[ForeignKey])		
				 values (@stepTypeId
						,@statusId
						,@personAliasId
						,@campusId
						,@nextStepDateTime
						,@nextStepDateTime
						,0
						,NEWID()
						,@createdDateTime
						,@createdByPersonAliasId
						,@stepSampleDataForeignKey)
        
			set @stepCounter += 1;
			set @stepTypeId = @nextStepTypeId;        
	    end
	end
	
	set @stepCounter -= 1;
	print N'--> Created ' + CAST(@stepCounter AS nvarchar(10)) + ' steps.';
    
	insert into Step
		        ([StepTypeId]
				,[StepStatusId] 
				,[PersonAliasId]
				,[CampusId]
				,[CompletedDateTime]
				,[StartDateTime]
				,[Order]
				,[Note]
				,[Guid]
				,[CreatedDateTime]
				,[CreatedByPersonAliasId]
				,[ForeignKey])
	    select  [StepTypeId]
				,[StepStatusId] 
				,[PersonAliasId]
				,[CampusId]
				,[CompletedDateTime]
				,[StartDateTime]
				,[Order]
				,[Note]
				,[Guid]
				,[CreatedDateTime]				
				,[CreatedByPersonAliasId]
				,[ForeignKey]
		from @stepTable
		--order by StartDateTime
	
	print N'Create Steps Sample Data: completed.';

end
