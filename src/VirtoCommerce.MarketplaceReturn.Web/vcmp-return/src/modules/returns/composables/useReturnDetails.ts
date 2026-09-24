import { Ref, ref, inject, ComputedRef } from "vue";
import { useRoute } from "vue-router";
import { toRef } from "@vueuse/core";
import { useAsync, useApiClient, useLoading, useModificationTracker } from "@vc-shell/framework";
import { VcmpSellerOrdersClient, type Seller } from "@vcmp-vendor-portal/api/marketplacevendor";
import {
  VcmpReturnClient,
  Return,
  CustomerOrder,
  OrderLineItem,
  UpdateReturnCommand,
} from "../../../api_client/VirtoCommerce.MarketplaceReturn";

export interface IUseReturnDetails {
  item: Ref<Return | undefined>;
  pristineItem: Ref<Return | undefined>;
  availableQuantities: Ref<Record<string, number>>;
  isModified: Ref<boolean>;
  loading: ComputedRef<boolean>;
  loadReturn: (id: string) => Promise<void>;
  loadForOrder: (orderId: string) => Promise<void>;
  saveReturn: () => Promise<void>;
}

export function useReturnDetails(): IUseReturnDetails {
  const { getApiClient } = useApiClient(VcmpReturnClient);
  const { getApiClient: getOrderApiClient } = useApiClient(VcmpSellerOrdersClient);

  const route = useRoute();
  const currentSeller = inject(
    "currentSeller",
    toRef({ id: route?.params?.sellerId as string | undefined }),
  ) as Ref<Partial<Seller>>;

  const item = ref<Return>();
  const { currentValue, pristineValue, isModified, resetModificationState } = useModificationTracker(item);
  const availableQuantities = ref<Record<string, number>>({});

  async function loadOrder(orderId: string): Promise<CustomerOrder | undefined> {
    const orderApiClient = await getOrderApiClient();
    const order = await orderApiClient.getById(orderId);
    if (!order) {
      return undefined;
    }

    return new CustomerOrder({
      number: order.number,
      items: (order.items ?? []).map(
        (orderItem) =>
          new OrderLineItem({
            id: orderItem.id,
            name: orderItem.name,
            sku: orderItem.sku,
            imageUrl: orderItem.imageUrl,
            price: orderItem.price,
            quantity: orderItem.quantity,
          }),
      ),
    });
  }

  const { action: loadReturn, loading: loadingReturn } = useAsync<string>(async (id) => {
    if (!id) {
      return;
    }

    const apiClient = await getApiClient();
    const result = await apiClient.getReturnById(id);
    if (result.orderId) {
      availableQuantities.value = await apiClient.getAvailableQuantities(result.orderId);
    }
    resetModificationState(result);
    currentValue.value = result;
  });

  const { action: loadForOrder, loading: loadingOrder } = useAsync<string>(async (orderId) => {
    if (!orderId) {
      return;
    }

    const apiClient = await getApiClient();
    const [quantities, order] = await Promise.all([apiClient.getAvailableQuantities(orderId), loadOrder(orderId)]);
    availableQuantities.value = quantities;

    const draft = new Return({
      orderId,
      status: "New",
      order,
      lineItems: [],
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
      // A line item just added via the picker has no pristine counterpart yet - its own
      // availableQuantity (captured at add time) is the ceiling instead.
      const max = pristineLineItem
        ? (pristineLineItem.quantity ?? 0) + (pristineLineItem.availableQuantity ?? 0)
        : (lineItem.availableQuantity ?? 0);
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
    availableQuantities,
    isModified,
    loading: useLoading(loadingReturn, loadingOrder, savingReturn),
    loadReturn,
    loadForOrder,
    saveReturn,
  };
}
