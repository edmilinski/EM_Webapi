CREATE LOGIN [ed] WITH PASSWORD = 'Password1.';
use [EMNotes]
CREATE USER [ed] FOR LOGIN [ed];
exec sp_addrolemember 'db_owner', 'ed'
