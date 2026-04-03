# URL Shortener
This Project is an API which shortens URLs into  concise Short Links.

## Functionality
This API allows for:
* Shortening of a URL into a unique Short Link, with the identifier being either auto generated or user defined.
* Securing a Short Link behind a password, allowing access to the URL only if the correct password is provided.
* Resolving a Short Link and redirecting to its referenced URL.
* Fetching analytics on a specific Short Link within a given date range:
  * Included information:
    * Total number of visits
    * Creation date
    * Most recent visit date
* Fetching analytics on all visits within a given date range:
  * Included information:
    * Total number of Short Links created
    * Total number of visits
    * Top 5 visited URLs

## Usage
While in developer mode, this API's endpoints can be interacted with through a swagger page, which can be found at
http://localhost:5048/swagger/index.html. The Resolve endpoint can be used directly from a web browser.

## Authentication
Every end point except for the resolution of a Short Link requires a JWT from the authentication endpoint.

## Hosting
After cloning, the requirements to host this project are the following:
* A SQL server with the connection string stored in "UrlShorteningContext" variable within appsettings.json.
* A webpage which reads user input to validate password protected Short Links. This URL should be placed in
the "PasswordValidationWebpage" variable in appsettings.json.

## Dependencies
* API
  * Microsoft.AspNetCore.OpenApi
  * Microsoft.AspNetCore.Authentication.JwtBearer
  * Microsoft.EntityFrameworkCore
  * Microsoft.EntityFrameworkCore.SqlServer
  * Microsoft.EntityFrameworkCore.Tools
  * Sqids
  * Swashbuckle.AspNetCore
* Application
  * Microsoft.AspNetCore.Authentication.JwtBearer
  * Sqids
* Infrastructure
  * Microsoft.EntityFrameworkCore
  * Microsoft.EntityFrameworkCore.SqlServer
  * Microsoft.EntityFrameworkCore.Tools