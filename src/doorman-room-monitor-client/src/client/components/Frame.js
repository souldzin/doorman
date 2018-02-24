import React from "react";
import tinycolor from "tinycolor2";

const VALUE_LOW = 22;
const VALUE_RANGE = 18;
// VALUE_HIGH = VALUE_LOW + VALUE_RANGE

function getFrame(props) {
    const propsFrame = props.frame || [];
    const frame = Array(64).fill(0).map((x, i) => propsFrame[i] || x);

    return frame;
}

function getFrameRows(frame) {
    return Array(8).fill(0).map((x, i) => {
        const start = i * 8;
        const end = start + 8;
        return frame.slice(start, end); 
    });
}

function getColor(value) {
    const per = Math.min(Math.max(0, value - VALUE_LOW), VALUE_RANGE) / VALUE_RANGE;
    const perf = Math.floor(per * 100) / 100;
    const red = 30 + perf * (200);
    const green = 90 - (perf) * 90;
    const blue = 230 - perf * (230);

    return tinycolor({ r: red, g: green, b: blue });
}

export function FrameCell({ value }) {
    const color = getColor(value);
    const style = {
        backgroundColor: color.toHexString()
    };
    return (
        <div className="frame-cell" style={style}>
            <span>{value}</span>
        </div>    
    );
}

export function FrameRow(props) {
    return (
        <div className="frame-row">
            {props.values.map((x, i) => <FrameCell value={x} key={i} />)}
        </div>
    );
}

export function FrameDisplay(props) {
    const frame = getFrame(props);
    const frameRows = getFrameRows(frame).map((x, i) => <FrameRow values={x} key={i} />);
    return (
        <div className="frame-display">
            {frameRows}
        </div>
    );
}
