export interface ReturnLineItemCandidate {
  orderLineItemId: string;
  name?: string;
  sku?: string;
  imageUrl?: string;
  price?: number;
  availableQuantity: number;
}
