import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {Player} from '../../../models/player';
import {Match} from '../../../models/match';
import {ThemeService} from '../../../services/common/theme.service';
import {MatchHelperService} from '../../../services/common/match-helper.service';
import {ChartHelperService} from '../../../services/common/chart-helper.service';

@Component({
  selector: 'app-players-statistics',
  templateUrl: './players-statistics.component.html',
  styleUrls: ['./players-statistics.component.scss']
})
export class PlayersStatisticsComponent implements OnInit, OnDestroy {
  subscriptions = [];
  playerId: string;
  playerName: string;
  playersInternal: Player[] = [];
  matchesInternal: Match[] = [];
  playedGamesData = [];
  playedGamesColumns = [];
  playedGamesOptions = {};
  playedGamesType = 'BarChart';
  wonLostDrawnGamesData = [];
  wonLostDrawnGamesColumns = [];
  wonLostDrawnGamesOptions = {};
  wonLostDrawnGamesGamesType = 'BarChart';
  teamMembersData = [];
  teamMembersColumns = [];
  teamMembersOptions = {};
  teamMembersType = 'PieChart';

  isDarkTheme = false;
  fontColor = '#000000';
  lineColor = 'lightgray';
  allPlayersString = 'All Players';

  constructor(private themeService: ThemeService) {
    this.allPlayersString = $localize`:@@playersStatisticsAllPlayers:All Players`;
  }

