import {Component, EventEmitter, HostListener, OnInit, Output} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';

@Component({
  selector: 'app-giphy',
  templateUrl: './giphy.component.html',
  styleUrls: ['./giphy.component.scss']
})
export class GiphyComponent implements OnInit {
  // TODO: replace with production API KEY
  private giphy = require('giphy-api')({
    apiKey : '{key}',
    https: true
  });
  giphyResults = [];
  showGiphySearch = false;
  giphyForm: FormGroup;
  offset: number;
  limit: number;

  @Output()
  giphyImageSelected: EventEmitter<string> = new EventEmitter<string>();

  constructor() {
    this.giphyForm = new FormGroup({
      giphySearchTerm: new FormControl('', Validators.required)
    });
    this.offset = 0;
    this.limit = 25;
    this.giphyResults = [];
  }

  ngOnInit() {
    this.onChanges();
  }

  // convenience getter for easy access to form fields
  get form() { return this.giphyForm.controls; }

  searchGiphy() {
    const searchTerm = this.form.giphySearchTerm.value;
    if (!searchTerm) {
      return;
    }
    this.giphy.search({
      q: searchTerm,
      limit: this.limit,
      offset: this.offset}).then(res => {
        if (this.offset > 0) {
          for (const d of res.data) {
            this.giphyResults.push(d);
          }
        } else {
          this.giphyResults = res.data;
        }
      })
      .catch(console.error);
  }

  loadTrendsGiphy() {
    this.giphy.trending({
      offset: this.offset,
      limit: this.limit})
      .then(res => {
        if (this.offset > 0) {
          for (const d of res.data) {
            this.giphyResults.push(d);
          }
        } else {
          this.giphyResults = res.data;
        }
      })
      .catch(console.error);
  }

  toggleGiphySearch() {
    if (!this.showGiphySearch) {
      this.offset = 0;
      this.loadTrendsGiphy();
    }
    this.showGiphySearch = !this.showGiphySearch;
  }

  sendGif(title: string | HTMLTitleElement, url: string) {
    this.giphyImageSelected.emit(`<img src="${url}" alt="${title}" class="giphy-image" style="max-width: 380px !important;">`);
    this.giphyForm.setValue({
      giphySearchTerm: ''
    });
    this.showGiphySearch = false;
  }

  onChanges(): void {
    this.giphyForm.get('giphySearchTerm').valueChanges.subscribe(val => {
      setTimeout( () => {
        this.offset = 0;
        this.searchGiphy(); }, 200 );
    });
  }

  @HostListener('scroll', ['$event'])
  onScroll(event: any) {
    if (event.target.offsetHeight + event.target.scrollTop >= event.target.scrollHeight) {
      this.offset = this.offset + this.limit;
      if (this.form.giphySearchTerm.value) {
        this.searchGiphy();
      } else {
        this.loadTrendsGiphy();
      }
    }
  }
}
