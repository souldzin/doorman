from urllib.parse import urljoin
import requests
from requests.adapters import HTTPAdapter

class MonitorClient:
    def __init__(self, base_url):
        s = requests.Session()
        s.mount("http://", HTTPAdapter(max_retries=5))
        s.mount("https://", HTTPAdapter(max_retries=5))

        self._base_url = base_url
        self._req = s

    def post_heartbeat(self):
        url = urljoin(self._base_url, "sensor/heartbeat")

        r = self._req.post(url)

    def post_frame(self, frame):
        url = urljoin(self._base_url, "sensor/frame")
        data = {
            'frame': frame   
        }

        r = self._req.post(url, json=data)
        r.raise_for_status()

    def post_event(self, event_type, delta):
        url = urljoin(self._base_url, "sensor/event")
        data = {
            'type': event_type,
            'delta': delta
        }

        r = self._req.post(url, json=data)
        r.raise_for_status()
    