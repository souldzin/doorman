const express = require('express');

function getClientIP(req) {
    return (req.headers['x-forwarded-for']
        || req.connection.remoteAddress 
        || req.socket.remoteAddress
    );
}

module.exports = function setupSensorRouter({ monitor, io }) {
    const router = express.Router();

    router.post('/heartbeat', (req, res) => {
        io.emit('heartbeat', {});
        console.log(`[room-monitor] - received sensor heartbeat | ${getClientIP(req)} | ${new Date()}`);

        res.send({});
    });

    router.post("/frame", (req, res) => {
        const data = req.body || {};

        if(data.frame) {
            io.emit('frame', data.frame);
        }

        res.send({});
    });

    router.post("/event", (req, res) => {
        const data = req.body || {};

        return monitor.pushRoomEvent(data)
            .then((x) => {
                res.send({});
            })
            .catch((e) => {
                res.status(400)
                    .json({
                        error: e
                    })
                    .end();
            });
    });

    return router;
}
