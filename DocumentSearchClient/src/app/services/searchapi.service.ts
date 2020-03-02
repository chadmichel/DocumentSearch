import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { SearchResult } from 'src/dto/SearchResult';

@Injectable({
  providedIn: 'root'
})
export class SearchapiService {


  baseUrl = 'http://localhost:5150/api/';


  constructor(private http: HttpClient) { }

  private async get<T>(url: string): Promise<T> {
    const options = this.httpOptions();
    const fullUrl = this.baseUrl + url;
    const p = this.http.get<T>(fullUrl, <Object>options).toPromise();
    return p;
  }

  public async Search(query: string): Promise<SearchResult> {
    const result = await this.get<SearchResult>('Search?query=' + query);
    console.log(result);
    return result;
  }

  httpOptions(): any {

    const subId = '';

    return {
        headers: new HttpHeaders({
            Accept: 'application/json',
            'Ocp-Apim-Subscription-Key': subId
        }),
    };
  }

}
