import time
from rx import Observable

BOUNDARY = (22*64) + 60

def get_next_state(state, next_zone):
    prev_time = state["time"]
    prev_zone = state["zone"]
    next_time = time.time()
    event = None
    entered = state["entered"] if next_zone == "high" else False

    if prev_zone == next_zone:
        if next_zone == "high" and not entered and next_time > (prev_time + 2):
            entered = True
            event = {
                "type": "enter",
                "delta": 1
            }
        next_time = state["time"]
    elif next_zone == "low" and next_time < (prev_time + 2):
        event = {
            "type": "exit",
            "delta": 1
        }

    return {
        "zone": next_zone,
        "time": next_time,
        "event": event,
        "entered": entered
    }

def get_zone(frame):
    value = sum(frame)
    return "low" if value < BOUNDARY else "high"

def scan_frames(frames):
    initial_state = {
        "zone": "low",
        "entered": False,
        "time": time.time(),
        "event": None
    }

    return frames.map(lambda x: get_zone(x)) \
        .scan(get_next_state, seed=initial_state) \
        .where(lambda x: x["event"] is not None) \
        .select(lambda x: x["event"])
