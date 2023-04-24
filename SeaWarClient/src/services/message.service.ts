import { Subject } from "rxjs";
import type { ToastMessageOptions } from "primevue/toast";

class MessageService {
  messageBus = new Subject<ToastMessageOptions>();

  AddMessage(message: ToastMessageOptions) {
    message.life = message.life || 3000;
    this.messageBus.next(message);
  }

  AddErrorMessage(summary: string, detail: string) {
    this.AddMessage({
      severity: "error",
      summary,
      detail,
      life: 5000,
    });
  }
}

export const messageService = new MessageService();
