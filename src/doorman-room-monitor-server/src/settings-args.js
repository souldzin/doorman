const yargs = require('yargs');

const ARG_MASTER = "master";
const ARG_PORT = "port";

const args = yargs
    .option(ARG_MASTER, {
        type: Boolean,
        default: true
    })
    .option(ARG_PORT, {
        type: Number,
        default: 9000
    })
    .argv;

module.exports = {
    args,
    ARG_MASTER,
    ARG_PORT
};
