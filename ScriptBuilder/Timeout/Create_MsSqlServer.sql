﻿declare @tableName nvarchar(max) = '[' + @schema + '].[' + @endpointName + 'TimeoutData]';

if not exists (
    select * from sys.objects
    where
        object_id = object_id(@tableName)
        and type in (N'U')
)
begin
declare @createTable nvarchar(max);
set @createTable = N'
    create table ' + @tableName + '(
        [Id] [uniqueidentifier]not null primary key,
        [Destination] [nvarchar](1024),
        [SagaId] [uniqueidentifier],
        [State] [varbinary](max),
        [Time] [datetime],
        [Headers] [nvarchar](max) not null,
        [PersistenceVersion] [nvarchar](23)not null
    )
';
exec(@createTable);
end