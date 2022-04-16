## Minutes: 2022-04-14
### Scribe: Keelan
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
## 4/14 Scrum

Discussion about trying to get Docker working on Ashlyn and Keelan's machines. Mainly just need to build the docker images again

What everyone is working on right now:
* Tom - Still struggling with Docker, almost there but there is a connection time out issue with MinIO.
* Ashlyn - Got most of the auth stuff merged in, made a few more tasks, need to pop up a message to notify their credentials are wrong on login, ability to edit password in edit user view
* Kenny - just put up filter PR, adding authentication to Building endpoints
* Joe - latest PR will insert a default admin user when the MongoConnector is instantiated, will be moving on to devicetype authentication, then do some backend clean up
* Keelan - Finished case insensitivity for username, move on to front end tasks

Note: Sprint 4 ends on Thursday next week instead of Tuesday. (Ashlyn)

## Minutes: 2022-04-12
### Scribe: Keelan
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
## 4/12 Scrum

What everyone is working on right now:
* Joe - working on devicetype auth and then will move on to building auth. doesn't have anything left to do after that, has a light task load, needs some more tasks.
* Keelan - busy weekend, still working on username case insensitivity, will put up PR soon after writing another test, also has light task load, needs more tasks
* Tom - Working through Docker networking issues (MinIO, Mongo)
* Ashlyn - Wrapping up authentication, has authentication working on front end
* Kenny - finishing up device filtering on front end and will start working on adding authentication on user endpoints

Keelan will assign backend tasks to Joe, and will take some of Ashlyn's front end tasks instead. Joe was assigned devicetype auth from Keelan, and then will work on adding default user upon first start.

Possible backend tasks:
* Create a default user on first start
* Add more MongoFilterGenerator filters

## Minutes: 2022-04-08
### Scribe: Keelan
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
	Brian Emtman
## Milestone 4 Demo

Likes:
* BEIMA does automatic data checking to prevent bad user input
* UI is clean and easy to use

Potential improvements:
* Being able to change device type of a current device (low priority)
* Mapping would be appreciated but not absolutely necessary (time constraints are understood)

Notes:
* Allow duplicate device tags
* If there is a duplicate device tag, then pop up a warning showing that there is a duplicate. Also show a warning that if they proceed, add a suffix to the device tag, and add some extra notes to help discern which one is which.

## Minutes: 2022-04-07
### Scribe: Keelan
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
## 4/7 Scrum

What everyone is working on:
* Keelan - working on username case insensitivity task
* Ashlyn - building integration
* Tom - Docker integration
* Joe - working on bug concerning amount of time tests take 
* Kenny -  device endpoints authentication

For the demo, we should just go over all the functionality and show Brian it is all working. Focus on the fact that it is using dynamic data now.

Action items:
* Meeting on Tuesday for documentation

## Minutes: 2022-04-06
### Scribe: Keelan
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
## Sprint 4 Planning Session

Sprint Goal:
Wrap up major pieces (authentication, filtering, building integration), write Docker file, and write documentation. We also want to clean up the UI and backend, leaving it in a state where we feel comfortable handing it off to another team.

What we did:
* Discussed goals for this upcoming sprint, prioritized different items
* Went through all existing tasks and assigned them priorities
* Created new tasks based on sprint goal
* Assigned all planned tasks to team members

Action Items:
* Send Dr. Henderson email saying we cannot bring a monitor to showcase (Kenny)
* Documentation writing meeting on Tuesday
* Ask Brian about whether we should enforce device tag uniqueness across device and building? Also need to ask if we are certain that device tag will be unique in each building. Also ask if there could be the same device tag across different device types.

## Minutes: 2022-04-05
### scribe: Keelan
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
## Sprint 4 Pre-Planning Session
Discussed what we should focus on during Sprint 4:
* Filtering - urgent (do query string on back end first)
* Searching for device by tag?
* Authentication (on all endpoints and login pages)
* Reporting (.csv stuff)
* Building endpoint integration
* Documentation - meeting next Tuesday?
* Make GitHub IO page
* Docker deployment
* UI and endpoint cleanup
* Mock map
* Add download functionality for files/photos for front end
* Graphics for showcase
* Change base URL for SDP server
* Add favicon and tab title
* Add SDP sticker

Make documentation lower priority

Action Items:
* Sprint planning session tomorrow at 6pm


## Minutes: 2022-04-05
### scribe: Ashlyn
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
## Sprint 3 Retro
What went well?
 * We got A LOT of work done (74 tasks)
 * We got a good flow going for this sprint, was able to work kinks out easier
 * We were able to work together for getting blocked tasks done
 * PRs had good feedback
 * We learned new things from others' PR comments

