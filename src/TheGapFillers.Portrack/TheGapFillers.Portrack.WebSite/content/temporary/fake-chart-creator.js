//Global variable that creates a chart for the .fake-chart
var createFakeCharts = createFakeCharts();
function createFakeCharts() {
    for (var i = 0; i < $(".fake-chart").length; i++) {
        var $border_color = "#efefef";
        var $grid_color = "#ddd";
        var $default_black = "#666";
        var $default_grey = "#ccc";
        var $primary_color = "#428bca";
        var $go_green = "#93caa3";
        var $jet_blue = "#70aacf";
        var $lemon_yellow = "#ffe38a";
        var $nagpur_orange = "#fc965f";
        var $ruby_red = "#fa9c9b";

        var data = [{
            label: "Performance",
            data: [[2006, 0], [2007, 1], [2008, 3], [2009, 8], [2010, 6], [2011, 7], [2012, 10], [2013, 9]]
        }, {
            label: "Benchmark",
            data: [[2006, 0], [2007, 1], [2008, 2], [2009, 5], [2010, 5], [2011, 6], [2012, 7], [2013, 7]]
        }];

        var options = {
            series: {
                lines: {
                    show: true,
                    lineWidth: 2,
                    fill: true, fillColor: { colors: [{ opacity: 0.5 }, { opacity: 0.2 }] }
                },
                points: {
                    show: true,
                    lineWidth: 2
                },
                shadowSize: 0
            },
            grid: {
                hoverable: true,
                clickable: true,
                tickColor: $border_color,
                borderWidth: 0
            },
            legend: {
                noColumns: 2
            },
            colors: [$primary_color, $ruby_red],
            xaxis: { ticks: 12, tickDecimals: 0 },
            yaxis: { ticks: 7, tickDecimals: 0 },
            selection: {
                mode: "x"
            }
        };
        var placeholder = $(".fake-chart")[i];
        var plot = $.plot(placeholder, data, options);

        $("#clearSelection").click(function () {
            plot.clearSelection();
        });

        $("#setSelection").click(function () {
            plot.setSelection({
                xaxis: {
                    from: 1994,
                    to: 1995
                }
            });
        });
    }
};
