function getRandomFrame() {
    return Array(64).fill(0).map(() => Math.floor(Math.random() * 30 + 15));
}

module.exports = {
    getRandomFrame
};