  @Input()
  set players(val: Player[]) {
    this.playersInternal = val;
    if (!this.playersInternal) {
      return;
    }
    const allPlayers = new Player();
    allPlayers.playerName = this.allPlayersString;
    allPlayers.id = this.allPlayersString;
    let contains = false;
    for (const p of this.playersInternal) {
      if (p.id === allPlayers.id) {
        contains = true;
        break;
      }
    }
    if (!contains) {
      this.playersInternal.unshift(allPlayers);
    }
    this.playerId = this.playersInternal[0].id;
    this.playerName = this.playersInternal[0].playerName;
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
      this.playedGamesOptions = ChartHelperService.initChartOptions($localize`:@@statisticsGames:Games`, $localize`:@@statisticsPlayedMatches:Played Matches`,
        this.fontColor, this.lineColor);
      this.wonLostDrawnGamesOptions = ChartHelperService.initChartOptions($localize`:@@statisticsGames:Games`,
        $localize`:@@wonLostDrawnMatches:Won / Lost / Drawn Matches`, this.fontColor, this.lineColor);
      this.teamMembersOptions = ChartHelperService.initChartOptions($localize`:@@statisticsTeamMembers:Team Members`, $localize`:@@statisticsPlayedMatches:Played Matches`,
        this.fontColor, this.lineColor);
    }));
  }

  private createDataSource(): void {
    this.playedGamesData = [];
    this.playedGamesColumns = [];
    this.wonLostDrawnGamesData = [];
    this.wonLostDrawnGamesColumns = [];
    this.teamMembersData = [];
    this.teamMembersColumns = [];
    if (!this.playersInternal || !this.matchesInternal) {
      return;
    }
    const wonKey = $localize`:@@statisticsWon:Won`;
    const lostKey = $localize`:@@statisticsLost:Lost`;
    const drawnKey = $localize`:@@statisticsDrawn:Drawn`;
    this.playedGamesColumns = [$localize`:@@statisticsGames:Games`];
    this.wonLostDrawnGamesColumns = [$localize`:@@statisticsGames:Games`, wonKey, lostKey, drawnKey];
    this.teamMembersColumns = [$localize`:@@statisticsTeamMembers:Team Members`, $localize`:@@statisticsPlayedMatches:Played Matches`];
    const gameMap: Map<string, number> = new Map<string, number>();
    const gamePlayerMap: Map<string, Map<string, number>> = new Map<string, Map<string, number>>();
    const gameWonLostDrawnMap: Map<string, Map<string, number>> = new Map<string, Map<string, number>>();
    const teamMemberMap: Map<string, number> = new Map<string, number>();
    for (const m of this.matchesInternal) {
      if (this.allPlayersString === this.playerId) {
        // played matches data source
        if (gamePlayerMap.has(m.game.name)) {
          gamePlayerMap.set(m.game.name, this.setPlayedMatchesMap(m, gamePlayerMap.get(m.game.name)));
        } else {
          gamePlayerMap.set(m.game.name, this.setPlayedMatchesMap(m, new Map<string, number>()));
        }
      } else if (m.team1.player1.id === this.playerId ||
        (m.team1.player2 && m.team1.player2.id === this.playerId) ||
        m.team2.player1.id === this.playerId ||
        (m.team2.player2 && m.team2.player2.id === this.playerId)) {
        // played matches data source
        if (gameMap.has(m.game.name)) {
          gameMap.set(m.game.name, gameMap.get(m.game.name) + 1);
        } else {
          gameMap.set(m.game.name, 1);
        }
        // won / lost / drawn matches
        const teamId = MatchHelperService.getTeamId(m, this.playersInternal.find(p => p.id === this.playerId));
        if (gameWonLostDrawnMap.has(m.game.name)) {
          if (m.winnerTeamId === teamId) {
            if (gameWonLostDrawnMap.get(m.game.name).has(wonKey)) {
              gameWonLostDrawnMap.get(m.game.name).set(wonKey, gameWonLostDrawnMap.get(m.game.name).get(wonKey) + 1);
            } else {
              gameWonLostDrawnMap.get(m.game.name).set(wonKey, 1);
            }
          } else if (m.loserTeamId === teamId) {
            if (gameWonLostDrawnMap.get(m.game.name).has(lostKey)) {
              gameWonLostDrawnMap.get(m.game.name).set(lostKey, gameWonLostDrawnMap.get(m.game.name).get(lostKey) + 1);
            } else {
              gameWonLostDrawnMap.get(m.game.name).set(lostKey, 1);
            }
          } else if (m.drawn) {
            if (gameWonLostDrawnMap.get(m.game.name).has(drawnKey)) {
              gameWonLostDrawnMap.get(m.game.name).set(drawnKey, gameWonLostDrawnMap.get(m.game.name).get(drawnKey) + 1);
            } else {
              gameWonLostDrawnMap.get(m.game.name).set(drawnKey, 1);
            }
          }
        } else {
          const wonLostDrawnMap: Map<string, number> = new Map<string, number>();
          if (m.winnerTeamId === teamId) {
            wonLostDrawnMap.set(wonKey, 1);
          } else if (m.loserTeamId === teamId) {
            wonLostDrawnMap.set(lostKey, 1);
          } else if (m.drawn) {
            wonLostDrawnMap.set(drawnKey, 1);
          }
          gameWonLostDrawnMap.set(m.game.name, wonLostDrawnMap);
        }
        // Team Members
        let teamMember = MatchHelperService.getTeamMember(m, this.playersInternal.find(p => p.id === this.playerId));
        if (teamMember === '') {
          teamMember = $localize`:@@statisticsNoTeamMember:No Team Member`;
        }
        if (teamMemberMap.has(teamMember)) {
          teamMemberMap.set(teamMember, teamMemberMap.get(teamMember) + 1);
        } else {
          teamMemberMap.set(teamMember, 1);
        }
      }
    }
    if (this.allPlayersString === this.playerId) {
      // played matches data source
      for (const key of gamePlayerMap.keys()) {
        const dataRow = [];
        dataRow.push(key);
        let first = true;
        for (const column of this.playedGamesColumns) {
          if (first) {
            first = false;
            continue;
          }
          dataRow.push(gamePlayerMap.get(key).get(column));
        }
        this.playedGamesData.push(dataRow);
      }
    } else {
      // played matches data source
      this.playedGamesColumns.push(this.playerName);
      for (const key of gameMap.keys()) {
        this.playedGamesData.push([key, gameMap.get(key)]);
      }
      // won / lost / drawn matches
      for (const key of gameWonLostDrawnMap.keys()) {
        const dataRow = [];
        dataRow.push(key);
        if (gameWonLostDrawnMap.get(key).get(wonKey)) {
          dataRow.push(gameWonLostDrawnMap.get(key).get(wonKey));
        } else {
          dataRow.push(0);
        }
        if (gameWonLostDrawnMap.get(key).get(lostKey)) {
          dataRow.push(gameWonLostDrawnMap.get(key).get(lostKey));
        } else {
          dataRow.push(0);
        }
        if (gameWonLostDrawnMap.get(key).get(drawnKey)) {
          dataRow.push(gameWonLostDrawnMap.get(key).get(drawnKey));
        } else {
          dataRow.push(0);
        }
        this.wonLostDrawnGamesData.push(dataRow);
      }
      // Team Members
      for (const key of teamMemberMap.keys()) {
        const dataRow = [];
        dataRow.push(key);
        if (teamMemberMap.get(key)) {
          dataRow.push(teamMemberMap.get(key));
        } else {
          dataRow.push(0);
        }
        this.teamMembersData.push(dataRow);
      }
    }
  }

  private setPlayedMatchesMap(m: Match, gMap: Map<string, number>): Map<string, number> {
    if (gMap.has(m.team1.player1.playerName)) {
      gMap.set(m.team1.player1.playerName, gMap.get(m.team1.player1.playerName) + 1);
    } else {
      gMap.set(m.team1.player1.playerName, 1);
      if (this.playedGamesColumns.indexOf(m.team1.player1.playerName) === -1) {
        this.playedGamesColumns.push(m.team1.player1.playerName);
      }
    }
    if (m.team1.player2) {
      if (gMap.has(m.team1.player2.playerName)) {
        gMap.set(m.team1.player2.playerName, gMap.get(m.team1.player2.playerName) + 1);
      } else {
        gMap.set(m.team1.player2.playerName, 1);
        if (this.playedGamesColumns.indexOf(m.team1.player2.playerName) === -1) {
          this.playedGamesColumns.push(m.team1.player2.playerName);
        }
      }
    }
    if (gMap.has(m.team2.player1.playerName)) {
      gMap.set(m.team2.player1.playerName, gMap.get(m.team2.player1.playerName) + 1);
    } else {
      gMap.set(m.team2.player1.playerName, 1);
      if (this.playedGamesColumns.indexOf(m.team2.player1.playerName) === -1) {
        this.playedGamesColumns.push(m.team2.player1.playerName);
      }
    }
    if (m.team2.player2) {
      if (gMap.has(m.team2.player2.playerName)) {
        gMap.set(m.team2.player2.playerName, gMap.get(m.team2.player2.playerName) + 1);
      } else {
        gMap.set(m.team2.player2.playerName, 1);
        if (this.playedGamesColumns.indexOf(m.team2.player2.playerName) === -1) {
          this.playedGamesColumns.push(m.team2.player2.playerName);
        }
      }
    }
    return gMap;
  }

  changePlayer(pId: string) {
    this.playerId = pId;
    this.playerName = this.playersInternal.find(p => p.id === pId).playerName;
    if (!pId) {
      return;
    }
    this.createDataSource();
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
