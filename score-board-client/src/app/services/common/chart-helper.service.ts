import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ChartHelperService {

  constructor() { }

  private static units = {
    years: {format: ['yyyy']},
    months: {format: ['MM']},
    days: {format: ['dd']},
    hours: {format: ['HH']},
    minutes: {format: ['mm']},
    seconds: {format: ['ss']},
    milliseconds: {format: ['SS']},
  };

  public static initChartOptions(vTitle: string, hTitle: string, fontColor: string, lineColor: string): any {
    return {
      curveType: 'function',
      legend: {
        position: 'right',
        textStyle: {
          color: fontColor,
          fontName: 'Roboto-Light',
          fontSize: 14
        },
        titleTextStyle: {
          color: fontColor,
          fontName: 'Roboto-Light',
          fontSize: 16
        }
      },
      is3D: true,
      animation: {
        duration: 1000,
        easing: 'out'
      },
      backgroundColor: {
        fill: 'transparent'
      },
      hAxis: {
        title: hTitle,
        textStyle: {
          color: fontColor,
          fontName: 'Roboto-Light',
          fontSize: 14
        },
        titleTextStyle: {
          color: fontColor,
          fontName: 'Roboto-Light',
          fontSize: 16,
        },
        gridlines: {
          color: lineColor,
          units: this.units
        },
        minorGridlines: {
          color: lineColor,
          units: this.units
        }
      },
      vAxis: {
        title: vTitle,
        textStyle: {
          color: fontColor,
          fontName: 'Roboto-Light',
          fontSize: 14
        },
        titleTextStyle: {
          color: fontColor,
          fontName: 'Roboto-Light',
          fontSize: 16
        },
        gridlines: {
          color: lineColor,
          units: this.units
        },
        minorGridlines: {
          color: lineColor,
          units: this.units
        }
      }
    };
  }
}
