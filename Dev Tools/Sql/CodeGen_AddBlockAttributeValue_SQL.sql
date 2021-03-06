/* Code Generate 'AddBlockAttributeValue(...)' for migrations. 
This will list all of the block attribute values starting with most recently Inserted
Just pick the top few that you need for your migration depending

-- AttributeValue
INSERT INTO [AttributeValue] ([IsSystem], [AttributeId], [EntityId], [Value], [Guid])
VALUES

*/

begin

declare
@crlf varchar(2) = char(13) + char(10),
@BlockId int = 885

SELECT 
	'(' +
	CONVERT(nvarchar(max), v.[IsSystem]) + ', @' +
	CONVERT(nvarchar(max), a.[Key]) + 'Id, @B_' +
	CONVERT(nvarchar(max), REPLACE(b.[Name],' ', '')) + 'Id, ''' +
	CONVERT(nvarchar(max), REPLACE(v.[Value],'''','''''')) + ''', ''' +
	CONVERT(nvarchar(50), v.Guid) + '''),' +
        @crlf [CodeGen Recently Added Attribute Values],
		'DECLARE @'+a.[Key]+'Id int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = ''' + CONVERT(nvarchar(50), a.Guid) + ''' )',
		'DECLARE @B_'+REPLACE(b.[Name],' ', '')+'Id int = ( SELECT TOP 1 [Id] FROM [Block] WHERE [Guid] = ''' + CONVERT(nvarchar(50), b.Guid) + ''' )'
  FROM [AttributeValue] [v]
  join [Attribute] [a] on [a].[Id] = [v].[AttributeId]
  left join [EntityType] [e] on [e].[Id] = [a].[EntityTypeId]
  join [FieldType] [ft] on [ft].[Id] = [a].[FieldTypeId]
  left join [Block] [b] on b.Id = [v].[EntityId]
  where 
  b.Id = @BlockId
order by [v].[Id] desc

end
