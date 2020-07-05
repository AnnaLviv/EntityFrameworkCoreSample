Methods to load related data:
1. Eager loading. Include related objects in query. Allows using DbSet Include method to retrieve data and related data (full graph of data) in the same call
2. Query Projections. Define the shape of query results. You can define the shape of full graph of data to be returned
3. Explicit Loading. Request related data of objects in memory. For cases when you already have some related data in memory, and want to go back to the DB to get some related data
4. Lazy loading. On-the-fly retrieval of related data. Easy accidentally to abuse it and create performace problems. Lazy loading is off by default.
Happens implicitly by mention of the navigation. All you have to do - is to make reference o that navigation from existing object.
Enable LL with these requirements:
	- every navigation property must be virtual
	- Microsoft.EntityFramework.Proxies package
	- In DbCOntext class ModelBuilder.UseLazyLoadingProxies() must be specified
