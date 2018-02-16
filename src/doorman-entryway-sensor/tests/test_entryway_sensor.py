from unittest.mock import Mock, call
from rx import Observable
from doorman.entryway.entryway_sensor import start_entryway_sensor, EVENT_ENTER

def test_notifies_with_events():
    source = Observable.from_(["", "", ""])
    notify = Mock()

    start_entryway_sensor(source, notify)

    notify.assert_has_calls([call({ "type": EVENT_ENTER })] * 3)