import React from "react";

import CountDisplay from "./CountDisplay";
import {FrameDisplay} from "./Frame";
import io from "socket.io-client";

class App extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            count: 11,
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

        socket.on('frame', (frame) => {
            this.setState({
                frame                
            })
        });

        this._socket = socket;
    }

    render() {
        const {count, frame} = this.state;

        return (
            <div className="home">
                <div className="home-item big-text">
                    <CountDisplay count={count} />
                </div>
                <div className="home-item">
                    <FrameDisplay frame={frame} />
                </div>
            </div>
        );
    }
}

export default App;
