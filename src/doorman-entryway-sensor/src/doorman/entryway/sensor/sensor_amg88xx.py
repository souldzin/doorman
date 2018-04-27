import time
from Adafruit_AMG88xx import Adafruit_AMG88xx
from rx import Observable

class AMG88xxSensor:
    def __init__(self):
        self._sensor = Adafruit_AMG88xx()
        time.sleep(0.1)

    def get_frames(self, scheduler):
        return Observable.generate_with_relative_time(0,
            lambda x: True,
            lambda x: self._sensor.readPixels(),
            lambda x: x,
            lambda x: 50,
            scheduler = scheduler
            ) \
            .skip(1)
