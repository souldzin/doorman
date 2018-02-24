const express = require('express');

function setupRouter(io) {
    const router = express.Router();

    router.post("/frame", (req, res) => {
        const data = req.body || {};

        // what if this isn't an array guys...
        if(!data.frame || !Array.isArray(data.frame)) {
            res.send({
                message: "Expected 'frame' in data"
            });
            res.sendStatus(400);
            return;
        }

        io.emit("frame", data.frame);
        res.send({});
    });

    return router;
}

module.exports = setupRouter;