What didn't go well?
 * Some blocking tasks, but not as bad as previously
 * Some PRs were sitting there in the queue for longer than normal

Action items
 * If PRs are not getting feedback, ping the chat

Next roles
 * Kenny is scrum master
 * Tom is product owner
 * Keelan is scribe

## Minutes: 2022-03-31
### scribe: Ashlyn
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
## Sprint 3 Scrum Meeting
 * standup
 	* Keelan
 		* got all filtering tasks done and merged
		* starting on user endpoints
 	* Kenny
 		* updated csv service, should be good to go
		* put up PR to fix null photo exception. Minio delete function would throw exception on null
		when azure didn't, added more tests
 	* Ashlyn
 		* did research on JWT, made some tasks for that
		* some user frontend tasks and auth tasks are blocked by tasks in the backend
		* there are few tasks I can do in the meantime
 	* Joe
 		* two PRs up for review, valid building id for device endpoints and year manu to be null
		* working on attributes validated on backend
 	* Tom
 		* was waiting for Ashlyn's PR to merge, they are merged as well as other PRs he needed
		* working on blank fields warning
		* enhance Notification functions
		* can start looking into the docker stuff

## Minutes: 2022-03-29
### scribe: Ashlyn
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
## Sprint 3 Scrum Meeting
 * standup
 	* Keelan
 		* got filtering for device and device types done
 		* working on filters for building and users
 		* then, will be working on user endpoints
 	* Kenny
 		* finished auth service in backend
 		* need user functionality for other tasks
 		* csv writter service is in progress
 	* Ashlyn
 		* working on adding success/warning/error messages to frontend
 		* frontend user tasks are blocked by user endpoints
 		* will start doing research on jwt and frontend auth, then will start on those tasks
 		* found a bug in backend, will get it logged and Kenny will look into it
 	* Joe
 		* last building endpoint in PR
 		* looking into updating device endpoints for building validation
 		* after that, look at other backend tasks
 	* Tom
 		* blocked by last building endpoint, but should be merged soon
 		* working on integrating building API call into add device page
 		* found a bug with warning message component and assigned to self
 		* looking into EXIF data extraction, there are libraries out there
 * we need to think about getting a docker file setup, will probably do that next sprint
 * if some of us have time, will start looking into incorporating a map view
 * will meet next on Thursday the 31st

## Minutes: 2022-03-17
### scribe: Ashlyn
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
## Sprint 3 Scrum Meeting
### Sprint load check in 
 * Tom
 	* 13 open tasks
 * Ashlyn
 	* 10 open tasks
 * Keelan 
 	* 6 open tasks
 * Kenny
 	* 6 open tasks (still need to make authentication tasks)
 * Joe
 	* 11 open tasks

### Action Items
 * Some tasks need title updated to be clear
 * Tasks still need to be made for some stories
 * Add notes to Blocked column or make a message in the scrum channel for blocked issues
 * Meet next Wednesday @ 6 for quick scrum

## Minutes: 2022-03-16
### scribe: Ashlyn
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
## Sprint 3 Planning:
### Scope
 * Frontend work
	* Integrate api calls to existing pages
	* Integrate adding/updating/removing photo/documents
	* Add remaining pages, like user and building
	* Error messages shared component and add them to needed pages
	* Authenication integration/any other validation
	* Filtering
 * Backend work
 	* Building and user endpoints
 	* Finish photo/files storage
 	* Authentication endpoints and validation code
 	* Filtering functionality in mongo and endpoints
 	* Export to csv
 * Other
	* If any of us have time, start on manuals, readme, and/or how-to articles for developing BEIMA

### Additional Items
 * Spring break will be a bonus week and no one will be required to produce work during that week.
 * For tasks with dependencies, we will have a "blocked" column in our board to indicate tasks that are blocked, as well as 
communicating in the scrum discord channel to left everyone know.

