import { SearchResultItem } from './SearchResultItem';

export interface SearchResult {
    Query: string;
    Items: SearchResultItem[];
}
