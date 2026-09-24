<template>
  <VcBlade v-loading="loading" :title="bladeTitle" :toolbar-items="bladeToolbar" :modified="isModified" width="60%">
    <VcForm v-if="item">
      <VcContainer>
        <VcRow class="tw-space-x-4">
          <VcCol :size="6">
            <VcCard :header="$t('RETURNS.PAGES.DETAILS.FORM.RETURN_INFO.TITLE')">
              <div class="tw-p-4 tw-space-y-4">
                <VcField
                  v-if="item.number"
                  :label="$t('RETURNS.PAGES.DETAILS.FORM.RETURN_INFO.NUMBER')"
                  :model-value="item.number"
                  orientation="horizontal"
                  copyable
                />

                <VcField
                  :label="$t('RETURNS.PAGES.DETAILS.FORM.RETURN_INFO.ORDER_NUMBER')"
                  :model-value="item.order?.number || item.orderId"
                  orientation="horizontal"
                  copyable
                />

                <VcField
                  v-if="item.createdDate"
                  :label="$t('RETURNS.PAGES.DETAILS.FORM.RETURN_INFO.CREATED_DATE')"
                  :model-value="item.createdDate"
                  type="date"
                  orientation="horizontal"
                />

                <VcRow>
                  <VcCol :size="6">
                    <VcLabel class="tw-my-2">{{ t("RETURNS.PAGES.DETAILS.FORM.RETURN_INFO.STATUS") }}</VcLabel>
                  </VcCol>
                  <VcCol :size="6">
                    <VcSelect v-model="item.status" :options="statuses" />
                  </VcCol>
                </VcRow>
              </div>
            </VcCard>
          </VcCol>

          <VcCol :size="6">
            <VcCard :header="$t('RETURNS.PAGES.DETAILS.FORM.RESOLUTION.TITLE')">
              <div class="tw-p-4">
                <VcTextarea
                  v-model="item.resolution"
                  :placeholder="$t('RETURNS.PAGES.DETAILS.FORM.RESOLUTION.PLACEHOLDER')"
                  :rows="6"
                />
              </div>
            </VcCard>
          </VcCol>
        </VcRow>

        <div class="tw-mt-4">
          <VcCard :header="$t('RETURNS.PAGES.DETAILS.FORM.LINE_ITEMS.TITLE')">
            <VcDataTable
              :items="lineItemsView"
              :total-count="lineItemsView.length"
              edit-mode="cell"
              state-key="return-details-line-items"
              :add-row="{ enabled: addableItems.length > 0, label: t('RETURNS.PAGES.DETAILS.FORM.LINE_ITEMS.TOOLBAR.ADD_ITEM') }"
              @cell-edit-complete="onEditComplete"
              @row-add="onAddItem"
            >
              <VcColumn
                id="imageUrl"
                :title="t('RETURNS.PAGES.DETAILS.FORM.LINE_ITEMS.PIC')"
                width="60px"
                type="image"
                class="tw-pr-0"
              />

              <VcColumn
                id="name"
                :title="t('RETURNS.PAGES.DETAILS.FORM.LINE_ITEMS.NAME')"
                :always-visible="true"
              >
                <template #body="{ data }">
                  <ReturnLineItemName :name="data.name" :sku="data.sku" />
                </template>
              </VcColumn>

              <VcColumn
                id="quantity"
                :title="t('RETURNS.PAGES.DETAILS.FORM.LINE_ITEMS.QUANTITY')"
                :always-visible="true"
                type="number"
                editable
                :rules="{ min_value: 1, required: true }"
              />

              <VcColumn
                id="availableQuantity"
                :title="t('RETURNS.PAGES.DETAILS.FORM.LINE_ITEMS.AVAILABLE_QUANTITY')"
                type="number"
              />

              <VcColumn
                id="price"
                :title="t('RETURNS.PAGES.DETAILS.FORM.LINE_ITEMS.PRICE')"
                type="money"
              />

              <VcColumn
                id="reason"
                :title="t('RETURNS.PAGES.DETAILS.FORM.LINE_ITEMS.REASON')"
              />
            </VcDataTable>
          </VcCard>
        </div>
      </VcContainer>
    </VcForm>
  </VcBlade>
</template>

<script lang="ts" setup>
import { computed, onMounted } from "vue";
import { IBladeToolbar, useBlade, usePopup } from "@vc-shell/framework";
import {
  VcBlade,
  VcForm,
  VcContainer,
  VcRow,
  VcCol,
  VcCard,
  VcField,
  VcLabel,
  VcSelect,
  VcTextarea,
  VcDataTable,
  VcColumn,
} from "@vc-shell/framework/ui";
import { useI18n } from "vue-i18n";
import { useReturnDetails, RETURN_STATUSES } from "../composables";
import { ReturnLineItem } from "../../../api_client/VirtoCommerce.MarketplaceReturn";
import { ReturnLineItemName } from "../components";
import { ReturnLineItemCandidate } from "../types";

