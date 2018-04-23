import time
import math
from collections import deque
from rx import Observable

BOUNDARY_MIN = (22*64)
BOUNDARY_OFFSET = 60
CURRENT_BOUNDARY = 0
FRAME_COLS = 8
BASELINE_INDEX = 20

class FrameCell:
    __slots__ = ['x', 'y', 'value', 'clusters', 'cluster_id', 'cluster_weight']
    def __init__(self, x, y, value):
        self.x = x
        self.y = y
        self.value = value
        self.clusters = dict()
        self.cluster_id = -1
        self.cluster_weight = 0

class FrameCluster:
    def __init__(self, cluster_id, cells):
        x_max = 0
        x_min = FRAME_COLS
        y_max = 0
        y_min = FRAME_COLS
        weight = 0
        count = 0
        x = 0
        y = 0

        for cell in cells:
            weight += cell.value
            count += 1
            x_max = max(x_max, cell.x)
            x_min = min(x_min, cell.x)
            y_max = max(y_max, cell.y)
            y_min = min(y_min, cell.y)
            x += cell.x * cell.value
            y += cell.y * cell.value

        x = max(x_min, math.floor(x / weight))
        y = max(y_min, math.floor(y / weight))

        self.x_max = x_max
        self.x_min = x_min
        self.y_max = y_max
        self.y_min = y_min
        self.weight = weight
        self.count = count
        self.x = x
        self.y = y
        self.cluster_id = cluster_id

def to_cells(frame):
    """Maps each element in the frame to (row, col, val)"""
    return [
        FrameCell(i // FRAME_COLS, i % FRAME_COLS, x)
        for (i, x)
        in enumerate(frame)
    ]

def get_top_cells(cells):
    cells_sorted = sorted(cells, key=lambda x: x.value)
    baseline = cells_sorted[BASELINE_INDEX].value
    cells_top = [
        x 
        for x 
        in (
            FrameCell(x.x, x.y, x.value - baseline)
            for x
            in cells_sorted[BASELINE_INDEX:] #Optimizing here...
        )
        if x.value > 1
    ]

    return cells_top

def to_matrix(cells):
    matrix = [([None] * FRAME_COLS) for x in range(FRAME_COLS)]
    
    for cell in cells:
        matrix[cell.x][cell.y] = cell

    return matrix

def create_cluster(cluster_id, cells):
    return FrameCluster(cluster_id, cells)

def get_surrounding_indexes(cell):
    idxs = [
        (cell.x - 1, cell.y - 1), (cell.x, cell.y - 1), (cell.x + 1, cell.y - 1),
        (cell.x - 1, cell.y),                           (cell.x + 1, cell.y),
        (cell.x - 1, cell.y + 1), (cell.x, cell.y + 1), (cell.x + 1, cell.y + 1)
    ]
    idxs = [i for i in idxs if (i[0] >= 0 and i[0] < FRAME_COLS and i[1] >= 0 and i[1] < FRAME_COLS)]

    return idxs

def get_neighbors(cells, cell, cluster_id):
    idxs = get_surrounding_indexes(cell)
    neighbors = [
        neighbor
        for neighbor
        in (cells[idx[0]][idx[1]] for idx in idxs)
        if neighbor is not None
    ]
    return neighbors

def set_cluster_assignment(cells, root_cell, cluster_id):
    neighbor_queue = deque()
    neighbor_queue.append((root_cell, get_neighbors(cells, root_cell, cluster_id)))
    visits = {root_cell}

    while len(neighbor_queue) > 0:
        (cell, neighbors) = neighbor_queue.popleft()

        neighbors = sorted((x for x in neighbors if x not in visits), key=lambda x: x.value, reverse=True)

        for neighbor in neighbors:
            distance = abs(cell.value - neighbor.value) / cell.value
            weight = cell.clusters[cluster_id] * (1 - min(distance / 2, 1))
            weight = max(0, weight - 0.1)
            neighbor.clusters[cluster_id] = weight

            visits.add(neighbor)

            neighbor_queue.append((neighbor, get_neighbors(cells, neighbor, cluster_id)))


def find_clusters(frame):
    # What is the baseline?
    cluster_count = 0
    clusters = dict()
    cells = to_cells(frame)
    cells_top = get_top_cells(cells)
    cells = to_matrix(cells_top)
    max_value = cells_top[len(cells_top) - 1].value if len(cells_top) > 0 else 1

    # Build cluster probabilities
    for cell in reversed(cells_top):
        if len(cell.clusters) > 0 and max(cell.clusters.values()) > 0.1:
            continue
        
        # Create a new cluster for this cell
        cell.clusters[cluster_count] = (cell.value / max_value) ** 3
        set_cluster_assignment(cells, cell, cluster_count)
        cluster_count += 1
    
    # Build clusters from most probable assignment
    for cell in cells_top:
        cluster_id = max(cell.clusters, key=cell.clusters.get)
        cluster_weight = cell.clusters[cluster_id]

        # Cut off low weighted cells
        if cluster_weight < 0.35:
            continue

        cell.cluster_id = cluster_id
        cell.cluster_weight = cluster_weight

        if not cluster_id in clusters:
            clusters[cluster_id] = []
        clusters[cluster_id].append(cell)

    # print_matrix(cells, lambda x: round(x.cluster_id, 2) if x.cluster_id >= 0 else "-")

    return [create_cluster(key, cells) for (key, cells) in clusters.items()]

def print_array(arr):
    print(", ".join((str(x) for x in arr)))


def print_row(row):
    print("---------------------")
    print_array((round(x, 2) for x in row))
    print("---------------------")

def print_matrix(cells, sel=None):
    if sel is None:
        sel = lambda x: x

    print("---------------------")
    for row in cells:
        print_array([(sel(x) if x is not None else "-") for x in row])
    print("---------------------")

def scan_frames(frames):
    initial_state = {
        "zone": "low",
        "entered": False,
        "time": time.time(),
        "event": None
    }

    return frames.map(find_clusters) \
        .map(to_matrix) \
        .do_action(lambda clusters: print_matrix(clusters, lambda x: x.cluster_id)) \
        .map(lambda x: None) \
        .where(lambda x: x is not None) 
