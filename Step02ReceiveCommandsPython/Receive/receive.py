
#import ptvsd
#ptvsd.enable_attach("my_secret", address = ('0.0.0.0', 3000))
#print("tcp://my_secret@rpi3:3000/")
#ptvsd.wait_for_attach()
import datetime
import random
import time
import os
import sys
import sys
import iothub_client
# pylint: disable=E0611
# (otherwise - false errors)
from iothub_client import IoTHubClient, IoTHubClientError, IoTHubTransportProvider, IoTHubClientResult, IoTHubClientStatus
from iothub_client import IoTHubMessage, IoTHubMessageDispositionResult, IoTHubError, DeviceMethodReturnValue
from iothub_client import IoTHubClientRetryPolicy, GetRetryPolicyReturnValue

# chose HTTP, AMQP, AMQP_WS or MQTT as transport protocol
PROTOCOL = IoTHubTransportProvider.MQTT
CONNECTION_STRING = "HostName=iottk20180514.azure-devices.net;DeviceId=PC01;SharedAccessKey=TDvyUZ2ypwa+e5TxuuSM8Kk7ZWVoH+MYEw1XTL8JQic="

RECEIVE_CONTEXT = 0
RECEIVE_CALLBACKS = 0
METHOD_CONTEXT = 0
METHOD_CALLBACKS = 0


def receive_message_callback(message, counter):
    global RECEIVE_CALLBACKS
    message_buffer = message.get_bytearray()
    size = len(message_buffer)
    print ( "Received Message [%d]:" % counter )
    print ( "    Data: <<<%s>>> & Size=%d" % (message_buffer[:size].decode('utf-8'), size) )
    map_properties = message.properties()
    key_value_pair = map_properties.get_internals()
    print ( "    Properties: %s" % key_value_pair )
    counter += 1
    RECEIVE_CALLBACKS += 1
    print ( "    Total calls received: %d" % RECEIVE_CALLBACKS )
    return IoTHubMessageDispositionResult.ACCEPTED

def device_method_callback(method_name, payload, user_context):
    global METHOD_CALLBACKS
    print ( "\nMethod callback called with:\nmethodName = %s\npayload = %s\ncontext = %s" % (method_name, payload, user_context) )
    METHOD_CALLBACKS += 1
    print ( "Total calls confirmed: %d\n" % METHOD_CALLBACKS )
    device_method_return_value = DeviceMethodReturnValue()
    device_method_return_value.response = "{ \"Response\": \"This is the response from the device\" }"
    device_method_return_value.status = 200
    return device_method_return_value

def send_confirmation_callback(message, result, user_context):
    print ( "IoT Hub responded to message with status: %s" % (result) )

def iothub_client_init():
    # prepare iothub client
    client = IoTHubClient(CONNECTION_STRING, PROTOCOL)
    client.set_message_callback(
        receive_message_callback, RECEIVE_CONTEXT)
    if client.protocol == IoTHubTransportProvider.MQTT or client.protocol == IoTHubTransportProvider.MQTT_WS:
        client.set_device_method_callback(
            device_method_callback, METHOD_CONTEXT)
    return client

if __name__ == '__main__':
    client = iothub_client_init()
    msg = ("{\"Dt\":\"%s\",\"MsgType\":\"MAllNum\",\"DeviceName\":\"testdev\",\"Light\":-965.0031270665472,\"Potentiometer1\":-85.72399644861996,\"Potentiometer2\":372.53954536189843,\"Pressure\":589.8832039951008,\"Temperature\":390.57400056210696,\"ADC3\":569.669765844746,\"ADC4\":201.32106328677546,\"ADC5\":948.2312285439696,\"ADC6\":540.9478191385655,\"ADC7\":186.8243615141212,\"Altitude\":-737.9231651110315,\"Latitude\":54.093053688095495,\"Longitude\":18.821393015218742}"
          % (datetime.datetime.now().isoformat()))
    client.send_event_async(IoTHubMessage(msg),send_confirmation_callback,None)
    #input("Waiting...")
    while True:
         time.sleep(2)

    
