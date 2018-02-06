# Design

## Overview

These are the major architectural components of the system, decomposed into two groups: Room Components and Cloud Components.

__Room Components__

The _Room Components_ are directly responsible for keeping track of the current occupancy in a single room. When a change is detected, this information is sent to the Occupancy Master. It is expected that these components will be deployed onto hardware inside the room. ([See room-hardware.md for specifics](./room-hardware.md)).

| Name | Description |
|------|-------------|
| Entryway Sensor | A sensor that captures entries and exits in a room.
| Displacement Sensor | A sensor that captures an approximate displacement of people in a room.
| Room Monitor | A micro computer that communicates with the sensors to track the current occupancy of a room.

__Server Components__

The _Server Components_ are responsible for managing the system, saving a history of room occupancy, and presenting the occupancy information in a usable dashboard. It is expected that these components will be deployed in a cloud environment.

| Name | Description |
|------|-------------|
| Occupancy Master | This server is the primary back-end of the system that exposes a secure API to communicate with the Room Monitor and Occupancy UI.
| Occupancy UI | This server is responsible for serving up a usable end-user UI for the system.

## IR Sensor Data

Both of the room sensors (i.e. the Entryway Sensor and the Displacement Sensor) connect to a low-res infrared thermal camera sensor. This sensor provides an array of 64 numbers that represent an 8x8 matrix of temperatures in Celsius.

We will use Python and the `Adafruit_AMG88xx` API to interface with the sensor.

Example:

```
data = sensor.readPixels()
print(data)

[
  20.75, 20.75, 21.75, 21.5, 21.5, 21.25, 20.75, 20.5,
  22.0, 21.0, 22.0, 21.0, 21.5, 21.5, 21.75, 21.5,
  21.0, 22.25, 21.25, 22.0, 21.75, 21.75, 21.5, 21.0,
  21.75, 21.5, 21.75, 21.5, 21.75, 21.25, 21.5, 21.75,
  22.0, 21.25, 21.5, 21.25, 21.75, 21.75, 21.25, 21.25,
  22.25, 22.25, 21.5, 21.75, 21.5, 21.5, 21.75, 21.75,
  22.0, 21.75, 22.0, 21.75, 22.25, 21.75, 21.75, 21.5,
  21.5, 22.0, 21.75, 21.75, 21.5, 21.75, 22.25, 21.0
]

```

## Entryway Sensor

__Deployable Artifact:__

_doorman-entryway-sensor.tgz_
  * Python code for entryway sensor

__Inputs:__

| What | From | Description |
|------|------|-------------|
| 8x8 sensor heatmap | IR Sensor | (see IR Sensor Data section)

__Outputs:__

| What | To | Description |
|------|----|-------------|
| `RoomOccupancyUpdate` | Room Monitor | A detected change in occupancy count

```
OccupancyUpdate {
  "type": "count"
  "change": 1
}
```

## Displacement Sensor

__Deployable Artifact:__

_doorman-displacement-sensor.tgz_
  * Python code for displacement sensor

__Inputs:__

| What | From | Description |
|------|------|-------------|
| "Activate" / "Deactivate" message | Room Monitor | A message to actiate or deactivate this sensor
| 8x8 sensor heatmap | IR Sensor | (see IR Sensor Data section)

__Outputs:__

| What | To | Description |
|------|----|-------------|
| `RoomOccupancyUpdate` | Room Monitor | The current displacement in the room (see IR Sensor Data)

```
RoomOccupancyUpdate {
  "type": "displacement"
  "value": [...] // see IR Sensor Data
}
```

## Room Monitor 

__Deployable Artifact:__

_doorman-room-monitor.tgz_
  * Python code for room monitor

__Inputs:__

| What | From | Description |
|------|------|-------------|
| `RoomOccupancyUpdate` | Entryway Sensor | A detected change in occupancy count
| `RoomOccupancyUpdate` | Displacement Sensor | The current displacement in the room (see IR Sensor Data)
| Reset | Occupancy Master | A message to reset the current state of the Room Monitor.

__Outputs:__

| What | To | Description |
|------|----|-------------|
| `RoomOccupancySnapshot` | Occupancy Master | An object containing the current occupancy count and displacement array.

```
// NOTE:
//   - The Occupancy Master will be responsible for setting the timestamp before saving it

RoomOccupancySnapshot {
  "count": 17,
  "displacement": [...]
}
```

## Occupancy Master

__Deployable Artifact:__

_doorman-occupancy-master.tgz
  * Python code for UI
  * PostgreSQL Schema and Db migration scripts

__Inputs:__

| What | From | Description |
|------|------|-------------|
| `RoomOccupancyShapshot` | Room Monitor |
| Real-time Connection | Client | This is a connection to receive real-time updates through WebSockets
