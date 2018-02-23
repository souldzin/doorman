import React from "react";
import ReactDOM from "react-dom";

// import static files to let webpack deal with them...
import '../client-assets/index.html';
import '../client-assets/styles/main.less';
import '../../client.config.js';

function App() {
    return (
        <div className="home">
            <div className="home-item big-text">
                <span>11</span>
            </div>
            <div className="home-item">
                <p>{DOORMAN_CONFIG.API_URL}</p>
            </div>
        </div>
    );
}

ReactDOM.render(<App />, document.getElementById("app"));