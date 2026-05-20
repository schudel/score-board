import { TestBed, async } from '@angular/core/testing';
import { AppComponent } from './app.component';
import {DashboardComponent} from './components/routes/dashboard/dashboard.component';
import {LoginComponent} from './components/routes/login/login.component';
import {PlayersComponent} from './components/routes/players/players.component';
import {MatchesComponent} from './components/routes/matches/matches.component';
import {GamesComponent} from './components/routes/games/games.component';
import {RankingComponent} from './components/routes/ranking/ranking.component';
import {LoadingScreenComponent} from './components/common/loading-screen/loading-screen.component';
import {AdminComponent} from './components/routes/admin/admin.component';
import {RegistrationComponent} from './components/routes/registration/registration.component';
import {PageNotFoundComponent} from './components/routes/page-not-found/page-not-found.component';
import {ActivationComponent} from './components/routes/activation/activation.component';
import {ProfileComponent} from './components/routes/profile/profile.component';
import {ChangePasswordComponent} from './components/common/change-password/change-password.component';
import {PersonalInformationComponent} from './components/common/personal-information/personal-information.component';
import {ProfileSettingsComponent} from './components/common/profile-settings/profile-settings.component';
import {NavigationComponent} from './components/common/navigation/navigation.component';
import {QuickPanelComponent} from './components/common/quick-panel/quick-panel.component';
import {NotificationsComponent} from './components/common/notifications/notifications.component';
import {FooterComponent} from './components/common/footer/footer.component';
import {PrivacyPolicyComponent} from './components/routes/privacy-policy/privacy-policy.component';
import {DisclaimersComponent} from './components/routes/disclaimers/disclaimers.component';
import {OrderByPipe} from './pipes/order-by.pipe';
import {ChatComponent} from './components/common/chat/chat.component';
import {PlayerInfoComponent} from './components/common/player-info/player-info.component';
import {ScoreBoardInfoComponent} from './components/common/score-board-info/score-board-info.component';
import {NumberOneWidgetComponent} from './components/common/number-one-widget/number-one-widget.component';
import {MatchListComponent} from './components/common/match-list/match-list.component';
import {LiveMatchesComponent} from './components/common/live-matches/live-matches.component';
import {DashboardTileComponent} from './components/common/dashboard-tile/dashboard-tile.component';
import {RankingWidgetComponent} from './components/common/ranking-widget/ranking-widget.component';
import {WidgetSelectorComponent} from './components/common/widget-selector/widget-selector.component';
import {GameComponent} from './components/routes/game/game.component';
import {MatchComponent} from './components/routes/match/match.component';
import {PlayerComponent} from './components/routes/player/player.component';
import {ModalDialogComponent} from './components/common/modal-dialog/modal-dialog.component';
import {LiveComponent} from './components/routes/live/live.component';
import {GiphyComponent} from './components/common/giphy/giphy.component';
import {AutofocusDirective} from './directives/autofocus.directive';
import {MatchDetailComponent} from './components/common/match-detail/match-detail.component';
import {RatingTimelineComponent} from './components/common/rating-timeline/rating-timeline.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {GridsterModule} from 'angular-gridster2';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {ToastrModule} from 'ngx-toastr';
import {NgxFileDropModule} from 'ngx-file-drop';
import {NgxPageScrollCoreModule} from 'ngx-page-scroll-core';
import {NgxPageScrollModule} from 'ngx-page-scroll';
import {QuillModule} from 'ngx-quill';
import {Ng2ImgMaxModule} from 'ng2-img-max';
import {PickerModule} from '@ctrl/ngx-emoji-mart';
import {GoogleChartsModule} from 'angular-google-charts';
import {FlexLayoutModule} from '@angular/flex-layout';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatOptionModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import {MatMomentDatetimeModule} from '@mat-datetimepicker/moment';
import {MatDatetimepickerModule} from '@mat-datetimepicker/core';
import {DragDropModule} from '@angular/cdk/drag-drop';
import {BrowserModule} from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {RouterTestingModule} from '@angular/router/testing';
import {AuthenticationGuard} from './guards/authentication.guard';
import {Role} from './models/role';
import {CommonModule} from '@angular/common';
import {AuthenticationService} from './services/rest/authentication.service';

describe('AppComponent', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
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
        RatingTimelineComponent
      ],
      imports: [
        CommonModule,
        BrowserModule,
        BrowserAnimationsModule,
        ReactiveFormsModule,
        RouterTestingModule.withRoutes(  [  { path: 'login', component: LoginComponent },
          { path: 'registration', component: RegistrationComponent },
          { path: 'activate/:id', component: ActivationComponent },
          { path: 'privacy-policy', component: PrivacyPolicyComponent },
          { path: 'disclaimers', component: DisclaimersComponent },
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

          { path: 'admin', component: AdminComponent, canActivate: [AuthenticationGuard], data: { roles: [Role.Admin] } },
          { path: '404', component: PageNotFoundComponent, canActivate: [AuthenticationGuard] },
          // otherwise redirect to 404 - page not found
          { path: '**', redirectTo: '/dashboard' }] ),
        FormsModule,
        MatToolbarModule,
        MatSidenavModule,
        MatButtonModule,
        MatCardModule,
        MatInputModule,
        MatDialogModule,
        MatTableModule,
        MatMenuModule,
        MatIconModule,
        MatProgressSpinnerModule,
        MatTooltipModule,
        MatCheckboxModule,
        MatListModule,
        MatSidenavModule,
        MatSortModule,
        MatTabsModule,
        MatSlideToggleModule,
        MatBadgeModule,
        MatSelectModule,
        MatOptionModule,
        MatChipsModule,
        MatAutocompleteModule,
        MatDatepickerModule,
        MatMomentDatetimeModule,
        MatDatetimepickerModule,
        MatFormFieldModule,
        DragDropModule,
        MatExpansionModule,
        ReactiveFormsModule,
        FlexLayoutModule,
        GridsterModule,
        HttpClientTestingModule,
        ToastrModule.forRoot({
          positionClass: 'toast-top-right',
          closeButton: true,
          preventDuplicates: true,
          progressBar: true
        }),
        NgxFileDropModule,
        NgxPageScrollCoreModule.forRoot({duration: 500, scrollOffset: 20}),
        NgxPageScrollModule,
        QuillModule.forRoot(),
        Ng2ImgMaxModule,
        PickerModule,
        GoogleChartsModule.forRoot()
      ]
    }).compileComponents();
  }));

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have as title 'Scoreboard'`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app.title).toEqual('Scoreboard');
  });
});
