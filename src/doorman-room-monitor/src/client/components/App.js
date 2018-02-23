import React from "react";

import CountDisplay from "./CountDisplay";
import {FrameDisplay} from "./Frame";

function getRandomFrame() {
    return Array(64).fill(0).map(() => Math.floor(Math.random() * 30 + 15));

}

class App extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            count: 11,
            frame: getRandomFrame()
        };
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
