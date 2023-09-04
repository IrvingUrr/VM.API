##VM.API

#Create DB
The scripts for the creation of the DB are provided inside the project SQLScripts under folder Scripts. The scripts must be executed in order, first CreateTableUser, then CreateTableUserPurchase and finally SeedData.

#Update Connection String
Please provide a valid connection string into appsettings.json file under ConnectionString.WebApiDatabase section. DataSource is Empty by default and Catalog is set to VirtualMind, in case of create a diferente Catalog Name, please update it accordingly.
