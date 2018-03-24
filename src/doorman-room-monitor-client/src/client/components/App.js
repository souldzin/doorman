import React from "react";

import CountDisplay from "./CountDisplay";
import {FrameDisplay} from "./Frame";
import io from "socket.io-client";

class App extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            count: 0,
            frame: []
        };
    }

    componentWillMount() {
        this._connectToSocket();
    }

    componentWillUnmount() {

    }

    _connectToSocket() {
        const socket = io(this.props.apiUrl);

        socket.on('connect', function(e){
            socket.emit('ready', 'I am ready!');
        });

        socket.on('state', (state) => {
            if(!state) {
                return;
            }
            const {count} = state;
            this.setState({
                count
            });
        });

        socket.on('frame', (frame) => {
            this.setState({
                frame
            });
        });

        this._socket = socket;
    }

    render() {
        const {count, frame} = this.state;

        return (
            <div className="home">
                <div className="home-item">
                    <h1>Occupancy</h1>
                    <div className="big-text">
                        <CountDisplay count={count} />
                    </div>
                </div>
                <div className="home-item">
                    <h1>Entryway Sensor 1</h1>
                    <FrameDisplay frame={frame} />
                </div>
                <div className="home-item">
                    <h1>Entryway Sensor 2</h1>
                    <FrameDisplay />
                </div>
                <div className="home-item">
                    <h1>Displacement Sensor</h1>
                    <FrameDisplay />
                </div>
            </div>
        );
    }
}

export default App;