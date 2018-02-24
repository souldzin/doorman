import time
import random
import math
from Adafruit_AMG88xx import Adafruit_AMG88xx

TYPE_REAL = "real"
TYPE_FAKE = "fake"

class Sensor:
    def __init__(self):
        self._sensor = Adafruit_AMG88xx()
        time.sleep(0.1)

    def get_frame(self):
        return self._sensor.readPixels()

class FakeSensor:
    def get_frame(self):
        time.sleep(0.1)
        return get_random_frame()

def get_random_value():
    val = random.random() * 30 + 15
    return math.floor(val * 10) / 10

def get_random_frame():
    return [get_random_value() for x in range(0,64)]

def get_sensor(t):
    if t == TYPE_REAL:
        return Sensor()
    elif t == TYPE_FAKE:
        return FakeSensor()
    else:
        legal_values = [TYPE_REAL, TYPE_FAKE]
        raise ValueError("Unexpected value for sensor type '{0}'. Legal values: '{1}'".format(t, legal_values))
