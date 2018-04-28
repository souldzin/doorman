import sys
import time
import traceback
from rx import Observable
from rx.concurrency import ThreadPoolScheduler
from rx.core import Scheduler
from doorman.entryway.sensor import create_sensor
from doorman.entryway.sensor import TYPE_REAL
from doorman.entryway.monitor_client import MonitorClient
from doorman.entryway.scanner import FrameScanner

def print_usage():
    print(
"""--------------
Usage:
  python app.py <endpoint>

Arguments:
  - endpoint (Example: "http://localhost:9080/frame")
  - flag (Example: real | random | file)
"""
    )

def log(msg):
    if(msg):
        print("[entryway-sensor] " + str(msg))
    else:
        print("")

def log_error(e):
    print("An error occurred!")
    log("Unexpected error occurred {0}".format(e))
    traceback.print_exc()

def main():
    arg_endpoint = sys.argv[1] if len(sys.argv) > 1 else None
    arg_sensor_type = sys.argv[2] if len(sys.argv) > 2 else TYPE_REAL
    arg_sensor_arg = sys.argv[3] if len(sys.argv) > 3 else None
    is_debug = "--debug" in sys.argv

    if(not arg_endpoint):
        print("An error occurred! Expected the 'endpoint' argument.")
        print_usage()
        sys.exit(1)

    client = MonitorClient(arg_endpoint)
    scheduler = ThreadPoolScheduler(4)

    print("Starting '{0}' sensor...".format(arg_sensor_type))
    sensor = create_sensor(arg_sensor_type, arg_sensor_arg)
    
    print("Starting scanner...")
    scanner = FrameScanner()

    frames = sensor.get_frames(scheduler = scheduler)
    events = scanner.scan(frames)

    if is_debug:
        print("subscribing to frames...")
        frames.subscribe(
            on_next = lambda x: client.post_frame(x),
            on_completed = lambda: log("completed frames"),
            on_error = log_error
        )

    print("subscribing to events...")
    last_event = events.map(lambda x: client.post_event(x['type'], x['delta'])).to_blocking().last()

    print("completed.")

if __name__ == '__main__':
    main()
