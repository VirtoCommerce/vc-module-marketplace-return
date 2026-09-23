import VirtoShellFramework, { notification, useUser, useLanguages } from "@vc-shell/framework";
import { createApp } from "vue";
import { RouterView } from "vue-router";
import { router } from "./router";
import * as locales from "./locales";
import Returns from "./modules/returns";
import { bootstrap } from "./bootstrap";

import "@vc-shell/framework/dist/index.css";

async function startApp() {
  const { loadUser } = useUser();

  try {
    await loadUser();
  } catch (e) {
    console.log(e);
  }

  const { currentLocale, setLocale } = useLanguages();

  const app = createApp(RouterView)
    .use(VirtoShellFramework, {
      router,
      i18n: {
        locale: import.meta.env.APP_I18N_LOCALE,
        fallbackLocale: import.meta.env.APP_I18N_FALLBACK_LOCALE,
      },
    })
    .use(Returns)
    .use(router);

  Object.entries(locales).forEach(([key, message]) => {
    app.config.globalProperties.$mergeLocaleMessage(key, message);
  });

  setLocale(currentLocale.value);

  bootstrap(app);

  app.config.errorHandler = (err) => {
    notification.error((err as Error).toString(), {
      timeout: 5000,
    });
  };

  await router.isReady();

  app.mount("#app");
}

startApp();
