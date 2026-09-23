import { defineAppModule, registerExternalWidget } from "@vc-shell/framework";
import { markRaw } from "vue";
import * as pages from "./pages";
import * as locales from "./locales";
import * as components from "./components";

registerExternalWidget({
  id: "ReturnsWidget",
  component: markRaw(components.ReturnsWidget),
  targetBlades: ["OrderDetails"],
  title: "RETURNS.WIDGET.TITLE",
});

export default defineAppModule({
  blades: pages,
  locales,
});
