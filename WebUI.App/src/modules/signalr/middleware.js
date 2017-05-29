import * as t from './actionTypes';
import * as c from './commands';

import * as sourceControls from './../application/content/data/sourceControls';
import * as sourceControlsDetails from './../application/content/data/sourceControlsDetails';
import * as sourceControlVersions from './../application/content/data/sourceControlVersions';

export default function middleware(store: any) {
    let indexOf = -1;
    return (next: any) => (action: any) => {
        switch (action.type) {
            case t.SEND:
                window._hub.server.send(action.payload);
                break;
            case t.RECEIVE:
                switch(action.payload.name)
                {
                    case c.SOURCE_CONTROLS_LIST:
                        store.dispatch(sourceControls.actions.loaded(action.payload.data));
                        break;
                    case c.SOURCE_CONTROLS_DETAILS:
                        store.dispatch(sourceControlsDetails.actions.loaded(action.payload.data));
                        break;
                    case c.SOURCE_CONTROL_VERSIONS_LIST:
                        if(store.getState()[sourceControlVersions.constants.NAME].isLoading)
                        {
                            store.dispatch(sourceControlVersions.actions.loaded(action.payload.data));
                        }
                        
                        indexOf = store.getState()[sourceControls.constants.NAME].data.findIndex((sc) => sc.id == action.payload.data.id);
                        if(indexOf != -1)
                        {
                            if(store.getState()[sourceControls.constants.NAME].data[indexOf].isVersionsLoading)
                            {
                                store.dispatch(sourceControls.actions.loadedVersions(action.payload.data));
                            }
                        }
                        
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