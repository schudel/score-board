export enum DashboardWidgetEnum {
  ScoreBoardInfo = 'ScoreBoardInfo',
  NoOneWidget = 'NoOneWidget',
  LiveMatches = 'LiveMatches',
  RecentMatches = 'RecentMatches',
  MyStatistics = 'MyStatistics',
  StartMatch = 'StartMatch',
  MyMatches = 'MyMatches',
  EpicMatches = 'EpicMatches',
  Ranking = 'Ranking'
}

export class DashboardWidget {
  private constructor(id: string, widgetEnum: DashboardWidgetEnum, displayName: string) {
    this.id = id;
    this.widgetEnum = widgetEnum;
    this.displayName = displayName;
    DashboardWidget.Widgets.push(this);
  }
  private static Widgets: Array<DashboardWidget> = new Array<DashboardWidget>();

  public static ScoreBoardInfo = new DashboardWidget('ScoreBoardInfo', DashboardWidgetEnum.ScoreBoardInfo, 'Scoreboard Info');
  public static NoOneWidget = new DashboardWidget('NoOneWidget', DashboardWidgetEnum.NoOneWidget, 'Number One');
  public static LiveMatches = new DashboardWidget('LiveMatches', DashboardWidgetEnum.LiveMatches, 'Live Matches');
  public static RecentMatches = new DashboardWidget('RecentMatches', DashboardWidgetEnum.RecentMatches, 'Recent Matches');
  public static MyStatistics = new DashboardWidget('MyStatistics', DashboardWidgetEnum.MyStatistics, 'My Statistics');
  public static StartMatch = new DashboardWidget('StartMatch', DashboardWidgetEnum.StartMatch, 'Start Match');
  public static MyMatches = new DashboardWidget('MyMatches', DashboardWidgetEnum.MyMatches, 'My Matches');
  public static EpicMatches = new DashboardWidget('EpicMatches', DashboardWidgetEnum.EpicMatches, 'Epic Matches');
  public static Ranking = new DashboardWidget('Ranking', DashboardWidgetEnum.Ranking, 'Ranking');

  id: string;
  widgetEnum: DashboardWidgetEnum;
  displayName: string;

  static getWidgets(): Array<DashboardWidget> {
    return DashboardWidget.Widgets;
  }
}
