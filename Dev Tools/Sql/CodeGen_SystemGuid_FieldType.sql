/* Helps code generate Rock\SystemGuid\FieldType.cs */
SELECT CONCAT('
/// <summary>
/// ', [Name] , ' field type
/// </summary>
public const string ', REPLACE(REPLACE(UPPER([Name]), ' ', '_'), '-', '_'), ' = "', [Guid], '";')
  FROM [FieldType]
ORDER BY Name