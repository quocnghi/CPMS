var clipboard_list = [];
var current_name;
var current_parent;

function select2DataCollectName(d) {
    if (d.children) d.children.forEach(select2DataCollectName);
    else if (d._children) d._children.forEach(select2DataCollectName);
    select2Data.push(d.name);
}

//===============================================
function searchTree(d) {
    if (d.children) d.children.forEach(searchTree);
    else if (d._children) d._children.forEach(searchTree);
    var searchFieldValue = eval(searchField);
    if (searchFieldValue && searchFieldValue.match(searchText)) {
        // Walk parent chain
        var ancestors = [];
        var parent = d;
        while (typeof parent !== "undefined") {
            ancestors.push(parent);
            //console.log(parent);
            parent.class = "found";
            parent = parent.parent;
        }
        //console.log(ancestors);
    }
}

//===============================================
function clearAll(d) {
    d.class = "";
    if (d.children) d.children.forEach(clearAll);
    else if (d._children) d._children.forEach(clearAll);
}

//===============================================
function collapse(d) {
    if (d.children) {
        d._children = d.children;
        d._children.forEach(collapse);
        d.children = null;
    }
}

//===============================================
function collapseAllNotFound(d) {
    if (d.children) {
        if (d.class !== "found") {
            d._children = d.children;
            d._children.forEach(collapseAllNotFound);
            d.children = null;
        } else d.children.forEach(collapseAllNotFound);
    }
}

//===============================================
function expandAll(d) {
    if (d._children) {
        d.children = d._children;
        d.children.forEach(expandAll);
        d._children = null;
    } else if (d.children) d.children.forEach(expandAll);
}

//===============================================
// Toggle children on click.
function toggle(d) {
    if (d.children) {
        d._children = d.children;
        d.children = null;
    } else {
        d.children = d._children;
        d._children = null;

        if (!d.children) {
            x = document.getElementById("claims_box");
            y = document.getElementById("claims_text");
            x.style.display = "block";
            current_name = d.name;
            console.log(d.ma)
            current_parent = d.parent.name;
            if (d.claimReason == undefined) {
                d.claimReason = 'Không có dữ liệu tương ứng'
            }
            y.innerHTML =
                "<h2>" + d.name + "</h2>" +
                "<h4>Mục Tiêu Đào Tạo</h4>" +
                "<p>" + d.claimReason + "</p>" +
                "<a  href='../cms/curriculum/overview/" + d.ma + "'> * Xem Chi Tiết Thông Tin Chung </a>" + "</br>" +
                "<a  href='../cms/curriculum/list/" + d.ma + "' > * Xem Nội Dung Chương Trình Đào Tạo </a>"
        }
    }
    clearAll(root);
    update(d);
    $("#searchName").select2("val", "");
}

//===============================================
$("#searchName").on("select2-selecting", function (e) {
    clearAll(root);
    expandAll(root);
    update(root);

    searchField = "d.name";
    searchText = e.object.text;
    searchTree(root);
    root.children.forEach(collapseAllNotFound);
    update(root);
});

//===============================================
var margin = {
    top: 20,
    right: 120,
    bottom: 20,
    left: 100
},
    width = 1500 - margin.left - margin.right,
    height = 650 - margin.top - margin.bottom,
    width2 = $(document).width(),
    height2 = $(document).height();

var i = 0,
    duration = 750,
    root;

var tree = d3.layout.tree().size([height, width]);

var diagonal = d3.svg.diagonal().projection(function (d) {
    return [d.y, d.x];
});

var svg = d3
    .select("#content")
    .append("svg")
    .attr("width", width2)
    .attr("height", height2)
    .append("g")
    .attr("transform", "translate(" + margin.left + "," + margin.top + ")");
d3.json("http://localhost:58806/CMS/Home/mmap", function (error, values) {
//d3.json(
//    "https://raw.githubusercontent.com/SharathchandraBangaloreMunibairegowda/Datahub/master/claimCategories.json",
    //function (error, values) {
        root = values;
        root.x0 = height / 2;
        root.y0 = 0;

        select2Data = [];
        select2DataCollectName(root);
        select2DataObject = [];
       
        select2Data
            .sort(function (a, b) {
                if (a > b) return 1; // sort
                if (a < b) return -1;
                return 0;
            })
            .filter(function (item, i, ar) {
                return ar.indexOf(item) === i;
            }) // remove duplicate items
            .filter(function (item, i, ar) {
                select2DataObject.push({
                    id: i,
                    text: item
                });
            });
        $("#searchName").select2({
            data: select2DataObject,
            containerCssClass: "search",
            placeholder: "Chọn nhánh cần xem"
        });

        function collapse(d) {
            if (d.children) {
                d._children = d.children;
                d._children.forEach(collapse);
                d.children = null;
            }
        }

        root.children.forEach(collapse);
        update(root);
    }
);

