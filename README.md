# Cloud Security Rubik's Cube

## Description
**TODO**: write a description on the project

***
## Installation
Currently, the installation for this program is as follows:
1. Install the appropriate version of MS SQL Server Express (w/ Management Studio). Make sure to set up a local database. In local tests we used MS SQL Express 2014.
2. Set up the database:
..1 Using the SQL Management Studio, run the schemaInit.sql script located in `ModelDB\schemaInit.sql`. You may need to run this script as an administrator.
..2 Again using the SQL Management Studio, run the dataInit.sql script located in `ModelDB\dataInit.sql`.
4. Install the CSRC application.
5. Run the CSRC application. The application should automatically detect local MS SQL servers.

The CSRC application should be installed now