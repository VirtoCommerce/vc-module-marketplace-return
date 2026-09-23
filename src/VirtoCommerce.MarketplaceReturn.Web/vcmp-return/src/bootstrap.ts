import { addMenuItem } from "@vc-shell/framework";
import { App } from "vue";

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export function bootstrap(app: App) {
  addMenuItem({
    title: "SHELL.MENU.DASHBOARD",
    icon: "lucide-home",
    priority: 0,
    url: "/",
  });
}
