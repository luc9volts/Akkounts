class Bubble {
    static #randomColor = () => {
        let letters = '0123456789ABCDEF';
        let color = '#';
        for (var i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    };
    static #circleRadiusScale = d3.scale.sqrt()
        .domain([0, 10000])
        .range([0, 70]);

    #activeColor = "#66b3ff";
    #inactiveColor = "#b3b3b3";

    constructor(name, size) {
        this.name = name;
        this.size = size;
        this.color = this.#activeColor;//Bubble.#color();
    }

    setActive() {
        this.color = this.#activeColor;
    }

    setInactive() {
        this.color = this.#inactiveColor;
    }

    get radius() {
        return Bubble.#circleRadiusScale(this.size);
    }

    get text() {
        return `${this.name} ${this.size}`;
    }
}

const width = window.innerWidth,
    height = window.innerHeight,
    svg = d3.select("body").append("svg")
        .attr("width", width)
        .attr("height", height);

let circles = svg.selectAll('circle'),
    text = svg.selectAll('text'),
    bubbleDataState = [];

const collide = node => {
    var r = node.radius + 16,
        nx1 = node.x - r,
        nx2 = node.x + r,
        ny1 = node.y - r,
        ny2 = node.y + r;
    return function (quad, x1, y1, x2, y2) {
        if (quad.point && (quad.point !== node)) {
            var x = node.x - quad.point.x,
                y = node.y - quad.point.y,
                l = Math.sqrt(x * x + y * y),
                r = node.radius + quad.point.radius;
            if (l < r) {
                l = (l - r) / l * .5;
                node.x -= x *= l;
                node.y -= y *= l;
                quad.point.x += x;
                quad.point.y += y;
            }
        }
        return x1 > nx2 || x2 < nx1 || y1 > ny2 || y2 < ny1;
    }
};

const force = d3.layout.force()
    .size([width, height])
    .on('tick', () => {
        var q = d3.geom.quadtree(bubbleDataState),
            i = 0,
            n = bubbleDataState.length;

        while (++i < n) q.visit(collide(bubbleDataState[i]));

        circles
            .attr('cx', d => d.x)
            .attr('cy', d => d.y);

        text
            .attr("x", d => d.x)
            .attr("y", d => d.y);
    });

const plot = (items) => {

    circles = circles.data(items, d => d.name);
    text = text.data(items, d => d.name);

    circles.exit().transition().remove();
    text.exit().remove();

    circles
        .enter()
        .append('circle');

    circles
        .attr('fill', d => d.color)
        .attr('r', d => d.radius);

    text
        .enter()
        .append('text')
        .attr("dy", "1.3em")
        .style("text-anchor", "middle")
        .attr("font-family", "Gill Sans", "Gill Sans MT")
        .attr("fill", "black");

    text
        .text(d => d.text)
        .attr("font-size", d => d.radius / 5);

    force
        .nodes(items)
        .start();
}

//connect with server via signalr
const connection = new signalR.HubConnectionBuilder().withUrl("/Hubs/notificationHub").build();
connection.start().then(() => console.log("connected")).catch(err => console.error(err.toString()));

const addBubble = txnInfo => {
    let exists = bubbleDataState.some(e => e.name == txnInfo.account);

    if (!exists)
        bubbleDataState.push(new Bubble(txnInfo.account, txnInfo.balance));
};

const updateBubble = txnInfo => {
    let bubbleIndex = bubbleDataState.findIndex(e => e.name == txnInfo.account);

    if (bubbleIndex >= 0) {
        bubbleDataState[bubbleIndex].size = txnInfo.balance;
        bubbleDataState[bubbleIndex].setActive();
    }
};

const removeBubble = account => {
    //bubbleDataState = bubbleDataState.filter(o => o.name != account);
    let bubbleIndex = bubbleDataState.findIndex(e => e.name == account);

    if (bubbleIndex >= 0) 
        bubbleDataState[bubbleIndex].setInactive();
};

//serializing server events
const addBubbleEvents = Bacon.fromBinder(sink => {
    connection.on("ReceiveTxnInfo", txnInfo => sink(txnInfo));
});

const removeBubbleEvents = Bacon.fromBinder(sink => {
    connection.on("ReceiveIdleInfo", account => sink(account));
});

const bubbleEvents = addBubbleEvents.merge(removeBubbleEvents);

bubbleEvents.onValue(bubbleData => {

    if (!bubbleData.account)
        removeBubble(bubbleData);
    else {
        addBubble(bubbleData);
        updateBubble(bubbleData)
    }

    plot(bubbleDataState);
});