import { computed, ref, inject, ComputedRef, Ref } from "vue";
import { useRoute } from "vue-router";
import { toRef } from "@vueuse/core";
import { useAsync, useApiClient, useLoading } from "@vc-shell/framework";
import type { Seller } from "@vcmp-vendor-portal/api/marketplacevendor";
import {
  VcmpReturnClient,
  Return,
  SearchReturnsQuery,
  ISearchReturnsQuery,
  ReturnSearchResult,
} from "../../../api_client/VirtoCommerce.MarketplaceReturn";

export const RETURN_STATUSES = ["New", "Approved", "Completed", "Canceled", "Processing"];

export interface IUseReturnsList {
  items: ComputedRef<Return[]>;
  totalCount: ComputedRef<number>;
  pages: ComputedRef<number>;
  currentPage: ComputedRef<number>;
  searchQuery: Ref<ISearchReturnsQuery>;
  loadReturns: (query?: ISearchReturnsQuery) => Promise<void>;
  loading: ComputedRef<boolean>;
  statuses: string[];
}

export interface UseReturnsListOptions {
  pageSize?: number;
  sort?: string;
  orderId?: string;
}

export function useReturnsList(options?: UseReturnsListOptions): IUseReturnsList {
  const { getApiClient } = useApiClient(VcmpReturnClient);

  const route = useRoute();
  const currentSeller = inject(
    "currentSeller",
    toRef({ id: route?.params?.sellerId as string | undefined }),
  ) as Ref<Partial<Seller>>;

  const pageSize = options?.pageSize || 20;
  const searchQuery = ref<ISearchReturnsQuery>({
    take: pageSize,
    sort: options?.sort,
    orderId: options?.orderId,
    sellerId: currentSeller.value?.id,
  });
  const searchResult = ref<ReturnSearchResult>();

  const { action: loadReturns, loading } = useAsync<ISearchReturnsQuery>(async (query) => {
    searchQuery.value = { ...searchQuery.value, ...(query || {}) };

    const apiClient = await getApiClient();
    searchResult.value = await apiClient.searchReturns(new SearchReturnsQuery(searchQuery.value));
  });

  return {
    items: computed(() => searchResult.value?.results || []),
    totalCount: computed(() => searchResult.value?.totalCount || 0),
    pages: computed(() => Math.ceil((searchResult.value?.totalCount || 1) / pageSize)),
    currentPage: computed(() => Math.ceil((searchQuery.value?.skip || 0) / Math.max(1, pageSize) + 1)),
    searchQuery,
    loadReturns,
    loading: useLoading(loading),
    statuses: RETURN_STATUSES,
  };
}
