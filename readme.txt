SOLUTION DESCRIPTION

TECHNOLOGIES USED
 1. Moq - for working with mocks in unit testing.
 2. Newtonsoft.Json - for json serialization.
 3. Swagger - For interactive API documentation.
 4. MsSql - for persistance of saved data.
 5. Git - as version control system.
 6. AWS - for hosting an API.
 7. c# - as main coding language.
 8. Powershell - for testing utility.
 
TESTING SOLUTION
 1. Use swagger - http://jsondiff-prod.eu-west-1.elasticbeanstalk.com/swagger/index.html

 2. Use TestingScript.ps1:
	TestingScript.ps1 -action left -id readmeTest -body eyJpbnB1dCI6InRlc3RWYWx1ZSJ9
	TestingScript.ps1 -action right -id readmeTest -body eyJpbnB1dCI6InJlc3RMYXZ1ZSJ9
	TestingScript.ps1 -id readmeTest
	
 3. Use Curl as per assignment
	curl -X POST https://localhost:44361/v1/diff/curl/left -H "accept: */*" -H "Content-Type:application/custom" -d "\"eyJpbnB1dCI6InRlc3RWYWx1ZSJ9\""
	curl -X POST https://localhost:44361/v1/diff/curl/right -H "accept: */*" -H "Content-Type:application/custom" -d "\"eyJpbnB1dCI6InRlc3RWYWx1ZSJ9\""																												 
	curl -X GET https://localhost:44361/v1/diff/curl -H "accept: */*" -H "Content-Type:application/custom"
	
SOLUTION LIMITATIONS
 1. ID is limited to only 128 characters. 
 2. Comparison is limited to only one type of a file - "JsonFile". 
    The app can be expanded to be able to test any text based file.
 3. Solution has no ability to remove old files from database. 
	"Housekeeping" can be introduced to periodically remove old files.
 4. App is not hosted securely. It should be accessed via HTTPS protocol.
 
TODOS
 1. Create better documentation for Swagger. Currently Swagger documentation is generated from XML comments.
 2. Introduce caching on database level.
 3. Introduce caching on http level.
 4. Create stress tests.
 5. Password to database is stored right in code base. Handle passwords in the app.
 