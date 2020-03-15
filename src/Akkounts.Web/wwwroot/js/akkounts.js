var connection = new signalR.HubConnectionBuilder().withUrl("/Hubs/notificationHub").build();

connection.start().then(function () {
    alert("ok");
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveTxnInfo", txnInfo => {
    initial_data.children.push(txnInfo);
    plot(initial_data, svg);
});

connection.on("ReceiveIdleInfo", actorName => {
    var node = document.createElement("LI");
    var textnode = document.createTextNode(`${actorName} - IDLE`);

    node.appendChild(textnode);
    document.getElementById("myList").appendChild(node);
});

var diameter = 960,
    format = d3.format(",d"),
    initial_data = {
        children: []
    },
    color = d3.scaleOrdinal(d3.schemeCategory20c);

var bubble = d3.pack()
    .size([diameter, diameter])
    .padding(1.5);

var svg = d3.select("body").append("svg")
    .attr("width", diameter)
    .attr("height", diameter)
    .attr("class", "bubble");

const plot = (data, svg) => {

    if (data.children.length <= 0) return;

    var root = d3.hierarchy(data)
        .sum(d => d.balance)
        .sort((a, b) => b.balance - a.balance);

    bubble(root);

    var node = svg.selectAll(".node")
        .data(root.children)
        .enter().append("g")
        .attr("class", "node")
        .attr("transform", d => {
            return "translate(" + d.x + "," + d.y + ")";
        });

    node.append("title")
        .text(d => d.data.balance + ": " + format(d.value));

    node.append("circle")
        .attr("r", function (d) { return d.r; })
        .style("fill", d => color(d.data.account));

    node.append("text")
        .attr("dy", ".3em")
        .style("text-anchor", "middle")
        .text(d => d.data.account.substring(0, d.r / 3));
};

d3.select(self.frameElement).style("height", diameter + "px");