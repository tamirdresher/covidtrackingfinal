<h1>Covid Patients Tracking System</h1>
A C# REST Api based system that allows different CRUD functions

<h2>Functionality</h2>
This system interacts with a remote MongoDB database.</br>
It allows patient creation, deletion and update.</br>
A nurse can use the different CRUD Endpoints to update the database with new patients that need to be tracked, </br>
add visit routes for infected patients, log encounters that each infected patient had with different people, </br>
make interviews with potential patients that each patient has encountered, get information about those encounters</br>
and get different statistics about healed, infected and isolated people in our system.</br>

<h2>Structure</h2>
The solution contains 2 projects:</br>
The first is 'covidtracking' - the project containing the system, implementd as a REST API service with different CRUD endpoints.</br>
The second is 'covidtracking.UnitTests' - the test project, containing different unit tests implemented with xUnit that cover the functionality of the system.</br>
The database used in this project is MongoDB and the connection string is hardcoded to ease the usage and testing.</br>

<h2>Architecture:</h2>
The 'covidtracking' is written as a REST API project.</br>
The project was implemented following the SOLID principles to allow the desired benefits.</br>
It contains divided into layers, kept in several folders, each contains the files used in that particular layer:</br>

<b>Controllers:</b></br>
The controllers are the layer that handles the different Http requests the API uses.</br>
Each controller handles the appropriate requests realated to that area of the project/data:</br>
For example:</br>
LabTestController handles requests related to lab tests creation,</br>
StatisticsController handles statistics fetching from the database and etc.</br>
Each controller communicates with the appropriate databases/collections needed for its operations.</br>

<b>Database:</b></br>
The database layer contains the different database models, each in its own internal folder for order and convenience.</br>
While we used MongoDB as our database in this project, any other DB (f.e. SQL SERVER...) can be used by implementing each model's</br>
interface contract. By using the Interfaces we enable our system to be more generic and responsive.</br>
The current concrete (MongoDB) models are injected using Dependency Injection in the Program.cs file.</br>
Each controller may use different DB models but each model only communicates with its single database/collection.</br>
<b>The models contain all the logic our system uses.</b>

<b>Dtos:</b></br>
We used Dtos (Data transfer objects) to handle Object creation and data transfer in a safe and agreed manner.</br>
Each area has its own Dtos and they are used accross the project to create links and provide correct connections.</br>

<b>Entities:</b></br>
The entities represent each Entity's formal implementation.</br>
The Mongo database knows and stores those entities (while the internal data mostly uses Dtos).</br>

<b>Settings:</b></br>
The Settings folder contains the MongoDB settings (connection string).</br>

<b>Utilities:</b></br>
The Utitilites folder contains different adapters and extensions used across the project.</br>

<h2>Testing</h2>
The project contains internal methods and statements to verify correctness of flow and protection in different edge points.</br>
We used conditional statements and REGEX to verify correctness.</br>
<b>Unit Testing</b></br>
the 'covidtracking.UnitTests' project contains the unit tests for our 'covidtracking' project.</br>
The unit tests, implemented using xUnit cover different endpoints and edge points our system has.</br>
The tests include ActionResult type Assertion, returned value comparison and it has passing/failing expected tests.</br>

<h1>Build and Workflow</h1>
<h2>Build and run</h2>
To run the project, you just need to clone the repository and perform a regular run using VSCode/Visual Studio.</br>
The project supports Swagger UI or can be accessed using Postman.</br>
Use "https://localhost:7122/swagger/index.html" for Swagger or just "https://localhost:7122/" for Postman.</br>
We created a Postman workspace containing the different Endpoints the system has at:</br>
https://www.postman.com/lunar-firefly-552571/workspace/covidtracking
<h2>Ci/Cd</h2>
This github repository has an automatic CI/CD workflow.</br>
It is implemented using github actions and is activated on each push/pull request to the 'main' branch.</br>
The workflow builds the project and commits 30 unit tests.</br>

<h1>Endpoints</h1>

| Endpoint                                     | Description                                                                                                                                                                              |
|----------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| PUT /patients                                | Add a new patient to be tracked by the system                                                                                                                                            |
| GET /patients                                | Returns the list of all the patients tracked by the system (only real patients and not potential ones)                                                                                   |
| PUT /patients/{id}/route                     | Add a location a patient visited during the last 7 days                                                                                                                                  |
| GET /patients/{id}/route                     | Returns the list of all the locations the patient visited in during the last 7 days                                                                                                      |
| PUT /patients/{id}/encounters                | Add the details of a person the patient met during the last 7 days                                                                                                                       |
| GET /patients/{id}/encounters                | Return the list of the people the patient met during the last 7 days                                                                                                                     |
| GET /patients/{id}/full                      | Returns the person details and whether he is sick or not together with all his/her lab tests                                                                                             |
| GET /patients/new?since=[VALUE]              | Will display a list of all sick people who were added after the value of 'since'                                                                                                         |
| GET /patients/potential                      | Returns the list of encounters where the person details were not inserted yet                                                                                                            |
| GET /patients/isolated                       | Returns the list of all the people in the system that are in isolation (person is isolated until he has two negative tests since he encountered an infected person or reported infected) |
| POST patients/potential/{potentialPatientId} | Remove the potential patient and transform him into real patient                                                                                                                         |
| POST /labtests                               | Add a lab test result                                                                                                                                                                    |
| GET /statistics                              | Returns statistics about the current state – amount of sicks, amount of isolated, how many have healed, and how many sick we have per city                                               |
