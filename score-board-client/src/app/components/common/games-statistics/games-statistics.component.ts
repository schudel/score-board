import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {ThemeService} from '../../../services/common/theme.service';
import {ChartHelperService} from '../../../services/common/chart-helper.service';
import {Match} from '../../../models/match';
import {Game} from '../../../models/game';

@Component({
  selector: 'app-games-statistics',
  templateUrl: './games-statistics.component.html',
  styleUrls: ['./games-statistics.component.scss']
})
export class GamesStatisticsComponent implements OnInit, OnDestroy {
  subscriptions = [];
  gameId: string;
  gamesInternal: Game[] = [];
  matchesInternal: Match[] = [];

  wonLostDrawnTeamsData = [];
  wonLostDrawnTeamsColumns = [];
  wonLostDrawnTeamsOptions = {};
  wonLostDrawnTeamsType = 'BarChart';
  wonLostDrawnPlayersData = [];
  wonLostDrawnPlayersColumns = [];
  wonLostDrawnPlayersOptions = {};
  wonLostDrawnPlayersType = 'BarChart';

  isDarkTheme = false;
  fontColor = '#000000';
  lineColor = 'lightgray';
  allGamesString = 'All Games';

  constructor(private themeService: ThemeService) {
    this.allGamesString = $localize`:@@gamesStatisticsAllGames:All Games`;
  }

  @Input()
  set games(val: Game[]) {
    this.gamesInternal = val;
    if (!this.gamesInternal) {
      return;
    }
    const allGames = new Game();
    allGames.name = this.allGamesString;
    allGames.id = this.allGamesString;
    let contains = false;
    for (const g of this.gamesInternal) {
      if (g.id === allGames.id) {
        contains = true;
        break;
      }
    }
    if (!contains) {
      this.gamesInternal.unshift(allGames);
    }
    this.gameId = this.gamesInternal[0].id;
    this.createDataSource();
  }

  @Input()
  set matches(val: Match[]) {
    this.matchesInternal = val;
    if (!this.matchesInternal) {
      return;
    }
    this.createDataSource();
  }

  ngOnInit() {
    this.subscriptions.push(this.themeService.isDarkTheme.subscribe(x => {
      this.isDarkTheme  = x;
      if (x) {
        this.fontColor = '#ffffff';
        this.lineColor = '#575757';
      } else {
        this.fontColor = '#000000';
        this.lineColor = 'lightgray';
      }
      this.wonLostDrawnTeamsOptions = ChartHelperService.initChartOptions($localize`:@@statisticsTeams:Teams`,
        $localize`:@@wonLostDrawnMatches:Won / Lost / Drawn Matches`, this.fontColor, this.lineColor);
      this.wonLostDrawnPlayersOptions = ChartHelperService.initChartOptions($localize`:@@statisticsPlayers:Players`, $localize`:@@wonLostDrawnMatches:Won / Lost / Drawn Matches`,
        this.fontColor, this.lineColor);
    }));
  }

