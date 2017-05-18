import * as t from './actionTypes';

export default function middleware(store: any) {
    return (next: any) => (action: any) => {
        switch (action.type) {
            case t.SEND:
                window._hub.server.send(action.payload);
                break;
            case t.RECEIVE:
                switch(action.payload.name)
                {
                    default:
                        break;
                }

                break;
            default:
                break;
        }
        return next(action);
    }
}