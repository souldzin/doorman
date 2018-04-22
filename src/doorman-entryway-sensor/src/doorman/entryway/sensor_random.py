import random
import math
from rx import Observable

def get_random_value():
    val = random.random() * 30 + 15
    return math.floor(val * 10) / 10

def get_random_frame():
    return [get_random_value() for x in range(0,64)]

class RandomSensor:
    def get_frames(self, scheduler):
        return Observable.generate_with_relative_time(0,
            lambda x: True,
            lambda x: get_random_frame(),
            lambda x: x,
            lambda x: 70,
            scheduler = scheduler
            ) \
            .skip(1)
