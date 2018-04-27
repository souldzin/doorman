from .sensor_file import FileSensor
from .sensor_amg88xx import AMG88xxSensor
from .sensor_random import RandomSensor

TYPE_FILE = "file"
TYPE_RANDOM = "random"
TYPE_REAL = "real"

def create_sensor(t, arg1):
    if t == TYPE_REAL:
        return AMG88xxSensor()
    elif t == TYPE_RANDOM:
        return RandomSensor()
    elif t == TYPE_FILE:
        return FileSensor(arg1)
    else:
        legal_values = [TYPE_FILE, TYPE_RANDOM, TYPE_REAL]
        raise ValueError("Unexpected value for sensor type '{0}'. Legal values: '{1}'".format(t, legal_values))
