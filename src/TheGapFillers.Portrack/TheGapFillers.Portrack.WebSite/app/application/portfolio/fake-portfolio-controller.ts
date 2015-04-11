module App {
    'use strict';

    export interface IFakePortfolioControler{
        createFakeCharts(): void;
    }

    class FakePortfolioControler implements IFakePortfolioControler {
        static Id = "FakePortfolioControler";

        static $inject = [];
        constructor() {
            this.createFakeCharts();
        }

        createFakeCharts(): void {
            var fakeData = [
                {
                    label: "Performance",
                    data: [[2006, 0], [2007, 1], [2008, 3], [2009, 8], [2010, 6], [2011, 7], [2012, 10], [2013, 9]]
                }, {
                    label: "Benchmark",
                    data: [[2006, 0], [2007, 1], [2008, 2], [2009, 5], [2010, 5], [2011, 6], [2012, 7], [2013, 7]]
                }
            ];

            var border_color = "#efefef";
            var primary_color = "#428bca";
            var go_green = "#93caa3";
            var ruby_red = "#fa9c9b";
            var options: jquery.flot.plotOptions = {
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
                    tickColor: border_color,
                    borderWidth: 0
                },
                legend: {
                    noColumns: 2
                },
                colors: [primary_color, ruby_red],
                selection: {
                    mode: "x"
                }
            };

            for (var i = 0; i < (".fake-chart").length; i++) {
                var placeholder = <any>$(".fake-chart")[i]; // graph 
                var plot = $.plot(placeholder, fakeData, options);
            }
        }
    }

    app.controller(FakePortfolioControler.Id, FakePortfolioControler);
}

