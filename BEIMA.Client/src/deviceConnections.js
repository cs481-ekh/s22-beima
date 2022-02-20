var DEBUG = true;
var debugUrl = 'localhost';
var debugPort = 7071
var prodUrl = '';
var prodPort = '';


/// <summary>
/// Gets a device with the specified ID
/// </summary>
/// <param name="objectId">The device ID to retrieve from the database (UUID)</param>
/// <returns>The device with the specified ID from the DB API in JSON format, or the error message from the API</returns>
async function GetDevice(deviceId) {

  const axios = require('axios');

  var host = DEBUG ? debugUrl : prodUrl;
  var port = DEBUG ? debugPort : prodPort;

  axios.get("http://" + host + ":" + port + "/api/device/?id=" + deviceId).then(response => { return response.data; })
    .catch(function (error) {
      if (error.response) {
        return error.response.data;
    }
  });
}

async function InsertDevice(newDevice) {
  try {
    const https = require('http');
    const axios = require('axios');
    console.log(newDevice);

    axios.post('http://localhost:7071/api/device', newDevice).then((res) => {
      console.log(`Status: ${res.status}`);
      console.log('Body: ', res.data);
    }).catch((err) => {
      console.error(err);
    });



    //var options = {
    //    host: DEBUG ? debugUrl : prodUrl,
    //    port: DEBUG ? debugPort : prodPort,
    //    path: "/api/Device/?operation=insert",
    //    method: "POST",
    //    headers:
    //        { 'Content-Type': 'application/json' } ,
    //        //'Content-Length': newDevice.Length },
    //    json: true,
    //    body: newDevice
    //};
    //
    //console.log("options created");
    //
    //console.log(options);
    //
    //var request = https.request(options, (response) => {
    //    console.log('DEBUG statusCode:', response.statusCode);
    //    console.log('DEBUG headers:', response.headers);
    //
    //    response.on('data', (data) => {
    //        console.log();
    //        console.log("DEBUG attempted to insert");
    //        console.log("DEBUG inserted: " + data);
    //        console.log();                
    //
    //        return data;
    //    });
    //})
    //
    //request.end();
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


GetDevice('620aeb23f50067dd0535bab').catch(console.error);
GetDevice('620aeb23f50067dd0535bab3').catch(console.error);
GetDevice('620b24c100319b2622228230').catch(console.error);
//InsertDevice(JSON.parse('[{ \"name\": \"deviceTypeId\", \"value\": \"testInsert\" },{ \"name\": \"serialNumber\", \"value\": \"insert12345\" }]')).catch(console.error);
//DeleteDevice('620b24c100319b2622228230').catch(console.error);
//UpdateDevice('[{ name: \'_id\', value: \'620aeb22f50067dd0535bab1\' },{ name: \'deviceTypeId\', value: \'a\' },{ name: \'serialNumber\', value: \'b12345\' }]').catch(console.error);
//UpdateDevice('[{ name: \'serialNumber\', value: \'b12345\' }]').catch(console.error);

// Export to make them available outside
module.exports.GetDevice = GetDevice;
module.exports.InsertDevice = InsertDevice;
module.exports.DeleteDevice = DeleteDevice;
module.exports.UpdateDevice = UpdateDevice;