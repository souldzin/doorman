const express = require("express");
const http = require('http');
const socket = require('socket.io');
const routes = {
    sensor: require('./routes/sensor.js')
}
const frameUtils = require('../utils/frames');

const ARGS = {
    port: Number(process.argv[2] || "9000")
};

const app = express();
const server = http.Server(app);
const io = socket(server);

app.use('/sensor', routes.sensor);

io.on('connection', function(socket) {
    console.log("a user has connected...");
});

// Start pinging random frames
// ---------------------------
setInterval(function(){
    io.emit('frame', frameUtils.getRandomFrame());    
}, 200);

// Start server 
// -----------------
server.listen(ARGS.port, () => {
    console.log(`doorman-room-monitor listening on port ${ARGS.port}.`);
});


