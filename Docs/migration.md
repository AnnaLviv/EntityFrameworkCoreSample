Via "Package Manager Console" CLI we can execute multiple entity framework commands.
List of avilable commands can be found via "get-help entityframework".

We can generate code migrations files via "add-migration init".
Migtations from code can be executed with "update-database -verbose" command.

We can generate .sql script via "script-migration".

For database migrations under developemnt it is most common do use code migrations.
For production migrations it is most common to use scripts.

Reverse engineering:
It is possible to create DBContext and classes out of exisitng databse.
Currently it is not possible to update these classes out of existing database.
Powershell command: scaffold-dbcontext. Provider and connection parameters are required.
Example: scaffold-dbcontext -provider Microsoft.EntityFrameworkCore.SqlServer -connection  "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SamuraiAppData"