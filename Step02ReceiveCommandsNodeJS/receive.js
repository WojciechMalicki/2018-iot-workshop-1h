'use strict';

var clientFromConnectionString = require('azure-iot-device-mqtt').clientFromConnectionString;
var Message = require('azure-iot-device').Message;

var connectionString = 'HostName=iottk20180514.azure-devices.net;DeviceId=PC01;SharedAccessKey=TDvyUZ2ypwa+e5TxuuSM8Kk7ZWVoH+MYEw1XTL8JQic=';

var client = clientFromConnectionString(connectionString);

function printResultFor(op) {
  return function printResult(err, res) {
    if (err) console.log(op + ' error: ' + err.toString());
    if (res) console.log(op + ' status: ' + res.constructor.name);
  };
}


var connectCallback = function (err) {
  if (err) {
    console.log('Could not connect: ' + err);
  } else {
    console.log('Client connected');
    //client.onDeviceMethodRequest
    client.on('message',function(msg) {
        console.log('Id: ' + msg.messageId + ' Body: ' + msg.data);
        client.complete(msg, printResultFor('completed'));
      }
    );
    setInterval(function(){
      console.log("Waiting");
    }, 1000);
  }
};

client.open(connectCallback);

