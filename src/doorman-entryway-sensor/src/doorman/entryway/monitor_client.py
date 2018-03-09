from urllib.parse import urljoin
import requests

class MonitorClient:
    def __init__(self, base_url):
        self._base_url = base_url

    def post_frame(self, frame):
        url = urljoin(self._base_url, "sensor/frame")
        data = {
            'frame': frame   
        }


        r = requests.post(url, json=data)
        r.raise_for_status()

    def post_event(self, event_type, delta):
        url = urljoin(self._base_url, "sensor/event")
        data = {
            'type': event_type,
            'delta': delta
        }

        r = requests.post(url, json=data)
        r.raise_for_status()
    