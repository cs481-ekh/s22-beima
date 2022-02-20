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

var DEBUG = true;
var debugUrl = 'http://localhost:7071/api/';
var prodURL = '';


/// <summary>
/// Gets a device with the specified ID
/// </summary>
/// <param name="objectId">The device ID to retrieve from the database (UUID)</param>
/// <returns>The device with the specified ID from the DB API</returns>
async function GetDevice(deviceId) {
    try {
        const https = require('http');

        var resource = DEBUG ? debugUrl + 'Device/?id=' + deviceId : prodURL + 'Device/?id=' + deviceId;

        https.get(resource, response => {
            let data = [];
            const headerDate = response.headers && response.headers.date ? response.headers.date : 'no response date';
            //console.log('DEBUG Status Code:', response.statusCode);
            //console.log('DEBUG Date in Response header:', headerDate);

            response.on('data', chunk => {
                data.push(chunk);
            });

            

            response.on('end', () => {
                //console.log('DEBUG Response ended: ');

                var response = Buffer.concat(data).toString()

                if (response === 'Invalid id.' || response === "Device could not be found." ) {
                    console.log(response);
                    return response;
                } else {
                    const device = JSON.parse(response);
                    //console.log("DEBUG " + Buffer.concat(data).toString());
                    console.log(device);
                    for (part of device) {
                        for (key in part) {
                            if (part.hasOwnProperty(key)) {
                                console.log("DEBUG Got device with key:  " + part[key] + "");
                            }
                        }

                    }
                    return device;
                }
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

        var resource = DEBUG ? debugUrl + 'InsertDevice/?deviceId=' + newDevice : prodURL + 'InsertDevice/?deviceId=' + newDevice;

        //var request = https.request('http://localhost:7071/api/Function2/?queryStringVar=' + JSON.stringify(newDevice)
        var request = https.request(resource, (response) => {
            //console.log('DEBUG statusCode:', response.statusCode);
            //console.log('DEBUG headers:', response.headers);

            response.on('data', (data) => {
                console.log();
                console.log("DEBUG attempted to insert");
                console.log("DEBUG inserted: " + data);
                console.log();                

                return data;
            });
        })

        request.end();
    }
    catch (e) {
        console.error(e);
    }
}

async function DeleteDevice(deviceId) {
    try {
        const https = require('http');

        var resource = DEBUG ? debugUrl + 'DeleteDevice/?deviceId=' + deviceId : prodURL + 'DeleteDevice/?deviceId=' + deviceId;

        var request = https.request(resource, (response) => {
            //console.log('DEBUG statusCode:', response.statusCode);
            //console.log('DEBUG headers:', response.headers);

            response.on('data', (data) => {
                console.log();
                console.log("DEBUG attempted to delete: " + deviceId);
                console.log("DEBUG deleted?: " + data);
                console.log();

                return data;
            });
        })

        request.end();
    }
    catch (e) {
        console.error(e);
    }
}

async function UpdateDevice(device) {
    try {
        const https = require('http');

        var string = JSON.stringify(device);

        var resource = DEBUG ? debugUrl + 'UpdateDevice/?device=' + deviceId : prodURL + 'UpdateDevice/?device=' + deviceId;

        var request = https.request(resource, (response) => {
            //console.log('DEBUG statusCode:', response.statusCode);
            //console.log('DEBUG headers:', response.headers);

            response.on('data', chunk => {
                data.push(chunk);
            });

            response.on('end', () => {
                //console.log('DEBUG Response ended: ');

                const device = JSON.parse(Buffer.concat(data).toString());
                //console.log("DEBUG " + Buffer.concat(data).toString());
                console.log(device);
                //for (part of device) {
                //    for (key in part) {
                //        if (part.hasOwnProperty(key)) {
                //            console.log("DEBUG Got device with key:  " + part[key] + "");
                //        }
                //    }

                //}

                return device;
            });
        })

        request.end();
    }
    catch (e) {
        console.error(e);
    }
}



GetDevice('620aeb23f50067dd0535bab3').catch(console.error);
//InsertDevice('[{ name: \'deviceTypeId\', value: \'testInsert\' },{ name: \'serialNumber\', value: \'insert12345\' }]').catch(console.error);
//DeleteDevice('620b24c100319b2622228230').catch(console.error);
//UpdateDevice('[{ name: \'_id\', value: \'620aeb22f50067dd0535bab1\' },{ name: \'deviceTypeId\', value: \'a\' },{ name: \'serialNumber\', value: \'b12345\' }]').catch(console.error);
//UpdateDevice('[{ name: \'serialNumber\', value: \'b12345\' }]').catch(console.error);
