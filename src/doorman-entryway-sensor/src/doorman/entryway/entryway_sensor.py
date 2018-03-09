EVENT_ENTER = "enter"
EVENT_EXIT = "exit"

def _create_event():
    return {
        "type": EVENT_ENTER
    }

def start_entryway_sensor(source, notify):
    source.map(lambda x: _create_event()) \
        .subscribe(lambda x: notify(x))
