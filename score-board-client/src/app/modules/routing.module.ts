import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RouterModule, Routes} from '@angular/router';
import {AuthenticationGuard} from '../guards/authentication.guard';
import {MatchesComponent} from '../components/routes/matches/matches.component';
import {PlayersComponent} from '../components/routes/players/players.component';
import {GamesComponent} from '../components/routes/games/games.component';
import {DashboardComponent} from '../components/routes/dashboard/dashboard.component';
import {LoginComponent} from '../components/routes/login/login.component';
import {RankingComponent} from '../components/routes/ranking/ranking.component';
import {Role} from '../models/role';
import {AdminComponent} from '../components/routes/admin/admin.component';
import {RegistrationComponent} from '../components/routes/registration/registration.component';
import {PageNotFoundComponent} from '../components/routes/page-not-found/page-not-found.component';
import {ActivationComponent} from '../components/routes/activation/activation.component';
import {ProfileComponent} from '../components/routes/profile/profile.component';
import {PrivacyPolicyComponent} from '../components/routes/privacy-policy/privacy-policy.component';
import {DisclaimersComponent} from '../components/routes/disclaimers/disclaimers.component';
import {GameComponent} from '../components/routes/game/game.component';
import {PlayerComponent} from '../components/routes/player/player.component';
import {MatchComponent} from '../components/routes/match/match.component';
import {LiveComponent} from '../components/routes/live/live.component';
import {StatisticsComponent} from '../components/routes/statistics/statistics.component';
import {RequestPasswordResetComponent} from '../components/routes/request-password-reset/request-password-reset.component';
import {ResetPasswordComponent} from '../components/routes/reset-password/reset-password.component';

const routes: Routes = [
  // Public routes: Login, Registration, etc.
  { path: 'login', component: LoginComponent },
  { path: 'registration', component: RegistrationComponent },
  { path: 'activate/:id', component: ActivationComponent },
  { path: 'privacy-policy', component: PrivacyPolicyComponent },
  { path: 'disclaimers', component: DisclaimersComponent },
  { path: 'request-password-reset', component: RequestPasswordResetComponent },
  { path: 'reset-password/:id', component: ResetPasswordComponent },
  // Authentication required
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthenticationGuard] },
  { path: 'games', component: GamesComponent, canActivate: [AuthenticationGuard] },
  { path: 'game/view/:id', component: GameComponent, canActivate: [AuthenticationGuard] },
  { path: 'game/edit/:id', component: GameComponent, canActivate: [AuthenticationGuard], data: { roles: [Role.Admin] } },
  { path: 'game/create', component: GameComponent, canActivate: [AuthenticationGuard], data: { roles: [Role.Admin] } },
  { path: 'players', component: PlayersComponent, canActivate: [AuthenticationGuard] },
  { path: 'player/view/:id', component: PlayerComponent, canActivate: [AuthenticationGuard] },
  { path: 'player/edit/:id', component: PlayerComponent, canActivate: [AuthenticationGuard], data: { roles: [Role.Admin] } },
  { path: 'player/create', component: PlayerComponent, canActivate: [AuthenticationGuard], data: { roles: [Role.Admin] } },
  { path: 'matches', component: MatchesComponent, canActivate: [AuthenticationGuard] },
  { path: 'match/view/:id', component: MatchComponent, canActivate: [AuthenticationGuard] },
  { path: 'match/edit/:id', component: MatchComponent, canActivate: [AuthenticationGuard] },
  { path: 'match/create', component: MatchComponent, canActivate: [AuthenticationGuard] },
  { path: 'ranking', component: RankingComponent, canActivate: [AuthenticationGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthenticationGuard] },
  { path: 'live', component: LiveComponent, canActivate: [AuthenticationGuard] },
  { path: 'live/:id', component: LiveComponent, canActivate: [AuthenticationGuard] },
  { path: 'statistics', component: StatisticsComponent, canActivate: [AuthenticationGuard] },

  { path: 'admin', component: AdminComponent, canActivate: [AuthenticationGuard], data: { roles: [Role.Admin] } },
  { path: '404', component: PageNotFoundComponent, canActivate: [AuthenticationGuard] },
  // otherwise redirect to 404 - page not found
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class RoutingModule { }
