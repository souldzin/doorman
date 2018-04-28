import pytest
import os
from doorman.entryway.scanner import FrameScanner
from doorman.entryway.sensor import FileSensor

TEST_RESOURCES = os.path.join(os.path.dirname(os.path.abspath(__file__)), "resources")

@pytest.fixture()
def scanner():
    return FrameScanner()

def create_enter_event():
    return {
        'type': 'enter',
        'delta': 1
    }

def create_exit_event():
    return {
        'type': 'exit',
        'delta': 1
    }

@pytest.mark.parametrize("test_input,expected", [
    ("test1-entry-normal.json", [create_enter_event()]),
    ("test2-exit-normal.json", [create_exit_event()]),
    ("test3-nothing.json", []),
    ("test4-nothing-1.json", []),
    ("test5-nothing-2.json", []),
    ("test6-nothing-enter-exit.json", [create_enter_event(), create_exit_event()]),
    ("test7-entry-child.json", [create_enter_event()])
])
def test_frame_scanner_with_recording(scanner, test_input, expected):
    path = os.path.join(TEST_RESOURCES, test_input)
    sensor = FileSensor(path)
    frames = sensor.get_frames(rel_time=0)
    result = [x for x in scanner.scan(frames).to_blocking().to_iterable()]
    assert result == expected
