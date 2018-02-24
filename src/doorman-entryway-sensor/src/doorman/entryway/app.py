import sys
import time
from rx import Observable
from rx.concurrency import ThreadPoolScheduler
from rx.core import Scheduler
from doorman.entryway.sensor import get_sensor
from doorman.entryway.sensor import TYPE_REAL
from doorman.entryway.monitor_client import MonitorClient

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
    client = MonitorClient(arg_endpoint)

    scheduler = ThreadPoolScheduler(2)

    frames = Observable.timer(100, period=100, scheduler=scheduler) \
        .map(lambda x: sensor.get_frame()) \
        .observe_on(Scheduler.event_loop)

    frames.subscribe(
        on_next = lambda x: client.post_frame(x),
        on_completed = lambda x: log("completed"),
        on_error = lambda e: log("Unexpected error occurred {0}".format(e))
    )

    scheduler.executor.shutdown()

if __name__ == '__main__':
    main()
