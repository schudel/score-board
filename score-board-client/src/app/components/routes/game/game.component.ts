import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {GameService} from '../../../services/rest/game.service';
import {Game} from '../../../models/game';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {FileSystemFileEntry, NgxFileDropEntry} from 'ngx-file-drop';
import {DomSanitizer, SafeResourceUrl} from '@angular/platform-browser';
import {NotificationService} from '../../../services/common/notification.service';
import {Player} from '../../../models/player';
import {Role} from '../../../models/role';
import {AuthenticationService} from '../../../services/rest/authentication.service';
import {MatchService} from '../../../services/rest/match.service';
import {Ng2ImgMaxService} from 'ng2-img-max';
import {Guid} from '../../../helpers/guid';
import {Observable} from 'rxjs';
import {map, startWith} from 'rxjs/operators';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.scss']
})
export class GameComponent implements OnInit, OnDestroy {
  subscriptions = [];
  game: Game;
  error = '';
  submitted = false;
  gameForm: FormGroup;
  files: NgxFileDropEntry[] = [];
  base64textString = '';
  image: SafeResourceUrl;
  isImageChanged = false;
  isEditMode = false;
  isCreateMode = false;
  currentPlayer: Player;
  numberOfMatches: number;
  genres: string[];
  types: string[];
  filteredTypes: Observable<string[]>;
  filteredGenres: Observable<string[]>;
  private gameId: string;

  constructor(private activeRoute: ActivatedRoute,
              private gameService: GameService,
              private notificationService: NotificationService,
              private sanitizer: DomSanitizer,
              private authenticationService: AuthenticationService,
              private matchService: MatchService,
              private ng2ImgMax: Ng2ImgMaxService) {
    this.gameForm = new FormGroup({
      name: new FormControl('', Validators.required),
      type: new FormControl('', Validators.required),
      genre: new FormControl('', Validators.required),
      beta: new FormControl('', Validators.required),
      drawProbability: new FormControl('', Validators.required),
      dynamicsFactor: new FormControl('', Validators.required),
      initialConservativeRating: new FormControl('', Validators.required),
      initialMean: new FormControl('', Validators.required),
      initialStandardDeviation: new FormControl('', Validators.required)
    });
    this.genres = [];
    this.types = [];
  }

  ngOnInit() {
    const routeParams = this.activeRoute.snapshot.params;
    const path = this.activeRoute.snapshot.routeConfig.path;
    if (path === 'game/view/:id') {
      this.isEditMode = false;
      this.isCreateMode = false;
      if (routeParams == null || routeParams.id == null) {
        return;
      }
      this.gameId = routeParams.id;
    } else if (path === 'game/edit/:id') {
      this.isEditMode = true;
      this.isCreateMode = false;
      if (routeParams == null || routeParams.id == null) {
        return;
      }
      this.gameId = routeParams.id;
      this.initFilters();
    } else if (path === 'game/create') {
      this.isEditMode = false;
      this.isCreateMode = true;
      this.game = new Game();
      this.initForm();
      this.initFilters();
      return;
    } else {
      this.isEditMode = false;
      this.isCreateMode = false;
      return;
    }
    this.subscriptions.push(this.gameService.get(this.gameId).subscribe(g => {
      this.game = g;
      this.subscriptions.push(this.matchService.countByGame(this.game.id).subscribe(x => this.numberOfMatches = x));
      this.initForm();
    }));
    this.subscriptions.push(this.authenticationService.currentPlayer.subscribe(x => this.currentPlayer = x));
  }

  // convenience getter for easy access to form fields
  get form() { return this.gameForm.controls; }

  initForm() {
    if (!this.isCreateMode) {
      this.gameForm.setValue({
        name: this.game.name,
        type:  this.game.type,
        genre: this.game.genre,
        beta: this.game.beta,
        drawProbability: this.game.drawProbability,
        dynamicsFactor: this.game.dynamicsFactor,
        initialConservativeRating: this.game.initialConservativeRating,
        initialMean: this.game.initialMean,
        initialStandardDeviation: this.game.initialStandardDeviation});
      if (this.game.image) {
        this.image = this.sanitizer.bypassSecurityTrustResourceUrl(this.game.image);
      }
    } else if (this.isCreateMode) {
      this.gameForm.setValue({
        name: '',
        type:  '',
        genre: '',
        beta: 4.166666666666667,
        drawProbability: 0.1,
        dynamicsFactor: 0.08333333333333333,
        initialConservativeRating: 0,
        initialMean: 25,
        initialStandardDeviation: 8.333333333333334});
    }

    if (!this.isEditMode && !this.isCreateMode) {
      this.form.name.disable();
      this.form.type.disable();
      this.form.genre.disable();
      this.form.beta.disable();
      this.form.drawProbability.disable();
      this.form.dynamicsFactor.disable();
      this.form.initialConservativeRating.disable();
      this.form.initialMean.disable();
      this.form.initialStandardDeviation.disable();
    }
  }

