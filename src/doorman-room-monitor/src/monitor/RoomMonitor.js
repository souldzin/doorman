const Rx = require("rxjs");

const TYPE_ENTER = "enter";
const TYPE_EXIT = "exit";

class RoomMonitor {
    constructor() {
        const state = {
            count: 0,
            event: null
        };

        this._action$ = new Rx.Subject();
        this._subject$ = new Rx.BehaviorSubject();

        const state$ = Rx.Observable.merge(this._action$)
            .scan((currentState, action) => action(currentState), state);

        // subscribe to behavior subject so that we can get the value in getState()
        state$.subscribe(this._subject$);

        this.state$ = state$;
    }

    getState() {
        return this._subject$.getValue();
    }

    pushRoomEvent(event) {
        return this._validateRoomEvent(event)
            .then((event) => {
                this._action$.next(state => this._handleRoomEvent(state, event));
            });
    }

    _handleRoomEvent(state, {type, delta}) {
        const updateFn = type === TYPE_ENTER 
            ? (count) => count + delta
            : (count) => Math.max(0, count - delta);

        return {
            ...state,
            count: updateFn(state.count)
        };
    }

    _validateRoomEvent(change) {
        if(!change) {
            return Promise.reject({
                message: "Expected room event, but it was not found."
            });
        } else if(!change.type) {
            return Promise.reject({
                message: "Expected 'type' property, but it was not found."
            });
        } else {
            return Promise.resolve(change)
        }
    }
}

module.exports = RoomMonitor;
