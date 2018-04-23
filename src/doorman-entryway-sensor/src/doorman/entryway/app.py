import sys
import time
import traceback
from rx import Observable
from rx.concurrency import ThreadPoolScheduler
from rx.core import Scheduler
from doorman.entryway.sensor import get_sensor
from doorman.entryway.sensor import TYPE_REAL
from doorman.entryway.monitor_client import MonitorClient
from doorman.entryway.scanner import scan_frames

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
        print("[entryway-sensor] " + str(msg))
    else:
        print("")

def log_error(e):
    log("Unexpected error occurred {0}".format(e))
    traceback.print_exc()

def main():
    arg_endpoint = sys.argv[1] if len(sys.argv) > 1 else None
    arg_sensor_type = sys.argv[2] if len(sys.argv) > 2 else TYPE_REAL
    arg_sensor_arg = sys.argv[3] if len(sys.argv) > 3 else None

    if(not arg_endpoint):
        print("An error occurred! Expected the 'endpoint' argument.")
        print_usage()
        sys.exit(1)

    client = MonitorClient(arg_endpoint)
    scheduler = ThreadPoolScheduler(4)

    print("Starting '{0}' sensor...".format(arg_sensor_type))
    sensor = get_sensor(arg_sensor_type, arg_sensor_arg)
    frames = sensor.get_frames(scheduler)
    events = scan_frames(frames)

    print("subscribing to events...")
    events.subscribe(
        on_next = lambda x: client.post_event(x['type'], x['delta']),
        on_completed = lambda: log("completed events"),
        on_error = log_error
    )

    print("subscribing to frames...")
    frames.subscribe(
        on_next = lambda x: client.post_frame(x),
        on_completed = lambda: log("completed frames"),
        on_error = log_error
    )

    scheduler.executor.shutdown()


if __name__ == '__main__':
    main()
