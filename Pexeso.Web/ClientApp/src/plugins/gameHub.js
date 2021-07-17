import {HubConnectionBuilder} from "@microsoft/signalr";

class GameHub {
    constructor() {
        this.client = new HubConnectionBuilder()
            .withUrl("/gameHub")
            .build();
    }

    start() {
        this.client.start();
    }
}

export default new GameHub();