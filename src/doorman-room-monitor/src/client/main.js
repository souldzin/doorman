import React from "react";
import ReactDOM from "react-dom";
import '../assets/index.html';
import '../../client-config.js';

function App() {
    return (
        <div>
            <h1>Really... I'm Loaded!</h1>
        </div>
    );
}

ReactDOM.render(<App />, document.getElementById("app"));