  private createDataSource() {
    this.wonLostDrawnTeamsColumns = [];
    this.wonLostDrawnTeamsData = [];
    this.wonLostDrawnPlayersColumns = [];
    this.wonLostDrawnPlayersData = [];
    if (!this.gamesInternal || !this.matchesInternal) {
      return;
    }
    const wonKey = $localize`:@@statisticsWon:Won`;
    const lostKey = $localize`:@@statisticsLost:Lost`;
    const drawnKey = $localize`:@@statisticsDrawn:`;
    this.wonLostDrawnTeamsColumns = [$localize`:@@statisticsTeams:statisticsTeams`, wonKey, lostKey, drawnKey];
    this.wonLostDrawnPlayersColumns = [$localize`:@@statisticsPlayers:Players`, wonKey, lostKey, drawnKey];

    const teamsWonLostDrawnMap: Map<string, Map<string, number>> = new Map<string, Map<string, number>>();
    const playersWonLostDrawnMap: Map<string, Map<string, number>> = new Map<string, Map<string, number>>();
    for (const m of this.matchesInternal) {
      if (m.game.id === this.gameId || this.gameId === this.allGamesString) {
        if (m.team1.id === m.winnerTeamId) {
          // is winner team
          this.setWonLostDrawnValues(teamsWonLostDrawnMap, m.team1.name, wonKey);
          this.setWonLostDrawnValues(playersWonLostDrawnMap, m.team1.player1.playerName, wonKey);
          if (m.team1.player2) {
            this.setWonLostDrawnValues(playersWonLostDrawnMap, m.team1.player2.playerName, wonKey);
          }
        }
        if (m.team1.id === m.loserTeamId) {
          // is loser team
          this.setWonLostDrawnValues(teamsWonLostDrawnMap, m.team1.name, lostKey);
          this.setWonLostDrawnValues(playersWonLostDrawnMap, m.team1.player1.playerName, lostKey);
          if (m.team1.player2) {
            this.setWonLostDrawnValues(playersWonLostDrawnMap, m.team1.player2.playerName, lostKey);
          }
        }
        if (m.team2.id === m.winnerTeamId) {
          // is winner team
          this.setWonLostDrawnValues(teamsWonLostDrawnMap, m.team2.name, wonKey);
          this.setWonLostDrawnValues(playersWonLostDrawnMap, m.team2.player1.playerName, wonKey);
          if (m.team2.player2) {
            this.setWonLostDrawnValues(playersWonLostDrawnMap, m.team2.player2.playerName, wonKey);
          }
        }
        if (m.team2.id === m.loserTeamId) {
          // is loser team
          this.setWonLostDrawnValues(teamsWonLostDrawnMap, m.team2.name, lostKey);
          this.setWonLostDrawnValues(playersWonLostDrawnMap, m.team2.player1.playerName, lostKey);
          if (m.team2.player2) {
            this.setWonLostDrawnValues(playersWonLostDrawnMap, m.team2.player2.playerName, lostKey);
          }
        }
        if (m.drawn) {
          // drawn
          this.setWonLostDrawnValues(teamsWonLostDrawnMap, m.team1.name, drawnKey);
          this.setWonLostDrawnValues(teamsWonLostDrawnMap, m.team2.name, drawnKey);
          this.setWonLostDrawnValues(playersWonLostDrawnMap, m.team1.player1.playerName, drawnKey);
          if (m.team1.player2) {
            this.setWonLostDrawnValues(playersWonLostDrawnMap, m.team1.player2.playerName, drawnKey);
          }
          this.setWonLostDrawnValues(playersWonLostDrawnMap, m.team2.player1.playerName, drawnKey);
          if (m.team2.player2) {
            this.setWonLostDrawnValues(playersWonLostDrawnMap, m.team2.player2.playerName, drawnKey);
          }
        }
      }
    }

    // won / lost / drawn teams
    for (const key of teamsWonLostDrawnMap.keys()) {
      const dataRow = [];
      dataRow.push(key);
      if (teamsWonLostDrawnMap.get(key).get(wonKey)) {
        dataRow.push(teamsWonLostDrawnMap.get(key).get(wonKey));
      } else {
        dataRow.push(0);
      }
      if (teamsWonLostDrawnMap.get(key).get(lostKey)) {
        dataRow.push(teamsWonLostDrawnMap.get(key).get(lostKey));
      } else {
        dataRow.push(0);
      }
      if (teamsWonLostDrawnMap.get(key).get(drawnKey)) {
        dataRow.push(teamsWonLostDrawnMap.get(key).get(drawnKey));
      } else {
        dataRow.push(0);
      }
      this.wonLostDrawnTeamsData.push(dataRow);
    }

    // won / lost / drawn players
    for (const key of playersWonLostDrawnMap.keys()) {
      const dataRow = [];
      dataRow.push(key);
      if (playersWonLostDrawnMap.get(key).get(wonKey)) {
        dataRow.push(playersWonLostDrawnMap.get(key).get(wonKey));
      } else {
        dataRow.push(0);
      }
      if (playersWonLostDrawnMap.get(key).get(lostKey)) {
        dataRow.push(playersWonLostDrawnMap.get(key).get(lostKey));
      } else {
        dataRow.push(0);
      }
      if (playersWonLostDrawnMap.get(key).get(drawnKey)) {
        dataRow.push(playersWonLostDrawnMap.get(key).get(drawnKey));
      } else {
        dataRow.push(0);
      }
      this.wonLostDrawnPlayersData.push(dataRow);
    }
  }

  setWonLostDrawnValues(map: Map<string, Map<string, number>> = new Map<string, Map<string, number>>(),
                        value: string, key: string): void {
    if (map.has(value)) {
      const wonMap = map.get(value);
      if (wonMap.has(key)) {
        wonMap.set(key, wonMap.get(key) + 1);
      } else {
        wonMap.set(key, 1);
      }
      map.set(value, wonMap);
    } else {
      const wonMap = new Map<string, number>();
      wonMap.set(key, 1);
      map.set(value, wonMap);
    }
  }

  changeGame(gId: string) {
    this.gameId = gId;
    if (!gId) {
      return;
    }
    this.createDataSource();
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
