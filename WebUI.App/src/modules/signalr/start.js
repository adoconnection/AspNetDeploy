export default function start(store: any, callback: Function) {
    window._hub = $.connection.appHub;

    window._hub.client.receive = (p: any) => {
        store.dispatch({ type: "signalr/RECEIVE", payload: p });
    };

    $.connection.hub.start(() => callback());
}
