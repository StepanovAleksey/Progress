import { createApp } from "vue";
import { createPinia } from "pinia";
import App from "./App.vue";

/* import the fontawesome core */
import PrimeVue from "primevue/config";

import Accordion from "primevue/accordion";
import AccordionTab from "primevue/accordiontab";
import Button from "primevue/button";
import SplitButton from "primevue/splitbutton";
import Splitter from "primevue/splitter";
import SplitterPanel from "primevue/splitterpanel";
import DynamicDialog from "primevue/dynamicdialog";
import DialogService from "primevue/dialogservice";
import InputText from "primevue/inputtext";
import Password from "primevue/password";
import Toast from "primevue/toast";
import ToastService from "primevue/toastservice";
import ProgressSpinner from "primevue/progressspinner";

import "./assets/main.css";

//core
import "primevue/resources/primevue.min.css";
//theme
import "primevue/resources/themes/lara-light-indigo/theme.css";
//icons
import "primeicons/primeicons.css";
import "primeflex/primeflex.css";
import { appAxios } from "./services/axios.service";
import { messageService } from "./services/message.service";

createApp(App)
  .use(createPinia())
  .use(PrimeVue)
  .use(DialogService)
  .use(ToastService)
  .component("Accordion", Accordion)
  .component("AccordionTab", AccordionTab)
  .component("Button", Button)
  .component("SplitButton", SplitButton)
  .component("Splitter", Splitter)
  .component("SplitterPanel", SplitterPanel)
  .component("DynamicDialog", DynamicDialog)
  .component("InputText", InputText)
  .component("Password", Password)
  .component("Toast", Toast)
  .component("ProgressSpinner", ProgressSpinner)
  .mount("#app");

appAxios.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response?.data?.errors) {
      Object.values(error.response.data.errors).forEach((values) => {
        (values as string[]).forEach((msg) => {
          messageService.AddErrorMessage("Ошибка сервера", msg);
        });
      });
    } else {
      messageService.AddErrorMessage("Ошибка сервера", error.response.data);
    }
    return Promise.reject(error);
  }
);
