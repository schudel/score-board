import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { DashboardComponent } from './components/routes/dashboard/dashboard.component';
import { LoginComponent } from './components/routes/login/login.component';
import { PlayersComponent } from './components/routes/players/players.component';
import { MatchesComponent } from './components/routes/matches/matches.component';
import { GamesComponent } from './components/routes/games/games.component';
import { RankingComponent } from './components/routes/ranking/ranking.component';
import {RoutingModule} from './modules/routing.module';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import { LoadingScreenComponent } from './components/common/loading-screen/loading-screen.component';
import {LoadingInterceptor} from './interceptors/loading-interceptor';
import {ErrorInterceptor} from './interceptors/error-interceptor';
import {JwtInterceptor} from './interceptors/jwt-interceptor';
import { AdminComponent } from './components/routes/admin/admin.component';
import { RegistrationComponent } from './components/routes/registration/registration.component';
import { PageNotFoundComponent } from './components/routes/page-not-found/page-not-found.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {MaterialModule} from './modules/material.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ActivationComponent } from './components/routes/activation/activation.component';
import {ToastrModule} from 'ngx-toastr';
import {GridsterModule} from 'angular-gridster2';
import { ProfileComponent } from './components/routes/profile/profile.component';
import {NgxFileDropModule} from 'ngx-file-drop';
import { ChangePasswordComponent } from './components/common/change-password/change-password.component';
import { PersonalInformationComponent } from './components/common/personal-information/personal-information.component';
import { ProfileSettingsComponent } from './components/common/profile-settings/profile-settings.component';
import {NgxPageScrollCoreModule} from 'ngx-page-scroll-core';
import {NgxPageScrollModule} from 'ngx-page-scroll';
import { NavigationComponent } from './components/common/navigation/navigation.component';
import { QuickPanelComponent } from './components/common/quick-panel/quick-panel.component';
import { NotificationsComponent } from './components/common/notifications/notifications.component';
import { FooterComponent } from './components/common/footer/footer.component';
import { PrivacyPolicyComponent } from './components/routes/privacy-policy/privacy-policy.component';
import { DisclaimersComponent } from './components/routes/disclaimers/disclaimers.component';
import { OrderByPipe } from './pipes/order-by.pipe';
import { ChatComponent } from './components/common/chat/chat.component';
import { PlayerInfoComponent } from './components/common/player-info/player-info.component';
import { ScoreBoardInfoComponent } from './components/common/score-board-info/score-board-info.component';
import { NumberOneWidgetComponent } from './components/common/number-one-widget/number-one-widget.component';
import { MatchListComponent } from './components/common/match-list/match-list.component';
import { LiveMatchesComponent } from './components/common/live-matches/live-matches.component';
import { DashboardTileComponent } from './components/common/dashboard-tile/dashboard-tile.component';
import {QuillModule} from 'ngx-quill';
import { RankingWidgetComponent } from './components/common/ranking-widget/ranking-widget.component';
import { WidgetSelectorComponent } from './components/common/widget-selector/widget-selector.component';
import { GameComponent } from './components/routes/game/game.component';
import { MatchComponent } from './components/routes/match/match.component';
import { PlayerComponent } from './components/routes/player/player.component';
import { ModalDialogComponent } from './components/common/modal-dialog/modal-dialog.component';
import {Ng2ImgMaxModule} from 'ng2-img-max';
import { LiveComponent } from './components/routes/live/live.component';
import { GiphyComponent } from './components/common/giphy/giphy.component';
import { AutofocusDirective } from './directives/autofocus.directive';
// import {RECAPTCHA_V3_SITE_KEY, RecaptchaV3Module} from 'ng-recaptcha';
// import {environment} from '../environments/environment';
import {PickerModule} from '@ctrl/ngx-emoji-mart';
import { MatchDetailComponent } from './components/common/match-detail/match-detail.component';
import {RatingTimelineComponent} from './components/common/rating-timeline/rating-timeline.component';
import {GoogleChartsModule} from 'angular-google-charts';
import { StatisticsComponent } from './components/routes/statistics/statistics.component';
import { PlayersStatisticsComponent } from './components/common/players-statistics/players-statistics.component';
import { GamesStatisticsComponent } from './components/common/games-statistics/games-statistics.component';
import { MatchesStatisticsComponent } from './components/common/matches-statistics/matches-statistics.component';
import { SideMenuComponent } from './components/common/side-menu/side-menu.component';
import { RequestPasswordResetComponent } from './components/routes/request-password-reset/request-password-reset.component';
import { ResetPasswordComponent } from './components/routes/reset-password/reset-password.component';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    LoginComponent,
    PlayersComponent,
    MatchesComponent,
    GamesComponent,
    RankingComponent,
    LoadingScreenComponent,
    AdminComponent,
    RegistrationComponent,
    PageNotFoundComponent,
    ActivationComponent,
    ProfileComponent,
    ChangePasswordComponent,
    PersonalInformationComponent,
    ProfileSettingsComponent,
    NavigationComponent,
    QuickPanelComponent,
    NotificationsComponent,
    FooterComponent,
    PrivacyPolicyComponent,
    DisclaimersComponent,
    OrderByPipe,
    ChatComponent,
    PlayerInfoComponent,
    ScoreBoardInfoComponent,
    NumberOneWidgetComponent,
    MatchListComponent,
    LiveMatchesComponent,
    DashboardTileComponent,
    RankingWidgetComponent,
    WidgetSelectorComponent,
    GameComponent,
    MatchComponent,
    PlayerComponent,
    ModalDialogComponent,
    LiveComponent,
    GiphyComponent,
    AutofocusDirective,
    MatchDetailComponent,
    RatingTimelineComponent,
    StatisticsComponent,
    PlayersStatisticsComponent,
    GamesStatisticsComponent,
    MatchesStatisticsComponent,
    SideMenuComponent,
    RequestPasswordResetComponent,
    ResetPasswordComponent
  ],
  entryComponents: [
    ModalDialogComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    RoutingModule,
    HttpClientModule,
    FormsModule,
    MaterialModule,
    ReactiveFormsModule,
    FlexLayoutModule,
    ToastrModule.forRoot({
      positionClass: 'toast-top-right',
      closeButton: true,
      preventDuplicates: true,
      progressBar: true
    }),
    GridsterModule,
    NgxFileDropModule,
    NgxPageScrollCoreModule.forRoot({duration: 500, scrollOffset: 20}),
    NgxPageScrollModule,
    QuillModule.forRoot(),
    Ng2ImgMaxModule,
    // RecaptchaV3Module,
    PickerModule,
    GoogleChartsModule.forRoot()
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    // { provide: RECAPTCHA_V3_SITE_KEY, useValue: environment.recaptchaSiteKey }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
