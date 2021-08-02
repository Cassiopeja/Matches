import { HubConnectionBuilder } from "@microsoft/signalr";
const eventEmitter = require("events");

class GameHub  extends eventEmitter {
  constructor() {
    super();
    this.client = new HubConnectionBuilder()
      .withUrl("/gameHub")
      .withAutomaticReconnect()
      .build();
  }

  start() {
    this.client.start().
    then(()=> {
      if (this.client.state === "Connected"){
        this.emit("ConnectedToHub");
      }
    });
  }
}

export default new GameHub();