  onSubmit() {
    this.submitted = true;
    // stop here if form is invalid
    if (this.gameForm.invalid) {
      return;
    }

    if (this.isCreateMode) {
      const newGame = new Game();
      newGame.id = Guid.newGuid().ToString();
      newGame.beta = this.form.beta.value;
      newGame.drawProbability = this.form.drawProbability.value;
      newGame.dynamicsFactor = this.form.dynamicsFactor.value;
      newGame.genre = this.form.genre.value;
      newGame.initialConservativeRating = this.form.initialConservativeRating.value;
      newGame.initialMean = this.form.initialMean.value;
      newGame.initialStandardDeviation = this.form.initialStandardDeviation.value;
      newGame.name = this.form.name.value;
      newGame.type = this.form.type.value;
      if (this.isImageChanged) {
        newGame.image = this.base64textString;
      }
      this.subscriptions.push(this.gameService.add(newGame).subscribe(() => {
        this.notificationService.success($localize`:@@addGameTextPart1:The Game` + ' \'' + newGame.name + '\' ' + $localize`:@@addGameTextPart2:was added` + '!', $localize`:@@addGameTitle:Add Game`);
      }));
    } else if (this.isEditMode) {
      this.game.beta = this.form.beta.value;
      this.game.drawProbability = this.form.drawProbability.value;
      this.game.dynamicsFactor = this.form.dynamicsFactor.value;
      this.game.genre = this.form.genre.value;
      this.game.initialConservativeRating = this.form.initialConservativeRating.value;
      this.game.initialMean = this.form.initialMean.value;
      this.game.initialStandardDeviation = this.form.initialStandardDeviation.value;
      this.game.name = this.form.name.value;
      this.game.type = this.form.type.value;
      if (this.isImageChanged) {
        this.game.image = this.base64textString;
      }
      this.subscriptions.push(this.gameService.update(this.game).subscribe(() => {
        this.notificationService.success($localize`:@@updateGameTextPart1:The Game` + ' \'' + this.game.name + '\' ' + $localize`:@@updateGameTextPart2:was updated` + '!', $localize`:@@updateGameTitle:Update Game`);
      }));
    }
  }

  removeImage() {
    this.image = '';
    this.base64textString = '';
    this.isImageChanged = true;
  }

  dropped(files: NgxFileDropEntry[]) {
    this.files = files;
    if (this.files.length > 1) {
      this.notificationService.error('You can only upload one Image',
        'Image upload failed');
    }

    for (const droppedFile of files) {
      // Is it a file?
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          if (file) {
            this.subscriptions.push(this.ng2ImgMax.resizeImage(file, 300, 200).subscribe(
              result => {
                const reader = new FileReader();
                reader.onload = this.handleReaderLoaded.bind(this);
                reader.readAsBinaryString(result);
              },
              error => {
                this.notificationService.error('Resize Image failed: ' + error,
                  'Resize Image failed');
              }
            ));
          }
          this.base64textString = 'data:' + file.type + ';base64,';
        });
      }
    }
  }

  handleReaderLoaded(readerEvt) {
    const binaryString = readerEvt.target.result;
    this.base64textString = this.base64textString + btoa(binaryString);
    this.image = this.sanitizer.bypassSecurityTrustResourceUrl(this.base64textString);
    this.isImageChanged = true;
  }

  public get isAdmin(): boolean {
    return this.currentPlayer && this.currentPlayer.role === Role.Admin;
  }

  private filterTypes(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.types.filter(option => option.toLowerCase().includes(filterValue));
  }

  private filterGenres(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.genres.filter(option => option.toLowerCase().includes(filterValue));
  }

  private initFilters() {
    this.subscriptions.push(this.gameService.getGenres().subscribe(x => {
      this.genres = x;
      this.filteredGenres = this.form.genre.valueChanges
        .pipe(
          startWith(''),
          map(value => {
            return this.filterGenres(value);
          })
        );
    }));
    this.subscriptions.push(this.gameService.getTypes().subscribe(x => {
      this.types = x;
      this.filteredTypes = this.form.type.valueChanges
        .pipe(
          startWith(''),
          map(value => {
            return this.filterTypes(value);
          })
        );
    }));
  }

  ngOnDestroy(): void {
    // when the component get's destroyed, unsubscribe all the subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
