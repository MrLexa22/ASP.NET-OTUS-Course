import { useEffect, useState } from 'react'
import withLoader from '../../CommonComponents/withLoader';
import FactsList, { type IFact } from '../FactsList/FactsList';
import axios, { type AxiosResponse } from "axios";
import type { IFactsResponse, ILinksPage } from '../IFactsResponse';
import './Facts.css';
import ErrorDialog from '../../CommonComponents/Error';

function extractPageFromUrl(url: string | null): number | null {
   if (!url) return null;
   // сначала попробовать регулярку (без бросания исключений)
   const match = url.match(/[?&]page=(\d+)/);
   if (match) return Number(match[1]);
   try {
      const u = new URL(url);
      const p = u.searchParams.get('page');
      return p ? Number(p) : null;
   } catch {
      return null;
   }
}

const FactsListWithLoading = withLoader(FactsList);

const fetchFacts = async (page: number, signal?: AbortSignal): Promise<IFactsResponse> => {
   try {
      const response: AxiosResponse<IFactsResponse> = await axios.get(`https://catfact.ninja/facts?page=${page}`, { signal });
      return response.data;
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
   } catch (err: any) {
      if (axios.isCancel?.(err) || err?.name === 'CanceledError') {
         throw err;
      }
      throw new Error(`Error fetching facts: ${err?.message ?? err}`);
   }
};

function Facts() {
   const [isLoading, setIsLoading] = useState(true);
   const [facts, setFacts] = useState<IFact[]>([]);
   const [page, setPage] = useState(1);
   const [error, setError] = useState<string | null>(null);
   const [retryToken, setRetryToken] = useState(0);

   const [linksPages, setLinksPages] = useState<ILinksPage[]>([]);

   useEffect(() => {
      const controller = new AbortController();
      setIsLoading(true);

      fetchFacts(page, controller.signal)
         .then((data) => {
            setFacts(data.data);
            setLinksPages(data.links);
            setError(null);
         })
         .catch((error) => {
            if (error?.name === 'CanceledError' || error?.message === 'canceled') return;
            console.error(error);
            setError(error?.message ? String(error.message) : String(error));
         })
         .finally(() => {
            setIsLoading(false);
         });

      return () => {
         controller.abort();
      };
   }, [page, retryToken]);

   function gotoPage(p: number) {
      setPage(p);
   }

   return (
      <div>
         <FactsListWithLoading isLoading={isLoading} facts={facts} />

         <div className="pagination" role="navigation" aria-label="Постраничная навигация">
            {linksPages.map((link, i) => {
               const url = link.url;
               const pageFromUrl = extractPageFromUrl(url);
               let targetPage = pageFromUrl ?? page;
               if (pageFromUrl === null) {
                  const label = link.label.toLowerCase();
                  if (label.includes('previous')) targetPage = Math.max(1, page - 1);
                  if (label.includes('next')) targetPage = page + 1;
               }

               return (
                  <button
                     key={i}
                     className="pageButton"
                     onClick={() => gotoPage(targetPage)}
                     disabled={link.active || !url}
                     aria-current={link.active ? 'page' : undefined}
                     aria-label={`Перейти на страницу ${targetPage}`}
                  >
                     {link.label}
                  </button>
               );
            })}
         </div>

         <ErrorDialog
            open={!!error}
            title="Ошибка загрузки фактов"
            message={error ?? undefined}
            onClose={() => setError(null)}
            onRetry={() => {
               setError(null);
               setRetryToken(t => t + 1);
            }}
         />
      </div >
   );
}

export default Facts