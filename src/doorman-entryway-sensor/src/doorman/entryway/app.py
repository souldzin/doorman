import sys
import requests
import time
from doorman.entryway.sensor import get_sensor
from doorman.entryway.sensor import TYPE_REAL

def print_usage():
    print(
"""--------------
Usage:
  python app.py <endpoint>

Arguments:
  - endpoint (Example: "http://localhost:9080/frame")
  - flag (Example: sensor | random)
"""
    )

def log(msg):
    if(msg):
        print("[log] " + str(msg))
    else:
        print("")

def post_frame(frame, url):
    data = {
        'frame': frame   
    }

    r = requests.post(url, json=data)
    r.raise_for_status()

def main():
    arg_endpoint = sys.argv[1] if len(sys.argv) > 1 else None
    arg_sensor_type = sys.argv[2] if len(sys.argv) > 2 else TYPE_REAL

    if(not arg_endpoint):
        print("An error occurred! Expected the 'endpoint' argument.")
        print_usage()
        sys.exit(1)

    print("Starting '{0}' sensor...".format(arg_sensor_type))
    sensor = get_sensor(arg_sensor_type)

    while True:
        frame = sensor.get_frame()
        post_frame(frame, arg_endpoint)

if __name__ == '__main__':
    main()