## Minutes: 2022-03-15
### scribe: Kenny
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
## Sprint 2 Retro:
### Overall Thoughts
Very happy with the increased amount of tasks completed.
We did not meet sprint goal. Remaining tasks are very close
to being completed that will allow sprint goal to be met however.
### What went wrong
Task prioritization could have been better.
Big pr's could have been split out better.
Planned to many tasks in the sprint.
Tasks descriptions could be better.
Tests were broken on build machine, but worked on vm's
and personal pc's.
### What went well
We doubled the amount of completed tasks.
The work we are commiting is very good quality.
Code reviews have been really good.
### What can be improved
We should add more columns in the task board to show if a task is blocked.
Communication can still be improved.
Frontend tests will be disabled as they will sometimes not work only
on the build machine.
### Other notes:
PR Auther should be the one to close branch.
We should go back to merging into main instead of a sprint branch.
We should use tasknumber_description for branch names.
Reminder to use the chosen pr format message.
## New Roles:
Scribe - Ashlyn \
Product Owner - Kenny \
Scrum Master - Joseph \
Dev - Tom \
Dev - Keelan

## Minutes: 2022-03-10
### scribe: Kenny
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
## Standup
* Ashlyn
	* Continuing work on updating pages with device calls.
	* Working on Creating building pages
* Tom
	* Working on js connector for update deivce ypt
	* Working on user pages
* Joseph
	* Working on verifying device and device type fields match
* Keelan
	* Working on Building out poco for building and user
	* Implementing mongo connector for building and user documents.
* Kenny
	* Finish testing for storage provider
	* Working on converting device class to c# object
## Other
* We should try not to create any new pull requests on Tuesday.

## Minutes: 2022-03-8
### scribe: Kenny
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
### Standup
* Kenny
	* Working on hooking storage into endpoints
* Keelan
	* Working on pocos for user and mongo connection calls
* Tom
	* Working on device types warning/errors
* Joseph
	* Working on building endpoints
* Ashlyn
	* Working on hooking pages up with api calls

## Minutes: 2022-03-4
### scribe: Kenny
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
	Brian Emtman
### Sponsorship Meeting
#### Overview
Gave demo to sponsorship, recieved feedback, and asked questions
#### Feedback
* Sponsor likes our current design of the UI.
* Sponsor would like to have filtering/pagination added to list views
	* Emphasised flexibility for this
	* Would like ability to export a csv of this data
* Asked about integrating with Microsoft Power BI
* Said to present a warning if longitude and latitude is off campus, but this can be the case
	
## Minutes: 2022-03-3
### scribe: Kenny
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
### Standup
* Kenny
	* Fix Cors issue
	* Fix typo and remove Homepage
	* Work on storage provider

* Ashlyn
	* Get PR from pulled over work up
	* Update pages to call the backend API

* Keelan
	* Add file and photo properties to the device schema

* Joseph
	* Get PR for device endpoints up
	* Work on device type endpoints

* Tom
	* Add longitude and latitude validation\
	* Update list pages to call backend API

### Other Notes
#### Tomorrow's Sponsorship Meeting
* Should ask about longitude and latitude radius for validation

## Mintues: 2022-03-2
### scribe: Kenny
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
### Assigned Roles:
	Kenny: Scribe
	Keelan: Scrum Master
	Joseph: Product Owner
### Spring 2 Planning
#### Sprint Goals/Tasks
* Finish up js connector calls to existing CRUD endpoints
* Finish up device type endpoints
* Finish upload/edit/view device functionality
* Setup Storage infrastrucutre for files/photos
	* Create CRUD endpoints for photos/images
	* Create storage service to interface between backend and minio server
	* Add to build/test script
* Reseach minio as object storage solution
	* Determine if it can act as an S3 compatible file server
	* Determine how to send and get objects 
* Create validation on input fields
* Create submit warning popup
	* Shared popup component
	* Page specific text 	
* Create basic user mangagement functionaltiy and page
* Create building CRUD endpoints
* Fix CORS crossite error


## Minutes: 2022-03-1
### scribe: Kenny
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
### Sprint 1 Retrospective Minutes:
#### What went wrong
	Pr size
	Inexperience with technology
	Co-dependencies causing blocking

#### What can be improved:
	Communication
	Not silioing
#### What went well:
	Pull request feedback
	Reaching out to help fix blockers
	The amount of work finished
### New Task Discussions
	Research minio object storage
	Fix cors errors
	Add frontend validation
	Pages for building, users, and auth
	Backend integration with frontend

## Minutes: 2022-02-24
### scribe: Joseph
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson

### Standup
* Tom
    * Working on GET request for frontend 
    * Check if the given object is valid and parse object
    * Will have a PR up very soon

* Kenny 
    * Made device/device types list view pages
    * Will need to communicate with Tom on type formatting
    * View individual device/device type pages created
    * Made help page

* Ashlyn
    * Doing add device/device type pages
    * Drag/Drop features with file type validation
    * Device types can add/remove associated fields
    * Primarily UI for now

