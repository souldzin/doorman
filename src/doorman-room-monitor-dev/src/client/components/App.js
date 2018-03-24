import React from "react";

import CountDisplay from "./CountDisplay";
import {FrameDisplay} from "./Frame";
import io from "socket.io-client";
const download = require("downloadjs");

class App extends React.Component {
    constructor(props) {
        super(props);

        this._save = [];
        this.state = {
            count: 0,
            frame: [],
            isRecording: false
        };
    }

    componentWillMount() {
        this._connectToSocket();
    }

    componentWillUnmount() {

    }

    toggleRecording() {
        if(this.state.isRecording) {
            this.stopRecording();
        } else {
            this.startRecording();
        }
    }

    stopRecording() {
        this.setState({
            isRecording: false
        });
        const frames = this._save;
        this._save = [];

        const data = JSON.stringify({
            frames
        });
        download(data, "recording.json", "application/json");
    }

    startRecording() {
        this.setState({
            isRecording: true
        });
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

            if(this.state.isRecording) {
                this._save.push(frame);
            }
        });

        this._socket = socket;
    }

    render() {
        const {count, frame, isRecording} = this.state;

        return (
            <div>
                <div className="controls">
                    <button className="btn" onClick={() => this.toggleRecording()}>
                        {!isRecording ? "Start Recording" : "Stop"}                    
                    </button>
                </div>
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
                </div>
            </div>
        );
    }
}

export default App;
