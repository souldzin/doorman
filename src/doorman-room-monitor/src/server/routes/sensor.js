const express = require('express');

module.exports = function setupSensorRouter({ monitor }) {
    const router = express.Router();

    router.post("/frame", (req, res) => {
        const data = req.body || {};

        return monitor.pushFrame(data.frame)
            .then((x) => {
                res.send({})
            })
            .catch((e) => {
                res.status(400)
                    .json({
                        error: e
                    })
                    .end();
            });
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
