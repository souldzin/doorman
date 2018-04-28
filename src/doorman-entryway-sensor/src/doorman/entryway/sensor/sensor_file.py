import json
from rx import Observable

class FileSensor:
    def __init__(self, filePath):
        with open(filePath) as data_file:
            data = json.load(data_file)

        self._frames = data["frames"]

    def get_frames(self, scheduler=None, rel_time=120):
        return Observable.generate_with_relative_time(0, 
            lambda x: x < len(self._frames),
            lambda x: x + 1,
            lambda x: self._frames[x],
            lambda x: rel_time,
            scheduler=scheduler
        )
