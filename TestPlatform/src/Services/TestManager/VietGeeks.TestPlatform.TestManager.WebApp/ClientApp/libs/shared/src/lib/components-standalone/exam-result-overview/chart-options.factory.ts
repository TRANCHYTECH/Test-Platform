import { ChartOptions } from './aggregated-grading.model';

export default function createChartOptions(chartValues: { finalPoints: number; totalPoints: number; percentage: number; }, unit: string, color: string): ChartOptions {
  return {
    series: [chartValues.percentage],
    chart: {
      height: 300,
      type: "radialBar",
    },
    plotOptions: {
      radialBar: {
        startAngle: -135,
        endAngle: 225,
        hollow: {
          margin: 0,
          size: "70%",
          background: "#fff",
          image: undefined,
          position: "front",
          dropShadow: {
            enabled: true,
            top: 3,
            left: 0,
            blur: 4,
            opacity: 0.24
          }
        },
        track: {
          background: "#fff",
          strokeWidth: "67%",
          margin: 0,
          dropShadow: {
            enabled: true,
            top: -3,
            left: 0,
            blur: 4,
            opacity: 0.35
          }
        },

        dataLabels: {
          show: true,
          name: {
            offsetY: -10,
            show: true,
            color: "#888",
            fontSize: "17px"
          },
          value: {
            formatter: function (val) {
              if (unit == 'Percent') {
                return parseInt(val.toString(), 10).toString();
              }

              return `${chartValues.finalPoints} / ${chartValues.totalPoints}`;
            },
            color: "#111",
            fontSize: "36px",
            show: true
          }
        }
      }
    },
    fill: {
      type: "basic",
      gradient: {
        shade: "dark",
        type: "horizontal",
        shadeIntensity: 0.5,
        gradientToColors: ["#ff7067"],
        inverseColors: false,
        opacityFrom: 1,
        opacityTo: 1,
        stops: [0, 100]
      }
    },
    stroke: {
      lineCap: "round"
    },
    labels: [unit],
    colors: [color]
  };
}