* Joe
    * Working on the device create endpoint
    * Hitting a few issues but will try to get a PR up tonight
    * Will continue to work on remaining device endpoints

* Keelan
    * New functionality for device types in mongo connector
    * Possibly work on device type class for back end
    * Will have most everything done by the weekend

### Other Items
* Remove duplicate titles on pages.
* Ashlyn/Kenny need to coordinate UI themes

## Minutes: 2022-02-22
### scribe: Joseph
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson

### Standup
* Tom
    * Db connection github issues?
    * Worked on "backend of the frontend"/hitting API from frontend
    * Clarifying questions on react export
    * Need to test post content

* Ashlyn
    * Finished doing shared components
    * Put up a new PR to tidy up some css things
    * Added function to capitalize words of title
    * Wrote up coding standards document

* Kenny 
    * List views for device/device types
    * Single item views for device/device types
    * Setup Azure server stuff

* Joe
    * Added device GET endpoint
    * Endpoint infastructure/test in place
    * Will continue working on the remaining endpoints for device
    * Start on device type endpoints if there's time

* Keelan
    * Keelan finishing up pull request changes for device object
    * Create C# object for device types next
    * Add mongoDB support for device types

### Other Items
* Reassign tasks (had some confusion on who was working on what)
* Look at sharded device view page depending on type
* Device endpoints should be split up instead of using query string
* Start using a sprint branch instead of pushing straight to master

### Meetings
* Reschedule demo day for next week
* Will not need a Wednesday meeting this week
* Scheduled next standup for Thursday

## Minutes: 2022-02-16
### scribe: Joseph
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
	
### General
* Ashlyn made a checklist in google drive to track overall progress

### Sprint 1 Planning
* Started planning of Sprint 1
	* Assigned Scrum Roles:
		* Scrum Master: Tom
		* Product Owner: Keelan
		* Scribe: Joseph
	* Need to be able to upload/remove a device and its data
	* Need to have factories in the backend to translate data into BSON documents
	* Need to add necessary functionality to DB client
	* Need to create an initial frontend framework/template
		* Setup shared HTML components/templates
		* Add initial HTML pages
			* List view
			* Create Device
			* View/Edit Device
			* Create Type
			* View/Edit Type
	* Need to set up GET/POST interactions between frontend/backend
	* Add associated testing
	* Ashlynn/Kenny/Tom on front end, Joseph/Keelan on backend - (subject to change)

### Upcoming meetings:
* Thursday @15:10 - Meet with Henderson
* Tuesday @15:00 - Meet to re-evaluate sprint scope/standup

## Sprint 0 Retrospective Minutes: 2022-02-15
### Scribe: Tom
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson

### What went well
	* code review
	* all pieces were set up/all tasks complete

### What could be improved
	* delete branches automatically
	* when closing PR, consistent messages

### Decisions
	* Will be using the /// command to generate headers in Visual Studio
	* Ensure folder/file structure for tests is consistent with main structure
		*naming example.cs and example.test.cs, change namespaces in test files to reflect the test structure
	* When closing a PR the subject will be structured as "<description> Closes (#<linked item number>)"
		* if description is too long - "<description>... Closes (#<linked item number>)"
	* Part of the next sprint goal should include the front end being able to display a device (even if static/demo device)
	* Came up wth a planning process
		* discuss story and tasks one at a time so all are aware of what needs to be accomplished
		* make sure to coordinate with project plan/timeline

### Agenda/Action Items for next meeting
	* Delete orphaned branches (completed 2022-02-15 <Ashlyn>)
	* Place main branch protection back in effect (completed 2022-02-15 <Ashlyn>)
	* Exception handling: need to decide a standard
	* Workflow needs to be changed to "on PR" rather than "on push"
	* Workflow needs to be changed to ignore docs folder as shown in class



### Sprint goal template
	By the end of this sprint we will be able to run a
	simple version of our [app / webpage / data
	analysis / pipeline ] to demonstrate the initial
	functionality. We will have all interfaces between
	our [ components ] connected and roughed-in. We
	will have our testing framework implemented with
	1 unit test stub for each of our [ components ].

## Minutes: 2022-02-09
### scribe: Tom
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson

## standup
* Joe
    * working on test frameworks, pr soon
* Keelan
    * db schema plans progressing
    * sample schema is going well, pr soon

* Kenny
    * working on deployments hooked in, when branch merged it gets deployed to cloud
    * 2 more actions/yaml files will be needed

