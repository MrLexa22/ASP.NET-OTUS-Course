import type { IFact } from "./FactsList/FactsList";

export interface IFactsResponse {
   current_page: number;
   data: IFact[];
   first_page_url: string;
   from: number;
   last_page: number;
   last_page_url: string;
   links: Array<ILinksPage>;
   next_page_url: string | null;
   path: string;
   per_page: number;
   prev_page_url: string | null;
   to: number;
   total: number;
}

export interface ILinksPage {
   url: string | null;
   label: string;
   active: boolean;
}