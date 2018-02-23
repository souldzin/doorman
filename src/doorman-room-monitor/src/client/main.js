// import static files to let webpack deal with them...
import '../client-assets/index.html';
import '../client-assets/styles/main.less';
import '../../client.config.js';

import React from "react";
import ReactDOM from "react-dom";
import App from './components/App';

ReactDOM.render(<App />, document.getElementById("app"));