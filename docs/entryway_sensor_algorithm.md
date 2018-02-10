# Algorithm for Entryway Sensor 
--------------------------------

```javascript
/**
 * processFrame()
 * 
 * __Description:__
 * This function processes the given frame and state, returning a new state.
 *
 * __Arguments:__
 * :param state: This is an object that represents the current state.
 * :param frame: This is the screenshot that we get from the ircamera. It is a 64 element array, representing an 8x8 matrix of floats.
 *
 */
function processFrame(state, frame) {
    // 1) Let's find the current objects state
    objects = findObjects(frame, state.objects)

    // 2) Let's see if any events happened between the new object state and the old one
    events = findObjectEvents(objects, state.objects)

    return {
        objects: objects,
        events: events
    }
}

function notify(event) {
    http.post(monitorURL, {
        event: event
    });
} 

function main() {
    ircamera.frame$
        // 1) for every frame, let's process it with the current state
        .scan(
            // process the new state from each frame
            (state, frame) => processFrame(state, frame),

            // initialize the state
            {
                // This is an array of "objects". Each object has a center (x,y) point and a velocity vector.
                objects: [], 
                // This is an array of events that have happened in the frame (i.e. "entry" or "exit")
                events: [], 
            }
        )
        // 2) Let's look at just the events...
        .flatMap(
            (state) => state.events || []
        )
        // 3) And send a notification for each event
        .subscribe(
            (event) => notify(event)
        );
}
```
