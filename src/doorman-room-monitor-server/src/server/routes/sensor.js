const express = require('express');

module.exports = function setupSensorRouter({ monitor, io }) {
    const router = express.Router();

    router.post('/heartbeat', (req, res) => {
        io.emit('heartbeat', {});
        console.log(`[room-monitor] - received sensor heartbeat ${new Date()}`);

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
