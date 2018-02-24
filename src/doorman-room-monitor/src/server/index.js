const express = require('express');
const bodyParser = require('body-parser');
const http = require('http');
const socket = require('socket.io');
const routes = {
    sensor: require('./routes/sensor.js')
}
const frameUtils = require('../utils/frames');

const port = Number(process.argv[2] || "9000")
const app = express();
const server = http.Server(app);
const io = socket(server);

app.use(bodyParser.json());
app.use('/sensor', routes.sensor(io));

io.on('connection', function(socket) {
    console.log("a user has connected...");
});

// Start server 
// -----------------
server.listen(port, () => {
    console.log(`doorman-room-monitor listening on port ${port}.`);
});


