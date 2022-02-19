//const {MongoClient} = require('mongodb');
//const uri = "mongodb://cs481-database:MCHV2HiJIjb7TDWq9S9EtZ1JfRqUKhKEmfUuXUSUFaT5ROO44p4utNndvNX8ixtWOjeOL740fIQ6l7P44EYJJw==@cs481-database.mongo.cosmos.azure.com:10255/?ssl=true&retrywrites=false&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@cs481-database@"
//const client = new MongoClient(uri);
//
//async function main() {
//	try {
//		await client.connect();
//
//		await listDatabases(client);
//	 
//	} catch (e) {
//		console.error(e);
//	}
//	finally {
//		await client.close();
//	}
//}
//
//async function listDatabases(client){
//    databasesList = await client.db().admin().listDatabases();
// 
//    console.log("Databases:");
//    databasesList.databases.forEach(db => console.log(` - ${db.name}`));
//};
//
//main().catch(console.error);

/// <summary>
/// Gets a device with the specified ID
/// </summary>
/// <param name="objectId">The device ID to retrieve from the database (UUID)</param>
/// <returns>The device with the specified ID from the DB API</returns>
async function GetDevice(objectId) {
    try {
        const https = require('http');

        //https.get('http://localhost:7071/api/Function1/?objectId=' + objectId, response => {
        https.get('http://localhost:7071/api/Function1/?name=' + objectId, response => {
            let data = [];
            const headerDate = response.headers && response.headers.date ? response.headers.date : 'no response date';
            //console.log('DEBUG Status Code:', response.statusCode);
            //console.log('DEBUG Date in Response header:', headerDate);

            response.on('data', chunk => {
                data.push(chunk);
            });

            response.on('end', () => {
                //console.log('DEBUG Response ended: ');

                const device = JSON.parse(Buffer.concat(data).toString());
                //console.log("DEBUG " + Buffer.concat(data).toString());

                for (part of device) {
                    for (key in part) {
                        if (part.hasOwnProperty(key)) {
                            console.log("DEBUG Got device with key:  "    + part[key] + "");
                        }
                    }

                }

                return device;
            });
        }).on('error', err => {
            console.log('Error: ', err.message);
        });
    }
    catch (e) {
        console.error(e);
    }
}

async function InsertDevice(newDevice) {
    try {
        const https = require('http');

        var postData = JSON.stringify({
            'msg': 'Hello World!'
        });

        var request = https.request('http://localhost:7071/api/Function2/', (response) => {
            console.log('statusCode:', response.statusCode);
            console.log('headers:', response.headers);

            response.on('data', (d) => {
                console.log("attempted to insert");
                console.log("inserted: " + d);
                console.log();
                console.log();
            });

            return d;


        }).on('error', (e) => {
            console.error(e);
        });;

        req.write(postData);
        req.end();


    }
    catch (e) {
        console.error(e);
    }
}

GetDevice('620aeb22f50067dd0535bab1').catch(console.error);
InsertDevice().catch(console.error);
