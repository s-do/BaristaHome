# BaristaHome
If you get any exception error it's probably because you don't have the db created once running this app
You should be able to get it going once you do these steps:
1. Above the Visual Studios window, go to Tools>NuGet Package Manager>Package Manager Console
2. Type in Add-Migration [AnyFileName] to create a migration file that pushes changes to your db based off of your Models 
3. Update-Database to create the db and you should be able to run the app w/ no exception error
