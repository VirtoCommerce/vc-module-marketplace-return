import { Ref, ref, inject, ComputedRef } from "vue";
import { useRoute } from "vue-router";
import { toRef } from "@vueuse/core";
import { useAsync, useApiClient, useLoading, useModificationTracker } from "@vc-shell/framework";
import type { Seller } from "@vcmp-vendor-portal/api/marketplacevendor";
import {
  VcmpReturnClient,
  Return,
  ReturnLineItem,
  UpdateReturnCommand,
} from "../../../api_client/virtocommerce.marketplacereturn";

export interface IUseReturnDetails {
  item: Ref<Return | undefined>;
  pristineItem: Ref<Return | undefined>;
  isModified: Ref<boolean>;
  loading: ComputedRef<boolean>;
  loadReturn: (id: string) => Promise<void>;
  loadForOrder: (orderId: string) => Promise<void>;
  saveReturn: () => Promise<void>;
}

export function useReturnDetails(): IUseReturnDetails {
  const { getApiClient } = useApiClient(VcmpReturnClient);

  const route = useRoute();
  const currentSeller = inject(
    "currentSeller",
    toRef({ id: route?.params?.sellerId as string | undefined }),
  ) as Ref<Partial<Seller>>;

  const item = ref<Return>();
  const { currentValue, pristineValue, isModified, resetModificationState } = useModificationTracker(item);

  const { action: loadReturn, loading: loadingReturn } = useAsync<string>(async (id) => {
    if (!id) {
      return;
    }

    const apiClient = await getApiClient();
    const result = await apiClient.getReturnById(id);
    resetModificationState(result);
    currentValue.value = result;
  });

  const { action: loadForOrder, loading: loadingOrder } = useAsync<string>(async (orderId) => {
    if (!orderId) {
      return;
    }

    const apiClient = await getApiClient();
    const availableQuantities = await apiClient.getAvailableQuantities(orderId);

    const draft = new Return({
      orderId,
      status: "New",
      lineItems: Object.entries(availableQuantities).map(
        ([orderLineItemId, availableQuantity]) =>
          new ReturnLineItem({
            orderLineItemId,
            availableQuantity,
            quantity: 0,
            price: 0,
          }),
      ),
    });

    resetModificationState(draft);
    currentValue.value = draft;
  });

  const { action: saveReturn, loading: savingReturn } = useAsync(async () => {
    if (!currentValue.value) {
      return;
    }

    const pristineLineItemsByOrderLineItemId = new Map(
      (pristineValue.value?.lineItems ?? []).map((lineItem) => [lineItem.orderLineItemId, lineItem]),
    );
    for (const lineItem of currentValue.value.lineItems ?? []) {
      const pristineLineItem = lineItem.orderLineItemId
        ? pristineLineItemsByOrderLineItemId.get(lineItem.orderLineItemId)
        : undefined;
      const max = (pristineLineItem?.quantity ?? 0) + (pristineLineItem?.availableQuantity ?? 0);
      const numeric = Number(lineItem.quantity);
      lineItem.quantity = Math.min(Math.max(Number.isFinite(numeric) ? numeric : 0, 0), max);
    }

    const apiClient = await getApiClient();
    await apiClient.updateReturn(
      new UpdateReturnCommand({
        sellerId: currentSeller.value?.id,
        sellerName: currentSeller.value?.name,
        orderReturn: currentValue.value,
      }),
    );
    resetModificationState(currentValue.value);
  });

  return {
    item: currentValue,
    pristineItem: pristineValue,
    isModified,
    loading: useLoading(loadingReturn, loadingOrder, savingReturn),
    loadReturn,
    loadForOrder,
    saveReturn,
  };
}
