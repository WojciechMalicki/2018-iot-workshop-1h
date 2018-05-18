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

function createMessage(devprefix="") {
  var dt = new Date();
  var ms = dt.getTime() / 10000;
  var devid =  Math.floor(Math.random() * 10);
  var data = null;
  var data = {
    Dt: dt,
    MsgType: "MAllNum",
    DeviceName: devprefix + "PC" + devid,
    Light: 1000 * Math.cos(ms / (175 + devid*4) ) * Math.sin(ms / 360) + Math.random() * 30,
    Potentiometer1: 1000 * Math.cos(ms / (275 + devid * 4)) * Math.sin(ms / (560 + devid * 8)) + Math.random() * 30,
    Potentiometer2: 1000 * Math.cos(ms / (60 + devid * 4)) * Math.sin(ms / (120 + devid * 4)) + Math.random() * 30,
    Pressure: (1000 * Math.cos(ms / (180 + devid * 4)) * Math.sin(ms / (1310 + devid * 14)) + Math.random() * 30),
    Temperature: (1000 * Math.cos(ms / (180 + devid * 4)) * Math.sin(ms / (110 + devid * 6)) + Math.random() * 30),
    ADC3: Math.random() * 1000.0,
    ADC4: Math.random() * 1000.0,
    ADC5: Math.random() * 1000.0,
    ADC6: Math.random() * 1000.0,
    ADC7: Math.random() * 1000.0,
    Altitude: (1000 * Math.cos(ms / (480 + devid * 4)) * Math.sin(ms / (2000 + devid * 22)) + Math.random() * 30),
    Latitude:  54.382059 - 0.3 + Math.random() * 0.6 ,
    Longitude: 18.607072 - 0.3 + Math.random() * 0.6
  };
  return data;
}

var connectCallback = function (err) {
  if (err) {
    console.log('Could not connect: ' + err);
  } else {
    console.log('Client connected');

    // Create a message and send it to the IoT Hub every second
    setInterval(function(){
        var data = createMessage("tk");
        var message = new Message(JSON.stringify(data));
        message.properties.add('temperatureAlert', (data.Temperature > 500) ? 'true' : 'false');
        console.log("Sending message: " + message.getData());
        client.sendEvent(message, printResultFor('send'));
    }, 100);
  }
};

client.open(connectCallback);

