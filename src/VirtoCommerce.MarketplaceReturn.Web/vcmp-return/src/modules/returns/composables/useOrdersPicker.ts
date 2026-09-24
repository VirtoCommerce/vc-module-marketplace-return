import { computed, ref, inject, ComputedRef, Ref } from "vue";
import { useRoute } from "vue-router";
import { toRef } from "@vueuse/core";
import { useAsync, useApiClient, useLoading } from "@vc-shell/framework";
import {
  VcmpSellerOrdersClient,
  type Seller,
  type SearchOrdersQuery,
  type CustomerOrder,
  type CustomerOrderSearchResult,
} from "@vcmp-vendor-portal/api/marketplacevendor";

export interface IUseOrdersPicker {
  items: ComputedRef<CustomerOrder[]>;
  totalCount: ComputedRef<number>;
  pages: ComputedRef<number>;
  currentPage: ComputedRef<number>;
  searchQuery: Ref<SearchOrdersQuery>;
  loadOrders: (query?: SearchOrdersQuery) => Promise<void>;
  loading: ComputedRef<boolean>;
}

export interface UseOrdersPickerOptions {
  pageSize?: number;
}

export function useOrdersPicker(options?: UseOrdersPickerOptions): IUseOrdersPicker {
  const { getApiClient } = useApiClient(VcmpSellerOrdersClient);

  const route = useRoute();
  const currentSeller = inject(
    "currentSeller",
    toRef({ id: route?.params?.sellerId as string | undefined }),
  ) as Ref<Partial<Seller>>;

  const pageSize = options?.pageSize || 20;
  const searchQuery = ref<SearchOrdersQuery>({
    take: pageSize,
    sellerId: currentSeller.value?.id,
  });
  const searchResult = ref<CustomerOrderSearchResult>();

  const { action: loadOrders, loading } = useAsync<SearchOrdersQuery>(async (query) => {
    searchQuery.value = { ...searchQuery.value, ...(query || {}) };

    const apiClient = await getApiClient();
    searchResult.value = await apiClient.searchOrders(searchQuery.value);
  });

  return {
    items: computed(() => searchResult.value?.results || []),
    totalCount: computed(() => searchResult.value?.totalCount || 0),
    pages: computed(() => Math.ceil((searchResult.value?.totalCount || 1) / pageSize)),
    currentPage: computed(() => Math.ceil((searchQuery.value?.skip || 0) / Math.max(1, pageSize) + 1)),
    searchQuery,
    loadOrders,
    loading: useLoading(loading),
  };
}
