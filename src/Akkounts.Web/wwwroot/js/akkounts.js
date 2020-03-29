﻿//connect with server via signalr
const connection = new signalR.HubConnectionBuilder().withUrl("/Hubs/notificationHub").build();
connection.start().then(() => alert("ok")).catch(err => console.error(err.toString()));

const addBubbleEvents = Bacon.fromBinder(sink => {
    connection.on("ReceiveTxnInfo", txnInfo => {
        sink(() => {
            initial_data.children.push(txnInfo);
            plot(initial_data, svg);
        });
    });
});

const removeBubbleEvents = Bacon.fromBinder(sink => {
    connection.on("ReceiveIdleInfo", actorName => {
        sink(() => {
            initial_data.children = initial_data.children.filter(o => o.account != actorName);
            plot(initial_data, svg);
        });
    });
});

const bubbleEvents = addBubbleEvents.merge(removeBubbleEvents);
bubbleEvents.onValue(f => f());

const width = 1200,
    height = 800,
    format = d3.format(",d"),
    color = d3.scaleOrdinal(d3.schemeCategory20c),
    circleRadiusScale = d3.scaleSqrt()
        .domain([0, 100])
        .range([0, 100]),
    bubble = d3.pack()
        .size([width, height])
        .padding(6),
    svg = d3.select("body").append("svg")
        .attr("width", width)
        .attr("height", height)
        .attr("class", "bubble"),
    initial_data = { children: [] };

const plot = (data, svg) => {

    if (data.children.length <= 0) return;

    let root = d3.hierarchy(data)
        .sum(d => d.balance)
        .sort((a, b) => b.balance - a.balance);

    bubble(root);

    let node = svg.selectAll(".node")
        .data(root.children);

    node.exit().remove();

    node.enter().append("g")
        .attr("class", "node")
        .attr("transform", d => "translate(" + d.x + "," + d.y + ")");

    node.append("title")
        .text(d => d.data.balance + ": " + format(d.value));

    let sum = data.children.reduce((acc, elem) => acc + elem.balance, 0);
    circleRadiusScale.domain([0, sum]);

    node.append("circle")
        .attr("r", d => circleRadiusScale(d.data.balance))
        .style("fill", d => color(d.data.account));

    node.append("text")
        .attr("dy", ".3em")
        .style("text-anchor", "middle")
        .text(d => d.data.account.substring(0, d.r / 3));
};

d3.select(self.frameElement).style("height", width + "px");