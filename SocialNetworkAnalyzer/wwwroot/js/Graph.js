var cyObject = null;
function Render() {
    cyObject = cytoscape({
        container: document.getElementById('cy'), // container to render in

        layout: {
            name: 'avsdf',
            nodeSeparation: 120
        },

        style: [
            {
                selector: 'node',
                style: {
                    'text-valign': 'center',
                    'color': '#000000',
                    'background-color': '#3a7ecf'
                }
            },

            {
                selector: 'edge',
                style: {
                    'width': 2,
                    'line-color': '#3a7ecf',
                    'opacity': 0.5
                }
            }
        ]
    });
}

function GetNodes(id) {
    var url = window.location.origin + '/Dataset/GetGraphNodes/' + id;

    $.ajax({
        url: url,
        async: true,
        type: "GET",
        success: function (data, status) {
            data.forEach(element => RenderNode(element));
        }
    });
}
function RenderNode(element) {
    cyObject.add({
        group: 'nodes',
        data: { id: element.id },
    });
}

function RenderEdges(element) {
    cyObject.add({
        group: 'edges',
        data: { target: element.target, source: element.source },
    });
}

function ResetGraph() {
    cyObject.nodes().forEach(function (ele) {
        ele.data().weight = Math.floor((Math.random() * 10) + 1);
    });

    var layout = cyObject.layout({
        name: 'circle',
        animate: true,
        sort: function (a, b) { return a.data('weight') - b.data('weight') },
        radius: 120,
        animationDuration: 1000,
        animationEasing: 'ease-in-out'
    });
    layout.run();
}
function GetEdges(id) {
    var url = window.location.origin + '/Dataset/GetGraphEdges/' + id;

    $.ajax({
        url: url,
        async: true,
        type: "GET",
        success: function (data, status) {
            data.forEach(element => RenderEdges(element));
            ResetGraph();
        }
    });
}