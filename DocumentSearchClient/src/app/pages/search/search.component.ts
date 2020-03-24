import { Component, OnInit } from '@angular/core';
import { SearchapiService } from 'src/app/services/searchapi.service';
import { SearchResult } from 'src/dto/SearchResult';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {

  public query: string;

  public result: SearchResult;
  public hasItems: boolean;

  constructor(private searchApi: SearchapiService) {
    this.hasItems = false;
  }

  ngOnInit() {
  }

  async search() {
    this.result = await this.searchApi.Search(this.query);
    console.log("items " + this.result.items);
    console.log(this.result);
    if (this.result != null && this.result.items != null) {
      console.log("has items");
      this.hasItems = true;
    } else {
      console.log("no items");
      this.hasItems = false;
    }
  }

}
