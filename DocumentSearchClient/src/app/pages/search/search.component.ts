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

  constructor(private searchApi: SearchapiService) { 

  }

  ngOnInit() {
  }

  async search() {    
    this.result = await (await this.searchApi.Search(this.query));
    console.log(this.result);
  }

}
