import time
from rx import Observable

BOUNDARY = (22*64) + 40

def get_next_state(state, next_zone):
    if next_zone == state["zone"]:
        return state

    next_time = time.time()
    event = None

    if next_zone == "low":
        event = {
            "type": "exit" if next_time < state["time"]+ 2 else "enter",
            "delta": 1
        }

    return {
        "zone": next_zone,
        "time": next_time,
        "event": event
    }

def get_zone(frame):
    print("{0} vs {1}".format(value, BOUNDARY))
    return "low" if value < BOUNDARY else "high"

def scan_frames(frames):
    initial_state = {
        "zone": "low",
        "time_entered": time.time(),
        "event": None
    }

    return frames.map(lambda x: get_zone(x)) \
        .scan(get_next_state, seed=initial_state) \
        .where(lambda x: x["event"] is not None) \
        .select(lambda x: x["event"])
