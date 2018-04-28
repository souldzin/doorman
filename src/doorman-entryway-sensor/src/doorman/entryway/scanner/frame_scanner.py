import time
import math
from collections import deque
from rx import Observable
from .printers import *

FRAME_COLS = 8                     # Number of columns in a frame
BASELINE_INDEX = 20                # This index represents the n'th lowest value in a frame
CLUSTER_DISTANCE_THRESHOLD = 6     # Cluster pairs won't be considered if their distance exceeds this value
ENTER = 'enter'
EXIT = 'exit'

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
        self.x_delta = 0
        self.y_delta = 0
        self.weight_delta = 0
        self.life = 0
        self.orig = None
        self.type = None

    def set_delta(self, cluster_a):
        self.x_delta = cluster_a.x_delta / 1.5 + (self.x - cluster_a.x)
        self.y_delta = cluster_a.y_delta / 1.5 + (self.y - cluster_a.y)
        self.weight_delta = cluster_a.weight_delta / 2 + (self.weight - cluster_a.weight) / 2
        self.orig = cluster_a.get_orig().increment()
        return self

    def increment(self):
        if(self.life < 100):
            self.life += 1

        return self

    def get_orig(self):
        return self.orig if self.orig is not None else self

    def get_next_pos(self):
        return (self.x + self.x_delta, self.y + self.y_delta)

    def get_next_weight(self):
        return self.weight + self.weight_delta

def to_cells(frame):
    """Maps each element in the frame to (row, col, val)"""
    return [
        FrameCell(i // FRAME_COLS, i % FRAME_COLS, x)
        for (i, x)
        in enumerate(frame)
    ]

def to_matrix(cells):
    matrix = [([None] * FRAME_COLS) for x in range(FRAME_COLS)]
    
    for cell in cells:
        matrix[cell.x][cell.y] = cell

    return matrix

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

def create_cluster(cluster_id, cells):
    return FrameCluster(cluster_id, cells)

def get_neighbor_indexes(cell):
    idxs = [
        (cell.x - 1, cell.y - 1), (cell.x, cell.y - 1), (cell.x + 1, cell.y - 1),
        (cell.x - 1, cell.y),                           (cell.x + 1, cell.y),
        (cell.x - 1, cell.y + 1), (cell.x, cell.y + 1), (cell.x + 1, cell.y + 1)
    ]
    idxs = [i for i in idxs if (i[0] >= 0 and i[0] < FRAME_COLS and i[1] >= 0 and i[1] < FRAME_COLS)]

    return idxs

def get_neighbors(cells, cell, cluster_id):
    idxs = get_neighbor_indexes(cell)
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

def get_cluster_distance(a, b):
    (a_x, a_y) = a.get_next_pos()
    a_weight = a.get_next_weight()

    pos_distance = math.sqrt((a_x - b.x) ** 2 + (a_y - b.y) ** 2)
    weight_distance = abs(a_weight - b.weight) / a_weight
    weight_factor = 1 + min(weight_distance, 2)

    return pos_distance * weight_factor

def find_nearest_clusters(prev_clusters, next_clusters):
    """
    find_nearest_clusters

    Algorithm description:

      1. Calculate distance between each cluster pair
      2. For each pair, until they are all assigned, starting with the lowest distance:
        a. Assign
        b. Calculate deltas for new cluster
      3. For each remaining previous cluster
        a. Add (prev_cluster, None) pair
      4. For each remaining next cluster
        a. Add (None, next_cluster) pair
    """
    print("finding nearest clusters...")
    result = []
    prev_clusters_len = len(prev_clusters)
    next_clusters_len = len(next_clusters)
    prev_remaining = set(range(prev_clusters_len))    
    next_remaining = set(range(next_clusters_len))
    pairs = []

    for prev_idx, prev_cluster in enumerate(prev_clusters):
        for next_idx, next_cluster in enumerate(next_clusters):
            distance = get_cluster_distance(prev_cluster, next_cluster)
            pairs.append((prev_idx, next_idx, distance))

    pairs = sorted(pairs, key=lambda x: x[2])

    for (prev_idx, next_idx, distance) in pairs:
        if prev_idx not in prev_remaining or next_idx not in next_remaining:
            continue
        if len(prev_remaining) == 0 or len(next_remaining) == 0:
            break

        prev_cluster = prev_clusters[prev_idx]
        next_cluster = next_clusters[next_idx]

        prev_remaining.remove(prev_idx)
        next_remaining.remove(next_idx)

        result.append((prev_cluster, next_cluster.set_delta(prev_cluster)))

    for prev_idx in prev_remaining:
        prev_cluster = prev_clusters[prev_idx]
        result.append((prev_cluster, None))

    for next_idx in next_remaining:
        next_cluster = next_clusters[next_idx]
        result.append((None, next_cluster))

    return result    


def find_clusters(frame):
    """
    find_clusters

    Algorithm description:
    
      1. Find the most important cells in the frame (i.e. top cells)
      2. For each top cell in top cells (starting at the largest): 
        a. If cell doesn't have a strong cluster assignment:
          a.1. Recursively calculate the weight of neighboring cells belonging to that cell
        b. If cell has a strong cluster assignment: 
          b.1. Leave it alone
      3. Group each cell that has the same cluster assignment and create a cluster from these cells
         
         (This will give you a centroid and weight)
    """
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

def is_exit(c):
    orig = c.get_orig()

    if c.type != EXIT and c.x_max > 5 and c.x_delta > 0:
        orig.type = EXIT
        return True

    return False

def is_enter(c_from, c_to):
    if c_from is None:
        return None

    orig = c_from.get_orig()

    if orig.type != ENTER and orig.life > 3 and orig.x_min > 5 and c_from.x_delta < 0:
        orig.type = ENTER
        return True

    return False

def create_exit_event():
    return {
        'type': 'exit',
        'delta': 1
    }

def create_enter_event():
    return {
        'type': 'enter',
        'delta': 1
    }

def map_to_event(pair):
    c_from, c_to = pair

    if c_to is None:
        return create_exit_event() if is_exit(c_from) else None

    return create_enter_event() if is_enter(c_from, c_to) else None

class FrameScanner:
    def scan(self, frames):
        initial_state = []

        return (frames
            .map(find_clusters)
            .do_action(lambda clusters: print_matrix(to_matrix(clusters), lambda x: x.weight)) 
            .scan(lambda acc, x: find_nearest_clusters([c[1] for c in acc if c[1] is not None], x), seed=initial_state)
            .flat_map(lambda x: x)
            .map(map_to_event)
            .where(lambda x: x is not None) 
        )
