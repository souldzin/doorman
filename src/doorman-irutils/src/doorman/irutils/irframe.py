import math as math

FRAME_WIDTH = 8
FRAME_HEIGHT = 8
FRAME_SIZE = FRAME_WIDTH * FRAME_HEIGHT

def to_index(x, y):
    """Converts x,y coordinate to an index on the frame"""
    return x + (y * FRAME_HEIGHT)

def to_point(index):
    y = index // FRAME_HEIGHT
    x = index % FRAME_HEIGHT    
    return (x, y)

def create_empty_frame(val = 20):
    return [val] * FRAME_SIZE

def find_points(frame, min_temp=20):
    """Find and return key points of (x,y) in an IR frame
    
    arguments:
    frame -- a 64 element array representing an 8x8 matrix of temperature
    """
    return []

def _update_frame_from_point(frame, point, low, high):
    med = low + math.floor((high - low) * 0.8)
    x, y = point
    x_start = max(x - 1, 0)
    x_end = min(x + 1, FRAME_WIDTH - 1)
    y_start = max(y - 1, 0)
    y_end = min(y + 1, FRAME_HEIGHT - 1)

    for nx in range(x_start, x_end + 1):
        for ny in range(y_start, y_end + 1):
            frame[to_index(nx, ny)] = med

    frame[to_index(x, y)] = high

    return frame


def create_frame_from_points(points, low=20, high=38):
    """Create and return a frame from the given points

    description:
    - The frame is initialized with the low value.
    - At each point, the values at and around the point are set 
      based on the center of the point.
    """
    frame = create_empty_frame(val = low)

    for point in points:
        _update_frame_from_point(frame, point, low, high)

    return frame