defineBlade({
  url: "/return-details",
  name: "ReturnDetails",
});

const { t } = useI18n({ useScope: "global" });
const { showConfirmation, showInfo } = usePopup();
const { param, options, openBlade, exposeToChildren, callParent, onBeforeClose } = useBlade<{ orderId?: string }>();

const { item, pristineItem, availableQuantities, isModified, loading, loadReturn, loadForOrder, saveReturn } =
  useReturnDetails();

const statuses = RETURN_STATUSES;

const isNew = computed(() => !item.value?.id);

const lineItemsView = computed(() => {
  const orderItemsById = new Map((item.value?.order?.items ?? []).map((orderItem) => [orderItem.id, orderItem]));
  return (item.value?.lineItems ?? []).map((lineItem) => {
    const orderItem = lineItem.orderLineItemId ? orderItemsById.get(lineItem.orderLineItemId) : undefined;
    return {
      ...lineItem,
      name: orderItem?.name,
      sku: orderItem?.sku,
      imageUrl: orderItem?.imageUrl,
    };
  });
});

const addableItems = computed<ReturnLineItemCandidate[]>(() => {
  const addedOrderLineItemIds = new Set((item.value?.lineItems ?? []).map((lineItem) => lineItem.orderLineItemId));
  return (item.value?.order?.items ?? [])
    .filter((orderItem) => orderItem.id && !addedOrderLineItemIds.has(orderItem.id))
    .map((orderItem) => ({
      id: orderItem.id as string,
      orderLineItemId: orderItem.id as string,
      name: orderItem.name,
      sku: orderItem.sku,
      imageUrl: orderItem.imageUrl,
      price: orderItem.price,
      availableQuantity: availableQuantities.value[orderItem.id as string] ?? 0,
    }))
    .filter((candidate) => candidate.availableQuantity > 0);
});

const bladeTitle = computed(() => item.value?.number || t("RETURNS.PAGES.DETAILS.NEW_TITLE"));

const bladeToolbar = computed((): IBladeToolbar[] => [
  {
    id: "save",
    title: t("RETURNS.PAGES.DETAILS.TOOLBAR.SAVE"),
    icon: "material-save",
    permissions: [isNew.value ? "marketplace-return:create" : "marketplace-return:update"],
    async clickHandler() {
      await saveReturn();
      await callParent("reload");
    },
    disabled: !isModified.value,
  },
]);

onBeforeClose(async () => {
  if (isModified.value) {
    return !(await showConfirmation(t("RETURNS.PAGES.DETAILS.CLOSE_CONFIRMATION")));
  }
  return false;
});

function onEditComplete(event: { data: unknown; field: string; newValue: unknown; index: number }) {
  if (!item.value?.lineItems) {
    return;
  }

  const currentLineItem = item.value.lineItems[event.index];
  if (!currentLineItem) {
    return;
  }

  let newValue = event.newValue;

  if (event.field === "quantity") {
    const pristineLineItem = currentLineItem.orderLineItemId
      ? pristineItem.value?.lineItems?.find((lineItem) => lineItem.orderLineItemId === currentLineItem.orderLineItemId)
      : undefined;
    // A line item just added via the picker has no pristine counterpart yet - its own
    // availableQuantity (captured at add time) is the ceiling instead.
    const max = pristineLineItem
      ? (pristineLineItem.quantity ?? 0) + (pristineLineItem.availableQuantity ?? 0)
      : (currentLineItem.availableQuantity ?? 0);
    const numeric = Number(event.newValue);
    const clamped = Math.min(Math.max(Number.isFinite(numeric) ? numeric : 0, 0), max);
    if (clamped !== numeric) {
      showInfo(t("RETURNS.PAGES.DETAILS.FORM.LINE_ITEMS.VALIDATION.MAX_QUANTITY", { max }));
    }
    newValue = clamped;
  }

  item.value.lineItems[event.index][event.field as keyof ReturnLineItem] = newValue as never;
}

function onAddItem(event: { defaults: Record<string, unknown>; cancel: () => void }) {
  event.cancel();
  openBlade({
    name: "ReturnLineItemPicker",
    options: { candidates: addableItems.value },
  });
}

function addLineItems(args: { items: ReturnLineItemCandidate[] }) {
  if (!item.value) {
    return;
  }
  if (!item.value.lineItems) {
    item.value.lineItems = [];
  }
  for (const candidate of args.items) {
    item.value.lineItems.push(
      new ReturnLineItem({
        orderLineItemId: candidate.orderLineItemId,
        availableQuantity: candidate.availableQuantity,
        quantity: Math.min(1, candidate.availableQuantity),
        price: candidate.price ?? 0,
      }),
    );
  }
}

exposeToChildren({ addLineItems });

onMounted(async () => {
  if (param.value) {
    await loadReturn(param.value);
  } else if (options.value?.orderId) {
    await loadForOrder(options.value.orderId);
  }
});
</script>