d3.select(self.frameElement).style("height", "800px");

function update(source) {
    // Compute the new tree layout.
    var nodes = tree.nodes(root).reverse(),
        links = tree.links(nodes);

    // Normalize for fixed-depth.
    nodes.forEach(function (d) {
        d.y = d.depth * 180;
    });

    // Update the nodes…
    var node = svg.selectAll("g.node").data(nodes, function (d) {
        return d.id || (d.id = ++i);
    });

    // Enter any new nodes at the parent's previous position.
    var nodeEnter = node
        .enter()
        .append("g")
        .attr("class", "node")
        .attr("transform", function (d) {
            return "translate(" + source.y0 + "," + source.x0 + ")";
        })
        .on("click", toggle);

    nodeEnter
        .append("circle")
        .attr("r", 8)
        .style("stroke-opacity", 1)
        .style("stroke", function (d) {
            return d.color;
        })
        .style("fill", function (d) {
            return d._children ? "transparent" : "#fff";
        });

    nodeEnter
        .append("text")
        .attr("x", function (d) {
            return d.children ? -12 : 12;
        })
        .attr("dy", ".35em")
        .attr("text-anchor", function (d) {
            return d.children ? "end" : "start";
        })
        .text(function (d) {
            return d.name;
        })
        .style("fill-opacity", 1e-6);

    // Transition nodes to their new position.
    var nodeUpdate = node
        .transition()
        .duration(duration)
        .attr("transform", function (d) {
            return "translate(" + d.y + "," + d.x + ")";
        });

    nodeUpdate
        .select("circle")
        .attr("r", 8)
        .style("stroke-opacity", 1)
        .style("stroke", function (d) { })
        .style("fill", function (d) {
            if (d.class === "found") return "#ff4136";
            else if (d._children && !d.children)
                //red
                return "#008000";
            else return "#fff";
        })
        .style("stroke", function (d) {
            if (d.class === "found") {
                return "#ff4136"; //red
            } else {
                return d.color;
            }
        })
        .style("stroke-width", function (d) {
            if (d.class === "found") {
                return "5px";
            } else {
                return "2.5px";
            }
        });

    nodeUpdate
        .select("text")
        .attr("x", function (d) {
            return d.children ? -12 : 12;
        })
        .attr("dy", ".35em")
        .attr("text-anchor", function (d) {
            return d.children ? "end" : "start";
        })
        .style("fill-opacity", 1);

    nodeUpdate.select("text").style("text-decoration", function (d) {
        return !d._children && !d.children ? "underline" : "none";
    });

    // Transition exiting nodes to the parent's new position.
    var nodeExit = node
        .exit()
        .transition()
        .duration(duration)
        .attr("transform", function (d) {
            return "translate(" + source.y + "," + source.x + ")";
        })
        .remove();

    nodeExit.select("circle").attr("r", 1e-6);

    nodeExit.select("text").style("fill-opacity", 1e-6);

    // Update the links…
    var link = svg.selectAll("path.link").data(links, function (d) {
        return d.target.id;
    });

    // Enter any new links at the parent's previous position.
    link
        .enter()
        .insert("path", "g")
        .attr("class", "link")
        .style("stroke", function (d) {
            return d.color;
        })
        .style("stroke", function (d) {
            return d.target.level;
        }) // Handle the appending of level color from data to link.
        .attr("d", function (d) {
            var o = {
                x: source.x0,
                y: source.y0
            };
            return diagonal({
                source: o,
                target: o
            });
        });

    // Transition links to their new position.
    link
        .transition()
        .duration(duration)
        .attr("d", diagonal)
        .style("stroke", function (d) {
            return d.color;
        })
        .style("stroke", function (d) {
            if (d.target.class === "found") {
                return "#ff4136";
            }
        })
        .style("stroke-opacity", function (d) {
            if (d.target.class === "found") {
                return 0.27;
            }
        })
        .style("stroke-width", function (d) {
            if (d.target.class === "found") {
                return ".25rem";
            }
        })
        .style("fill-opacity", function (d) {
            if (d.target.class === "found") {
                return 1;
            }
        });

    // Transition exiting nodes to the parent's new position.
    link
        .exit()
        .transition()
        .duration(duration)
        .attr("d", function (d) {
            var o = {
                x: source.x,
                y: source.y
            };
            return diagonal({
                source: o,
                target: o
            });
        })
        .remove();

    // Stash the old positions for transition.
    nodes.forEach(function (d) {
        d.x0 = d.x;
        d.y0 = d.y;
    });
}

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];
var span2 = document.getElementsByClassName("close2")[0];

span.onclick = function () {
    claims_box.style.display = "none";
};

