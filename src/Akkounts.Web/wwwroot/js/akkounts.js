const width = 960,
    height = 500,
    bubbleDataState = [{ radius: 0, fixed: true }, { account: "ACC0100", balance: 200 }, { account: "ACC0950", balance: 250 }],
    circleRadiusScale = d3.scale.sqrt()
        .domain([0, 250])
        .range([0, 50]),
    nodes = bubbleDataState.map(d => {
        d.radius = circleRadiusScale(d.balance);
        d.fixed = false;
        return d;
    }),
    root = nodes[0],
    color = d3.scale.category10(),
    svg = d3.select("body").append("svg")
        .attr("width", width)
        .attr("height", height);

root.radius = 0;
root.fixed = true;

const plot = () => {
    let circle = svg.selectAll("circle")
        .data(nodes.slice(1));

    circle.exit().remove();

    circle.enter().append("circle")
        .attr("r", d => d.radius)
        .style("fill", (d, i) => color(i % 3));

    force.nodes(nodes);
    tick();
    force.start();
}

const tick = () => {
    var q = d3.geom.quadtree(nodes),
        i = 0,
        n = nodes.length;

    while (++i < n) q.visit(collide(nodes[i]));

    svg.selectAll("circle")
        .attr("cx", d => d.x)
        .attr("cy", d => d.y);
}

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
    };
};

const force = d3.layout.force()
    .gravity(0.05)
    .charge((d, i) => i ? 0 : -2000)
    .nodes(nodes)
    .size([width, height])
    .on("tick", tick);

plot(nodes, svg);

window.setTimeout(() => {
    nodes.push({ account: "ACC0500", balance: 150, radius: circleRadiusScale(150), fixed: false });
    //nodes.pop();
    plot(nodes, svg);
}, 3000);

window.setTimeout(() => {
    nodes.pop();
    plot(nodes, svg);
}, 6000);

// const width = 1200,
//     height = 800,
//     format = d3.format(",d"),
//     color = d3.scaleOrdinal(d3.schemeCategory20c),
//     circleRadiusScale = d3.scaleSqrt()
//         .domain([0, 100])
//         .range([0, 100]),
//     bubble = d3.pack()
//         .size([width, height])
//         .padding(6),
//     svg = d3.select("body").append("svg")
//         .attr("width", width)
//         .attr("height", height)
//         .attr("class", "bubble"),
//     bubbleDataState = { children: [] };

// const plot = (data, svg) => {

//     if (data.children.length <= 0) return;

//     let root = d3.hierarchy(data)
//         .sum(d => d.balance)
//         .sort((a, b) => b.balance - a.balance);

//     bubble(root);

//     let node = svg.selectAll(".node")
//         .data(root.children);

//     node.exit().remove();

//     node.enter().append("g")
//         .attr("class", "node")
//         .attr("transform", d => "translate(" + d.x + "," + d.y + ")");

//     node.append("title")
//         .text(d => d.data.balance + ": " + format(d.value));

//     let sum = data.children.reduce((acc, elem) => acc + elem.balance, 0);
//     circleRadiusScale.domain([0, sum]);

//     node.append("circle")
//         .attr("r", d => circleRadiusScale(d.data.balance))
//         .style("fill", d => color(d.data.account));

//     node.append("text")
//         .attr("dy", ".3em")
//         .style("text-anchor", "middle")
//         .text(d => d.data.account.substring(0, d.r / 3));
// };

// d3.select(self.frameElement).style("height", width + "px");

//connect with server via signalr
const connection = new signalR.HubConnectionBuilder().withUrl("/Hubs/notificationHub").build();
//connection.start().then(() => alert("ok")).catch(err => console.error(err.toString()));

const addBubble = txnInfo => {
    bubbleDataState.children.push(txnInfo);
    plot(bubbleDataState, svg);
};

const removeBubble = account => {
    bubbleDataState.children = bubbleDataState.children.filter(o => o.account != account);
    plot(bubbleDataState, svg);
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
    if (bubbleData.account)
        addBubble(bubbleData);
    else
        removeBubble(bubbleData);
});