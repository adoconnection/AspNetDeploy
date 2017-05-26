import * as t from './actionTypes';
import * as c from './commands';

import * as sourceControls from './../application/content/data/sourceControls';
import * as sourceControlsDetails from './../application/content/data/sourceControlsDetails';

export default function middleware(store: any) {
    return (next: any) => (action: any) => {
        switch (action.type) {
            case t.SEND:
                window._hub.server.send(action.payload);
                break;
            case t.RECEIVE:
                switch(action.payload.name)
                {
                    case c.SOURCE_CONTROLS_LIST:
                        store.dispatch(sourceControls.actions.list(action.payload.data));
                        break;
                    case c.SOURCE_CONTROLS_DETAILS:
                        store.dispatch(sourceControlsDetails.actions.list(action.payload.data));
                        break;
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