// The only JavaScript in the page: where the map is on screen, needed to zoom and drag with the mouse.
export function rect(element) {
    const r = element.getBoundingClientRect();
    return [r.left, r.top, r.width, r.height];
}
