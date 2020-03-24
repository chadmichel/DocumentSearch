import { SearchResultItem } from './SearchResultItem';

export interface SearchResult {
    query: string;
    items: SearchResultItem[];
}
