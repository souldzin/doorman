import express from "express"

const ARGS = {
    port = Number(process.argv[2] || "9000")
};

const app = express();

app.get('/', (req, res) => res.send('Hello World!'));

app.listen(ARGS.port, () => {
    console.log(`doorman-room-monitor listening on port ${ARGS.port}.`);
});