* Tom
    * remove restriction for PRs for sprint 0
    * looked at CI to see why the build.sh "wasn't found" but still failed with exit code 127 as expected
    * put in templates


* Ashlyn
    * pulled code down
    * trying to set up project
    * getting ready to look at cypress


## Action Items 
* see if we can reduce # of notifications  on PRs/CI - all

* look at putting reviewers on automatically to all PRs based on the template - Tom

## General
* VS code versions/upgrade/ignore version number on proj file
* nothing new from sponsor
* discussed rubric for build/test/ci submission

## Sprint 0 Objective

	By the end of this sprint we will be able to complile the 
	back end and launch the front end. The build, test, and CI 
	will run when creating a PR at a minimum. Testing frameworks 
	will be set up and have at least 1 test set up. PRs will 
	automatically request reviews from all other members and 
	templates will be accessible for selection when creating new 
	bugs, tasks, stories, and epics.

## Minutes: 2022-02-08
### scribe: Tom
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
	
* Making relevant branch names
* Covered what was already completed for front/back end initial setup
* Discussed functional testing being added as API calls
* Started planning of Sprint 0
	* Assigned Scrum Roles:
		* Scrum Master: Ashlyn
		* Product Owner: Joseph
		* Scribe: Tom
	* Need templates for Issue/Bug/Story
	* Create Task for the Build/Test/CI Setup
	* Minimal build.sh commands for node/DB installs
	* Set up a test stub for the back end
	* Add commands required for Build
	* Add commands required for Test
	* Cypress setup
	* React setup command
	* Minimal Database Schema
	* Base app setup stubs
	* Setup Main branch Protection

## Minutes: 2022-02-2
### scribe: Ashlyn
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson

* Worked on cleaning up remaining sections of the Project Plan
	* Each of us will read over the plan before class on Thursday so we can submit
* Discussed UI layout
	* Login page
	* Main page with left side navigation bar
	* Nav bar
		* List View off all entries with search
			* limited amount of entries per page
			* small map view on the right of the list
		* Single entry View
			* all entry data
			* small map with entry pin only
		* Add
		* Edit
		* Map (if we get to it)
	* Will use BSU colors
	* Made some mock ups to send to Brian

## Minutes: 2022-01-28
### scribe: Tom
### Attendees:
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
	Brian Emtman
	
## Items discussed:
* Introductions and history for each member
got history on our sponsor and what they want to accomplish
went over lisencing options and our suggestions for which to use
	* MIT decided  
* Went through the project plan draft to give Brian the overall view and timeline regarding our proposal
* Discussed data types needed to store information about the systems we will be catalogging
* Indicated that our system is going to be dynamic and allow custom associated fields for each piece
	* Brian wanted to be able to manually add geolocation if it was not contained in a photo metadata
	* Need to be able to store a manual or some other type of documentation to accompany the entry
		* most important capability would be to tie it to the specific device
* We were able to confirm that our Functional Requirements met his needs
	positively confirmed CSV files would be good for import/export
	Strongly indicated that a map interface is desired (we may not be able to call this a stretch)
	* wants to have the ability to filter the map based on chillers, HVAC, electrical, etc
	* we floated the idea of file storage based on geolocation
	* he floated the idea of exporting data in a configuration that could be imported into a google map interface
* Assumptions and Limitations were ok
* Showed the task decomp and how it tied to the timeline
mobile app/website are both acceptable, being able to load data on the road at all is desireable
	* Map over mobile is his preference
* If removing a device property, remove it across all devices of that type, warn with record count about removal to let the user make an informed decision
* Milestones and timeline were covered as well as an expectation being set to have a signoff on the project next week

* Request for schematic/diagram of the UI
	* not looking for anything super fancy, utilitarian is fine, just wants to see what we envision
* Comm frequency? 
	* wants to at least meet for the Milestone 3/4 demo, otherwise anything on Zoom if there's large issues to discuss will be played by ear
	* intermediate checkpoints? questions? email is fine to get answers, no need for official meetings to keep things moving


## Minutes: 2022-01-26
### scribe: Tom
#### Attendees: 
	Joseph Moore
	Kenny Miller
	Tom Hess
	Keelan Chen
	Ashlyn Adamson
			
### Items discussed
* Project Plan document - collaboration on completion during meeting
  * Want additional clarification for section 3.1
  * tabled some items for clarification and further development
* Team contract - polished and finished
* Discussed workarounds for not having to have our own VM during development
  * temporary DB instance to point development branch at until ready to deploy to actual VM
* how will testing be done?
  * front end
  * back end
  * automation options
  * frameworks
