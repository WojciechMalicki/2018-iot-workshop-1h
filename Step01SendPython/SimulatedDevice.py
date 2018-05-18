# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

import datetime
import random
import time
import sys
import math

#Important library
# pip install azure-iothub-device-client azure-iothub-service-client
# 

import iothub_client
# pylint: disable=E0611
from iothub_client import IoTHubClient, IoTHubClientError, IoTHubTransportProvider, IoTHubClientResult
from iothub_client import IoTHubMessage, IoTHubMessageDispositionResult, IoTHubError, DeviceMethodReturnValue

# The device connection string to authenticate the device with your IoT hub.
# Using the Azure CLI:
# az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyNodeDevice --output table
CONNECTION_STRING = "HostName=iottk20180514.azure-devices.net;DeviceId=PC01;SharedAccessKey=TDvyUZ2ypwa+e5TxuuSM8Kk7ZWVoH+MYEw1XTL8JQic="

# Using the MQTT protocol.
PROTOCOL = IoTHubTransportProvider.MQTT

# Define the JSON message to send to IoT Hub.
MSG_TXT = ("{"
"\"Dt\": \"%s\", "
"\"MsgType\": \"MAllNum\", "
"\"DeviceName\": %s, "
"\"Light\": %.4f, "
"\"Potentiometer1\": %.4f, "
"\"Potentiometer2\": %.4f, "
"\"Pressure\": %.4f, "
"\"Temperature\": %.4f, "
"\"ADC3\": %.4f, "
"\"ADC4\": %.4f, "
"\"ADC5\": %.4f, "
"\"ADC6\": %.4f, "
"\"ADC7\": %.4f, "
"\"Altitude\": %.4f, "
"\"Latitude\":  %.6f , "
"\"Longitude\": %.6f "
"}")


def send_confirmation_callback(message, result, user_context):
    print ( "IoT Hub responded to message with status: %s" % (result) )

def iothub_client_init():
    # Create an IoT Hub client
    client = IoTHubClient(CONNECTION_STRING, PROTOCOL)
    return client

def createMessage(devprefix="tk"):
    devid = math.floor(random.random()*10)
    ms = time.time() * 1000
    device = "%sPC%d" % (devprefix,devid)

    msg_txt_formatted = MSG_TXT % (
    datetime.datetime.now().isoformat(),
    device,
    1000 * math.cos(ms / (175 + devid*4) ) * math.sin(ms / 360) + random.random() * 30,
    1000 * math.cos(ms / (275 + devid * 4)) * math.sin(ms / (560 + devid * 8)) + random.random() * 30,
    1000 * math.cos(ms / (60 + devid * 4)) * math.sin(ms / (120 + devid * 4)) + random.random() * 30,
    (1000 * math.cos(ms / (180 + devid * 4)) * math.sin(ms / (1310 + devid * 14)) + random.random() * 30),
    (1000 * math.cos(ms / (180 + devid * 4)) * math.sin(ms / (110 + devid * 6)) + random.random() * 30),
    random.random() * 1000.0,
    random.random() * 1000.0,
    random.random() * 1000.0,
    random.random() * 1000.0,
    random.random() * 1000.0,
    (1000 * math.cos(ms / (480 + devid * 4)) * math.sin(ms / (2000 + devid * 22)) + random.random() * 30), 
    54.382059 - 0.3 + random.random() * 0.6, 
    18.607072 - 0.3 + random.random() * 0.6
    )
    message = IoTHubMessage(msg_txt_formatted)
    return message


def iothub_client_telemetry_sample_run():

    try:
        client = iothub_client_init()
        print ( "IoT Hub device sending periodic messages, press Ctrl-C to exit" )

        while True:
            # Build the message with simulated telemetry values.

            
            message = createMessage();

            # Add a custom application property to the message.
            # An IoT hub can filter on these properties without access to the message body.
            prop_map = message.properties()
            prop_map.add("ABC", ("%.2f" % random.random()))

            # Send the message.
            print( "Sending message: %s" % message.get_string() )
            client.send_event_async(message, send_confirmation_callback, None)
            time.sleep(1)

    except IoTHubError as iothub_error:
        print ( "Unexpected error %s from IoTHub" % iothub_error )
        return
    except KeyboardInterrupt:
        print ( "IoTHubClient sample stopped" )

if __name__ == '__main__':
    print ( "IoT Hub Quickstart #1 - Simulated device" )
    print ( "Press Ctrl-C to exit" )
    iothub_client_telemetry_sample_run()